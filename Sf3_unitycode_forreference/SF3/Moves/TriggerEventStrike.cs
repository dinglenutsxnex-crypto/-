using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerEventStrike : TriggerEvent
	{
		public TriggerEventStrike(Mapping eventMap)
			: base(ETriggerEvents.EVENT_STRIKE, eventMap)
		{
		}
	}
}
