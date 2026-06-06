using UnityEngine;

[ExecuteInEditMode]
public class HelloWorld : MonoBehaviour
{
	public GUIStyle style;

	public GUIStyle style2;

	private void Start()
	{
	}

	private void OnGUI()
	{
		GUI.Label(new Rect((float)Screen.width / 2f - 150f, (float)Screen.height / 2f - 25f, 300f, 50f), "В поисках концепции...", style);
		GUI.Label(new Rect((float)Screen.width / 2f - 150f, (float)Screen.height / 2f + 75f, 300f, 50f), DebugUI.FullInfo, style2);
	}
}
