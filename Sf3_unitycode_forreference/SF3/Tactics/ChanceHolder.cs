using System.Collections.Generic;
using System.Linq;

namespace SF3.Tactics
{
	public class ChanceHolder
	{
		private readonly Dictionary<ChanceType, Chance> _chances;

		public ChanceHolder()
		{
			_chances = new Dictionary<ChanceType, Chance>();
		}

		public void Add(Chance chance)
		{
			if (!_chances.ContainsKey(chance.GetChanceType()))
			{
				_chances.Add(chance.GetChanceType(), chance);
			}
			else
			{
				Messenger.Error(string.Format("ChanceHolder already contains chance of type: [{0}]. No new chance of this type will be added.", chance.GetChanceType()), this);
			}
		}

		public double GetFactorValue(ChanceType chanceType, FactorType factorType)
		{
			return (!_chances.ContainsKey(chanceType)) ? ((double)Chance.DefaultFactorValue) : _chances[chanceType].GetFactorValue(factorType);
		}

		public double GetChanceLimitValue(ChanceType chanceType)
		{
			return (!_chances.ContainsKey(chanceType)) ? Chance.DefaultLimitValue : _chances[chanceType].GetLimitValue();
		}

		public double GetChanceBaseValue(ChanceType chanceType)
		{
			return (!_chances.ContainsKey(chanceType)) ? Chance.DefaultBaseValue : _chances[chanceType].GetBaseValue();
		}

		public List<ChanceType> GetChanceTypesUsed()
		{
			return _chances.Keys.ToList();
		}

		public override string ToString()
		{
			StringWrapper stringWrapper = StringWrapper.Create(this);
			if (_chances.Count == 0)
			{
				stringWrapper.Wrap("<No chances. Holder is empty :(>");
			}
			else
			{
				foreach (KeyValuePair<ChanceType, Chance> chance in _chances)
				{
					stringWrapper.Wrap(chance.Value.ToString());
				}
			}
			return stringWrapper.ToString();
		}
	}
}
