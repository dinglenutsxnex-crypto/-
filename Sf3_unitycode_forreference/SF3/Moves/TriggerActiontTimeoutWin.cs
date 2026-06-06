using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerActiontTimeoutWin : TriggerActionRoundResetable
	{
		public TriggerActiontTimeoutWin(Node yamlNode)
			: base(EActionType.TIMEOUT_WIN, yamlNode)
		{
		}

		protected override void ApplyAction(object modelData)
		{
			base.ApplyAction(modelData);
			FightController.Settings.IsTimeoutWin = true;
		}

		public override void Reset()
		{
			FightController.Settings.IsTimeoutWin = false;
		}
	}
}
