using System;
using UnityEngine;

public class DrawCircle : MonoBehaviour
{
	public float radius;

	public float angle;

	public float steps;

	private Vector3 prevPoint;

	private Vector3 nextPoint;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		prevPoint = Vector3.zero;
		prevPoint.x = Mathf.Cos(0f) * radius * base.transform.localScale.x;
		prevPoint.y = Mathf.Sin(0f) * radius * base.transform.localScale.y;
		prevPoint += base.transform.position;
		for (int i = 1; (float)i <= steps; i++)
		{
			nextPoint = Vector3.zero;
			nextPoint.x = Mathf.Cos(angle * ((float)i / steps) * ((float)Math.PI / 180f)) * radius * base.transform.localScale.x;
			nextPoint.y = Mathf.Sin(angle * ((float)i / steps) * ((float)Math.PI / 180f)) * radius * base.transform.localScale.y;
			nextPoint += base.transform.position;
			Gizmos.DrawLine(prevPoint, nextPoint);
			prevPoint = nextPoint;
		}
	}
}
