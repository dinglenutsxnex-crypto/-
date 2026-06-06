using Nekki.Yaml;
using SF3;
using SF3.Moves;

public class TriggerEventModulePreEnter : TriggerEvent
{
	public TriggerEventModulePreEnter(Mapping eventMap)
		: base(ETriggerEvents.EVENT_MODULE_PRE_ENTER, eventMap)
	{
	}

	public override bool Equal(BattleEventArgs args)
	{
		if (!base.Equal(args))
		{
			return false;
		}
		IntentModule intentModule = (IntentModule)args.EventData;
		string text = intentModule.TypeModule.ToString();
		return name.ToUpper().Equals(text.ToUpper());
	}
}
