using UnityEngine;
using UnityEngine.UI;

public class OffsetFromLabelUI : MonoBehaviour
{
	public UnityEngine.UI.Text label;

	public float offsetX;

	private float savedWidth;

	private Transform transf;

	private void Awake()
	{
		transf = base.transform;
		SetOffset();
	}

	private void SetOffset()
	{
		savedWidth = label.preferredWidth;
		Vector3 localPosition = label.transform.localPosition;
		localPosition.x -= label.preferredWidth + offsetX;
		transf.localPosition = localPosition;
	}

	private void Update()
	{
		if (savedWidth != label.preferredWidth)
		{
			SetOffset();
		}
	}
}
