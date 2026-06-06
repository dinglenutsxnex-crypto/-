using Google.Protobuf.Reflection;

namespace common
{
	public enum AuthType
	{
		[OriginalName("NONE")]
		None = 0,
		[OriginalName("DEVICE")]
		Device = 1,
		[OriginalName("GAME_CENTER")]
		GameCenter = 2,
		[OriginalName("GOOGLE_PLAY")]
		GooglePlay = 3,
		[OriginalName("VK")]
		Vk = 4,
		[OriginalName("FB")]
		Fb = 5
	}
}
