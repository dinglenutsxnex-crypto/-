using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerActionWithoutHP : TriggerActionRoundResetable
	{
		public TriggerActionWithoutHP(Node yamlNode)
			: base(EActionType.WITHOUT_HP, yamlNode)
		{
		}

		protected override void ApplyAction(object modelData)
		{
			base.ApplyAction(modelData);
			Enable(false);
		}

		public override void Reset()
		{
			Enable(true);
		}

		private void Enable(bool enable)
		{
			BattleInterface.Instance.HPBarsEnable(enable);
			FightController.Settings.IsHpFight = enable;
		}
	}
}
