using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SF3.GameModels;
using SF3.Items;
using SF3.Moves;
using SF3.UserData;
using UnityEngine;
using sf3DTO;

namespace SF3
{
	public class FightController
	{
		public enum EFightStage
		{
			None = 0,
			FightStart = 1,
			FightEnd = 2,
			RoundStart = 3,
			RoundEnd = 4,
			RoundFightStart = 5,
			RoundFightEnd = 6
		}

		public RewardMultipyerCounter RewardMultipyerCounter;

		private FightResult _fightResult;

		private int _modelsAnimationEndCounter;

		public static FightControllerSettings Settings { get; private set; }

		public static FightController Instance { get; private set; }

		public RoundController RoundController { get; private set; }

		public EFightStage FightStage { get; private set; }

		public FightInfo CurrentFight { get; private set; }

		public FightController()
		{
			Instance = this;
			RoundController = new RoundController();
			RewardMultipyerCounter = new RewardMultipyerCounter();
		}

		public void Initialize()
		{
			RoundController.Initialize();
			FightStage = EFightStage.None;
			_fightResult = null;
			_modelsAnimationEndCounter = 0;
		}

		public IEnumerator InitFight()
		{
			Debug.Log("[InitFight]");
			Settings = new FightControllerSettings();
			CurrentFight = BattlesManager.currentBattle.GetCurrentFight();
			_fightResult = null;
			yield return Routiner.Go(SetFightStage(EFightStage.RoundStart));
		}

		public void Update()
		{
			if (FightStage != 0)
			{
				RoundController.Update();
				if (FightStage == EFightStage.RoundFightStart && RoundController.CheckEndRound() != 0)
				{
					Routiner.Go(SetFightStage(EFightStage.RoundFightEnd));
				}
			}
		}

