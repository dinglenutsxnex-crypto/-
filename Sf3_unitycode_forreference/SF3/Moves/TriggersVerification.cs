using System.Collections.Generic;
using System.Text;
using SF3.GameModels;
using SF3.Settings;

namespace SF3.Moves
{
	public class TriggersVerification
	{
		public class TriggerStack
		{
			public bool applyTrigger;

			public Model currentModel { get; private set; }

			public BattleEventArgs currentEventData { get; private set; }

			public InfoTrigger currentTrigger { get; private set; }

			public TriggerStack()
			{
				applyTrigger = false;
			}

			public TriggerStack(Model model, BattleEventArgs data, InfoTrigger trigger)
			{
				currentModel = model;
				currentEventData = data;
				currentTrigger = trigger;
			}

			public void SetModel(Model model)
			{
				currentModel = model;
			}

			public void SetEventData(BattleEventArgs data)
			{
				currentEventData = data;
			}

			public void SetTrigger(InfoTrigger trigger)
			{
				currentTrigger = trigger;
			}

			public void Clear()
			{
				currentModel = null;
				currentEventData = null;
				currentTrigger = null;
				applyTrigger = false;
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("Model: [{0}], BattleEventArgs: [{1}], InfoTrigger: [{2}], Applyed: [{3}]", currentModel.name, currentEventData.ToString(), currentTrigger.ToString(), applyTrigger.ToString());
				return stringBuilder.ToString();
			}
		}

		private static readonly Dictionary<int, TriggerStack> AnimationsTriggers;

		public static TriggerStack currentVerificationData { get; private set; }

		static TriggersVerification()
		{
			currentVerificationData = new TriggerStack();
			AnimationsTriggers = new Dictionary<int, TriggerStack>();
		}

		public static void CheckTriggers(BattleEventArgs args)
		{
			List<TriggerStack> list = new List<TriggerStack>();
			foreach (Model model in ModelsManager.Instance.Models)
			{
				if (model.id == args.SenderID)
				{
					model.CheckKeyInverted();
				}
				if (model.modelMoves != null)
				{
					list.AddRange(SelectByEvent(model, args));
					CheckTactics(model, args);
				}
			}
			if (list.Count > 0)
			{
				list.Sort((TriggerStack x, TriggerStack y) => (x.currentTrigger.priority < y.currentTrigger.priority) ? 1 : ((x.currentTrigger.priority != y.currentTrigger.priority) ? (-1) : 0));
				SelectByConditions(list);
			}
			ModelsApplyEvent(args);
		}

		private static List<TriggerStack> SelectByEvent(Model model, BattleEventArgs args)
		{
			List<TriggerStack> list = new List<TriggerStack>();
			currentVerificationData.SetModel(model);
			currentVerificationData.SetEventData(args);
			foreach (InfoTrigger eventTrigger in model.GetEventTriggers(args.EventType))
			{
				currentVerificationData.SetTrigger(eventTrigger);
				foreach (TriggerEvent @event in eventTrigger.events)
				{
					if (@event.type == args.EventType && @event.Equal(args))
					{
						list.Add(new TriggerStack(model, args, eventTrigger));
						break;
					}
				}
			}
			return list;
		}

		private static void SelectByConditions(List<TriggerStack> stackTriggers)
		{
			foreach (TriggerStack stackTrigger in stackTriggers)
			{
				if (CheckConditionsEqual(stackTrigger.currentModel, stackTrigger.currentEventData, stackTrigger.currentTrigger.conditions, stackTrigger.currentTrigger))
				{
					TriggerStack stack = new TriggerStack(stackTrigger.currentModel, stackTrigger.currentEventData, stackTrigger.currentTrigger);
					if (stackTrigger.currentTrigger.HasAction(TriggerAction.EActionType.ANIMATION) || stackTrigger.currentTrigger.HasAction(TriggerAction.EActionType.ANIMATION_RANDOM))
					{
						AddAnimation(stack);
					}
					else
					{
						ApplyActions(stack);
					}
				}
			}
		}

		public static bool CheckConditionsEqual(Model model, BattleEventArgs args, List<IConditionEqual> conditions, InfoTrigger trigger = null)
		{
			currentVerificationData.SetModel(model);
			currentVerificationData.SetEventData(args);
			currentVerificationData.SetTrigger(trigger);
			if (trigger != null && args != null)
			{
				trigger.SetCausedEvent(args.EventType);
			}
			foreach (IConditionEqual condition in conditions)
			{
				if (condition.IsEqual() == false)
				{
					return false;
				}
			}
			return true;
		}

		private static void CheckTactics(Model model, BattleEventArgs args)
		{
			if (model.IsAI)
			{
				currentVerificationData.SetModel(model);
				currentVerificationData.SetEventData(args);
				currentVerificationData.SetTrigger(null);
				if (TacticsSettings.IsDelaySettings(args))
				{
					model.Ai.ResetDelay();
				}
			}
		}

		private static void AddAnimation(TriggerStack stack)
		{
			if (!AnimationsTriggers.ContainsKey(stack.currentModel.id))
			{
				AnimationsTriggers.Add(stack.currentModel.id, stack);
			}
			else if (AnimationsTriggers[stack.currentModel.id].currentTrigger.priority < stack.currentTrigger.priority)
			{
				AnimationsTriggers[stack.currentModel.id] = stack;
			}
		}

		public static void ResetTriggers()
		{
			currentVerificationData.Clear();
			AnimationsTriggers.Clear();
		}

		public static void ModelsApplyEvent(BattleEventArgs args)
		{
			currentVerificationData.SetEventData(args);
			currentVerificationData.SetTrigger(null);
			foreach (Model model in ModelsManager.Instance.Models)
			{
				currentVerificationData.SetModel(model);
				model.ApplyEvent(args);
			}
		}

		public static void ApplyAnimations()
		{
			if (AnimationsTriggers.Count <= 0)
			{
				return;
			}
			foreach (TriggerStack value in AnimationsTriggers.Values)
			{
				if (!value.applyTrigger)
				{
					ApplyActions(value);
				}
			}
		}

		private static void ApplyActions(TriggerStack stack)
		{
			BattleLog.Frame(stack.currentModel, stack.currentTrigger);
			currentVerificationData.SetModel(stack.currentModel);
			currentVerificationData.SetEventData(stack.currentEventData);
			currentVerificationData.SetTrigger(stack.currentTrigger);
			stack.currentTrigger.ApplyActions(stack.currentModel);
			stack.applyTrigger = true;
		}

		public static void SelectConditions(Model model, BattleEventArgs args, List<InfoTrigger> triggers)
		{
			if (!FightController.TacticsCanReact())
			{
				return;
			}
			triggers.Sort((InfoTrigger x, InfoTrigger y) => (x.priority < y.priority) ? 1 : ((x.priority != y.priority) ? (-1) : 0));
			foreach (InfoTrigger trigger in triggers)
			{
				if (CheckConditionsEqual(model, args, trigger.conditions, trigger))
				{
					TriggerStack stack = new TriggerStack(model, args, trigger);
					if (trigger.HasAction(TriggerAction.EActionType.ANIMATION) || trigger.HasAction(TriggerAction.EActionType.ANIMATION_RANDOM))
					{
						AddAnimation(stack);
					}
					else
					{
						ApplyActions(stack);
					}
				}
			}
		}
	}
}
