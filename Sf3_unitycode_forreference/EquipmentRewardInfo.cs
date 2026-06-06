using SF3.Items;
using sf3DTO;

public class EquipmentRewardInfo : BaseRewardInfo, IRarable
{
	private Rarity rarityType;

	public EquipmentType equipmentType { get; protected set; }

	public EquipmentRewardInfo(EquipmentType type, Rarity rarity)
	{
		equipmentType = type;
		rarityType = rarity;
	}

	public Rarity GetRarityType()
	{
		return rarityType;
	}

	public override string GetSpriteName()
	{
		return "drop_" + equipmentType.ToString().ToLower();
	}
}
