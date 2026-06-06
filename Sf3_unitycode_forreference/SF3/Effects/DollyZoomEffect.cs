using System;
using System.Collections.Generic;
using SF3.Audio;
using SF3.GameModels;
using UnityEngine;

namespace SF3.Effects
{
	[Serializable]
	public class DollyZoomEffect : IGameEffect
	{
		public string name;

		public int inFrames;

		public int outFrames;

		public AnimationCurve inCurve;

		public AnimationCurve outCurve;

		public AnimationCurve inZoom;

		public AnimationCurve outZoom;

		public float cameraMaxFOVChange;

		public float cameraInitFOVChange;

		private float _height;

		private float _originalFOV;

		private float _lastMaxFOV;

		private float _lastFOV;

		private float _additionalFOV;

		private float _lastMaxAdditionalFOV;

		private float _initDistance;

		private int _counter;

		private bool _playStart;

		private bool _playEnd;

		private Dictionary<int, bool> _modelsDollyZoomUsed;

		public Vector3 shift { get; private set; }

		public float FOV { get; private set; }

		public bool active { get; private set; }

		public void Create()
		{
			_modelsDollyZoomUsed = new Dictionary<int, bool>();
		}

		public void Initialize()
		{
			_modelsDollyZoomUsed.Clear();
			shift = Vector3.zero;
			FOV = 0f;
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
				if (!_modelsDollyZoomUsed.ContainsKey(model.id))
				{
					_modelsDollyZoomUsed.Add(model.id, false);
				}
				if (!_modelsDollyZoomUsed[model.id])
				{
					_modelsDollyZoomUsed[model.id] = true;
					EffectsManager.StopAll("DollyZoom");
					_playStart = true;
					_playEnd = false;
					_counter = 0;
					active = true;
					_originalFOV = Camera.main.fieldOfView;
					_initDistance = Mathf.Abs(Camera.main.transform.position.z - SceneConfig.SpawnPointPlayer.z);
					_height = GetHeight(_initDistance, _originalFOV);
				}
			}
		}

		public void StopForce()
		{
			if (active)
			{
				active = false;
				_playStart = false;
				_playEnd = false;
				FOV = 0f;
				shift = Vector3.zero;
				GameTimeController.ChangeGameTime(1f);
				AudioManager.Instance.SetPitch(1f);
			}
		}

		public void Reset()
		{
			StopForce();
			_modelsDollyZoomUsed.Clear();
		}

		public void Stop()
		{
			if (!_playEnd && active)
			{
				_lastMaxFOV = FOV - _additionalFOV;
				_lastMaxAdditionalFOV = _additionalFOV;
				_playEnd = true;
				_playStart = false;
				_counter = 0;
			}
		}

		public void Update()
		{
			if (_playStart)
			{
				FOV = Mathf.Lerp(0f, cameraMaxFOVChange, inCurve.Evaluate((float)_counter / (float)inFrames));
				_additionalFOV = Mathf.Lerp(0f, cameraInitFOVChange, inZoom.Evaluate((float)_counter / (float)inFrames));
				FOV += _additionalFOV;
				shift = Vector3.forward * (_initDistance - GetDistanceFromFOVAndHeight(FOV - _additionalFOV + _originalFOV, _height));
				_counter++;
				if (_counter >= inFrames)
				{
					_playStart = false;
				}
			}
			else if (_playEnd)
			{
				if (ModelsManager.Instance.Player != null)
				{
					FOV = Mathf.Lerp(_lastMaxFOV, 0f, outCurve.Evaluate((float)_counter / (float)outFrames));
					_additionalFOV = Mathf.Lerp(_lastMaxAdditionalFOV, 0f, outZoom.Evaluate((float)_counter / (float)outFrames));
					FOV += _additionalFOV;
					shift = Vector3.forward * (_initDistance - GetDistanceFromFOVAndHeight(FOV - _additionalFOV + _originalFOV, _height));
				}
				_counter++;
				if (_counter >= outFrames)
				{
					StopForce();
				}
			}
		}

		private float GetHeight(float distance, float FOV)
		{
			return 2f * distance * Mathf.Tan(FOV * 0.5f * ((float)Math.PI / 180f));
		}

		private float GetFOV(float _height, float distance)
		{
			return 2f * Mathf.Atan(_height * 0.5f / distance) * 57.29578f;
		}

		private float GetDistanceFromFOVAndHeight(float fov, float _height)
		{
			return 0.5f * _height / Mathf.Tan(fov * ((float)Math.PI / 180f) / 2f);
		}
	}
}
