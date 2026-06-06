using UnityEngine;

public class TrailAnimatedTexture : MonoBehaviour
{
	public int colCount = 2;

	public int rowCount = 6;

	public int rowNumber;

	public int colNumber;

	public int totalCells = 12;

	public int fps = 16;

	public bool loop;

	public int tile = 1;

	private float StartTime;

	private Material material;

	private void Start()
	{
		material = GetComponent<TrailRenderer>().material;
		StartTime = Time.time;
	}

	private void Update()
	{
		SetSpriteAnimation(colCount, rowCount, rowNumber, colNumber, totalCells, fps, loop, tile);
	}

	private void SetSpriteAnimation(int colCount, int rowCount, int rowNumber, int colNumber, int totalCells, int fps, bool loop, int tile)
	{
		if (!loop)
		{
			float num = Time.time - StartTime;
			int num2 = (int)(num * (float)fps);
			if (num2 < totalCells)
			{
				num2 %= totalCells;
				float x = 1f / (float)colCount;
				float y = 1f / (float)rowCount;
				Vector2 scale = new Vector2(x, y);
				Vector2 offset = new Vector2((float)(num2 % colCount + colNumber) * scale.x, 1f - scale.y - (float)(num2 / colCount + rowNumber) * scale.y);
				material.SetTextureScale("_MainTex", scale);
				material.SetTextureOffset("_MainTex", offset);
			}
		}
		else
		{
			float num3 = Time.time - StartTime;
			int num4 = (int)(num3 * (float)fps);
			num4 %= totalCells;
			float x2 = 1f / (float)colCount;
			float y2 = 1f / (float)rowCount;
			Vector2 scale2 = new Vector2(x2, y2);
			Vector2 offset2 = new Vector2((float)(num4 % colCount + colNumber) * scale2.x, 1f - scale2.y - (float)(num4 / colCount + rowNumber) * scale2.y);
			material.SetTextureScale("_MainTex", scale2);
			material.SetTextureOffset("_MainTex", offset2);
		}
	}
}
