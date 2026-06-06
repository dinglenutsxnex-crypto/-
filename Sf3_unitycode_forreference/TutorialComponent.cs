using UnityEngine;

public class TutorialComponent : MonoBehaviour
{
	public string id;

	public string groupTag;

	public ArrowPosition arrowPosition;

	public bool isDragable;

	public bool isDragTarget;

	protected bool _isSelect;

	[SerializeField]
	protected GameObject selectorPrf;

	private ButtonSelectionNGUI _selector;

	[SerializeField]
	protected Vector2 borderPadding;

	[SerializeField]
	protected UISprite icon;

	[SerializeField]
	protected UILabel label;

	[SerializeField]
	protected GameObject selectMaskPrf;

	private UIWidget _selectMask;

	protected Color _iconColor;

	protected Color _labelColor;

	private TutorialArrowNGUI _arrow;

	protected UIWidget _widget;

	protected Camera _reelCamera;

	protected bool _isDrag;

	protected bool _isVisible = true;

	private Vector2 _lastTouch;

	public bool IsLocked { get; protected set; }

	public virtual Vector3 Viewport
	{
		get
		{
			return UICamera.mainCamera.WorldToViewportPoint(base.transform.position);
		}
	}

	public virtual bool IsNative()
	{
		return false;
	}

	protected virtual void CreateSelectionParts()
	{
		CreateArrow();
		CreateSelectionBorder();
		CreateSelectionMask();
	}

	protected virtual void CreateArrow()
	{
		if (arrowPosition != ArrowPosition.None)
		{
			_arrow = TutorialManager.Instance.TutorialBlockNGUI.CreateArrow();
			_arrow.SetPosition(_widget, arrowPosition);
		}
	}

	protected virtual void CreateSelectionBorder()
	{
		if (!(selectorPrf == null))
		{
			_selector = TutorialManager.Instance.TutorialBlockNGUI.AddSelection(selectorPrf);
			_selector.Set(_widget, borderPadding, icon, label);
		}
	}

	protected virtual void CreateSelectionMask()
	{
		if (!(selectMaskPrf == null))
		{
			_selectMask = TutorialManager.Instance.TutorialBlockNGUI.AddMask(selectMaskPrf);
			SetSettings(_selectMask);
		}
	}

	protected virtual void DestroySelectionParts()
	{
		if (_selector != null)
		{
			Object.Destroy(_selector.gameObject);
		}
		if (_arrow != null)
		{
			Object.Destroy(_arrow.gameObject);
		}
		if (_selectMask != null)
		{
			Object.Destroy(_selectMask.gameObject);
		}
	}

	protected virtual void Awake()
	{
		TrySetIdFromPrefab();
		InitComponents();
		InitColor();
	}

	private void Start()
	{
		if (base.transform.parent != null)
		{
			_reelCamera = base.transform.parent.gameObject.GetComponentInChildren<Camera>();
		}
	}

	protected virtual void TrySetIdFromPrefab()
	{
		if (TutorialManager.Instance == null)
		{
			Debug.LogWarning("Couldn't find tutorial manager");
		}
		else if (!string.IsNullOrEmpty(id))
		{
			SetId(id);
		}
	}

	protected virtual void InitComponents()
	{
		UIButton component = GetComponent<UIButton>();
		if (component != null)
		{
			component.onClick.Add(new EventDelegate(OnClick));
		}
		_widget = GetComponent<UIWidget>();
	}

	protected virtual void InitColor()
	{
		if (icon != null)
		{
			_iconColor = icon.color;
		}
		if (label != null)
		{
			_labelColor = label.color;
		}
	}

	public void OnClick()
	{
		if (IsLocked)
		{
			Release();
			TutorialManager.Instance.Release(id);
		}
	}

	private void OnDestroy()
	{
		if (_selector != null)
		{
			Object.Destroy(_selector.gameObject);
		}
		TutorialManager.Instance.RemoveWithCheck(id, base.gameObject);
	}

