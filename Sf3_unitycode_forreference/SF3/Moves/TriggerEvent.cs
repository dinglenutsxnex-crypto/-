using System;
using System.Collections.Generic;
using Nekki.Yaml;
using SF3.GameModels;
using UnityEngine;

namespace SF3.Moves
{
	[Serializable]
	public class TriggerEvent : Parseable
	{
		[SerializeField]
		protected string name;

		[SerializeField]
		protected EPlayerType playerType;

		protected object[] arguments;

		private static Dictionary<ETriggerEvents, Type> classesEvents;

		public static Dictionary<string, ETriggerEvents> eventsNamesCompliance;

		public ETriggerEvents type { get; private set; }

		private TriggerEvent(ETriggerEvents typeValue)
		{
			type = typeValue;
			playerType = EPlayerType.Both;
			name = string.Empty;
		}

		public TriggerEvent(ETriggerEvents typeValue, Mapping eventMap)
			: this(typeValue)
		{
			if (eventMap != null)
			{
				BaseMapping = eventMap;
				string outResult;
				if (TryGetString(out outResult, "Name", string.Empty, string.Empty, null, false))
				{
					name = outResult;
				}
				if (TryGetString(out outResult, "Player", string.Empty, string.Empty, null, false))
				{
					playerType = Model.GetPlayerTypeByName(outResult);
				}
			}
		}

		public virtual bool Equal(BattleEventArgs args)
		{
			bool result = false;
			switch (playerType)
			{
			case EPlayerType.This:
				if (args.SenderID == TriggersVerification.currentVerificationData.currentModel.id)
				{
					result = true;
				}
				break;
			case EPlayerType.Enemy:
				if (args.SenderID == TriggersVerification.currentVerificationData.currentModel.enemy.id)
				{
					result = true;
				}
				break;
			case EPlayerType.Parent:
				if (TriggersVerification.currentVerificationData.currentModel.parentModel != null && args.SenderID == TriggersVerification.currentVerificationData.currentModel.parentModel.id)
				{
					result = true;
				}
				break;
			case EPlayerType.Child:
				if (TriggersVerification.currentVerificationData.currentModel.childModels.Count <= 0)
				{
					break;
				}
				foreach (Model childModel in TriggersVerification.currentVerificationData.currentModel.childModels)
				{
					if (args.SenderID == childModel.id)
					{
						result = true;
						break;
					}
				}
				break;
			case EPlayerType.Both:
				result = true;
				break;
			}
			return result;
		}

		public bool EqualArguments(object[] args)
		{
			SetArguments(args);
			return Equal();
		}

		protected virtual bool Equal()
		{
			return true;
		}

		protected virtual void SetArguments(object[] args)
		{
			arguments = args;
		}

		public virtual object GetArgument(string field)
		{
			return null;
		}

		public override string ToString()
		{
			return string.Concat("TriggerEvent [type: ", type, " name: ", name, "]");
		}

		protected bool CompareString(string str1, string str2)
		{
			char[] array = str1.Trim('\0', '\n', '\r').ToCharArray();
			char[] array2 = str2.Trim('\0', '\n', '\r').ToCharArray();
			if (array.Length != array2.Length)
			{
				return false;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != array2[i])
				{
					return false;
				}
			}
			return true;
		}

