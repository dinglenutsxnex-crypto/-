using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SF3.Items;
using UnityEngine;

public class SF3UiLogger
{
	public enum SubType
	{
		ButtonClick = 0,
		AbilitesClick = 1,
		EquipClick = 2,
		UnequipClick = 3,
		ImbuePerk = 4,
		RemovePerk = 5,
		InventorySlotChanged = 6,
		ShopSlotChanged = 7,
		ShopSelectItem = 8,
		MapSelectBattle = 9,
		DialogClose = 10,
		MenuOpen = 11
	}

	private static SF3UiLogger _logger;

	private readonly Dictionary<SubType, string> _subTypeVsName = new Dictionary<SubType, string>
	{
		{
			SubType.ButtonClick,
			"button_click"
		},
		{
			SubType.AbilitesClick,
			"abilites_click"
		},
		{
			SubType.EquipClick,
			"equip_click"
		},
		{
			SubType.UnequipClick,
			"unequip_click"
		},
		{
			SubType.ImbuePerk,
			"imbue_perk_click"
		},
		{
			SubType.RemovePerk,
			"remove_perk_click"
		},
		{
			SubType.InventorySlotChanged,
			"inventory_slot_changed"
		},
		{
			SubType.ShopSlotChanged,
			"shop_slot_changed"
		},
		{
			SubType.ShopSelectItem,
			"shop_select_item"
		},
		{
			SubType.MapSelectBattle,
			"map_select_battle"
		},
		{
			SubType.DialogClose,
			"dialog_close"
		},
		{
			SubType.MenuOpen,
			"menu_open"
		}
	};

	private readonly UserBadgesManager.BadgeTypes[] _logBadgeTypes = new UserBadgesManager.BadgeTypes[4]
	{
		UserBadgesManager.BadgeTypes.Boosters,
		UserBadgesManager.BadgeTypes.Inventory,
		UserBadgesManager.BadgeTypes.Map,
		UserBadgesManager.BadgeTypes.Shop
	};

	public static SF3UiLogger instance
	{
		get
		{
			if (_logger == null)
			{
				_logger = new SF3UiLogger();
			}
			return _logger;
		}
	}

	public void AddEvent(SubType type)
	{
		AddEvent(type, new JObject());
	}

	public void AddEvent(SubType type, JObject json)
	{
		if (!_subTypeVsName.ContainsKey(type))
		{
			Debug.LogError("Unknow type " + type);
			return;
		}
		json["subtype"] = _subTypeVsName[type];
		Analytics.Logger.AddEvent("UI_EVENT", json);
	}

	public void AddButtonClickEvent(ConstantsSF3.ELocationSceneModule screen, string buttonName)
	{
		JObject jObject = new JObject();
		jObject["buttonName"] = buttonName;
		jObject["screen"] = screen.ToString();
		AddEvent(SubType.ButtonClick, jObject);
	}

	public void AddImbueEvent(BaseItem item, BaseItem oldPerk, BaseItem newPerk)
	{
		JObject jObject = new JObject();
		jObject["itemId"] = item.id;
		jObject["oldPerk"] = ((oldPerk == null) ? (-1) : oldPerk.id);
		jObject["newPerk"] = newPerk.id;
		AddEvent(SubType.ImbuePerk, jObject);
	}

	public void AddRemovePerkEvent(BaseItem item, BaseItem perk)
	{
		JObject jObject = new JObject();
		jObject["itemId"] = item.id;
		jObject["perkId"] = perk.id;
		AddEvent(SubType.RemovePerk, jObject);
	}

	public void AddDialogCloseEvent(string alias, string button, int time)
	{
		JObject jObject = new JObject();
		jObject["alias"] = alias;
		jObject["time"] = time;
		jObject["button"] = button;
		AddEvent(SubType.DialogClose, jObject);
	}

	public void AddAbilitesClickEvent(BaseItem item, long badgeCount)
	{
		JObject jObject = new JObject();
		jObject["itemId"] = item.id;
		jObject["badge"] = badgeCount;
		AddEvent(SubType.AbilitesClick, jObject);
	}

	public void AddInventoryChangeSlotEvent(string srcSlot, string dstSlot, string mode, long count)
	{
		JObject jObject = new JObject();
		jObject["srcSlot"] = srcSlot;
		jObject["dstSlot"] = dstSlot;
		jObject["badge"] = count;
		jObject["mode"] = mode;
		AddEvent(SubType.InventorySlotChanged, jObject);
	}

	public void AddMapSelectBattleEvent(string dstId, string srcId, bool dialog)
	{
		JObject jObject = new JObject();
		jObject["dstID"] = dstId;
		jObject["srcID"] = srcId;
		jObject["dialog"] = dialog;
		AddEvent(SubType.MapSelectBattle, jObject);
	}

	public void AddShopSelectItemEvent(BaseItem selectItem)
	{
		IRarable rarable = selectItem as IRarable;
		JObject jObject = new JObject();
		jObject["id"] = selectItem.id;
		jObject["rarity"] = ((rarable == null) ? "none" : rarable.GetRarityType().ToString());
		jObject["itemType"] = ((!(selectItem is Booster)) ? "equipment" : "booster");
		AddEvent(SubType.ShopSelectItem, jObject);
	}

	public void AddShopSlotChangeEvent(string dstSlot, string srcSlot, long badge)
	{
		JObject jObject = new JObject();
		jObject["srcSlot"] = dstSlot;
		jObject["dstSlot"] = srcSlot;
		jObject["badge"] = badge;
		AddEvent(SubType.ShopSlotChanged, jObject);
	}

	public void AddMenuOpenEvent()
	{
		JObject jObject = new JObject();
		jObject["screen"] = BaseModuleController.CurrentType.ToString();
		UserBadgesManager.BadgeTypes[] logBadgeTypes = _logBadgeTypes;
		for (int i = 0; i < logBadgeTypes.Length; i++)
		{
			UserBadgesManager.BadgeTypes type = logBadgeTypes[i];
			jObject[type.ToString()] = UserBadgesManager.Instance.GetBadgeNum(type);
		}
		instance.AddEvent(SubType.MenuOpen, jObject);
	}

	public void AddEquipEvent(BaseItem item)
	{
		JObject jObject = new JObject();
		jObject["id"] = item.id;
		instance.AddEvent(SubType.EquipClick, jObject);
	}

	public void AddUnequipEvent(BaseItem item)
	{
		JObject jObject = new JObject();
		jObject["id"] = item.id;
		instance.AddEvent(SubType.UnequipClick, jObject);
	}
}
