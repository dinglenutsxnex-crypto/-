using System.Collections.Generic;
using UnityEngine;

public class TiledMap : MonoBehaviour
{
	[SerializeField]
	private string tilePrefix = "map_";

	[SerializeField]
	private int tileWidth = 2048;

	[SerializeField]
	private int tileHeight = 2048;

	[SerializeField]
	private string pathToTile = "UI/Map";

	private UIWidget widget;

	private List<UITexture> tiles;

	private int fillX;

	private int fillY;

	private void Start()
	{
		widget = GetComponent<UIWidget>();
		if (widget == null)
		{
			Debug.LogError("UIWidget not found.");
			return;
		}
		tiles = new List<UITexture>();
		Tiled();
	}

	private void Tiled()
	{
		if (tiles.Count > 0)
		{
			Clear();
		}
		float num = Mathf.Ceil((float)widget.width / (float)tileWidth);
		float num2 = Mathf.Ceil((float)widget.height / (float)tileHeight);
		int num3 = 0;
		Bounds bounds = NGUIMath.CalculateAbsoluteWidgetBounds(base.transform);
		Vector3 vector = base.transform.InverseTransformPoint(bounds.min);
		Vector3 vector2 = base.transform.InverseTransformPoint(bounds.max);
		int num4 = 0;
		int num5 = 0;
		for (int i = 0; (float)i < num2; i++)
		{
			for (int j = 0; (float)j < num; j++)
			{
				num4 = ((fillX + tileWidth > widget.width) ? (widget.width - fillX) : tileWidth);
				num5 = ((fillY + tileHeight > widget.height) ? (widget.height - fillY) : tileHeight);
				GameObject gameObject = CreateTileObject(num3, num4, num5);
				gameObject.transform.localPosition = new Vector3(vector.x + (float)(tileWidth * j), vector2.y + (float)(tileHeight * i));
				num3++;
				fillX += num4;
			}
			fillY += num5;
		}
	}

	private void Clear()
	{
		foreach (UITexture tile in tiles)
		{
			GlobalLoad.Unload(tile.mainTexture);
			Object.Destroy(tile.gameObject);
		}
		fillX = 0;
		fillY = 0;
	}

	private GameObject CreateTileObject(int tileNumber, int width, int height)
	{
		string text = tileNumber.ToString();
		GameObject gameObject = new GameObject("tile_" + text);
		UITexture uITexture = gameObject.AddComponent<UITexture>();
		Texture2D loadTexture2D = GlobalLoad.GetLoadTexture2D(pathToTile + "/" + tilePrefix + text);
		loadTexture2D.wrapMode = TextureWrapMode.Clamp;
		uITexture.mainTexture = loadTexture2D;
		uITexture.pivot = UIWidget.Pivot.TopLeft;
		uITexture.depth = -1000;
		uITexture.width = width;
		uITexture.height = height;
		tiles.Add(uITexture);
		gameObject.transform.parent = base.transform;
		gameObject.transform.localScale = Vector3.one;
		return gameObject;
	}
}
