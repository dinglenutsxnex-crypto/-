using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerEventGamePause : TriggerEvent
	{
		public TriggerEventGamePause(Mapping eventMap)
			: base(ETriggerEvents.EVENT_GAME_PAUSE, eventMap)
		{
		}
	}
}
