using System;
using System.Collections;
using System.Collections.Generic;
using Nekki.UI;
using SF3.UserData;
using UnityEngine;

[RequireComponent(typeof(NekkiUIDragObject))]
public class NekkiUIDragScaleWidget : MonoBehaviour
{
	private const float MinimalScroll = 0.001f;

	[SerializeField]
	private float _scaleMultiplier = 2f;

	[SerializeField]
	private float maxScale = 2f;

	[SerializeField]
	private int wheelMoveNumber = 20;

	[SerializeField]
	private bool autoZoomOnDoubleClick = true;

	private UIWidget _widget;

	private NekkiUIDragObject _dragScript;

	private Camera cam;

	private float minScale = float.MinValue;

	private float scaleFactor;

	private float wheelScaleFactor;

	private Vector3 lastPosition;

	private float lastDistance = -1f;

	private float lastScale;

	private Vector3 lastScalePoint;

	private int pressCount;

	public Action OnScaleChange;

	[SerializeField]
	private List<Transform> unscalableWidgets;

	[SerializeField]
	private Vector3 unscalableWidgetsLossyScale;

	[SerializeField]
	private float maxExtendedScale = 2.3f;

	[SerializeField]
	private float backToMaxScaleDuration = 0.5f;

	private bool doubleClick;

	private float _widgetScale = 1f;

	public UIWidget Target
	{
		get
		{
			return _widget;
		}
		set
		{
			if (_widget == null && value != null)
			{
				UICamera.onScreenResize = (UICamera.OnScreenResize)Delegate.Combine(UICamera.onScreenResize, new UICamera.OnScreenResize(OnScreenResize));
			}
			_widget = value;
			if (_widget == null)
			{
				_dragScript.target = null;
				_dragScript.enabled = false;
				UICamera.onScreenResize = (UICamera.OnScreenResize)Delegate.Remove(UICamera.onScreenResize, new UICamera.OnScreenResize(OnScreenResize));
				base.enabled = false;
			}
			else
			{
				_widget.transform.localScale = new Vector3(_widgetScale, _widgetScale, _widgetScale);
				_dragScript.target = value.transform;
				CheckCamera();
				UpdateWidgetMinScale();
				_dragScript.enabled = true;
				base.enabled = true;
			}
		}
	}

	public float WidgetScale
	{
		get
		{
			return (!(_widget != null)) ? _widgetScale : _widget.transform.localScale.x;
		}
		set
		{
			_widgetScale = value;
			SetWidgetScale(value);
		}
	}

	public event Action onScale;

	private IEnumerator Start()
	{
		yield return new WaitForEndOfFrame();
		while (!_dragScript)
		{
			_dragScript = GetComponent<NekkiUIDragObject>();
		}
		if (_dragScript.target != null)
		{
			while (!Target)
			{
				Target = _dragScript.target.GetComponent<UIWidget>();
				yield return new WaitForEndOfFrame();
			}
			if (_widget != null)
			{
				UpdateWidgetMinScale();
			}
			OnScroll(0.001f);
		}
		else
		{
			_dragScript.enabled = false;
			base.enabled = false;
		}
	}

	private void OnDestroy()
	{
		UICamera.onScreenResize = (UICamera.OnScreenResize)Delegate.Remove(UICamera.onScreenResize, new UICamera.OnScreenResize(OnScreenResize));
	}

	private void CheckCamera()
	{
		if (cam == null)
		{
			cam = NGUITools.FindCameraForLayer(_widget.transform.parent.gameObject.layer);
		}
	}

