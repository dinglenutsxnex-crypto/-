using System.Collections.Generic;
using Jint.Native;
using Jint.Native.Array;
using Nekki;
using Nekki.Yaml;
using SimpleJSON;
using sf3DTO;

namespace SF3.Items
{
	public class Booster : BaseItem, UserBadgesManager.IBadgeItem, IRarable
	{
		private const string CURRENCIES_KEY = "Currencies";

		private const string CURRENCY_KEY = "Currency";

		private const string EQUIPMENTS_KEY = "Equipments";

		private const string EQUIPMENT_KEY = "Equipment";

		private const string EXPERIENCE_KEY = "Experience";

		private const string ID_KEY = "ID";

		private const string MODEL_ID_KEY = "ModelID";

		private const string PERKS_KEY = "Perks";

		private const string PERK_KEY = "Perk";

		private const string STACK_LEVEL_KEY = "StackLevel";

		private const string TYPE_KEY = "Type";

		private const string VALUE_KEY = "Value";

		private Rarity _rarity;

		private int _zone;

		private int _lootCount;

		public long instance_id { get; private set; }

		public List<BaseItem> equipments { get; private set; }

		public List<BaseItem> perks { get; private set; }

		public List<Currency> currencies { get; private set; }

		public long experience { get; private set; }

		public int Zone
		{
			get
			{
				return _zone;
			}
		}

		public int LootCount
		{
			get
			{
				return _lootCount;
			}
		}

		public List<BaseItem> AllItems
		{
			get
			{
				List<BaseItem> list = new List<BaseItem>();
				foreach (BaseItem equipment in equipments)
				{
					list.Add(equipment);
				}
				foreach (BaseItem perk in perks)
				{
					list.Add(perk);
				}
				return list;
			}
		}

		private Booster()
		{
			instance_id = -1L;
			base.id = -1;
			equipments = new List<BaseItem>();
			perks = new List<BaseItem>();
			currencies = new List<Currency>();
		}

		private Booster(Loot lootData)
		{
			instance_id = -1L;
			base.id = -1;
			equipments = new List<BaseItem>();
			perks = new List<BaseItem>();
			currencies = new List<Currency>();
			if (lootData.Equipments.Count > 0)
			{
				equipments.AddRange(Equipment.Create(lootData.Equipments.RepeatedToList()));
			}
			if (lootData.Perks.Count > 0)
			{
				perks.AddRange(Perk.Create(lootData.Perks.RepeatedToList()));
			}
			if (lootData.Currencies.Count > 0)
			{
				currencies.AddRange(lootData.Currencies);
			}
			experience = lootData.Experience;
		}

		public Booster(KeyValuePair<string, JsValue> keyValuePair)
			: base(keyValuePair)
		{
			instance_id = -1L;
			Dictionary<string, JsValue> dictionary = keyValuePair.Value.AsDictionary();
			_zone = dictionary["Zone"].AsDictionary()["Value"].AsInteger();
			TryParseEnum(out _rarity, GetNodeSafe(dictionary, "Rarity", "COMMON"), Rarity.Common);
			ArrayInstance arrayInstance = dictionary["Loot"].AsDictionary()["And"].AsArray();
			_lootCount = (int)arrayInstance.Length;
		}

		public Rarity GetRarityType()
		{
			return _rarity;
		}

		public static List<Booster> Create(List<sf3DTO.Booster> boosters)
		{
			List<Booster> list = new List<Booster>();
			foreach (sf3DTO.Booster booster in boosters)
			{
				list.Add(Create(booster));
			}
			return list;
		}

		public static Booster Create(sf3DTO.Booster booster)
		{
			Booster booster2 = new Booster(booster.Loot);
			booster2.instance_id = booster.InstanceId;
			booster2.id = booster.ModelId;
			return booster2;
		}

		public static Booster Create(Mapping boosterMapp)
		{
			Booster booster = new Booster();
			Scalar text = boosterMapp.GetText("ID");
			if (text != null)
			{
				booster.instance_id = long.Parse(text.text);
			}
			text = boosterMapp.GetText("ModelID");
			if (text != null)
			{
				booster.id = int.Parse(text.text);
			}
			Sequence sequence = boosterMapp.GetSequence("Equipments");
			if (sequence != null)
			{
				foreach (Mapping item in sequence.nodesInside)
				{
					Equipment equipment;
					ItemsManager.TryGetEquipmentById(int.Parse(item.GetText("ID").text), out equipment);
					equipment.SetStackLevel(double.Parse(item.GetText("StackLevel").text));
					booster.equipments.Add(equipment);
				}
			}
			sequence = boosterMapp.GetSequence("Perks");
			if (sequence != null)
			{
				foreach (Mapping item2 in sequence.nodesInside)
				{
					Perk perk;
					ItemsManager.TryGetPerkById(int.Parse(item2.GetText("ID").text), out perk);
					perk.SetStackLevel(double.Parse(item2.GetText("StackLevel").text));
					booster.perks.Add(perk);
				}
			}
			sequence = boosterMapp.GetSequence("Currencies");
			if (sequence != null)
			{
				foreach (Mapping item3 in sequence.nodesInside)
				{
					booster.currencies.Add(new Currency
					{
						CurrencyType = EnumsCompliancer.GetEnumerator<CurrencyType>(item3.GetText("Type").text),
						Value = long.Parse(item3.GetText("Value").text)
					});
				}
			}
			text = boosterMapp.GetText("Experience");
			if (text != null)
			{
				booster.experience = long.Parse(text.text);
			}
			return booster;
		}

