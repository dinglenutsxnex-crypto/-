using Google.Protobuf.Reflection;

namespace common
{
	public enum SortOrder
	{
		[OriginalName("ASC")]
		Asc = 0,
		[OriginalName("DESC")]
		Desc = 1
	}
}
