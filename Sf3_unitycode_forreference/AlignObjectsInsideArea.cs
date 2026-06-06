using UnityEngine;

public class AlignObjectsInsideArea : MonoBehaviour
{
	public Transform[] objectsToAlign;

	public float areaWidth;

	public float scaleFactor = 1f;

	private void Start()
	{
		float num = (float)Screen.width / (float)Screen.height / 1.33f;
		Vector3 localPosition = base.transform.localPosition;
		float num2 = scaleFactor * (areaWidth * num - areaWidth) / 2f;
		Transform[] array = objectsToAlign;
		foreach (Transform transform in array)
		{
			localPosition = transform.localPosition;
			localPosition.x += (transform.localPosition.x * num - transform.localPosition.x) * scaleFactor;
			localPosition.x -= num2;
			transform.localPosition = localPosition;
		}
	}
}
