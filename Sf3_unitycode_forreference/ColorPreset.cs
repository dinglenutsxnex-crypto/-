using System;
using UnityEngine;

[Serializable]
public class ColorPreset
{
	public int ID;

	public Color color;

	public int minSaturation;

	public int maxSaturation = 255;

	public Color GetColor(float value)
	{
		float H;
		float V;
		float S;
		Color.RGBToHSV(color, out H, out S, out V);
		S = (value * (float)(maxSaturation - minSaturation) + (float)minSaturation) / 255f;
		return Color.HSVToRGB(H, S, V);
	}
}
