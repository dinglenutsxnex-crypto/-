namespace SF3.Moves
{
	public class SwitchFrame
	{
		public bool IsMirror { get; set; }

		public int Frame { get; set; }

		public SwitchFrame(bool mirror, int frame)
		{
			IsMirror = mirror;
			Frame = frame;
		}
	}
}
