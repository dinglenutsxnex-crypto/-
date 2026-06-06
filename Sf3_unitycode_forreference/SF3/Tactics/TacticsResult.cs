using System.Runtime.InteropServices;
using SF3.Moves;

namespace SF3.Tactics
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct TacticsResult
	{
		public InfoAnimation provocation { get; private set; }

		public string reactionName { get; private set; }

		public float distance { get; private set; }

		public float startFrame { get; private set; }

		public float hitFrame { get; private set; }

		public TacticsResult(InfoAnimation provocation, string reactionName, float distance, float startFrame, float hitFrame)
		{
			this.provocation = provocation;
			this.reactionName = reactionName;
			this.distance = distance;
			this.startFrame = startFrame;
			this.hitFrame = hitFrame;
		}
	}
}
