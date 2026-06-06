using Nekki.Yaml;
using SF3.GameModels;

namespace SF3.Moves
{
	public class TriggerActionToggleShadowForm : TriggerAction
	{
		private readonly bool _isInfinity;

		private readonly string _effectName;

		private readonly bool _activate;

		public TriggerActionToggleShadowForm(Node yamlNode)
			: base(EActionType.TOGGLE_SHADOW_FORM, yamlNode)
		{
			TryGetBool(out _activate, "Value", false, string.Empty, this);
			TryGetBool(out _isInfinity, "Infinity", false, string.Empty, null, false);
			TryGetString(out _effectName, "EffectName", string.Empty, string.Empty, null, false);
		}

		protected override void ApplyAction(object modelData)
		{
			if (_activate)
			{
				ShadowFormController.Instance.ActivateShadowForm(((Model)modelData).id, _effectName, _isInfinity);
			}
			else
			{
				ShadowFormController.Instance.DisableShadowForm(((Model)modelData).id);
			}
		}
	}
}
