using System;
using System.Collections.Generic;
using Nekki.Yaml;
using SF3.GameModels;
using SF3.Utils;

namespace SF3.Moves
{
	public class TriggerAction : Parseable, ITriggerAction
	{
		public enum EActionType
		{
			NONE = 0,
			ANIMATION = 1,
			ANIMATION_RANDOM = 2,
			PAUSE_ANIMATION = 3,
			SET_ATTRIBUTE = 4,
			SET_VARIABLE = 5,
			HIT_EFFECT = 6,
			GUI_EFFECT = 7,
			SHOW_STATUS_ICON = 8,
			CLEAR_STATUS_ICON = 9,
			SHOW_SF_ICON = 10,
			CUSTOM_EFFECT = 11,
			DESTROY_EFFECT = 12,
			FREEZE_FRAME = 13,
			SHAKE_CAMERA = 14,
			CREATE_MODEL = 15,
			DESTROY_ME = 16,
			ACTIVATE_COLLIDERS = 17,
			WALL_REACTION = 18,
			SET_SHADOW_CHARGE = 19,
			TOGGLE_SHADOW_FORM = 20,
			SET_HIT = 21,
			APPLY_STATUS = 22,
			CLEAR_STATUS = 23,
			DISARM = 24,
			PLAY_SOUND = 25,
			MOVE = 26,
			HIHGLIGHT_KEY = 27,
			END_TRY_ON = 28,
			SHOW_DIALOG = 29,
			SHOW_SYSTEM_ALERT = 30,
			SET_USER_VARIABLE = 31,
			REMOVE_USER_VARIABLE = 32,
			SHOW_CONFIGURABLE_DIALOG = 33,
			SET_FIGHT_VISIBLE = 34,
			SET_FIGHT_AVAILABLE = 35,
			DELETE_BATTLE = 36,
			SELECT_BATTLE = 37,
			ADD_TIMER = 38,
			REMOVE_TIMER = 39,
			GENERATE_BATTLE = 40,
			GENERATEPERIODIC_BATTLE = 41,
			GIVE_EXPERIENCE = 42,
			GIVE_COINS = 43,
			GIVE_BONUS = 44,
			GIVE_ITEM = 45,
			SET_ITEM_VISIBILITY = 46,
			SET_ITEM_EQUIPPED = 47,
			SWITCH_TACTICS = 48,
			WAIT = 49,
			GO_TO_MODULE = 50,
			GO_TO_MODULE_TRIGGER = 51,
			COMPLETE_PURCHASE = 52,
			HIGHLIGHT_UI_BUTTON = 53,
			SWITCH_HIGHLIGHT_UI_ELEMENT = 54,
			SWITCH_KEY = 55,
			DARKNESS = 56,
			CALL_EVENT = 57,
			INVERT_JOYSTICK = 58,
			SCORE_FIGHT = 59,
			WITHOUT_HP = 60,
			WITHOUT_SF = 61,
			WITHOUT_TIME = 62,
			QUEST_QUEUE = 63,
			TUTORIAL_COMPLETE = 64,
			INVISIBLE_WARRIOR = 65,
			REPULSION_TYPE = 66,
			SHOW_SELECTED_BATTLE_INFO = 67,
			CENTER_CAMERA_TO_MAP_POINT = 68,
			SET_DOJO = 69,
			SHOW_DUMMY_BATTLE = 70,
			HOT_GROUND = 71,
			LOSE = 72,
			TIMEOUT_WIN = 73,
			SHOW_TUTORIAL_PANEL = 74,
			UPDATE_TUTORIAL_PANEL = 75,
			HIDE_TUTORIAL_PANEL = 76,
			FEEDBACK_DIALOG = 77,
			SHADOW_CHARGE_BURN_DOWN = 78,
			UI_ELEMENTS = 79,
			SET_SHADER_EFFECT = 80,
			SET_FIGHT_SETTINGS = 81,
			SHADOWLESS = 82,
			ANALYTICS_LOG = 83
		}

		private readonly int _delayFrames;

		private readonly List<Model> _applyActionOn;

		private static Dictionary<string, Type> _actions;

		public string name { get; protected set; }

		public EActionType type { get; private set; }

		public EPlayerType targetType { get; private set; }

		private TriggerAction(EActionType type)
		{
			this.type = type;
			targetType = EPlayerType.This;
			name = string.Empty;
			_delayFrames = 0;
			_applyActionOn = new List<Model>();
		}

