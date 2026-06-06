using System.Collections.Generic;
using SF3.Items;
using SF3.UserData;

public class RewardItemProvider
{
	public class StackedItem
	{
		public BaseItem itemStacked { get; protected set; }

		public int levelups { get; protected set; }

		public float levelUpBar { get; protected set; }

		public StackedItem(BaseItem itemsStacked, int levelups, float levelUpBar)
		{
			itemStacked = itemsStacked;
			this.levelups = levelups;
			this.levelUpBar = levelUpBar;
		}
	}

	private int stackedIdx;

	public BaseItem originItem { get; protected set; }

	public BaseItem itemInInventory { get; protected set; }

	public List<StackedItem> itemsStacked { get; protected set; }

	public RewardItemProvider(BaseItem item)
	{
		originItem = item;
		itemsStacked = new List<StackedItem>();
		if (originItem is Perk)
		{
			itemInInventory = UserManager.UserModelInfo.GetPerkByID(originItem.id);
		}
		else
		{
			itemInInventory = UserManager.UserModelInfo.GetItemByID(originItem.id);
		}
		if (itemInInventory != null)
		{
			itemInInventory = itemInInventory.Clone() as BaseItem;
			AddStackedItem(originItem);
		}
		stackedIdx = 0;
	}

	private StackedItem GetStackedItem(BaseItem item)
	{
		IStackable stackable = item as IStackable;
		BaseItem baseItem = ((itemInInventory == null) ? originItem : itemInInventory);
		BaseItem baseItem2 = ((itemsStacked.Count != 0) ? (itemsStacked[itemsStacked.Count - 1].itemStacked.Clone() as BaseItem) : (baseItem.Clone() as BaseItem));
		IStackable stackable2 = baseItem2 as IStackable;
		IRarable rarable = baseItem2 as IRarable;
		if (stackable == null || stackable2 == null || rarable == null)
		{
			return null;
		}
		int levelUpCount = stackable2.GetLevelUpCount(stackable);
		stackable2.MergeSimilarItems(stackable);
		float barValue = stackable2.GetBarValue();
		return new StackedItem(baseItem2, levelUpCount, barValue);
	}

	public void AddStackedItem(BaseItem item)
	{
		StackedItem stackedItem = GetStackedItem(item);
		if (stackedItem != null)
		{
			itemsStacked.Add(stackedItem);
		}
	}

	private StackedItem GetStackedItemByStackId(int stackedId)
	{
		if (stackedId < 0 || itemsStacked.Count <= stackedId)
		{
			return null;
		}
		return itemsStacked[stackedId];
	}

	public StackedItem GetCurrentStackedItem()
	{
		return GetStackedItemByStackId(stackedIdx);
	}

	public StackedItem GetNextStackedItem()
	{
		return GetStackedItemByStackId(stackedIdx + 1);
	}

	public StackedItem GetPrvStackedItem()
	{
		return GetStackedItemByStackId(stackedIdx - 1);
	}

	public StackedItem SwitchToNextStackItem()
	{
		stackedIdx++;
		return GetStackedItemByStackId(stackedIdx);
	}

	public StackedItem GetLastStackedItem()
	{
		if (itemsStacked.Count == 0)
		{
			return null;
		}
		return itemsStacked[itemsStacked.Count - 1];
	}

	public static List<RewardItemProvider> CreateFromList(List<BaseItem> items)
	{
		Dictionary<int, RewardItemProvider> dictionary = new Dictionary<int, RewardItemProvider>();
		foreach (BaseItem item in items)
		{
			if (!dictionary.ContainsKey(item.id))
			{
				dictionary.Add(item.id, new RewardItemProvider(item));
			}
			else
			{
				dictionary[item.id].AddStackedItem(item);
			}
		}
		List<RewardItemProvider> list = new List<RewardItemProvider>();
		foreach (KeyValuePair<int, RewardItemProvider> item2 in dictionary)
		{
			list.Add(item2.Value);
		}
		return list;
	}
}
