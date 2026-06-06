using Nekki;
using Nekki.Yaml;
using SF3.GameModels;
using SF3.Items;
using SF3.Moves;

public class TriggerActionShowSfIcon : TriggerAction
{
	private readonly int _framesDuration;

	private readonly EquipmentType _equipmentType;

	public TriggerActionShowSfIcon(Node node)
		: base(EActionType.SHOW_SF_ICON, node)
	{
		string outResult;
		if (TryGetString(out outResult, "Equipment", string.Empty, string.Empty, this))
		{
			_equipmentType = EnumsCompliancer.GetEnumerator<EquipmentType>(outResult);
		}
		TryGetInt(out _framesDuration, "Frames", 60, string.Empty);
	}

	protected override void ApplyAction(object modelData)
	{
		BattleInterface.Instance.ShadowPerksCooldown(((Model)modelData).id, _equipmentType, _framesDuration);
	}
}
