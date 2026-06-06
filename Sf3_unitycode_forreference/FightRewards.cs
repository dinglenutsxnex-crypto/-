using System;
using System.Collections.Generic;
using System.Linq;
using Jint.Native;
using Jint.Native.Array;
using Nekki;
using Nekki.Yaml;
using SF3.Items;
using SimpleJSON;
using sf3DTO;

public class FightRewards
{
	public class RoundReward
	{
		public List<Equipment> equipments { get; private set; }

		public List<SF3.Items.Perk> perks { get; private set; }

		public List<SF3.Items.Booster> boosters { get; private set; }

		public List<Currency> currencies { get; private set; }

		public long experience { get; private set; }

		public RoundReward()
		{
			experience = 0L;
			currencies = new List<Currency>();
			equipments = new List<Equipment>();
			perks = new List<SF3.Items.Perk>();
			boosters = new List<SF3.Items.Booster>();
		}

		public static RoundReward Create(Dictionary<string, JsValue> roundData)
		{
			RoundReward roundReward = new RoundReward();
			foreach (KeyValuePair<string, JsValue> roundDatum in roundData)
			{
				switch (roundDatum.Key)
				{
				case "Currencies":
				{
					Dictionary<string, JsValue> dictionary2 = roundDatum.Value.AsDictionary();
					foreach (KeyValuePair<string, JsValue> item in dictionary2)
					{
						roundReward.currencies.Add(new Currency
						{
							CurrencyType = EnumsCompliancer.GetEnumerator<CurrencyType>(int.Parse(item.Key)),
							Value = item.Value.AsLong()
						});
					}
					break;
				}
				case "Experience":
					roundReward.experience = roundDatum.Value.AsLong();
					break;
				case "Equipments":
				case "Perks":
				{
					Dictionary<string, JsValue> dictionary = roundDatum.Value.AsDictionary();
					double stackLevel = dictionary["SL"].AsNumber();
					ArrayInstance arrayInstance = dictionary["Models"].AsArray();
					for (int i = 0; i < arrayInstance.Length; i++)
					{
						BaseItem itemById = ItemsManager.GetItemById(arrayInstance[i].AsDictionary()["ID"].AsInteger());
						((IStackable)itemById).SetStackLevel(stackLevel);
						if (itemById is Equipment)
						{
							roundReward.equipments.Add((Equipment)itemById);
						}
						else if (itemById is SF3.Items.Perk)
						{
							roundReward.perks.Add((SF3.Items.Perk)itemById);
						}
					}
					break;
				}
				}
			}
			return roundReward;
		}

		public static RoundReward Create(Mapping lootData)
		{
			RoundReward roundReward = new RoundReward();
			foreach (Node item in lootData.nodesInside)
			{
				switch (item.key)
				{
				case "Currencies":
				{
					Sequence sequence4 = item as Sequence;
					foreach (Mapping item2 in sequence4.nodesInside)
					{
						roundReward.currencies.Add(new Currency
						{
							CurrencyType = EnumsCompliancer.GetEnumerator<CurrencyType>(item2.GetText("Type").text),
							Value = long.Parse(item2.GetText("Value").text)
						});
					}
					break;
				}
				case "Experience":
				{
					Scalar scalar = item as Scalar;
					roundReward.experience = long.Parse(scalar.text);
					break;
				}
				case "Equipments":
				{
					Sequence sequence3 = item as Sequence;
					foreach (Mapping item3 in sequence3.nodesInside)
					{
						Equipment equipment;
						ItemsManager.TryGetEquipmentById(int.Parse(item3.GetText("ID").text), out equipment);
						equipment.SetStackLevel(double.Parse(item3.GetText("StackLevel").text));
						roundReward.equipments.Add(equipment);
					}
					break;
				}
				case "Perks":
				{
					Sequence sequence2 = item as Sequence;
					foreach (Mapping item4 in sequence2.nodesInside)
					{
						SF3.Items.Perk perk;
						ItemsManager.TryGetPerkById(int.Parse(item4.GetText("ID").text), out perk);
						perk.SetStackLevel(double.Parse(item4.GetText("StackLevel").text));
						roundReward.perks.Add(perk);
					}
					break;
				}
				case "Boosters":
				{
					Sequence sequence = item as Sequence;
					break;
				}
				}
			}
			return roundReward;
		}

