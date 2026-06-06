using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerEventTimeOut : TriggerEvent
	{
		public TriggerEventTimeOut(Mapping eventMap)
			: base(ETriggerEvents.EVENT_TIME_OUT, eventMap)
		{
		}
	}
}
