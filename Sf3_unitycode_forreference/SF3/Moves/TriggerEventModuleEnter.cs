using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerEventModuleEnter : TriggerEvent
	{
		public TriggerEventModuleEnter(Mapping eventMap)
			: base(ETriggerEvents.EVENT_MODULE_ENTER, eventMap)
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
}
