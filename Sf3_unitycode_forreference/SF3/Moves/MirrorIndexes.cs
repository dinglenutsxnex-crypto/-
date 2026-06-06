namespace SF3.Moves
{
	public class MirrorIndexes
	{
		public int rightIndex { get; private set; }

		public int leftIndex { get; private set; }

		public MirrorIndexes(int rightIndexVal, int leftIndexVal)
		{
			rightIndex = rightIndexVal;
			leftIndex = leftIndexVal;
		}
	}
}
