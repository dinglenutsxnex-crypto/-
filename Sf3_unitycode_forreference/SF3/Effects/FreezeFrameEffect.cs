using System;
using SF3.Utils;

namespace SF3.Effects
{
	[Serializable]
	public class FreezeFrameEffect : IGameEffect
	{
		public int setFreezeFrames { get; set; }

		public bool active { get; private set; }

		public void Create()
		{
		}

		public void Initialize()
		{
			setFreezeFrames = 0;
			active = false;
		}

		public void Play()
		{
			if (!active)
			{
				active = true;
				BehaviourTimer.CreateFramesTimer(setFreezeFrames, Stop);
				BattleController.PauseGame();
			}
		}

		public void Stop()
		{
		}

		public void StopForce()
		{
		}

		private void Stop(object objData)
		{
			active = false;
			setFreezeFrames = 0;
			BattleController.ResumeGame();
		}

		public void Reset()
		{
			Stop(null);
		}
	}
}
