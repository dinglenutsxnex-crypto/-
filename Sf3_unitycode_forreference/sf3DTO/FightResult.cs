using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public enum FightResult
	{
		[OriginalName("UNKNOWN_FIGHT_RESULT")]
		UnknownFightResult = 0,
		[OriginalName("WIN")]
		Win = 1,
		[OriginalName("LOSS")]
		Loss = 2,
		[OriginalName("SURRENDER")]
		Surrender = 3
	}
}
