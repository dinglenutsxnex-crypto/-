using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public enum RequestErrorCode
	{
		[OriginalName("DEPRECATED_REQUEST_ERROR_CODE")]
		DeprecatedRequestErrorCode = 0,
		[OriginalName("PLAYER_NOT_FOUND")]
		PlayerNotFound = 10000,
		[OriginalName("INVALID_CONFIG_VERSION")]
		InvalidConfigVersion = 10001,
		[OriginalName("INVALID_DISPLAY_NAME")]
		InvalidDisplayName = 20000,
		[OriginalName("RECOVERABLE_OFFLINE_REQUEST_ERROR")]
		RecoverableOfflineRequestError = 30000,
		[OriginalName("UNRECOVERABLE_OFFLINE_REQUEST_ERROR")]
		UnrecoverableOfflineRequestError = 30001,
		[OriginalName("INVALID_LOG_EVENT")]
		InvalidLogEvent = 40000,
		[OriginalName("BRAWLER_CANNOT_FIND_ENEMY")]
		BrawlerCannotFindEnemy = 50000,
		[OriginalName("BRAWLER_FIGHT_MISSED")]
		BrawlerFightMissed = 50001,
		[OriginalName("BRAWLER_INVALID_ENEMY")]
		BrawlerInvalidEnemy = 50002,
		[OriginalName("BRAWLER_ALREADY_STARTED")]
		BrawlerAlreadyStarted = 50003,
		[OriginalName("BRAWLER_INVALID_TOTAL_ROUNDS")]
		BrawlerInvalidTotalRounds = 50004,
		[OriginalName("CBT_INVALID_EMAIL")]
		CbtInvalidEmail = 60000
	}
}
