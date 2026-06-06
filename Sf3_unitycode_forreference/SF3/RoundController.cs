using System;
using System.Collections.Generic;
using SF3.Audio;
using SF3.Effects;
using SF3.GameModels;
using SF3.Moves;
using SF3.UserData;
using UnityEngine;

namespace SF3
{
	public class RoundController
	{
		private readonly RoundDataSaver _roundDataSaver;

		private RoundResults _roundResults;

		private bool _roundProcess;

		public static RoundController Instance { get; private set; }

		public int RoundsTotal { get; private set; }

		public int RoundTimeTotal { get; private set; }

		public int CurrentRoundNumber { get; private set; }

		public int PlayerWinCount { get; private set; }

		public int EnemyWinCount { get; private set; }

		public RoundInfo CurrentRound { get; private set; }

		public RoundController()
		{
			Instance = this;
			_roundDataSaver = new RoundDataSaver();
			Initialize();
		}

		public void Initialize()
		{
			RoundsTotal = 0;
			RoundTimeTotal = 0;
			CurrentRoundNumber = 0;
			_roundProcess = false;
			PlayerWinCount = 0;
			EnemyWinCount = 0;
			_roundDataSaver.Initialize();
			_roundResults = RoundResults.Create(ERoundResult.IN_PROGRESS);
		}

		public void ClearRoundData(FightInfo currentFight)
		{
			Debug.Log("[ClearRoundData]");
			CurrentRoundNumber++;
			CurrentRound = currentFight.GetRound(PlayerWinCount + 1);
			Tutorial.Reset();
			ModelsManager.Instance.ClearBattleModels();
			AudioManager.Instance.Mute(false);
			EffectsManager.Reset();
			EffectsManager.Instance.EffectsEnabling(true);
			BattleEventsControl.Instance.ClearEvents();
			GameTimeController.Reset();
			InteractiveModelObject.Reset();
			RoundResetableManager.Instance.ResetRules();
			RulesController.Instance.ClearRules.InvokeSafe();
			GameVariables.ClearGameVariables();
			ModelCapsules.ShowCapsules = false;
			GlobalLoad.UnloadUnusedAssets();
			SF3FPSLogger.instance.Initialize();
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		public bool AnySFRule()
		{
			List<string> list = new List<string>();
			list.Add("sargesf");
			list.Add("perpetualsf");
			foreach (string item in list)
			{
				if (RulesController.Instance.HasActiveRule(item))
				{
					return true;
				}
			}
			return false;
		}

		public void InitNewRound(FightInfo currentFight)
		{
			Debug.Log("[InitNewRound]");
			ShadowFormController.Instance.SetLocationShadowFormEnabled(false, true);
			ModelInfo userModelInfo;
			ModelInfo warrior;
			using (new TimerNode("RulesAndAttributes", "InitRound"))
			{
				ModelsAttributesController.Instance.Initialize();
				userModelInfo = UserManager.UserModelInfo;
				userModelInfo.PreInit();
				warrior = CurrentRound.warrior;
				warrior.PreInit();
				RulesController.Instance.Initialize(CurrentRound, userModelInfo, warrior);
				LocationAudioSettings.NeedSpecialShadowFormSound = !AnySFRule();
			}
			using (new TimerNode("ModelsLoad", "InitRound"))
			{
				ModelsManager.Instance.CreateBattleModels(userModelInfo, warrior);
			}
			using (new TimerNode("SomeShit", "InitRound"))
			{
				ShadowFormController.Instance.DisposeUnused();
				ModelsManager.Instance.SetModelsActive(true);
				_roundDataSaver.GetModelData(ModelsManager.Instance.Player.name).ApplyToModel(ModelsManager.Instance.Player.id);
				_roundDataSaver.GetModelData(ModelsManager.Instance.Enemy.name).ApplyToModel(ModelsManager.Instance.Enemy.id);
				BattleInterface.Instance.InitializeFightHud();
				BattleKeyManager.Instance.InitBattleKeys();
				BattleKeyManager.Instance.ActivateBattleKeys(true);
				BattleKeyManager.Instance.EnableBattleKeysEvents(false);
				if (CurrentRoundNumber == 1)
				{
					BattleLog.Begin(currentFight, ModelsManager.Instance.Player, ModelsManager.Instance.Enemy);
				}
				else
				{
					BattleLog.UpdateModels(ModelsManager.Instance.Player, ModelsManager.Instance.Enemy);
				}
				RoundsTotal = currentFight.roundsToWin;
				RoundTimeTotal = currentFight.roundTime;
				BattleInterface.Instance.ResetBattleGUIManager();
				BattleInterface.Instance.SetBattleTimerFrames(SF3Utils.SecondsToFrames(RoundTimeTotal) + 1);
				if (ModelsManager.Instance.Player.modelShadowForm.shadowEnergy >= 1f)
				{
					ActionButtons.PlayShadowFull();
					StickHelper.Instance.ShowShadowHint();
				}
				_roundProcess = false;
				_roundResults.RoundResult = ERoundResult.IN_PROGRESS;
			}
			using (new TimerNode("WarmUpShaders", "InitRound"))
			{
				Shader.WarmupAllShaders();
			}
			using (new TimerNode("GC", "InitRound"))
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}
		}

		public void InitBattleCamera(bool instant)
		{
			if (ModelsManager.Instance.Player != null)
			{
				SceneConfig.LeftBorderX = SceneConfig.LocationLeftBorder;
				SceneConfig.RightBorderX = SceneConfig.LocationRightBorder;
				BattleCamera.SetModels(ModelsManager.Instance.Player, ModelsManager.Instance.Enemy);
				BattleCamera.Instance.ActivateBattleCamera(instant);
			}
			else
			{
				BattleCamera.MoveToDefault(true);
			}
		}

