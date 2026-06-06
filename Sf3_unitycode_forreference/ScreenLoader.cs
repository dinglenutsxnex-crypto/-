using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenLoader : MonoBehaviour
{
	private void Start()
	{
		SceneManager.LoadSceneAsync("loadScreen");
	}
}
