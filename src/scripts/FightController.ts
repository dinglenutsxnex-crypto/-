/**
 * FightController.ts
 * Mirror of SF3/FightController.cs
 *
 * Drives the fight state machine:
 *   None → RoundStart → RoundFightStart ↔ RoundFightEnd → RoundEnd → (loop or FightEnd)
 */

export enum EFightStage {
  None = 0,
  FightStart = 1,
  FightEnd = 2,
  RoundStart = 3,
  RoundEnd = 4,
  RoundFightStart = 5,
  RoundFightEnd = 6,
}

export interface IFightInfo {
  battleID: string;
  fightID: string;
  roundsToWin: number;
  roundsToLose: number;
}

export type FightStageCallback = (stage: EFightStage) => void;

export class FightController {
  private static _instance: FightController;
  static get instance(): FightController { return FightController._instance; }

  private _stage: EFightStage = EFightStage.None;
  private _currentFight?: IFightInfo;
  private _onStageChange?: FightStageCallback;

  get fightStage(): EFightStage { return this._stage; }
  get currentFight(): IFightInfo | undefined { return this._currentFight; }

  constructor() {
    FightController._instance = this;
  }

  setStageChangeCallback(cb: FightStageCallback): void {
    this._onStageChange = cb;
  }

  initialize(): void {
    this._stage = EFightStage.None;
    this._currentFight = undefined;
  }

  async initFight(fightInfo: IFightInfo): Promise<void> {
    this._currentFight = fightInfo;
    await this._setFightStage(EFightStage.RoundStart);
  }

  update(): void {
    // Per-frame checks (round timer, health thresholds) triggered externally.
  }

  winCurrentRound(playerWon: boolean): void {
    this._setFightStage(EFightStage.RoundFightEnd);
  }

  setFightResult(winnerId: number, surrender: boolean): void {
    this._setFightStage(EFightStage.FightEnd);
  }

  static tacticsCanReact(): boolean {
    return (
      FightController._instance._stage === EFightStage.RoundFightStart
    );
  }

  private async _setFightStage(
    stage: EFightStage,
    surrender = false,
    winnerId?: number
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

  private async _roundStart(): Promise<void> {
    // Notify listeners; models / UI setup happens in BattleController
    console.log(`[FightController] RoundStart`);
    await this._setFightStage(EFightStage.RoundFightStart);
  }

  private _roundFightStart(): void {
    console.log(`[FightController] RoundFightStart – fight active`);
  }

  private _roundFightEnd(): void {
    console.log(`[FightController] RoundFightEnd`);
    this._setFightStage(EFightStage.RoundEnd);
  }

  private _roundEnd(): void {
    console.log(`[FightController] RoundEnd`);
    // Determine whether fight is over; for now always start a new round.
    this._setFightStage(EFightStage.RoundStart);
  }

  private _fightEnd(surrender: boolean, winnerId?: number): void {
    console.log(
      `[FightController] FightEnd – winner: ${winnerId ?? "none"}, surrender: ${surrender}`
    );
  }
}
