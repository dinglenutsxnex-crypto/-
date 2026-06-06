using System.Collections.Generic;
using UnityEngine;

namespace SF3.Tactics
{
	public class HitRange
	{
		public enum Type : byte
		{
			none = 0,
			random = 1,
			aHitsB = 2,
			bHitsA = 3,
			collision = 4
		}

		public Type type = Type.aHitsB;

		public float begin;

		public float end;

		public float baseDamage;

		public int frameIndex;

		public HitRange()
		{
		}

		public HitRange(float begin, float end, Type type, float baseDamage, int frameIndex)
			: this()
		{
			this.begin = begin;
			this.end = end;
			this.type = type;
			this.baseDamage = baseDamage;
			this.frameIndex = frameIndex;
		}

		public List<HitRange> GetSeparatedRanges(HitRange range)
		{
			List<HitRange> list = new List<HitRange>();
			if (range.begin > end || range.end < begin)
			{
				list.Add(range);
				return list;
			}
			if (range.begin < begin)
			{
				list.Add(new HitRange(range.begin, begin, range.type, range.baseDamage, range.frameIndex));
			}
			if (range.end > end)
			{
				list.Add(new HitRange(end, range.end, range.type, range.baseDamage, range.frameIndex));
			}
			return list;
		}

		public bool ResolveHitConflictWith(HitRange conflict, out List<HitRange> resolved)
		{
			resolved = new List<HitRange>();
			if (conflict.begin >= end || conflict.end <= begin)
			{
				resolved.Add(this);
				resolved.Add(conflict);
				return false;
			}
			float num = 0f;
			float num2 = 0f;
			if (type == Type.random && conflict.type == Type.random)
			{
				num = Mathf.Min(begin, conflict.begin);
				num2 = Mathf.Max(end, conflict.end);
				resolved.Add(new HitRange(num, num2, Type.random, baseDamage, frameIndex));
				return true;
			}
			if (begin < conflict.begin)
			{
				resolved.Add(new HitRange(begin, conflict.begin, type, baseDamage, frameIndex));
				num = conflict.begin;
			}
			if (begin > conflict.begin)
			{
				resolved.Add(new HitRange(conflict.begin, begin, conflict.type, conflict.baseDamage, conflict.frameIndex));
				num = begin;
			}
			if (end > conflict.end)
			{
				resolved.Add(new HitRange(conflict.end, end, type, baseDamage, frameIndex));
				num2 = conflict.end;
			}
			if (end < conflict.end)
			{
				resolved.Add(new HitRange(end, conflict.end, conflict.type, conflict.baseDamage, conflict.frameIndex));
				num2 = end;
			}
			if (begin == conflict.begin)
			{
				num = begin;
			}
			if (end == conflict.end)
			{
				num2 = end;
			}
			float num3 = Mathf.Min(baseDamage, conflict.baseDamage);
			resolved.Add(new HitRange(num, num2, Type.random, num3, frameIndex));
			return true;
		}

		public override string ToString()
		{
			return begin + " " + end + " " + type;
		}
	}
}
