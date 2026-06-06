using Nekki.Yaml;
using SF3.Moves;

public class TriggerActionGoToModule : TriggerActionQuest
{
	public TriggerActionGoToModule(Node yamlNode)
		: base(EActionType.GO_TO_MODULE_TRIGGER, yamlNode, false)
	{
	}

	protected override void ApplyAction(object modelData)
	{
		base.ApplyAction(modelData);
		IntentParametrs intentParametrs = new IntentParametrs(base.BaseClass);
		IntentModule intentModule = BaseModuleController.CreateIntent(intentParametrs.ModuleType, intentParametrs);
		intentModule.TransitionType = intentParametrs.TransitionType;
		BaseModuleController.GoToModule(intentModule, base.CloseAction);
	}
}
