using Nekki;
using Nekki.Yaml;
using UnityEngine;

public class ImageConfig
{
	public string Sprite { get; set; }

	public string Texture { get; set; }

	public string Atlas { get; set; }

	public float ScaleX { get; protected set; }

	public float ScaleY { get; protected set; }

	public float OffsetX { get; protected set; }

	public float OffsetY { get; protected set; }

	public Color Color { get; protected set; }

	public bool Empty
	{
		get
		{
			return string.IsNullOrEmpty(Sprite) || string.IsNullOrEmpty(Atlas);
		}
	}

	public ImageConfig()
	{
		Sprite = string.Empty;
		Atlas = string.Empty;
		ScaleX = 1f;
		ScaleY = 1f;
		OffsetX = 0f;
		OffsetY = 0f;
		Color = Color.white;
	}

	public ImageConfig(string texture)
		: this()
	{
		Texture = texture;
		SetSpriteAndAtlas(texture);
	}

	public ImageConfig(Mapping config)
		: this()
	{
		if (config == null)
		{
			return;
		}
		foreach (Node item in config.nodesInside)
		{
			switch (item.key)
			{
			case "Sprite":
				Texture = item.value.ToString();
				SetSpriteAndAtlas(Texture);
				break;
			case "ScaleX":
				ScaleX = float.Parse(item.value.ToString());
				break;
			case "ScaleY":
				ScaleY = float.Parse(item.value.ToString());
				break;
			case "OffsetX":
				OffsetX = float.Parse(item.value.ToString());
				break;
			case "OffsetY":
				OffsetY = float.Parse(item.value.ToString());
				break;
			case "Color":
				Color = NekkiUtils.HexToColor(item.value.ToString());
				break;
			}
		}
	}

	protected static float ParseFloat(string value, float defValue)
	{
		float result;
		if (!float.TryParse(value, out result))
		{
			return defValue;
		}
		return result;
	}

	private void SetSpriteAndAtlas(string texture)
	{
		string[] array = texture.Split('/');
		Sprite = array[1];
		Atlas = array[0];
	}

	public override string ToString()
	{
		return (!Empty) ? ("{" + string.Format("Sprite = {0}, Atlas = {1}, Scale = ({2}:{3}), Offset = ({4},{5}), Color = {6}", Sprite, Atlas, ScaleX, ScaleY, OffsetX, OffsetY, Color) + "}") : "{EMPTY}";
	}
}
