using UnityEngine;

namespace SF3.Effects
{
	public class ParticleSystemCtrl : MonoBehaviour
	{
		private bool _unscaledUpdate;

		private ParticleSystem[] _particles;

		private void Awake()
		{
			_particles = GetComponentsInChildren<ParticleSystem>();
		}

		private void Update()
		{
			if (_unscaledUpdate && !GameTimeController.systemPaused)
			{
				if (GameTimeController.timeScale < 0.01f)
				{
					_particles[0].Simulate(GameTimeController.unscaledDeltaTime, true, false);
				}
				else
				{
					UnPauseParticles();
				}
			}
		}

		private void UnPauseParticles()
		{
			if (_particles != null && _particles[0].isPaused)
			{
				_particles[0].Play();
			}
		}

		public void SetUnscaledUpdate(bool enableUnscaledUpdate)
		{
			_unscaledUpdate = enableUnscaledUpdate;
			if (!_unscaledUpdate)
			{
				UnPauseParticles();
			}
		}
	}
}
