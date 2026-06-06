using SF3.GameModels;
using UnityEngine;

namespace SF3.Effects
{
	public abstract class GameEffectBase : MonoBehaviour, IGameEffect
	{
		protected Transform transf;

		protected GameObject obj;

		protected Vector3 defaultAngles;

		protected Vector3 defaultScale;

		protected Vector3 defaultPos;

		public bool ignoreTimeScale;

		public Model model;

		public bool IsReady
		{
			get
			{
				return !obj.activeSelf;
			}
		}

		protected virtual void Awake()
		{
			transf = base.transform;
			obj = base.gameObject;
			defaultPos = transf.localPosition;
			defaultAngles = transf.localEulerAngles;
			defaultScale = transf.localScale;
			Disable();
			if ((bool)base.gameObject.GetComponent<ParticleSystem>())
			{
				ParticleSystemCtrl particleSystemCtrl = base.gameObject.GetComponent<ParticleSystemCtrl>();
				if (particleSystemCtrl == null)
				{
					particleSystemCtrl = base.gameObject.AddComponent<ParticleSystemCtrl>();
				}
				particleSystemCtrl.SetUnscaledUpdate(ignoreTimeScale);
			}
		}

		public void Create()
		{
		}

		public void Initialize()
		{
		}

		public void Play()
		{
		}

		public void Stop()
		{
		}

		public void StopForce()
		{
		}

		public void Reset()
		{
		}

		public virtual void Play(Model model)
		{
			this.model = model;
		}

		public virtual void Play(Model model, bool leftSide)
		{
		}

		public virtual void Play(Model model, bool leftSide, string alias, string[] values)
		{
		}

		public virtual void Play(Model model, bool leftSide, float yPos)
		{
		}

		public virtual void Play(Model model, bool leftSide, float yPos, string alias, string[] values)
		{
		}

		public virtual void Play(Model model, Vector3 atPos, Vector3 angles, int maxParticles, bool follow)
		{
			Play(model, atPos, angles, maxParticles);
		}

		public virtual void Play(Model model, Vector3 atPos, Vector3 angles, int maxParticles)
		{
			Play(model, atPos, angles);
		}

		public virtual void Play(Model model, Vector3 angle, bool applyToEnemy, string attachToBone, bool loop, bool follow, Vector3 shift)
		{
			Play(model, Vector3.zero);
		}

		public virtual void Play(Model model, Vector3 angle, bool applyToEnemy, string attachToBone, bool loop, bool follow, Vector3 shift, Vector3 followAxis, bool attachLocal)
		{
			Play(model, Vector3.zero);
		}

		public virtual void Play(Model model, Vector3 atPos, Vector3 angles, int maxParticles, string boneName, Vector3 offset, bool onEnemy, bool detached)
		{
			Play(model, atPos, angles);
		}

		public virtual void Play(Model model, Vector3 atPos, Vector3 angles)
		{
			Play(model, atPos);
			transf.localEulerAngles = defaultAngles + angles;
		}

		public virtual void Play(Model model, Vector3 atPos, Quaternion _rotation)
		{
			Play(model, atPos);
			transf.rotation = _rotation;
		}

		public virtual void Play(Model model, Vector3 atPos)
		{
			transf.position = atPos;
			this.model = model;
			obj.SetActive(true);
		}

		public virtual void Disable()
		{
			obj.SetActive(false);
		}

		private void Update()
		{
			if (!GameTimeController.systemPaused)
			{
				OnUpdate();
			}
		}

		protected virtual void OnUpdate()
		{
		}
	}
}
