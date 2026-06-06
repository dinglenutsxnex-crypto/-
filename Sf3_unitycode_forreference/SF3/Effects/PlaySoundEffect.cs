using System;
using SF3.Audio;
using UnityEngine;

namespace SF3.Effects
{
	[Serializable]
	public class PlaySoundEffect : IGameEffect
	{
		[SerializeField]
		private string clipName;

		public void Create()
		{
			AudioManager.Instance.LoadSound(clipName);
		}

		public void Initialize()
		{
		}

		public void Play()
		{
			AudioManager.Instance.PlaySound(clipName);
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
	}
}
