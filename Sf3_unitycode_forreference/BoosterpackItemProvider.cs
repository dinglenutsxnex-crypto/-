using System.Collections.Generic;
using SF3.Items;
using SF3.UserData;

public class BoosterpackItemProvider
{
	public BaseItem originItem { get; protected set; }

	public BaseItem ownedItem { get; protected set; }

	public BaseItem itemStacked { get; protected set; }

	public int levelups { get; protected set; }

	public float levelUpBar { get; protected set; }

	public BoosterpackItemProvider(BaseItem item, BaseItem owned = null)
	{
		originItem = item;
		ownedItem = ((owned == null) ? GetOwnedItemFromInventory() : owned);
		if (ownedItem != null)
		{
			ownedItem = ownedItem.Clone() as BaseItem;
			DoStack();
		}
	}

	private BaseItem GetOwnedItemFromInventory()
	{
		if (originItem is Perk)
		{
			return UserManager.UserModelInfo.GetPerkByID(originItem.id);
		}
		return UserManager.UserModelInfo.GetItemByID(originItem.id);
	}

	private void DoStack()
	{
		itemStacked = ownedItem.Clone() as BaseItem;
		IStackable stackable = originItem as IStackable;
		IStackable stackable2 = itemStacked as IStackable;
		IRarable rarable = itemStacked as IRarable;
		if (stackable != null && stackable2 != null && rarable != null)
		{
			levelups = stackable2.GetLevelUpCount(stackable);
			stackable2.MergeSimilarItems(stackable);
			levelUpBar = stackable2.GetBarValue();
		}
	}

	public static List<BoosterpackItemProvider> CreateFromList(List<BaseItem> items)
	{
		List<BoosterpackItemProvider> list = new List<BoosterpackItemProvider>();
		items.Reverse();
		foreach (BaseItem item in items)
		{
			BoosterpackItemProvider boosterpackItemProvider = list.FindLast((BoosterpackItemProvider x) => x.originItem.id == item.id);
			if (boosterpackItemProvider != null)
			{
				BaseItem owned = ((boosterpackItemProvider.itemStacked != null) ? boosterpackItemProvider.itemStacked : boosterpackItemProvider.originItem);
				list.Add(new BoosterpackItemProvider(item, owned));
			}
			else
			{
				list.Add(new BoosterpackItemProvider(item));
			}
		}
		list.Reverse();
		return list;
	}
}
