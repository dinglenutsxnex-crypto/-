using UnityEngine;

public class HorizontalScaleForResolution : MonoBehaviour
{
	private const float DEFAULT_ASPECT = 1.33f;

	public float scaleFactor = 1f;

	private void Awake()
	{
		float num = (float)Screen.width / (float)Screen.height / 1.33f;
		UIWidget component = GetComponent<UIWidget>();
		Vector3 position = base.transform.position;
		position.x = 0f;
		base.transform.position = position;
		if (component != null)
		{
			component.width += Mathf.RoundToInt(scaleFactor * ((float)component.width * num - (float)component.width));
		}
	}
}
