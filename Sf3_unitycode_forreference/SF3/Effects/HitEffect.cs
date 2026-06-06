using SF3.GameModels;
using UnityEngine;

namespace SF3.Effects
{
	public class HitEffect : GameEffectBase
	{
		private ParticleSystem particlesEffect;

		public float duration;

		public bool overrideDuration;

		private float _timer;

		private bool _checkOnDisable;

		private bool _follow;

		private Capsule _attachedTo;

		private Vector3 _offsetFromCapsule;

		private Animator[] _animatorsComponents;

		protected override void Awake()
		{
			particlesEffect = GetComponent<ParticleSystem>();
			if (!overrideDuration)
			{
				duration = GetDuration(particlesEffect);
			}
			_animatorsComponents = base.transform.GetComponentsInChildren<Animator>();
			base.Awake();
		}

		private float GetDuration(ParticleSystem system)
		{
			return system.main.duration;
		}

		public override void Play(Model m, Vector3 atPos, Vector3 angles, int maxParticles, bool follow)
		{
			SetMaxParticles(particlesEffect, maxParticles);
			base.Play(m, atPos, angles);
			particlesEffect.Clear(true);
			particlesEffect.Stop(true);
			particlesEffect.Play(true);
			_checkOnDisable = true;
			_timer = 0f;
			_follow = follow;
			if (follow)
			{
				_attachedTo = Model.hitResult.StrikeData.collisionEdge;
				_offsetFromCapsule = _attachedTo.Center - atPos;
			}
			Animator[] animatorsComponents = _animatorsComponents;
			foreach (Animator animator in animatorsComponents)
			{
				animator.enabled = true;
			}
		}

		private void SetMaxParticles(ParticleSystem system, int count)
		{
			ParticleSystem.MainModule main = system.main;
			main.maxParticles = count;
		}

		public override void Disable()
		{
			Animator[] animatorsComponents = _animatorsComponents;
			foreach (Animator animator in animatorsComponents)
			{
				animator.enabled = false;
			}
			obj.SetActive(false);
		}

		protected override void OnUpdate()
		{
			if (_checkOnDisable)
			{
				if (_follow)
				{
					transf.position = _attachedTo.Center + _offsetFromCapsule;
				}
				_timer += GameTimeController.deltaTime;
				if (_timer >= duration)
				{
					_checkOnDisable = false;
					Disable();
				}
			}
		}
	}
}
