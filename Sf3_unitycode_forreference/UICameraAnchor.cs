using System;
using UnityEngine;

[ExecuteInEditMode]
public class UICameraAnchor : MonoBehaviour
{
	public enum Side
	{
		BottomLeft = 0,
		Left = 1,
		TopLeft = 2,
		Top = 3,
		TopRight = 4,
		Right = 5,
		BottomRight = 6,
		Bottom = 7,
		Center = 8
	}

	[SerializeField]
	private Camera _uiCamera;

	[SerializeField]
	private bool _anchorHorizontal = true;

	[SerializeField]
	private bool _anchorVertical = true;

	[SerializeField]
	private Side _side = Side.Center;

	[SerializeField]
	private bool _executeAndDisable = true;

	[SerializeField]
	[HideInInspector]
	private Vector3 _relativeOffset = Vector3.zero;

	private Transform _transform;

	private Animation _animation;

	private Rect _cameraRect = default(Rect);

	private bool _isStarted;

	private void Awake()
	{
		_transform = base.transform;
		_animation = GetComponent<Animation>();
		UICamera.onScreenResize = (UICamera.OnScreenResize)Delegate.Combine(UICamera.onScreenResize, new UICamera.OnScreenResize(ScreenSizeChanged));
	}

	private void OnDestroy()
	{
		UICamera.onScreenResize = (UICamera.OnScreenResize)Delegate.Remove(UICamera.onScreenResize, new UICamera.OnScreenResize(ScreenSizeChanged));
	}

	private void ScreenSizeChanged()
	{
		if (_isStarted && _executeAndDisable)
		{
			Update();
		}
	}

	private void Start()
	{
		if (_uiCamera == null)
		{
			_uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
		}
		Update();
		_isStarted = true;
	}

	private void Update()
	{
		UpdatePosition();
	}

	public void ForcedUpdate()
	{
		if (_uiCamera == null)
		{
			_uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
		}
		Update();
		_isStarted = true;
	}

	private void UpdatePosition()
	{
		if ((_animation != null && _animation.enabled && _animation.isPlaying) || !(_uiCamera != null))
		{
			return;
		}
		_cameraRect = _uiCamera.pixelRect;
		float x = (_cameraRect.xMin + _cameraRect.xMax) * 0.5f;
		float y = (_cameraRect.yMin + _cameraRect.yMax) * 0.5f;
		Vector3 position = new Vector3(x, y, 0f);
		float width = _cameraRect.width;
		float height = _cameraRect.height;
		if (_side != Side.Center)
		{
			if (_side == Side.Right || _side == Side.TopRight || _side == Side.BottomRight)
			{
				position.x = _cameraRect.xMax;
			}
			else if (_side == Side.Top || _side == Side.Center || _side == Side.Bottom)
			{
				position.x = x;
			}
			else
			{
				position.x = _cameraRect.xMin;
			}
			if (_side == Side.Top || _side == Side.TopRight || _side == Side.TopLeft)
			{
				position.y = _cameraRect.yMax;
			}
			else if (_side == Side.Left || _side == Side.Center || _side == Side.Right)
			{
				position.y = y;
			}
			else
			{
				position.y = _cameraRect.yMin;
			}
		}
		Vector3 vector = _uiCamera.WorldToScreenPoint(_transform.position);
		if (_anchorHorizontal)
		{
			position.x += _relativeOffset.x * width;
		}
		else
		{
			position.x = vector.x;
		}
		if (_anchorVertical)
		{
			position.y += _relativeOffset.y * height;
		}
		else
		{
			position.y = vector.y;
		}
		if (_uiCamera.orthographic)
		{
			position.x = Mathf.Round(position.x);
			position.y = Mathf.Round(position.y);
		}
		position.z = vector.z;
		position = _uiCamera.ScreenToWorldPoint(position);
		if (_transform.position != position)
		{
			_transform.position = position;
		}
		if (_executeAndDisable && Application.isPlaying)
		{
			base.enabled = false;
		}
	}
}
