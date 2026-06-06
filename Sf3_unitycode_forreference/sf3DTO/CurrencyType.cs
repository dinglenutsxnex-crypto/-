using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public enum CurrencyType
	{
		[OriginalName("UNKNOWN_CURRENCY_TYPE")]
		UnknownCurrencyType = 0,
		[OriginalName("COIN")]
		Coin = 1,
		[OriginalName("BONUS")]
		Bonus = 2,
		[OriginalName("SHADOW")]
		Shadow = 3
	}
}
