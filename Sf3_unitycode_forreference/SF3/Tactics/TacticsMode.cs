using System.Collections.Generic;
using Nekki.Yaml;
using sf3DTO;

namespace SF3.Tactics
{
	public class TacticsMode
	{
		public const int DefaultDelayValue = -1;

		private readonly AiMode _mode;

		private readonly int _delay;

		private readonly ChanceHolder _chances;

		public TacticsMode()
		{
			_mode = AiMode.NoneMode;
			_delay = -1;
			_chances = new ChanceHolder();
		}

		public TacticsMode(Sequence sequence)
			: this()
		{
			if (!SF3Utils.TryParseEnum(out _mode, sequence.key, AiMode.NoneMode))
			{
				Messenger.Error(string.Format("Failed to parse tactics mode type. Value to parse : [{0}].", sequence.key), this);
			}
			if (!YamlUtils.TryGetInt(out _delay, sequence, "Delay", -1))
			{
				Messenger.Error("Failed to parse tactics delay value. Unable to find node by name.", this);
			}
			Node node = YamlUtils.GetNode(sequence, "Chances");
			if (node != null)
			{
				Sequence sequence2 = node as Sequence;
				if (sequence2 == null)
				{
					return;
				}
				{
					foreach (Node item in sequence2.nodesInside)
					{
						Mapping mapping = item as Mapping;
						if (mapping == null)
						{
							continue;
						}
						foreach (Node item2 in mapping.nodesInside)
						{
							_chances.Add(new Chance(item2));
						}
					}
					return;
				}
			}
			Messenger.Error(string.Format("Failed to parse tactics chances. Chances node not found. Tactics mode: [{0}]", _mode.ToString()), this);
		}

		public double GetFactorValue(ChanceType chanceType, FactorType factorType)
		{
			return _chances.GetFactorValue(chanceType, factorType);
		}

		public double GetChanceLimitValue(ChanceType chanceType)
		{
			return _chances.GetChanceLimitValue(chanceType);
		}

		public double GetChanceBaseValue(ChanceType chanceType)
		{
			return _chances.GetChanceBaseValue(chanceType);
		}

		public List<ChanceType> GetChanceTypesUsed()
		{
			return _chances.GetChanceTypesUsed();
		}

		public AiMode GetAiMode()
		{
			return _mode;
		}

		public int GetDelay()
		{
			return _delay;
		}

		public override string ToString()
		{
			StringWrapper stringWrapper = StringWrapper.Create(this);
			stringWrapper.Wrap("_mode", _mode.ToString());
			stringWrapper.Wrap("_delay", _delay);
			stringWrapper.Wrap(_chances.ToString());
			return stringWrapper.ToString();
		}
	}
}
