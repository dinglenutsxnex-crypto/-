using UnityEngine;

namespace SF3
{
	public class GlowEffectController : MonoBehaviour
	{
		private OptimizedGlow glowEffect;

		private static GlowEffectController _instance;

		private bool glowDisabled;

		private bool shadowFromActive;

		public static GlowEffectController instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Camera.main.gameObject.AddComponent<GlowEffectController>();
					_instance.InitGlow();
				}
				return _instance;
			}
		}

		private void Start()
		{
			if (_instance != this)
			{
				Object.Destroy(this);
				return;
			}
			_instance = this;
			InitGlow();
		}

		private void InitGlow()
		{
			glowEffect = Camera.main.GetComponent<OptimizedGlow>();
		}

		public void SetBlurIterations(int blurIterations)
		{
			glowEffect.blurIterations = blurIterations;
		}

		public void SetSimpleGlow()
		{
			glowEffect.enabled = false;
			glowEffect.enabled = true;
		}

		public void ToggleGlow(bool glowEnabled)
		{
			glowDisabled = !glowEnabled;
			if (shadowFromActive)
			{
				if (glowDisabled)
				{
					DisableGlow();
				}
				else
				{
					EnableGlow();
				}
			}
		}

		public void EnableGlow()
		{
			if (!glowDisabled)
			{
				shadowFromActive = true;
				glowEffect.enabled = true;
			}
		}

		public void DisableGlow()
		{
			shadowFromActive = false;
			glowEffect.enabled = false;
		}

		private void OnApplicationFocus(bool focused)
		{
			if (shadowFromActive)
			{
				glowEffect.enabled = false;
				glowEffect.enabled = true;
			}
		}
	}
}
