using UnityEngine;

public class SetInfoPanel : MonoBehaviour
{
	public bool active
	{
		get
		{
			return base.gameObject.activeSelf;
		}
	}

	public void DisablePanel()
	{
	}

	public void ActivatePanel()
	{
	}
}
