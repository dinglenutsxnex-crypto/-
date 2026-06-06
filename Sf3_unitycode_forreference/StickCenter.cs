using SF3;
using UnityEngine;

public class StickCenter : ExtentionBehaviour
{
	[SerializeField]
	private Camera _uiCamera;

	private Vector3 TargetPosition;

	private Vector3 BasePosition;

	private Vector3 CaclulatadPosision;

	public float MaxRadius;

	public float SafeRadius;

	private bool _inDrag;

	public float DeltaSpeed = 1.2f;

	[SerializeField]
	private Stick _joystick;

	[SerializeField]
	private MultiTweenTransition _multiTween;

	[SerializeField]
	private SphereCollider SphereCollider;

	public LayerMask JoystickLayer;

	private bool InSafeRadius
	{
		get
		{
			return Vector3.Distance(base.transform.localPosition, BasePosition) < SafeRadius;
		}
	}

	private Vector3 baseDeltaTouch
	{
		get
		{
			if (SystemProperties.IsMobilePlatform)
			{
				for (int i = 0; i < Input.touchCount; i++)
				{
					Ray ray = _uiCamera.ScreenPointToRay(Input.touches[i].position);
					RaycastHit[] array = Physics.RaycastAll(ray, 100f, JoystickLayer);
					if (array.Length > 0)
					{
						Vector3 vector = base.transform.parent.InverseTransformPoint(array[0].point);
						return new Vector3(vector.x, vector.y, base.transform.position.z);
					}
				}
				return Vector3.zero;
			}
			Ray ray2 = _uiCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] array2 = Physics.RaycastAll(ray2, float.PositiveInfinity, JoystickLayer.value);
			if (array2.Length > 0)
			{
				Vector3 vector2 = base.transform.parent.InverseTransformPoint(array2[0].point);
				return new Vector3(vector2.x, vector2.y, base.transform.position.z);
			}
			return Vector3.zero;
		}
	}

	private void Start()
	{
		BasePosition = base.transform.localPosition;
		TargetPosition = BasePosition;
		CaclulatadPosision = BasePosition;
		_multiTween.TweenOut();
		_uiCamera = base.transform.root.GetComponentInChildren<Camera>();
		base.gameObject.layer = 30;
	}

	private void Update()
	{
		base.transform.localPosition = Vector3.ClampMagnitude(CaclulatadPosision + (TargetPosition - CaclulatadPosision) * ((!_inDrag) ? (GameTimeController.unscaledDeltaTime * 10f) : 1f), MaxRadius);
		if (_inDrag)
		{
			_joystick.DragProcess(base.transform, InSafeRadius);
		}
	}

	private void OnPress(bool isPressed)
	{
		if (isPressed)
		{
			OnDragStart();
		}
		else
		{
			OnDragEnd();
		}
	}

	private void OnDrag(Vector2 delta)
	{
		Vector3 vector = baseDeltaTouch;
		TargetPosition = ((!(vector == Vector3.zero)) ? vector : TargetPosition);
	}

	private void OnDragStart()
	{
		SphereCollider.radius = 300f;
		TargetPosition = CaclulatadPosision + baseDeltaTouch;
		_inDrag = true;
		_multiTween.TweenIn();
		OnDrag(Vector3.zero);
	}

	private void OnDragEnd()
	{
		_multiTween.TweenOut();
		_joystick.Release();
		TargetPosition = BasePosition;
		_inDrag = false;
		SphereCollider.radius = 200f;
	}
}
