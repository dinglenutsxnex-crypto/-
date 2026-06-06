using UnityEngine;

public class ResoluitonFix : MonoBehaviour
{
	public float defAspectRatio = 1.5f;

	public bool y;

	public bool x;

	private void Start()
	{
		float num = (float)Screen.width / (float)Screen.height;
		float num2 = num / defAspectRatio;
		Vector3 localScale = base.transform.localScale;
		if (y)
		{
			localScale.y = num2;
		}
		if (x)
		{
			localScale.x = num2;
		}
		base.transform.localScale = localScale;
	}
}
