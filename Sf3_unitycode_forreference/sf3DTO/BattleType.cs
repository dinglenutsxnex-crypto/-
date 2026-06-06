using Google.Protobuf.Reflection;

namespace sf3DTO
{
	public enum BattleType
	{
		[OriginalName("UNKNOWN_BATTLE_TYPE")]
		UnknownBattleType = 0,
		[OriginalName("MAIN")]
		Main = 1,
		[OriginalName("SIDE")]
		Side = 2,
		[OriginalName("SURVIVAL")]
		Survival = 3,
		[OriginalName("DAILY")]
		Daily = 4,
		[OriginalName("MISSION")]
		Mission = 5,
		[OriginalName("BRAWLER")]
		Brawler = 11,
		[OriginalName("TEST")]
		Test = 100,
		[OriginalName("LOCAL")]
		Local = 200,
		[OriginalName("DOJO")]
		Dojo = 300
	}
}
