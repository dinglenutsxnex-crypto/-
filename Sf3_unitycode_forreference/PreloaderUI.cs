using UnityEngine;

public class PreloaderUI : MonoBehaviour
{
	private static PreloaderUI instance;

	private Canvas canvas;

	public static void ShowPreloader(bool show)
	{
		if (instance != null)
		{
			instance.ShowCanvas(show);
		}
	}

	private void Awake()
	{
		instance = this;
		canvas = GetComponent<Canvas>();
	}

	public void ShowCanvas(bool show)
	{
		if (canvas != null)
		{
			canvas.gameObject.SetActive(show);
		}
	}

	private void OnDestroy()
	{
		instance = null;
	}
}
