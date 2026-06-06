using System;
using Google.Protobuf.WellKnownTypes;
using Network.core.events;
using UnityEngine;
using sf3DTO;

namespace SF3.UserData
{
	public static class RefreshBattlesProcessing
	{
		public static void RefreshBattles(Action<NetworkEvent> callBack = null, float? timeOut = null)
		{
			UserDataController.waitingForRefreshBattles = true;
			callBack = (Action<NetworkEvent>)Delegate.Combine(callBack, (Action<NetworkEvent>)delegate
			{
				UserDataController.waitingForRefreshBattles = false;
			});
			if (BattlesManager.NeedSendChapter())
			{
				Send_SetChapter(callBack, timeOut);
			}
			else
			{
				Send_RefreshBattles(callBack, timeOut);
			}
		}

		private static void Send_SetChapter(Int32Value request, Action<NetworkEvent> callback = null, float? timeOut = null)
		{
			Debug.Log(string.Format("<color=red>Try to change chapter from [{0}] to [{1}]</color>", UserManager.Instance.GetCurrentChapter(), request.Value));
			callback = (Action<NetworkEvent>)Delegate.Combine(callback, new Action<NetworkEvent>(Response_SetChapter));
			NetworkConnection.Send("set_chapter", request, callback, null, timeOut);
		}

		private static void Response_SetChapter(NetworkEvent eventData)
		{
			if (eventData.success)
			{
				UserDataController.UpdatePlayerData(eventData.getExtensible<Player>());
				Debug.Log(string.Format("Successfully switched to chapter <b><color=red>[{0}]</color></b>", UserManager.Instance.GetCurrentChapter()));
			}
		}

		private static void Send_SetChapter(Action<NetworkEvent> callBack = null, float? timeOut = null)
		{
			int? nextChapter = BattlesManager.instance.GetNextChapter();
			Int32Value int32Value = new Int32Value();
			int32Value.Value = nextChapter.Value;
			Int32Value request = int32Value;
			Send_SetChapter(request, callBack, timeOut);
		}

		private static void Send_RefreshBattles(Action<NetworkEvent> callBack = null, float? timeOut = null)
		{
			if (FightController.Instance != null && FightController.Instance.FightStage != FightController.EFightStage.FightEnd && !FightController.Instance.IsDojo())
			{
				callBack(NetworkEvent.createCancelEvent("refresh_battles", null));
				return;
			}
			callBack = (Action<NetworkEvent>)Delegate.Combine(callBack, new Action<NetworkEvent>(Response_RefreshBattles));
			NetworkConnection.Send("refresh_battles", new Empty(), callBack, null, timeOut);
		}

		private static void Response_RefreshBattles(NetworkEvent eventData)
		{
			if (eventData.success)
			{
				BattleData extensible = eventData.getExtensible<BattleData>();
				UserDataController.UpdateDataManager(UserDataManagerType.Badges, extensible);
				UserDataController.UpdateDataManager(UserDataManagerType.Battles, extensible);
			}
		}
	}
}
