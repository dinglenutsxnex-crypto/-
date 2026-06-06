using System;
using SF3.GameModels;
using SF3.Moves;
using UnityEngine;

namespace SF3.Effects
{
	[Serializable]
	public class GameTimeEffects : IGameEffectGeneral
	{
		[SerializeField]
		private SlowmotionEffect _slowmotionEffect;

		private FreezeFrameEffect _freezeFrameEffect;

		public bool slowMotionActive
		{
			get
			{
				return _slowmotionEffect.active;
			}
		}

		public bool freezeFrameActive
		{
			get
			{
				return _freezeFrameEffect.active;
			}
		}

		public AnimationCurve slowmotionCurve
		{
			get
			{
				return _slowmotionEffect.inOutCurve;
			}
		}

		public void Create()
		{
			_freezeFrameEffect = new FreezeFrameEffect();
			_freezeFrameEffect.Create();
			_slowmotionEffect.Create();
		}

		public void Initialize()
		{
			_slowmotionEffect.Initialize();
			_freezeFrameEffect.Initialize();
		}

		public void Update()
		{
			_slowmotionEffect.Update();
		}

		public void Stop(string effectName, int modelUsedID)
		{
			if (effectName.Equals("LastHitSlowmotion"))
			{
				_slowmotionEffect.Stop();
			}
		}

		public void Play(string name, int modelUsedID = -1)
		{
			if (name.Equals("LastHitSlowmotion"))
			{
				_slowmotionEffect.Play(null);
			}
		}

		public void DisposeEffectsByModel(int modelID)
		{
		}

		public bool Play(Model model, string name)
		{
			if (name.Equals("LastHitSlowmotion"))
			{
				_slowmotionEffect.Play(model);
				return true;
			}
			return false;
		}

		public bool Play(string name, TriggerActionFreezeFrame actFreezeFrames)
		{
			if (slowMotionActive)
			{
				_slowmotionEffect.StopForce();
			}
			_freezeFrameEffect.setFreezeFrames = actFreezeFrames.duration;
			_freezeFrameEffect.Play();
			return false;
		}

		public void StopAll(bool forced = false)
		{
			_slowmotionEffect.Reset();
			_freezeFrameEffect.Reset();
		}

		public void Reset(string effectName)
		{
			if (effectName.Equals("LastHitSlowmotion"))
			{
				_slowmotionEffect.Reset();
			}
		}

		public void Stop(string effectName, bool forced = false)
		{
			if (effectName.Equals("LastHitSlowmotion"))
			{
				_slowmotionEffect.Stop();
			}
		}
	}
}
