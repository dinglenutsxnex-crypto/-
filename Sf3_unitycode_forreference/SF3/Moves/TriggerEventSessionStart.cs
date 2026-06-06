using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerEventSessionStart : TriggerEvent
	{
		public TriggerEventSessionStart(Mapping eventMap)
			: base(ETriggerEvents.QEVENT_SESSION_START, eventMap)
		{
		}
	}
}
