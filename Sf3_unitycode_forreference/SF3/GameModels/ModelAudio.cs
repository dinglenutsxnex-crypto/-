using UnityEngine;

namespace SF3.GameModels
{
	public class ModelAudio
	{
		private AudioSource _modelAudio;

		public ModelAudio(AudioSource audio)
		{
			_modelAudio = audio;
			_modelAudio.playOnAwake = false;
		}

		public void PlaySound(AudioClip clip)
		{
			_modelAudio.clip = clip;
			_modelAudio.Play();
		}
	}
}
