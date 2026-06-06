using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerActionWithoutSF : TriggerActionRoundResetable
	{
		public TriggerActionWithoutSF(Node yamlNode)
			: base(EActionType.WITHOUT_SF, yamlNode)
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
			ShadowFormController.Instance.EnableUse(enable);
			BattleInterface.Instance.ShadowFormEnable(enable);
		}
	}
}
