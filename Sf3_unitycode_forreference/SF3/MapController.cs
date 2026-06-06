using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Nekki.UI;
using SF3.Audio;
using SF3.Moves;
using SF3.UserData;
using UnityEngine;
using sf3DTO;

namespace SF3
{
	public class MapController : UIModuleHolder
	{
		public delegate void OnMapUpdateEventHandler();

		[SerializeField]
		private string _onOpenSoundName = string.Empty;

		[SerializeField]
		private string _onCloseSoundName = string.Empty;

		[SerializeField]
		private Transform _mapTransform;

		[SerializeField]
		private MapBattleUI _mapBattleUI;

		[SerializeField]
		private InfoBattleUI _infoBattleUI;

		[SerializeField]
		private float _centeringVerticalOffset = 20f;

		[SerializeField]
		private UIButton _mapBtn;

		[SerializeField]
		private readonly Vector3 _mapButtonStackOffsetCollapsed = new Vector2(5f, 0f);

		[SerializeField]
		private readonly Vector3 _mapButtonStackOffsetExpanded = new Vector2(80f, 0f);

		public AnimationCurve MapButtonStackCurve;

		public float MapButtonStackTransitionTime = 0.5f;

		[SerializeField]
		private UIPanel _mapPanel;

		private Vector2 _mapSize;

		[SerializeField]
		private NekkiUIDragObject _drag;

		[SerializeField]
		private NekkiUIDragScaleWidget _dragScaleWidget;

		public AnimationCurve CenteringCurve;

		public float CenteringSpeed;

		private float _centeringTimer;

		private float _centeringTime;

		private Vector3 _startCenteringPoint;

		private Vector3 _centeringPoint;

		private Action _onFinishCentering;

		private IBattleInfo _selectedBattle;

		public static MapController Instance { get; private set; }

		public static IBattleInfo SelectedBattle
		{
			get
			{
				return Instance._selectedBattle;
			}
		}

		public static IBattleInfo SelectedBattleUI
		{
			get
			{
				return Instance._mapBattleUI.selectedBattle;
			}
		}

		public Vector3 MapButtonStackOffsetCollapsed
		{
			get
			{
				return _mapButtonStackOffsetCollapsed;
			}
		}

		public Vector3 MapButtonStackOffsetExpanded
		{
			get
			{
				return _mapButtonStackOffsetExpanded;
			}
		}

		public bool CenteringActive { get; private set; }

		public bool IsStackAnimatingProcess { get; set; }

		public bool ScaleActive
		{
			get
			{
				return _dragScaleWidget.enabled;
			}
			set
			{
				_dragScaleWidget.enabled = value;
			}
		}

		public static event OnMapUpdateEventHandler OnMapUpdate;

		protected override void Awake()
		{
			base.Awake();
			Instance = this;
		}

		private void Update()
		{
			if (CenteringActive)
			{
				_centeringTimer += GameTimeController.unscaledDeltaTime;
				if (_centeringTime > 0f)
				{
					SetMapPosition(Vector3.Lerp(_startCenteringPoint, _centeringPoint, CenteringCurve.Evaluate(_centeringTimer / _centeringTime)));
				}
				if (_centeringTimer >= _centeringTime)
				{
					OnEndMove();
				}
			}
			UpdateDragBounds();
		}

		public void UpdateDragBounds()
		{
			_drag.ConstrainToBounds();
		}

		protected override void OnDestroy()
		{
			BattlesManager.OnBattlesUpdate -= InitMap;
			base.OnDestroy();
		}

		public void SelectBattleByClick(int battleId)
		{
			if (!_mapBattleUI.IsSelectBattle(battleId))
			{
				StartMapp(battleId);
			}
		}

		public void MissClick()
		{
			if (_selectedBattle != null)
			{
				_mapBattleUI.UnSelectBattle();
				_infoBattleUI.ShowBattleInfo(false);
				QuestController.Instance.ThrowEvent(ETriggerEvents.QEVENT_MAP_MISS_CLICK, _selectedBattle);
			}
		}

