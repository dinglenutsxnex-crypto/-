using System;
using Google.Protobuf.WellKnownTypes;
using Network.core.events;
using Org.BouncyCastle.Utilities.Encoders;
using SF3.UserData;
using UnityEngine;
using sf3DTO;

namespace SF3
{
	public static class BrawlerHelper
	{
		private static BrawlerEnemy currentEnemy;

		private const string request_tag = "BRAWLER_REQUEST";

		public static void GetNewBrawlerFightAsync(Action<BrawlerFight> OnFightSearchFinished)
		{
			if (currentEnemy != null)
			{
				Debug.LogError("Error: Requesting a new fight while we still have a currentEnemy");
				currentEnemy = null;
			}
			NetworkConnection.Send("brawler_start", new Empty(), delegate(NetworkEvent e)
			{
				BrawlerFight brawlerFight = null;
				HandleBrawlerStartErrors(e);
				if (e.success)
				{
					brawlerFight = e.getExtensible<BrawlerFight>();
					currentEnemy = brawlerFight.Enemy;
				}
				OnFightSearchFinished.InvokeSafe(brawlerFight);
			});
		}

		private static void HandleBrawlerStartErrors(NetworkEvent e)
		{
			e.HandleErrorAsDialog(RequestErrorCode.BrawlerCannotFindEnemy, RequestErrorCode.BrawlerAlreadyStarted);
		}

		public static void SendBrawlerFight(FightResult fightResult)
		{
			BrawlerFinishRequest brawlerFinishRequest = new BrawlerFinishRequest();
			brawlerFinishRequest.Enemy = currentEnemy;
			brawlerFinishRequest.Result = fightResult.AsDto();
			brawlerFinishRequest.TotalRounds = fightResult.roundsPlayed;
			BrawlerFinishRequest brawlerFinishRequest2 = brawlerFinishRequest;
			brawlerFinishRequest2.Multipliers.AddRange(fightResult.GetRewardMultipliers());
			SaveSentFight(brawlerFinishRequest2);
			if (NetworkConnection.current.IsNetworkEstablished())
			{
				NetworkConnection.Send("brawler_finish", brawlerFinishRequest2, OnBrawlerSent);
			}
			currentEnemy = null;
		}

		private static void OnBrawlerSent(NetworkEvent e)
		{
			e.HandleErrorAsDialog(RequestErrorCode.BrawlerFightMissed, RequestErrorCode.BrawlerInvalidTotalRounds, RequestErrorCode.BrawlerInvalidEnemy);
			if (!e.success)
			{
				return;
			}
			ClearSentFight();
			BrawlerFinish extensible = e.getExtensible<BrawlerFinish>();
			foreach (Currency currency in extensible.Reward.Currencies)
			{
				UserManager.AddCurrency(currency);
			}
			UserManager.ApplyRatingDelta(extensible.RatingDelta);
		}

		private static void SaveSentFight(BrawlerFinishRequest request)
		{
			PlayerPrefs.SetString("BRAWLER_REQUEST", Base64.ToBase64String(request.ToBinary()));
		}

		public static BrawlerFinishRequest LoadSentFight()
		{
			if (!PlayerPrefs.HasKey("BRAWLER_REQUEST"))
			{
				return null;
			}
			try
			{
				return Base64.Decode(PlayerPrefs.GetString("BRAWLER_REQUEST")).FromBinary<BrawlerFinishRequest>();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return null;
			}
		}

		private static void ClearSentFight()
		{
			PlayerPrefs.DeleteKey("BRAWLER_REQUEST");
		}

		internal static void ResendBrawlerFinish(BrawlerFight brawler_fight)
		{
			NetworkConnection.current.OnNetworkEstablished += ResendSavedBrawler;
		}

		private static void ResendSavedBrawler()
		{
			NetworkConnection.current.OnNetworkEstablished -= ResendSavedBrawler;
			BrawlerFinishRequest brawlerFinishRequest = LoadSentFight();
			if (brawlerFinishRequest == null)
			{
				BrawlerFinishRequest brawlerFinishRequest2 = new BrawlerFinishRequest();
				brawlerFinishRequest2.Result = sf3DTO.FightResult.Surrender;
				brawlerFinishRequest2.TotalRounds = 0;
				brawlerFinishRequest = brawlerFinishRequest2;
			}
			NetworkConnection.Send("brawler_finish", brawlerFinishRequest, OnBrawlerSent);
		}
	}
}
