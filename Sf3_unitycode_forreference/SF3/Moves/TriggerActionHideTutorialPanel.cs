using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerActionHideTutorialPanel : TriggerAction
	{
		public TriggerActionHideTutorialPanel(Node yamlNode)
			: base(EActionType.HIDE_TUTORIAL_PANEL, yamlNode)
		{
		}

		protected override void ApplyAction(object modelData)
		{
			base.ApplyAction(modelData);
			TutorialManager.Instance.HidePanel();
		}
	}
}
