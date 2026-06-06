using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Jint.Native;
using Jint.Native.Array;
using Nekki;
using Nekki.Yaml;
using SF3.Items;
using SF3.Settings;
using SF3.UserData;
using SimpleJSON;
using UnityEngine;
using sf3DTO;

namespace SF3.GameModels
{
	[Serializable]
	public class ModelInfo : ICloneable
	{
		public class Color
		{
			public int id { get; private set; }

			public double value { get; private set; }

			public Color(int colorID, double colorValue)
			{
				SetID(colorID);
				SetValue(colorValue);
			}

			public void SetID(int colorID)
			{
				id = colorID;
			}

			public void SetValue(double colorValue)
			{
				value = colorValue;
			}
		}

		public class EquippedResult
		{
			public int oldItemID { get; private set; }

			public Equipment oldItem { get; private set; }

			public int newItemID { get; private set; }

			public Equipment newItem { get; private set; }

			public EquippedResult(Equipment oldItemValue, Equipment newItemValue)
			{
				oldItem = oldItemValue;
				oldItemID = ((oldItem != null) ? oldItem.id : (-1));
				newItem = newItemValue;
				newItemID = newItem.id;
			}
		}

		protected Dictionary<EquipmentType, List<Equipment>> _equipmentAll;

		protected ItemHolder _itemHolder;

		protected Dictionary<int, List<SF3.Items.Booster>> _boostersAll;

		protected Dictionary<EquipmentType, Equipment> _equipmentEquipped;

		protected Dictionary<EquipmentType, Equipment> _equipmentDefault;

		protected Dictionary<int, IPerk> _perksEquipped;

		protected Dictionary<int, IPerk> _perksEquippedTemporary;

		private sf3DTO.Color _skinColor = new sf3DTO.Color();

		private sf3DTO.Color _hairColor = new sf3DTO.Color();

		private List<string> tags;

		private List<string> resultTags;

		private List<Rule> _rules = new List<Rule>();

		private Dictionary<EquipmentType, ModelObject> _dropped;

		private bool useEquipmentAttributes;

		public ModelAttributes attributes { get; private set; }

		public string skeleton { get; private set; }

		public string head { get; private set; }

		public sf3DTO.Color skinColor
		{
			get
			{
				return _skinColor;
			}
		}

		public sf3DTO.Color hairColor
		{
			get
			{
				return _hairColor;
			}
		}

		public bool isControl { get; private set; }

		public bool isPlayer { get; private set; }

		public Gender gender { get; private set; }

		public string alias { get; private set; }

		public AiMode aiMode { get; private set; }

		public double warriorPower { get; private set; }

		public float maxLife { get; private set; }

		public float currentLife { get; private set; }

		public int score { get; private set; }

		public List<Rule> rules
		{
			get
			{
				return _rules;
			}
		}

		public event Action onZeroHP;

		protected ModelInfo()
		{
			_equipmentAll = new Dictionary<EquipmentType, List<Equipment>>();
			foreach (EquipmentType enumerator2 in EnumsCompliancer.GetEnumerators<EquipmentType>())
			{
				_equipmentAll[enumerator2] = new List<Equipment>();
			}
			_boostersAll = new Dictionary<int, List<SF3.Items.Booster>>();
			_equipmentEquipped = new Dictionary<EquipmentType, Equipment>();
			_equipmentDefault = new Dictionary<EquipmentType, Equipment>();
			_perksEquipped = new Dictionary<int, IPerk>();
			_perksEquippedTemporary = new Dictionary<int, IPerk>();
			SetSkeleton(FightSettings.defaultSkeleton);
			SetAlias(string.Empty);
			isControl = false;
			isPlayer = false;
			SetAIMode(AiMode.RegularMode);
			attributes = new ModelAttributes();
			tags = new List<string>(FightSettings.defaultTags);
			resultTags = new List<string>();
			head = string.Empty;
			maxLife = 1f;
			currentLife = maxLife;
			score = 0;
			_itemHolder = new ItemHolder();
		}

		public ModelInfo(Mapping userData)
			: this()
		{
			Parse(userData);
			InitDependencies();
		}

		public ModelInfo(JSONNode userData)
			: this()
		{
			Parse(userData);
			InitDependencies();
		}

		public ModelInfo(Warrior userData)
			: this()
		{
			Parse(userData);
			InitDependencies();
		}

		public ModelInfo(Dictionary<string, JsValue> userData)
			: this()
		{
			Parse(userData);
			InitDependencies();
		}

		public ModelInfo(BrawlerEnemy userData)
			: this()
		{
			Parse(userData);
			InitDependencies();
		}

		public void SetAIMode(AiMode mode)
		{
			aiMode = mode;
		}

		private string GetHeadByID(int HeadId)
		{
			Dictionary<string, JsValue> dictionary = JS.Instance.GetScope("Heads").AsDictionary();
			foreach (KeyValuePair<string, JsValue> item in dictionary)
			{
				Dictionary<string, JsValue> dictionary2 = item.Value.AsObject().AsDictionary();
				if (dictionary2["ID"].AsInteger() == HeadId)
				{
					return dictionary2["Head"].AsString();
				}
			}
			return dictionary.First().Value.AsObject().AsDictionary()["Head"].AsString();
		}

