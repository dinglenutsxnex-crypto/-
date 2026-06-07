/**
 * FightController.ts
 * Mirror of SF3/FightController.cs
 *
 * State machine:
 *   None → RoundStart → RoundFightStart ↔ (update loop) → RoundFightEnd
 *        → RoundEnd → (next round OR FightEnd)
 *
 * Key differences from the old stub:
 *   – Owns a RoundController (mirroring Unity's constructor).
 *   – update(dt) is called every frame by BattleController; checks
 *     RoundController.checkEndRound() and advances the stage.
 *   – Stage transitions drive FightHUD via onStageChange callbacks.
 *   – RoundEnd correctly tests win/lose counts before looping.
 */

import { ModelInfo }             from "./GameModels/ModelInfo";
import { RoundController }       from "./RoundController";
import { ERoundResult }          from "./ERoundResult";
import { FightControllerSettings } from "./FightControllerSettings";

export { ERoundResult };

export enum EFightStage {
  None           = 0,
  FightStart     = 1,
  FightEnd       = 2,
  RoundStart     = 3,
  RoundEnd       = 4,
  RoundFightStart = 5,
  RoundFightEnd  = 6,
}

export interface IRoundInfo {
  warrior: ModelInfo;
}

export interface IFightInfo {
  battleID:     string;
  fightID:      string;
  roundsToWin:  number;
  roundsToLose: number;
  roundTime?:   number;  // seconds; default 99
  hpRecovery?:  number;
  rounds?:      IRoundInfo[];
}

export type FightStageCallback = (stage: EFightStage) => void;

// ─── Callback shapes for HUD wiring ─────────────────────────────────────────

export interface IFightHUDCallbacks {
  /** Called at the start of every round with the new round number. */
  onRoundStart:  (roundNumber: number, roundsToWin: number, roundTime: number) => void;
  /** Called when FIGHT! banner should show; cb fires when banner finishes. */
  onFightStart:  (cb: () => void) => void;
  /** Called when a round ends — banner (KO/GREAT/PERFECT), then cb. */
  onRoundEnd:    (playerWon: boolean, isPerfect: boolean, isGreat: boolean, cb: () => void) => void;
  /** Called when the fight is fully over. */
  onFightEnd:    (playerWon: boolean) => void;
  /** Called every frame while fight is active — drives HP bars. */
  onHPUpdate:    (playerRatio: number, enemyRatio: number) => void;
  /** Called every frame while timer is running — drives timer display. */
  onTimerUpdate: (secondsLeft: number) => void;
}

export class FightController {
  private static _instance: FightController;
  static get instance(): FightController { return FightController._instance; }

  // ─── Sub-controllers ──────────────────────────────────────────────────────
  readonly roundController: RoundController;
  static settings: FightControllerSettings;

  // ─── State ────────────────────────────────────────────────────────────────
  private _stage: EFightStage = EFightStage.None;
  private _currentFight?: IFightInfo;
  private _onStageChange?: FightStageCallback;
  private _hudCallbacks?: IFightHUDCallbacks;
  private _advancing = false; // guard against re-entrant stage advances

  get fightStage():    EFightStage        { return this._stage; }
  get currentFight():  IFightInfo | undefined { return this._currentFight; }

  constructor() {
    FightController._instance  = this;
    this.roundController       = new RoundController();
    FightController.settings   = new FightControllerSettings();
  }

  // ─── Setup ───────────────────────────────────────────────────────────────

  setStageChangeCallback(cb: FightStageCallback): void {
    this._onStageChange = cb;
  }

  /** Wire up HUD so stage transitions automatically update the UI. */
  setHUDCallbacks(cbs: IFightHUDCallbacks): void {
    this._hudCallbacks = cbs;
  }

  initialize(): void {
    this._stage        = EFightStage.None;
    this._currentFight = undefined;
    this._advancing    = false;
    this.roundController.initialize();
    FightController.settings = new FightControllerSettings();
  }

  // ─── Entry point ─────────────────────────────────────────────────────────

  async initFight(fightInfo: IFightInfo): Promise<void> {
    this._currentFight = fightInfo;
    await this._setFightStage(EFightStage.RoundStart);
  }

  // ─── Per-frame update (called by BattleController) ───────────────────────

  update(dt: number): void {
    if (this._stage !== EFightStage.RoundFightStart) return;
    if (this._advancing) return;

    // Tick round (timer + dead-check)
    this.roundController.update(dt);

    // Notify HUD every frame
    this._hudCallbacks?.onHPUpdate(
      this.roundController.playerHP.currentHPRatio,
      this.roundController.enemyHP.currentHPRatio,
    );
    this._hudCallbacks?.onTimerUpdate(this.roundController.timeLeft);

    // Check round-end condition
    if (this.roundController.checkEndRound() !== ERoundResult.IN_PROGRESS) {
      this._advancing = true;
      this._setFightStage(EFightStage.RoundFightEnd).finally(() => {
        this._advancing = false;
      });
    }
  }

