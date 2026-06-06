using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SimpeFPS : MonoBehaviour
{
	public float refreshRate = 0.5f;

	public UnityEngine.UI.Text text;

	private int counter;

	private float timer;

	public Color Good;

	public Color Norm;

	public Color Low;

	private void Awake()
	{
		if (text == null)
		{
			text = GetComponent<UnityEngine.UI.Text>();
		}
	}

	private IEnumerator DeleteNGUI()
	{
		yield return new WaitForSeconds(2f);
		UIWidget[] widjet = Object.FindObjectsOfType<UIWidget>();
		for (int i = 0; i < widjet.Length; i++)
		{
			Object.Destroy(widjet[i]);
		}
		UIRoot root = Object.FindObjectOfType<UIRoot>();
		Object.Destroy(root);
		UIPanel[] panels = Object.FindObjectsOfType<UIPanel>();
		for (int j = 0; j < panels.Length; j++)
		{
			Object.Destroy(panels[j]);
		}
	}

	private void Update()
	{
		if (text == null)
		{
			return;
		}
		timer += Time.deltaTime;
		counter++;
		if (timer >= refreshRate)
		{
			float num = (float)counter / refreshRate;
			if (num > 50f)
			{
				text.color = Good;
			}
			else if (num > 20f)
			{
				text.color = Norm;
			}
			else
			{
				text.color = Low;
			}
			text.text = "FPS: " + num.ToString("00.0") + string.Empty;
			timer = 0f;
			counter = 0;
		}
	}
}
