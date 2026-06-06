using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerEventWallHit : TriggerEvent
	{
		public TriggerEventWallHit(Mapping eventMap)
			: base(ETriggerEvents.EVENT_WALL_HIT, eventMap)
		{
		}
	}
}
