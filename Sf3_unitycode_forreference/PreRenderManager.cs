using System.Collections.Generic;
using UnityEngine;

public class PreRenderManager : MonoBehaviour
{
	public static PreRenderManager Instance;

	private int listPointer;

	private List<RenderTexture> preRenderedTexturesPool;

	private List<RenderTexture> preRenderedTexturesUsed;

	public Shader preRenderShader;

	private void Awake()
	{
		Instance = this;
		preRenderedTexturesPool = new List<RenderTexture>();
		preRenderedTexturesUsed = new List<RenderTexture>();
	}

	public RenderTexture PreRenderTexture(Material mat, string textureName)
	{
		Texture texture = mat.GetTexture(textureName);
		if (texture == null)
		{
			Debug.LogWarning("Missing texture" + textureName + " " + mat.name);
			return null;
		}
		RenderTexture textureFromPool = GetTextureFromPool(texture.width);
		Shader shader = mat.shader;
		mat.shader = preRenderShader;
		Graphics.Blit(texture, textureFromPool, mat);
		mat.shader = shader;
		return textureFromPool;
	}

	private RenderTexture GetTextureFromPool(int size)
	{
		RenderTexture renderTexture = null;
		for (int i = 0; i < preRenderedTexturesPool.Count; i++)
		{
			RenderTexture renderTexture2 = preRenderedTexturesPool[i];
			if (renderTexture2.width == size)
			{
				renderTexture = renderTexture2;
				preRenderedTexturesPool.RemoveAt(i);
				break;
			}
		}
		if (renderTexture == null)
		{
			renderTexture = new RenderTexture(size, size, 0);
			renderTexture.filterMode = FilterMode.Bilinear;
		}
		preRenderedTexturesUsed.Add(renderTexture);
		return renderTexture;
	}

	public void ReturnTexture(RenderTexture texture)
	{
		if (preRenderedTexturesUsed.Contains(texture))
		{
			preRenderedTexturesUsed.Remove(texture);
			preRenderedTexturesPool.Add(texture);
		}
	}
}
