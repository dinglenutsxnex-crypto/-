using UnityEngine;

public class ActionButton : MonoBehaviour
{
	public delegate void ActionButtonPressed(bool isPressed);

	public event ActionButtonPressed Pressed;

	private void OnPress(bool isPressed)
	{
		if (this.Pressed != null)
		{
			this.Pressed(isPressed);
		}
	}
}
