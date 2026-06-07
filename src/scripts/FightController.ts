/**
 * FightController.ts
 * Mirror of SF3/FightController.cs
 *
 * Drives the fight state machine:
 *   None → RoundStart → RoundFightStart ↔ RoundFightEnd → RoundEnd → (loop or FightEnd)
 *
 * Dojo path  (isDojo = true)  mirrors DojoRound() in Unity:
 *   RoundStart → init HUD round → immediately RoundFightStart (no banners, no anim wait)
 *
 * Normal path mirrors Round() in Unity:
 *   RoundStart → "ROUND X" banner → wait for both model intro anims → "FIGHT!" banner → RoundFightStart
 */

import { FightHUD } from "./ui/FightHUD";

export enum EFightStage {
  None           = 0,
  FightStart     = 1,
  FightEnd       = 2,
  RoundStart     = 3,
  RoundEnd       = 4,
  RoundFightStart = 5,
  RoundFightEnd  = 6,
}

export interface IFightInfo {
  battleID:     string;
  fightID:      string;
  roundsToWin:  number;
  roundsToLose: number;
  roundTime?:   number;
  isDojo?:      boolean;
}

export type FightStageCallback = (stage: EFightStage) => void;

export class FightController {
  private static _instance: FightController;
  static get instance(): FightController { return FightController._instance; }

  private _stage: EFightStage = EFightStage.None;
  private _currentFight?: IFightInfo;
  private _onStageChange?: FightStageCallback;
  private _hud?: FightHUD;

  private _currentRound  = 0;
  private _playerWins    = 0;
  private _enemyWins     = 0;

  get fightStage(): EFightStage { return this._stage; }
  get currentFight(): IFightInfo | undefined { return this._currentFight; }

  constructor() {
    FightController._instance = this;
  }

  setStageChangeCallback(cb: FightStageCallback): void {
    this._onStageChange = cb;
  }

  initialize(): void {
    this._stage        = EFightStage.None;
    this._currentFight = undefined;
    this._currentRound = 0;
    this._playerWins   = 0;
    this._enemyWins    = 0;
  }

  async initFight(fightInfo: IFightInfo, hud?: FightHUD): Promise<void> {
    this._currentFight = fightInfo;
    this._hud          = hud;
    this._currentRound = 0;
    this._playerWins   = 0;
    this._enemyWins    = 0;
    await this._setFightStage(EFightStage.RoundStart);
  }

  update(): void {
    // Per-frame checks (round timer, health thresholds) triggered externally.
  }

  winCurrentRound(playerWon: boolean): void {
    if (playerWon) {
      this._playerWins++;
      this._hud?.addPlayerWin();
    } else {
      this._enemyWins++;
      this._hud?.addEnemyWin();
    }
    this._setFightStage(EFightStage.RoundFightEnd);
  }

  setFightResult(winnerId: number, surrender: boolean): void {
    this._setFightStage(EFightStage.FightEnd, surrender, winnerId);
  }

  static tacticsCanReact(): boolean {
    return FightController._instance._stage === EFightStage.RoundFightStart;
  }

  private async _setFightStage(
    stage: EFightStage,
    surrender = false,
    winnerId?: number,
  ): Promise<void> {
    this._stage = stage;
    this._onStageChange?.(stage);

    switch (stage) {
      case EFightStage.RoundStart:
        await this._roundStart();
        break;
      case EFightStage.RoundFightStart:
        this._roundFightStart();
        break;
      case EFightStage.RoundFightEnd:
        this._roundFightEnd();
        break;
      case EFightStage.RoundEnd:
        this._roundEnd();
        break;
      case EFightStage.FightEnd:
        this._fightEnd(surrender, winnerId);
        break;
    }
  }

  // ─── Stage handlers ────────────────────────────────────────────────────────

  /**
   * Mirrors the DojoRound() / Round() split in FightController.cs.
   *
   * Dojo   (DojoRound):  no banners, no animation wait — straight to RoundFightStart.
   * Normal (Round):      show "ROUND X", wait for intro anim, show "FIGHT!".
   */
  private async _roundStart(): Promise<void> {
    this._currentRound++;
    const fi       = this._currentFight!;
    const roundTime = fi.roundTime ?? 99;

    console.log(`[FightController] RoundStart — round ${this._currentRound}`);

    this._hud?.initRound(this._currentRound, fi.roundsToWin, roundTime);

    if (fi.isDojo) {
      // ── Dojo path: mirrors DojoRound() ──────────────────────────────────
      // Unity: ClearRoundData → InitNewRound → InitBattleCamera(true) → HideLoader → RoundFightStart
      // No "ROUND X" banner, no intro-animation wait, no "FIGHT!" banner.
      // HUD is already initialised above; go straight to active fight.
      await this._setFightStage(EFightStage.RoundFightStart);
    } else {
      // ── Normal fight path: mirrors Round() ──────────────────────────────
      // Show "ROUND X" banner, then wait for both models' intro anim, then "FIGHT!"
      await new Promise<void>(resolve => {
        this._hud?.showRoundStart(resolve) ?? resolve();
      });
      await new Promise<void>(resolve => {
        this._hud?.showFightStart(resolve) ?? resolve();
      });
      await this._setFightStage(EFightStage.RoundFightStart);
    }
  }

  /** Mirrors SetRoundFightStart() — enables input, starts timer. */
  private _roundFightStart(): void {
    console.log(`[FightController] RoundFightStart — fight active`);
    this._hud?.show();
    this._hud?.startTimer();
  }

  /** Mirrors SetRoundFightEnd() — disables input, stops timer. */
  private _roundFightEnd(): void {
    console.log(`[FightController] RoundFightEnd`);
    this._hud?.stopTimer();

    // Brief pause before evaluating result — mirrors BattleInterface timing.
    setTimeout(() => {
      this._setFightStage(EFightStage.RoundEnd);
    }, 1000);
  }

  /** Mirrors SetRoundEnd() — checks if fight is over, loops or ends. */
  private _roundEnd(): void {
    console.log(`[FightController] RoundEnd`);
    const fi = this._currentFight!;

    if (
      this._playerWins >= fi.roundsToWin ||
      this._enemyWins  >= (fi.roundsToLose ?? fi.roundsToWin)
    ) {
      this._setFightStage(EFightStage.FightEnd);
    } else {
      this._setFightStage(EFightStage.RoundStart);
    }
  }

  private _fightEnd(surrender: boolean, winnerId?: number): void {
    console.log(
      `[FightController] FightEnd — winner: ${winnerId ?? "none"}, surrender: ${surrender}`,
    );
    this._hud?.stopTimer();
  }
}
