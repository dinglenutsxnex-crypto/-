/**
 * RoundController.ts
 * Mirror of SF3/RoundController.cs
 *
 * Owns per-round state:
 *   – round number, win counts
 *   – live HP tracking for both fighters
 *   – countdown timer
 *   – end-of-round check (dead / timeout)
 *
 * FightController calls:
 *   clearRoundData()  →  reset counters / HP / timer
 *   initNewRound()    →  apply max-HP from ModelInfo, arm timer
 *   startFight()      →  begin countdown
 *   update(dt)        →  tick timer, check end condition
 *   checkEndRound()   →  returns current ERoundResult
 *   endRoundFight()   →  stop timer, snapshot winner
 *   setRoundWinner()  →  called externally (e.g. instant-KO cheat)
 *
 * FightHUD is driven by FightController, not by RoundController directly.
 */

import { ERoundResult }   from "./ERoundResult";
import { RoundResults }   from "./RoundResults";
import { ModelHPStatus }  from "./GameModels/ModelHPStatus";
import { ModelsManager }  from "./ModelsManager";

const DEFAULT_ROUND_TIME = 99; // seconds, mirrors Unity default

export class RoundController {
  private static _instance: RoundController;
  static get instance(): RoundController { return RoundController._instance; }

  // ─── Persistent across rounds ───────────────────────────────────────────
  private _currentRoundNumber = 0;
  private _playerWinCount     = 0;
  private _enemyWinCount      = 0;
  private _roundsTotal        = 0;
  private _roundTimeTotal     = DEFAULT_ROUND_TIME;

  get currentRoundNumber(): number { return this._currentRoundNumber; }
  get playerWinCount():     number { return this._playerWinCount; }
  get enemyWinCount():      number { return this._enemyWinCount; }
  get roundsTotal():        number { return this._roundsTotal; }
  get roundTimeTotal():     number { return this._roundTimeTotal; }

  // ─── Live round state ───────────────────────────────────────────────────
  private _roundProcess   = false;
  private _timeLeft       = DEFAULT_ROUND_TIME; // seconds, float
  private _roundResults   = new RoundResults();

  readonly playerHP = new ModelHPStatus();
  readonly enemyHP  = new ModelHPStatus();

  get timeLeft():       number      { return this._timeLeft; }
  get roundResult():    ERoundResult { return this._roundResults.result; }
  get roundResults():   RoundResults { return this._roundResults; }

  constructor() {
    RoundController._instance = this;
    this.initialize();
  }

  // ─── Called once when FightController.initialize() runs ─────────────────

  initialize(): void {
    this._currentRoundNumber = 0;
    this._playerWinCount     = 0;
    this._enemyWinCount      = 0;
    this._roundsTotal        = 0;
    this._roundTimeTotal     = DEFAULT_ROUND_TIME;
    this._roundProcess       = false;
    this._timeLeft           = DEFAULT_ROUND_TIME;
    this._roundResults.reset();
    this.playerHP.reset();
    this.enemyHP.reset();
  }

  // ─── Per-round lifecycle (called by FightController) ────────────────────

  /**
   * Mirrors ClearRoundData() — bump round number, reset HP, timer, results.
   * Called at the top of every round before InitNewRound.
   */
  clearRoundData(roundsToWin: number, roundTime: number): void {
    this._currentRoundNumber++;
    this._roundsTotal    = roundsToWin;
    this._roundTimeTotal = roundTime;
    this._timeLeft       = roundTime;
    this._roundProcess   = false;
    this._roundResults.reset();
    console.log(`[RoundController] clearRoundData – round ${this._currentRoundNumber}`);
  }

  /**
   * Mirrors InitNewRound() — sync HP from live model data.
   * At this point ModelsManager has already placed models.
   */
  initNewRound(): void {
    const mm = ModelsManager.instance;
    const playerMaxHP = mm?.player?.info?.maxLife ?? 1;
    const enemyMaxHP  = mm?.enemy?.info?.maxLife  ?? 1;

    // Re-create HP trackers with correct max values
    this.playerHP["_maxHP"]     = playerMaxHP;
    this.playerHP["_currentHP"] = playerMaxHP;
    this.enemyHP["_maxHP"]      = enemyMaxHP;
    this.enemyHP["_currentHP"]  = enemyMaxHP;

    console.log(`[RoundController] initNewRound – playerMaxHP:${playerMaxHP} enemyMaxHP:${enemyMaxHP}`);
  }

