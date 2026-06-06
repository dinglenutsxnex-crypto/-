using System;
using UnityEngine;
using UnityEngine.Audio;

namespace SF3.Audio
{
	public class LocationAudioSettings : MonoBehaviour
	{
		[Serializable]
		public class ParamsEQ
		{
			public float centerFreq = 8000f;

			public float octaveRange = 1f;

			public float frequencyGain = 1f;
		}

		public AudioSourceSettings audioSettings;

		public AudioClip shadowFormAmbient;

		public ParamsEQ defaultEQParams;

		public ParamsEQ shadowFormEQParams;

		public AnimationCurve shadowFormEQCurve;

		public float eqAnimationTime;

		public AudioMixerGroup audioMixerGroup;

		private AudioSource _shadowSource;

		private AudioSource _musicSource;

		private float _timer;

		private bool _activateShadow;

		private bool _isActivateShadow;

		private static string _musicName;

		public static LocationAudioSettings Instance { get; private set; }

		public static bool NeedSpecialShadowFormSound { get; set; }

		public static void SetPitch(float pitchValue)
		{
			Instance._musicSource.pitch = pitchValue;
		}

		public static void SetVolume(float volume)
		{
			Instance._musicSource.volume = volume;
			Instance._shadowSource.volume = volume;
		}

		public static void PlayMusicByName(string name = "")
		{
			_musicName = name;
		}

		private void Awake()
		{
			Instance = this;
			AudioManager.Instance.SetAudioSettings(audioSettings);
			shadowFormEQCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
			_activateShadow = false;
			_isActivateShadow = false;
			_shadowSource = CreateAudioSource(shadowFormAmbient);
			_musicSource = CreateAudioSource(GetAudioClip(), audioMixerGroup);
			SetEQParams(_musicSource, defaultEQParams);
			_musicSource.Play();
		}

		private void OnDestroy()
		{
			GlobalLoad.Unload(_musicSource);
			GlobalLoad.Unload(_shadowSource);
		}

		private void Update()
		{
			if (_activateShadow && NeedSpecialShadowFormSound)
			{
				_timer += GameTimeController.deltaTime;
				SetShadowProgerss(_timer);
			}
		}

		private void SetShadowProgerss(float timer)
		{
			float num = shadowFormEQCurve.Evaluate(timer / eqAnimationTime);
			if (!_isActivateShadow)
			{
				num = 1f - num;
			}
			float value = Mathf.Lerp(defaultEQParams.centerFreq, shadowFormEQParams.centerFreq, num);
			float value2 = Mathf.Lerp(defaultEQParams.octaveRange, shadowFormEQParams.octaveRange, num);
			float value3 = Mathf.Lerp(defaultEQParams.frequencyGain, shadowFormEQParams.frequencyGain, num);
			SetMixerFloat(_musicSource, "centerFreq", value);
			SetMixerFloat(_musicSource, "octaveRange", value2);
			SetMixerFloat(_musicSource, "frequencyGain", value3);
			if (timer >= eqAnimationTime)
			{
				_activateShadow = false;
			}
		}

		private AudioSource CreateAudioSource(AudioClip clip = null, AudioMixerGroup mixer = null)
		{
			GameObject gameObject = new GameObject("ShadowFormAmbient");
			gameObject.transform.parent = base.transform;
			gameObject.name = "Audio_" + ((!(clip != null)) ? string.Empty : clip.name);
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.clip = clip;
			audioSource.volume = AudioManager.musicVolume;
			audioSource.playOnAwake = false;
			audioSource.loop = true;
			audioSource.outputAudioMixerGroup = mixer;
			return audioSource;
		}

		private void SetEQParams(AudioSource source, ParamsEQ eqParams)
		{
			SetMixerFloat(source, "centerFreq", eqParams.centerFreq);
			SetMixerFloat(source, "octaveRange", eqParams.octaveRange);
			SetMixerFloat(source, "frequencyGain", eqParams.frequencyGain);
		}

		private void SetMixerFloat(AudioSource source, string name, float value)
		{
			if ((bool)source && (bool)source.outputAudioMixerGroup)
			{
				source.outputAudioMixerGroup.audioMixer.SetFloat(name, value);
			}
		}

		public void ActivateShadowFormEQSetting(bool activate)
		{
			if (activate && !_shadowSource.isPlaying && NeedSpecialShadowFormSound)
			{
				_shadowSource.Play();
			}
			else if (!activate && _shadowSource.isPlaying)
			{
				_shadowSource.Stop();
			}
			_activateShadow = true;
			_isActivateShadow = activate;
		}

		private AudioClip GetAudioClip()
		{
			return GlobalLoad.GetLoadAudioClipInternal("music", _musicName);
		}
	}
}
