using Nekki.Yaml;
using SF3;
using SF3.Moves;

public class TriggerActionShowSelectedBattleInfo : TriggerActionQuest
{
	private readonly bool _show;

	public TriggerActionShowSelectedBattleInfo(Node node)
		: base(EActionType.SHOW_SELECTED_BATTLE_INFO, node)
	{
		TryGetBool(out _show, "Show", true, string.Empty, null, false);
	}

	protected override void ApplyAction(object modelData)
	{
		MapController.Instance.ShowBattleInfo(_show);
	}
}
