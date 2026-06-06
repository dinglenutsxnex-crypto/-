using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerEventSlowdown : TriggerEvent
	{
		public TriggerEventSlowdown(Mapping eventMap)
			: base(ETriggerEvents.EVENT_SLOWDOWN, eventMap)
		{
		}
	}
}
