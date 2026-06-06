using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using Network.core.events;
using SF3.Items;
using UnityEngine;
using sf3DTO;

namespace SF3.UserData
{
	public static class UserDataController
	{
		private static bool _isCreated;

		private static Dictionary<UserDataManagerType, IUserDataManager> _userDataManagers;

		public static bool waitingForRefreshBattles { get; set; }

		public static bool waitingForGenerateShop { get; private set; }

		public static void AddUserDataManager(UserDataManagerType type, IUserDataManager manager)
		{
			if (!_userDataManagers.ContainsKey(type))
			{
				_userDataManagers.Add(type, manager);
			}
		}

		public static void Clear()
		{
			_isCreated = false;
		}

		public static void Create()
		{
			if (!_isCreated)
			{
				_userDataManagers = new Dictionary<UserDataManagerType, IUserDataManager>();
				waitingForRefreshBattles = false;
				waitingForGenerateShop = false;
				UserManager.Create();
				UserBadgesManager.Create();
				BattlesManager.Create();
				UserShopManager.Create();
				NetworkConnection.current.OnPlayerUpdate += Response_OnPlayerUpdate;
				_isCreated = true;
			}
		}

		public static void InitPlayer()
		{
			foreach (IUserDataManager value in _userDataManagers.Values)
			{
				value.Initialize();
			}
			UserBadgesManager.Instance.Initialize();
			JS.Instance.EnumsCompliancer.Clear();
			BundlesController.Instance.LoadBundles();
		}

		public static void UpdatePlayerData(Player playerData)
		{
			foreach (IUserDataManager value in _userDataManagers.Values)
			{
				value.UpdateUserData(playerData);
			}
		}

		public static CreatePlayerRequest GetCreatePlayerRequestData()
		{
			CreatePlayerRequest createPlayerRequest = new CreatePlayerRequest();
			createPlayerRequest.DisplayName = UserManager.UserModelInfo.alias;
			CreatePlayerRequest createPlayerRequest2 = createPlayerRequest;
			Appearance appearance = new Appearance();
			appearance.HeadId = UserManager.GetHeadId();
			appearance.Gender = UserManager.UserModelInfo.gender;
			appearance.HairColor = UserManager.GetHairColor();
			appearance.SkinColor = UserManager.GetSkinColor();
			Appearance appearance2 = appearance;
			createPlayerRequest2.Appearance = appearance2;
			return createPlayerRequest2;
		}

		public static MutableOfflineState GetClientState(long stateId)
		{
			MutableOfflineState mutableOfflineState = new MutableOfflineState();
			mutableOfflineState.Currencies.AddRange(UserManager.GetCurrencies());
			mutableOfflineState.Experience = UserManager.GetExperience();
			mutableOfflineState.Inventory = UserManager.GetInventory();
			mutableOfflineState.Level = UserManager.GetLevel();
			mutableOfflineState.StateId = stateId;
			return mutableOfflineState;
		}

		public static void Send_FinishFight(FightResult fightResultCurr)
		{
			switch (fightResultCurr.currentBattle.GetBattleType())
			{
			case sf3DTO.BattleType.Test:
			case sf3DTO.BattleType.Local:
				break;
			case sf3DTO.BattleType.Brawler:
				BrawlerHelper.SendBrawlerFight(fightResultCurr);
				break;
			default:
				SendOfflineFight(fightResultCurr);
				break;
			}
		}

		public static void SendOfflineFight(FightResult fightResult)
		{
			FinishFightRequest finishFightRequest = new FinishFightRequest();
			finishFightRequest.BattleModelId = fightResult.currentBattle.GetID();
			finishFightRequest.CurrentFightIndex = fightResult.currentFight.fightID;
			finishFightRequest.BattleCounter = fightResult.currentBattleCounter;
			finishFightRequest.WonRounds = fightResult.roundsWon;
			finishFightRequest.FinishTime = NetworkConnection.current.getCurrentServerTime();
			finishFightRequest.Result = fightResult.AsDto();
			finishFightRequest.TotalRounds = fightResult.roundsPlayed;
			FinishFightRequest finishFightRequest2 = finishFightRequest;
			finishFightRequest2.Multipliers.AddRange(fightResult.GetRewardMultipliers());
			NetworkConnection.SendOffline("finish_fight", finishFightRequest2);
			RefreshBattlesProcessing.RefreshBattles();
		}

