using System.Collections.Generic;
using Nekki.Yaml;
using SF3.Moves;
using SF3.UserData;

public class TriggerActionQuestQueue : TriggerActionQuest
{
	private bool hasQuests;

	public TriggerActionQuestQueue(Node node)
		: base(EActionType.QUEST_QUEUE, node, false)
	{
	}

	protected override void ApplyAction(object modelData)
	{
		base.ApplyAction(modelData);
		List<string> questQueue = UserManager.GetQuestQueue();
		hasQuests = questQueue.Count > 0;
		if (hasQuests)
		{
			QuestController.Instance.ForciblySetQueue(questQueue);
			BaseModuleController.GoToDefault(OnOpenModule);
		}
		else
		{
			ModuleController.GoToDojo(OnOpenModule);
		}
	}

	private void OnOpenModule()
	{
		CloseAction();
	}
}
