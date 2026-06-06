using UnityEngine;

public class SwitchUI : MonoBehaviour
{
	public GameObject UIRoot;

	public GameObject UnityUI;

	private bool unityUIActive;

	private void Awake()
	{
		unityUIActive = true;
		ChangeUI();
	}

	public void ChangeUI()
	{
		unityUIActive = !unityUIActive;
		UIRoot.SetActive(!unityUIActive);
		UnityUI.SetActive(unityUIActive);
	}
}
