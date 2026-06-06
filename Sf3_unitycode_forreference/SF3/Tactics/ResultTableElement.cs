using System.Collections.Generic;

namespace SF3.Tactics
{
	public class ResultTableElement
	{
		public List<ResultsTableFrame> frames { get; private set; }

		public int midFrames { get; private set; }

		public ResultTableElement(List<ResultsTableFrame> frames, int midFrames)
		{
			this.frames = frames;
			this.midFrames = midFrames;
		}
	}
}
