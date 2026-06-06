namespace SF3
{
	public class FightControllerSettings
	{
		public const bool ShowFightStartDefault = true;

		public const bool IsHpFightDefault = true;

		public const bool IsScoreFightDefault = false;

		public const int ScoreCountDefault = 0;

		public const bool IsTimeoutWinDefault = false;

		public bool ShowFightStart { get; set; }

		public bool IsHpFight { get; set; }

		public bool IsScoreFight { get; set; }

		public int ScoreCount { get; set; }

		public bool IsTimeoutWin { get; set; }

		public FightControllerSettings()
		{
			ShowFightStart = true;
			IsHpFight = true;
			IsScoreFight = false;
			ScoreCount = 0;
			IsTimeoutWin = false;
		}
	}
}