		protected TriggerAction(EActionType type, Node node)
			: this(type)
		{
			if (node != null && !(node is Scalar))
			{
				BaseMapping = (Mapping)((Mapping)node).nodesInside[0];
				string outResult;
				targetType = ((!TryGetString(out outResult, "ApplyTo", string.Empty, string.Empty, this, false)) ? EPlayerType.This : Model.GetPlayerTypeByName(outResult));
				TryGetString(out outResult, "Name", string.Empty, string.Empty, this, false);
				name = outResult;
				TryGetInt(out _delayFrames, "Frame", 0, string.Empty, this, false);
			}
		}

		public void Apply(Model modelState = null, List<ITriggerAction> actions = null)
		{
			if (_delayFrames > 0)
			{
				BehaviourTimer.CreateGameFramesTimer(_delayFrames, modelState, ApplyThisAction);
			}
			else
			{
				ApplyThisAction(modelState);
			}
		}

		protected virtual void ApplyThisAction(object modelData)
		{
			MovesController.ApplyAction(this, modelData);
			Fill(modelData as Model);
			for (int i = 0; i < _applyActionOn.Count; i++)
			{
				ApplyAction(_applyActionOn[i]);
			}
		}

		protected virtual void ApplyAction(object modelData)
		{
		}

		private void Fill(Model model)
		{
			_applyActionOn.Clear();
			switch (targetType)
			{
			case EPlayerType.This:
			case EPlayerType.Parent:
			case EPlayerType.Child:
				_applyActionOn.Add(model);
				break;
			case EPlayerType.Enemy:
				_applyActionOn.Add(model.enemy);
				break;
			case EPlayerType.Both:
				_applyActionOn.Add(model);
				_applyActionOn.Add(model.enemy);
				break;
			default:
				Messenger.Error(string.Format("Trigger is not implemented for type: [{0}]. Setting default type: [{1}]", targetType.ToString(), EPlayerType.This.ToString()));
				_applyActionOn.Add(model);
				break;
			}
		}

