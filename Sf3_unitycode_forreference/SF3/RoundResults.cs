using System.Collections.Generic;
using SF3.GameModels;

namespace SF3
{
	public class RoundResults
	{
		private const float GreatHealth = 0.1f;

		private double _comboCountMax;

		private const string ComboCounter = "Reward_Bonus_Combo";

		public ModelInfo RoundWinner { get; set; }

		public ERoundResult RoundResult { get; set; }

		public bool IsPerfect
		{
			get
			{
				List<Rule> rulesByType = RoundWinner.GetRulesByType("ScoreFight");
				bool flag = rulesByType != null && rulesByType.Count > 0;
				return RoundWinner.currentLife == 1f && RoundResult != ERoundResult.TIME_OUT && !flag;
			}
		}

		public bool IsGreat
		{
			get
			{
				return RoundWinner.currentLife <= 0.1f;
			}
		}

		private RoundResults()
		{
			_comboCountMax = 0.0;
		}

		private double GetGameVariableValue(string key, int ownerId)
		{
			GameVariables.LocalVariable variable = GameVariables.GetVariable(ownerId, key);
			if (variable != null)
			{
				return (double)variable.value;
			}
			return 0.0;
		}

		private void SetRewardCounter(string key, double value, bool rewrite = false)
		{
			FightController.Instance.RewardMultipyerCounter.Set(key, value, rewrite);
		}

		public void UpdateRewardCountersOf(ModelType type)
		{
			int idByType = GetIdByType(type);
			foreach (string value in RewardMultipyerCounter.Instance.RewardMultipliersMap.Values)
			{
				double gameVariableValue = GetGameVariableValue(value, idByType);
				if (gameVariableValue > 0.0)
				{
					if (!value.Equals("Reward_Bonus_Combo"))
					{
						SetRewardCounter(value, gameVariableValue);
					}
					else if (gameVariableValue > _comboCountMax)
					{
						SetRewardCounter(value, gameVariableValue, true);
						_comboCountMax = gameVariableValue;
					}
				}
			}
		}

		public int GetWinnerId()
		{
			return GetIdByType((RoundResult != ERoundResult.PLAYER_WIN) ? ModelType.Enemy : ModelType.Player);
		}

		private static int GetIdByType(ModelType type)
		{
			return (type != 0) ? ModelsManager.Instance.Enemy.id : ModelsManager.Instance.Player.id;
		}

		public static RoundResults Create(ERoundResult status)
		{
			RoundResults roundResults = new RoundResults();
			roundResults.RoundResult = status;
			return roundResults;
		}
	}
}
