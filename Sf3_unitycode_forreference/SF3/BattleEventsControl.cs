using System;
using System.Collections.Generic;
using SF3.Moves;
using SF3.Settings;

namespace SF3
{
	public class BattleEventsControl
	{
		private static BattleEventsControl _instance;

		private BattleEvent _battleEvents;

		private List<BattleEventArgs> _eventsStack;

		private float _eventEveryFrameStep;

		private float _eventEveryFrameNext;

		public static BattleEventsControl Instance
		{
			get
			{
				return _instance;
			}
		}

		public BattleEventsControl()
		{
			_instance = this;
			_battleEvents = new BattleEvent();
			_eventsStack = new List<BattleEventArgs>();
			float.TryParse(FightSettings.GetEventProperty(ETriggerEvents.EVENT_EVERY_FRAME, "Step"), out _eventEveryFrameStep);
			_eventEveryFrameNext = GameTimeController.battleTime + _eventEveryFrameStep;
		}

		public void ClearEvents()
		{
			_battleEvents.Clear();
			_eventsStack.Clear();
		}

		public void PushEvent(BattleEventArgs args)
		{
			_eventsStack.Add(args);
		}

		public void ThrowEvents()
		{
			while (_eventsStack.Count > 0)
			{
				for (int i = 0; i < _eventsStack.Count; i++)
				{
					BattleEventArgs battleEventArgs = _eventsStack[i];
					TriggersVerification.CheckTriggers(battleEventArgs);
					_battleEvents.ThrowEvent(battleEventArgs);
				}
				_eventsStack.Clear();
				TriggersVerification.ApplyAnimations();
			}
			TriggersVerification.ResetTriggers();
		}

		public void RegisterEventCallback(ETriggerEvents eventType, Action<BattleEventArgs> handler)
		{
			_battleEvents.AddEventCallback(eventType, handler);
		}

		public void RemoveEventCallback(ETriggerEvents eventType, Action<BattleEventArgs> handler)
		{
			_battleEvents.RemoveEventCallback(eventType, handler);
		}

		public void RegisterCallbackToAllEvents(Action<BattleEventArgs> handler)
		{
			foreach (KeyValuePair<string, ETriggerEvents> item in TriggerEvent.eventsNamesCompliance)
			{
				_battleEvents.AddEventCallback(item.Value, handler);
			}
		}

		public void Update()
		{
			if (GameTimeController.battleTime > _eventEveryFrameNext)
			{
				PushEvent(new BattleEventArgs(ETriggerEvents.EVENT_EVERY_FRAME));
				_eventEveryFrameNext = GameTimeController.battleTime + _eventEveryFrameStep;
			}
		}
	}
}
