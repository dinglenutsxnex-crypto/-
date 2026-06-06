using System;
using SF3.Moves;

namespace SF3
{
	public class BattleEventData
	{
		public ETriggerEvents EventType { get; private set; }

		public event Action<BattleEventArgs> OnBattleEvent;

		public BattleEventData(ETriggerEvents type)
		{
			EventType = type;
		}

		public void CallEvent(BattleEventArgs e)
		{
			if (this.OnBattleEvent != null)
			{
				this.OnBattleEvent(e);
			}
		}

		public void Clear()
		{
			if (this.OnBattleEvent != null)
			{
				Delegate[] invocationList = this.OnBattleEvent.GetInvocationList();
				foreach (Delegate @delegate in invocationList)
				{
					OnBattleEvent -= (Action<BattleEventArgs>)@delegate;
				}
			}
		}
	}
}
