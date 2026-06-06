using Google.Protobuf.Reflection;

namespace common
{
	public enum RequestErrorCode
	{
		[OriginalName("OK")]
		Ok = 0,
		[OriginalName("ERROR")]
		Error = 1,
		[OriginalName("INVALID_SESSION_STATUS")]
		InvalidSessionStatus = 100,
		[OriginalName("SESSION_ALREADY_START_LOGIN")]
		SessionAlreadyStartLogin = 200,
		[OriginalName("PLAYER_ALREADY_START_LOGIN")]
		PlayerAlreadyStartLogin = 201,
		[OriginalName("INVALID_CLIENT_SERVER_PROTOCOL_VERSION")]
		InvalidClientServerProtocolVersion = 202,
		[OriginalName("INVALID_LOGIN_OR_PASSWORD")]
		InvalidLoginOrPassword = 203,
		[OriginalName("INVALID_SECURITY_INFO")]
		InvalidSecurityInfo = 204,
		[OriginalName("MAX_VALUE")]
		MaxValue = 9999
	}
}
