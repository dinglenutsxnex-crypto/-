using System;
using System.Collections.Generic;
using SF3.GameModels;
using UnityEngine;

namespace SF3.Moves
{
	[Serializable]
	public struct ModelIntervalAttack
	{
		private IntervalAttack _intervalAttack;

		public int intervalStart
		{
			get
			{
				return _intervalAttack.start;
			}
		}

		public List<Capsule> normalAttackingCapsules { get; private set; }

		public List<Capsule> mirrorAttackingCapsules { get; private set; }

		public ModelIntervalAttack(Model modelState, IntervalAttack intervalValue)
		{
			this = default(ModelIntervalAttack);
			_intervalAttack = intervalValue;
			normalAttackingCapsules = new List<Capsule>();
			mirrorAttackingCapsules = new List<Capsule>();
			string[] attackingParts = _intervalAttack.attackingParts;
			foreach (string text in attackingParts)
			{
				Capsule capsule = modelState.modelComponents.modelCapsules.GetCapsule(text);
				if (capsule != null)
				{
					AddAttackingCapsule(capsule);
				}
				else
				{
					Debug.LogWarning(string.Format("Cant find IntervalAttack capsule with name [{0}]", text));
				}
			}
		}

		public bool Equal(IntervalAttack intervalValue)
		{
			if (intervalValue.start == _intervalAttack.start && intervalValue.finish == _intervalAttack.finish)
			{
				return true;
			}
			return false;
		}

		private void AddAttackingCapsule(Capsule attackingCapsule)
		{
			normalAttackingCapsules.Add(attackingCapsule);
			if (attackingCapsule.leftMirrorCapsule != null)
			{
				mirrorAttackingCapsules.Add(attackingCapsule.leftMirrorCapsule);
			}
			else
			{
				mirrorAttackingCapsules.Add(attackingCapsule);
			}
		}
	}
}
