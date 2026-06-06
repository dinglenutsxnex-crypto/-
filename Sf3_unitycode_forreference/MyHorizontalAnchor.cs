using UnityEngine;

public class MyHorizontalAnchor : MonoBehaviour
{
	public enum AnchorSide
	{
		left = 0,
		right = 1,
		center = 2
	}

	public AnchorSide anchor;

	public float worldUnitsOffset;

	public Camera uiCam;

	private bool searchingForCamera;

	public bool useAspectRatio;

	public float scaleByAspect;

	private void Start()
	{
		searchingForCamera = true;
		SetPosition();
	}

	private void OnEnable()
	{
		SetPosition();
	}

	private void SetPosition()
	{
		if (NekkiUIRoot.Camera != null)
		{
			uiCam = NekkiUIRoot.Camera;
		}
		if (!uiCam && UICamera.currentCamera != null)
		{
			uiCam = UICamera.currentCamera;
		}
		if (uiCam != null)
		{
			searchingForCamera = false;
			Vector3 position = uiCam.transform.position;
			Vector2 vector = new Vector2(1f, uiCam.WorldToViewportPoint(base.transform.position).y);
			if (anchor == AnchorSide.left)
			{
				vector.x = 0f;
			}
			else if (anchor == AnchorSide.center)
			{
				vector.x = 0.5f;
			}
			Vector2 zero = Vector2.zero;
			if (useAspectRatio)
			{
				float num = (float)Screen.width / (float)Screen.height / 1.33f - 1f;
				float num2 = worldUnitsOffset + scaleByAspect * num;
				zero = (Vector2)uiCam.WorldToViewportPoint(new Vector3(position.x + num2, 0f, 0f)) - new Vector2(0.5f, 0.5f);
			}
			else
			{
				zero = (Vector2)uiCam.WorldToViewportPoint(new Vector3(position.x + worldUnitsOffset, 0f, 0f)) - new Vector2(0.5f, 0.5f);
			}
			vector.x += zero.x;
			base.transform.position = uiCam.ViewportToWorldPoint(vector);
		}
	}

	private void Update()
	{
		if (searchingForCamera)
		{
			SetPosition();
		}
	}
}
