using System.Collections.Generic;
using Nekki;
using Nekki.Yaml;
using SF3.GameModels;
using SF3.Items;
using SimpleJSON;
using sf3DTO;

namespace SF3.UserData
{
	public class UserModelInfo : ModelInfo, ICurrencyOperations
	{
		private int _level;

		private long _experience;

		private long _levelExperience;

		private CurrencyHolder _currencyHolder;

		public int level
		{
			get
			{
				return _level;
			}
		}

		public long experience
		{
			get
			{
				return _experience;
			}
		}

		public long levelExperience
		{
			get
			{
				return _levelExperience;
			}
		}

		public UserModelInfo(JSONNode userData)
			: base(userData)
		{
			Parse(userData);
		}

		private void Parse(Mapping data)
		{
			_currencyHolder = new CurrencyHolder();
			_level = 1;
			_experience = 0L;
			_levelExperience = 0L;
			foreach (Node datum in data)
			{
				switch (datum.key)
				{
				case "Level":
					_level = int.Parse(datum.value.ToString());
					break;
				case "LevelExperience":
					_levelExperience = long.Parse(datum.value.ToString());
					break;
				case "Experience":
					_experience = int.Parse(datum.value.ToString());
					break;
				case "Currency":
					foreach (Node item in datum)
					{
						CurrencyType enumerator3 = EnumsCompliancer.GetEnumerator<CurrencyType>(item.key);
						_currencyHolder.SetCurrency(enumerator3, long.Parse(item.value.ToString()));
					}
					break;
				}
			}
		}

		private void Parse(JSONNode data)
		{
			_currencyHolder = new CurrencyHolder();
			_level = 1;
			_experience = 0L;
			_levelExperience = 0L;
			JSONClass jSONClass = data as JSONClass;
			foreach (KeyValuePair<string, JSONNode> item in jSONClass)
			{
				JSONNode value = item.Value;
				switch (item.Key)
				{
				case "Level":
					_level = value.AsInt;
					break;
				case "LevelExperience":
					_levelExperience = value.AsLong;
					break;
				case "Experience":
					_experience = value.AsInt;
					break;
				case "Currency":
				{
					JSONClass jSONClass2 = value as JSONClass;
					foreach (KeyValuePair<string, JSONNode> item2 in jSONClass2)
					{
						JSONNode value2 = item2.Value;
						CurrencyType enumerator3 = EnumsCompliancer.GetEnumerator<CurrencyType>(item2.Key);
						_currencyHolder.SetCurrency(enumerator3, value2.AsLong);
					}
					break;
				}
				}
			}
		}

		public void SetLevelExperience(long value)
		{
			_levelExperience = value;
		}

		public void SetLevel(int value)
		{
			_level = value;
		}

		public void SetExperience(long value)
		{
			_experience = value;
		}

		public void SetEquipments(List<Equipment> equipments)
		{
			foreach (KeyValuePair<EquipmentType, List<Equipment>> item in _equipmentAll)
			{
				item.Value.Clear();
			}
			foreach (Equipment equipment in equipments)
			{
				_equipmentAll[equipment.GetEquipmentType()].Add(equipment);
			}
		}

		public void SetPerks(List<SF3.Items.Perk> perks)
		{
			_itemHolder._perksAll.Clear();
			foreach (SF3.Items.Perk perk in perks)
			{
				_itemHolder.AddPerk(perk);
			}
		}

		public void SetBoosters(List<SF3.Items.Booster> boosters)
		{
			_boostersAll.Clear();
			foreach (SF3.Items.Booster booster in boosters)
			{
				AddBoosterToArray(booster);
			}
		}

		public void UpdateModelInfo()
		{
			InitDependencies();
		}

		public override BaseItem RemoveItem(int itemID)
		{
			BaseItem baseItem = base.RemoveItem(itemID);
			UserManager.RemoveEquipment(baseItem as Equipment);
			return baseItem;
		}

		public override EquippedResult EquipItem(int itemID)
		{
			EquippedResult equippedResult = base.EquipItem(itemID);
			if (equippedResult != null)
			{
				UserManager.EquipItem(equippedResult.oldItemID, GetEquipped(equippedResult.newItem.GetEquipmentType()));
				CalculateSummaryAttributes();
			}
			return equippedResult;
		}

		public override Equipment UnEquipItem(int unequipID)
		{
			Equipment equipment = base.UnEquipItem(unequipID);
			if (equipment != null)
			{
				UserManager.UnEquipItem(equipment);
				CalculateSummaryAttributes();
			}
			return equipment;
		}

		public Currency GetCurrency(CurrencyType type)
		{
			return _currencyHolder.GetCurrency(type);
		}

		public long GetCurrencyValue(CurrencyType type)
		{
			return _currencyHolder.GetCurrencyValue(type);
		}

		public void SetCurrency(Currency c)
		{
			SetCurrency(c.CurrencyType, c.Value);
		}

		public void SetCurrency(CurrencyType type, long value)
		{
			_currencyHolder.SetCurrency(type, value);
		}

		public void AddCurrecy(CurrencyType type, long value)
		{
			_currencyHolder.AddCurrecy(type, value);
		}

		public void AddCurrecy(Currency c)
		{
			AddCurrecy(c.CurrencyType, c.Value);
		}

		public void SubtractCurrency(CurrencyType type, long value)
		{
			_currencyHolder.SubtractCurrency(type, value);
		}

		public void SubtractCurrency(Currency c)
		{
			SubtractCurrency(c.CurrencyType, c.Value);
		}

		public bool HasAnyCurrency()
		{
			return _currencyHolder.HasAnyCurrency();
		}
	}
}
