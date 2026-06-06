using System.Linq;
using UnityEngine;

public class CheckJoystick : MonoBehaviour
{
	[SerializeField]
	private UISprite sprite;

	private void Start()
	{
	}

	private void Update()
	{
		if ((bool)sprite)
		{
			sprite.alpha = (Input.GetJoystickNames().ToList().Exists((string n) => n != string.Empty) ? 1 : 0);
		}
	}
}
