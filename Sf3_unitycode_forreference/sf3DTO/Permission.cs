using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public enum Permission
	{
		[OriginalName("NONE")]
		None = 0,
		[OriginalName("CHEATER")]
		Cheater = 0x40000000
	}
}
