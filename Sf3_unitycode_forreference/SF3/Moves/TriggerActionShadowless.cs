using Nekki.Yaml;
using SF3.GameModels;

namespace SF3.Moves
{
	public class TriggerActionShadowless : TriggerActionRoundResetable
	{
		public TriggerActionShadowless(Node yamlNode)
			: base(EActionType.SHADOWLESS, yamlNode)
		{
		}

		protected override void ApplyAction(object modelData)
		{
			int id = ((Model)modelData).id;
			BattleInterface.Instance.SetShadowPerkState(id, ShadowPerksState.NotActive);
			ShadowFormController.Instance.EnableUse(false, id);
		}

		public override void Reset()
		{
			ShadowFormController.Instance.EnableUse(true);
		}
	}
}