		public override void Initialize()
		{
			_mapPanel.alpha = 0f;
			float num = (float)Screen.width / (float)Screen.height / 1.33f;
			_mapPanel.SetRect(0f, 0f, 962f * num, 760f);
			_mapSize = _mapTransform.GetComponent<UIWidget>().localSize / 2f;
			_drag.target = _mapTransform.GetComponent<UIWidget>().transform;
			_drag.dragEffect = NekkiUIDragObject.DragEffect.Momentum;
			_drag.updateTouch = true;
			_drag.restrictWithinPanel = true;
			_dragScaleWidget.WidgetScale = UserManager.GetMapScale();
			_infoBattleUI.Initialize();
			_mapBattleUI.Initialize();
			MapBattleStack.Initialize(_mapBattleUI);
			AudioManager.Instance.LoadSound(_onOpenSoundName, _onCloseSoundName);
			BattlesManager.OnBattlesUpdate += InitMap;
			NekkiUIDragScaleWidget dragScaleWidget = _dragScaleWidget;
			dragScaleWidget.OnScaleChange = (Action)Delegate.Combine(dragScaleWidget.OnScaleChange, new Action(UpdateScale));
			_mapBtn.OnPressEvent += MissClick;
		}

		private void UpdateScale()
		{
			_mapBattleUI.UpdateBattleIconScale(_dragScaleWidget.WidgetScale);
		}

		private IBattleInfo GetSelectedBattle()
		{
			int selectedBattleID = UserManager.GetSelectedBattleID();
			return _mapBattleUI.GetBattleMapp(selectedBattleID);
		}

		private Vector3 CalculateCameraPosition(Vector3 battleMapPosition)
		{
			float widgetScale = _dragScaleWidget.WidgetScale;
			Vector3 result = default(Vector3);
			result.x = (0f - _mapSize.x + battleMapPosition.x) * (0f - widgetScale);
			result.y = (_mapSize.y - battleMapPosition.y) * (0f - widgetScale);
			result.z = 0f;
			result.x -= _infoBattleUI.backgroundWidth / 2;
			result.y -= CurrencyUI.instance.height / 2f - _centeringVerticalOffset;
			return result;
		}

		public void GoToCamera(IBattleInfo battle, bool instantFocus = false, Action onFinishCamera = null)
		{
			if (battle != null)
			{
				GoToCamera(battle.GetLocation().position, instantFocus, onFinishCamera);
			}
		}

		public void GoToCamera(Vector3 battleMapPosition, bool instantFocus = false, Action onFinishCamera = null)
		{
			_onFinishCentering = (Action)Delegate.Combine(_onFinishCentering, onFinishCamera);
			Vector3 vector = CalculateCameraPosition(battleMapPosition);
			if (instantFocus || _mapTransform.localPosition == vector)
			{
				SetMapPosition(vector);
				OnEndMove();
				return;
			}
			_startCenteringPoint = _mapTransform.localPosition;
			_centeringPoint = vector;
			_centeringTime = Vector3.Distance(_startCenteringPoint, _centeringPoint) / CenteringSpeed;
			CenteringActive = true;
		}

		private void SetMapPosition(Vector3 position)
		{
			_mapTransform.localPosition = position;
		}

		private void ResetMove()
		{
			CenteringActive = false;
			_centeringTimer = 0f;
			_centeringTime = 0f;
			_onFinishCentering = delegate
			{
			};
		}

		private void OnEndMove()
		{
			_onFinishCentering.InvokeSafe();
			ResetMove();
			QuestController.Instance.ThrowEvent(ETriggerEvents.QEVENT_BATTLE_SELECTION_END, _selectedBattle);
		}

		public override void HideModule(Action callbackOnClosed)
		{
			ModelsManager.Instance.ShowModels();
			DOTween.To(() => _mapPanel.alpha, delegate(float x)
			{
				_mapPanel.alpha = x;
			}, 0f, 1f).OnComplete(delegate
			{
				base.gameObject.SetActive(false);
				AudioManager.Instance.PlaySound(_onCloseSoundName);
				callbackOnClosed.InvokeSafe();
			});
		}

		public override void ShowModule(IntentModule intent, Action callbackOnOpen)
		{
			callbackOnOpen.InvokeSafe();
			InitMap();
			StartCoroutine(InitMapCorutine(_selectedBattle));
		}

