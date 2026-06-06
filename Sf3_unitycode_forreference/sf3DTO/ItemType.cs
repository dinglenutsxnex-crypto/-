using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public enum ItemType
	{
		[OriginalName("UNKNOWN_ITEM_TYPE")]
		UnknownItemType = 0,
		[OriginalName("HELMET")]
		Helmet = 1,
		[OriginalName("ARMOR")]
		Armor = 2,
		[OriginalName("WEAPON")]
		Weapon = 3,
		[OriginalName("RANGED")]
		Ranged = 4,
		[OriginalName("MAGIC")]
		Magic = 5
	}
}
