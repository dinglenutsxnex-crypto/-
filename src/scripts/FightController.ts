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
  isDojo?:      boolean;
  roundTime?:   number;
}

export type FightStageCallback = (stage: EFightStage) => void;

export class FightController {
  private static _instance: FightController;
  static get instance(): FightController { return FightController._instance; }

  private _stage: EFightStage = EFightStage.None;
  private _currentFight?: IFightInfo;
  private _onStageChange?: FightStageCallback;

  private _playerWins = 0;
  private _enemyWins  = 0;

  get fightStage():    EFightStage        { return this._stage; }
  get currentFight():  IFightInfo | undefined { return this._currentFight; }
  get playerWins():    number             { return this._playerWins; }
  get enemyWins():     number             { return this._enemyWins; }

  constructor() { FightController._instance = this; }

  setStageChangeCallback(cb: FightStageCallback): void {
    this._onStageChange = cb;
  }

  initialize(): void {
    this._stage        = EFightStage.None;
    this._currentFight = undefined;
    this._playerWins   = 0;
    this._enemyWins    = 0;
  }

  async initFight(fightInfo: IFightInfo): Promise<void> {
    this._currentFight = fightInfo;
    this._playerWins   = 0;
    this._enemyWins    = 0;
    await this._setFightStage(EFightStage.RoundStart);
  }

  winCurrentRound(playerWon: boolean): void {
    if (playerWon) {
      this._playerWins++;
    } else if (!this._currentFight?.isDojo) {
      this._enemyWins++;
    }
    this._setFightStage(EFightStage.RoundFightEnd);
  }

  setFightResult(winnerId: number, surrender: boolean): void {
    this._setFightStage(EFightStage.FightEnd);
  }

  static tacticsCanReact(): boolean {
    return FightController._instance._stage === EFightStage.RoundFightStart;
  }

  private async _setFightStage(
    stage:     EFightStage,
    surrender  = false,
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

  private async _roundStart(): Promise<void> {
    console.log(`[FightController] RoundStart (dojo=${this._currentFight?.isDojo})`);
    await this._setFightStage(EFightStage.RoundFightStart);
  }

  private _roundFightStart(): void {
    console.log(`[FightController] RoundFightStart`);
  }

  private _roundFightEnd(): void {
    console.log(`[FightController] RoundFightEnd`);
    this._setFightStage(EFightStage.RoundEnd);
  }

  private _roundEnd(): void {
    console.log(`[FightController] RoundEnd — playerWins=${this._playerWins} enemyWins=${this._enemyWins}`);
    const f = this._currentFight!;
    const fightOver = this._playerWins >= f.roundsToWin
      || (!f.isDojo && this._enemyWins >= f.roundsToLose);
    if (fightOver) {
      this._setFightStage(EFightStage.FightEnd);
    } else {
      this._setFightStage(EFightStage.RoundStart);
    }
  }

  private _fightEnd(surrender: boolean, winnerId?: number): void {
    console.log(
      `[FightController] FightEnd — winner: ${winnerId ?? "none"}, surrender: ${surrender}`,
    );
  }
}
