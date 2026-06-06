using System;
using System.Collections.Generic;
using SF3.Audio;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SF3
{
	public class ButtonAudio : MonoBehaviour
	{
		public enum EButtonState
		{
			PRESS = 0,
			CLICK = 1
		}

		[Serializable]
		private class ButtonStateAudio
		{
			public string soundName = string.Empty;

			public EButtonState buttonState = EButtonState.CLICK;
		}

		[SerializeField]
		private ButtonStateAudio[] _buttonStateAudios;

		private Dictionary<EButtonState, string> _sounds;

		private bool _buttonFound;

		private void Start()
		{
			InitSounds();
			InitButton();
			LoadSounds();
		}

		private void InitSounds()
		{
			_sounds = new Dictionary<EButtonState, string>();
			ButtonStateAudio[] buttonStateAudios = _buttonStateAudios;
			foreach (ButtonStateAudio buttonStateAudio in buttonStateAudios)
			{
				if (!string.IsNullOrEmpty(buttonStateAudio.soundName))
				{
					_sounds.Add(buttonStateAudio.buttonState, buttonStateAudio.soundName);
				}
			}
		}

		private void InitButton()
		{
			TryInitAsUIButton();
			TryInitAsEventTrigger();
		}

		private void LoadSounds()
		{
			if (!_buttonFound || !(AudioManager.Instance != null))
			{
				return;
			}
			foreach (KeyValuePair<EButtonState, string> sound in _sounds)
			{
				AudioManager.Instance.LoadSound(sound.Value);
			}
		}

		private void TryInitAsUIButton()
		{
			UIButton component = base.gameObject.GetComponent<UIButton>();
			if (!(component == null))
			{
				EventDelegate delValue = new EventDelegate(OnClickAction);
				component.onClick.RemoveAll((EventDelegate e) => e.Equals(delValue));
				component.OnPressEvent -= OnPressAction;
				if (_sounds.ContainsKey(EButtonState.CLICK))
				{
					component.onClick.Add(delValue);
				}
				if (_sounds.ContainsKey(EButtonState.PRESS))
				{
					component.OnPressEvent += OnPressAction;
				}
				_buttonFound = true;
			}
		}

		private void TryInitAsEventTrigger()
		{
			if (GetComponent<EventTrigger>() == null)
			{
				return;
			}
			if (_sounds.ContainsKey(EButtonState.CLICK))
			{
				GetEntry(EventTriggerType.PointerClick).callback.AddListener(delegate(BaseEventData data)
				{
					OnPointerClickAction((PointerEventData)data);
				});
				GetEntry(EventTriggerType.Drop).callback.AddListener(delegate(BaseEventData data)
				{
					OnPointerClickAction((PointerEventData)data);
				});
			}
			if (_sounds.ContainsKey(EButtonState.PRESS))
			{
				GetEntry(EventTriggerType.PointerDown).callback.AddListener(delegate(BaseEventData data)
				{
					OnPointerPressAction((PointerEventData)data);
				});
			}
			_buttonFound = true;
		}

		private EventTrigger.Entry GetEntry(EventTriggerType type)
		{
			EventTrigger component = base.gameObject.GetComponent<EventTrigger>();
			EventTrigger.Entry entry = component.triggers.Find((EventTrigger.Entry x) => x.eventID == type);
			if (entry == null)
			{
				entry = new EventTrigger.Entry();
				entry.eventID = type;
				component.triggers.Add(entry);
			}
			return entry;
		}

		private void OnPointerClickAction(PointerEventData eventData)
		{
			OnClickAction();
		}

		private void OnClickAction()
		{
			if (_sounds.ContainsKey(EButtonState.CLICK))
			{
				AudioManager.Instance.PlaySound(_sounds[EButtonState.CLICK]);
			}
		}

		private void OnPointerPressAction(PointerEventData eventData)
		{
			OnPressAction();
		}

		private void OnPressAction()
		{
			if (_sounds.ContainsKey(EButtonState.PRESS))
			{
				AudioManager.Instance.PlaySound(_sounds[EButtonState.PRESS]);
			}
		}
	}
}
