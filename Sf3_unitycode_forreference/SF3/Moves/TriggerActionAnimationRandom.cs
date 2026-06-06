using System.Collections.Generic;
using System.Linq;
using Nekki.Yaml;
using SF3.GameModels;
using UnityEngine;

namespace SF3.Moves
{
	public class TriggerActionAnimationRandom : TriggerAction
	{
		public string[] ArrayNames;

		private int _prevID;

		private int _repetition;

		private const int AllowedRep = 0;

		public TriggerActionAnimationRandom(Node yamNode)
			: base(EActionType.ANIMATION_RANDOM, yamNode)
		{
			HashSet<string> hashSet = new HashSet<string>();
			Sequence sequence = BaseMapping.GetSequence("Names");
			if (sequence != null)
			{
				for (int i = 0; i < sequence.nodesInside.Count; i++)
				{
					hashSet.Add(sequence.nodesInside[i].value.ToString().ToLower());
				}
			}
			ArrayNames = hashSet.ToArray();
			_prevID = Random.Range(0, ArrayNames.Length);
			_repetition = 0;
		}

		protected override void ApplyAction(object modelData)
		{
			int num = NextRandom(ArrayNames.Length);
			((Model)modelData).modelAnimation.Play(ArrayNames[num]);
		}

		private int NextRandom(int range)
		{
			int num = Random.Range(0, range);
			if (num == _prevID)
			{
				_repetition++;
				if (_repetition > 0)
				{
					num = ((num < range - 1) ? (num + 1) : 0);
				}
			}
			else
			{
				_repetition = 0;
			}
			_prevID = num;
			return num;
		}
	}
}
