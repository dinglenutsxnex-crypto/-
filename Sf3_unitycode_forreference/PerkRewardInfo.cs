using SF3.Items;
using sf3DTO;

public class PerkRewardInfo : BaseRewardInfo, IRarable
{
	private Rarity rarityType;

	public PerkRewardInfo(Rarity rarity)
	{
		rarityType = rarity;
	}

	public Rarity GetRarityType()
	{
		return rarityType;
	}

	public override string GetSpriteName()
	{
		return "drop_perk";
	}
}
