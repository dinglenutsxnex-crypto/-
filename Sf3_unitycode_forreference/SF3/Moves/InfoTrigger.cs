using System;
using System.Collections.Generic;
using SF3.GameModels;
using UnityEngine;

namespace SF3.Moves
{
	[Serializable]
	public class InfoTrigger
	{
		[SerializeField]
		private string _name;

		[SerializeField]
		private string _parentName;

		[SerializeField]
		private List<TriggerEvent> _events;

		private Dictionary<ETriggerEvents, List<TriggerEvent>> _eventsDict = new Dictionary<ETriggerEvents, List<TriggerEvent>>();

		[SerializeField]
		private List<IConditionEqual> _conditions;

		[SerializeField]
		private List<ITriggerAction> _actions;

		[SerializeField]
		private List<string> _templates;

		[SerializeField]
		private int _priority;

		[SerializeField]
		private Tactics _tactics;

		[SerializeField]
		private bool _unresumable;

		[SerializeField]
		private bool _allowInFight;

		[SerializeField]
		private ETriggerEvents _calledByEvent;

		private string _animationName;

		public string name
		{
			get
			{
				return _name;
			}
		}

		public string parentName
		{
			get
			{
				return _parentName;
			}
		}

		public string fullName
		{
			get
			{
				return parentName + "." + name;
			}
		}

		public List<TriggerEvent> events
		{
			get
			{
				return _events;
			}
		}

		public List<IConditionEqual> conditions
		{
			get
			{
				return _conditions;
			}
		}

		public List<ITriggerAction> actions
		{
			get
			{
				return _actions;
			}
		}

		public List<string> templates
		{
			get
			{
				return _templates;
			}
		}

		public int priority
		{
			get
			{
				return _priority;
			}
		}

		public Tactics tactics
		{
			get
			{
				return _tactics;
			}
		}

		public bool unresumable
		{
			get
			{
				return _unresumable;
			}
		}

		public bool allowInFight
		{
			get
			{
				return _allowInFight;
			}
		}

		public ETriggerEvents calledByEvent
		{
			get
			{
				return _calledByEvent;
			}
		}

		public string animationName
		{
			get
			{
				return _animationName;
			}
		}

		public InfoTrigger(string parentNameVal, string triggerName = "")
		{
			_events = new List<TriggerEvent>();
			_conditions = new List<IConditionEqual>();
			_actions = new List<ITriggerAction>();
			_templates = new List<string>();
			_name = triggerName;
			_parentName = parentNameVal;
			_priority = 0;
		}

		public override string ToString()
		{
			StringWrapper stringWrapper = StringWrapper.Create();
			stringWrapper.Head(typeof(InfoTrigger).Name);
			stringWrapper.Wrap("name", name);
			stringWrapper.Wrap("animationName", animationName);
			stringWrapper.Wrap("priority", priority);
			return stringWrapper.ToString();
		}

		public void SetEvents(List<TriggerEvent> newEvents)
		{
			_events = newEvents;
			_eventsDict.Clear();
			foreach (TriggerEvent @event in _events)
			{
				if (!_eventsDict.ContainsKey(@event.type))
				{
					_eventsDict.Add(@event.type, new List<TriggerEvent>());
				}
				_eventsDict[@event.type].Add(@event);
			}
		}

		public void SetConditions(List<IConditionEqual> newConditions)
		{
			_conditions = newConditions;
		}

		public void SetActions(List<ITriggerAction> newAcions)
		{
			_actions = newAcions;
			_animationName = string.Empty;
			foreach (ITriggerAction action in _actions)
			{
				if (action.type == TriggerAction.EActionType.ANIMATION || action.type == TriggerAction.EActionType.ANIMATION_RANDOM)
				{
					_animationName = action.name;
					break;
				}
			}
		}

		public void SetTemplates(List<string> newTemplates)
		{
			_templates = newTemplates;
		}

		public void SetTactics(Tactics newTactics)
		{
			_tactics = newTactics;
		}

		public void SetPriority(int value)
		{
			_priority = value;
		}

		public void SetUnresumable(bool value)
		{
			_unresumable = value;
		}

		public void SetAllowInFight(bool value)
		{
			_allowInFight = value;
		}

		public TriggerEvent HasEvent(ETriggerEvents eventType, object[] args = null)
		{
			foreach (TriggerEvent @event in events)
			{
				if (@event.type == eventType && @event.EqualArguments(args))
				{
					return @event;
				}
			}
			return null;
		}

		public bool IsEqualConditions()
		{
			foreach (IConditionEqual condition in conditions)
			{
				if (condition.IsEqual() == false)
				{
					return false;
				}
			}
			return true;
		}

		public TriggerEvent GetEvent(ETriggerEvents eventType)
		{
			foreach (TriggerEvent @event in events)
			{
				if (@event.type == eventType)
				{
					return @event;
				}
			}
			return null;
		}

		public List<TriggerEvent> GetEvents(ETriggerEvents eTriggerEvents)
		{
			if (_eventsDict.ContainsKey(eTriggerEvents))
			{
				return _eventsDict[eTriggerEvents];
			}
			return new List<TriggerEvent>();
		}

		public void ApplyActions(Model modelState)
		{
			for (int i = 0; i < actions.Count; i++)
			{
				if (actions[i] is TriggerActionShowDialog)
				{
					actions[i].Apply(modelState, actions);
					break;
				}
				actions[i].Apply(modelState);
			}
		}

		public static InfoTrigger GetTriggerByAnimationName(string animationName, List<InfoTrigger> triggers)
		{
			foreach (InfoTrigger trigger in triggers)
			{
				foreach (TriggerAction action in trigger.actions)
				{
					if ((action.type == TriggerAction.EActionType.ANIMATION || action.type == TriggerAction.EActionType.ANIMATION_RANDOM) && action.name.Equals(animationName))
					{
						return trigger;
					}
				}
			}
			return null;
		}

		public bool HasAction(TriggerAction.EActionType eActionType)
		{
			foreach (TriggerAction action in actions)
			{
				if (action.type == eActionType)
				{
					return true;
				}
			}
			return false;
		}

		public void SetCausedEvent(ETriggerEvents triggerEvent)
		{
			_calledByEvent = triggerEvent;
		}
	}
}
