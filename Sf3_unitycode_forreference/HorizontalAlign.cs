using System.Collections.Generic;
using UnityEngine;

public class HorizontalAlign : MonoBehaviour
{
	public enum Anchor
	{
		center = 0,
		right = 1
	}

	public List<Transform> objects;

	public Anchor anchor;

	public float offsetFactor = 1f;

	private const float DEFAULT_ASPECT = 1.33f;

	private void Awake()
	{
		float num = (float)Screen.width / (float)Screen.height / 1.33f;
		if (objects == null || objects.Count == 0)
		{
			ReadTransforms();
		}
		for (int i = 0; i < objects.Count; i++)
		{
			Vector3 localPosition = objects[i].transform.localPosition;
			if (anchor == Anchor.center)
			{
				localPosition.x *= num;
			}
			else if (anchor == Anchor.right)
			{
				localPosition.x += (localPosition.x - localPosition.x * num) * offsetFactor;
			}
			objects[i].transform.localPosition = localPosition;
		}
	}

	private void ReadTransforms()
	{
		objects = new List<Transform>();
		for (int i = 0; i < base.transform.childCount; i++)
		{
			if (base.transform.GetChild(i) != base.transform)
			{
				objects.Add(base.transform.GetChild(i));
			}
		}
	}
}
