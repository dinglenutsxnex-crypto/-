using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerEventEveryFrame : TriggerEvent
	{
		public TriggerEventEveryFrame(Mapping eventMap)
			: base(ETriggerEvents.EVENT_EVERY_FRAME, eventMap)
		{
		}
	}
}
