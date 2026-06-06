using System;
using System.Collections.Generic;
using SF3.Audio;
using SF3.GameModels;
using UnityEngine;

namespace SF3.Effects
{
	[Serializable]
	public class SlowmotionEffect : IGameEffect
	{
		public string name;

		public int inOutFrames;

		public AnimationCurve inOutCurve;

		public float timeScaleValue;

		private int _counter;

		private bool _playStart;

		private bool _playEnd;

		private Dictionary<int, bool> _modelsSlowmotionsUsed;

		[SerializeField]
		private PlaySoundEffect _activateSlowmotionSound;

		[SerializeField]
		private PlaySoundEffect _disableSlowmotionSound;

		public bool active { get; private set; }

		public void Create()
		{
			_modelsSlowmotionsUsed = new Dictionary<int, bool>();
			_activateSlowmotionSound.Create();
			_disableSlowmotionSound.Create();
		}

		public void Initialize()
		{
			_modelsSlowmotionsUsed.Clear();
			_activateSlowmotionSound.Initialize();
			_disableSlowmotionSound.Initialize();
			active = false;
			_counter = 0;
		}

		public void Play()
		{
		}

		public void Play(Model model)
		{
			if (!active)
			{
				if (!_modelsSlowmotionsUsed.ContainsKey(model.id))
				{
					_modelsSlowmotionsUsed.Add(model.id, false);
				}
				if (!_modelsSlowmotionsUsed[model.id])
				{
					_modelsSlowmotionsUsed[model.id] = true;
					_playStart = true;
					_playEnd = false;
					_counter = 0;
					active = true;
					_activateSlowmotionSound.Play();
				}
			}
		}

		public void StopForce()
		{
			active = false;
			_playStart = false;
			_playEnd = false;
			GameTimeController.ChangeGameTime(1f);
			AudioManager.Instance.SetPitch(1f);
		}

		public void Reset()
		{
			_modelsSlowmotionsUsed.Clear();
		}

		public void Stop()
		{
			if (!_playEnd)
			{
				_playEnd = true;
				_playStart = false;
				_counter = 0;
				_disableSlowmotionSound.Play();
			}
		}

		public void Update()
		{
			if (_playStart)
			{
				_counter++;
				float num = Mathf.Lerp(1f, timeScaleValue, inOutCurve.Evaluate(_counter / inOutFrames));
				GameTimeController.ChangeGameTime(num);
				AudioManager.Instance.SetPitch(num);
				if (_counter >= inOutFrames)
				{
					_playStart = false;
				}
			}
			else if (_playEnd)
			{
				_counter++;
				float num2 = Mathf.Lerp(timeScaleValue, 1f, inOutCurve.Evaluate(_counter / inOutFrames));
				GameTimeController.ChangeGameTime(num2);
				AudioManager.Instance.SetPitch(num2);
				if (_counter >= inOutFrames)
				{
					StopForce();
				}
			}
		}
	}
}
