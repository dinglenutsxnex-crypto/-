using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf;
using SF3.Items;
using SF3.UserData;
using UnityEngine;
using sf3DTO;

public class UserBadgesManager : MonoBehaviour, IUserDataManager
{
	public interface IBadgeItem
	{
		int GetBadgeID();
	}

	public enum BadgeTypes
	{
		Inventory = 0,
		Shop = 1,
		Map = 2,
		Boosters = 3,
		Perks = 4
	}

	private class BaseBadge
	{
		private List<long> _curentIds;

		private List<long> _newItems;

		private static Dictionary<BadgeTypes, BaseBadge> _badges = new Dictionary<BadgeTypes, BaseBadge>();

		public BadgeTypes Type { get; protected set; }

		public long NewElements
		{
			get
			{
				return _newItems.Count;
			}
		}

		protected BaseBadge(BadgeTypes type)
		{
			_curentIds = new List<long>();
			_newItems = new List<long>();
			Type = type;
		}

		protected BaseBadge(string source)
		{
			_curentIds = new List<long>();
			_newItems = new List<long>();
			string[] array = source.Split(new char[1] { '^' }, StringSplitOptions.RemoveEmptyEntries);
			Type = (BadgeTypes)Enum.Parse(typeof(BadgeTypes), array[0]);
			string[] array2 = array[1].Split(new char[1] { '@' }, StringSplitOptions.RemoveEmptyEntries);
			if (array2.Length > 0)
			{
				string[] array3 = array2[0].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				string[] array4 = array3;
				foreach (string s in array4)
				{
					_curentIds.Add(long.Parse(s));
				}
			}
			if (array2.Length > 1)
			{
				string[] array5 = array2[1].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				string[] array6 = array5;
				foreach (string s2 in array6)
				{
					_newItems.Add(long.Parse(s2));
				}
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Concat(Type, "^"));
			stringBuilder.Append(NewElements + ",");
			foreach (long curentId in _curentIds)
			{
				stringBuilder.Append(curentId + ",");
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			stringBuilder.Append("@");
			foreach (long newItem in _newItems)
			{
				stringBuilder.Append(newItem + ",");
			}
			return stringBuilder.ToString().TrimEnd(',');
		}

		private void Reset(List<long> shownIds)
		{
			if (shownIds == null)
			{
				_newItems.Clear();
				return;
			}
			foreach (long shownId in shownIds)
			{
				if (_newItems.Contains(shownId))
				{
					_newItems.Remove(shownId);
				}
			}
		}

		private void Remove(List<long> shownIds)
		{
			foreach (long shownId in shownIds)
			{
				if (_newItems.Contains(shownId))
				{
					_newItems.Remove(shownId);
				}
				if (_curentIds.Contains(shownId))
				{
					_curentIds.Remove(shownId);
				}
			}
		}

		private void Clear()
		{
			_newItems.Clear();
			_curentIds.Clear();
		}

		private void UpdateItems(List<long> items)
		{
			foreach (long item in items)
			{
				if (!_curentIds.Contains(item) && !_newItems.Contains(item))
				{
					_newItems.Add(item);
				}
			}
			_curentIds = items;
		}

		private static List<BaseBadge> LoadBadges()
		{
			List<BaseBadge> list = new List<BaseBadge>();
			string @string = PlayerPrefs.GetString("SavedBadgesData", string.Empty);
			if (!string.IsNullOrEmpty(@string))
			{
				string[] array = @string.Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries);
				string[] array2 = array;
				foreach (string source in array2)
				{
					list.Add(new BaseBadge(source));
				}
			}
			return list;
		}

