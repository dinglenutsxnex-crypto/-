using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public enum Rarity
	{
		[OriginalName("UNKNOWN_RARITY")]
		UnknownRarity = 0,
		[OriginalName("COMMON")]
		Common = 1,
		[OriginalName("RARE")]
		Rare = 2,
		[OriginalName("EPIC")]
		Epic = 3,
		[OriginalName("LEGENDARY")]
		Legendary = 4
	}
}
