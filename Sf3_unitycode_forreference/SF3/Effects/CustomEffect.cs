using System;
using SF3.GameModels;
using UnityEngine;

namespace SF3.Effects
{
	public class CustomEffect : GameEffectBase
	{
		[Serializable]
		public class DelayedEffectActivation
		{
			public GameObject effect;

			public float delay;

			private float _nextEffectActivation;

			private bool _done;

			public void Init()
			{
				effect.SetActive(false);
				_nextEffectActivation = GameTimeController.battleTime + delay;
				_done = false;
			}

			public void Update()
			{
				if (!_done && _nextEffectActivation < GameTimeController.battleTime)
				{
					effect.SetActive(true);
					_done = true;
				}
			}
		}

		public bool takeInAccountCharacterDirection;

		[SerializeField]
		private bool _flipByAccountCharacterDirection;

		private ParticleSystem _particlesEffect;

		public float duration;

		public bool overrideDuration;

		private Vector3 _followAxis;

		private float _timer;

		private bool _checkOnDisable;

		private Bone _bone;

		private Model _model;

		private string _attachToBone;

		private bool _follow;

		private Vector3 _shift;

		private Vector3 _currentShift;

		private TrailRenderer[] _trails;

		public DelayedEffectActivation[] delayedEffects;

		private EDirectionType _currentParentModelDirection;

		private bool attachToLocal;

		protected override void Awake()
		{
			base.Awake();
			_particlesEffect = GetComponent<ParticleSystem>();
			ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
			if (!overrideDuration)
			{
				duration = 0f;
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					float num = GetDuration(componentsInChildren[i]);
					if (duration < num)
					{
						duration = num;
					}
				}
			}
			_trails = GetComponentsInChildren<TrailRenderer>();
		}

		private static float GetDuration(ParticleSystem system)
		{
			return system.main.duration;
		}

		private void OnDisable()
		{
			if (_trails != null)
			{
				TrailRenderer[] trails = _trails;
				foreach (TrailRenderer trailRenderer in trails)
				{
					trailRenderer.Clear();
				}
			}
			if (_model != null)
			{
				_model.moveControl.OnDirectionChangedEvent -= OnModelDirectionChanged;
			}
		}

		public override void Play(Model m, Vector3 atPos, Vector3 freezeAxisValue)
		{
			base.Play(m, atPos);
			_timer = 0f;
			_checkOnDisable = true;
			InitDelayedEffects();
			_followAxis = freezeAxisValue;
		}

		private void InitDelayedEffects()
		{
			if (delayedEffects != null)
			{
				DelayedEffectActivation[] array = delayedEffects;
				foreach (DelayedEffectActivation delayedEffectActivation in array)
				{
					delayedEffectActivation.Init();
				}
			}
		}

		public override void Play(Model m, Vector3 atPos)
		{
			Play(m, atPos, Vector3.zero);
		}

		public override void Play(Model m, Vector3 angle, bool applyToEnemy, string attachToBone, bool loop, bool follow, Vector3 shift, Vector3 followAxis, bool attachLocal)
		{
			base.Play(m, Vector3.zero);
			if (_particlesEffect != null)
			{
				EnableEmission(_particlesEffect);
				ParticleSystem.MainModule main = _particlesEffect.main;
				main.loop = loop;
				_particlesEffect.Clear(true);
				_particlesEffect.Stop(true);
				_particlesEffect.Play(true);
			}
			attachToLocal = attachLocal;
			_attachToBone = attachToBone;
			_follow = follow;
			_shift = shift;
			_model = ((!applyToEnemy) ? m : model.enemy);
			UpdateBone();
			if (!loop)
			{
				_timer = 0f;
				_checkOnDisable = true;
			}
			InitDelayedEffects();
			_followAxis = followAxis;
			_model.moveControl.OnDirectionChangedEvent += OnModelDirectionChanged;
			OnModelDirectionChanged(_model.moveControl.moveDirection);
			transf.position = _bone.transform.position + _currentShift;
			BlackFireSkin component = GetComponent<BlackFireSkin>();
			if (component != null)
			{
				component.Init(_bone.transform.gameObject, model.GetBone("pelvis").transform, m);
			}
		}

		private void DisableEmission(ParticleSystem particlesEffect)
		{
			EnableEmission(particlesEffect, false);
		}

		private void EnableEmission(ParticleSystem particlesEffect, bool enable = true)
		{
			ParticleSystem.EmissionModule emission = particlesEffect.emission;
			emission.enabled = enable;
		}

		public override void Play(Model m, Vector3 angle, bool applyToEnemy, string attachToBone, bool loop, bool follow, Vector3 shift)
		{
			Play(m, angle, applyToEnemy, attachToBone, loop, follow, shift, Vector3.zero, false);
		}

		private void UpdateBone()
		{
			_bone = model.GetBone(_attachToBone);
		}

		private void OnModelDirectionChanged(EDirectionType newDirection)
		{
			_currentShift = _shift;
			Vector3 localEulerAngles = defaultAngles;
			Vector3 localScale = defaultScale;
			if (newDirection != EDirectionType.RIGHT && newDirection == EDirectionType.LEFT)
			{
				if (takeInAccountCharacterDirection)
				{
					localEulerAngles.y = 180f - localEulerAngles.y;
					_currentShift.x *= -1f;
					localScale.z = 0f - localScale.z;
				}
				if (_flipByAccountCharacterDirection)
				{
					localScale.x = 0f - localScale.x;
				}
			}
			transf.localEulerAngles = localEulerAngles;
			transf.localScale = localScale;
		}

		protected override void OnUpdate()
		{
			if (delayedEffects != null)
			{
				DelayedEffectActivation[] array = delayedEffects;
				foreach (DelayedEffectActivation delayedEffectActivation in array)
				{
					delayedEffectActivation.Update();
				}
			}
			if (_follow && _bone != null && _bone.transform != null)
			{
				Vector3 position = _bone.transform.position + _currentShift;
				if (_followAxis.x > 1f || _followAxis.x < 1f)
				{
					position.x = transf.position.x;
				}
				if (_followAxis.y > 1f || _followAxis.y < 1f)
				{
					position.y = transf.position.y;
				}
				if (_followAxis.z > 1f || _followAxis.z < 1f)
				{
					position.z = transf.position.z;
				}
				transf.position = position;
			}
			if (_checkOnDisable)
			{
				_timer += GameTimeController.deltaTime;
				if (_timer >= duration)
				{
					_checkOnDisable = false;
					obj.SetActive(false);
				}
			}
			else if (_follow && (model == null || !model.active))
			{
				_timer = 0f;
				_checkOnDisable = true;
				if (_particlesEffect != null)
				{
					DisableEmission(_particlesEffect);
				}
			}
			if (attachToLocal)
			{
				base.transform.position = _bone.transform.position + defaultPos;
				base.transform.rotation = _bone.transform.rotation * Quaternion.Euler(defaultAngles);
			}
		}
	}
}
