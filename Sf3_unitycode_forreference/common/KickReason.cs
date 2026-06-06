using Google.Protobuf.Reflection;

namespace common
{
	public enum KickReason
	{
		[OriginalName("ANOTHER_SESSION_LOGIN")]
		AnotherSessionLogin = 0,
		[OriginalName("BAN")]
		Ban = 1,
		[OriginalName("INVALID_VERSION")]
		InvalidVersion = 2,
		[OriginalName("LOCK_FOR_MAINTENANCE")]
		LockForMaintenance = 3
	}
}
