using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public enum Constants
	{
		[OriginalName("DEPRECATED_CONSTANT")]
		DeprecatedConstant = 0,
		[OriginalName("CURRENT_VERSION")]
		CurrentVersion = 2,
		[OriginalName("MIN_SUPPORTED_VERSION")]
		MinSupportedVersion = 2,
		[OriginalName("MAX_REQUESTED_PLAYER_COUNT")]
		MaxRequestedPlayerCount = 100
	}
}
