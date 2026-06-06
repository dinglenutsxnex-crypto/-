using System.Collections.Generic;
using System.Linq;

namespace SF3.Tactics
{
	public class ResultsTable
	{
		public const string COMPLIANCE_TABLE_NAME = "COMPLIANCE.bytes";

		public const string REVERSE_RESULT_PREFIX = "REVERSE_";

		public string name { get; private set; }

		public Dictionary<string, ResultTableElement> elements { get; private set; }

		public int midFrames { get; private set; }

		public ResultsTable(string name, int midFrames)
		{
			this.name = name;
			this.midFrames = midFrames;
			elements = new Dictionary<string, ResultTableElement>();
		}

		private float GetDistance(string animationName)
		{
			float num = 0f;
			foreach (ResultsTableFrame frame in elements[animationName].frames)
			{
				foreach (HitRange range in frame.ranges)
				{
					if (range.end > num)
					{
						num = range.end;
					}
				}
			}
			return num;
		}

		public void Add(string animationName, ResultTableElement row)
		{
			elements[animationName] = row;
		}

		public int GetSize()
		{
			return elements.Values.Sum((ResultTableElement element) => element.frames.Count);
		}
	}
}
