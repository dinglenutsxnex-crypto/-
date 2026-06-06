using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerActionWithoutTime : TriggerActionRoundResetable
	{
		public TriggerActionWithoutTime(Node yamlNode)
			: base(EActionType.WITHOUT_TIME, yamlNode)
		{
		}

		protected override void ApplyAction(object modelData)
		{
			base.ApplyAction(modelData);
			BattleInterface.Instance.BattleTimerEnable(false);
		}

		public override void Reset()
		{
			BattleInterface.Instance.BattleTimerEnable(true);
		}
	}
}