		public static void Send_GenerateShop(float? timeOut = null)
		{
			waitingForGenerateShop = true;
			NetworkConnection.Send("generate_shop", new Empty(), Response_GenerateShop, null, timeOut);
		}

		public static void Send_BuyItem(BuyItemRequest request)
		{
			NetworkConnection.SendOffline("buy_item", request);
		}

		public static void Send_Equip(EquipRequest request)
		{
			NetworkConnection.SendOffline("equip", request);
		}

		public static void Send_InsertPerk(InsertPerkRequest request)
		{
			NetworkConnection.SendOffline("insert_perk", request);
		}

		public static void Send_BuyBooster(BuyBoosterRequest request, float? timeOut = null)
		{
			NetworkConnection.Send("buy_booster", request, Response_BuyBooster, null, timeOut);
		}

		public static void Send_OpenBooster(OpenBoosterRequest request)
		{
			NetworkConnection.SendOffline("open_booster", request);
		}

		public static void Send_SetAppearance(Appearance request, float? timeOut = null)
		{
			NetworkConnection.Send("cheat_set_appearance", request);
		}

		private static void Response_OnPlayerUpdate(ExtendedPlayer extPlayer)
		{
			bool flag = IsNewPlayer(extPlayer, UserManager.GetPlayerID());
			if (flag)
			{
				UserManager.Instance.ClearUser();
				UserManager.SetPlayerID(extPlayer);
			}
			UpdatePlayerData(extPlayer.PrimaryPlayer);
			waitingForRefreshBattles = false;
			waitingForGenerateShop = false;
			if (flag || UserManager.SetABTag(extPlayer.PrimaryPlayer.AbTag))
			{
				GameRestarter.ShowRestartDialog("player_updated");
			}
		}

		private static bool IsNewPlayer(ExtendedPlayer extPlayer, long currentID)
		{
			Player primaryPlayer = extPlayer.PrimaryPlayer;
			long num = ((primaryPlayer != null) ? primaryPlayer.PublicPlayer.ShortPlayer.PlayerId : (-1));
			if (currentID != num)
			{
				Debug.Log("New player id " + num + " is not equal with old " + currentID);
			}
			return currentID != num;
		}

		private static void Response_GenerateShop(NetworkEvent eventData)
		{
			if (eventData.success)
			{
				Shop extensible = eventData.getExtensible<Shop>();
				_userDataManagers[UserDataManagerType.Badges].UpdateUserData(extensible);
				_userDataManagers[UserDataManagerType.Shop].UpdateUserData(extensible);
			}
			waitingForGenerateShop = false;
		}

		private static void Response_BuyBooster(NetworkEvent eventData)
		{
			if (eventData.success)
			{
				BuyBoosterResponse extensible = eventData.getExtensible<BuyBoosterResponse>();
				UserManager.AddItem(SF3.Items.Booster.Create(extensible.Booster), true);
				UserManager.SetCurrencies(extensible.Currency.RepeatedToList());
				_userDataManagers[UserDataManagerType.Badges].UpdateUserData(extensible.Booster);
				ShopManager.Instance.OnBuyItemSuccess();
			}
			else
			{
				ShopManager.Instance.OnBuyItemFailure();
			}
		}

		public static void UpdateDataManager(UserDataManagerType type, BattleData data)
		{
			if (_userDataManagers.ContainsKey(type))
			{
				_userDataManagers[type].UpdateUserData(data);
			}
		}
	}
}