		private void AddEquipmentToArray(Equipment equipment)
		{
			_equipmentAll[equipment.GetEquipmentType()].Add(equipment);
		}

		protected void AddBoosterToArray(SF3.Items.Booster booster)
		{
			if (!_boostersAll.ContainsKey(booster.id))
			{
				_boostersAll.Add(booster.id, new List<SF3.Items.Booster>());
			}
			_boostersAll[booster.id].Add(booster);
		}

		private void CreateDefaultEquipment()
		{
			_equipmentDefault = new Dictionary<EquipmentType, Equipment>();
			foreach (List<Equipment> value in _equipmentAll.Values)
			{
				foreach (Equipment item in value)
				{
					if (item.IsDefault() && !_equipmentDefault.ContainsKey(item.GetEquipmentType()))
					{
						_equipmentDefault.Add(item.GetEquipmentType(), item);
					}
				}
			}
			foreach (EquipmentType enumerator4 in EnumsCompliancer.GetEnumerators<EquipmentType>())
			{
				Equipment defaultEquipment = Equipment.GetDefaultEquipment(enumerator4);
				if (defaultEquipment != null && !_equipmentDefault.ContainsKey(defaultEquipment.GetEquipmentType()))
				{
					_equipmentDefault.Add(defaultEquipment.GetEquipmentType(), defaultEquipment);
				}
			}
			if (isPlayer && _equipmentDefault.ContainsKey(EquipmentType.Helmet))
			{
				_equipmentDefault[EquipmentType.Helmet].SetModel(UserManager.GetHair());
			}
		}

		private void SetTags(string[] tagsValue)
		{
			tags.Clear();
			tags.AddRange(tagsValue);
		}

		public void SetSkinColor(sf3DTO.Color color)
		{
			_skinColor = color;
		}

		public void SetHairColor(sf3DTO.Color color)
		{
			_hairColor = color;
		}

		public void SetHead(string headName)
		{
			head = headName;
		}

		private void SetSkeleton(string skeletonName)
		{
			skeleton = skeletonName;
		}

		public void PreInit()
		{
			resultTags.Clear();
			foreach (string tag in tags)
			{
				if (!resultTags.Contains(tag))
				{
					resultTags.Add(tag);
				}
			}
			CreateEquipped();
		}

		public void CreateEquipped()
		{
			_dropped = null;
			_equipmentEquipped.Clear();
			foreach (KeyValuePair<EquipmentType, List<Equipment>> item in _equipmentAll)
			{
				foreach (Equipment item2 in item.Value)
				{
					if (item2.IsEquipped())
					{
						_equipmentEquipped.Add(item.Key, item2);
						break;
					}
				}
				if (item.Key == EquipmentType.Weapon && !_equipmentEquipped.ContainsKey(EquipmentType.Weapon) && item.Value.Count > 0)
				{
					_equipmentEquipped.Add(item.Key, item.Value[0]);
					item.Value[0].SetEquipped(true);
				}
			}
		}

		private void CollectPerks()
		{
			_perksEquipped.Clear();
			CollectFromEquippedEquipment();
			CollectTemporaryEquipped();
		}

		private void CollectFromEquippedEquipment()
		{
			foreach (Equipment value in _equipmentEquipped.Values)
			{
				foreach (ItemSlot slotItem in value.GetSlotItems())
				{
					if (slotItem.HasPerk())
					{
						SF3.Items.Perk perk = slotItem.perk;
						if (perk != null)
						{
							_perksEquipped.Add(perk.GetId(), perk);
						}
					}
				}
			}
		}

		private void CollectTemporaryEquipped()
		{
			foreach (KeyValuePair<int, IPerk> item in _perksEquippedTemporary)
			{
				_perksEquipped.Add(item.Key, item.Value);
			}
		}

		public void SetAlias(string value)
		{
			alias = value;
		}

		public List<string> GetEquippedTags()
		{
			List<string> list = new List<string>();
			list.AddRange(resultTags.Distinct());
			List<Equipment> list2 = new List<Equipment>(_equipmentEquipped.Values);
			if (_dropped != null)
			{
				foreach (ModelObject value in _dropped.Values)
				{
					for (int i = 0; i < list2.Count; i++)
					{
						if (list2[i].id == value.equipment.id || !list2[i].IsEquipped())
						{
							list2.Remove(list2[i]);
							break;
						}
					}
				}
			}
			foreach (Equipment item in list2)
			{
				List<string> allTags = item.GetAllTags(isPlayer);
				foreach (string item2 in allTags)
				{
					if (!list.Contains(item2))
					{
						list.Add(item2);
					}
				}
			}
			foreach (KeyValuePair<int, IPerk> item3 in _perksEquipped)
			{
				List<string> list3 = item3.Value.GetTags();
				foreach (string item4 in list3)
				{
					if (!list.Contains(item4))
					{
						list.Add(item4);
					}
				}
			}
			return list;
		}

