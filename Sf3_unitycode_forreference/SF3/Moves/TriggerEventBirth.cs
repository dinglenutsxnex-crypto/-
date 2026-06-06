using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerEventBirth : TriggerEvent
	{
		public TriggerEventBirth(Mapping eventMap)
			: base(ETriggerEvents.EVENT_BIRTH, eventMap)
		{
		}
	}
}
