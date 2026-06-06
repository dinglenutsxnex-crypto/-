using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Jint.Native;
using Nekki;
using Nekki.Yaml;
using SF3.Settings;
using SF3.UserData;
using SF3_Attributes;
using SimpleJSON;
using UnityEngine;
using sf3DTO;

namespace SF3.Items
{
	public class Equipment : BattleItem, IEquipment, ICloneable, ISlotable, IAttributable, IRarable, IFactionable, IInformable, IStackable
	{
		private const string ALIAS_KEY = "Alias";

		private const string EQUIPPED_KEY = "Equipped";

		private const string ID_KEY = "ID";

		private const string NEW_KEY = "New";

		private const string PERKS_KEY = "Perks";

		private const string SHADOW_MARK_KEY = "ShadowMark";

		private const string ENEMY_SHADOW_MARK_KEY = "EnemyShadowMark";

		private const string SLOT_INDEX_KEY = "SlotIndex";

		private const string STACK_LEVEL_KEY = "StackLevel";

		private Faction _faction;

		private Rarity _rarity;

		private EquipmentType _typeEquipment;

		private string _enemyShadowMark;

		private string _shadowMark;

		private bool _hidden;

		private bool _isDefault;

		private AttributesHolder _attributesHolder;

		private ItemSlot[] _slots;

		private bool _isNew;

		private bool _equipped;

		private double _stackLevel;

		public int level { get; protected set; }

		public string EnemyShadowMark
		{
			get
			{
				return _enemyShadowMark;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					_enemyShadowMark = value;
				}
				else
				{
					_enemyShadowMark = value;
				}
			}
		}

