using Nekki.Yaml;
using SF3.Moves;

public class TriggerEventQuestBattleSelectionEnd : TriggerEventQuestBattleSelectionStart
{
	public TriggerEventQuestBattleSelectionEnd(Mapping eventMap)
		: base(eventMap, ETriggerEvents.QEVENT_BATTLE_SELECTION_END)
	{
	}
}
