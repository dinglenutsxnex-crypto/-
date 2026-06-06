using System;
using UnityEngine;

public class UIScrollViewCustom : UIScrollView
{
	public enum CellVisibility
	{
		FULL = 0,
		NONE = 1,
		TOP_PART = 2,
		BOTTOM_PART = 3
	}

	[SerializeField]
	private Vector2 _cellSize;

	[SerializeField]
	private AnimationCurve _scrollInertiaCurve;

	[SerializeField]
	private float _scrollInertiaTime = 0.5f;

	[SerializeField]
	public Vector2 padding = Vector2.zero;

	private bool _isInertia;

	private Vector3 _lastPosition;

	private Vector3 _direction;

	private float _inertiaTimeEnd;

	private Vector3 _lastScrollLocalPoint;

	private Vector3 _nextScrollLocalPoint;

	private Vector3 _lastScrollPanelPoint;

	private Vector3 _nextScrollPanelPoint;

	private Vector3 _minScrollPoint;

	private Vector3 _maxScrollPoint;

	private int _visibleRowsCount;

	private int _rowsCount;

	public bool disableRecalcBounds;

	private Bounds cacheBounds;

	private Vector2 _currentVisibleRows;

	public Vector2 cellSize
	{
		get
		{
			return _cellSize;
		}
		set
		{
			_cellSize.x = Mathf.Abs(value.x);
			_cellSize.y = Mathf.Abs(value.y);
		}
	}

	public bool isScrolling
	{
		get
		{
			return _isInertia;
		}
	}

	public override Bounds bounds
	{
		get
		{
			if (!disableRecalcBounds)
			{
				mCalculatedBounds = false;
				cacheBounds = base.bounds;
				cacheBounds.size = new Vector3(cacheBounds.size.x + padding.x, cacheBounds.size.y + padding.y, cacheBounds.size.z);
			}
			return cacheBounds;
		}
	}

	public Vector2 CurrentVisibleRows
	{
		get
		{
			if (_currentVisibleRows == Vector2.zero)
			{
				Vector2 vector = mTrans.localPosition - _minScrollPoint;
				_currentVisibleRows = Vector2.zero;
				if (_direction.y < 0f)
				{
					_currentVisibleRows.x = (int)Mathf.Ceil(vector.y / _cellSize.y);
				}
				else
				{
					_currentVisibleRows.x = (int)Mathf.Floor(vector.y / _cellSize.y);
				}
				_currentVisibleRows.y = _currentVisibleRows.x + (float)_visibleRowsCount - 1f;
			}
			return _currentVisibleRows;
		}
	}

	private void Awake()
	{
		mPanel = GetComponent<UIPanel>();
		mTrans = base.transform;
		onDragStarted = (OnDragNotification)Delegate.Combine(onDragStarted, new OnDragNotification(OnScrollDragStarted));
		onDragFinished = (OnDragNotification)Delegate.Combine(onDragFinished, new OnDragNotification(OnScrollDragFinished));
		_lastPosition = mTrans.localPosition;
	}

	private void LateUpdate()
	{
		if (_isInertia)
		{
			if (_inertiaTimeEnd > Time.time)
			{
				float num = _inertiaTimeEnd - Time.time;
				float t = _scrollInertiaCurve.Evaluate(num / _scrollInertiaTime);
				mTrans.localPosition = Vector3.Lerp(_lastScrollLocalPoint, _nextScrollLocalPoint, t);
				mPanel.clipOffset = Vector3.Lerp(_lastScrollPanelPoint, _nextScrollPanelPoint, t);
			}
			else
			{
				_isInertia = false;
				mTrans.localPosition = _nextScrollLocalPoint;
				mPanel.clipOffset = _nextScrollPanelPoint;
			}
			RestrictWithinBounds(false, base.canMoveHorizontally, base.canMoveVertically);
		}
		_currentVisibleRows = Vector2.zero;
	}

	private void OnScrollDragStarted()
	{
		_lastPosition = mTrans.localPosition;
	}