		private static void Save(BadgeTypes sender)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<BadgeTypes, BaseBadge> badge in _badges)
			{
				stringBuilder.Append(string.Concat(badge.Value, "|"));
			}
			PlayerPrefs.SetString("SavedBadgesData", stringBuilder.ToString().TrimEnd('|'));
			Instance.RefreshIcons(sender);
			PlayerPrefs.Save();
		}

		public static void Init()
		{
			List<BaseBadge> list = LoadBadges();
			_badges.Clear();
			foreach (BaseBadge item in list)
			{
				_badges.Add(item.Type, item);
			}
		}

		public static long GetBadgeNum(BadgeTypes type)
		{
			if (_badges.ContainsKey(type))
			{
				return _badges[type].NewElements;
			}
			return 0L;
		}

		public static void Reset(BadgeTypes type, List<long> shownIds)
		{
			if (_badges.ContainsKey(type))
			{
				_badges[type].Reset(shownIds);
				Save(type);
			}
		}

		public static void UpdateItems(BadgeTypes type, List<long> items)
		{
			if (!_badges.ContainsKey(type))
			{
				_badges.Add(type, new BaseBadge(type));
			}
			_badges[type].UpdateItems(items);
			Save(type);
		}

		public static bool GetIsNew(BadgeTypes type, long id)
		{
			if (_badges.ContainsKey(type))
			{
				return _badges[type]._newItems.Contains(id);
			}
			return false;
		}

		public static void Clear(BadgeTypes type)
		{
			if (_badges.ContainsKey(type))
			{
				_badges[type].Clear();
			}
			Save(type);
		}

		public static void Remove(BadgeTypes type, List<long> id)
		{
			if (_badges.ContainsKey(type))
			{
				_badges[type].Remove(id);
				Save(type);
			}
		}
	}

	private bool _nextDataIsNotNew;

	private static UserBadgesManager _instance;

	private static List<BadgeUnit> _units = new List<BadgeUnit>();

	private static List<BattleBadgeUnit> _mapUnits = new List<BattleBadgeUnit>();

	public static UserBadgesManager Instance
	{
		get
		{
			return _instance;
		}
	}

	public static void Create()
	{
		if (_instance == null)
		{
			_instance = new GameObject("BadgesManager").AddComponent<UserBadgesManager>();
			BaseBadge.Init();
			_units.Clear();
			_mapUnits.Clear();
			StaticObjectsManager.AddObject(_instance.gameObject);
			UserDataController.AddUserDataManager(UserDataManagerType.Badges, _instance);
		}
	}

	public void Reset(BadgeTypes type, object data)
	{
		switch (type)
		{
		case BadgeTypes.Inventory:
		{
			if (data == null)
			{
				BaseBadge.Reset(type, null);
				break;
			}
			List<Equipment> equipmentOfType = UserManager.UserModelInfo.GetEquipmentOfType((EquipmentType)data, false);
			List<long> list4 = new List<long>();
			foreach (Equipment item in equipmentOfType)
			{
				if (!list4.Contains(item.id))
				{
					list4.Add(item.id);
				}
			}
			BaseBadge.Reset(type, list4);
			break;
		}
		case BadgeTypes.Perks:
		{
			List<ISlotItem> availablePerksForItem = UserManager.UserModelInfo.GetAvailablePerksForItem((Equipment)data);
			List<long> list3 = new List<long>();
			foreach (ISlotItem item2 in availablePerksForItem)
			{
				if (!list3.Contains(item2.GetId()))
				{
					list3.Add(item2.GetId());
				}
			}
			BaseBadge.Reset(type, list3);
			break;
		}
		case BadgeTypes.Shop:
		{
			List<BaseItem> list = (List<BaseItem>)data;
			if (list == null)
			{
				break;
			}
			List<long> list2 = new List<long>();
			foreach (BaseItem item3 in list)
			{
				if (!list2.Contains(item3.id))
				{
					list2.Add(item3.id);
				}
			}
			BaseBadge.Reset(type, list2);
			break;
		}
		case BadgeTypes.Map:
		{
			long num = (long)data;
			BaseBadge.Reset(BadgeTypes.Map, new List<long> { num });
			{
				foreach (BattleBadgeUnit mapUnit in _mapUnits)
				{
					if (mapUnit.Id == num)
					{
						mapUnit.Refresh(false);
					}
				}
				break;
			}
		}
		case BadgeTypes.Boosters:
		{
			SF3.Items.Booster booster = (SF3.Items.Booster)data;
			BaseBadge.Reset(type, new List<long> { booster.instance_id });
			break;
		}
		}
	}

	private void RefreshIcons(BadgeTypes type)
	{
		foreach (BadgeUnit unit in _units)
		{
			if (unit.Type == type)
			{
				unit.Refresh(BaseBadge.GetBadgeNum(type));
			}
		}
	}

	internal void RegisterUnit(BattleBadgeUnit badgeUnit)
	{
		_mapUnits.Add(badgeUnit);
		badgeUnit.Refresh(BaseBadge.GetIsNew(BadgeTypes.Map, badgeUnit.Id));
	}

	internal void UnregisterUnit(BattleBadgeUnit badgeUnit)
	{
		if (_mapUnits.Contains(badgeUnit))
		{
			_mapUnits.Remove(badgeUnit);
		}
	}

	public void RegisterUnit(BadgeUnit badgeUnit)
	{
		_units.Add(badgeUnit);
		badgeUnit.Refresh(BaseBadge.GetBadgeNum(badgeUnit.Type));
	}

	public void UnregisterUnit(BadgeUnit badgeUnit)
	{
		if (_units.Contains(badgeUnit))
		{
			_units.Remove(badgeUnit);
		}
	}

	public void Initialize()
	{
		UpdateUserData(null);
	}

	public void UpdateUserData(IMessage dataObject)
	{
		if (!UserManager.IsTutorialComplete())
		{
			return;
		}
		if (dataObject is Player || dataObject is Shop)
		{
			BaseBadge.UpdateItems(BadgeTypes.Shop, GetShop(dataObject));
		}
		if (dataObject is Player || dataObject is Inventory)
		{
			BaseBadge.UpdateItems(BadgeTypes.Inventory, GetInventory(dataObject));
			BaseBadge.UpdateItems(BadgeTypes.Perks, GetPerks(dataObject));
			if (_nextDataIsNotNew)
			{
				_nextDataIsNotNew = false;
				BaseBadge.Reset(BadgeTypes.Inventory, null);
				BaseBadge.Reset(BadgeTypes.Perks, null);
			}
		}
		if (dataObject is Player || dataObject is BattleData)
		{
			BaseBadge.UpdateItems(BadgeTypes.Map, GetMap(dataObject));
		}
		if (dataObject is Player || dataObject is sf3DTO.Booster)
		{
			BaseBadge.UpdateItems(BadgeTypes.Boosters, GetBoosters(dataObject));
		}
	}

	private List<long> GetShop(IMessage dataObject)
	{
		List<long> list = new List<long>();
		Shop shop = null;
		if (dataObject != null)
		{
			shop = ((!(dataObject is Player)) ? (dataObject as Shop) : ((Player)dataObject).Shop);
			foreach (sf3DTO.ShopItem item in shop.Items)
			{
				BaseItem itemByID = UserManager.UserModelInfo.GetItemByID(item.ModelId);
				if (!list.Contains(item.ModelId) && itemByID == null)
				{
					list.Add(item.ModelId);
				}
			}
		}
		return list;
	}

	private List<long> GetInventory(IMessage dataObject)
	{
		List<long> list = new List<long>();
		Inventory inventory = null;
		if (dataObject != null)
		{
			inventory = ((!(dataObject is Player)) ? (dataObject as Inventory) : ((Player)dataObject).Inventory);
			foreach (Item item in inventory.Items)
			{
				Equipment equipmentByID = UserManager.UserModelInfo.GetEquipmentByID(item.ModelId);
				if ((equipmentByID == null || !equipmentByID.IsHidden()) && !list.Contains(item.ModelId))
				{
					list.Add(item.ModelId);
				}
			}
		}
		return list;
	}

	private List<long> GetPerks(IMessage dataObject)
	{
		List<long> list = new List<long>();
		Inventory inventory = null;
		if (dataObject != null)
		{
			inventory = ((!(dataObject is Player)) ? (dataObject as Inventory) : ((Player)dataObject).Inventory);
			foreach (sf3DTO.Perk perk in inventory.Perks)
			{
				if (!list.Contains(perk.ModelId))
				{
					list.Add(perk.ModelId);
				}
			}
		}
		return list;
	}

	private List<long> GetMap(IMessage dataObject)
	{
		List<long> list = new List<long>();
		BattleData battleData = null;
		if (dataObject != null)
		{
			battleData = ((!(dataObject is Player)) ? (dataObject as BattleData) : ((Player)dataObject).BattleData);
			foreach (Battle battle in battleData.Battles)
			{
				foreach (GeneratedBattle battle2 in battle.Battles)
				{
					if (!list.Contains(battle2.ModelId))
					{
						list.Add(battle2.ModelId);
					}
				}
			}
		}
		return list;
	}

	private List<long> GetBoosters(IMessage dataObject)
	{
		List<long> list = new List<long>();
		if (dataObject != null)
		{
			if (dataObject is Player)
			{
				Inventory inventory = ((Player)dataObject).Inventory;
				foreach (sf3DTO.Booster booster2 in inventory.Boosters)
				{
					if (!list.Contains(booster2.InstanceId))
					{
						list.Add(booster2.InstanceId);
					}
				}
			}
			else
			{
				sf3DTO.Booster booster = dataObject as sf3DTO.Booster;
				if (booster != null && !list.Contains(booster.InstanceId))
				{
					list.Add(booster.InstanceId);
				}
			}
		}
		return list;
	}

	public void AddItem(BadgeTypes badgeType, IBadgeItem badgeItem)
	{
		if (badgeItem != null && UserManager.IsTutorialComplete())
		{
			BaseBadge.UpdateItems(badgeType, new List<long> { badgeItem.GetBadgeID() });
		}
	}

	public void RemoveItem(BaseItem result)
	{
		if (result is Equipment)
		{
			BaseBadge.Remove(BadgeTypes.Inventory, new List<long> { result.id });
		}
		else if (result is SF3.Items.Perk)
		{
			BaseBadge.Remove(BadgeTypes.Perks, new List<long> { result.id });
		}
		else if (result is SF3.Items.Booster)
		{
			BaseBadge.Remove(BadgeTypes.Boosters, new List<long> { ((SF3.Items.Booster)result).instance_id });
		}
	}

	public void Clear(BadgeTypes type)
	{
		BaseBadge.Clear(type);
	}

	public long WhichItemsisNew(BadgeTypes type, List<SF3.UserData.ShopItem> items)
	{
		int num = 0;
		foreach (SF3.UserData.ShopItem item in items)
		{
			if (BaseBadge.GetIsNew(type, item.item.id))
			{
				num++;
			}
		}
		return num;
	}

	public long WhichItemsisNew(BadgeTypes type, List<Equipment> items)
	{
		int num = 0;
		foreach (Equipment item in items)
		{
			if (BaseBadge.GetIsNew(type, item.id))
			{
				num++;
			}
		}
		return num;
	}

	public long GetNewPerksFor(BaseItem equipment)
	{
		List<ISlotItem> availablePerksForItem = UserManager.UserModelInfo.GetAvailablePerksForItem((Equipment)equipment);
		int num = 0;
		foreach (ISlotItem item in availablePerksForItem)
		{
			if (BaseBadge.GetIsNew(BadgeTypes.Perks, item.GetId()))
			{
				num++;
			}
		}
		return num;
	}

	public void TutorialComplete()
	{
		Reset(BadgeTypes.Inventory, null);
		_nextDataIsNotNew = true;
	}

	public long GetBadgeNum(BadgeTypes type)
	{
		return BaseBadge.GetBadgeNum(type);
	}
}
