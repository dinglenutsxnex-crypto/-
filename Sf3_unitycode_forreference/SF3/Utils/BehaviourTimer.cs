using System;
using System.Collections.Generic;

namespace SF3.Utils
{
	public class BehaviourTimer
	{
		public abstract class SingleTimer
		{
			public Action<object> callbacks;

			public object callbacksParam;

			public Action<float> OnUpdate;

			protected bool _stopped;

			public bool IsActive { get; protected set; }

			public bool SendCallBack { get; protected set; }

			public abstract bool UpdateTimer();

			public virtual bool Start()
			{
				return !IsActive || _stopped;
			}

			public virtual void Stop()
			{
				_stopped = true;
			}

			public virtual void ForceStop()
			{
				SendCallBack = false;
				Stop();
			}
		}

		private class SecondsTimer : SingleTimer
		{
			private readonly float _forSeconds;

			private float _endTime;

			public SecondsTimer(float forSeconds)
			{
				_forSeconds = forSeconds;
			}

			public override bool UpdateTimer()
			{
				if (!base.IsActive)
				{
					return false;
				}
				if (_stopped)
				{
					return true;
				}
				if (OnUpdate != null)
				{
					OnUpdate(1f - (_endTime - GameTimeController.time) / _forSeconds);
				}
				return _endTime <= GameTimeController.time;
			}

			public override bool Start()
			{
				if (!base.Start())
				{
					return false;
				}
				base.SendCallBack = true;
				_stopped = false;
				base.IsActive = true;
				_endTime = GameTimeController.time + _forSeconds;
				return true;
			}
		}

		private class FramesTimer : SingleTimer
		{
			protected readonly int ForFrames;

			protected int EndFrame;

			protected int StartFrame;

			public FramesTimer(int forFrames)
			{
				ForFrames = forFrames;
			}

			public override bool UpdateTimer()
			{
				return Update(GameTimeController.frameCount);
			}

			protected virtual bool Update(int currentFrame)
			{
				if (!base.IsActive)
				{
					return false;
				}
				if (_stopped)
				{
					return true;
				}
				OnUpdateCheck(currentFrame);
				return EndFrame <= currentFrame;
			}

			protected virtual void OnUpdateCheck(int currentFrame)
			{
				if (OnUpdate != null)
				{
					OnUpdate(1f - (float)(EndFrame - currentFrame) / (float)(EndFrame - StartFrame));
				}
			}

			public override bool Start()
			{
				if (!base.Start())
				{
					return false;
				}
				base.SendCallBack = true;
				_stopped = false;
				base.IsActive = true;
				StartFrame = GameTimeController.frameCount;
				EndFrame = StartFrame + ForFrames;
				return true;
			}
		}

		private class GameFramesTimer : FramesTimer
		{
			public GameFramesTimer(int forFrames)
				: base(forFrames)
			{
			}

			protected override bool Update(int currentFrame)
			{
				return !GameTimeController.gamePaused && base.Update(currentFrame);
			}

			public override bool UpdateTimer()
			{
				return Update((int)GameTimeController.battleTime);
			}

			protected override void OnUpdateCheck(int currentFrame)
			{
				if (OnUpdate != null)
				{
					OnUpdate(1f - ((float)EndFrame - GameTimeController.battleTime) / (float)(EndFrame - StartFrame));
				}
			}

			public override bool Start()
			{
				if (!base.Start())
				{
					return false;
				}
				base.SendCallBack = true;
				_stopped = false;
				base.IsActive = true;
				StartFrame = (int)GameTimeController.battleTime;
				EndFrame = StartFrame + ForFrames;
				return true;
			}
		}

		private static readonly List<SingleTimer> Timers;

		static BehaviourTimer()
		{
			Timers = new List<SingleTimer>();
		}

		public static void Clear()
		{
			Timers.Clear();
		}

		public static void Update()
		{
			for (int i = 0; i < Timers.Count; i++)
			{
				if (Timers[i].UpdateTimer())
				{
					if (Timers[i].SendCallBack)
					{
						Timers[i].callbacks(Timers[i].callbacksParam);
					}
					Timers.RemoveAt(i);
					i--;
				}
			}
		}

		private static SingleTimer CreateBaseTimer(SingleTimer timer, Action<float> onUpdate, object callBackParameter, params Action<object>[] callbacks)
		{
			if (callbacks == null)
			{
				throw new Exception(string.Format("Timer havnt any callbacks"));
			}
			timer.OnUpdate = onUpdate;
			foreach (Action<object> b in callbacks)
			{
				timer.callbacks = (Action<object>)Delegate.Combine(timer.callbacks, b);
			}
			timer.callbacksParam = callBackParameter;
			timer.Start();
			Timers.Add(timer);
			return timer;
		}

		public static SingleTimer CreateSecondsTimer(float forSeconds, object callBacksParameter, params Action<object>[] callbacks)
		{
			return CreateBaseTimer(new SecondsTimer(forSeconds), null, callBacksParameter, callbacks);
		}

		public static SingleTimer CreateSecondsTimer(float forSeconds, params Action<object>[] callbacks)
		{
			return CreateBaseTimer(new SecondsTimer(forSeconds), null, null, callbacks);
		}

		public static SingleTimer CreateSecondsTimer(float forSeconds, Action<float> onUpdate, params Action<object>[] callbacks)
		{
			return CreateBaseTimer(new SecondsTimer(forSeconds), onUpdate, null, callbacks);
		}

		public static SingleTimer CreateGameFramesTimer(int forFrames, object callBacksParameter, params Action<object>[] callbacks)
		{
			return CreateBaseTimer(new GameFramesTimer(forFrames), null, callBacksParameter, callbacks);
		}

		public static SingleTimer CreateGameFramesTimer(int forFrames, Action<float> onUpdate, object callBacksParameter, params Action<object>[] callbacks)
		{
			return CreateBaseTimer(new GameFramesTimer(forFrames), onUpdate, callBacksParameter, callbacks);
		}

		public static SingleTimer CreateGameFramesTimer(int forFrames, params Action<object>[] callbacks)
		{
			return CreateBaseTimer(new GameFramesTimer(forFrames), null, null, callbacks);
		}

		public static SingleTimer CreateFramesTimer(int forFrames, Action<float> onUpdate, object callBacksParameter, params Action<object>[] callbacks)
		{
			return CreateBaseTimer(new FramesTimer(forFrames), onUpdate, callBacksParameter, callbacks);
		}

		public static SingleTimer CreateFramesTimer(int forFrames, object callBacksParameter, params Action<object>[] callbacks)
		{
			return CreateFramesTimer(forFrames, null, callBacksParameter, callbacks);
		}

		public static SingleTimer CreateFramesTimer(int forFrames, params Action<object>[] callbacks)
		{
			return CreateFramesTimer(forFrames, null, null, callbacks);
		}
	}
}
