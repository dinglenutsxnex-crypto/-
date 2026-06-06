using System.Collections;
using System.Collections.Generic;
using SF3.Effects;
using UnityEngine;

namespace SF3.Audio
{
	public class AudioManager : MonoBehaviour
	{
		private Dictionary<string, AudioSource> sources;

		private List<GameObject> sounds;

		private AudioSourceSettings audioSettings;

		private static bool muted;

		private static float pitch;

		private const string SOUNDS_VOLUME_KEY = "sounds volume";

		private const string MUSIC_VOLUME_KEY = "music volume";

		public static AudioManager Instance { get; private set; }

		public static float volume { get; private set; }

		public static float musicVolume { get; private set; }

		public static void Initialize()
		{
			if (Instance == null)
			{
				Instance = new GameObject("AudioManager").AddComponent<AudioManager>();
				StaticObjectsManager.AddObject(Instance.gameObject);
				Instance.sounds = new List<GameObject>();
				Instance.sources = new Dictionary<string, AudioSource>();
				muted = false;
				volume = PlayerPrefs.GetFloat("sounds volume", 1f);
				musicVolume = PlayerPrefs.GetFloat("music volume", 1f);
				pitch = 1f;
			}
		}

		public void SetAudioSettings(AudioSourceSettings sourceSettings)
		{
			audioSettings = sourceSettings;
			foreach (AudioSource value in sources.Values)
			{
				audioSettings.ApplySettings(value);
			}
		}

		public void SetPitch(float pitchValue)
		{
			Routiner.Go(SetPitchRoutine());
		}

		private IEnumerator SetPitchRoutine()
		{
			yield return new WaitForEndOfFrame();
			while (!EffectsManager.Instance)
			{
				yield return new WaitForEndOfFrame();
			}
			if (sources == null)
			{
				yield break;
			}
			foreach (AudioSource value in sources.Values)
			{
				value.pitch = pitch;
			}
		}

		public void Mute(bool mute)
		{
			AudioListener.volume = ((!mute) ? volume : 0f);
		}

		public void SetVolume(float volumeValue, bool save = false)
		{
			volume = volumeValue;
			foreach (AudioSource value in sources.Values)
			{
				value.volume = volume;
			}
			if (save)
			{
				PlayerPrefs.SetFloat("sounds volume", volumeValue);
			}
		}

		public void SetMusicVolume(float volumeValue)
		{
			musicVolume = volumeValue;
			LocationAudioSettings.SetVolume(volumeValue);
			PlayerPrefs.SetFloat("music volume", musicVolume);
		}

		public void LoadSound(params string[] sounds)
		{
			foreach (string text in sounds)
			{
				if (!text.IsNullOrEmpty())
				{
					LoadSound(text);
				}
			}
		}

		public AudioSource LoadSound(string clipName)
		{
			return LoadAudio("sounds", clipName);
		}

		private AudioSource LoadAudio(string audioTypePath, string clipName)
		{
			if (clipName.IsNullOrEmpty())
			{
				return null;
			}
			if (!sources.ContainsKey(clipName))
			{
				AudioClip loadAudioClipInternal = GlobalLoad.GetLoadAudioClipInternal(audioTypePath, clipName);
				AudioSource audioSource = CreateAudioSource();
				audioSource.clip = loadAudioClipInternal;
				sources.Add(clipName, audioSource);
				return audioSource;
			}
			return sources[clipName];
		}

		private AudioSource CreateAudioSource()
		{
			GameObject gameObject = new GameObject("Sound");
			gameObject.transform.parent = base.transform;
			sounds.Add(gameObject);
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.playOnAwake = false;
			audioSource.mute = muted;
			audioSource.volume = volume;
			audioSource.pitch = pitch;
			if (audioSettings != null)
			{
				audioSettings.ApplySettings(audioSource);
			}
			return audioSource;
		}

		public void PlaySound(string audioName, float spatialBlend = 0f)
		{
			PlaySound(audioName, base.transform.position, spatialBlend);
		}

		public void PlaySound(string audioName, Vector3 pos, float spatialBlend = 0f)
		{
			if (audioName.IsNullOrEmpty())
			{
				Debug.LogWarning("Audio name is null or empty");
				return;
			}
			if (!sources.ContainsKey(audioName) && LoadSound(audioName) == null)
			{
				Debug.LogWarning("Can't find audio with name " + audioName);
				return;
			}
			sources[audioName].transform.position = pos;
			sources[audioName].spatialBlend = spatialBlend;
			sources[audioName].Play();
		}

		public void ClearAllSound()
		{
			foreach (GameObject sound in sounds)
			{
				Object.Destroy(sound);
			}
			sources.Clear();
			sounds.Clear();
		}
	}
}
