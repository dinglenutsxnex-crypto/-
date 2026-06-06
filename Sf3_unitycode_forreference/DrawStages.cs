using Nekki;
using UnityEngine;

public class DrawStages : MonoBehaviour
{
	public float Radius;

	public int StageNumber;

	public int MaxInRow = 3;

	public Vector2 Indent;

	public bool Draw;

	private void Awake()
	{
		if (!NekkiUtils.IsDebug)
		{
			Object.Destroy(this);
		}
	}

	private void OnDrawGizmos()
	{
		if (!Draw)
		{
			return;
		}
		float num = Mathf.Ceil((float)StageNumber / (float)MaxInRow);
		Vector3 zero = Vector3.zero;
		if (StageNumber > 1)
		{
			if (StageNumber < MaxInRow)
			{
				zero.x = (0f - Indent.x) * ((float)StageNumber / 2f);
			}
			else
			{
				zero.x = (0f - Indent.x) * ((float)MaxInRow / 2f);
			}
			zero.x += Indent.x / 2f;
		}
		if (num > 1f)
		{
			zero.y = (0f - Indent.y) * (num / 2f);
			zero.y += Indent.y / 2f;
		}
		int num2 = 0;
		for (int i = 0; (float)i < num; i++)
		{
			int num3 = StageNumber - num2;
			if (num3 < MaxInRow)
			{
				zero.x = 0f;
				if (num3 > 1)
				{
					if (num3 < MaxInRow)
					{
						zero.x = (0f - Indent.x) * ((float)num3 / 2f);
					}
					zero.x += Indent.x / 2f;
				}
			}
			for (int j = 0; j < MaxInRow; j++)
			{
				Vector3 vector = zero + new Vector3((float)j * Indent.x, (float)i * Indent.y, 0f);
				vector.x *= base.transform.lossyScale.x;
				vector.y *= base.transform.lossyScale.y;
				vector.z *= base.transform.lossyScale.z;
				Gizmos.DrawWireSphere(base.transform.position + vector, Radius);
				num2++;
				if (num2 == StageNumber)
				{
					return;
				}
			}
		}
	}
}
