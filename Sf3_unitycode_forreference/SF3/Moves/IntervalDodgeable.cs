using System;
using System.Collections.Generic;
using Nekki.Yaml;

namespace SF3.Moves
{
	[Serializable]
	public class IntervalDodgeable : IntervalAnimation
	{
		public int distance { get; private set; }

		public IntervalAttack intervalAttack { get; private set; }

		public IntervalDodgeable(IntervalAttack attackIntervalValue)
			: base(EIntervalsType.INTERVAL_DODGEABLE)
		{
			distance = 0;
			intervalAttack = attackIntervalValue;
		}

		public override List<IntervalAnimation> Parse(Mapping intervalNode)
		{
			base.Parse(intervalNode);
			Scalar text = intervalNode.GetText("Distance");
			if (text != null)
			{
				SetDistance(int.Parse(text.text));
			}
			List<IntervalAnimation> list = new List<IntervalAnimation>();
			list.Add(this);
			return list;
		}

		public void SetDistance(int value)
		{
			distance = value;
		}
	}
}
