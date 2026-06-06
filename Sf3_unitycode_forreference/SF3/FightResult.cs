using System;
using System.Collections.Generic;
using Nekki;
using SF3.Items;
using UnityEngine;
using sf3DTO;

namespace SF3
{
	public class FightResult
	{
		private readonly FightRewards.RoundReward _fightReward;

		private readonly Dictionary<string, long> _fightRewardBonus;

		private readonly Dictionary<CurrencyType, long> _currency;

		private readonly Dictionary<CurrencyType, long> _currencyForMultipliers;

		private readonly List<RewardMultipyerCounterUnit> _rewardMultipliers;

		public IBattleInfo currentBattle { get; private set; }

		public sf3DTO.BattleType BattleType
		{
			get
			{
				return currentBattle.GetBattleType();
			}
		}

		public FightInfo currentFight { get; private set; }

		public int roundsWon { get; private set; }

		public int roundsPlayed { get; private set; }

		public sf3DTO.FightResult resultType { get; private set; }

		public int currentBattleCounter { get; private set; }

		public FightResult(IBattleInfo currentBattleValue, FightInfo currentFightValue, int roundsWonValue, int roundsPlayedValue, bool surrender, RewardMultipyerCounter additionalRewards)
		{
			currentBattle = currentBattleValue;
			currentBattleCounter = ((currentBattle is GenericBattleInfo) ? ((GenericBattleInfo)currentBattle).GetBattleCounter() : 0);
			currentFight = currentFightValue;
			roundsWon = roundsWonValue;
			roundsPlayed = roundsPlayedValue;
			SetFightResult(surrender);
			_fightReward = currentFight.GetRewardsByRoundsWin(roundsWon);
			_fightRewardBonus = new Dictionary<string, long>();
			_currency = new Dictionary<CurrencyType, long>();
			_currencyForMultipliers = new Dictionary<CurrencyType, long>();
			_rewardMultipliers = new List<RewardMultipyerCounterUnit>();
			CalculateRewards(additionalRewards);
		}

		public bool CanGiveRewards()
		{
			return currentBattle.GetBattleType() != sf3DTO.BattleType.Test;
		}

		public static FightResult CreateFightResultWin(IBattleInfo battleInfo)
		{
			if (!battleInfo.GetIsCompleted() && battleInfo.GetCurrentFight() != null)
			{
				return new FightResult(battleInfo, battleInfo.GetCurrentFight(), battleInfo.GetCurrentFight().roundsToWin, battleInfo.GetCurrentFight().roundsToWin, false, null);
			}
			return null;
		}

		private void SetFightResult(bool surrender)
		{
			if (surrender)
			{
				resultType = sf3DTO.FightResult.Surrender;
			}
			else if (currentFight.roundsToWin == roundsWon)
			{
				resultType = sf3DTO.FightResult.Win;
			}
			else
			{
				resultType = sf3DTO.FightResult.Loss;
			}
		}

		private void CalculateRewards(RewardMultipyerCounter additionalRewards)
		{
			if (resultType != sf3DTO.FightResult.Surrender)
			{
				FillRewardMultipliers(additionalRewards);
				FillCurrency();
				CalculateBonus();
			}
		}

		private void FillRewardMultipliers(RewardMultipyerCounter additionalRewards)
		{
			if (additionalRewards == null)
			{
				return;
			}
			foreach (RewardMultipyerCounterUnit multiplier in additionalRewards.GetMultipliers())
			{
				int num = JS.CallFunction("getRewardMultiplierLimit", multiplier.ID).AsInteger();
				num *= roundsPlayed;
				multiplier.SetValue(Mathf.Min((float)multiplier.Value, num));
				_rewardMultipliers.Add(multiplier);
			}
		}

		private void FillCurrency()
		{
			List<CurrencyType> enumerators = EnumsCompliancer.GetEnumerators<CurrencyType>();
			foreach (CurrencyType item in enumerators)
			{
				_currency.Add(item, currentFight.GetCurrencyByRoundsWin(roundsWon, item));
			}
			foreach (CurrencyType item2 in enumerators)
			{
				_currencyForMultipliers.Add(item2, currentFight.GetCurrencyByRoundsWinForMultiplies(roundsWon, item2));
			}
		}

		private void CalculateBonus()
		{
			if (_rewardMultipliers.Count == 0)
			{
				return;
			}
			foreach (RewardMultipyerCounterUnit rewardMultiplier in _rewardMultipliers)
			{
				double value = rewardMultiplier.Value;
				if (value > 0.0)
				{
					long value2 = Calculate(_currencyForMultipliers[CurrencyType.Coin], rewardMultiplier.ID, (int)Math.Round(value));
					_fightRewardBonus.Add(rewardMultiplier.Name, value2);
				}
			}
		}

		private long Calculate(long baseReward, int multiplierId, int amount)
		{
			return JsFunction.CalculateRewardMultiplier(baseReward, multiplierId, amount, roundsPlayed, true);
		}

		public double GetRewardMultiplierUsagesQuantity(string key)
		{
			foreach (RewardMultipyerCounterUnit rewardMultiplier in _rewardMultipliers)
			{
				if (rewardMultiplier.Name.Equals(key))
				{
					return rewardMultiplier.Value;
				}
			}
			return 0.0;
		}

		public Dictionary<string, long> GetFightRewardBonus()
		{
			return _fightRewardBonus;
		}

		public long GetRewardExperience()
		{
			return _fightReward.experience;
		}

		public long GetRewardCurrency(CurrencyType type)
		{
			return (!_currency.ContainsKey(type)) ? 0 : _currency[type];
		}

		public long GetRewardCurrencyForMultipliers(CurrencyType type)
		{
			return (!_currencyForMultipliers.ContainsKey(type)) ? 0 : _currencyForMultipliers[type];
		}

		public List<Equipment> GetRewardEquipment()
		{
			return _fightReward.equipments;
		}

		public List<SF3.Items.Perk> GetRewardPerks()
		{
			return _fightReward.perks;
		}

		public FightRewards.RoundReward GetFightReward()
		{
			return _fightReward;
		}

		public List<SF3.Items.Booster> GetRewardBoosters()
		{
			return _fightReward.boosters;
		}

		public override string ToString()
		{
			return string.Format("CurrentFight:[{0}], RoundsWon:[{1}], FightResult:[{2}]", currentFight.fightID, roundsWon, resultType);
		}

		public IEnumerable<FinishFightRewardMultiplier> GetRewardMultipliers()
		{
			List<FinishFightRewardMultiplier> list = new List<FinishFightRewardMultiplier>();
			foreach (RewardMultipyerCounterUnit rewardMultiplier in _rewardMultipliers)
			{
				list.Add(new FinishFightRewardMultiplier
				{
					Amount = (int)rewardMultiplier.Value,
					MultiplierId = rewardMultiplier.ID
				});
			}
			return list;
		}
	}
}
