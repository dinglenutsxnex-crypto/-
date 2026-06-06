using System;
using System.Collections.Generic;
using System.Linq;
using SF3.GameModels;
using SF3.Moves;
using UnityEngine;

namespace SF3.Effects
{
	[Serializable]
	public class AnimationEffects : IGameEffectGeneral
	{
		private class EffectsPullObject
		{
			public HashSet<int> modelsUsed;

			public List<GameEffectBase> effects;

			public bool isSingleInstance { get; private set; }

			public EffectsPullObject(bool isSingle)
			{
				isSingleInstance = isSingle;
				modelsUsed = new HashSet<int>();
				effects = new List<GameEffectBase>();
			}

			public void RemoveForModel(int modelID)
			{
				modelsUsed.Remove(modelID);
				if (modelsUsed.Count == 0)
				{
					foreach (GameEffectBase effect in effects)
					{
						UnityEngine.Object.Destroy(effect.gameObject);
					}
					effects.Clear();
				}
				else if (!isSingleInstance)
				{
					int num = effects.Count / (modelsUsed.Count + 1);
					for (int i = 0; i < num; i++)
					{
						UnityEngine.Object.Destroy(effects[i].gameObject);
					}
					effects.RemoveRange(0, num);
				}
			}
		}

		private class HitEffectStack
		{
			private List<GUIEffect> _effects;

			private float[] _positions;

			public HitEffectStack(float[] positions)
			{
				_positions = positions;
				_effects = new List<GUIEffect>();
			}

			public void Remove(GUIEffect item)
			{
				if (item.name.Contains("Combo"))
				{
				}
				_effects.Remove(item);
				Refresh();
			}

			public float Add(GUIEffect item)
			{
				if (item.name.Contains("Combo"))
				{
				}
				int num = _effects.FindIndex((GUIEffect effect) => effect.priorityInStack <= item.priorityInStack);
				if (num == -1)
				{
					num = _effects.Count;
					_effects.Add(item);
				}
				else
				{
					_effects.Insert(num, item);
				}
				Refresh(item);
				num = Mathf.Clamp(num, 0, _positions.Length - 1);
				return _positions[num];
			}

			private void Refresh(GUIEffect item = null)
			{
				for (int i = 0; i < _effects.Count && i < _positions.Length; i++)
				{
					if (!_effects[i].Equals(item))
					{
						_effects[i].MoveTo(_positions[i]);
					}
				}
			}
		}

		[SerializeField]
		private List<GameObject> _prefabEffects;

		[SerializeField]
		private float[] _hitEffectsPositions;

		private HitEffectStack _leftHitEffectStack;

		private HitEffectStack _rightHitEffectStack;

		private static Dictionary<string, EffectsPullObject> _effects;

		private Transform _parentTransform;

		public void Create()
		{
		}

		public void Create(Transform transfParent, int count = 1)
		{
			_parentTransform = transfParent;
			_effects = new Dictionary<string, EffectsPullObject>();
			_leftHitEffectStack = new HitEffectStack(_hitEffectsPositions);
			_rightHitEffectStack = new HitEffectStack(_hitEffectsPositions);
		}

		public void Reset(string name)
		{
			if (!_effects.ContainsKey(name))
			{
				return;
			}
			foreach (GameEffectBase effect in _effects[name].effects)
			{
				effect.StopForce();
			}
		}

		public void DisposeEffectsByModel(int modelUsedID)
		{
			for (int i = 0; i < _effects.Count; i++)
			{
				KeyValuePair<string, EffectsPullObject> keyValuePair = _effects.ElementAt(i);
				if (keyValuePair.Value.modelsUsed.Contains(modelUsedID))
				{
					keyValuePair.Value.RemoveForModel(modelUsedID);
					if (keyValuePair.Value.effects.Count == 0)
					{
						_effects.Remove(keyValuePair.Key);
						i--;
					}
				}
			}
		}

		public void PrecreateEffect(string effectName, int count, bool isSingleInstance, int modelUsedID)
		{
			if (count < 1 || effectName.Length == 0)
			{
				return;
			}
			EffectsPullObject effectsPullObject = null;
			GameObject original = null;
			if (!_effects.ContainsKey(effectName))
			{
				_effects.Add(effectName, new EffectsPullObject(isSingleInstance));
			}
			foreach (GameObject prefabEffect in _prefabEffects)
			{
				if (prefabEffect.name.Equals(effectName))
				{
					effectsPullObject = _effects[effectName];
					original = prefabEffect;
					break;
				}
			}
			if (effectsPullObject == null)
			{
				return;
			}
			effectsPullObject.modelsUsed.Add(modelUsedID);
			if (effectsPullObject.effects.Count > 0 && isSingleInstance)
			{
				return;
			}
			for (int i = 0; i < count; i++)
			{
				GameEffectBase component = UnityEngine.Object.Instantiate(original).GetComponent<GameEffectBase>();
				if (!(component is GUIEffect))
				{
					component.transform.parent = _parentTransform;
				}
				effectsPullObject.effects.Add(component);
			}
		}

