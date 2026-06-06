using System;
using System.Collections.Generic;
using SF3.GameModels;
using SF3.Moves;
using UnityEngine;

namespace SF3.Effects
{
	public class EffectsManager : MonoBehaviour, ISceneInitializationObject
	{
		[Serializable]
		public class ShaderReplacementEffect
		{
			public Material material;

			public string name;

			public List<MaterialUtility.CharacterSkinType> types;
		}

		private static EffectsManager _instance;

		[SerializeField]
		private GameTimeEffects _gameTimeEffects;

		[SerializeField]
		private AnimationEffects _animationEffects;

		[SerializeField]
		private CameraMotionEffects _cameraMotionEffects;

		private List<IGameEffectGeneral> _effectsGeneral;

		public float audioSlowmotionScale = 0.4f;

		private bool _effectsEnable;

		public List<ShaderReplacementEffect> shaderReplacementEffects;

		public static EffectsManager Instance
		{
			get
			{
				return _instance;
			}
		}

		public static bool slowMotionActive
		{
			get
			{
				return _instance._gameTimeEffects.slowMotionActive;
			}
		}

		public static bool freezeFrameActive
		{
			get
			{
				return _instance._gameTimeEffects.freezeFrameActive;
			}
		}

		public AnimationCurve slowmotionCurve
		{
			get
			{
				return _gameTimeEffects.slowmotionCurve;
			}
		}

		private void Awake()
		{
			_instance = this;
			_effectsEnable = false;
			_gameTimeEffects.Create();
			_animationEffects.Create(base.transform);
			_cameraMotionEffects.Create();
			_effectsGeneral = new List<IGameEffectGeneral> { _animationEffects, _gameTimeEffects, _cameraMotionEffects };
		}

		public void Initialize()
		{
			foreach (IGameEffectGeneral item in _effectsGeneral)
			{
				item.Initialize();
			}
			_effectsEnable = false;
		}

		public void DisposePreviousLocation()
		{
		}

		public void DisposeEffectsByModel(int modelUsedID)
		{
			foreach (IGameEffectGeneral item in _effectsGeneral)
			{
				item.DisposeEffectsByModel(modelUsedID);
			}
		}

		public void PrecreateEffect(string effectName, int count, bool isSingleInstance, int modelUsedID)
		{
			_animationEffects.PrecreateEffect(effectName, count, isSingleInstance, modelUsedID);
		}

		public void PrecreateEffect(string effectName, int count, int modelUsedID)
		{
			PrecreateEffect(effectName, count, false, modelUsedID);
		}

		public void PrecreateEffect(string effectName, int modelUsedID)
		{
			PrecreateEffect(effectName, 1, false, modelUsedID);
		}

		private void Update()
		{
			if (_effectsEnable && !GameTimeController.systemPaused)
			{
				_gameTimeEffects.Update();
				_cameraMotionEffects.Update();
			}
		}

		public static void Reset()
		{
			foreach (IGameEffectGeneral item in _instance._effectsGeneral)
			{
				item.StopAll();
			}
		}

		public static void Reset(string effectName)
		{
			foreach (IGameEffectGeneral item in _instance._effectsGeneral)
			{
				item.Reset(effectName);
			}
		}

		public void EffectsEnabling(bool enable)
		{
			_effectsEnable = enable;
			if (!_effectsEnable)
			{
				Reset();
			}
		}

		public static void StopAll(string effectName, int modelID = -1)
		{
			foreach (IGameEffectGeneral item in _instance._effectsGeneral)
			{
				item.Stop(effectName, modelID);
			}
		}

		public static void ShakeCamera(float duration, Vector3 amplitude, Vector3 period)
		{
			_instance._cameraMotionEffects.Shake(duration, amplitude, period);
		}

		public static void DeteleEffectFromStack(GUIEffect item)
		{
			_instance._animationEffects.DeleteEffectFromStack(item);
		}

		public static void PlayEffect(string name, TriggerActionFreezeFrame action)
		{
			_instance._gameTimeEffects.Play(name, action);
		}

		public static void PlayEffect(string name, TriggerActionShakeCamera action)
		{
			_instance._cameraMotionEffects.Play(name, action);
		}

		public static void PlayEffect(Model model, string name, int modelUsedID)
		{
			_instance._animationEffects.PlayEffect(model, name, modelUsedID);
			_instance._gameTimeEffects.Play(model, name);
			_instance._cameraMotionEffects.Play(model, name);
		}

		public static void PlayEffect(Model model, string name, bool leftSide, string alias, string[] values, int modelUsedID)
		{
			_instance._animationEffects.PlayEffect(model, name, leftSide, alias, values, modelUsedID);
			_instance._gameTimeEffects.Play(name);
			_instance._cameraMotionEffects.Play(name);
		}

		public static void PlayHitEffect(Model model, string name, Vector3 atPos, Vector3 angle, int maxParticles, bool follow, int modelUsedID)
		{
			_instance._animationEffects.PlayHitEffect(model, name, atPos, angle, maxParticles, follow, modelUsedID);
		}

		public static void PlayCustomEffect(Model model, string name, Vector3 angle, TriggerCustomEffect effectTrigger)
		{
			_instance._animationEffects.PlayCustomEffect(model, name, angle, effectTrigger);
			_instance._gameTimeEffects.Play(name);
			_instance._cameraMotionEffects.Play(name);
		}

		public static void PlayCustomEffect(Model model, string name, string boneName)
		{
			_instance._animationEffects.PlayCustomEffect(model, name, Vector3.zero, EPlayerType.This, "pelvis", true, true, Vector3.zero, Vector3.zero, false);
			_instance._gameTimeEffects.Play(name);
			_instance._cameraMotionEffects.Play(name);
		}
	}
}
