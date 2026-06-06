using UnityEngine;

public class BlurPostEffect : MonoBehaviour
{
	public int blurIterations;

	public Material blurMaterial;

	private RenderTexture glowRenderTextureA;

	private RenderTexture glowRenderTextureB;

	private RenderTexture blurTexture;

	public int factor = 2;

	public float blurSpread = 1.25f;

	private bool _blurEnabled;

	private bool requestRenderGlow;

	public static BlurPostEffect Instance;

	public bool blurEnabled
	{
		get
		{
			return _blurEnabled;
		}
		set
		{
			if (!_blurEnabled && value)
			{
				requestRenderGlow = true;
			}
			_blurEnabled = value;
			base.enabled = value;
		}
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		CalculateGlow(source, destination);
	}

	private void Awake()
	{
		Instance = this;
		blurTexture = new RenderTexture(Screen.width, Screen.height, 0);
		blurEnabled = false;
	}

	private void CalculateGlow(RenderTexture source, RenderTexture destination)
	{
		if (requestRenderGlow)
		{
			RenderBlur(source, destination);
			requestRenderGlow = false;
		}
		else
		{
			Graphics.Blit(blurTexture, destination);
		}
	}

	private void RenderBlur(RenderTexture source, RenderTexture destination)
	{
		blurTexture.DiscardContents();
		glowRenderTextureA = RenderTexture.GetTemporary(source.width / factor, source.height / factor);
		glowRenderTextureB = RenderTexture.GetTemporary(source.width / factor, source.height / factor);
		Graphics.Blit(source, glowRenderTextureA);
		for (int i = 0; i < blurIterations; i++)
		{
			blurMaterial.SetVector("_Offset", new Vector4(1f / (float)glowRenderTextureA.height * blurSpread, 0f));
			Graphics.Blit(glowRenderTextureA, glowRenderTextureB, blurMaterial);
			glowRenderTextureA.DiscardContents();
			blurMaterial.SetVector("_Offset", new Vector4(0f, 1f / (float)glowRenderTextureA.width * blurSpread));
			Graphics.Blit(glowRenderTextureB, glowRenderTextureA, blurMaterial);
		}
		RenderTexture.ReleaseTemporary(glowRenderTextureB);
		RenderTexture.ReleaseTemporary(glowRenderTextureA);
		Graphics.Blit(glowRenderTextureA, blurTexture);
	}
}