		public void PrecreateEffect(string effectName, int count, int modelUsedID)
		{
			PrecreateEffect(effectName, count, false, modelUsedID);
		}

		public void Initialize()
		{
			StopAll(true);
		}

		public void Stop(string name, bool forced = false)
		{
			Stop(name, -1);
		}

		public void Stop(string name, int modelID = -1)
		{
			if (!_effects.ContainsKey(name))
			{
				return;
			}
			foreach (GameEffectBase effect in _effects[name].effects)
			{
				if (modelID == -1 || effect.model == null || effect.model.id == modelID)
				{
					effect.Disable();
				}
			}
		}

		public void StopAll(bool forced = false)
		{
			foreach (EffectsPullObject value in _effects.Values)
			{
				foreach (GameEffectBase effect in value.effects)
				{
					effect.Disable();
				}
			}
		}

		private GameEffectBase GetEffectByName(string name, int modelUsedID)
		{
			if (_effects.ContainsKey(name))
			{
				foreach (GameEffectBase effect in _effects[name].effects)
				{
					if (effect.IsReady)
					{
						return effect;
					}
				}
			}
			PrecreateEffect(name, 1, modelUsedID);
			if (_effects.ContainsKey(name))
			{
				foreach (GameEffectBase effect2 in _effects[name].effects)
				{
					if (effect2.IsReady)
					{
						return effect2;
					}
				}
			}
			return null;
		}

		public void DeleteEffectFromStack(GUIEffect item)
		{
			if (item.LeftSide)
			{
				_leftHitEffectStack.Remove(item);
			}
			else
			{
				_rightHitEffectStack.Remove(item);
			}
		}

		public void Play(string name, int modelUsedID)
		{
			GameEffectBase effectByName = GetEffectByName(name, modelUsedID);
			if (effectByName != null)
			{
				effectByName.Play();
			}
		}

		public void PlayEffect(Model model, string name, int modelUsedID)
		{
			GameEffectBase effectByName = GetEffectByName(name, modelUsedID);
			if (effectByName != null)
			{
				effectByName.Play(model);
			}
		}

		public void PlayEffect(Model model, string name, bool leftSide, string alias, string[] values, int modelUsedID)
		{
			GameEffectBase effectByName = GetEffectByName(name, modelUsedID);
			if (effectByName != null)
			{
				if (effectByName is GUIEffect && (effectByName as GUIEffect).usesStack)
				{
					float yPos = ((!leftSide) ? _rightHitEffectStack.Add(effectByName as GUIEffect) : _leftHitEffectStack.Add(effectByName as GUIEffect));
					effectByName.Play(model, leftSide, yPos, alias, values);
				}
				else
				{
					effectByName.Play(model, leftSide, alias, values);
				}
			}
		}

		public void PlayHitEffect(Model model, string name, Vector3 atPos, Vector3 angle, int maxParticles, bool follow, int modelUsedID)
		{
			GameEffectBase effectByName = GetEffectByName(name, modelUsedID);
			if (effectByName != null)
			{
				effectByName.Play(model, atPos, angle, maxParticles, follow);
			}
		}

		public void PlayCustomEffect(Model model, string name, Vector3 angle, TriggerCustomEffect effectTrigger)
		{
			PlayCustomEffect(model, name, angle, effectTrigger.targetType, effectTrigger.attachToBone, effectTrigger.looped, effectTrigger.follow, effectTrigger.shift, effectTrigger.followAxis, effectTrigger.attachToLocal);
		}

		public void PlayCustomEffect(Model model, string name, Vector3 angle, EPlayerType targetType, string bone, bool looped, bool follow, Vector3 shift, Vector3 followAxis, bool attachToLocal)
		{
			GameEffectBase effectByName = GetEffectByName(name, model.id);
			if (effectByName != null)
			{
				effectByName.Play(model, angle, targetType == EPlayerType.Enemy, bone, looped, follow, shift, followAxis, attachToLocal);
			}
		}
	}
}
