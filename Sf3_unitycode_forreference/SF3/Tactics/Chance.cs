using System;
using System.Collections.Generic;
using Nekki.Yaml;

namespace SF3.Tactics
{
	public class Chance
	{
		public static readonly float DefaultFactorValue;

		public static readonly float DefaultLimitValue;

		public static readonly float DefaultBaseValue;

		private readonly ChanceType _chanceType;

		private readonly float _base;

		private readonly float _limit;

		public Dictionary<FactorType, double> Factors { get; private set; }

		private Chance()
		{
			_chanceType = ChanceType.Block;
			_base = DefaultBaseValue;
			_limit = DefaultLimitValue;
			Factors = new Dictionary<FactorType, double>();
			foreach (FactorType value in Enum.GetValues(typeof(FactorType)))
			{
				Factors.Add(value, DefaultFactorValue);
			}
		}

		public Chance(Node node)
			: this()
		{
			if (!SF3Utils.TryParseEnum(out _chanceType, node.key, ChanceType.Block))
			{
				Messenger.Error(string.Format("Failed to parse tactics chance type. Value to parse : [{0}].", node.key), this);
			}
			Mapping mapping = node as Mapping;
			if (mapping == null)
			{
				return;
			}
			YamlUtils.TryGetFloat(out _base, mapping, "Base", DefaultBaseValue);
			YamlUtils.TryGetFloat(out _limit, mapping, "Limit", DefaultLimitValue);
			Sequence node2 = mapping.GetNode("Factors") as Sequence;
			foreach (FactorType value in Enum.GetValues(typeof(FactorType)))
			{
				float result;
				YamlUtils.TryGetFloat(out result, node2, value.ToString(), DefaultFactorValue);
				Factors[value] = result;
			}
		}

		public double GetFactorValue(FactorType factorType)
		{
			return Factors[factorType];
		}

		public float GetLimitValue()
		{
			return _limit;
		}

		public float GetBaseValue()
		{
			return _base;
		}

		public ChanceType GetChanceType()
		{
			return _chanceType;
		}

		public override string ToString()
		{
			StringWrapper stringWrapper = StringWrapper.Create(this);
			stringWrapper.Wrap("_chanceType", _chanceType.ToString());
			stringWrapper.Wrap("_base", _base);
			stringWrapper.Wrap("_limit", _limit);
			foreach (KeyValuePair<FactorType, double> factor in Factors)
			{
				stringWrapper.Wrap(string.Format("Factors: <Key: [{0}], Value: [{1}]>\n", factor.Key, factor.Value));
			}
			return stringWrapper.ToString();
		}
	}
}
