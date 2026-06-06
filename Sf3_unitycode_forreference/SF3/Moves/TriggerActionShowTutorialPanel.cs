using Nekki.Yaml;
using UnityEngine;

namespace SF3.Moves
{
	public class TriggerActionShowTutorialPanel : TriggerActionTutorialPanel
	{
		private Vector2 offset;

		private string pannelType;

		private string animation;

		public TriggerActionShowTutorialPanel(Node yamlNode)
			: base(EActionType.SHOW_TUTORIAL_PANEL, yamlNode)
		{
			TryGetVector2(out offset, "Offset", string.Empty, null, false);
			TryGetString(out pannelType, "Type", string.Empty, string.Empty);
			TryGetString(out animation, "Animation", string.Empty, string.Empty, null, false);
		}

		protected override void ApplyAction(object modelData)
		{
			base.ApplyAction(modelData);
			TutorialManager.Instance.ShowPanel(new TutorialManager.OpenTutorialCallData(pannelType, message, GetRPNArray(), animation, offset));
		}
	}
}
