using UnityEngine;

public class TutorialMaskGenerator : MonoBehaviour
{
	private const int reducingTexture = 4;

	[SerializeField]
	public Canvas canvas;

	[SerializeField]
	public Camera canvasCamera;

	private RenderTexture texture;

	public static TutorialMaskGenerator Instance { get; protected set; }

	public GameObject AddMask(GameObject mask)
	{
		GameObject gameObject = Object.Instantiate(mask);
		gameObject.transform.SetParent(canvas.transform, false);
		return gameObject;
	}

	public RenderTexture GetTexture()
	{
		if (texture == null)
		{
			Init();
		}
		return texture;
	}

	public RectTransform AddMask(GameObject selectMaskPrf, RectTransform rect)
	{
		RectTransform rectTransform = TutorialManager.Instance.TutorialBlockNative.AddMask(selectMaskPrf);
		Vector3 position = NekkiCanvasRoot.instance.GetCanvasCamera().WorldToViewportPoint(rect.position);
		Vector3 position2 = canvasCamera.ViewportToWorldPoint(position);
		float scaleFactor = NekkiUIRoot.Instance.canvas.scaleFactor;
		float scaleFactor2 = canvas.scaleFactor;
		float num = scaleFactor / scaleFactor2;
		position2 /= num;
		rectTransform.position = position2;
		rectTransform.sizeDelta = rect.sizeDelta / 4f;
		return rectTransform;
	}

	public void Enable(bool enable)
	{
		base.gameObject.SetActive(enable);
	}

	private void Awake()
	{
		Init();
		Enable(false);
	}

	private void Init()
	{
		Instance = this;
		CreateRenderTexture();
	}

	private void CreateRenderTexture()
	{
		texture = new RenderTexture(Screen.width / 4, Screen.height / 4, 16);
		texture.Create();
		canvasCamera.targetTexture = texture;
	}
}