  /** Mirrors StartFight() — arms the countdown, enables model reactions. */
  startFight(): void {
    this._roundProcess = true;
    console.log("[RoundController] startFight");
  }

  /** Called every engine frame by FightController when stage === RoundFightStart. */
  update(dt: number): void {
    if (!this._roundProcess) return;

    // Tick timer
    if (this._timeLeft > 0) {
      this._timeLeft = Math.max(0, this._timeLeft - dt);
    }

    // Check timeout
    if (this._timeLeft <= 0 && this._roundResults.result === ERoundResult.IN_PROGRESS) {
      this._handleTimeout();
    }

    // Check dead (driven externally via applyDamage → HP drops to 0)
    this._checkDead();
  }

  /** Returns IN_PROGRESS until the round has a definitive outcome. */
  checkEndRound(): ERoundResult {
    return this._roundResults.result;
  }

  /**
   * Mirrors EndRoundFight() — stop countdown, snapshot winner HP.
   * Called by FightController when transitioning to RoundFightEnd.
   */
  endRoundFight(): void {
    this._roundProcess = false;

    // Snapshot winner HP for perfect/great checks
    if (this._roundResults.winnerIsPlayer) {
      this._roundResults.winnerHP = this.playerHP.currentHPRatio;
    } else {
      this._roundResults.winnerHP = this.enemyHP.currentHPRatio;
    }

    console.log(
      `[RoundController] endRoundFight – result:${ERoundResult[this._roundResults.result]} ` +
      `playerHP:${this.playerHP.currentHPRatio.toFixed(2)} enemyHP:${this.enemyHP.currentHPRatio.toFixed(2)}`
    );
  }

  /** Hard-set round winner (used by FightController.winCurrentRound and cheat commands). */
  setRoundWinner(result: ERoundResult): void {
    if (this._roundResults.result !== ERoundResult.IN_PROGRESS) return; // already settled
    this._roundResults.result        = result;
    this._roundResults.winnerIsPlayer = result === ERoundResult.PLAYER_WIN;
  }

  /** Commit win counts after the round ends — called by FightController._roundEnd(). */
  recordRoundResult(): void {
    if (this._roundResults.result === ERoundResult.PLAYER_WIN) {
      this._playerWinCount++;
    } else {
      this._enemyWinCount++;
    }
    console.log(
      `[RoundController] recordRoundResult – P:${this._playerWinCount} E:${this._enemyWinCount}`
    );
  }

  // ─── HP convenience (called from attack / damage events) ─────────────────

  applyDamageToPlayer(amount: number): void {
    this.playerHP.applyDamage(amount);
  }

  applyDamageToEnemy(amount: number): void {
    this.enemyHP.applyDamage(amount);
  }

  // ─── Private ─────────────────────────────────────────────────────────────

  private _checkDead(): void {
    if (this._roundResults.result !== ERoundResult.IN_PROGRESS) return;

    if (this.playerHP.isDead && this.enemyHP.isDead) {
      // Simultaneous KO — enemy wins by convention (mirrors Unity)
      this.setRoundWinner(ERoundResult.ENEMY_WIN);
    } else if (this.playerHP.isDead) {
      this.setRoundWinner(ERoundResult.ENEMY_WIN);
    } else if (this.enemyHP.isDead) {
      this.setRoundWinner(ERoundResult.PLAYER_WIN);
    }
  }

  private _handleTimeout(): void {
    // Mirror Unity: if isTimeoutWin → player wins, else enemy wins, result tagged TIME_OUT
    this._roundResults.result        = ERoundResult.TIME_OUT;
    // In the dojo we keep it simple: higher HP wins, tie goes to enemy
    if (this.playerHP.currentHPRatio > this.enemyHP.currentHPRatio) {
      this._roundResults.winnerIsPlayer = true;
      this._roundResults.result         = ERoundResult.PLAYER_WIN;
    } else {
      this._roundResults.winnerIsPlayer = false;
      this._roundResults.result         = ERoundResult.ENEMY_WIN;
    }
    console.log(`[RoundController] timeout – winner: ${this._roundResults.winnerIsPlayer ? "PLAYER" : "ENEMY"}`);
  }
}
