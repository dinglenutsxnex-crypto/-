using System;
using System.Collections.Generic;
using System.Linq;
using Nekki;
using SF3.GameModels;
using SF3.Settings;

namespace SF3.Tactics
{
	public class ChancesCalculator
	{
		private readonly List<ChanceType> _chanceTypesUsed;

		private readonly TacticsMode _mode;

		private readonly Dictionary<string, ICounter> _animationFactorCounters;

		public ChancesCalculator(Model model)
		{
			_mode = TacticsSettings.GetTacticsMode(model.GetAiMode());
			_chanceTypesUsed = _mode.GetChanceTypesUsed();
			_animationFactorCounters = new Dictionary<string, ICounter>();
		}

		private void InitFor(string animationName)
		{
			if (!_animationFactorCounters.ContainsKey(animationName))
			{
				_animationFactorCounters.Add(animationName, new DefaultCounter());
				string[] names = Enum.GetNames(typeof(FactorType));
				foreach (string key in names)
				{
					_animationFactorCounters[animationName].Set(key, 0.0, true);
				}
			}
		}

		public void IncrementCounter(string animationName, FactorType type, double value = 1.0)
		{
			InitFor(animationName);
			_animationFactorCounters[animationName].Increment(type.ToString(), value);
		}

		public ChanceType GetReactionType(string animationName)
		{
			ChanceType result = ChanceType.Block;
			if (!HasAnimationToReact(animationName) || _chanceTypesUsed.IsEmpty())
			{
				return result;
			}
			Dictionary<ChanceType, double> dictionary = new Dictionary<ChanceType, double>();
			foreach (ChanceType item in _chanceTypesUsed)
			{
				dictionary.Add(item, GetTotalWeightFor(animationName, item));
			}
			IOrderedEnumerable<KeyValuePair<ChanceType, double>> orderedEnumerable = dictionary.OrderBy((KeyValuePair<ChanceType, double> pair) => pair.Value);
			StringWrapper stringWrapper = StringWrapper.Create();
			stringWrapper.Head("Chances used for calculation:");
			foreach (KeyValuePair<ChanceType, double> item2 in orderedEnumerable)
			{
				stringWrapper.Wrap(string.Format("<Name: [{0}], Value: [{1}]>\n", item2.Key, item2.Value));
			}
			TacticsLogger.Instance.Log(stringWrapper.ToString(), this);
			double num = 0.0;
			double num2 = dictionary.Select((KeyValuePair<ChanceType, double> pair) => pair.Value).Sum();
			float num3 = NekkiMath.randomFloat((float)num, (float)num2);
			double num4 = 0.0;
			IEnumerator<KeyValuePair<ChanceType, double>> enumerator3 = orderedEnumerable.GetEnumerator();
			while (enumerator3.MoveNext())
			{
				num4 += enumerator3.Current.Value;
				if (num4 >= (double)num3)
				{
					TacticsLogger.Instance.Log(string.Format("WeightMax: [{0}], WeightChosen: [{1}], EntryData: [Key: <{2}>, Value: <{3}>]", num2, num3, enumerator3.Current.Key, enumerator3.Current.Value), this);
					return enumerator3.Current.Key;
				}
			}
			return result;
		}

		private double GetTotalWeightFor(string animationName, ChanceType chanceType)
		{
			if (!HasAnimationToReact(animationName))
			{
				return 0.0;
			}
			double num = 0.0;
			foreach (FactorType value2 in Enum.GetValues(typeof(FactorType)))
			{
				double value = _animationFactorCounters[animationName].GetValue(value2.ToString());
				num += value * _mode.GetFactorValue(chanceType, value2);
			}
			num += _mode.GetChanceBaseValue(chanceType);
			return (!(num > _mode.GetChanceLimitValue(chanceType))) ? num : _mode.GetChanceLimitValue(chanceType);
		}

		private bool HasAnimationToReact(string animationName)
		{
			return _animationFactorCounters.ContainsKey(animationName);
		}
	}
}
