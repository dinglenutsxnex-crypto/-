using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerEventTryOn : TriggerEvent
	{
		public TriggerEventTryOn(Mapping eventMap)
			: base(ETriggerEvents.EVENT_TRY_ON, eventMap)
		{
		}
	}
}