	private void Update()
	{
		if (_dragScript == null)
		{
			return;
		}
		_dragScript.enabled = Input.touchCount < 2;
		if (Input.touchCount >= 2 && pressCount >= 1 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary) && (Input.GetTouch(1).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Stationary))
		{
			if (lastDistance > -1f)
			{
				float num = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position) - lastDistance;
				Vector3 scalePoint = getScalePoint();
				Vector3 vector = lastPosition - scalePoint;
				WidgetScale = lastScale + scaleFactor * num;
				UserManager.SetMapScale(WidgetScale);
				float num2 = WidgetScale / lastScale;
				vector.x *= num2;
				vector.y *= num2;
				_widget.transform.localPosition = scalePoint * 2f + vector - lastScalePoint;
				_dragScript.ConstrainToBounds();
				SendMessage("OnMultiTouchScale");
				this.onScale.InvokeSafe();
			}
			else
			{
				lastDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
				lastScale = WidgetScale;
				lastPosition = _widget.transform.localPosition;
				lastScalePoint = getScalePoint();
			}
		}
		else
		{
			lastDistance = -1f;
		}
	}

	private Vector3 getScalePoint()
	{
		Vector2 vector = Vector2.Lerp(Input.GetTouch(0).position, Input.GetTouch(1).position, 0.5f);
		CheckCamera();
		Vector3 position = cam.ScreenToWorldPoint(new Vector3(vector.x, vector.y));
		return _widget.transform.parent.InverseTransformPoint(position);
	}

	public void OnScroll(float delta)
	{
		Vector3 pos;
		ZoomByScroll(delta * _scaleMultiplier, out pos);
		this.onScale.InvokeSafe();
	}

	private float ZoomByScroll(float delta, out Vector3 pos, bool setData = true)
	{
		Vector3 scalePoint = _widget.transform.parent.worldToLocalMatrix.MultiplyPoint3x4(UICamera.lastWorldPosition);
		return ZoomToPoint(WidgetScale + wheelScaleFactor * delta, scalePoint, out pos, setData);
	}

	private float ZoomToPoint(float scale, Vector3 scalePoint, out Vector3 pos, bool setData = true)
	{
		Vector3 vector = _widget.transform.localPosition - scalePoint;
		Vector3 localScale = _widget.transform.localScale;
		SetWidgetScale(scale, setData);
		float widgetScale = WidgetScale;
		float num = WidgetScale / localScale.x;
		vector.x *= num;
		vector.y *= num;
		Vector3 localPosition = _widget.transform.localPosition;
		_widget.transform.localPosition = scalePoint + vector;
		_dragScript.ConstrainToBounds();
		pos = _widget.transform.localPosition;
		if (!setData)
		{
			_widget.transform.localPosition = localPosition;
			_widget.transform.localScale = localScale;
			UpdateUnscalableWidgets();
		}
		UserManager.SetMapScale(widgetScale);
		return widgetScale;
	}

	private void OnPress(bool state)
	{
		pressCount += (state ? 1 : (-1));
	}

	private void OnDoubleClick()
	{
		if (autoZoomOnDoubleClick)
		{
			doubleClick = true;
			Vector3 pos;
			float num = ZoomByScroll((!(WidgetScale >= maxScale)) ? 1000 : (-1000), out pos, false);
			TweenPosition.Begin(_widget.gameObject, 0.15f, pos);
			TweenScale.Begin(_widget.gameObject, 0.15f, new Vector3(num, num, 1f));
		}
	}

	private void SetWidgetScale(float value, bool informChange = true)
	{
		value = Mathf.Clamp(value, minScale, (!doubleClick) ? maxExtendedScale : maxScale);
		doubleClick = false;
		if (_widget != null && _widget.transform.localScale.x != value)
		{
			_widget.transform.localScale = new Vector3(value, value, 1f);
			UpdateUnscalableWidgets();
			if (informChange && OnScaleChange != null)
			{
				OnScaleChange();
			}
		}
	}

	private void OnScreenResize()
	{
		UpdateWidgetMinScale();
		_dragScript.ConstrainToBounds();
	}

	private void UpdateWidgetMinScale()
	{
		float num = ((_dragScript.panel.clipping != 0) ? _dragScript.panel.baseClipRegion.z : ((float)NekkiUIRoot.Instance.ScreenWidth));
		float num2 = ((_dragScript.panel.clipping != 0) ? _dragScript.panel.baseClipRegion.w : ((float)NekkiUIRoot.Instance.ScreenHeight));
		minScale = Mathf.Max(num / (float)_widget.width, num2 / (float)_widget.height);
		scaleFactor = (maxScale - minScale) * 2f / (float)Screen.width * _scaleMultiplier;
		wheelScaleFactor = (maxScale - minScale) * 10f / (float)wheelMoveNumber;
		WidgetScale = WidgetScale;
	}

	public void AddUnscalableWidget(Transform widget)
	{
		unscalableWidgets.Add(widget);
		UpdateUnscalableWidget(widget);
	}

	public void RemoveUnscalableWidget(Transform widget)
	{
		unscalableWidgets.Remove(widget);
	}

	private void UpdateUnscalableWidgets()
	{
		for (int i = 0; i < unscalableWidgets.Count; i++)
		{
			if (unscalableWidgets[i] == null)
			{
				unscalableWidgets.RemoveAt(i);
				i--;
			}
			else
			{
				UpdateUnscalableWidget(unscalableWidgets[i]);
			}
		}
	}

	private void UpdateUnscalableWidget(Transform widget)
	{
		Transform parent = widget.parent;
		widget.parent = null;
		widget.localScale = unscalableWidgetsLossyScale;
		widget.parent = parent;
	}
}
