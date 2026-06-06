using System;
using System.Collections.Generic;
using Nekki.Yaml;
using UnityEngine;

namespace SF3.Moves
{
	[Serializable]
	public class Tactics
	{
		[SerializeField]
		private int _mathWeight;

		[SerializeField]
		private List<IConditionEqual> _conditions;

		public int mathWeight
		{
			get
			{
				return _mathWeight;
			}
		}

		public List<IConditionEqual> conditions
		{
			get
			{
				return _conditions;
			}
		}

		public Tactics(int weightVal)
		{
			SetMathWeight(weightVal);
			_conditions = new List<IConditionEqual>();
		}

		public Tactics()
			: this(0)
		{
		}

		public void SetMathWeight(int mathWeightVal)
		{
			_mathWeight = mathWeightVal;
		}

		public void SetConditions(List<IConditionEqual> conditionsVal)
		{
			_conditions = conditionsVal;
		}

		public static Tactics Parse(Mapping tacticsMapping)
		{
			Tactics tactics = new Tactics();
			Scalar text = tacticsMapping.GetText("MathWeight");
			if (text != null)
			{
				tactics.SetMathWeight(int.Parse(text.text));
			}
			Sequence sequence = tacticsMapping.GetSequence("Conditions");
			if (sequence != null)
			{
				List<IConditionEqual> list = Condition.Parse(sequence);
				tactics.SetConditions(list);
			}
			return tactics;
		}
	}
}
