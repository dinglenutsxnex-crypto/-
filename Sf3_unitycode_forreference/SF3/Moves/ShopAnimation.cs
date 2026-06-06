using System;

namespace SF3.Moves
{
	[Serializable]
	public class ShopAnimation
	{
		public bool isExists { get; set; }

		public bool runOnStart { get; set; }

		public string animationName { get; set; }

		public ShopAnimation()
		{
			animationName = string.Empty;
		}
	}
}