		private void SaveRoundData()
		{
			_roundDataSaver.SaveRoundData(ModelsManager.Instance.Player);
			_roundDataSaver.SaveRoundData(ModelsManager.Instance.Enemy);
		}

		public void ShowStartFightGUI(Action callback)
		{
			BattleInterface.Instance.ShowStartRoundFight(callback);
		}

		public void ShowStartRoundGUI(Action callback)
		{
			BattleInterface.Instance.ShowStartRound(callback, CurrentRoundNumber);
		}

		public void ShowNextRoundGUI(Action callback)
		{
			BattleInterface.Instance.ShowStartRound(callback, CurrentRoundNumber + 1);
		}

		public void StartFight()
		{
			BattleInterface.Instance.BattleTimerActive(true);
			_roundProcess = true;
		}

		public void ShowEndRoundGUI(Action callback)
		{
			_roundResults.RoundWinner = ((_roundResults.RoundResult != ERoundResult.PLAYER_WIN) ? ModelsManager.Instance.Enemy.modelInfo : ModelsManager.Instance.Player.modelInfo);
			LocaleImport.LocaleString winnerName = Localization.Get(_roundResults.RoundWinner.alias);
			Action endRoundText;
			if (BattleController.IsPVP)
			{
				endRoundText = delegate
				{
					BattleInterface.Instance.ShowEndRoundFight_PVP(callback, _roundResults.RoundResult, winnerName);
				};
			}
			else
			{
				endRoundText = delegate
				{
					BattleInterface.Instance.ShowEndRoundFight(callback, _roundResults.RoundResult);
				};
			}
			Action action = endRoundText;
			if (_roundResults.RoundWinner.isControl)
			{
				if (_roundResults.IsPerfect)
				{
					action = (Action)Delegate.Combine(action, (Action)delegate
					{
						BattleInterface.Instance.ShowPerfect(endRoundText);
					});
				}
				else if (_roundResults.IsGreat)
				{
					action = (Action)Delegate.Combine(action, (Action)delegate
					{
						BattleInterface.Instance.ShowGreat(endRoundText);
					});
				}
			}
			if (_roundResults.RoundResult == ERoundResult.PLAYER_WIN)
			{
				PlayerWinCount++;
				BattleInterface.Instance.ColorPlayerRoundsUI(PlayerWinCount);
			}
			else
			{
				EnemyWinCount++;
				BattleInterface.Instance.ColorEnemyRoundsUI(EnemyWinCount);
			}
			action();
		}

		public void Update()
		{
			BattleInterface.Instance.UpdateBattleGUIManager();
			if (_roundProcess)
			{
				BattleInterface.Instance.UpdateRoundTimer();
			}
		}

		public ERoundResult CheckEndRound()
		{
			CheckDead();
			CheckTimeOut();
			return _roundResults.RoundResult;
		}

		private void CheckTimeOut()
		{
			if (BattleInterface.Instance.TimeCount <= 0f)
			{
				BattleKeyManager.Instance.InitBattleKeys();
				BattleKeyManager.Instance.EnableBattleKeysEvents(false);
				BattleController.ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_TIME_OUT));
				if (FightController.Settings.IsScoreFight)
				{
					CheckRoundWinnerAfterTimeoutByScore();
				}
				else
				{
					CheckRoundWinnerAfterTimeout();
				}
			}
		}

		private void CheckRoundWinnerAfterTimeoutByScore()
		{
			SetRoundWinner((ModelsManager.Instance.Player.modelInfo.score <= ModelsManager.Instance.Enemy.modelInfo.score) ? ERoundResult.ENEMY_WIN : ERoundResult.PLAYER_WIN);
		}

		private void CheckDead()
		{
			if (ModelsManager.Instance.Player.isDead || ModelsManager.Instance.Enemy.isDead)
			{
				SetRoundWinner(ModelsManager.Instance.Player.isDead ? ERoundResult.ENEMY_WIN : ERoundResult.PLAYER_WIN);
			}
		}

		private void CheckRoundWinnerAfterTimeout()
		{
			if (!AllAnimationsFinished())
			{
				return;
			}
			CheckDead();
			if (_roundResults.RoundResult != ERoundResult.PLAYER_WIN && _roundResults.RoundResult != ERoundResult.ENEMY_WIN)
			{
				if (FightController.Settings.IsTimeoutWin)
				{
					SetRoundWinner(ERoundResult.PLAYER_WIN);
					return;
				}
				SetRoundWinner(ERoundResult.ENEMY_WIN);
				_roundResults.RoundResult = ERoundResult.TIME_OUT;
				GameVariables.AddVariable("Timeout", 1);
			}
		}

		private bool AllAnimationsFinished()
		{
			return GameVariables.GetVariable(ModelsManager.Instance.Player.id, "Timeout_Postpone") != null && GameVariables.GetVariable(ModelsManager.Instance.Enemy.id, "Timeout_Postpone") != null;
		}

		public void SetRoundWinner(ERoundResult winner)
		{
			_roundResults.RoundResult = winner;
			GameVariables.AddVariable(_roundResults.GetWinnerId(), "Winner", 1);
		}

		public void EndRoundFight()
		{
			BattleLog.RoundEnd(_roundResults.RoundResult == ERoundResult.PLAYER_WIN);
			BattleInterface.Instance.BattleTimerActive(false);
			_roundProcess = false;
			SaveRoundData();
		}

		public void UpdateRewardCounters()
		{
			_roundResults.UpdateRewardCountersOf(ModelType.Player);
			FightController.Instance.RewardMultipyerCounter.PrintAllData("Reward Counters");
		}

		public ERoundResult GetRoundResult()
		{
			return _roundResults.RoundResult;
		}
	}
}