		public static Booster Create(JSONClass boosters)
		{
			Booster booster = new Booster();
			if (boosters.ContainsKey("ID"))
			{
				booster.instance_id = boosters["ID"].AsLong;
			}
			if (boosters.ContainsKey("ModelID"))
			{
				booster.id = boosters["ModelID"].AsInt;
			}
			if (boosters.ContainsKey("Equipments"))
			{
				JSONArray asArray = boosters["Equipments"].AsArray;
				foreach (JSONClass item in asArray)
				{
					Equipment equipment;
					ItemsManager.TryGetEquipmentById(item["ID"].AsInt, out equipment);
					equipment.SetStackLevel(item["StackLevel"].AsDouble);
					booster.equipments.Add(equipment);
				}
			}
			if (boosters.ContainsKey("Perks"))
			{
				JSONArray asArray2 = boosters["Perks"].AsArray;
				foreach (JSONClass item2 in asArray2)
				{
					Perk perk;
					ItemsManager.TryGetPerkById(item2["ID"].AsInt, out perk);
					perk.SetStackLevel(item2["StackLevel"].AsDouble);
					booster.perks.Add(perk);
				}
			}
			if (boosters.ContainsKey("Currencies"))
			{
				JSONArray asArray3 = boosters["Currencies"].AsArray;
				foreach (JSONClass item3 in asArray3)
				{
					booster.currencies.Add(new Currency
					{
						CurrencyType = EnumsCompliancer.GetEnumerator<CurrencyType>(item3["Type"].Value),
						Value = item3["Value"].AsLong
					});
				}
			}
			if (boosters.ContainsKey("Experience"))
			{
				booster.experience = boosters["Experience"].AsLong;
			}
			return booster;
		}

		public override List<Node> ToYaml()
		{
			List<Node> list = new List<Node>();
			list.Add(new Scalar("ID", instance_id.ToString()));
			list.Add(new Scalar("ModelID", base.id.ToString()));
			if (equipments == null)
			{
				return list;
			}
			if (equipments.Count > 0)
			{
				List<Node> list2 = new List<Node>(equipments.Count);
				foreach (BaseItem equipment in equipments)
				{
					list2.Add(new Mapping("Equipment", equipment.ToYaml()));
				}
				list.Add(new Sequence("Equipments", list2));
			}
			if (perks.Count > 0)
			{
				List<Node> list3 = new List<Node>(perks.Count);
				foreach (BaseItem perk in perks)
				{
					list3.Add(new Mapping("Perk", perk.ToYaml()));
				}
				list.Add(new Sequence("Perks", list3));
			}
			if (currencies.Count > 0)
			{
				List<Node> list4 = new List<Node>(currencies.Count);
				foreach (Currency currency in currencies)
				{
					list4.Add(new Mapping("Currency", new List<Node>
					{
						new Scalar("Type", currency.CurrencyType.ToString()),
						new Scalar("Value", currency.Value.ToString())
					}));
				}
				list.Add(new Sequence("Currencies", list4));
			}
			list.Add(new Scalar("Experience", experience.ToString()));
			return list;
		}

		public override JSONClass ToJSON()
		{
			JSONClass jSONClass = new JSONClass();
			jSONClass.Add("ID", new JSONData(instance_id));
			jSONClass.Add("ModelID", new JSONData(base.id));
			AddBaseItems(jSONClass, "Equipments", "Equipment", equipments);
			AddBaseItems(jSONClass, "Perks", "Perk", perks);
			if (currencies.Count > 0)
			{
				JSONArray jSONArray = new JSONArray();
				foreach (Currency currency in currencies)
				{
					JSONClass jSONClass2 = new JSONClass();
					jSONClass2.Add("Type", new JSONData(currency.CurrencyType.ToString()));
					jSONClass2.Add("Value", new JSONData(currency.Value));
					jSONArray.Add("Currency", jSONClass2);
				}
				jSONClass.Add("Currencies", jSONArray);
			}
			jSONClass.Add("Experience", new JSONData(experience));
			return jSONClass;
		}

		private void AddBaseItems(JSONClass itemClass, string arrayKey, string itemKey, List<BaseItem> items)
		{
			if (items.Count <= 0)
			{
				return;
			}
			JSONArray jSONArray = new JSONArray();
			foreach (BaseItem item in items)
			{
				jSONArray.Add(itemKey, item.ToJSON());
			}
			itemClass.Add(arrayKey, jSONArray);
		}

		public new int GetBadgeID()
		{
			return (int)instance_id;
		}
	}
}
