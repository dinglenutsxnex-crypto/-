using System;
using SF3.Moves;

namespace SF3
{
	public class BattleEvent
	{
		private readonly BattleEventData[] _battleEvents;

		public BattleEvent()
		{
			_battleEvents = new BattleEventData[Enum.GetNames(typeof(ETriggerEvents)).Length];
			for (int i = 0; i < _battleEvents.Length; i++)
			{
				_battleEvents[i] = new BattleEventData((ETriggerEvents)i);
			}
		}

		public void AddEventCallback(ETriggerEvents eventType, Action<BattleEventArgs> handler)
		{
			_battleEvents[(int)eventType].OnBattleEvent += handler;
		}

		public void RemoveEventCallback(ETriggerEvents eventType, Action<BattleEventArgs> handler)
		{
			_battleEvents[(int)eventType].OnBattleEvent -= handler;
		}

		public void ThrowEvent(BattleEventArgs e)
		{
			_battleEvents[(int)e.EventType].CallEvent(e);
		}

		public void Clear()
		{
			if (_battleEvents != null)
			{
				BattleEventData[] battleEvents = _battleEvents;
				foreach (BattleEventData battleEventData in battleEvents)
				{
					battleEventData.Clear();
				}
			}
		}
	}
}
