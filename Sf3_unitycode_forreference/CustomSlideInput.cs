using UnityEngine;

public class CustomSlideInput : MonoBehaviour
{
	public delegate void SlideInputEvent(GameObject target);

	private Camera _camera;

	public event SlideInputEvent onDown;

	public event SlideInputEvent onPress;

	public event SlideInputEvent onUp;

	private void Start()
	{
		_camera = GetComponent<Camera>();
		if (_camera == null)
		{
			Debug.LogError("Camera not found");
		}
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			CheckCollider(this.onDown);
		}
		if (Input.GetMouseButton(0))
		{
			CheckCollider(this.onPress);
		}
		if (Input.GetMouseButtonUp(0))
		{
			CheckCollider(this.onUp);
		}
	}

	private void CheckCollider(SlideInputEvent action)
	{
		Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo) && action != null && hitInfo.collider != null)
		{
			action(hitInfo.collider.gameObject);
		}
	}
}
