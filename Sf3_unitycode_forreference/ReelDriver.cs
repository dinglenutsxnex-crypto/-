using System;
using System.Collections;
using System.Collections.Generic;
using SF3;
using SF3.Audio;
using SF3.Items;
using SF3.UserData;
using UnityEngine;

public class ReelDriver : MonoBehaviour
{
	public delegate void ItemSelectedEventHandler(BaseItem item);

	public delegate void ItemInsertedEventHadler(BaseItem item, PerkSlot slot);

	public class SplineObject
	{
		public GameObject Obj;

		public BaseItem Item;

		private float _lastPosition;

		private ReelDriver _reel;

		public int ID
		{
			get
			{
				return Item.id;
			}
		}

		public float PositionOnCurve
		{
			get
			{
				if ((_reel._inDrag || _reel._dragBack) && !Dragged)
				{
					float num = Vector2.Distance(_reel._dragedItem.Obj.transform.localPosition, Vector3.zero);
					float num2 = Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height);
					float num3 = ((!(num > num2 / 10f)) ? (num / (num2 / 10f)) : 1f);
					if (_reel._dragedItem.Index == 0)
					{
						return (float)Index * _reel._cellSize + _reel._ajust - num3 * _reel._cellSize;
					}
					if (_reel._dragedItem.Index == _reel.Count - 1)
					{
						return (float)Index * _reel._cellSize + _reel._ajust + num3 * _reel._cellSize;
					}
					if (Index < _reel._dragedItem.Index)
					{
						return (float)Index * _reel._cellSize + _reel._ajust + num3 * _reel._cellSize;
					}
				}
				return (float)Index * _reel._cellSize + _reel._ajust;
			}
		}

		public CardItem ReelItem { get; private set; }

		public int Index { get; private set; }

		public bool Dragged { get; private set; }

		public SplineObject(ReelDriver reel, GameObject go, int index, BaseItem item)
		{
			Obj = go;
			Item = item;
			_reel = reel;
			go.name = ID + "_" + index;
			Index = index;
			ReelItem = Obj.GetComponent<CardItem>();
			ReelItem.Init(item);
			UpdatePosition();
		}

		public bool UpdatePosition()
		{
			bool result = false;
			if (Dragged)
			{
				return result;
			}
			float num = Mathf.Abs(_lastPosition - PositionOnCurve);
			if (((_lastPosition > 0.5f && PositionOnCurve <= 0.5f) || (_lastPosition < 0.5f && PositionOnCurve >= 0.5f)) && num < _reel._cellSize)
			{
				result = true;
			}
			_lastPosition = PositionOnCurve;
			Obj.transform.position = _reel.PositionAt(PositionOnCurve) * _reel._positionScale;
			Obj.transform.eulerAngles = _reel.RotationAt(PositionOnCurve) * _reel._rotationScale;
			Obj.transform.localScale = Vector3.one * _reel._scale.Evaluate(PositionOnCurve);
			ReelItem.UpdateDepth((int)(_reel._posZ.Evaluate(PositionOnCurve) * -100f));
			ReelItem.UpdateShade(_reel._shade.Evaluate(PositionOnCurve));
			ReelItem.UpdateItem();
			return result;
		}

		public void StartDrag()
		{
			Dragged = true;
			ReelItem.Drag(true);
			Obj.transform.parent = _reel._dragPivot;
			Obj.transform.localPosition = Vector3.zero;
			_reel._startDragScale = Obj.transform.localScale;
			Obj.transform.localScale = Vector3.one * _reel._dragScale;
			Obj.SetActive(false);
			Obj.SetActive(true);
		}

		public void EndDrag()
		{
			Dragged = false;
			ReelItem.Drag(false);
			UpdatePosition();
		}

