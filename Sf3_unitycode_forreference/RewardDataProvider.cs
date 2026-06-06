using System.Collections.Generic;
using SF3.Items;

public class RewardDataProvider
{
	public List<RewardItemProvider> rewardItemProvider { get; protected set; }

	public EquipmentSaver equipedItems { get; protected set; }

	public int startLevel { get; protected set; }

	public long startExp { get; protected set; }

	public RewardDataProvider(List<BaseItem> rewardItem, int level, long exp)
	{
		rewardItemProvider = RewardItemProvider.CreateFromList(rewardItem);
		equipedItems = new EquipmentSaver();
		startLevel = level;
		startExp = exp;
		equipedItems.SaveEquipedItems();
	}
}
