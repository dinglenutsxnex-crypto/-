using System;
using SF3.Moves;

namespace SF3
{
	public class BattleEventArgs : EventArgs
	{
		public int SenderID { get; private set; }

		public object EventData { get; private set; }

		public ETriggerEvents EventType { get; private set; }

		public BattleEventArgs(ETriggerEvents eventTypeVal, int senderVal = -1, object eventDataVal = null)
		{
			EventType = eventTypeVal;
			SenderID = senderVal;
			EventData = eventDataVal;
		}

		public BattleEventArgs()
			: this(ETriggerEvents.EVENT_NONE)
		{
		}

		public void SetSender(int senderVal)
		{
			SenderID = senderVal;
		}

		public void SetEventData(object eventDataVal)
		{
			EventData = eventDataVal;
		}

		public void SetEventType(ETriggerEvents eventTypeVal)
		{
			EventType = eventTypeVal;
		}

		public void Reset()
		{
			SenderID = -1;
			EventData = null;
		}
	}
}
