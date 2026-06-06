using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerActionScoreFight : TriggerActionRoundResetable
	{
		private readonly string _score;

		public TriggerActionScoreFight(Node yamlNode)
			: base(EActionType.SCORE_FIGHT, yamlNode)
		{
			TryGetString(out _score, "Score", string.Empty, string.Empty, null, false);
		}

		protected override void ApplyAction(object modelData)
		{
			base.ApplyAction(modelData);
			EnableHP(false);
			BattleInterface.Instance.ScoreBarsEnable(true);
			FightController.Settings.IsScoreFight = true;
			FightController.Settings.ScoreCount = new RpnValue<int>(_score);
		}

		public override void Reset()
		{
			EnableHP(true);
			BattleInterface.Instance.ScoreBarsEnable(false);
			FightController.Settings.IsScoreFight = false;
		}

		private void EnableHP(bool enable)
		{
			BattleInterface.Instance.HPBarsEnable(enable);
			FightController.Settings.IsHpFight = enable;
		}
	}
}
