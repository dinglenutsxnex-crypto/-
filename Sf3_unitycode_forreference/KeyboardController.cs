using UnityEngine;

public class KeyboardController : AbstractController
{
	private static KeyboardController _instance;

	public static KeyboardController Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = new GameObject("keyboardController").AddComponent<KeyboardController>();
			}
			return _instance;
		}
	}

	private void Awake()
	{
		_instance = this;
	}

	private void Update()
	{
		if (!SystemProperties.IsMobilePlatform)
		{
			TrackKeys();
		}
	}

	public static void SetActive(bool value)
	{
		if ((bool)_instance)
		{
			_instance.gameObject.SetActive(value);
		}
	}
}
