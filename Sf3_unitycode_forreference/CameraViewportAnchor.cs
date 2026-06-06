using UnityEngine;

public class CameraViewportAnchor : MonoBehaviour
{
	private const float DEFAULT_WIDTH = 1.33f;

	public float offsetByAspect;

	public float width = 500f;

	public float scaleFactor = 1f;

	public float temporaryFix = 1f;

	private Camera cam;

	private void Awake()
	{
		cam = GetComponent<Camera>();
		if (cam != null)
		{
			Rect rect = cam.rect;
			float num = (float)Screen.width / (float)Screen.height / 1.33f;
			float num2 = offsetByAspect * (num - 1f);
			rect.x *= num;
			rect.x += num2;
			cam.rect = rect;
		}
	}
}
