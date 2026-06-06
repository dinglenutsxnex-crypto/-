using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerActionUpdateTutorialPanel : TriggerActionTutorialPanel
	{
		public TriggerActionUpdateTutorialPanel(Node yamlNode)
			: base(EActionType.UPDATE_TUTORIAL_PANEL, yamlNode)
		{
		}

		protected override void ApplyAction(object modelData)
		{
			base.ApplyAction(modelData);
			TutorialManager.Instance.tutorialPanel.SetMessage(message, GetRPNArray());
		}
	}
}
