using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerEventItemFloorHit : TriggerEvent
	{
		public TriggerEventItemFloorHit(Mapping mapping)
			: base(ETriggerEvents.EVENT_ITEM_FLOOR_HIT, mapping)
		{
		}
	}
}