		public static RoundReward Create(Loot lootData)
		{
			RoundReward resultLoot = new RoundReward();
			if (lootData.Equipments.Count > 0)
			{
				lootData.Equipments.RepeatedToList().ForEach(delegate(Item equipmentValue)
				{
					resultLoot.equipments.Add(Equipment.Create(equipmentValue));
				});
			}
			if (lootData.Perks.Count > 0)
			{
				resultLoot.perks.AddRange(lootData.Perks.Select(SF3.Items.Perk.Create).ToList());
			}
			if (lootData.Boosters.Count > 0)
			{
				resultLoot.boosters.AddRange(SF3.Items.Booster.Create(lootData.Boosters.RepeatedToList()));
			}
			if (lootData.Currencies.Count > 0)
			{
				resultLoot.currencies.AddRange(lootData.Currencies);
			}
			resultLoot.experience = lootData.Experience;
			return resultLoot;
		}

		public RoundReward Clone()
		{
			RoundReward result = MemberwiseClone() as RoundReward;
			result.currencies = new List<Currency>();
			currencies.ForEach(delegate(Currency currencyValue)
			{
				result.currencies.Add(currencyValue);
			});
			result.equipments = new List<Equipment>();
			equipments.ForEach(delegate(Equipment equipmentValue)
			{
				result.equipments.Add(equipmentValue);
			});
			result.perks = new List<SF3.Items.Perk>();
			perks.ForEach(delegate(SF3.Items.Perk perkValue)
			{
				result.perks.Add(perkValue);
			});
			return result;
		}

		public Mapping ToYAML()
		{
			List<Node> list = new List<Node>();
			list.Add(new Scalar("Experience", experience.ToString()));
			if (currencies.Count > 0)
			{
				List<Node> list2 = new List<Node>(currencies.Count);
				foreach (Currency currency in currencies)
				{
					list2.Add(new Mapping("Currency", new List<Node>
					{
						new Scalar("Type", currency.CurrencyType.ToString()),
						new Scalar("Value", currency.Value.ToString())
					}));
				}
				list.Add(new Sequence("Currencies", list2));
			}
			if (equipments.Count > 0)
			{
				List<Node> list3 = new List<Node>(equipments.Count);
				foreach (Equipment equipment in equipments)
				{
					list3.Add(new Mapping("Equipment", equipment.ToYaml()));
				}
				list.Add(new Sequence("Equipments", list3));
			}
			if (perks.Count > 0)
			{
				List<Node> list4 = new List<Node>(perks.Count);
				foreach (SF3.Items.Perk perk in perks)
				{
					list4.Add(new Mapping("Perk", perk.ToYaml()));
				}
				list.Add(new Sequence("Perks", list4));
			}
			return new Mapping("FightReward", list);
		}
	}

	private List<RoundReward> _rewardsByRoundWins;

	private List<BaseRewardInfo> _rewardsIcon;

	public List<BaseRewardInfo> rewardsIcon
	{
		get
		{
			return _rewardsIcon;
		}
	}

	public FightRewards()
	{
		_rewardsByRoundWins = new List<RoundReward>();
	}

	public JSONClass ToJSON()
	{
		throw new NotImplementedException();
	}

	public void SetRewards(ArrayInstance rewardsArray)
	{
		_rewardsByRoundWins.Clear();
		for (int i = 0; i < rewardsArray.Length; i++)
		{
			_rewardsByRoundWins.Add(RoundReward.Create(rewardsArray[i].AsDictionary()));
		}
	}

