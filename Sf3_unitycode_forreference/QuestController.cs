using System.Collections.Generic;
using SF3;
using SF3.Moves;
using SF3.UserData;
using UnityEngine;

public class QuestController
{
	public static bool EnableQuestLogMessages;

	private Dictionary<string, InfoTriggerSet> _quests;

	private readonly List<InfoTrigger> _queueTriggers;

	private readonly List<ITriggerAction> _queueActions;

	private readonly Dictionary<ETriggerEvents, TriggerEvent> _triggerEvents;

	private InfoTrigger _currentTrigger;

	private ITriggerAction _currentAction;

	private static QuestController _instance;

	public static QuestController Instance
	{
		get
		{
			return _instance ?? (_instance = new QuestController());
		}
	}

	public QuestController()
	{
		_currentTrigger = null;
		_quests = new Dictionary<string, InfoTriggerSet>();
		_queueTriggers = new List<InfoTrigger>();
		_queueActions = new List<ITriggerAction>();
		_triggerEvents = new Dictionary<ETriggerEvents, TriggerEvent>();
	}

	public void Init()
	{
		Parse();
	}

	public static void Clear()
	{
		_instance = null;
	}

	private void Parse()
	{
		string quests = ConfigsSourceResolver.Quests;
		MovesParser.ParseTriggersFromContent("quest", quests, _quests);
	}

	private void SetQueueTriggers(InfoTrigger trigger)
	{
		_queueTriggers.Add(trigger);
		_queueTriggers.Sort(SortQueueTriggers);
	}

	private int SortQueueTriggers(InfoTrigger x, InfoTrigger y)
	{
		return (x.priority < y.priority) ? 1 : ((x.priority != y.priority) ? (-1) : 0);
	}

	public void CleanQueueTriggers()
	{
		_queueTriggers.Clear();
	}

	public void ForciblySetQueue(List<string> userQuests)
	{
		foreach (string userQuest in userQuests)
		{
			string[] array = userQuest.Split('.');
			string text = array[0];
			string text2 = array[1];
			foreach (InfoTriggerSet value in _quests.Values)
			{
				if (!text.Equals(value.name))
				{
					continue;
				}
				foreach (InfoTrigger trigger in value.triggers)
				{
					if (text2.Equals(trigger.name))
					{
						SetQueueTriggers(trigger);
					}
				}
			}
		}
	}

	public bool ThrowEvent(ETriggerEvents e, params object[] args)
	{
		bool flag = false;
		foreach (InfoTriggerSet value in _quests.Values)
		{
			foreach (InfoTrigger trigger in value.triggers)
			{
				TriggerEvent triggerEvent = trigger.HasEvent(e, args);
				if (triggerEvent != null)
				{
					_triggerEvents[e] = triggerEvent;
					if (trigger.IsEqualConditions())
					{
						SetQueueTriggers(trigger);
						UserManager.AddQuestQueue(trigger);
						flag = true;
					}
				}
			}
		}
		string text = string.Empty;
		foreach (object obj in args)
		{
			text = string.Concat(text, obj, " ");
		}
		Log("ThrowEvent", string.Concat("Event:", e, " HasTrigger:", flag, " QueueCount:", _queueTriggers.Count, " Args:", text));
		RunQueueTriggers();
		return flag;
	}

	public void RunForciblyQueue()
	{
		RunQueueTriggers(true);
	}

	public bool IsQueueEmpty()
	{
		return _queueTriggers.Count == 0;
	}

	private void RunQueueTriggers(bool forcibly = false)
	{
		if (_queueTriggers.Count > 0 && !IsActiveActions())
		{
			int num = ((!forcibly) ? GetIndexTrigger() : 0);
			if (num > -1)
			{
				_currentTrigger = _queueTriggers[num];
				_queueTriggers.RemoveAt(num);
				SetQueueActions();
			}
		}
	}

	private int GetIndexTrigger()
	{
		if (BaseModuleController.CurrentType == ConstantsSF3.ELocationSceneModule.Fight)
		{
			for (int i = 0; i < _queueTriggers.Count; i++)
			{
				InfoTrigger infoTrigger = _queueTriggers[i];
				if (infoTrigger.allowInFight)
				{
					return i;
				}
			}
			return -1;
		}
		return 0;
	}

	private void SetQueueActions()
	{
		if (_currentTrigger == null)
		{
			return;
		}
		_queueActions.Clear();
		foreach (ITriggerAction action in _currentTrigger.actions)
		{
			_queueActions.Add(action);
		}
		RunActions();
	}

	private void RunActions()
	{
		if (_queueActions.Count > 0)
		{
			_currentAction = _queueActions[0];
			_queueActions.RemoveAt(0);
			Log("RunAction", _currentTrigger.name + " - " + _currentAction);
			_currentAction.Apply();
		}
		else
		{
			RunQueueTriggers();
		}
	}

	public void CloseAction(TriggerAction action)
	{
		if (_currentAction != null && _currentAction == action)
		{
			Log("CloseAction", _currentTrigger.name + " - " + _currentAction);
			_currentAction = null;
			if (_queueActions.Count == 0)
			{
				UserManager.RemoveQuestQueue(_currentTrigger);
			}
			RunActions();
		}
	}

	public void CallEvent(string data)
	{
		ThrowEvent(ETriggerEvents.QEVENT_CALL, data);
	}

	private bool IsActiveActions()
	{
		return _queueActions.Count != 0 || _currentAction != null;
	}

	private void Log(string type, string body = "")
	{
		if (EnableQuestLogMessages)
		{
			Messenger.Log(string.Format("Frame:[{0}], Type:[{1}], Body:[{2}] ", Time.frameCount, type, body), this);
		}
	}

	public object GetEventArgument(string name, string field)
	{
		ETriggerEvents eventTypeByName = TriggerEvent.GetEventTypeByName(name);
		return (!_triggerEvents.ContainsKey(eventTypeByName)) ? string.Empty : _triggerEvents[eventTypeByName].GetArgument(field);
	}
}
