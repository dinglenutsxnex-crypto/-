using System.Collections.Generic;
using DG.Tweening;
using SF3.GameModels;
using SF3.Items;
using UnityEngine;

namespace SF3
{
	public class ShadowPerksController : UIModuleHolder
	{
		private enum RootObjectsStates
		{
			Visible = 0,
			Hidden = 1,
			Tween = 2
		}

		[SerializeField]
		private ShadowPerksSlotsHolder _playerSlots;

		[SerializeField]
		private ShadowPerksSlotsHolder _foeSlots;

		[SerializeField]
		[Range(0f, 1f)]
		private float _readyAlpha = 0.4f;

		[SerializeField]
		[Range(0f, 1f)]
		private float _notActiveAlpha = 0.2f;

		[SerializeField]
		private float _fadeDuration;

		[SerializeField]
		private float _offset3x4;

		[SerializeField]
		private float _offetNormal;

		[SerializeField]
		private RectTransform _playerRoot;

		[SerializeField]
		private RectTransform _foeRoot;

		private Dictionary<int, ShadowPerksSlotsHolder> _slotsHolder;

		private static readonly Vector2 _nguiOffset = new Vector2(75f, 780f);

		private RootObjectsStates _rootObjectsState;

		private static ShadowPerksController _instance;

		public static float FadeDuration
		{
			get
			{
				return _instance._fadeDuration;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			_instance = this;
			_playerSlots.Init();
			_foeSlots.Init();
		}

		public void Init(Model player, Model foe)
		{
			_slotsHolder = new Dictionary<int, ShadowPerksSlotsHolder>
			{
				{ player.id, _playerSlots },
				{ foe.id, _foeSlots }
			};
			_slotsHolder[player.id].SetModel(player);
			_slotsHolder[foe.id].SetModel(foe);
			SetShadowPerksState(ShadowPerksState.Ready);
			AnchorTo(_playerRoot, NekkiUIRootModules.Instance.GetModule("PlayerHUD").GetComponent<HUD>(), false);
			AnchorTo(_foeRoot, NekkiUIRootModules.Instance.GetModule("FoeHUD").GetComponent<HUD>(), true);
		}

		private void AnchorTo(RectTransform root, HUD hud, bool isFoe)
		{
			Vector3 position = hud.currentNameLabel.transform.position;
			RectTransform rectTransform = NekkiCanvasRoot.instance.RectTransform;
			Vector2 vector = NekkiUIRoot.Camera.WorldToViewportPoint(position);
			float num = vector.x * rectTransform.sizeDelta.x - rectTransform.sizeDelta.x * 0.5f;
			Vector2 nguiOffset = _nguiOffset;
			float x = num - nguiOffset.x * (float)((!isFoe) ? 1 : (-1));
			float num2 = vector.y * rectTransform.sizeDelta.y - rectTransform.sizeDelta.y * 0.5f;
			Vector2 nguiOffset2 = _nguiOffset;
			Vector2 anchoredPosition = new Vector2(x, num2 - nguiOffset2.y);
			root.anchoredPosition = anchoredPosition;
		}

		public void Invert(bool invert)
		{
			_playerSlots.Inverted = invert;
			_foeSlots.Inverted = !invert;
		}

		protected void Update()
		{
			if (_rootObjectsState != RootObjectsStates.Tween)
			{
				if ((ScreenTexture.Instance.Active || FoggingController.Instance.Active) && _rootObjectsState == RootObjectsStates.Visible)
				{
					TeweenRoots(Vector3.zero, 0f, RootObjectsStates.Hidden);
				}
				else if (!ScreenTexture.Instance.Active && !FoggingController.Instance.Active && _rootObjectsState == RootObjectsStates.Hidden)
				{
					TeweenRoots(Vector3.one, 0.3f, RootObjectsStates.Visible);
				}
			}
		}

		private void TeweenRoots(Vector3 target, float time, RootObjectsStates stateAfterTween)
		{
			if (time == 0f)
			{
				_foeRoot.transform.localScale = target;
				_playerRoot.transform.localScale = target;
				_rootObjectsState = stateAfterTween;
				return;
			}
			_rootObjectsState = RootObjectsStates.Tween;
			_foeRoot.DOScale(target, time);
			_playerRoot.DOScale(target, time).OnComplete(delegate
			{
				_rootObjectsState = stateAfterTween;
			});
		}

		public void SetShadowPerksState(ShadowPerksState state)
		{
			SetShadowPerkState(1, state);
			SetShadowPerkState(2, state);
		}

		public void ClearShadowPerkSlot(int modelId, EquipmentType equipmentType)
		{
			_slotsHolder[modelId].ClearShadowPerkSlot(equipmentType);
		}

		public void SetShadowPerkState(int modelId, ShadowPerksState state)
		{
			_slotsHolder[modelId].SetShadowPerkState(state);
		}

		public void StartCooldown(int modelId, EquipmentType equipmentType, int framesDuration)
		{
			_slotsHolder[modelId].StartCooldown(equipmentType, framesDuration);
		}

		public static float GetAlpha(ShadowPerksState state)
		{
			switch (state)
			{
			case ShadowPerksState.Active:
				return 1f;
			case ShadowPerksState.Ready:
				return _instance._readyAlpha;
			case ShadowPerksState.NotActive:
				return _instance._notActiveAlpha;
			default:
				return 0f;
			}
		}
	}
}
