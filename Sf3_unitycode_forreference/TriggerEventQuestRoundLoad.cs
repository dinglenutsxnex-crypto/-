using Nekki.Yaml;
using SF3.Moves;

public class TriggerEventQuestRoundLoad : TriggerEvent
{
	public TriggerEventQuestRoundLoad(Mapping eventMap)
		: base(ETriggerEvents.QEVENT_ROUND_LOAD, eventMap)
	{
	}
}