	public void SetRewards(Sequence loots)
	{
		_rewardsByRoundWins.Clear();
		loots.nodesInside.ForEach(delegate(Node lootMap)
		{
			_rewardsByRoundWins.Add(RoundReward.Create((Mapping)lootMap));
		});
	}

	public void SetRewards(List<Loot> loots)
	{
		_rewardsByRoundWins.Clear();
		loots.ForEach(delegate(Loot lootValue)
		{
			_rewardsByRoundWins.Add(RoundReward.Create(lootValue));
		});
	}

	public RoundReward GetRewardsByRoundsWin(int roundsWin)
	{
		if (_rewardsByRoundWins.Count == 0)
		{
			return new RoundReward();
		}
		if (_rewardsByRoundWins.Count <= roundsWin)
		{
			return _rewardsByRoundWins[_rewardsByRoundWins.Count - 1];
		}
		return _rewardsByRoundWins[roundsWin];
	}

	public long GetCurrencyByRoundsWinForMultiplies(int roundsWin, CurrencyType type)
	{
		long currencyByRoundsWin = GetCurrencyByRoundsWin(roundsWin, type);
		if (currencyByRoundsWin > 0)
		{
			return currencyByRoundsWin;
		}
		for (int i = 0; i < _rewardsByRoundWins.Count; i++)
		{
			Currency currencyByTypeOrNull = GetCurrencyByTypeOrNull(_rewardsByRoundWins[i].currencies, type);
			if (currencyByTypeOrNull != null)
			{
				return currencyByTypeOrNull.Value;
			}
		}
		return currencyByRoundsWin;
	}

	public long GetCurrencyByRoundsWin(int roundsWin, CurrencyType type)
	{
		Currency currencyByTypeOrNull = GetCurrencyByTypeOrNull(GetRewardsByRoundsWin(roundsWin).currencies, type);
		return (currencyByTypeOrNull == null) ? 0 : currencyByTypeOrNull.Value;
	}

	private Currency GetCurrencyByTypeOrNull(IEnumerable<Currency> currency, CurrencyType type)
	{
		return currency.FirstOrDefault((Currency c) => c.CurrencyType == type && c.Value > 0);
	}

	public FightRewards Clone()
	{
		FightRewards result = MemberwiseClone() as FightRewards;
		result._rewardsByRoundWins = new List<RoundReward>();
		_rewardsByRoundWins.ForEach(delegate(RoundReward rewardValue)
		{
			result._rewardsByRoundWins.Add(rewardValue.Clone());
		});
		return result;
	}

	public bool HasRewards()
	{
		return _rewardsByRoundWins.Count > 0;
	}

	public Sequence ToYAML()
	{
		return new Sequence("Rewards", ((IEnumerable<RoundReward>)_rewardsByRoundWins).Select((Func<RoundReward, Node>)((RoundReward rewardValue) => rewardValue.ToYAML())).ToList());
	}

	public void SetRewardIcon(ArrayInstance rewardIconArray)
	{
		_rewardsIcon = new List<BaseRewardInfo>();
		for (int i = 0; i < rewardIconArray.Length; i++)
		{
			Dictionary<string, JsValue> dictionary = rewardIconArray[i].AsDictionary();
			Rarity enumerator = EnumsCompliancer.GetEnumerator<Rarity>(dictionary["Rarity"].AsInteger());
			if (dictionary.ContainsKey("ItemType"))
			{
				EquipmentType enumerator2 = JS.Instance.EnumsCompliancer.GetEnumerator<EquipmentType>("ITEMTYPE", dictionary["ItemType"].AsInteger());
				if (enumerator2 != 0)
				{
					_rewardsIcon.Add(new EquipmentRewardInfo(enumerator2, enumerator));
				}
				else
				{
					_rewardsIcon.Add(new PerkRewardInfo(enumerator));
				}
			}
			else
			{
				_rewardsIcon.Add(new EquipmentRewardInfo(EquipmentType.None, enumerator));
			}
		}
	}
}
