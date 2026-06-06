using System;
using System.Collections.Generic;
using Nekki.Yaml;
using SF3.Settings;

namespace SF3.Moves
{
	[Serializable]
	public class IntervalBlockable : IntervalAnimation
	{
		public int distance { get; private set; }

		public List<string> names { get; private set; }

		public IntervalAttack intervalAttack { get; private set; }

		public IntervalBlockable(IntervalAttack attackIntervalValue)
			: base(EIntervalsType.INTERVAL_BLOCKABLE)
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
			Sequence sequence = intervalNode.GetSequence("Name");
			if (sequence != null)
			{
				List<string> list = new List<string>();
				foreach (Scalar item in sequence.nodesInside)
				{
					list.Add(item.text);
				}
				SetNames(list);
			}
			base.start = IntervalAnimation.GetValidValue(intervalAttack.start - int.Parse(FightSettings.GetEventProperty(ETriggerEvents.EVENT_BLOCK_CHECK, "StartFrames")));
			base.finish = intervalAttack.finish;
			List<IntervalAnimation> list2 = new List<IntervalAnimation>();
			list2.Add(this);
			return list2;
		}

		public void SetDistance(int value)
		{
			distance = value;
		}

		public void SetNames(List<string> namesValue)
		{
			names = namesValue;
		}
	}
}