		public static void Init()
		{
			Dictionary<string, Type> dictionary = new Dictionary<string, Type>();
			dictionary.Add("Animation", typeof(TriggerActionAnimation));
			dictionary.Add("AnimationRandom", typeof(TriggerActionAnimationRandom));
			dictionary.Add("SetAttribute", typeof(TriggerActionSetAttribute));
			dictionary.Add("SetUserVariable", typeof(TriggerActionSetUserVariable));
			dictionary.Add("SetVariable", typeof(TriggerActionSetVariable));
			dictionary.Add("HitEffect", typeof(TriggerHitEffect));
			dictionary.Add("GUIEffect", typeof(TriggerGUIEffect));
			dictionary.Add("CustomEffect", typeof(TriggerCustomEffect));
			dictionary.Add("DestroyEffect", typeof(TriggerDestroyEffect));
			dictionary.Add("FreezeFrame", typeof(TriggerActionFreezeFrame));
			dictionary.Add("ShakeCamera", typeof(TriggerActionShakeCamera));
			dictionary.Add("CreateModel", typeof(TriggerActionCreateModel));
			dictionary.Add("DestroyMe", typeof(TriggerActionDestroyMe));
			dictionary.Add("ActivateColliders", typeof(TriggerActionActivateColliders));
			dictionary.Add("WallReaction", typeof(TriggerActionWallReaction));
			dictionary.Add("SetShadowCharge", typeof(TriggerActionSetShadowCharge));
			dictionary.Add("ToggleShadowForm", typeof(TriggerActionToggleShadowForm));
			dictionary.Add("SetHit", typeof(TriggerActionSetHit));
			dictionary.Add("ApplyStatus", typeof(TriggerActionApplyStatus));
			dictionary.Add("ClearStatus", typeof(TriggerActionClearStatus));
			dictionary.Add("Disarm", typeof(TriggerActionDisarm));
			dictionary.Add("PlaySound", typeof(TriggerActionPlaySound));
			dictionary.Add("ShowStatusIcon", typeof(TriggerActionShowStatusIcon));
			dictionary.Add("ClearStatusIcon", typeof(TriggerActionClearStatusIcon));
			dictionary.Add("ShowSFIcon", typeof(TriggerActionShowSfIcon));
			dictionary.Add("Move", typeof(TriggerActionMove));
			dictionary.Add("HighlightKey", typeof(TriggerActionHighlightKey));
			dictionary.Add("EndTryOn", typeof(TriggerActionEndTryOn));
			dictionary.Add("PauseAnimation", typeof(TriggerActionPauseAnimation));
			dictionary.Add("Dialog", typeof(TriggerActionShowDialog));
			dictionary.Add("SystemAlert", typeof(TriggerActionShowSystemAlert));
			dictionary.Add("SwitchTactics", typeof(TriggerActionSwitchTactics));
			dictionary.Add("SwitchKey", typeof(TriggerActionSwitchKey));
			dictionary.Add("GoToModule", typeof(TriggerActionGoToModule));
			dictionary.Add("HighlightUI", typeof(TriggerActionHighlightUIButton));
			dictionary.Add("Wait", typeof(TriggerActionWait));
			dictionary.Add("Call", typeof(TriggerActionCallEvent));
			dictionary.Add("RuleDarkness", typeof(TriggerActionDarkness));
			dictionary.Add("RuleInvertJoystick", typeof(TriggerActionInvertJoystick));
			dictionary.Add("RuleScoreFight", typeof(TriggerActionScoreFight));
			dictionary.Add("RuleWithoutHP", typeof(TriggerActionWithoutHP));
			dictionary.Add("RuleWithoutSF", typeof(TriggerActionWithoutSF));
			dictionary.Add("RuleShadowless", typeof(TriggerActionShadowless));
			dictionary.Add("RuleWithoutTime", typeof(TriggerActionWithoutTime));
			dictionary.Add("SelectBattle", typeof(TriggerActionSelectBattle));
			dictionary.Add("CompletePurchase", typeof(TriggerActionCompletePurchase));
			dictionary.Add("RemoveUserVariable", typeof(TriggerActionRemoveUserVariable));
			dictionary.Add("QuestStartQueue", typeof(TriggerActionQuestQueue));
			dictionary.Add("RepulsionType", typeof(TriggerActionDisableRepulsion));
			dictionary.Add("TutorialComplete", typeof(TriggerActionTutorialComplete));
			dictionary.Add("InvisibleWarrior", typeof(TriggerActionInvisibleWarrior));
			dictionary.Add("ShowSelectedBattleInfo", typeof(TriggerActionShowSelectedBattleInfo));
			dictionary.Add("CenterCameraToMapPoint", typeof(TriggerActionCenterCameraToMapPoint));
			dictionary.Add("SetDojo", typeof(TriggerActionSetDojo));
			dictionary.Add("HotGround", typeof(TriggerActionHotGround));
			dictionary.Add("Lose", typeof(TriggerActionLose));
			dictionary.Add("TimeoutWin", typeof(TriggerActiontTimeoutWin));
			dictionary.Add("ShowDummyBattle", typeof(TriggerActionShowDummyBattle));
			dictionary.Add("ShowTutorialPanel", typeof(TriggerActionShowTutorialPanel));
			dictionary.Add("HideTutorialPanel", typeof(TriggerActionHideTutorialPanel));
			dictionary.Add("UpdateTutorialPanel", typeof(TriggerActionUpdateTutorialPanel));
			dictionary.Add("FeedbackDialog", typeof(TriggerActionShowFeedbackDialog));
			dictionary.Add("ShadowChargeBurnDown", typeof(TriggerActionSetShadowChargeBurnDown));
			dictionary.Add("UIElements", typeof(TriggerActionUIElemets));
			dictionary.Add("SwitchHighlightUIElement", typeof(TriggerActionSwitchHighlightUIElement));
			dictionary.Add("SetShader", typeof(TriggerActionSetCharacterShader));
			dictionary.Add("FightSettings", typeof(TriggerActionSetFightSettings));
			dictionary.Add("AnalyticsLog", typeof(TriggerActionAnalyticsLog));
			_actions = dictionary;
		}

		public static ITriggerAction Create(Node item)
		{
			string text = ((!(item is Mapping)) ? ((Scalar)item).text : ((Mapping)item).nodesInside[0].key);
			if (_actions.ContainsKey(text))
			{
				return (ITriggerAction)Activator.CreateInstance(_actions[text], item);
			}
			throw new Exception("TriggerAction Not Found:" + text);
		}
	}
}
