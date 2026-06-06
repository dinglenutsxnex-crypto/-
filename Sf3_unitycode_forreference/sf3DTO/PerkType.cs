using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public enum PerkType
	{
		[OriginalName("UNKNOWN_PERK_TYPE")]
		UnknownPerkType = 0,
		[OriginalName("PERK")]
		Perk = 1,
		[OriginalName("ENCHANTMENT")]
		Enchantment = 2,
		[OriginalName("MOVE")]
		Move = 3
	}
}
