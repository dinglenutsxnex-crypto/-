using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public enum Gender
	{
		[OriginalName("UNKNOWN_GENDER")]
		UnknownGender = 0,
		[OriginalName("MALE")]
		Male = 1,
		[OriginalName("FEMALE")]
		Female = 2
	}
}