		public static void Init()
		{
			eventsNamesCompliance = null;
			classesEvents = null;
			Register("KeyPress", ETriggerEvents.EVENT_KEY_PRESSED, typeof(TriggerEventKeyPressed));
			Register("Hit", ETriggerEvents.EVENT_HIT, typeof(TriggerEventHit));
			Register("PostHit", ETriggerEvents.EVENT_POST_HIT, typeof(TriggerEventPostHit));
			Register("AnimationStart", ETriggerEvents.EVENT_ANIMATION_START, typeof(TriggerEventAnimationStart));
			Register("AnimationEnd", ETriggerEvents.EVENT_ANIMATION_END, typeof(TriggerEventAnimationEnd));
			Register("IntervalStart", ETriggerEvents.EVENT_INTERVAL_START, typeof(TriggerEventIntervalStart));
			Register("IntervalEnd", ETriggerEvents.EVENT_INTERVAL_END, typeof(TriggerEventIntervalEnd));
			Register("StageChange", ETriggerEvents.EVENT_STAGE_CHANGE, typeof(TriggerEventStageChange));
			Register("VariableDestruction", ETriggerEvents.EVENT_VARIABLE_DESTRUCTION, typeof(TriggerEventVariableDestruction));
			Register("GamePause", ETriggerEvents.EVENT_GAME_PAUSE, typeof(TriggerEventGamePause));
			Register("Birth", ETriggerEvents.EVENT_BIRTH, typeof(TriggerEventBirth));
			Register("FloorHit", ETriggerEvents.EVENT_FLOOR_HIT, typeof(TriggerEventFloorHit));
			Register("Slowdown", ETriggerEvents.EVENT_SLOWDOWN, typeof(TriggerEventSlowdown));
			Register("BorderHit", ETriggerEvents.EVENT_BORDER_HIT, typeof(TriggerEventBorderHit));
			Register("WallHit", ETriggerEvents.EVENT_WALL_HIT, typeof(TriggerEventWallHit));
			Register("Strike", ETriggerEvents.EVENT_STRIKE, typeof(TriggerEventStrike));
			Register("ShadowChargeBurndown", ETriggerEvents.EVENT_SHADOW_CHARGE_BURNDOWN, typeof(TriggerEventShadowChargeBurndown));
			Register("DodgeCheck", ETriggerEvents.EVENT_DODGE_CHECK, typeof(TriggerEventDodgeCheck));
			Register("BlockCheck", ETriggerEvents.EVENT_BLOCK_CHECK, typeof(TriggerEventBlockCheck));
			Register("Equip", ETriggerEvents.EVENT_EQUIP, typeof(TriggerEventEquip));
			Register("ModulePreEnter", ETriggerEvents.EVENT_MODULE_PRE_ENTER, typeof(TriggerEventModulePreEnter));
			Register("ModuleEnter", ETriggerEvents.EVENT_MODULE_ENTER, typeof(TriggerEventModuleEnter));
			Register("ItemFloorHit", ETriggerEvents.EVENT_ITEM_FLOOR_HIT, typeof(TriggerEventItemFloorHit));
			Register("EveryFrame", ETriggerEvents.EVENT_EVERY_FRAME, typeof(TriggerEventEveryFrame));
			Register("TryOn", ETriggerEvents.EVENT_TRY_ON, typeof(TriggerEventTryOn));
			Register("SessionStart", ETriggerEvents.QEVENT_SESSION_START, typeof(TriggerEventSessionStart));
			Register("PreSceneChange", ETriggerEvents.QEVENT_PRE_SCENE_CHANGE, typeof(TriggerEventPreSceneChange));
			Register("PostSceneChange", ETriggerEvents.QEVENT_POST_SCENE_CHANGE, typeof(TriggerEventPostSceneChange));
			Register("FightEnter", ETriggerEvents.QEVENT_FIGHT_ENTER, typeof(TriggerEventFightEnter));
			Register("Call", ETriggerEvents.QEVENT_CALL, typeof(TriggerEventCall));
			Register("PrePurchase", ETriggerEvents.QEVENT_PRE_PURCHASE, typeof(TriggerEventPrePurchase));
			Register("ButtonPressed", ETriggerEvents.EVENT_TUTORIAL_BUTTON_PRESSED, typeof(TriggerEventButtonPressed));
			Register("DragEnd", ETriggerEvents.EVENT_TUTORIAL_DRAG_END, typeof(TriggerEventDragEnd));
			Register("TimeOut", ETriggerEvents.EVENT_TIME_OUT, typeof(TriggerEventTimeOut));
			Register("FightEnd", ETriggerEvents.QEVENT_FIGHT_END, typeof(TriggerEventQuestFightEnd));
			Register("BattleSelectionStart", ETriggerEvents.QEVENT_BATTLE_SELECTION_START, typeof(TriggerEventQuestBattleSelectionStart));
			Register("BattleSelectionEnd", ETriggerEvents.QEVENT_BATTLE_SELECTION_END, typeof(TriggerEventQuestBattleSelectionEnd));
			Register("MapMissClick", ETriggerEvents.QEVENT_MAP_MISS_CLICK, typeof(TriggerEventQuestMapMissClick));
			Register("MapOpened", ETriggerEvents.QEVENT_MAP_OPENED, typeof(TriggerEventQuestBattleCenteringStart));
			Register("RoundLoad", ETriggerEvents.QEVENT_ROUND_LOAD, typeof(TriggerEventQuestRoundLoad));
		}

		private static void Register(string name, ETriggerEvents type, Type classEvent)
		{
			if (classesEvents == null)
			{
				classesEvents = new Dictionary<ETriggerEvents, Type>();
				eventsNamesCompliance = new Dictionary<string, ETriggerEvents>();
			}
			eventsNamesCompliance.Add(name, type);
			classesEvents.Add(type, classEvent);
		}

		public static ETriggerEvents GetEventTypeByName(string name)
		{
			if (eventsNamesCompliance.ContainsKey(name))
			{
				return eventsNamesCompliance[name];
			}
			Debug.LogError("getEventTypeByName - unknown type: " + name);
			return ETriggerEvents.EVENT_NONE;
		}

		public static string GetEventNameByType(ETriggerEvents triggerEvent)
		{
			foreach (KeyValuePair<string, ETriggerEvents> item in eventsNamesCompliance)
			{
				if (item.Value == triggerEvent)
				{
					return item.Key;
				}
			}
			return string.Empty;
		}

		public static List<TriggerEvent> Parse(Sequence nodes)
		{
			List<TriggerEvent> list = new List<TriggerEvent>();
			foreach (Node item2 in nodes.nodesInside)
			{
				Mapping mapping = null;
				ETriggerEvents eTriggerEvents = ETriggerEvents.EVENT_NONE;
				if (item2 is Scalar)
				{
					eTriggerEvents = GetEventTypeByName(item2.ToString());
				}
				else
				{
					mapping = (Mapping)item2;
					eTriggerEvents = GetEventTypeByName(mapping.nodesInside[0].key);
					mapping = mapping.GetMapping(mapping.nodesInside[0].key);
				}
				if (eTriggerEvents == ETriggerEvents.EVENT_NONE)
				{
					Debug.LogError(string.Format("Cant find event with name {0}", item2));
				}
				else if (classesEvents.ContainsKey(eTriggerEvents))
				{
					TriggerEvent item = (TriggerEvent)Activator.CreateInstance(classesEvents[eTriggerEvents], mapping);
					list.Add(item);
				}
				else
				{
					Debug.LogError(string.Format("Havn't any event initialization for {0}", eTriggerEvents));
				}
			}
			return list;
		}
	}
}
