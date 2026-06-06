using System;
using System.Collections;
using SF3.Items;
using UnityEngine;

public class ReelItem : MonoBehaviour
{
	public delegate void OnDrag();

	public delegate void OnHoverDelegate(ReelItem target, bool isOver);

	public delegate void OnPressDelegate(ReelItem target, bool press);

	public enum RewardStatus
	{
		NEW = 0,
		UPGRADE = 1
	}

	[Serializable]
	public class DepthLayer
	{
		[SerializeField]
		public int Depth;

		[SerializeField]
		public UIWidget[] Widgets;
	}

	protected Transform _transform;

	private int _maxLocalDepth;

	[SerializeField]
	private UIButton _button;

	[SerializeField]
	private BoxCollider _collider;

	[SerializeField]
	protected UISprite _shade;

	[SerializeField]
	protected float selectScale = 0.3f;

	[SerializeField]
	protected UISprite _selectionBorder;

	protected TweenScale _tweenScale;

	protected TutorialComponent _tutorialComponent;

	public DepthLayer[] layers;

	private BaseItem _item;

	private GameObject _flipPivot;

	protected bool _selectionBorderActive;

	protected Vector3 beginScale;

	public UIButton Button
	{
		get
		{
			return _button;
		}
	}

	public BaseItem Item
	{
		get
		{
			return _item;
		}
		protected set
		{
			_item = value;
			this.OnItemUpdate(_item);
		}
	}

	public event OnDrag OnDragStartEvent;

	public event OnDrag OnDragEndEvent;

	public event OnHoverDelegate OnHoverEvent;

	public event OnPressDelegate OnPressEvent;

	public event Action<BaseItem> OnItemUpdate = delegate
	{
	};

	private void Awake()
	{
		_transform = base.transform;
		_tutorialComponent = base.gameObject.GetComponent<TutorialComponent>();
		_tweenScale = GetComponent<TweenScale>();
		if (_tweenScale != null)
		{
			_tweenScale.enabled = false;
		}
		_selectionBorderActive = false;
		_selectionBorder.gameObject.SetActive(false);
	}

	public void ActivateSelectionBorder(bool activate)
	{
		if (_selectionBorderActive != activate)
		{
			_selectionBorderActive = activate;
			if (activate)
			{
				_tweenScale.from = beginScale;
				_tweenScale.to = new Vector3(beginScale.x + selectScale, beginScale.y + selectScale, beginScale.z + selectScale);
				_tweenScale.tweenFactor = 0f;
				_tweenScale.PlayForward();
			}
			else
			{
				_tweenScale.enabled = false;
				_transform.localScale = beginScale;
			}
		}
	}

	public virtual Vector2 LocalSize()
	{
		return Vector2.zero;
	}

	public void AddClickCallback(EventDelegate newCallBack)
	{
		_button.onClick.Add(newCallBack);
	}

	public void ActivateButton(bool isActive)
	{
		_button.enabled = isActive;
	}

	public void ActivateCollider(bool isActive)
	{
		_collider.enabled = isActive;
	}

	public void UpdateDepth(int baseDepth)
	{
		DepthLayer[] array = layers;
		foreach (DepthLayer depthLayer in array)
		{
			UIWidget[] widgets = depthLayer.Widgets;
			foreach (UIWidget uIWidget in widgets)
			{
				uIWidget.depth = depthLayer.Depth + baseDepth * _maxLocalDepth;
			}
		}
	}

	public void Drag(bool drag)
	{
		base.gameObject.SetActive(false);
		base.gameObject.SetActive(true);
		if (this.OnDragStartEvent != null && drag)
		{
			this.OnDragStartEvent();
		}
		if (this.OnDragEndEvent != null && !drag)
		{
			this.OnDragEndEvent();
		}
	}

	public void Destruct(Action onTweenFinish = null)
	{
		StartCoroutine(DestructRoutine(onTweenFinish));
	}

	private IEnumerator DestructRoutine(Action onTweenFinish)
	{
		for (int i = 0; i < 10; i++)
		{
			_transform.localScale *= 0.9f;
			yield return new WaitForEndOfFrame();
		}
		if (onTweenFinish != null)
		{
			onTweenFinish();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void Appear(Action onTweenFinish = null)
	{
		StartCoroutine(AppearRoutine(onTweenFinish));
	}

	private IEnumerator AppearRoutine(Action onTweenFinish)
	{
		Vector3 scale = _transform.localScale;
		for (int i = 0; i < 10; i++)
		{
			_transform.localScale = scale / 10f * i;
			yield return new WaitForEndOfFrame();
		}
		_transform.localScale = scale;
		if (onTweenFinish != null)
		{
			onTweenFinish();
		}
	}

	protected void FindMaxLocalDepth()
	{
		DepthLayer[] array = layers;
		foreach (DepthLayer depthLayer in array)
		{
			if (depthLayer.Depth > _maxLocalDepth)
			{
				_maxLocalDepth = depthLayer.Depth;
			}
		}
	}

	private void OnHover(bool isOver)
	{
		if (this.OnHoverEvent != null)
		{
			this.OnHoverEvent(this, isOver);
		}
	}

	private void OnPress(bool press)
	{
		if (this.OnPressEvent != null)
		{
			this.OnPressEvent(this, press);
		}
	}

	public virtual void Init(BaseItem item)
	{
	}

	public virtual void ShowRewardStatus(RewardStatus status)
	{
	}

	public virtual void HideRewardStatus()
	{
	}

	protected GameObject GetFlipPivot()
	{
		CreateFlipPivot();
		return _flipPivot;
	}

	protected void SetFlipPivot(Vector3 pivotPoint)
	{
		CreateFlipPivot();
		_flipPivot.transform.localPosition = pivotPoint;
	}

	private void CreateFlipPivot()
	{
		if (_flipPivot == null)
		{
			_flipPivot = new GameObject();
			_flipPivot.transform.SetParent(base.transform);
			_flipPivot.name = "FlipPivot";
		}
	}

	public void UpdateItem()
	{
		this.OnItemUpdate(_item);
	}
}
