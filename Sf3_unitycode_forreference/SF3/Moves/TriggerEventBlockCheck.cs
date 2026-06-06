using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerEventBlockCheck : TriggerEvent
	{
		public TriggerEventBlockCheck(Mapping eventMap)
			: base(ETriggerEvents.EVENT_BLOCK_CHECK, eventMap)
		{
		}
	}
}
