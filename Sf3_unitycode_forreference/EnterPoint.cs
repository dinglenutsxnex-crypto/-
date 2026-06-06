using System;
using System.Collections;
using Nekki.Core;
using Nekki.GamingService;
using Nekki.Utils;
using SF3;
using SF3.Audio;
using SF3.BattleUtils;
using SF3.Items;
using SF3.KeyPressInfo;
using SF3.Moves;
using SF3.Settings;
using SF3.UserData;
using SF3.Utils;
using UnityEngine;

public class EnterPoint : GameInit
{
	private bool _networkInitFinished;

	public override void Init(params Action[] actions)
	{
		Subscribe(actions);
		Subscribe(StartGame);
		Initialize();
	}

	public void StartGame()
	{
		Routiner.Go(StartGameCoroutine());
	}

	private IEnumerator StartGameCoroutine()
	{
		AdvLog.Init();
		BootLogger.Init();
		TimerNode.Clear();
		TimerNode.SetParent(new TimerNode("EnterPoint"));
		using (new TimerNode("TextureUtils", "EnterPoint"))
		{
			TexturesUtils.Init();
		}
		using (TimerNode timerNode2 = new TimerNode("Wait root and loadscreen", "EnterPoint"))
		{
			yield return new WaitUntil(() => NekkiUIRootModules.Instance != null);
			yield return new WaitUntil(() => LoadScreen.Instance != null);
		}
		bool bundlesInited;
		using (new TimerNode("ShowLoader & InitGlobalTimer", "EnterPoint"))
		{
			Execute(delegate
			{
				ConfigsSourceResolver.Init(NetworkConfigManager.ProxyObject);
			}, "ConfigsSourceResolver.Init");
			Execute(delegate
			{
				LoadScreen.ShowLoader(null, 0f, true);
			}, "LoadScreen.Instance.Show");
			Execute(delegate
			{
				GlobalTimer.Init();
			}, "GlobalTimer.Init");
			bundlesInited = false;
			BundlesController.Instance.OnSuccessfulBundles += delegate
			{
				bundlesInited = true;
			};
		}
		using (new TimerNode("Userdata,Config,Internal", "EnterPoint"))
		{
			Execute(UserDataController.Create, "UserDataController.Create();");
			Execute(NetworkConfigManager.Init, "NetworkConfigManager.Init");
			Execute(InternalSettingsSF3.Init, "InternalSettingsSF3.Init");
		}
		using (TimerNode timerNode5 = new TimerNode("JS init", "EnterPoint"))
		{
			bool jsInited = false;
			Execute(delegate
			{
				JS.Instance.InitializeGameScripts(delegate
				{
					jsInited = true;
				});
			}, "JS.Init");
			yield return new WaitUntil(() => jsInited);
		}
		using (new TimerNode("Triggers", "EnterPoint"))
		{
			Execute(GameSettings.Initialize, "GameSettings.Initialize");
			Execute(Condition.Init, "Condition.Init");
			Execute(TriggerEvent.Init, "TriggerEvent.Init");
			Execute(TriggerAction.Init, "TriggerAction.Init");
			Execute(ItemsManager.Init, "ItemsManager.Init");
			Execute(FightSettings.Init, "FightSettings.Init");
			Execute(MovesController.Init, "MovesController.Init");
			Execute(UserDataController.InitPlayer, "UserDataController.InitPlayerFromServer");
		}
		NetworkInitializer.current.onInitFinished += delegate
		{
			_networkInitFinished = true;
		};
		using (TimerNode timerNode7 = new TimerNode("Auth & analitics", "EnterPoint"))
		{
			Execute(delegate
			{
				GamingService.Instance.authorize(delegate
				{
					if (NetworkConnection.shouldAutoConnect)
					{
						NetworkInitializer.current.Init();
						DisconnectOnExit.Init(NetworkConnection.current);
						Routiner.AddUpdate(NetworkConnection.current.UpdateNetwork);
					}
				});
			}, "GamingService.Instance.authorize");
			Execute(Analytics.current.Init, "Analytics.current.Init");
			yield return new WaitUntil(() => _networkInitFinished);
			yield return new WaitUntil(() => bundlesInited);
		}
		using (new TimerNode("Quality, localisation etc.", "EnterPoint"))
		{
			Execute(BaseModuleController.Init, "ModuleController.Init");
			Execute(QuestController.Instance.Init, "QuestController.Instance.Init");
			Execute(TacticsSettings.Init, "TacticsSettings.Init");
			Execute(AudioManager.Initialize, "AudioManager.Initialize");
			Execute(QualityManager.Init, "QualityManager.Init");
			Execute(BattleKey.Init, "BattleKey.Init");
			Execute(TimeManager.Instance.Init, "TimeManager.Instance.Init");
			Execute(delegate
			{
				LocalizationInitializer.Init(SystemLanguage.English, SystemLanguage.English);
			}, "LocalizationInitializer.Init");
			SF3BattleUtils.Initialize();
			Execute(Sandbox.Init, "Sandbox.Init");
			Execute(SceneManager.CreateObject, "SceneManager.CreateObject");
		}
		TimerNode.LogHierarchy();
		TimerNode.Clear();
		QuestController.Instance.ThrowEvent(ETriggerEvents.QEVENT_SESSION_START);
		SF3DeviceInfoLogger.instance.SendLog();
	}

	private void Execute(Action bootStep, string stepName)
	{
		if (GameInit.CanContinueInit)
		{
			BootLogger.Call(bootStep, stepName);
		}
		else
		{
			Debug.LogWarning("Stopped Init");
		}
	}
}
