using System;
using System.Collections.Generic;
using DG.Tweening;
using SF3.GameModels;
using SF3.Items;
using UnityEngine;

namespace SF3
{
	public class ShadowPerksSlotsHolder : MonoBehaviour
	{
		[SerializeField]
		private ShadowPerkSlot _topSlot;

		[SerializeField]
		private ShadowPerkSlot _bottomSlot;

		[SerializeField]
		private ShadowPerkSlot _forwardSlot;

		[SerializeField]
		private ShadowPerkSlot _backSlot;

		private bool _inverted;

		private CanvasGroup _slotsGroup;

		private Dictionary<EquipmentType, ShadowPerkSlot> _shadowPerkSlots;

		private bool _isPlayer;

		private float _forwardSlotPosX;

		private float _backSlotPosX;

		public bool Inverted
		{
			set
			{
				if (_inverted != value)
				{
					_inverted = value;
					FlipHorizontaly();
				}
			}
		}

		public void Init()
		{
			_slotsGroup = _forwardSlot.GetComponentInParent<CanvasGroup>();
			_topSlot.Init();
			_bottomSlot.Init();
			_forwardSlot.Init();
			_backSlot.Init();
			_forwardSlotPosX = _forwardSlot.RectTransform.localPosition.x;
			_backSlotPosX = _backSlot.RectTransform.localPosition.x;
			_inverted = false;
		}

		public void SetModel(Model model)
		{
			_isPlayer = model.isPlayer;
			_shadowPerkSlots = new Dictionary<EquipmentType, ShadowPerkSlot>
			{
				{
					EquipmentType.Helmet,
					_topSlot
				},
				{
					EquipmentType.Armor,
					_bottomSlot
				},
				{
					EquipmentType.Weapon,
					_forwardSlot
				},
				{
					EquipmentType.Ranged,
					_backSlot
				}
			};
			foreach (EquipmentType key in _shadowPerkSlots.Keys)
			{
				_shadowPerkSlots[key].SetPerk(GetShadowMark(model, key));
			}
		}

		public void ClearShadowPerkSlot(EquipmentType equipmentType)
		{
			_shadowPerkSlots[equipmentType].ClearPerk();
		}

		public void SetShadowPerkState(ShadowPerksState state)
		{
			SetTransparency(ShadowPerksController.GetAlpha(state), ShadowPerksController.FadeDuration);
			if (state == ShadowPerksState.NotActive)
			{
				ClearSlots();
			}
		}

		private void SetTransparency(float alpha, float duration = 0f)
		{
			_slotsGroup.DOFade(alpha, duration);
		}

		public void StartCooldown(EquipmentType type, int framesDuraion)
		{
			Action onFinish = null;
			if (_isPlayer)
			{
				StickHelper.Instance.SetShadowHintByType(type, false);
				onFinish = delegate
				{
					StickHelper.Instance.SetShadowHintByType(type, true);
				};
			}
			_shadowPerkSlots[type].Cooldown(framesDuraion, type, onFinish);
		}

		public void FlipHorizontaly()
		{
			float x;
			float x2;
			if (_inverted)
			{
				x = _backSlotPosX;
				x2 = _forwardSlotPosX;
			}
			else
			{
				x = _forwardSlotPosX;
				x2 = _backSlotPosX;
			}
			_forwardSlot.RectTransform.localPosition = new Vector3(x, _forwardSlot.RectTransform.localPosition.y, _forwardSlot.RectTransform.localPosition.z);
			_backSlot.RectTransform.localPosition = new Vector3(x2, _backSlot.RectTransform.localPosition.y, _backSlot.RectTransform.localPosition.z);
			_forwardSlot.FlipArrowTexture(_inverted);
			_backSlot.FlipArrowTexture(_inverted);
		}

		private string GetShadowMark(Model model, EquipmentType type)
		{
			Equipment equippedForType = model.GetEquippedForType(type);
			if (model.id == 2)
			{
				return (equippedForType != null) ? equippedForType.EnemyShadowMark : string.Empty;
			}
			return (equippedForType != null) ? equippedForType.ShadowMark : string.Empty;
		}

		public void ClearSlots()
		{
			_topSlot.ClearPerk();
			_bottomSlot.ClearPerk();
			_forwardSlot.ClearPerk();
			_backSlot.ClearPerk();
		}
	}
}