		public void Initialize(int modelID)
		{
			attributes.Initialize(modelID);
		}

		public void CalculateSummaryAttributes()
		{
			if (isPlayer || useEquipmentAttributes)
			{
				attributes.CalculateSummaryAttributes(_equipmentEquipped);
			}
			else
			{
				attributes.CalculateWarriorSummaryAttributes(_equipmentEquipped, warriorPower);
			}
		}

		public Equipment[] GetEquippedItems()
		{
			return _equipmentEquipped.Values.ToArray();
		}

		public Equipment GetEquipped(EquipmentType type, bool getVisibleOnly = false)
		{
			Equipment result = null;
			if (_equipmentEquipped.ContainsKey(type))
			{
				result = ((_equipmentEquipped[type].IsHidden() && getVisibleOnly) ? null : _equipmentEquipped[type]);
			}
			return result;
		}

		public Equipment GetEquipped(int itemID)
		{
			return _equipmentEquipped.Values.FirstOrDefault((Equipment equippedValue) => equippedValue.id.Equals(itemID));
		}

		public int GetEquippedIDForType(EquipmentType type)
		{
			return (!_equipmentEquipped.ContainsKey(type)) ? (-1) : _equipmentEquipped[type].id;
		}

		public Equipment GetDefaultEquipment(EquipmentType type)
		{
			return (!_equipmentDefault.ContainsKey(type)) ? null : _equipmentDefault[type];
		}

		public BaseItem GetItemByID(int id)
		{
			return _equipmentAll.Values.SelectMany((List<Equipment> itemsByTypeValue) => itemsByTypeValue).FirstOrDefault((Equipment item) => item.id.Equals(id));
		}

		public Equipment GetEquipmentByID(int id)
		{
			return (Equipment)GetItemByID(id);
		}

		public List<Equipment> GetEquipment()
		{
			List<Equipment> list = new List<Equipment>();
			foreach (List<Equipment> value in _equipmentAll.Values)
			{
				list.AddRange(value);
			}
			return list;
		}

		public IEnumerable<SF3.Items.Perk> GetPerks()
		{
			return _itemHolder._perksAll.GetPerkAll();
		}

		public Dictionary<int, List<SF3.Items.Booster>> GetBoostersAll()
		{
			return _boostersAll;
		}

		internal IEnumerable<sf3DTO.Booster> GetDTOBoosters()
		{
			List<sf3DTO.Booster> list = new List<sf3DTO.Booster>();
			foreach (List<SF3.Items.Booster> value in GetBoostersAll().Values)
			{
				foreach (SF3.Items.Booster item in value)
				{
					Loot loot = new Loot();
					foreach (Equipment equipment in item.equipments)
					{
						loot.Equipments.Add(equipment.AsDto());
					}
					foreach (SF3.Items.Perk perk in item.perks)
					{
						loot.Perks.Add(perk.AsDto());
					}
					loot.Experience = item.experience;
					loot.Currencies.AddRange(item.currencies);
					list.Add(new sf3DTO.Booster
					{
						InstanceId = item.instance_id,
						ModelId = item.id,
						Loot = loot
					});
				}
			}
			return list;
		}

		public SF3.Items.Booster GetBoosterToSpend(int modelID)
		{
			if (_boostersAll.ContainsKey(modelID) && _boostersAll[modelID].Count > 0)
			{
				return _boostersAll[modelID][0];
			}
			return null;
		}

		public void SpendBooster(int modelID)
		{
			if (_boostersAll.ContainsKey(modelID) && _boostersAll[modelID].Count > 0)
			{
				_boostersAll[modelID].Remove(_boostersAll[modelID][0]);
			}
		}

		public void SpendBooster(SF3.Items.Booster booster)
		{
			if (_boostersAll.ContainsKey(booster.id) && _boostersAll[booster.id].Contains(booster))
			{
				_boostersAll[booster.id].Remove(booster);
			}
		}

		public virtual EquippedResult EquipItem(int itemID)
		{
			Equipment equipment = GetItemByID(itemID) as Equipment;
			Equipment equipment2 = null;
			EquippedResult result = null;
			if (equipment != null)
			{
				if (equipment.level > UserManager.GetLevel())
				{
					return null;
				}
				if (!_equipmentEquipped.ContainsKey(equipment.GetEquipmentType()))
				{
					_equipmentEquipped.Add(equipment.GetEquipmentType(), equipment);
				}
				else
				{
					equipment2 = _equipmentEquipped[equipment.GetEquipmentType()];
					equipment2.SetEquipped(false);
				}
				_equipmentEquipped[equipment.GetEquipmentType()] = equipment;
				equipment.SetEquipped(true);
				CollectAndCalculate();
				result = new EquippedResult(equipment2, equipment);
			}
			return result;
		}

