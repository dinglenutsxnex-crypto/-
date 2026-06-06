using Nekki.Yaml;
using SF3.Moves;
using SF3.UserData;

public class TriggerActionTutorialComplete : TriggerActionQuest
{
	public TriggerActionTutorialComplete(Node node)
		: base(EActionType.TUTORIAL_COMPLETE, node)
	{
	}

	protected override void ApplyAction(object modelData)
	{
		base.ApplyAction(modelData);
		UserManager.TutorialComplete();
	}
}