	protected void OnDraging(bool start)
	{
		if (isDragable)
		{
			_isDrag = start;
			if (_selector != null)
			{
				_selector.gameObject.SetActive(!_isDrag);
			}
		}
	}

	public void SetBlockLayer(bool value)
	{
		IsLocked = value;
	}

	public virtual void Select()
	{
		SetBlockLayer(true);
		SelectComponent(true);
		TutorialManager.Instance.TutorialBlockNative.ShowDarkness(false);
		TutorialManager.Instance.TutorialBlockNGUI.ShowDarkness(true);
	}

	public void Release()
	{
		SelectComponent(false);
		IsLocked = false;
		TutorialManager.Instance.TutorialBlockNative.ShowDarkness(false);
		TutorialManager.Instance.TutorialBlockNGUI.ShowDarkness(false);
	}

	protected virtual void SelectComponent(bool select)
	{
		if (select)
		{
			CreateSelectionParts();
		}
		else
		{
			DestroySelectionParts();
		}
		_isSelect = select;
		isDragable = false;
		isDragTarget = false;
		if (icon != null)
		{
			icon.color = ((!select) ? _iconColor : Color.clear);
		}
		if (label != null)
		{
			label.color = ((!select) ? _labelColor : Color.clear);
		}
	}

	public void SetId(string id)
	{
		TutorialManager.Instance.Remove(this.id);
		this.id = id;
		TutorialManager.Instance.Add(this.id, this);
	}

	protected virtual void Update()
	{
		if (!IsLocked || !Input.GetMouseButtonDown(0))
		{
			return;
		}
		Vector2 vector = Input.mousePosition;
		if (vector != _lastTouch)
		{
			_lastTouch = vector;
			Vector3 position = UICamera.currentCamera.ScreenToWorldPoint(vector);
			position = UICamera.currentCamera.transform.InverseTransformPoint(position);
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			Bounds bounds;
			if (_reelCamera != null)
			{
				bounds = NGUIMath.CalculateRelativeWidgetBounds(_reelCamera.transform, _widget.transform);
				Vector3 position2 = _reelCamera.WorldToScreenPoint(_reelCamera.transform.TransformPoint(bounds.center));
				zero = UICamera.currentCamera.transform.InverseTransformPoint(UICamera.currentCamera.ScreenToWorldPoint(position2));
			}
			else
			{
				bounds = NGUIMath.CalculateRelativeWidgetBounds(UICamera.currentCamera.transform, _widget.transform);
				zero = bounds.center;
			}
			zero2 = bounds.size;
			float num = zero.x - zero2.x / 2f;
			float num2 = zero.y - zero2.y / 2f;
			float num3 = zero.x + zero2.x / 2f;
			float num4 = zero.y + zero2.y / 2f;
			if (position.x > num && position.x < num3 && position.y > num2 && position.y < num4 && _reelCamera == null && !isDragTarget)
			{
				_widget.SendMessage("OnClick");
			}
		}
	}

	public bool GetVisible()
	{
		return _isVisible;
	}

	public void SetVisible(bool visible)
	{
		_isVisible = visible;
		base.gameObject.SetActive(visible);
	}

	private void SetSettings(UIWidget obj)
	{
		_reelCamera = base.transform.parent.gameObject.GetComponentInChildren<Camera>();
		if (_reelCamera == null)
		{
			obj.SetAnchor(_widget.gameObject, 0, 0, 0, 0);
			return;
		}
		obj.SetAnchor((Transform)null);
		obj.height = _widget.height;
		obj.width = _widget.width;
		obj.pivot = _widget.pivot;
		obj.transform.position = GetReelWidgetPosition();
		obj.transform.localScale = GetReelWidgetSacale();
	}

	protected Vector3 GetReelWidgetPosition()
	{
		Vector3 position = _reelCamera.WorldToScreenPoint(_widget.transform.position);
		return UICamera.currentCamera.ScreenToWorldPoint(position);
	}

	protected Vector3 GetReelWidgetSacale()
	{
		return _reelCamera.gameObject.transform.parent.localScale;
	}
}
