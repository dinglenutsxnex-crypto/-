using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerEventDodgeCheck : TriggerEvent
	{
		public TriggerEventDodgeCheck(Mapping eventMap)
			: base(ETriggerEvents.EVENT_DODGE_CHECK, eventMap)
		{
		}
	}
}
