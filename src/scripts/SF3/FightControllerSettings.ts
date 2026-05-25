// Ported from Assembly-CSharp/SF3/FightControllerSettings.cs

export class FightControllerSettings {
  static readonly SHOW_FIGHT_START_DEFAULT = true;
  static readonly IS_HP_FIGHT_DEFAULT      = true;
  static readonly IS_SCORE_FIGHT_DEFAULT   = false;
  static readonly SCORE_COUNT_DEFAULT      = 0;
  static readonly IS_TIMEOUT_WIN_DEFAULT   = false;

  showFightStart: boolean;
  isHpFight:      boolean;
  isScoreFight:   boolean;
  scoreCount:     number;
  isTimeoutWin:   boolean;

  constructor() {
    this.showFightStart = FightControllerSettings.SHOW_FIGHT_START_DEFAULT;
    this.isHpFight      = FightControllerSettings.IS_HP_FIGHT_DEFAULT;
    this.isScoreFight   = FightControllerSettings.IS_SCORE_FIGHT_DEFAULT;
    this.scoreCount     = FightControllerSettings.SCORE_COUNT_DEFAULT;
    this.isTimeoutWin   = FightControllerSettings.IS_TIMEOUT_WIN_DEFAULT;
  }
}
