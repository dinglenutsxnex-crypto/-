using System;
using System.Collections.Generic;
using SF3.GameModels;
using SF3.Moves;
using UnityEngine;

namespace SF3.Effects
{
	[Serializable]
	public class CameraMotionEffects : IGameEffectGeneral
	{
		private CameraShakeEffect _cameraShakeEffect;

		[SerializeField]
		private DollyZoomEffect _dollyZoomEffect;

		private Dictionary<string, IGameEffect> _effects;

		private Camera _camera;

		public float originalFOV { get; private set; }

		public void Create()
		{
			_camera = Camera.main;
			_effects = new Dictionary<string, IGameEffect>();
			_cameraShakeEffect = new CameraShakeEffect();
			_effects.Add("CameraShakeEffect", _cameraShakeEffect);
			_effects.Add("DollyZoom", _dollyZoomEffect);
			foreach (KeyValuePair<string, IGameEffect> effect in _effects)
			{
				effect.Value.Create();
			}
		}

		public void Initialize()
		{
			originalFOV = _camera.fieldOfView;
			foreach (KeyValuePair<string, IGameEffect> effect in _effects)
			{
				effect.Value.Initialize();
			}
		}

		public void DisposeEffectsByModel(int modelID)
		{
		}

		public void Stop(string effectName, int modelUsedID)
		{
			if (_effects.ContainsKey(effectName))
			{
				_effects[effectName].Stop();
			}
		}

		public bool Play(string name, TriggerActionShakeCamera actShakeCamera)
		{
			_cameraShakeEffect.StartShake(actShakeCamera);
			return true;
		}

		public void Shake(float duration, Vector3 amplitude, Vector3 period)
		{
			_cameraShakeEffect.StartShake(duration, amplitude, period);
		}

		public bool Play(Model model, string name)
		{
			if (name.Equals("DollyZoom"))
			{
				_dollyZoomEffect.Play(model);
				return true;
			}
			return false;
		}

		public void Play(string name, int modelid = -1)
		{
		}

		public void Update()
		{
			if (_cameraShakeEffect != null)
			{
				_cameraShakeEffect.Update();
			}
			_dollyZoomEffect.Update();
			_camera.transform.localPosition = _cameraShakeEffect.shift + _dollyZoomEffect.shift;
			_camera.fieldOfView = originalFOV + _dollyZoomEffect.FOV;
		}

		public void Reset(string effectName)
		{
			if (_effects.ContainsKey(effectName))
			{
				_effects[effectName].Reset();
			}
		}

		public void Stop(string effectName, bool forced = false)
		{
			if (_effects.ContainsKey(effectName))
			{
				_effects[effectName].Stop();
			}
		}

		public void StopAll(bool forced = false)
		{
			foreach (KeyValuePair<string, IGameEffect> effect in _effects)
			{
				effect.Value.Reset();
			}
		}
	}
}