		public string ShadowMark
		{
			get
			{
				return _shadowMark;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					_shadowMark = value;
				}
				else
				{
					_shadowMark = value;
				}
			}
		}

		public double stackLevel
		{
			get
			{
				return _stackLevel;
			}
		}

		public int UsageQuantity { get; private set; }

		public float CooldownSeconds { get; private set; }

		public Equipment(KeyValuePair<string, JsValue> keyValuePair)
			: base(keyValuePair)
		{
			_isNew = false;
			_equipped = false;
			Dictionary<string, JsValue> dic = keyValuePair.Value.AsDictionary();
			JsValue value = keyValuePair.Value;
			level = int.Parse(GetNodeSafe(dic, "Level", "0"), CultureInfo.InvariantCulture);
			TryParseEnum(out _faction, GetNodeSafe(dic, "Faction", "NONE"), Faction.UnknownFaction);
			TryParseEnum(out _rarity, GetNodeSafe(dic, "Rarity", "COMMON"), Rarity.Common);
			TryParseEnum(out _typeEquipment, GetNodeSafe(dic, "ItemType", "NONE"), EquipmentType.None);
			_slots = new ItemSlot[JsFunction.calcItemPerkSlotsByModelId(base.id)];
			for (int i = 0; i < _slots.Length; i++)
			{
				_slots[i] = new ItemSlot(i);
			}
			ShadowMark = GetNodeSafe(value, "ShadowMark", string.Empty);
			EnemyShadowMark = GetNodeSafe(value, "EnemyShadowMark", string.Empty);
			UsageQuantity = int.Parse(GetNodeSafe(value, "UsageQuantity", "0"));
			CooldownSeconds = float.Parse(GetNodeSafe(value, "CooldownSeconds", "0"));
			_hidden = GetNodeSafe(dic, "Hidden", "0") == "1";
			_isDefault = GetNodeSafe(dic, "Default", "0") == "1";
			_attributesHolder = new AttributesHolder(_typeEquipment);
			SetStackLevel(0.0);
		}

		public static Equipment Create(int itemID)
		{
			return ItemsManager.GetEquipmentById(itemID);
		}

		public static Equipment Create(Mapping itemData)
		{
			Equipment equipment = Create(int.Parse(itemData.GetText("ID").text));
			if (equipment != null)
			{
				equipment.FillData(itemData);
			}
			return equipment;
		}

		public static Equipment Create(Item equipmentData)
		{
			Equipment equipment = Create(equipmentData.ModelId);
			equipment.SetEquipped(equipmentData.Equipped);
			equipment.SetStackLevel(equipmentData.StackLevel);
			equipment.SetPerks(equipmentData.Perks.ToList());
			return equipment;
		}

		private void SetPerks(List<sf3DTO.PerkSlot> perks)
		{
			ClearPerks();
			for (int i = 0; i < perks.Count; i++)
			{
				Perk perk = Perk.Create(perks[i].PerkModelId);
				_slots[perks[i].SlotIndex].SetPerk(perk);
			}
		}

		public static List<BaseItem> Create(List<Item> equipmentsData)
		{
			List<BaseItem> list = new List<BaseItem>();
			foreach (Item equipmentsDatum in equipmentsData)
			{
				list.Add(Create(equipmentsDatum));
			}
			return list;
		}

		public void ClearPerks()
		{
			for (int i = 0; i < _slots.Length; i++)
			{
				_slots[i].RemovePerk();
			}
		}

		private void FillData(Mapping itemData)
		{
			_isNew = GetNodeSafe(itemData, "New", "0").Equals("1");
			Sequence sequence = itemData.GetSequence("Perks");
			if (sequence != null)
			{
				foreach (Mapping item in sequence)
				{
					int num = int.Parse(item.GetText("SlotIndex").text);
					if (num < _slots.Length)
					{
						_slots[num].SetPerk(Perk.Create(item));
					}
				}
			}
			SetEquipped(GetNodeSafe(itemData, "Equipped", "0") == "1");
			ShadowMark = GetNodeSafe(itemData, "ShadowMark", string.Empty);
			EnemyShadowMark = GetNodeSafe(itemData, "EnemyShadowMark", string.Empty);
			SetStackLevel(double.Parse(GetNodeSafe(itemData, "StackLevel", "0"), CultureInfo.InvariantCulture));
		}

		public void FillData(JSONClass itemData)
		{
			_isNew = GetNodeSafe(itemData, "New", "0").Equals("1");
			if (itemData.ContainsKey("Perks"))
			{
				JSONArray asArray = itemData["Perks"].AsArray;
				foreach (JSONClass item in asArray)
				{
					int asInt = item["SlotIndex"].AsInt;
					if (asInt < _slots.Length)
					{
						_slots[asInt].SetPerk(Perk.Create(item));
					}
				}
			}
			SetEquipped(GetNodeSafe(itemData, "Equipped", "0") == "1");
			ShadowMark = GetNodeSafe(itemData, "ShadowMark", string.Empty);
			EnemyShadowMark = GetNodeSafe(itemData, "EnemyShadowMark", string.Empty);
			SetStackLevel(double.Parse(GetNodeSafe(itemData, "StackLevel", "0"), CultureInfo.InvariantCulture));
		}

		public double GetStackLevel()
		{
			return _stackLevel;
		}

		public void SetStackLevel(double value)
		{
			_stackLevel = value;
			UpdateAttributes();
		}

		public override List<Node> ToYaml()
		{
			List<Node> list = new List<Node>();
			list.Add(new Scalar("ID", base.id.ToString()));
			list.Add(new Scalar("StackLevel", stackLevel.ToString(CultureInfo.InvariantCulture)));
			List<Node> list2 = list;
			if (!string.IsNullOrEmpty(_shadowMark))
			{
				list2.Add(new Scalar("ShadowMark", ShadowMark));
			}
			if (!string.IsNullOrEmpty(_enemyShadowMark))
			{
				list2.Add(new Scalar("EnemyShadowMark", EnemyShadowMark));
			}
			if (_equipped)
			{
				list2.Add(new Scalar("Equipped", "1"));
			}
			if (_isNew)
			{
				list2.Add(new Scalar("New", "1"));
			}
			if (HasPerks())
			{
				List<Node> list3 = new List<Node>();
				for (int i = 0; i < _slots.Length; i++)
				{
					if (_slots[i].HasPerk())
					{
						Mapping mapping = new Mapping("Perks", _slots[i].ToYaml());
						mapping.Add(new Scalar("SlotIndex", _slots[i].slotIndex.ToString()));
						list3.Add(mapping);
					}
				}
				list2.Add(new Sequence("Perks", list3));
			}
			return list2;
		}

		public override JSONClass ToJSON()
		{
			JSONClass jSONClass = new JSONClass();
			jSONClass.Add("ID", new JSONData(base.id));
			jSONClass.Add("StackLevel", new JSONData(stackLevel));
			if (!string.IsNullOrEmpty(_shadowMark))
			{
				jSONClass.Add("ShadowMark", new JSONData(ShadowMark));
			}
			if (!string.IsNullOrEmpty(_enemyShadowMark))
			{
				jSONClass.Add("EnemyShadowMark", new JSONData(EnemyShadowMark));
			}
			if (_equipped)
			{
				jSONClass.Add("Equipped", new JSONData(1));
			}
			if (_isNew)
			{
				jSONClass.Add("New", new JSONData(1));
			}
			if (HasPerks())
			{
				JSONArray jSONArray = new JSONArray();
				for (int i = 0; i < _slots.Length; i++)
				{
					if (_slots[i].HasPerk())
					{
						JSONClass jSONClass2 = _slots[i].ToJSON();
						jSONClass2.Add("SlotIndex", new JSONData(_slots[i].slotIndex));
						jSONArray.Add(jSONClass2);
					}
				}
				jSONClass.Add("Perks", jSONArray);
			}
			return jSONClass;
		}

		public bool HasPerks()
		{
			return _slots.Any((ItemSlot pr) => pr.HasPerk());
		}

		public List<string> GetAllTags(bool isPlayer)
		{
			List<string> tags = GetTags();
			ItemSlot[] slots = _slots;
			foreach (ItemSlot itemSlot in slots)
			{
				if (itemSlot.HasPerk())
				{
					tags.AddRange(itemSlot.GetTags());
				}
			}
			if (isPlayer)
			{
				if (!ShadowMark.IsNullOrEmpty())
				{
					tags.Add(ShadowMark);
				}
			}
			else if (!EnemyShadowMark.IsNullOrEmpty())
			{
				tags.Add(EnemyShadowMark);
			}
			return tags.Distinct().ToList();
		}

		public void MergeSimilarItems(IStackable toMerge)
		{
			Equipment equipment = toMerge as Equipment;
			if (equipment == null)
			{
				Debug.LogError("Something wrong.. This is not instance of Equipment. " + toMerge.GetType());
				return;
			}
			double num = JsFunction.MergeStackLevels(stackLevel, equipment.stackLevel, (int)_rarity, UserManager.UserModelInfo.level);
			SetStackLevel(num);
		}

		public int GetLevelUpCount(IStackable newItem)
		{
			return JsFunction.GetLevelUpCount(stackLevel, newItem.GetStackLevel(), (int)_rarity);
		}

		public float GetBarValue()
		{
			return JsFunction.GetBar(stackLevel, (int)_rarity);
		}

		public float GetLevelUpBar(IStackable newItem)
		{
			return JsFunction.GetLevelUpBar(stackLevel, newItem.GetStackLevel(), (int)_rarity);
		}

		private void UpdateAttributes()
		{
			_attributesHolder.UpdateAttributesBattle(JsFunction.GetAttributesBattle(stackLevel, base.id));
			_attributesHolder.UpdateAttributesVisible(JsFunction.GetAttributesVisible(stackLevel, base.id));
		}

		public void ClearAttributes()
		{
			_attributesHolder.Clear();
		}

		public override string ToString()
		{
			StringWrapper stringWrapper = StringWrapper.Create();
			stringWrapper.Add(base.ToString());
			stringWrapper.Head("Equipment data");
			stringWrapper.Wrap("TypeEquipment", _typeEquipment.ToString());
			stringWrapper.Wrap("StackLevel", _stackLevel);
			stringWrapper.Wrap("faction", _faction.ToString());
			stringWrapper.Wrap("rarity", _rarity.ToString());
			stringWrapper.Wrap("hidden", _hidden);
			stringWrapper.Wrap("isDefault", _isDefault);
			stringWrapper.Wrap("slotQuantity", _slots.Length);
			stringWrapper.Wrap("ShadowMark", ShadowMark);
			stringWrapper.Wrap("EnemyShadowMark", EnemyShadowMark);
			ItemSlot[] slots = _slots;
			foreach (ItemSlot itemSlot in slots)
			{
				stringWrapper.Wrap("Perk(new)", ("id: " + itemSlot != null) ? (itemSlot.GetId() + ", (Alias: " + base.alias + ")") : "-");
			}
			foreach (KeyValuePair<AttributeType, float> attributesForDisplayDatum in GetAttributesForDisplayData())
			{
				stringWrapper.Wrap("Attributes-Display", string.Concat("Key: ", attributesForDisplayDatum.Key, ", Value: ", attributesForDisplayDatum.Value));
			}
			foreach (KeyValuePair<AttributeType, float> attributesForCombatDatum in GetAttributesForCombatData())
			{
				stringWrapper.Wrap("Attributes-Combat", string.Concat("Key: ", attributesForCombatDatum.Key, ", Value: ", attributesForCombatDatum.Value));
			}
			stringWrapper.Wrap("isNew", _isNew);
			stringWrapper.Wrap("UsageQuantity", UsageQuantity);
			stringWrapper.Wrap("CooldownSeconds", CooldownSeconds);
			return stringWrapper.ToString();
		}

		public Faction GetFactionType()
		{
			return _faction;
		}

		public Rarity GetRarityType()
		{
			return _rarity;
		}

		public int GetSlotQuantity()
		{
			return _slots.Length;
		}

		public bool HasSlots()
		{
			return _slots.Length > 0;
		}

		public List<ItemSlot> GetSlotItems()
		{
			return _slots.ToList();
		}

		public bool HasPerk(Perk perk)
		{
			return HasPerk(perk.id);
		}

		public bool HasPerk(int perkID)
		{
			return _slots.Any((ItemSlot slotItem) => slotItem.HasPerk(perkID));
		}

		public int PerkOfTypeCount(Perk perk)
		{
			List<ItemSlot> list = _slots.Where((ItemSlot slotItem) => slotItem.HasPerk() && slotItem.perk.GetPerkType() == perk.GetPerkType()).ToList();
			return list.Count;
		}

		public List<sf3DTO.PerkSlot> GetPerkSlots()
		{
			List<sf3DTO.PerkSlot> list = new List<sf3DTO.PerkSlot>();
			ItemSlot[] slots = _slots;
			foreach (ItemSlot itemSlot in slots)
			{
				if (itemSlot.HasPerk())
				{
					list.Add(new sf3DTO.PerkSlot
					{
						PerkModelId = itemSlot.GetId(),
						SlotIndex = itemSlot.slotIndex
					});
				}
			}
			return list;
		}

		public bool CanAddSlotItem(ISlotItem item)
		{
			return !HasPerk((Perk)item);
		}

		public bool AddSlotItem(ISlotItem slotItem, int slotIndex)
		{
			if (CanAddSlotItem(slotItem) && slotIndex < _slots.Length)
			{
				_slots[slotIndex].SetPerk((Perk)slotItem);
				return true;
			}
			return false;
		}

		public bool RemoveSlotItem(ISlotItem item, out int slotId)
		{
			ItemSlot itemSlot = _slots.SingleOrDefault((ItemSlot pr) => pr.HasPerk(item.GetId()));
			if (itemSlot != null)
			{
				itemSlot.RemovePerk();
				slotId = itemSlot.slotIndex;
				return true;
			}
			slotId = -1;
			return false;
		}

		public bool HasTag(string tagValue)
		{
			return _tags.Any((string tgv) => tgv.Equals(tagValue));
		}

		public static Equipment GetDefaultEquipment(EquipmentType type)
		{
			return FightSettings.defaultEquipments.FirstOrDefault((Equipment equipment) => equipment._typeEquipment == type);
		}

		public static Equipment GetDefaultEquipmentSafe(EquipmentType type)
		{
			Equipment defaultEquipment = GetDefaultEquipment(type);
			return (defaultEquipment != null) ? ((Equipment)defaultEquipment.Clone()) : null;
		}

		public override object Clone()
		{
			Equipment equipment = base.Clone() as Equipment;
			equipment._attributesHolder = (AttributesHolder)_attributesHolder.Clone();
			equipment._slots = _slots.Select((ItemSlot item) => item.Clone() as ItemSlot).ToArray();
			return equipment;
		}

		public Attributes GetAttributesForCombat()
		{
			return _attributesHolder.GetAttributes(AttributePurpose.Combat);
		}

		public SortedDictionary<AttributeType, float> GetAttributesForDisplayData()
		{
			return GetAttributesData(AttributePurpose.Display);
		}

		public SortedDictionary<AttributeType, float> GetAttributesForCombatData()
		{
			return GetAttributesData(AttributePurpose.Combat);
		}

		private SortedDictionary<AttributeType, float> GetAttributesData(AttributePurpose purpose)
		{
			return _attributesHolder.GetAttributesData(purpose);
		}

		private Dictionary<AttributePurpose, SortedDictionary<AttributeType, float>> GetAttributesDataAll()
		{
			Dictionary<AttributePurpose, SortedDictionary<AttributeType, float>> dictionary = new Dictionary<AttributePurpose, SortedDictionary<AttributeType, float>>();
			foreach (AttributePurpose enumerator2 in EnumsCompliancer.GetEnumerators<AttributePurpose>())
			{
				dictionary[enumerator2] = GetAttributesData(enumerator2);
			}
			return dictionary;
		}

		public float GetAttributesForCombatValue(AttributeType attributeKey)
		{
			return GetAttributeValue(AttributePurpose.Combat, attributeKey);
		}

		public float GetAttributesForDisplayValue(AttributeType attributeKey)
		{
			return GetAttributeValue(AttributePurpose.Display, attributeKey);
		}

		private float GetAttributeValue(AttributePurpose purpose, AttributeType key)
		{
			return _attributesHolder.GetAttributeValue(purpose, key);
		}

		public bool IsNew()
		{
			return _isNew;
		}

		public void SetNew(bool setNew)
		{
			_isNew = setNew;
		}

		public EquipmentType GetEquipmentType()
		{
			return _typeEquipment;
		}

		public void SetEquipmentType(EquipmentType type)
		{
			_typeEquipment = type;
		}

		public bool IsHidden()
		{
			return _hidden;
		}

		public bool IsDefault()
		{
			return _isDefault;
		}

		public bool IsEquipped()
		{
			return _equipped;
		}

		public void SetEquipped(bool equipped)
		{
			_equipped = equipped;
		}
	}
}
