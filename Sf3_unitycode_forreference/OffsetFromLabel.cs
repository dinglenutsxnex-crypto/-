using UnityEngine;

public class OffsetFromLabel : MonoBehaviour
{
	public UILabel label;

	public float offsetX;

	public float offsetY;

	private int savedWidth;

	private Transform transf;

	public bool alignToStartPos;

	public float startXPos;

	private void Awake()
	{
		transf = base.transform;
		SetOffset();
		if (alignToStartPos)
		{
			startXPos *= (float)Screen.width / (float)Screen.height / 1.33f;
			AlightToStartPos();
		}
	}

	private void SetOffset()
	{
		savedWidth = label.width;
		Vector3 localPosition = label.transform.localPosition;
		localPosition.x -= (float)label.width + offsetX;
		localPosition.y += offsetY;
		transf.localPosition = localPosition;
	}

	private void AlightToStartPos()
	{
		if (alignToStartPos)
		{
			float num = label.transform.localPosition.x - transf.localPosition.x;
			Vector3 localPosition = label.transform.localPosition;
			localPosition.x = startXPos + num / 2f;
			label.transform.localPosition = localPosition;
			SetOffset();
		}
	}

	private void Update()
	{
		if (savedWidth != label.width)
		{
			SetOffset();
			AlightToStartPos();
		}
	}
}
