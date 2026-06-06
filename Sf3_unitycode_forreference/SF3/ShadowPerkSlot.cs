using System;
using SF3.Items;
using SF3.Utils;
using UnityEngine;

namespace SF3
{
	public class ShadowPerkSlot : MonoBehaviour
	{
		[SerializeField]
		private Color _emptyColor;

		[SerializeField]
		private Color _fullColor;

		[SerializeField]
		private Color _arrowColor;

		[SerializeField]
		private ImageWrapper _ability;

		[SerializeField]
		private ImageWrapper _foreground;

		[SerializeField]
		private ImageWrapper _background;

		[SerializeField]
		private ImageWrapper _arrow;

		private BehaviourTimer.SingleTimer _cooldownTimer;

		private RectTransform _rectTransform;

		private Transform _backgroundTransform;

		private Quaternion _normalBackgroundRotation;

		private Quaternion _flippedBackgroundRotation;

		public RectTransform RectTransform
		{
			get
			{
				return _rectTransform;
			}
		}

		public void Init()
		{
			_rectTransform = GetComponent<RectTransform>();
			_backgroundTransform = _background.transform;
			_normalBackgroundRotation = _backgroundTransform.localRotation;
			_flippedBackgroundRotation = Quaternion.Euler(_normalBackgroundRotation.eulerAngles + new Vector3(0f, 0f, 180f));
		}

		public void SetPerk(string perkName)
		{
			bool flag = perkName != string.Empty;
			_ability.gameObject.SetActive(flag);
			_foreground.gameObject.SetActive(flag);
			_background.color = ((!flag) ? _emptyColor : _fullColor);
			_arrow.color = _arrowColor;
			if (flag)
			{
				_ability.sprite = GlobalLoad.GetLoadSpriteInternal("shadowPerksIcons", perkName);
				_foreground.fillAmount = 0f;
				_arrow.gameObject.SetActive(false);
			}
			else
			{
				_arrow.gameObject.SetActive(true);
			}
		}

		public void Cooldown(int framesDuration, EquipmentType type, Action onFinish = null)
		{
			if (_cooldownTimer != null)
			{
				_cooldownTimer.ForceStop();
			}
			_cooldownTimer = _foreground.CreateFillTimerDescending(framesDuration, onFinish);
		}

		public void FlipArrowTexture(bool isFlipped)
		{
			if (isFlipped)
			{
				_backgroundTransform.localRotation = _flippedBackgroundRotation;
			}
			else
			{
				_backgroundTransform.localRotation = _normalBackgroundRotation;
			}
		}

		public void ClearPerk()
		{
			SetPerk(string.Empty);
		}
	}
}
