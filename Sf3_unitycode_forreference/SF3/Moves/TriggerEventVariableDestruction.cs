using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerEventVariableDestruction : TriggerEvent
	{
		public TriggerEventVariableDestruction(Mapping eventMap)
			: base(ETriggerEvents.EVENT_VARIABLE_DESTRUCTION, eventMap)
		{
		}

		public override bool Equal(BattleEventArgs args)
		{
			if (!base.Equal(args))
			{
				return false;
			}
			return name.Length == 0 || name.Equals(args.EventData.ToString());
		}
	}
}