		private IEnumerator SetFightStage(EFightStage stageValue, bool surrender = false, int? winnerId = null)
		{
			FightStage = stageValue;
			switch (stageValue)
			{
			case EFightStage.FightEnd:
				SetFightEnd(surrender, winnerId);
				break;
			case EFightStage.RoundStart:
				yield return Routiner.Go(RoundStartCoroutine());
				break;
			case EFightStage.RoundEnd:
				SetRoundEnd();
				break;
			case EFightStage.RoundFightStart:
				SetRoundFightStart();
				break;
			case EFightStage.RoundFightEnd:
				SetRoundFightEnd();
				break;
			}
			BattleController.ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_STAGE_CHANGE, -1, stageValue));
			Debug.Log(string.Format("SetFightStage [{0}] ", stageValue));
		}

		private void SetRoundFightEnd()
		{
			RoundController.EndRoundFight();
			BattleKeyManager.Instance.ActivateBattleKeys(false);
			BattleKeyManager.Instance.EnableBattleKeysEvents(false);
			_modelsAnimationEndCounter = 2;
		}

		private void SetRoundFightStart()
		{
			BattleKeyManager.Instance.EnableBattleKeysEvents(true);
			RoundController.StartFight();
		}

		private IEnumerator RoundStartCoroutine()
		{
			BattleLog.RoundStart(RoundController.CurrentRoundNumber);
			yield return Routiner.Go(RoundStartProcess());
		}

		private void RoundFightStartProcess()
		{
			Routiner.Go(SetFightStage(EFightStage.RoundFightStart));
		}

		private IEnumerator DojoRound()
		{
			using (TimerNode timerNode2 = new TimerNode("EntireDojo", "InitBattle"))
			{
				RoundController.ClearRoundData(CurrentFight);
				using (new TimerNode("InitRound", "EntireDojo"))
				{
					RoundController.InitNewRound(CurrentFight);
				}
				RoundController.InitBattleCamera(true);
				BattleController.RegisterEventCallback(ETriggerEvents.EVENT_ANIMATION_END, OnStageAnimationsEnd);
				if (UserDataController.waitingForRefreshBattles)
				{
					BattleController.Instance.EventsEnable(false);
					BattleController.Instance.BattleEnable(false);
					while (UserDataController.waitingForRefreshBattles)
					{
						yield return new WaitForEndOfFrame();
					}
					BattleController.Instance.BattleEnable(true);
					BattleController.Instance.EventsEnable(true);
				}
				LoadScreen.HideLoader(null);
				Routiner.Go(SetFightStage(EFightStage.RoundFightStart));
			}
		}

		private IEnumerator ShowFoggedScreenshot()
		{
			bool LoadscreenActive = true;
			LoadScreen.Instance.ShowFightStart(delegate
			{
				QualityManager.Instance.SetShadowForcedOff(false);
				LoadscreenActive = false;
			});
			yield return new WaitForEndOfFrame();
			yield return new WaitUntil(() => !LoadscreenActive);
		}

		private IEnumerator RoundControllerLoading()
		{
			using (new TimerNode("ClearData", "Loading"))
			{
				RoundController.ClearRoundData(CurrentFight);
			}
			yield return new WaitForEndOfFrame();
			using (new TimerNode("InitRound", "Loading"))
			{
				RoundController.InitNewRound(CurrentFight);
			}
			BattleController.ThrowEvent(new BattleEventArgs(ETriggerEvents.QEVENT_ROUND_LOAD));
			yield return new WaitForEndOfFrame();
			yield return new WaitUntil(() => !DialogsManager.IsDialog);
		}

		private IEnumerator ShowRoundUI()
		{
			bool loadscreenActive = true;
			RoundController.Instance.ShowStartRoundGUI(delegate
			{
				loadscreenActive = false;
				PauseWindow.IncrementShowCounter();
			});
			yield return new WaitUntil(() => !loadscreenActive);
		}

		private IEnumerator HideRoundUI()
		{
			bool loadscreenActive = true;
			LoadScreen.Instance.HideRoundEnd(delegate
			{
				loadscreenActive = false;
			});
			yield return new WaitUntil(() => !loadscreenActive);
		}

		private IEnumerator Round()
		{
			using (TimerNode timerNode4 = new TimerNode("Entire Round", "InitBattle"))
			{
				using (TimerNode timerNode = new TimerNode("Camera Movement", "Entire Round"))
				{
					if (RoundController.Instance.CurrentRoundNumber > 0)
					{
						BattleCamera.Instance.RoundEndTweenMotion();
						InteractiveModelObject.HideAll();
						ShadowFormController.Instance.ClearShadowEffect();
						GlowEffectController.instance.DisableGlow();
						yield return new WaitUntil(() => BattleCamera.Instance.RoundEndTweenIsReady());
						QualityManager.Instance.SetShadowForcedOff(true);
						yield return null;
					}
					else
					{
						yield return new WaitForEndOfFrame();
						BattleCamera.MoveToSpawnCentre(true);
					}
				}
				PauseWindow.ResetShowCounter();
				BattleCamera.Instance.SetCameraBlocked(true);
				yield return new WaitForEndOfFrame();
				using (TimerNode timerNode2 = new TimerNode("Screenshot", "Entire Round"))
				{
					yield return ShowFoggedScreenshot();
				}
				using (TimerNode timerNode3 = new TimerNode("Loading", "Entire Round"))
				{
					yield return RoundControllerLoading();
				}
			}
			BattleController.Instance.BattleEnable(true);
			using (TimerNode timerNode5 = new TimerNode("RoundUI", "Entire Round"))
			{
				yield return ShowRoundUI();
				yield return HideRoundUI();
				BattleCamera.Instance.SetCameraBlocked(false);
				RoundController.InitBattleCamera(false);
				BattleController.RegisterEventCallback(ETriggerEvents.EVENT_ANIMATION_END, OnStageAnimationsEnd);
				_modelsAnimationEndCounter = 2;
			}
		}

		private IEnumerator RoundStartProcess()
		{
			yield return (!IsDojo()) ? Round() : DojoRound();
		}

		public bool IsDojo()
		{
			return BattlesManager.currentBattleType == BattleType.Dojo;
		}

		private void SetRoundEnd()
		{
			RoundController.UpdateRewardCounters();
			if (RoundController.PlayerWinCount == RoundController.RoundsTotal || RoundController.EnemyWinCount == CurrentFight.roundsToLose)
			{
				Routiner.Go(SetFightStage(EFightStage.FightEnd));
			}
			else
			{
				Routiner.Go(SetFightStage(EFightStage.RoundStart));
			}
		}

		private void SetFightEnd(bool surrender, int? winnerId = null)
		{
			Debug.Log(string.Format("FightEnd -- CurrentBattle[{0}] -- CurrentFight [{1}] -- PlayerRoundWins [{2}] ", BattlesManager.currentBattle.GetBattleInfo().name, CurrentFight.battleID, RoundController.PlayerWinCount));
			int num = RoundController.PlayerWinCount;
			int num2 = RoundController.EnemyWinCount;
			if (winnerId.HasValue)
			{
				num = ((winnerId == 1) ? CurrentFight.roundsToWin : 0);
				num2 = ((winnerId != 1) ? CurrentFight.roundsToWin : 0);
			}
			_fightResult = new FightResult(BattlesManager.currentBattle, CurrentFight, num, num + num2, surrender, RewardMultipyerCounter);
			List<BaseItem> list = new List<BaseItem>();
			list.AddRange(((IEnumerable<Equipment>)_fightResult.GetRewardEquipment()).Select((Func<Equipment, BaseItem>)((Equipment equipment) => equipment)));
			list.AddRange(((IEnumerable<SF3.Items.Perk>)_fightResult.GetRewardPerks()).Select((Func<SF3.Items.Perk, BaseItem>)((SF3.Items.Perk equipment) => equipment)));
			list.AddRange(((IEnumerable<SF3.Items.Booster>)_fightResult.GetRewardBoosters()).Select((Func<SF3.Items.Booster, BaseItem>)((SF3.Items.Booster equipment) => equipment)));
			RewardDataProvider rewardDataProvider = new RewardDataProvider(list, UserManager.GetLevel(), UserManager.GetExperience());
			FightEnd(_fightResult);
			if (!surrender)
			{
				ShowDialog(_fightResult, rewardDataProvider, CloseFight);
			}
			else
			{
				CloseFight();
			}
			if (TutorialManager.Instance.tutorialPanel != null)
			{
				TutorialManager.Instance.HidePanel();
			}
		}

		public void FightEnd(FightResult fightResult)
		{
			GameTimeController.ResetTimeScale();
			BattlesManager.CompleteFight(fightResult);
			BattleLog.EndFight(fightResult.resultType == sf3DTO.FightResult.Win);
			RewardMultipyerCounter.Clear();
			QuestController.Instance.ThrowEvent(ETriggerEvents.QEVENT_FIGHT_END, fightResult.currentFight.battleID, fightResult.currentFight.fightID, fightResult.resultType);
		}

		public void ShowDialog(FightResult fightResult, RewardDataProvider rewardDataProvider, Action callback)
		{
			BattleInterface.Instance.ShowEndGame(delegate
			{
				switch (fightResult.resultType)
				{
				case sf3DTO.FightResult.Win:
				case sf3DTO.FightResult.Loss:
					RewardsWindow.Show(fightResult, rewardDataProvider, callback);
					break;
				case sf3DTO.FightResult.Surrender:
					callback();
					break;
				}
			}, fightResult);
		}

		public void CloseFight()
		{
			RoundResetableManager.Instance.ResetRules();
			LoadScreen.ShowLoader(delegate
			{
				if (QuestController.Instance.IsQueueEmpty())
				{
					ModuleController.GoToMap();
				}
				else
				{
					QuestController.Instance.RunForciblyQueue();
				}
			}, 0f, true);
		}

		private void OnStageAnimationsEnd(BattleEventArgs eventArgs)
		{
			if (eventArgs == null || eventArgs.EventData == null || _modelsAnimationEndCounter <= 0 || ((ModelInfoAnimation)eventArgs.EventData).animation == null || !((ModelInfoAnimation)eventArgs.EventData).animation.StageExist(FightStage))
			{
				return;
			}
			_modelsAnimationEndCounter--;
			if (_modelsAnimationEndCounter != 0)
			{
				return;
			}
			switch (FightStage)
			{
			case EFightStage.RoundStart:
				ShowGuiFightStart();
				break;
			case EFightStage.RoundFightEnd:
				RoundController.ShowEndRoundGUI(delegate
				{
					Routiner.Go(SetFightStage(EFightStage.RoundEnd));
				});
				break;
			}
		}

		private void ShowGuiFightStart()
		{
			if (Settings.ShowFightStart)
			{
				RoundController.ShowStartFightGUI(RoundFightStartProcess);
			}
			else
			{
				RoundFightStartProcess();
			}
		}

		public void SetFightResult(int winnerId, bool surrender)
		{
			Routiner.Go(SetFightStage(EFightStage.FightEnd, surrender, winnerId));
		}

		public void WinCurrentRound(ERoundResult winner)
		{
			RoundController.SetRoundWinner(winner);
			Routiner.Go(SetFightStage(EFightStage.RoundFightEnd));
		}

		public static bool TacticsCanReact()
		{
			return Instance.FightStage == EFightStage.RoundFightStart;
		}
	}
}
