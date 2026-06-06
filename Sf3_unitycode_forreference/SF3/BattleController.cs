using System;
using System.Collections;
using SF3.Audio;
using SF3.GameModels;
using SF3.Moves;
using SF3.Utils;
using UnityEngine;

namespace SF3
{
	public class BattleController : ExtentionBehaviour, ISceneInitializationObject
	{
		private static BattleController _instance;

		private FightController _fightController;

		private BattleEventsControl _battleEvents;

		private BattleKeyManager _keyManager;

		private ShadowFormController _shadowFormController;

		private bool _battleEnable;

		private bool _eventsEnable;

		public static BattleController Instance
		{
			get
			{
				return _instance;
			}
		}

		public FightController fightController
		{
			get
			{
				return _fightController;
			}
		}

		public static bool IsPVP
		{
			get
			{
				return ModelsManager.Instance.Player.isControl && ModelsManager.Instance.Enemy.isControl;
			}
		}

		public event Action OnApplicationQuitEvent = delegate
		{
		};

		private void Awake()
		{
			_instance = this;
			_fightController = new FightController();
			_battleEvents = new BattleEventsControl();
			_keyManager = new BattleKeyManager();
			_shadowFormController = new ShadowFormController();
			_battleEnable = false;
			EventsEnable(true);
			FrameSkipController.UpdateFrame = UpdateFrame;
		}

		public void Initialize()
		{
			_shadowFormController.Initialize();
			_fightController.Initialize();
			_battleEnable = false;
		}

		public void DisposePreviousLocation()
		{
			_battleEnable = false;
			BehaviourTimer.Clear();
			_battleEvents.ClearEvents();
		}

		public IEnumerator InitBattle()
		{
			Debug.Log("[InitBattle]");
			yield return _fightController.InitFight();
			_battleEnable = true;
			FrameSkipController.SyncFrameCountToTime();
		}

		private void Update()
		{
			FrameSkipController.MoveToNextFrame();
		}

		private void OnApplicationFocus(bool value)
		{
			FrameSkipController.SyncFrameCountToTime();
		}

		private void OnApplicationPause(bool value)
		{
			FrameSkipController.SyncFrameCountToTime();
		}

		private void UpdateFrame()
		{
			GameTimeController.UpdateBattleTime();
			BehaviourTimer.Update();
			if (_battleEnable)
			{
				if (!GameTimeController.gamePaused)
				{
					_battleEvents.Update();
				}
				_keyManager.Update();
				if (!GameTimeController.gamePaused)
				{
					GameVariables.Update();
					_fightController.Update();
					if (ModelsManager.Instance.Player != null && ModelsManager.Instance.Enemy != null)
					{
						ModelsManager.Instance.UpdateModels();
					}
					_shadowFormController.Update();
				}
			}
			if (_eventsEnable)
			{
				_battleEvents.ThrowEvents();
			}
		}

		public void BattleEnable(bool isEnable)
		{
			_battleEnable = isEnable;
		}

		public void EventsEnable(bool isEnable)
		{
			_eventsEnable = isEnable;
		}

		private void OnApplicationQuit()
		{
			this.OnApplicationQuitEvent();
		}

		public static void RegisterEventCallback(ETriggerEvents eventType, Action<BattleEventArgs> handler)
		{
			Instance._battleEvents.RegisterEventCallback(eventType, handler);
		}

		public static void RemoveEventCallback(ETriggerEvents eventType, Action<BattleEventArgs> handler)
		{
			Instance._battleEvents.RemoveEventCallback(eventType, handler);
		}

		public static void RegisterCallbackToAllEvents(Action<BattleEventArgs> handler)
		{
			Instance._battleEvents.RegisterCallbackToAllEvents(handler);
		}

		public static void ThrowEvent(BattleEventArgs args)
		{
			if (_instance._eventsEnable)
			{
				_instance._battleEvents.PushEvent(args);
			}
		}

		public static void PauseGame(bool pauseSounds = false)
		{
			GameTimeController.GameTimePause(pauseSounds);
			BattleKeyManager.Pause();
			ModelsManager.Instance.EnableModelsColliders(false);
			ModelsManager.Instance.SetModelsRagdollSleepState(true, 0);
			foreach (InteractiveModelObject droppedInteractiveObject in InteractiveModelObject.droppedInteractiveObjects)
			{
				droppedInteractiveObject.modelObject.SetModelsRagdollSleepState(true, 0);
			}
		}

		public static void ResumeGame()
		{
			GameTimeController.GameTimeResume();
			AudioManager.Instance.SetPitch(1f);
			BattleKeyManager.Unpause();
			ModelsManager.Instance.EnableModelsColliders(true);
			ModelsManager.Instance.SetModelsRagdollSleepState(false, 0);
			foreach (InteractiveModelObject droppedInteractiveObject in InteractiveModelObject.droppedInteractiveObjects)
			{
				droppedInteractiveObject.modelObject.SetModelsRagdollSleepState(false, 0);
			}
		}

		public static void SystemPause()
		{
			GameTimeController.SystemTimePause();
			BattleKeyManager.Pause();
			ModelsManager.Instance.EnableModelsColliders(false);
			ModelsManager.Instance.SetModelsRagdollSleepState(true, 0);
		}

		public static void SystemResume()
		{
			GameTimeController.SystemTimeResume();
			BattleKeyManager.Unpause();
			ModelsManager.Instance.EnableModelsColliders(true);
			ModelsManager.Instance.SetModelsRagdollSleepState(false, 0);
		}
	}
}