		private void InitMap()
		{
			UpdateMapBattles();
			IBattleInfo selectedBattle = GetSelectedBattle();
			IBattleInfo battleInfo = selectedBattle;
			if (BattlesManager.LastCompleteBattle != null)
			{
				battleInfo = BattlesManager.LastCompleteBattle;
				BattlesManager.LastCompleteBattle = null;
			}
			Vector3 mapPosition = CalculateCameraPosition(battleInfo.GetLocation().position);
			SetMapPosition(mapPosition);
			SelectBattle(selectedBattle, false);
			if (MapController.OnMapUpdate != null)
			{
				MapController.OnMapUpdate();
			}
		}

		private IEnumerator InitMapCorutine(IBattleInfo battle)
		{
			int waitFrames = 11;
			for (int i = 0; i < waitFrames; i++)
			{
				yield return new WaitForEndOfFrame();
			}
			while (DialogsManager.IsDialog)
			{
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForEndOfFrame();
			QuestController.Instance.ThrowEvent(ETriggerEvents.QEVENT_MAP_OPENED, battle);
		}

		public void StartMapp(int battleID, bool instantFocus = false)
		{
			StartMapp(BattlesManager.instance.GetBattle(battleID), instantFocus);
		}

		public void StartMapp(IBattleInfo battle, bool instantFocus = false)
		{
			SelectBattle(battle);
			GoToCamera(battle, instantFocus);
		}

		public void SelectBattle(IBattleInfo battle, bool isThrowEvent = true)
		{
			_selectedBattle = battle;
			_infoBattleUI.LoadBattleInfo(battle);
			_mapBattleUI.SelectBattle(battle);
			UserManager.SetSelectedBattle(battle.GetID());
			if (isThrowEvent)
			{
				QuestController.Instance.ThrowEvent(ETriggerEvents.QEVENT_BATTLE_SELECTION_START, battle);
			}
		}

		public void ShowBattleInfo(bool show)
		{
			_infoBattleUI.ShowBattleInfo(show);
		}

		private void ClearBattles()
		{
			ShowBattleInfo(false);
			foreach (MapBattleButton mapBattlesIcon in _mapBattleUI.mapBattlesIcons)
			{
				_dragScaleWidget.RemoveUnscalableWidget(mapBattlesIcon.transform);
			}
			_mapBattleUI.ClearBattles();
			MapBattleStack.Instance.Clear();
		}

		public void UpdateMapBattles()
		{
			ClearBattles();
			List<IBattleInfo> list = new List<IBattleInfo>();
			List<IBattleInfo> battlesVisible = BattlesManager.instance.GetBattlesVisible();
			List<IBattleInfo> battlesFutureAvailable = BattlesManager.instance.GetBattlesFutureAvailable();
			List<IBattleInfo> list2 = BattlesManager.instance.GetBattles().FindAll((IBattleInfo info) => info.GetBattleType() == sf3DTO.BattleType.Daily);
			foreach (IBattleInfo item in list2)
			{
				if (((DailyBattleInfo)item).DailyUpdateTimePassed)
				{
					RefreshBattlesProcessing.RefreshBattles();
					break;
				}
			}
			list.AddRange(battlesVisible);
			list.AddRange(battlesFutureAvailable);
			foreach (IBattleInfo item2 in battlesVisible)
			{
				AddBattle(item2, (!item2.GetIsAvailable()) ? MapBattleButton.DecorationType.Unavailable : MapBattleButton.DecorationType.Available);
			}
			foreach (IBattleInfo item3 in battlesFutureAvailable)
			{
				AddBattle(item3, MapBattleButton.DecorationType.Available);
			}
			foreach (IBattleInfo item4 in list2)
			{
				if (!list.Contains(item4))
				{
					list.Add(item4);
					AddBattle(item4, MapBattleButton.DecorationType.Unavailable);
				}
			}
		}

		public void AddBattle(IBattleInfo newBattle, MapBattleButton.DecorationType decorationType)
		{
			MapBattleButton mapBattleButton = _mapBattleUI.CreateMapBattle(newBattle, decorationType);
			_dragScaleWidget.AddUnscalableWidget(mapBattleButton.transform);
			MapBattleStack.Instance.Add(mapBattleButton, newBattle);
		}
	}
}
