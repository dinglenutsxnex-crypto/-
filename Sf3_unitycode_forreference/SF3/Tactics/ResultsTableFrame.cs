using System.Collections.Generic;

namespace SF3.Tactics
{
	public class ResultsTableFrame
	{
		public List<HitRange> ranges = new List<HitRange>();

		public ResultsTableFrame()
		{
		}

		public ResultsTableFrame(ResultsTableFrame table)
		{
			foreach (HitRange range in table.ranges)
			{
				ranges.Add(new HitRange(range.begin, range.end, range.type, range.baseDamage, range.frameIndex));
			}
		}

		public List<HitRange> InsertRange(List<HitRange> insertingRanges)
		{
			if (insertingRanges.Count == 0)
			{
				return new List<HitRange>();
			}
			foreach (HitRange range in ranges)
			{
				List<HitRange> list = new List<HitRange>();
				foreach (HitRange insertingRange in insertingRanges)
				{
					list.AddRange(range.GetSeparatedRanges(insertingRange));
				}
				insertingRanges = list;
			}
			ranges.AddRange(insertingRanges);
			return insertingRanges;
		}

		public List<HitRange> InsertRange(HitRange insertingRange)
		{
			List<HitRange> list = new List<HitRange>();
			list.Add(insertingRange);
			List<HitRange> insertingRanges = list;
			return InsertRange(insertingRanges);
		}

		public HitRange GetRange(float rangePoint)
		{
			foreach (HitRange range in ranges)
			{
				if ((rangePoint >= range.begin && rangePoint <= range.end) || (rangePoint >= range.end && rangePoint <= range.begin))
				{
					return range;
				}
			}
			return null;
		}

		public HitRange GetLastRange()
		{
			HitRange hitRange = null;
			foreach (HitRange range in ranges)
			{
				if (hitRange == null || hitRange.end < range.end)
				{
					hitRange = range;
				}
			}
			return hitRange;
		}

		public HitRange GetFirstRange()
		{
			HitRange hitRange = null;
			foreach (HitRange range in ranges)
			{
				if (hitRange == null || hitRange.begin > range.begin)
				{
					hitRange = range;
				}
			}
			return hitRange;
		}

		public HitRange GetFirstRange(HitRange.Type type)
		{
			HitRange hitRange = null;
			foreach (HitRange range in ranges)
			{
				if (range.type == type && (hitRange == null || hitRange.begin > range.begin))
				{
					hitRange = range;
				}
			}
			return hitRange;
		}

		public void MergeRanges(bool mergeBHitARanges)
		{
			HitRange hitRange = null;
			for (int i = 0; i < ranges.Count; i++)
			{
				HitRange hitRange2 = ranges[i];
				if (hitRange != null && ((mergeBHitARanges && hitRange.type == HitRange.Type.bHitsA && hitRange2.type == HitRange.Type.bHitsA && hitRange.baseDamage == hitRange2.baseDamage) || (hitRange.type != HitRange.Type.bHitsA && hitRange2.type != HitRange.Type.bHitsA)))
				{
					hitRange.end = hitRange2.end;
					ranges.RemoveAt(i);
					i--;
				}
				else
				{
					hitRange = hitRange2;
				}
			}
		}
	}
}
