using System;
using SF3.Moves;
using UnityEngine;

namespace SF3.Effects
{
	[Serializable]
	public class CameraShakeEffect : IGameEffect
	{
		private struct ShakeData
		{
			public float duration;

			public Vector3 amplitude;

			public Vector3 period;
		}

		private bool _active;

		private float _currentEffectDuration;

		private ShakeData _shakeData;

		public Vector3 shift { get; private set; }

		public void Create()
		{
		}

		public void Initialize()
		{
			shift = Vector3.zero;
			_active = false;
			_currentEffectDuration = 0f;
		}

		public void Update()
		{
			if (_active)
			{
				_currentEffectDuration += 1f;
				if (_currentEffectDuration == _shakeData.duration)
				{
					Stop();
				}
				Shake();
			}
		}

		public void Play()
		{
		}

		public void Stop()
		{
			_active = false;
			shift = Vector3.zero;
		}

		public void Reset()
		{
			StopForce();
		}

		public void StopForce()
		{
			Stop();
		}

		public void StartShake(TriggerActionShakeCamera shakeCameraAction)
		{
			StartShake(shakeCameraAction.Duration, shakeCameraAction.Amplitude, shakeCameraAction.Period);
		}

		public void StartShake(float duration, Vector3 amplitude, Vector3 period)
		{
			if (duration != 0f)
			{
				_currentEffectDuration = 0f;
				_shakeData.duration = duration;
				_shakeData.amplitude = amplitude;
				_shakeData.period = period;
				_active = true;
			}
		}

		private void Shake()
		{
			float duration = _shakeData.duration;
			float x = _shakeData.amplitude.x;
			float y = _shakeData.amplitude.y;
			float z = _shakeData.amplitude.z;
			float x2 = _shakeData.period.x;
			float y2 = _shakeData.period.y;
			float z2 = _shakeData.period.z;
			float x3 = 0f;
			float y3 = 0f;
			float z3 = 0f;
			if (x2 != 0f)
			{
				x3 = Mathf.Sin(_currentEffectDuration * 2f * (float)Math.PI / x2) * x * (duration - _currentEffectDuration) / duration;
			}
			if (y2 != 0f)
			{
				y3 = Mathf.Sin(_currentEffectDuration * 2f * (float)Math.PI / y2) * y * (duration - _currentEffectDuration) / duration;
			}
			if (z2 != 0f)
			{
				z3 = Mathf.Sin(_currentEffectDuration * 2f * (float)Math.PI / z2) * z * (duration - _currentEffectDuration) / duration;
			}
			shift = new Vector3(x3, y3, z3);
		}
	}
}
