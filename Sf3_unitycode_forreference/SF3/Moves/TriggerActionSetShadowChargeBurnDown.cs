using Nekki.Yaml;
using SF3.GameModels;

namespace SF3.Moves
{
	public class TriggerActionSetShadowChargeBurnDown : TriggerAction
	{
		private readonly bool _enable;

		public TriggerActionSetShadowChargeBurnDown(Node yamNode)
			: base(EActionType.SHADOW_CHARGE_BURN_DOWN, yamNode)
		{
			TryGetBool(out _enable, "Enable", true, string.Empty, null, false);
		}

		protected override void ApplyAction(object modelData)
		{
			ShadowFormController.Instance.EnableShadowChargeBurnDown(((Model)modelData).id, _enable);
		}
	}
}