		public virtual Equipment UnEquipItem(int unequipID)
		{
			Equipment equipped = GetEquipped(unequipID);
			if (equipped != null)
			{
				equipped.SetEquipped(false);
				_equipmentEquipped.Remove(equipped.GetEquipmentType());
				CollectAndCalculate();
			}
			return equipped;
		}

		public void AddTag(string tag)
		{
			if (!ContainsTag(tag))
			{
				resultTags.Add(tag);
			}
		}

		public void EquipItemNotExisted(Equipment newItem)
		{
			_equipmentEquipped[newItem.GetEquipmentType()] = newItem;
			CollectAndCalculate();
		}

		public bool UnEquipItemNotExisted(Equipment newItem, bool equipLastEquipped = true)
		{
			EquipmentType equipmentType = newItem.GetEquipmentType();
			if (_equipmentEquipped.ContainsKey(equipmentType))
			{
				_equipmentEquipped[equipmentType].SetEquipped(false);
				Equipment equipment = null;
				if (equipLastEquipped)
				{
					List<Equipment> equipmentOfType = GetEquipmentOfType(equipmentType, false);
					foreach (Equipment item in equipmentOfType)
					{
						if (item.IsEquipped())
						{
							equipment = item;
							break;
						}
					}
				}
				if (equipment != null)
				{
					_equipmentEquipped[equipmentType] = equipment;
				}
				else
				{
					_equipmentEquipped.Remove(equipmentType);
				}
				CollectAndCalculate();
				return true;
			}
			return false;
		}

		public List<Equipment> GetEquipmentOfType(EquipmentType type, bool onlyVisible = true)
		{
			List<Equipment> list = new List<Equipment>();
			if (_equipmentAll.ContainsKey(type))
			{
				List<Equipment> list2 = _equipmentAll[type];
				foreach (Equipment item in list2)
				{
					if (!onlyVisible || !item.IsHidden())
					{
						list.Add(item);
					}
				}
			}
			return list;
		}

		public List<BaseItem> GetItemsOfType(EquipmentType type, bool onlyVisible = true)
		{
			List<BaseItem> list = new List<BaseItem>();
			foreach (Equipment item in GetEquipmentOfType(type, onlyVisible))
			{
				list.Add(item);
			}
			return list;
		}

		public List<ISlotItem> GetAvailablePerksForItem(Equipment itemValue)
		{
			return _itemHolder.GetAvailablePerksForItemClone(itemValue);
		}

		public SF3.Items.Perk GetPerkByID(int prkID)
		{
			return _itemHolder.GetPerkByID(prkID);
		}

		public void UpdateItemPerksData(ISlotable item)
		{
			List<ItemSlot> slotItems = item.GetSlotItems();
			for (int i = 0; i < slotItems.Count; i++)
			{
				SF3.Items.Perk perkByID = GetPerkByID(slotItems[i].GetId());
				if (perkByID != null)
				{
					slotItems[i].perk.SetStackLevel(perkByID.GetStackLevel());
				}
			}
		}

		public void RemovePerkFromCollection(IPerk perk)
		{
			if (_perksEquippedTemporary.ContainsKey(perk.GetId()))
			{
				_perksEquippedTemporary.Remove(perk.GetId());
				CollectPerks();
				CalculateSummaryAttributes();
			}
		}

		public void AddPerkInCollection(IPerk perk)
		{
			if (ShouldAddPerkTemporary(perk))
			{
				_perksEquippedTemporary.Add(perk.GetId(), perk);
			}
			CollectAndCalculate();
		}

		private bool ShouldAddPerkTemporary(IPerk perk)
		{
			return !_perksEquipped.ContainsKey(perk.GetId()) || (_perksEquipped.ContainsKey(perk.GetId()) && _perksEquipped[perk.GetId()].GetStackLevel() < perk.GetStackLevel());
		}

		public virtual List<T> AddItems<T>(List<T> newItems, bool mergeStackLvl) where T : BaseItem
		{
			List<T> list = new List<T>();
			foreach (T newItem in newItems)
			{
				list.Add(AddItem(newItem, mergeStackLvl) as T);
			}
			return list;
		}

		public virtual BaseItem AddItem(BaseItem newItem, bool mergeStackLvl)
		{
			BaseItem result = null;
			Equipment equipment = newItem as Equipment;
			if (equipment != null)
			{
				if (!_equipmentAll.ContainsKey(equipment.GetEquipmentType()))
				{
					_equipmentAll.Add(equipment.GetEquipmentType(), new List<Equipment>());
				}
				result = GetItemByID(newItem.id);
				if (result == null)
				{
					result = equipment;
					AddEquipmentToArray(equipment);
				}
				else if (mergeStackLvl)
				{
					result = MergeSimilarItems(equipment);
				}
				else
				{
					IStackable stackable = result as IStackable;
					if (stackable != null)
					{
						stackable.SetStackLevel((newItem as IStackable).GetStackLevel());
					}
				}
				if (isPlayer && !equipment.IsHidden())
				{
					UserBadgesManager.Instance.AddItem(UserBadgesManager.BadgeTypes.Inventory, result);
				}
				return result;
			}
			SF3.Items.Perk perk = newItem as SF3.Items.Perk;
			if (perk != null)
			{
				result = MergeSimilarItems(perk);
				if (result == null)
				{
					result = perk;
					_itemHolder.AddPerk(perk);
				}
				if (isPlayer)
				{
					UserBadgesManager.Instance.AddItem(UserBadgesManager.BadgeTypes.Perks, result);
				}
				return result;
			}
			SF3.Items.Booster booster = newItem as SF3.Items.Booster;
			if (booster != null)
			{
				result = booster;
				AddBoosterToArray(booster);
				if (isPlayer)
				{
					UserBadgesManager.Instance.AddItem(UserBadgesManager.BadgeTypes.Boosters, booster);
				}
				return result;
			}
			return result;
		}

