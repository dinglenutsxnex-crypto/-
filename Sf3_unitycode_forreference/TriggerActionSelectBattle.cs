using Nekki.Yaml;
using SF3;
using SF3.Moves;

public class TriggerActionSelectBattle : TriggerActionQuest
{
	private readonly bool _instantFocus;

	private readonly string _battleId;

	public TriggerActionSelectBattle(Node yamlNode)
		: base(EActionType.SELECT_BATTLE, yamlNode)
	{
		TryGetBool(out _instantFocus, "InstantFocus", false, string.Empty, null, false);
		TryGetString(out _battleId, "ID", string.Empty, string.Empty);
	}

	protected override void ApplyAction(object modelData)
	{
		base.ApplyAction(modelData);
		int battleID = int.Parse(_battleId.AsRpn());
		MapController.Instance.StartMapp(battleID, _instantFocus);
	}
}
