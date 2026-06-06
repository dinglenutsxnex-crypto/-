using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using Network.core.events;
using common;
using sf3DTO;

public class SFCMD
{
	public const string CREATE_PLAYER = "create_player";

	public const string GET_PLAYER = "get_player";

	public const string PROCESS_OFFLINE_BATCH = "process_offline_batch";

	public const string PING = "ping";

	public const string JOIN_ZONE = "join_zone";

	public const string KICK = "kick";

	public const string GENERATE_SHOP = "generate_shop";

	public const string BUY_ITEM = "buy_item";

	public const string EQUIP = "equip";

	public const string INSERT_PERK = "insert_perk";

	public const string OPEN_BOOSTER = "open_booster";

	public const string BUY_BOOSTER = "buy_booster";

	public const string FINISH_FIGHT = "finish_fight";

	public const string REFRESH_BATTLES = "refresh_battles";

	public const string SET_CHAPTER = "set_chapter";

	public const string LOG = "log";

	public const string BRAWLER_START = "brawler_start";

	public const string BRAWLER_FINISH = "brawler_finish";

	public const string CHEAT_RESET_PLAYER = "cheat_reset_player";

	public const string CHEAT_ADD_ITEM = "cheat_add_item";

	public const string CHEAT_ADD_PERK = "cheat_add_perk";

	public const string CHEAT_RESET_PERKS = "cheat_reset_perks";

	public const string CHEAT_SET_CURRENCY = "cheat_set_currency";

	public const string CHEAT_SET_EXPERIENCE = "cheat_set_experience";

	public const string CHEAT_GENERATE_BATTLE = "cheat_generate_battle";

	public const string CHEAT_GET_PLAYER = "cheat_get_player";

	public const string CHEAT_SET_PLAYER = "cheat_set_player";

	public const string CHEAT_SET_APPEARANCE = "cheat_set_appearance";

	public const string CHEAT_SET_ALL_ITEMS = "cheat_set_all_items";

	public const string REFRESH_CONFIG_EVENT = "refresh_config_event";

	public static void Init()
	{
		NetworkEventManager.Add("create_player", typeof(CreatePlayerRequest), typeof(ExtendedPlayer), new List<string>(), new List<string> { "InvalidDisplayName" });
		NetworkEventManager.Add("get_player", typeof(GetPlayerRequest), typeof(ExtendedPlayer), new List<string>(), new List<string> { "RecoverableOfflineRequestError", "UnrecoverableOfflineRequestError" });
		NetworkEventManager.Add("process_offline_batch", typeof(OfflineRequestBatch), typeof(Empty), new List<string> { "state_change" }, new List<string> { "RecoverableOfflineRequestError", "UnrecoverableOfflineRequestError" });
		NetworkEventManager.Add("ping", typeof(common.Timestamp), typeof(PingResponse), new List<string>(), new List<string>());
		NetworkEventManager.Add("join_zone", typeof(JoinZoneEvent));
		NetworkEventManager.Add("kick", typeof(KickEvent));
		NetworkEventManager.Add("generate_shop", typeof(Empty), typeof(Shop), new List<string> { "state_change" }, new List<string>());
		NetworkEventManager.Add("buy_item", typeof(BuyItemRequest), typeof(Empty), new List<string> { "offline" }, new List<string>());
		NetworkEventManager.Add("equip", typeof(EquipRequest), typeof(Empty), new List<string> { "offline" }, new List<string>());
		NetworkEventManager.Add("insert_perk", typeof(InsertPerkRequest), typeof(Empty), new List<string> { "offline" }, new List<string>());
		NetworkEventManager.Add("open_booster", typeof(OpenBoosterRequest), typeof(Empty), new List<string> { "offline" }, new List<string>());
		NetworkEventManager.Add("buy_booster", typeof(BuyBoosterRequest), typeof(BuyBoosterResponse), new List<string> { "state_change" }, new List<string>());
		NetworkEventManager.Add("finish_fight", typeof(FinishFightRequest), typeof(Empty), new List<string> { "offline" }, new List<string>());
		NetworkEventManager.Add("refresh_battles", typeof(Empty), typeof(BattleData), new List<string> { "state_change" }, new List<string>());
		NetworkEventManager.Add("set_chapter", typeof(Int32Value), typeof(Player), new List<string> { "state_change" }, new List<string>());
		NetworkEventManager.Add("log", typeof(LogRequest), typeof(Empty), new List<string>(), new List<string> { "InvalidLogEvent" });
		NetworkEventManager.Add("brawler_start", typeof(Empty), typeof(BrawlerFight), new List<string>(), new List<string> { "BrawlerCannotFindEnemy", "BrawlerAlreadyStarted" });
		NetworkEventManager.Add("brawler_finish", typeof(BrawlerFinishRequest), typeof(BrawlerFinish), new List<string> { "state_change" }, new List<string> { "BrawlerFightMissed", "BrawlerInvalidTotalRounds", "BrawlerInvalidEnemy" });
		NetworkEventManager.Add("cheat_reset_player", typeof(Empty), typeof(Empty), new List<string>(), new List<string>());
		NetworkEventManager.Add("cheat_add_item", typeof(Item), typeof(Item), new List<string> { "state_change" }, new List<string>());
		NetworkEventManager.Add("cheat_add_perk", typeof(Perk), typeof(Perk), new List<string> { "state_change" }, new List<string>());
		NetworkEventManager.Add("cheat_reset_perks", typeof(Empty), typeof(Empty), new List<string> { "state_change" }, new List<string>());
		NetworkEventManager.Add("cheat_set_currency", typeof(Currency), typeof(Empty), new List<string> { "state_change" }, new List<string>());
		NetworkEventManager.Add("cheat_set_experience", typeof(Int64Value), typeof(Player), new List<string> { "state_change" }, new List<string>());
		NetworkEventManager.Add("cheat_generate_battle", typeof(Int32Value), typeof(Battle), new List<string>(), new List<string>());
		NetworkEventManager.Add("cheat_get_player", typeof(Empty), typeof(StringValue), new List<string>(), new List<string>());
		NetworkEventManager.Add("cheat_set_player", typeof(StringValue), typeof(Empty), new List<string> { "state_change" }, new List<string>());
		NetworkEventManager.Add("cheat_set_appearance", typeof(Appearance), typeof(Empty), new List<string> { "state_change" }, new List<string>());
		NetworkEventManager.Add("cheat_set_all_items", typeof(DoubleValue), typeof(Player), new List<string> { "state_change" }, new List<string>());
		NetworkEventManager.Add("refresh_config_event", typeof(StringValue));
	}
}