		private BaseItem MergeSimilarItems(IStackable newItem)
		{
			IStackable stackable = null;
			if (newItem is Equipment)
			{
				Equipment equipmentItem = (Equipment)newItem;
				List<Equipment> equipmentOfType = GetEquipmentOfType(equipmentItem.GetEquipmentType());
				if (equipmentOfType.Count > 0)
				{
					stackable = equipmentOfType.SingleOrDefault((Equipment item) => item.id.Equals(equipmentItem.id));
				}
			}
			else if (newItem is SF3.Items.Perk)
			{
				SF3.Items.Perk perkItem = (SF3.Items.Perk)newItem;
				List<SF3.Items.Perk> perkByTargetType = _itemHolder.GetPerkByTargetType(perkItem.GetTargetItemType());
				if (perkByTargetType.Count > 0)
				{
					stackable = perkByTargetType.SingleOrDefault((SF3.Items.Perk item) => item.id.Equals(perkItem.id));
				}
			}
			if (stackable != null)
			{
				stackable.MergeSimilarItems(newItem);
				return (BaseItem)stackable;
			}
			return null;
		}

		public virtual BaseItem RemoveItem(int itemID)
		{
			BaseItem itemByID = GetItemByID(itemID);
			if (isPlayer)
			{
				UserBadgesManager.Instance.RemoveItem(itemByID);
			}
			Equipment equipment = itemByID as Equipment;
			if (equipment != null)
			{
				Equipment equipment2 = equipment;
				if (equipment2.IsEquipped())
				{
					UnEquipItem(itemID);
				}
				_equipmentAll[equipment2.GetEquipmentType()].Remove(equipment2);
			}
			return itemByID;
		}

		public void CollectAndCalculate()
		{
			CollectPerks();
			CalculateSummaryAttributes();
		}

		public void SetIsPlayer(bool isPlayerVar)
		{
			isPlayer = isPlayerVar;
		}

		public void SetIsControl(bool isControlVar)
		{
			isControl = isControlVar;
		}

		public void ResetModelInfoStats()
		{
			ResetLifeState();
			CalculateSummaryAttributes();
			if (this.onZeroHP != null)
			{
				Delegate[] invocationList = this.onZeroHP.GetInvocationList();
				foreach (Delegate @delegate in invocationList)
				{
					onZeroHP -= (Action)@delegate;
				}
			}
		}

		public void ResetLifeState()
		{
			if (FightController.Instance.CurrentFight != null && FightController.Instance.CurrentFight.hpRecovery > 0f && currentLife > 0f)
			{
				currentLife = Mathf.Clamp(currentLife + maxLife * FightController.Instance.CurrentFight.hpRecovery, 0f, maxLife);
			}
			else
			{
				currentLife = maxLife;
			}
			score = 0;
		}

		public void SetGender(Gender genderVar)
		{
			gender = genderVar;
		}

		public void ChangeLife(float delta)
		{
			float a = currentLife + delta;
			currentLife = Mathf.Max(Mathf.Min(a, maxLife), 0f);
			if (currentLife <= 0f)
			{
				this.onZeroHP.InvokeSafe();
			}
		}

		public void IncreaseScore(int amount)
		{
			score += amount;
		}

		public object GetEquippedPerkAttributeValue(int perkId, string attributeName)
		{
			IPerk equippedPerk = GetEquippedPerk(perkId);
			return (equippedPerk == null) ? ((object)0) : equippedPerk.GetAttributeValue(attributeName);
		}

		private IPerk GetEquippedPerk(int perkId)
		{
			return GetEquippedPerk((IPerk perk) => perk.GetId() == perkId);
		}

