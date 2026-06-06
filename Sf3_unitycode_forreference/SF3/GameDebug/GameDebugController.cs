using Nekki;
using SF3.GameModels;
using SF3.Moves;
using SF3.Utils;
using SF3_Attributes;
using UnityEngine;

namespace SF3.GameDebug
{
	public class GameDebugController : MonoBehaviour
	{
		public bool test;

		public string capsule = "Head";

		private static GameDebugController instance;

		public static GameDebugController Instatce
		{
			get
			{
				return instance;
			}
		}

		private void Awake()
		{
			instance = this;
		}

		private void Start()
		{
			if (!NekkiUtils.IsDebug)
			{
				Object.Destroy(this);
			}
		}

		private void Update()
		{
			if (!GameTimeController.gamePaused && BattlesManager.currentBattleType != 0 && !Sf3ConsoleCommands.isActive)
			{
				if (Input.GetKeyDown("1"))
				{
					RoundWin(ModelsManager.Instance.Player);
				}
				else if (Input.GetKeyDown("2"))
				{
					RoundWin(ModelsManager.Instance.Enemy);
				}
				else if (Input.GetKeyDown("3"))
				{
					BattleWin(ModelsManager.Instance.Player, true);
				}
				else if (Input.GetKeyDown("4"))
				{
					BattleWin(ModelsManager.Instance.Enemy);
				}
				else if (Input.GetKeyDown("5"))
				{
					ModuleController.GoToFight(BattlesManager.currentBattle.GetCurrentFight());
				}
				else if (Input.GetKeyDown("6"))
				{
					BattleWin(ModelsManager.Instance.Player);
				}
			}
			if (test)
			{
				IntervalAttack intervalAttack = new IntervalAttack();
				intervalAttack.SetImpulse(Vector3.zero);
				intervalAttack.damage.SetBaseDamage(0f);
				StrikeData strikeData = new StrikeData(intervalAttack);
				strikeData.attackingModel = ModelsManager.Instance.Enemy;
				strikeData.collisionEdge = ModelsManager.Instance.Player.modelComponents.modelCapsules.GetCapsule(capsule);
				StrikeData strikeData2 = strikeData;
				BattleController.ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_HIT, strikeData2.attackingModel.enemy.id, strikeData2));
			}
		}

		public static void CheckDebugAction(EDebugActions currentDebugAction)
		{
			if (!(ModelsManager.Instance.Player == null))
			{
				switch (currentDebugAction)
				{
				case EDebugActions.ROUND_WIN_PLAYER:
					RoundWin(ModelsManager.Instance.Player);
					break;
				case EDebugActions.ROUND_WIN_ENEMY:
					RoundWin(ModelsManager.Instance.Enemy);
					break;
				case EDebugActions.BATTLE_WIN_PLAYER:
					BattleWin(ModelsManager.Instance.Player);
					break;
				case EDebugActions.BATTLE_WIN_ENEMY:
					BattleWin(ModelsManager.Instance.Enemy);
					break;
				case EDebugActions.MAP_BATTLE_WIN:
					MapBattleFightWin();
					break;
				case EDebugActions.LOOSE_HEALTH_ENEMY:
					LooseHealth(ModelsManager.Instance.Enemy);
					break;
				case EDebugActions.LOOSE_HEALTH_PLAYER:
					LooseHealth(ModelsManager.Instance.Player);
					break;
				}
			}
		}

		private static void LooseHealth(Model modelValue)
		{
			modelValue.modelInfo.ChangeLife(0f - (modelValue.health - 0.001f));
		}

		private static void RoundWin(Model modelValue)
		{
			RoundResetableManager.Instance.ResetRules();
			IntervalAttack intervalAttack = new IntervalAttack();
			intervalAttack.SetImpulse(Vector3.zero);
			intervalAttack.damage.SetBaseDamage(999999f);
			StrikeData strikeData = new StrikeData(intervalAttack);
			strikeData.intervalAttack.damage.SetAttackAttributes(AttributeType.UnarmedDamage);
			strikeData.strikePoint = (strikeData.strikeEffectPoint = Vector3.zero);
			strikeData.attackingModel = modelValue;
			BattleController.ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_STRIKE, strikeData.attackingModel.id, strikeData));
		}

		private static void BattleWin(Model modelValue, bool skipRewards = false)
		{
			FightController.Instance.SetFightResult(modelValue.id, false);
			if (skipRewards)
			{
				FightController.Instance.CloseFight();
			}
		}

		private static void MapBattleFightWin()
		{
			if (MapController.SelectedBattle.GetIsHidden())
			{
				return;
			}
			FightResult fightResult = FightResult.CreateFightResultWin(MapController.SelectedBattle);
			if (fightResult == null)
			{
				return;
			}
			FightInfo currentFight = fightResult.currentFight;
			IntentParametrs intentParametrs = new IntentParametrs();
			intentParametrs.ModuleType = ConstantsSF3.ELocationSceneModule.Fight;
			intentParametrs.BattleID = currentFight.battleID;
			intentParametrs.FightID = currentFight.fightID;
			intentParametrs.TransitionType = IntentModule.ModuleTransitionType.Jump;
			IntentParametrs intentParametrs2 = intentParametrs;
			IntentModule intentModule = BaseModuleController.CreateIntent(intentParametrs2.ModuleType, intentParametrs2);
			intentModule.IsSkip = true;
			NetworkConnection.current.EnterDarkPocket();
			if (BaseModuleController.GoToModule(intentModule).IsInterrupted)
			{
				return;
			}
			FightController.Instance.FightEnd(fightResult);
			BehaviourTimer.CreateFramesTimer(1, delegate
			{
				if (!QuestController.Instance.IsQueueEmpty())
				{
					QuestController.Instance.RunForciblyQueue();
				}
			});
		}
	}
}
