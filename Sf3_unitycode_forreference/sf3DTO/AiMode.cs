using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public enum AiMode
	{
		[OriginalName("UNKNOWN_AI_MODE")]
		UnknownAiMode = 0,
		[OriginalName("REGULAR_MODE")]
		RegularMode = 1,
		[OriginalName("NONE_MODE")]
		NoneMode = 2,
		[OriginalName("SENSEI_MODE")]
		SenseiMode = 3,
		[OriginalName("DOJO_MODE")]
		DojoMode = 4
	}
}
