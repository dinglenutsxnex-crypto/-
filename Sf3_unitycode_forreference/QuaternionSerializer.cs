using System;
using UnityEngine;

[Serializable]
public struct QuaternionSerializer
{
	public float x;

	public float y;

	public float z;

	public float w;

	public Vector3 eulerAngles
	{
		get
		{
			return Q.eulerAngles;
		}
	}

	public Quaternion Q
	{
		get
		{
			return new Quaternion(x, y, z, w);
		}
	}

	public QuaternionSerializer(Quaternion q)
	{
		x = q.x;
		y = q.y;
		z = q.z;
		w = q.w;
	}

	public static implicit operator Quaternion(QuaternionSerializer q)
	{
		return q.Q;
	}
}
