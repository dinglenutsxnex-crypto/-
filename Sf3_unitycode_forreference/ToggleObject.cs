using UnityEngine;

public class ToggleObject : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.SetActive(false);
		base.gameObject.SetActive(true);
	}
}
