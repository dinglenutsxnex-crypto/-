using SF3;
using UnityEngine;

public class MaterialAlphaAnimation : MonoBehaviour
{
	public enum AnimationStyle
	{
		Loop = 0,
		PingPong = 1
	}

	public float animationTime;

	public AnimationCurve animationCurve;

	private float timer;

	public string materialColorProperty;

	private Material material;

	private float alphaValue;

	private Color matColor;

	public AnimationStyle animationStyle;

	private bool backAnimation;

	private void Awake()
	{
		timer = 0f;
		backAnimation = false;
		material = GetComponent<MeshRenderer>().material;
		if (materialColorProperty.Length > 0)
		{
			materialColorProperty = materialColorProperty.Trim();
			if (materialColorProperty[0] != '_')
			{
				materialColorProperty = "_" + materialColorProperty;
			}
		}
	}

	private void Update()
	{
		timer += GameTimeController.deltaTime;
		alphaValue = animationCurve.Evaluate((!backAnimation) ? (timer / animationTime) : (1f - timer / animationTime));
		matColor = material.GetColor(materialColorProperty);
		matColor.a = alphaValue;
		material.SetColor(materialColorProperty, matColor);
		if (timer >= animationTime)
		{
			if (animationStyle == AnimationStyle.PingPong)
			{
				backAnimation = !backAnimation;
			}
			else
			{
				backAnimation = false;
			}
			timer = 0f;
		}
	}
}