		public string PrintEquipment()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<EquipmentType, Equipment> item in _equipmentEquipped)
			{
				stringBuilder.Append(string.Format("{0} : {1}\n", item.Key, item.Value.model));
			}
			return stringBuilder.ToString();
		}

		public string GetEquippedImage(EquipmentType type)
		{
			if (_equipmentEquipped.ContainsKey(type))
			{
				return _equipmentEquipped[type].Image;
			}
			return null;
		}

		public bool GetEquippedIsHidden(EquipmentType type)
		{
			if (_equipmentEquipped.ContainsKey(type))
			{
				return _equipmentEquipped[type].IsHidden();
			}
			return true;
		}

		public void Drop(Dictionary<EquipmentType, ModelObject> droppedItemsNew)
		{
			_dropped = droppedItemsNew;
		}

		public Equipment GetEquippedByType(EquipmentType type)
		{
			return (!_equipmentEquipped.ContainsKey(type)) ? null : _equipmentEquipped[type];
		}

		public void AddAndEquipSingleInstance(Equipment newEquipment, bool mergeStackLvl)
		{
			Equipment equippedByType = GetEquippedByType(newEquipment.GetEquipmentType());
			if (equippedByType != null)
			{
				RemoveItem(newEquipment.id);
			}
			AddItem(newEquipment, mergeStackLvl);
			EquipItem(newEquipment.id);
		}

		public bool ContainsTag(string tag)
		{
			return resultTags.Any((string res) => res.Equals(tag));
		}

		public void SetHair(string hair)
		{
			_equipmentDefault[EquipmentType.Helmet].SetModel(hair);
		}

		public string GetHair()
		{
			return _equipmentDefault[EquipmentType.Helmet].model;
		}

		public List<Rule> GetRulesByType(string type)
		{
			List<Rule> list = new List<Rule>();
			foreach (Rule rule in rules)
			{
				if (rule.Type.ToUpper().Equals(type.ToUpper()))
				{
					list.Add(rule);
				}
			}
			return list;
		}

		public IPerk GetEquippedPerkByAnimationGroup(string[] animationGroupNames)
		{
			return GetEquippedPerk((IPerk perk) => animationGroupNames.Any((string name) => name.ToLower().Equals(perk.GetName().ToLower())));
		}

		public IPerk GetEquippedPerk(Func<IPerk, bool> criteria)
		{
			IPerk perk = _perksEquipped.Values.FirstOrDefault(criteria);
			return perk ?? _perksEquippedTemporary.Values.FirstOrDefault(criteria);
		}

		public object Clone()
		{
			ModelInfo modelInfo = (ModelInfo)MemberwiseClone();
			modelInfo.attributes = new ModelAttributes();
			modelInfo._dropped = new Dictionary<EquipmentType, ModelObject>();
			modelInfo._rules = new List<Rule>();
			modelInfo._hairColor = _hairColor.Clone();
			modelInfo._skinColor = _skinColor.Clone();
			modelInfo._equipmentAll = CollectionExtensions.DeapCopy(_equipmentAll);
			modelInfo._boostersAll = CollectionExtensions.DeapCopy(_boostersAll);
			modelInfo._equipmentEquipped = CollectionExtensions.DeapCopy(_equipmentEquipped);
			modelInfo._equipmentDefault = CollectionExtensions.DeapCopy(_equipmentDefault);
			modelInfo._perksEquipped = CollectionExtensions.DeapCopy(_perksEquipped);
			modelInfo._perksEquippedTemporary = CollectionExtensions.DeapCopy(_perksEquippedTemporary);
			(modelInfo._itemHolder = new ItemHolder()).AddPerks(_itemHolder.GetAll());
			return modelInfo;
		}

		private void Parse(Dictionary<string, JsValue> userData)
		{
			foreach (KeyValuePair<string, JsValue> userDatum in userData)
			{
				switch (userDatum.Key)
				{
				case "Alias":
					SetAlias(userDatum.Value.AsString());
					break;
				case "AiMode":
					SetAIMode(EnumsCompliancer.GetEnumerator<AiMode>(userDatum.Value.AsInteger()));
					break;
				case "Gender":
					SetGender(EnumsCompliancer.GetEnumerator<Gender>(userDatum.Value.AsInteger()));
					break;
				case "WarriorPower":
					warriorPower = userDatum.Value.AsNumber();
					break;
				case "Tags":
					SetTags(userDatum.Value.AsArray().ToStringArray());
					break;
				case "Appearance":
				{
					Dictionary<string, JsValue> dictionary2 = userDatum.Value.AsDictionary();
					SetHead(dictionary2["Head"].AsDictionary()["Head"].AsString());
					ParseColor(dictionary2["SkinColor"].AsDictionary(), out _skinColor);
					ParseColor(dictionary2["HairColor"].AsDictionary(), out _hairColor);
					break;
				}
				case "Equipments":
				{
					ArrayInstance arrayInstance = userDatum.Value.AsArray();
					for (int i = 0; i < arrayInstance.Length; i++)
					{
						Dictionary<string, JsValue> dictionary = arrayInstance[i].AsDictionary();
						Equipment equipment = Equipment.Create(dictionary["ID"].AsInteger());
						equipment.SetEquipped(true);
						AddEquipmentToArray(equipment);
					}
					break;
				}
				}
			}
		}

		private void Parse(Warrior warriorData)
		{
			SetAlias(warriorData.Alias);
			SetAIMode(warriorData.AiMode);
			SetGender(warriorData.Gender);
			Dictionary<string, JsValue> dictionary = JS.Instance.GetScope("WarriorAppearencesMap").AsDictionary();
			Dictionary<string, JsValue> dictionary2 = dictionary[warriorData.AppearanceId.ToString()].AsDictionary();
			Dictionary<string, JsValue> dictionary3 = dictionary2["Head"].AsDictionary();
			SetHead(dictionary3["Head"].ToString());
			ParseColor(dictionary2["HairColor"].AsDictionary(), out _hairColor);
			ParseColor(dictionary2["SkinColor"].AsDictionary(), out _skinColor);
			warriorPower = warriorData.Power;
			warriorData.Perks.ToList().ForEach(delegate(sf3DTO.Perk perkValue)
			{
				SF3.Items.Perk perk = SF3.Items.Perk.Create(perkValue);
				_itemHolder.AddPerk(perk);
				AddPerkInCollection(perk);
			});
			warriorData.Equipments.ToList().ForEach(delegate(WarriorItemId equipmentValue)
			{
				Equipment equipment = Equipment.Create(equipmentValue.ModelId);
				equipment.ClearPerks();
				equipment.SetEquipped(true);
				AddEquipmentToArray(equipment);
			});
		}

		private void Parse(JSONNode data)
		{
			JSONClass jSONClass = data as JSONClass;
			foreach (KeyValuePair<string, JSONNode> item in jSONClass)
			{
				JSONNode value = item.Value;
				switch (item.Key)
				{
				case "Gender":
					SetGender(EnumsCompliancer.GetEnumerator<Gender>(value.Value));
					break;
				case "Skeleton":
					SetSkeleton(value.Value);
					break;
				case "Appearance":
				{
					JSONClass jSONClass3 = (JSONClass)value;
					SetHead(jSONClass3["Head"].Value);
					ParseColor((JSONClass)jSONClass3["SkinColor"], out _skinColor);
					ParseColor((JSONClass)jSONClass3["HairColor"], out _hairColor);
					break;
				}
				case "Tags":
					SetTags(value.AsArray.ToStringArray());
					break;
				case "Perks":
				{
					JSONArray asArray3 = value.AsArray;
					foreach (JSONClass item2 in asArray3)
					{
						SF3.Items.Perk perkLoaded = SF3.Items.Perk.Create(item2);
						_itemHolder.AddPerk(perkLoaded);
					}
					break;
				}
				case "Equipments":
				{
					JSONArray asArray2 = value.AsArray;
					foreach (JSONClass item3 in asArray2)
					{
						Equipment equipment = Equipment.Create(item3["ID"].AsInt);
						equipment.FillData(item3);
						AddEquipmentToArray(equipment);
					}
					break;
				}
				case "Boosters":
				{
					JSONArray asArray = value.AsArray;
					foreach (JSONClass item4 in asArray)
					{
						SF3.Items.Booster booster = SF3.Items.Booster.Create(item4);
						AddBoosterToArray(booster);
					}
					break;
				}
				}
			}
		}

		private void Parse(Mapping data)
		{
			foreach (Node datum in data)
			{
				switch (datum.key)
				{
				case "Alias":
					SetAlias(datum.value.ToString());
					break;
				case "Controller":
					SetIsControl(datum.value.ToString() == "1");
					break;
				case "AiMode":
				{
					string enumeratorName = datum.value.ToString();
					AiMode enumerator5 = EnumsCompliancer.GetEnumerator<AiMode>(enumeratorName);
					SetAIMode(enumerator5);
					break;
				}
				case "Player":
					SetIsPlayer(datum.value.ToString() == "1");
					break;
				case "Gender":
					SetGender(EnumsCompliancer.GetEnumerator<Gender>(datum.value.ToString()));
					break;
				case "Skeleton":
					SetSkeleton(datum.value.ToString());
					break;
				case "Head":
					SetHead(datum.value.ToString());
					break;
				case "WarriorPower":
					warriorPower = double.Parse(datum.value.ToString(), CultureInfo.InvariantCulture);
					break;
				case "SkinColor":
					ParseColor((Mapping)datum, out _skinColor);
					break;
				case "HairColor":
					ParseColor((Mapping)datum, out _hairColor);
					break;
				case "Tags":
					SetTags(((Sequence)datum).ToStringArray());
					break;
				case "Perks":
				{
					Sequence sequence3 = datum as Sequence;
					foreach (Mapping item in sequence3)
					{
						SF3.Items.Perk perkLoaded = SF3.Items.Perk.Create(item);
						_itemHolder.AddPerk(perkLoaded);
					}
					break;
				}
				case "Equipments":
				{
					Sequence sequence2 = datum as Sequence;
					foreach (Mapping item2 in sequence2)
					{
						Equipment equipment = Equipment.Create(item2);
						AddEquipmentToArray(equipment);
					}
					break;
				}
				case "Boosters":
				{
					Sequence sequence = datum as Sequence;
					foreach (Mapping item3 in sequence)
					{
						SF3.Items.Booster booster = SF3.Items.Booster.Create(item3);
						AddBoosterToArray(booster);
					}
					break;
				}
				}
			}
		}

		private void Parse(BrawlerEnemy warriorData)
		{
			SetAlias(warriorData.ShortPlayer.Nickname);
			aiMode = AiMode.RegularMode;
			gender = warriorData.Appearance.Gender;
			head = GetHeadByID(warriorData.Appearance.HeadId);
			_hairColor = warriorData.Appearance.HairColor;
			_skinColor = warriorData.Appearance.SkinColor;
			_itemHolder.AddPerks(warriorData.Perks.Select(SF3.Items.Perk.Create).ToList());
			useEquipmentAttributes = true;
			warriorData.Items.ToList().ForEach(delegate(Item equipmentValue)
			{
				Equipment equipment = Equipment.Create(equipmentValue);
				equipment.SetEquipped(true);
				AddEquipmentToArray(equipment);
			});
		}

		protected void InitDependencies()
		{
			CreateDefaultEquipment();
			CreateEquipped();
			CollectAndCalculate();
		}

		public void ParseColor(Dictionary<string, JsValue> inputNode, out sf3DTO.Color colorData)
		{
			colorData = new sf3DTO.Color();
			colorData.ColorId = inputNode["Color"].AsDictionary()["ID"].AsInteger();
			colorData.Value = inputNode["Value"].AsNumber();
		}

		public void ParseColor(Mapping inputNode, out sf3DTO.Color colorData)
		{
			colorData = new sf3DTO.Color();
			colorData.ColorId = int.Parse(inputNode.GetMapping("Color").GetText("ID").text);
			colorData.Value = double.Parse(inputNode.GetText("Value").text);
		}

		public void ParseColor(JSONClass root, out sf3DTO.Color colorData)
		{
			colorData = new sf3DTO.Color();
			foreach (KeyValuePair<string, JSONNode> item in root)
			{
				JSONNode value = item.Value;
				switch (item.Key)
				{
				case "Color":
					colorData.ColorId = int.Parse(value.Value);
					break;
				case "Value":
					colorData.Value = double.Parse(value.Value);
					break;
				}
			}
		}

		public Mapping ToYAML()
		{
			List<Node> list = new List<Node>();
			list.Add(new Scalar("Alias", alias));
			list.Add(new Scalar("Gender", EnumsCompliancer.GetEnumeratorName<Gender>((int)gender) ?? Gender.Male.ToString()));
			list.Add(new Scalar("Head", head));
			list.Add(new Scalar("AiMode", (aiMode == AiMode.RegularMode) ? string.Empty : EnumsCompliancer.GetEnumeratorName<AiMode>((int)aiMode)));
			list.Add(new Scalar("WarriorPower", warriorPower.ToString(CultureInfo.InvariantCulture)));
			List<Node> list2 = list;
			if (tags.Except(FightSettings.defaultTags).Count() != 0)
			{
				list2.Add(new Sequence("Tags", ((IEnumerable<string>)tags).Select((Func<string, Node>)((string el) => new Scalar("Tag", el))).ToList()));
			}
			Mapping mapping = new Mapping("Color", new Node[1]
			{
				new Scalar("ID", hairColor.ColorId.ToString())
			});
			Scalar scalar = new Scalar("Value", hairColor.Value.ToString());
			list2.Add(new Mapping("HairColor", new Node[2] { mapping, scalar }));
			mapping = new Mapping("Color", new Node[1]
			{
				new Scalar("ID", skinColor.ColorId.ToString())
			});
			scalar = new Scalar("Value", skinColor.Value.ToString());
			list2.Add(new Mapping("SkinColor", new Node[2] { mapping, scalar }));
			List<Equipment> list3 = (from eqqValue in _equipmentAll.Values
				from eqqSingle in eqqValue
				select eqqSingle).ToList();
			if (list3.Count > 0)
			{
				list2.Add(new Sequence("Equipments", ((IEnumerable<Equipment>)list3).Select((Func<Equipment, Node>)((Equipment eqq) => new Mapping("Item", eqq.ToYaml()))).ToList()));
			}
			IEnumerable<SF3.Items.Perk> perkAll = _itemHolder._perksAll.GetPerkAll();
			if (perkAll.Count() > 0)
			{
				list2.Add(new Sequence("Perks", perkAll.Select((Func<SF3.Items.Perk, Node>)((SF3.Items.Perk eqq) => new Mapping("Item", eqq.ToYaml()))).ToList()));
			}
			List<SF3.Items.Booster> list4 = (from booValue in _boostersAll.Values
				from booSingle in booValue
				select booSingle).ToList();
			if (list4.Count > 0)
			{
				list2.Add(new Sequence("Boosters", ((IEnumerable<SF3.Items.Booster>)list4).Select((Func<SF3.Items.Booster, Node>)((SF3.Items.Booster boo) => new Mapping("Item", boo.ToYaml()))).ToList()));
			}
			return new Mapping("ModelInfo", list2);
		}

		public JSONClass ToJSON()
		{
			throw new NotImplementedException();
		}
	}
}