  // ─── External fight-end / surrender ─────────────────────────────────────

  winCurrentRound(playerWon: boolean): void {
    this.roundController.setRoundWinner(
      playerWon ? ERoundResult.PLAYER_WIN : ERoundResult.ENEMY_WIN,
    );
    this._setFightStage(EFightStage.RoundFightEnd);
  }

  setFightResult(winnerId: number, surrender: boolean): void {
    this._setFightStage(EFightStage.FightEnd);
  }

  static tacticsCanReact(): boolean {
    return FightController._instance._stage === EFightStage.RoundFightStart;
  }

  // ─── Stage machine ───────────────────────────────────────────────────────

  private async _setFightStage(stage: EFightStage): Promise<void> {
    this._stage = stage;
    this._onStageChange?.(stage);
    console.log(`[FightController] stage → ${EFightStage[stage]}`);

    switch (stage) {
      case EFightStage.RoundStart:     await this._roundStart();    break;
      case EFightStage.RoundFightStart:     this._roundFightStart(); break;
      case EFightStage.RoundFightEnd:       this._roundFightEnd();   break;
      case EFightStage.RoundEnd:            this._roundEnd();        break;
      case EFightStage.FightEnd:            this._fightEnd();        break;
    }
  }

  // ─── RoundStart ──────────────────────────────────────────────────────────

  private async _roundStart(): Promise<void> {
    const fi    = this._currentFight!;
    const time  = fi.roundTime ?? 99;

    // Reset round state
    this.roundController.clearRoundData(fi.roundsToWin, time);
    this.roundController.initNewRound();

    // Notify HUD — show "ROUND N" banner, wait for it to finish, then show "FIGHT!"
    await new Promise<void>(resolve => {
      const rn = this.roundController.currentRoundNumber;
      this._hudCallbacks?.onRoundStart(rn, fi.roundsToWin, time);

      if (this._hudCallbacks?.onFightStart) {
        // Give the banner a moment, then chain to FIGHT! then RoundFightStart
        setTimeout(() => {
          this._hudCallbacks!.onFightStart(() => {
            resolve();
          });
        }, 100); // slight delay so HUD can paint
      } else {
        resolve();
      }
    });

    await this._setFightStage(EFightStage.RoundFightStart);
  }

  // ─── RoundFightStart ─────────────────────────────────────────────────────

  private _roundFightStart(): void {
    // Arms the per-frame countdown in RoundController
    this.roundController.startFight();
    console.log("[FightController] RoundFightStart — fight active");
  }

  // ─── RoundFightEnd ───────────────────────────────────────────────────────

  private _roundFightEnd(): void {
    this.roundController.endRoundFight();

    const result      = this.roundController.roundResults;
    const playerWon   = result.winnerIsPlayer;
    const isPerfect   = result.isPerfect;
    const isGreat     = result.isGreat;

    // Show end-of-round banner then advance
    if (this._hudCallbacks?.onRoundEnd) {
      this._hudCallbacks.onRoundEnd(playerWon, isPerfect, isGreat, () => {
        this._setFightStage(EFightStage.RoundEnd);
      });
    } else {
      this._setFightStage(EFightStage.RoundEnd);
    }
  }

  // ─── RoundEnd ────────────────────────────────────────────────────────────

  private _roundEnd(): void {
    // Commit win counts
    this.roundController.recordRoundResult();

    const rc = this.roundController;
    const fi = this._currentFight!;

    const playerWon  = rc.playerWinCount >= fi.roundsToWin;
    const enemyWon   = rc.enemyWinCount  >= (fi.roundsToLose ?? fi.roundsToWin);

    if (playerWon || enemyWon) {
      this._setFightStage(EFightStage.FightEnd);
    } else {
      // Next round
      this._setFightStage(EFightStage.RoundStart);
    }
  }

  // ─── FightEnd ────────────────────────────────────────────────────────────

  private _fightEnd(): void {
    const playerWon = this.roundController.playerWinCount > this.roundController.enemyWinCount;
    console.log(
      `[FightController] FightEnd — ` +
      `P:${this.roundController.playerWinCount} ` +
      `E:${this.roundController.enemyWinCount} ` +
      `winner:${playerWon ? "PLAYER" : "ENEMY"}`
    );
    this._hudCallbacks?.onFightEnd(playerWon);
  }
}