		public void SetIndex(int index)
		{
			Index = index;
		}
	}

	[Header("position splines:")]
	[SerializeField]
	private AnimationCurve _posX;

	[SerializeField]
	private AnimationCurve _posY;

	[SerializeField]
	private AnimationCurve _posZ;

	[Header("rotation splines:")]
	[SerializeField]
	private AnimationCurve _rotationX;

	[SerializeField]
	private AnimationCurve _rotationY;

	[SerializeField]
	private AnimationCurve _rotationZ;

	[Header("shade spline:")]
	[SerializeField]
	private AnimationCurve _shade;

	[Header("size scale spline:")]
	[SerializeField]
	private AnimationCurve _scale;

	[Header("camera position offset")]
	[SerializeField]
	private Vector2 _cameraPositionOffset;

	[SerializeField]
	private Vector2 _camera4x3PositionOffset;

	[Header("the value which is formed by position spline:")]
	[SerializeField]
	private float _positionScale = 10f;

	[Header("the value which is formed by rotation spline:")]
	[SerializeField]
	private float _rotationScale = 360f;

	[Header("swipe power multiplier:")]
	[SerializeField]
	private float _swipeScale = 5f;

	[Header("reel centering speed:")]
	[SerializeField]
	private float _centerOnSpeed = 1f;

	[Header("distance between cards on reel:")]
	[Header("should be in range (0-1)")]
	[SerializeField]
	private float _cellSize = 0.2f;

	[Header("base spline object:")]
	[SerializeField]
	private GameObject _prototype;

	[Header("Cameras:")]
	[SerializeField]
	private Camera _reelCamera;

	[SerializeField]
	private Camera _uiCamera;

	[Header("Layers:")]
	[SerializeField]
	private string _reelLayer;

	[SerializeField]
	private string _uiLayer;

	[Header("Delay before drag stats:")]
	[SerializeField]
	private float _dragDellay;

	private float _velocity;

	private bool _isAnim;

	[Header("Drag:")]
	[SerializeField]
	private Transform _dragPivot;

	[SerializeField]
	private float _dragScale;

	[Header("Velocity tolerance:")]
	[SerializeField]
	private float _velocityTolerance;

	[Header("Sounds:")]
	[SerializeField]
	private string _cardSwipeSound;

	[SerializeField]
	private string _cardClickSound;

	[Header("Sell:")]
	[SerializeField]
	private GameObject _sellPivot;

	[Header("Control container(optional):")]
	[SerializeField]
	private Transform _controlContainer;

	[Header("Progress bar(optional):")]
	[SerializeField]
	private GameObject _progressbarPrf;

	[SerializeField]
	private float progressYOffset = -200f;

	[Header("Buttons (optional):")]
	[SerializeField]
	private GameObject _leftBtnPrf;

	[SerializeField]
	private GameObject _rightBtnPrf;

	[SerializeField]
	private float _xOffset = -200f;

	[SerializeField]
	private float _yOffset = -200f;

	private UIButton _leftBtn;

	private UIButton _rightBtn;

	private UIProgressBar _progressbar;

	private List<SplineObject> _pool = new List<SplineObject>();

	private int _centerOnIndex = -1;

	private float _ajust;

	private bool _inSwipe;

	private bool _inDrag;

	private bool _dragBack;

	private bool _canStartDrag;

	private bool _allowDrag;

	private Vector3 _startDragScale;

	private Vector3 _lastSwipePosition;

	private Vector3 _firstSwipePosition;

	private SplineObject _dragedItem;

	public BaseItem Selected { get; private set; }

	public bool Moving
	{
		get
		{
			return _centerOnIndex >= 0;
		}
	}

	public int Count
	{
		get
		{
			return _pool.Count;
		}
	}

	public event ItemSelectedEventHandler ItemSelected;

	public event ItemSelectedEventHandler ItemStartChanging;

	public event ItemSelectedEventHandler ItemSwiped;

	public event ItemInsertedEventHadler ItemInserted;

	public event Action Cleared;

	private void Start()
	{
		_prototype.gameObject.SetActive(false);
		_sellPivot.SetActive(false);
		_reelCamera.transform.position = PositionAt(0.5f) * _positionScale + new Vector3(0f, 0f, -1.5f);
		if (!_uiCamera)
		{
			_uiCamera = GetComponentInParent<UICamera>().cachedCamera;
		}
		float aspect = _uiCamera.aspect;
		_reelCamera.rect = new Rect(_cameraPositionOffset, Vector2.one);
		if (aspect >= 1.3f && aspect < 1.5f)
		{
			_reelCamera.rect = new Rect(_camera4x3PositionOffset, Vector2.one);
		}
		else if (aspect >= 1.5f)
		{
			_reelCamera.rect = new Rect(_cameraPositionOffset, Vector2.one);
		}
		Vector3 position = _uiCamera.ScreenToWorldPoint(_reelCamera.WorldToScreenPoint(PositionAt(0.5f)));
		_dragPivot.position = position;
		if (_progressbarPrf != null && _progressbar == null)
		{
			CreateProgress();
			UpdateProgress();
		}
		if (_leftBtnPrf != null && _rightBtnPrf != null && _leftBtn == null && _rightBtn == null)
		{
			CreateButtons();
		}
	}

	private void Update()
	{
		SwipeUpdate();
		if (!VelocityMove())
		{
			FocusOnClose();
		}
	}

	public void FocusOnItem(int itemId, bool instant)
	{
		FocusOn(GetIndexOf(itemId), instant);
	}

	public void FocusOn(int id, bool instant)
	{
		_centerOnIndex = id;
		if (instant)
		{
			float amount = 0.5f - _pool[id].PositionOnCurve;
			OnItemSelected(_pool[id].Item);
			Move(amount);
			_centerOnIndex = -1;
		}
		base.gameObject.SetActive(true);
	}

	private int GetIndexOf(int itemId)
	{
		for (int i = 0; i < _pool.Count; i++)
		{
			if (_pool[i].ID == itemId)
			{
				return i;
			}
		}
		return -1;
	}

	public bool CheckTutorialDragable(ReelItem item)
	{
		if (UIBlocker.Instance.IsLocked)
		{
			TutorialComponent component = item.GetComponent<TutorialComponent>();
			if (!component.isDragable)
			{
				return false;
			}
		}
		return true;
	}

	private void SwipeUpdate()
	{
		if ((GameTimeController.gamePaused && !UIBlocker.Instance.IsLocked) || _dragBack)
		{
			return;
		}
		float num = 0f;
		if (Input.GetMouseButtonDown(0) && !_isAnim)
		{
			UICamera.Raycast(Input.mousePosition);
			if (UICamera.lastRaycastedCollider != null)
			{
				ReelItem component = UICamera.lastRaycastedCollider.gameObject.GetComponent<ReelItem>();
				if ((bool)component)
				{
					if (!CheckTutorialDragable(component))
					{
						return;
					}
					_centerOnIndex = -1;
					_inSwipe = true;
					_lastSwipePosition = Input.mousePosition;
					_firstSwipePosition = _lastSwipePosition;
					_canStartDrag = true;
				}
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			_inSwipe = false;
			if (_inDrag)
			{
				UICamera.Raycast(Input.mousePosition);
				if (UICamera.lastRaycastedCollider != null)
				{
					ProcessSlotOrDistruction(UICamera.lastRaycastedCollider);
				}
			}
			else if (_canStartDrag)
			{
				UICamera.Raycast(Input.mousePosition);
				if (UICamera.lastRaycastedCollider != null)
				{
					ReelItem component2 = UICamera.lastRaycastedCollider.gameObject.GetComponent<ReelItem>();
					if ((bool)component2)
					{
						foreach (SplineObject item in _pool)
						{
							if (item.ReelItem == component2)
							{
								if ((item.PositionOnCurve > 0.5f && item.PositionOnCurve <= 0.5f + _cellSize * 1.5f) || (item.PositionOnCurve < 0.5f && item.PositionOnCurve >= 0.5f - _cellSize * 1.5f))
								{
									_centerOnIndex = item.Index;
								}
								AudioManager.Instance.PlaySound(_cardClickSound);
								_velocity = 0f;
								return;
							}
						}
					}
					else
					{
						FindCloseTarget();
						FocusOnClose();
					}
				}
			}
			else if (Mathf.Abs(_velocity) > 1E-06f)
			{
				if (_velocity > 0f)
				{
					for (int num2 = _pool.Count - 1; num2 >= 0; num2--)
					{
						if (_pool[num2].PositionOnCurve + _velocity <= 0.5f)
						{
							_velocity += 0.5f - (_pool[num2].PositionOnCurve + _velocity);
							break;
						}
					}
				}
				else
				{
					for (int i = 0; i < _pool.Count; i++)
					{
						if (_pool[i].PositionOnCurve + _velocity >= 0.5f)
						{
							_velocity += 0.5f - (_pool[i].PositionOnCurve + _velocity);
							break;
						}
					}
				}
			}
			else
			{
				FocusOnClose();
			}
		}
		if (_inSwipe || _inDrag)
		{
			if (_canStartDrag && Vector3.Distance(_lastSwipePosition, _firstSwipePosition) > (float)Screen.width / 100f)
			{
				_canStartDrag = false;
				if (this.ItemStartChanging != null)
				{
					this.ItemStartChanging(Selected);
				}
			}
			Vector3 mousePosition = Input.mousePosition;
			Vector3 vector = new Vector3((mousePosition.x - _lastSwipePosition.x) / (float)Screen.width, (mousePosition.y - _lastSwipePosition.y) / (float)Screen.height, 0f);
			if (_inDrag)
			{
				if (_dragedItem != null)
				{
					_dragedItem.Obj.transform.position = _uiCamera.ScreenToWorldPoint(mousePosition);
				}
				_lastSwipePosition = mousePosition;
				return;
			}
			num = vector.x * _swipeScale;
			_lastSwipePosition = mousePosition;
		}
		if (!UIBlocker.Instance.IsLocked)
		{
			_velocity += num;
		}
	}

	private void ProcessSlotOrDistruction(Collider slot)
	{
		if (slot != null)
		{
			PerkSlot componentInParent = slot.GetComponentInParent<PerkSlot>();
			if (slot != null && _dragedItem.Item is Perk && componentInParent != null)
			{
				PropertiesPanel.Instance.Imbue(componentInParent, _dragedItem.Item);
				if (this.ItemInserted != null)
				{
					this.ItemInserted(_dragedItem.Item, componentInParent);
				}
			}
		}
		StopDragItem(false);
	}

	private SplineObject GetObjectFor(ReelItem item)
	{
		foreach (SplineObject item2 in _pool)
		{
			if (item2.ReelItem == item)
			{
				return item2;
			}
		}
		return null;
	}

	public void ChangeType(EquipmentType type, List<BaseItem> items = null, List<BaseItem> sloted = null)
	{
		ChangeType(_allowDrag, type, items, sloted);
	}

	public void ChangeType(bool allowDrag, EquipmentType type, List<BaseItem> items = null, List<BaseItem> sloted = null, bool refreshImageSack = true)
	{
		_allowDrag = allowDrag;
		Clear();
		if (items == null || items.Count == 0)
		{
			items = UserManager.UserModelInfo.GetItemsOfType(type);
		}
		items.Sort(ItemComparators.GetComparator(ComparerPurpose.ReelDriverDesc));
		if (refreshImageSack)
		{
			if (sloted != null && sloted.Count > 0)
			{
				List<BaseItem> list = new List<BaseItem>(items);
				list.AddRange(sloted);
			}
			InventoryManager.Instance.UpdateSubtypesIcons();
		}
		for (int i = 0; i < items.Count; i++)
		{
			_pool.Add(CreateCard(items[i], i));
		}
		UpdateButtonState((Selected != null) ? GetIndexOf(Selected.id) : 0);
		UpdateProgress();
	}

	private void Clear()
	{
		foreach (SplineObject item in _pool)
		{
			UnityEngine.Object.Destroy(item.Obj);
		}
		_pool.Clear();
		if (this.Cleared != null)
		{
			this.Cleared();
		}
	}

	private bool Move(float amount)
	{
		if (!_inDrag && !_dragBack)
		{
			if (Mathf.Abs(amount) < 1E-05f)
			{
				return false;
			}
			_ajust = Mathf.Clamp(_ajust + amount, (0f - _cellSize) * (float)_pool.Count + 0.5f + _cellSize, 0.5f);
		}
		foreach (SplineObject item in _pool)
		{
			if (item.UpdatePosition())
			{
				AudioManager.Instance.PlaySound(_cardSwipeSound);
				if (this.ItemSwiped != null)
				{
					this.ItemSwiped(item.Item);
				}
			}
		}
		return true;
	}

	private bool VelocityMove()
	{
		if (Mathf.Approximately(_velocity, 0f))
		{
			return false;
		}
		float num = Mathf.Clamp(_ajust + _velocity * 0.2f, (0f - _cellSize) * (float)_pool.Count + 0.5f + _cellSize, 0.5f);
		_velocity -= _velocity * 0.2f;
		if (Math.Abs(num - ((0f - _cellSize) * (float)_pool.Count + 0.5f + _cellSize)) < 1E-05f || Math.Abs(num - 0.5f) < _velocityTolerance)
		{
			_velocity = 0f;
		}
		_ajust = num;
		if (Mathf.Abs(_velocity) < 0.0001f || (!_inSwipe && !_inDrag && Mathf.Abs(_velocity) < 0.1f))
		{
			_velocity = 0f;
			FindCloseTarget();
		}
		foreach (SplineObject item in _pool)
		{
			if (item.UpdatePosition())
			{
				AudioManager.Instance.PlaySound(_cardSwipeSound);
				if (this.ItemSwiped != null)
				{
					this.ItemSwiped(item.Item);
				}
			}
		}
		return true;
	}

	private void FindCloseTarget(float close = 0f)
	{
		float num = 2f;
		_centerOnIndex = -1;
		for (int i = 0; i < _pool.Count; i++)
		{
			float num2 = Mathf.Abs(_pool[i].PositionOnCurve - 0.5f);
			if (num2 < num)
			{
				_centerOnIndex = i;
				num = num2;
			}
		}
	}

	private void FocusOnClose()
	{
		if (_centerOnIndex != -1)
		{
			if (_canStartDrag)
			{
				_canStartDrag = false;
				if (this.ItemStartChanging != null)
				{
					this.ItemStartChanging(Selected);
				}
			}
			_isAnim = true;
			float max = Mathf.Abs(_pool[_centerOnIndex].PositionOnCurve - 0.5f);
			float num = Mathf.Clamp(_centerOnSpeed * Time.deltaTime, 0f, max);
			if (_pool[_centerOnIndex].PositionOnCurve > 0.5f)
			{
				Move(num * -1f);
			}
			else if (_pool[_centerOnIndex].PositionOnCurve < 0.5f)
			{
				Move(num);
			}
			if (Math.Abs(_pool[_centerOnIndex].PositionOnCurve - 0.5f) < 0.001f)
			{
				OnItemSelected(_pool[_centerOnIndex].Item);
				_centerOnIndex = -1;
			}
		}
		else
		{
			_isAnim = false;
		}
	}

	private Vector3 PositionAt(float splinePosition)
	{
		splinePosition = Mathf.Clamp01(splinePosition);
		return new Vector3(_posX.Evaluate(splinePosition), _posY.Evaluate(splinePosition), _posZ.Evaluate(splinePosition));
	}

	private Vector3 RotationAt(float splinePosition)
	{
		splinePosition = Mathf.Clamp01(splinePosition);
		return new Vector3(_rotationX.Evaluate(splinePosition), _rotationY.Evaluate(splinePosition), _rotationZ.Evaluate(splinePosition));
	}

	private void OnItemSelected(BaseItem item)
	{
		if (item != Selected)
		{
			Selected = item;
		}
		UpdateProgress();
		UpdateButtonState(GetIndexOf(Selected.id));
		_isAnim = false;
		_canStartDrag = false;
		if (this.ItemSelected != null)
		{
			this.ItemSelected(item);
		}
	}

	public void FocusOnCenter()
	{
		if (Count > 0)
		{
			int index = Count / 2;
			FocusOnItem(_pool[index].ID, true);
		}
		else
		{
			Selected = null;
			_centerOnIndex = -1;
		}
	}

	public void FocusOnMostSuitableThing()
	{
		if (Count <= 0)
		{
			return;
		}
		int num = -1;
		int num2 = -1;
		for (int i = 0; i < _pool.Count; i++)
		{
			IInformable informable = _pool[i].Item as IInformable;
			if (informable != null && informable.IsNew())
			{
				num = _pool[i].ID;
			}
			if (_pool[i].Item is Equipment && ((Equipment)_pool[i].Item).IsEquipped())
			{
				num2 = _pool[i].ID;
			}
		}
		if (num == -1)
		{
			num = ((num2 != -1) ? num2 : _pool[0].ID);
		}
		else
		{
			foreach (SplineObject item in _pool)
			{
				IInformable informable2 = item.Item as IInformable;
				if (informable2 != null && informable2.IsNew())
				{
					informable2.SetNew(false);
					UserManager.UpdateEquipmentData(item.Item.id);
				}
			}
			Equipment equipment = _pool[0].Item as Equipment;
			if (equipment != null)
			{
				InventoryManager.Instance.UpdateSlot(equipment.GetEquipmentType());
			}
		}
		FocusOnItem(num, true);
	}

	private void StartDragItem(SplineObject item)
	{
		_dragedItem = item;
		_dragedItem.StartDrag();
		_inDrag = true;
	}

	private void StopDragItem(bool removeDragged, Action onTweenFinish = null)
	{
		_sellPivot.SetActive(false);
		if (removeDragged)
		{
			if (_dragedItem != null)
			{
				_dragedItem.ReelItem.Destruct(onTweenFinish);
				_pool.Remove(_dragedItem);
				for (int i = 0; i < _pool.Count; i++)
				{
					_pool[i].SetIndex(i);
				}
				_centerOnIndex = _dragedItem.Index;
				if (_centerOnIndex >= _pool.Count - 1)
				{
					_centerOnIndex = _pool.Count - 1;
				}
			}
			_inDrag = false;
		}
		else if (_dragedItem != null)
		{
			StartCoroutine(ReturnItemToTheReel());
		}
		else
		{
			_inDrag = false;
		}
	}

	private SplineObject CreateCard(BaseItem item, int index = -1)
	{
		if (index == -1)
		{
			for (int i = 0; i < _pool.Count; i++)
			{
				if (index != -1)
				{
					_pool[i].SetIndex(_pool[i].Index + 1);
				}
			}
			if (index == -1)
			{
				index = _pool.Count;
			}
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(_prototype);
		gameObject.transform.parent = base.transform;
		gameObject.transform.localScale = _prototype.transform.localScale;
		gameObject.transform.localPosition = _prototype.transform.localPosition;
		gameObject.SetActive(true);
		return new SplineObject(this, gameObject, index, item);
	}

	private void InsertItemAndDrag(BaseItem item, Action onAppear)
	{
		if (item != null)
		{
			SplineObject splineObject = CreateCard(item);
			_pool.Insert(splineObject.Index, splineObject);
			OnItemSelected(item);
			FocusOnItem(item.id, true);
			splineObject.Obj.SetActive(false);
			StartCoroutine(InsertAndDragRoutine(splineObject, onAppear));
		}
	}

	private IEnumerator InsertAndDragRoutine(SplineObject so, Action onAppear)
	{
		for (int i = 0; i < 3; i++)
		{
			yield return new WaitForEndOfFrame();
		}
		_dragedItem = so;
		_inDrag = true;
		_dragedItem.StartDrag();
		_dragedItem.Obj.transform.position = _uiCamera.ScreenToWorldPoint(Input.mousePosition);
		_dragedItem.ReelItem.UpdateShade(0f);
		_dragedItem.ReelItem.UpdateDepth(10);
		so.Obj.SetActive(true);
		_dragedItem.ReelItem.Appear(onAppear);
	}

	private IEnumerator ReturnItemToTheReel()
	{
		_dragBack = true;
		for (int i = 0; i < 20; i++)
		{
			yield return new WaitForEndOfFrame();
			_dragedItem.Obj.transform.position = Vector3.Lerp(_dragedItem.Obj.transform.position, _dragPivot.position, (float)i / 20f);
			_dragedItem.Obj.transform.localScale = Vector3.Lerp(_dragedItem.Obj.transform.localScale, _startDragScale, (float)i / 20f);
		}
		_inDrag = false;
		_dragBack = false;
		_dragedItem.Obj.transform.parent = base.transform;
		_dragedItem.EndDrag();
	}

	private void CreateProgress()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(_progressbarPrf);
		_progressbar = gameObject.GetComponent<UIProgressBar>();
		gameObject.transform.parent = _controlContainer;
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.position = _uiCamera.ScreenToWorldPoint(_reelCamera.WorldToScreenPoint(PositionAt(0.5f)));
		gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, progressYOffset, gameObject.transform.localPosition.z);
		gameObject.SetActive(false);
	}

	private void UpdateProgress()
	{
		if (_progressbarPrf == null || _progressbar == null)
		{
			return;
		}
		if (_pool.Count == 0)
		{
			_progressbar.gameObject.SetActive(false);
		}
		else
		{
			if (Selected == null)
			{
				return;
			}
			if (Selected is Equipment)
			{
				_progressbar.gameObject.SetActive(true);
				Equipment equipment = Selected as Equipment;
				_progressbar.value = equipment.GetBarValue();
			}
			else if (Selected is Perk)
			{
				_progressbar.gameObject.SetActive(true);
				Perk perk = Selected as Perk;
				if (perk != null)
				{
					_progressbar.value = perk.GetBarValue();
				}
			}
		}
	}

	private void CreateButtons()
	{
		Vector3 position = _uiCamera.ScreenToWorldPoint(_reelCamera.WorldToScreenPoint(PositionAt(0.5f)));
		GameObject gameObject = UnityEngine.Object.Instantiate(_leftBtnPrf);
		_leftBtn = gameObject.GetComponent<UIButton>();
		gameObject.transform.parent = _controlContainer;
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.position = position;
		gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x + _xOffset, _yOffset, gameObject.transform.localPosition.z);
		gameObject.SetActive(false);
		_leftBtn.onClick.Add(new EventDelegate(leftBtnOnClick));
		gameObject = UnityEngine.Object.Instantiate(_rightBtnPrf);
		_rightBtn = gameObject.GetComponent<UIButton>();
		gameObject.transform.parent = _controlContainer;
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.position = position;
		gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x - _xOffset, _yOffset, gameObject.transform.localPosition.z);
		gameObject.SetActive(false);
		_rightBtn.onClick.Add(new EventDelegate(rightBtnOnClick));
	}

	public void leftBtnOnClick()
	{
		int indexOf = GetIndexOf(Selected.id);
		if (indexOf != 0)
		{
			indexOf--;
			UpdateButtonState(indexOf);
			FocusOn(_pool[indexOf].Index, false);
		}
	}

	public void rightBtnOnClick()
	{
		int indexOf = GetIndexOf(Selected.id);
		if (indexOf != _pool.Count - 1)
		{
			indexOf++;
			UpdateButtonState(indexOf);
			FocusOn(indexOf, false);
		}
	}

	private void UpdateButtonState(int current)
	{
		if ((_leftBtnPrf == null && _rightBtnPrf == null) || (_leftBtn == null && _rightBtn == null))
		{
			return;
		}
		if (_pool.Count == 0)
		{
			_leftBtn.gameObject.SetActive(false);
			_rightBtn.gameObject.SetActive(false);
			return;
		}
		if (current == 0)
		{
			_leftBtn.gameObject.SetActive(false);
		}
		else
		{
			_leftBtn.gameObject.SetActive(true);
		}
		if (current == _pool.Count - 1)
		{
			_rightBtn.gameObject.SetActive(false);
		}
		else
		{
			_rightBtn.gameObject.SetActive(true);
		}
	}

	public void DisableReelCamera()
	{
		_reelCamera.gameObject.SetActive(false);
	}

	public void EnableReelCamera()
	{
		_reelCamera.gameObject.SetActive(true);
	}
}
