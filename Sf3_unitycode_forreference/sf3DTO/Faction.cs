using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public enum Faction
	{
		[OriginalName("UNKNOWN_FACTION")]
		UnknownFaction = 0,
		[OriginalName("LEGION")]
		Legion = 1,
		[OriginalName("HERALDS")]
		Heralds = 2,
		[OriginalName("DYNASTY")]
		Dynasty = 3
	}
}
