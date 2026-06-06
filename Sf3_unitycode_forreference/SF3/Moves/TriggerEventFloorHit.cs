using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerEventFloorHit : TriggerEvent
	{
		public TriggerEventFloorHit(Mapping eventMap)
			: base(ETriggerEvents.EVENT_FLOOR_HIT, eventMap)
		{
		}
	}
}
