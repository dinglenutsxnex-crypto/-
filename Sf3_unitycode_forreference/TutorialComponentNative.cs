using UnityEngine;
using UnityEngine.UI;

public class TutorialComponentNative : TutorialComponent
{
	private ButtonSelectionNative _selector;

	private TutorialArrowNative _arrow;

	[SerializeField]
	protected Image _selectorIcon;

	[SerializeField]
	protected UnityEngine.UI.Text _selectorLabel;

	private RectTransform _selectMask;

	private RectTransform _rect;

	public override Vector3 Viewport
	{
		get
		{
			Camera canvasCamera = NekkiCanvasRoot.instance.GetCanvasCamera();
			return canvasCamera.WorldToViewportPoint(base.transform.position);
		}
	}

	public override bool IsNative()
	{
		return true;
	}

	protected override void Update()
	{
	}

	public override void Select()
	{
		SetBlockLayer(true);
		SelectComponent(true);
		TutorialManager.Instance.TutorialBlockNative.ShowDarkness(true);
		TutorialManager.Instance.TutorialBlockNGUI.ShowDarkness(false);
	}

	protected override void Awake()
	{
		TrySetIdFromPrefab();
		InitComponents();
		InitColor();
	}

	protected override void InitComponents()
	{
		_rect = GetComponent<RectTransform>();
		Button component = GetComponent<Button>();
		if (component != null)
		{
			component.onClick.AddListener(base.OnClick);
		}
	}

	protected override void InitColor()
	{
		if (_selectorIcon != null)
		{
			_iconColor = _selectorIcon.color;
		}
		if (_selectorLabel != null)
		{
			_labelColor = _selectorLabel.color;
		}
	}

	protected override void CreateArrow()
	{
		if (arrowPosition != ArrowPosition.None)
		{
			if (_arrow == null)
			{
				_arrow = TutorialManager.Instance.TutorialBlockNative.CreateArrow();
			}
			_arrow.SetPosition(_rect, arrowPosition);
		}
	}

	protected override void CreateSelectionBorder()
	{
		if (!(selectorPrf == null))
		{
			if (_selector == null)
			{
				_selector = TutorialManager.Instance.TutorialBlockNative.AddSelection(selectorPrf);
			}
			_selector.Set(_rect, _selectorIcon, _selectorLabel);
		}
	}

	protected override void CreateSelectionMask()
	{
		if (!(selectMaskPrf == null) && _selectMask == null)
		{
			_selectMask = TutorialMaskGenerator.Instance.AddMask(selectMaskPrf, _rect);
		}
	}

	protected override void SelectComponent(bool select)
	{
		_isSelect = select;
		isDragable = false;
		isDragTarget = false;
		if (select)
		{
			CreateSelectionParts();
			SetSelectionIconColor(Color.clear);
			SetSelectionLabelColor(Color.clear);
		}
		else
		{
			DestroySelectionParts();
			SetSelectionIconColor(_iconColor);
			SetSelectionLabelColor(_labelColor);
		}
	}

	protected void SetSelectionIconColor(Color color)
	{
		if (_selectorIcon != null)
		{
			_selectorIcon.color = color;
		}
	}

	protected void SetSelectionLabelColor(Color color)
	{
		if (_selectorLabel != null)
		{
			_selectorLabel.color = color;
		}
	}

	protected override void DestroySelectionParts()
	{
		base.DestroySelectionParts();
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
}
