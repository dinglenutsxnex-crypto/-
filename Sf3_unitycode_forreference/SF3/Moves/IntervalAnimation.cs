using System;
using System.Collections.Generic;
using Nekki.Yaml;
using UnityEngine;

namespace SF3.Moves
{
	[Serializable]
	public class IntervalAnimation
	{
		private static Dictionary<string, EIntervalsType> intervalNamesCompliance;

		public string name { get; protected set; }

		public EIntervalsType type { get; protected set; }

		public int start { get; protected set; }

		public int finish { get; protected set; }

		public IntervalAnimation(EIntervalsType typeInterval = EIntervalsType.INTERVAL_NONE)
		{
			name = string.Empty;
			type = typeInterval;
			start = 0;
			finish = int.MaxValue;
		}

		static IntervalAnimation()
		{
			intervalNamesCompliance = new Dictionary<string, EIntervalsType>
			{
				{
					"Uninterrupt",
					EIntervalsType.INTERVAL_UNINTERRUPT
				},
				{
					"SelfUninterrupt",
					EIntervalsType.INTERVAL_SELF_UNINTERRUPT
				},
				{
					"Attack",
					EIntervalsType.INTERVAL_ATTACK
				},
				{
					"NoRepulsion",
					EIntervalsType.INTERVAL_NO_REPULSION
				},
				{
					"NoWallRepulsion",
					EIntervalsType.INTERVAL_NO_WALL_REPULSION
				},
				{
					"Dodgeable",
					EIntervalsType.INTERVAL_DODGEABLE
				},
				{
					"Blockable",
					EIntervalsType.INTERVAL_BLOCKABLE
				},
				{
					"Slowmo",
					EIntervalsType.INTERVAL_SLOWMOTION
				},
				{
					"Invincible",
					EIntervalsType.INTERVAL_INVINCIBLE
				},
				{
					"Transitional",
					EIntervalsType.INTERVAL_TRANSITIONAL
				},
				{
					"DissolveWeapon",
					EIntervalsType.INTERVAL_DISSOLVE_WEAPON
				}
			};
		}

		public void SetName(string textName)
		{
			name = textName;
			type = GetTypeByName(name);
		}

		public void SetStart(int startVal)
		{
			start = GetValidValue(startVal);
		}

		public void SetFinish(int finishVal)
		{
			finish = GetValidValue(finishVal);
		}

		public bool IsKeyInInterval(int keyFrame)
		{
			return keyFrame >= start && keyFrame <= finish;
		}

		public static EIntervalsType GetTypeByName(string typeName)
		{
			return intervalNamesCompliance.ContainsKey(typeName) ? intervalNamesCompliance[typeName] : EIntervalsType.INTERVAL_NONE;
		}

		public static int GetValidValue(int toValidateValue)
		{
			return (toValidateValue >= 0) ? toValidateValue : 0;
		}

		public static List<IntervalAnimation> Create(Sequence intervalsNodes)
		{
			List<IntervalAnimation> list = new List<IntervalAnimation>();
			foreach (Mapping item in intervalsNodes.nodesInside)
			{
				Scalar text = item.GetText("Type");
				IntervalAnimation intervalAnimation = ((text == null) ? new IntervalAnimation() : (text.text.Equals("Attack") ? new IntervalAttack() : ((!text.text.Equals("TurnOffRepulsionBones")) ? new IntervalAnimation() : new IntervalExcludeRepulsion())));
				List<IntervalAnimation> list2 = intervalAnimation.Parse(item);
				if (list2.Count > 0)
				{
					list.AddRange(list2);
				}
				else
				{
					Debug.LogWarning("IntervalAnimation is null");
				}
			}
			return list;
		}

		public virtual List<IntervalAnimation> Parse(Mapping intervalNode)
		{
			Scalar scalar = intervalNode.GetText("Name") ?? intervalNode.GetText("Type");
			if (scalar != null)
			{
				SetName(scalar.text);
			}
			scalar = intervalNode.GetText("Start");
			if (scalar != null)
			{
				SetStart(int.Parse(scalar.text) - 1);
			}
			scalar = intervalNode.GetText("End");
			if (scalar != null)
			{
				SetFinish(int.Parse(scalar.text) - 1);
			}
			else
			{
				SetFinish(int.MaxValue);
			}
			List<IntervalAnimation> list = new List<IntervalAnimation>();
			list.Add(this);
			return list;
		}
	}
}