	private void OnScrollDragFinished()
	{
		Vector3 direction = mTrans.localPosition - _lastPosition;
		if (!(Mathf.Abs(direction.y) < 1f))
		{
			_direction = direction;
			if (_direction.y > 0f)
			{
				ScrollDownByCell();
			}
			else
			{
				ScrollUpByCell();
			}
			_isInertia = true;
		}
	}

	public void ScrollVerticalToCell(int cellRowNumber)
	{
		if (!((float)cellRowNumber >= CurrentVisibleRows.x) || !((float)cellRowNumber <= CurrentVisibleRows.y))
		{
			int num = ((!((float)cellRowNumber < CurrentVisibleRows.x)) ? (cellRowNumber - (int)CurrentVisibleRows.y) : (cellRowNumber - (int)CurrentVisibleRows.x));
			_lastScrollLocalPoint = mTrans.localPosition;
			_nextScrollLocalPoint.y = _lastScrollLocalPoint.y + (float)num * _cellSize.y;
			if (_nextScrollLocalPoint.y > _maxScrollPoint.y)
			{
				_nextScrollLocalPoint.y = _maxScrollPoint.y;
			}
			if (_nextScrollLocalPoint.y < _minScrollPoint.y)
			{
				_nextScrollLocalPoint.y = _minScrollPoint.y;
			}
			_lastScrollPanelPoint = mPanel.clipOffset;
			_nextScrollPanelPoint = _nextScrollLocalPoint;
			_nextScrollPanelPoint.y *= -1f;
			_inertiaTimeEnd = Time.time + _scrollInertiaTime;
			_isInertia = true;
		}
	}

	public void ScrollDownByCell()
	{
		_direction.y = 1f;
		if (!(CurrentVisibleRows.y >= (float)(_rowsCount - 1)))
		{
			ScrollVerticalToCell((int)CurrentVisibleRows.y + 1);
		}
	}

	public void ScrollUpByCell()
	{
		_direction.y = -1f;
		if (!(CurrentVisibleRows.x <= 0f))
		{
			ScrollVerticalToCell((int)CurrentVisibleRows.x - 1);
		}
	}

	public new void ResetPosition()
	{
		base.ResetPosition();
		_isInertia = false;
	}

	public new void InvalidateBounds()
	{
		base.InvalidateBounds();
		_minScrollPoint = mTrans.localPosition;
		_maxScrollPoint = new Vector2(_minScrollPoint.x, _minScrollPoint.y);
		_maxScrollPoint.x += bounds.size.x - mPanel.baseClipRegion.z + padding.x;
		_maxScrollPoint.y += bounds.size.y - mPanel.baseClipRegion.w + padding.y;
		_visibleRowsCount = (int)Mathf.Floor(mPanel.baseClipRegion.w / _cellSize.y);
		_rowsCount = (int)Mathf.Ceil(bounds.size.y / _cellSize.y);
	}

	public void RecalculateBounds()
	{
		Vector3 localPosition = mTrans.localPosition;
		Vector2 clipOffset = mPanel.clipOffset;
		ResetPosition();
		InvalidateBounds();
		disableRecalcBounds = true;
		mTrans.localPosition = localPosition;
		mPanel.clipOffset = clipOffset;
	}

	public CellVisibility GetVisibilityForCell(Transform cell)
	{
		return GetVisibilityForCell(cell, _cellSize);
	}

	public CellVisibility GetVisibilityForCell(Transform cell, Vector2 cellGraphicsSize)
	{
		if (cell.parent != base.transform)
		{
			return CellVisibility.NONE;
		}
		Vector2 vector = mTrans.localPosition - _minScrollPoint;
		Vector2 vector2 = vector + mPanel.GetViewSize();
		Vector2 vector3 = (_cellSize - cellGraphicsSize) / 2f;
		Vector3 vector4 = -cell.transform.localPosition;
		bool flag = vector4.y + vector3.y >= vector.y;
		bool flag2 = vector4.y + _cellSize.y - vector3.y <= vector2.y;
		if (flag)
		{
			if (flag2)
			{
				return CellVisibility.FULL;
			}
			return CellVisibility.TOP_PART;
		}
		if (flag2)
		{
			return CellVisibility.BOTTOM_PART;
		}
		return CellVisibility.NONE;
	}
}
