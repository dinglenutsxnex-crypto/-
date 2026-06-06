using UnityEngine;

public class FireEffect : MonoBehaviour
{
	private RenderTexture glowRenderTextureA;

	private RenderTexture glowRenderTextureB;

	[SerializeField]
	private int factor = 4;

	[SerializeField]
	private float erosionSpread = 2f;

	[SerializeField]
	private LayerMask fireEffectLayer;

	private Camera origCamera;

	private Camera shaderCamera;

	[SerializeField]
	private Shader fireBoundsShader;

	[SerializeField]
	private Material outputMat;

	[SerializeField]
	private Shader erosionShader;

	[SerializeField]
	private LayerMask fireLayer;

	private bool isInit;

	private void Awake()
	{
		origCamera = GetComponent<Camera>();
		OnEffectEnable();
	}

	private void OnEnable()
	{
	}

	public void OnEffectEnable()
	{
		glowRenderTextureA = new RenderTexture(origCamera.pixelWidth / factor, origCamera.pixelHeight / factor, 16, RenderTextureFormat.ARGB32);
		glowRenderTextureA.wrapMode = TextureWrapMode.Clamp;
		glowRenderTextureA.useMipMap = false;
		glowRenderTextureA.filterMode = FilterMode.Bilinear;
		glowRenderTextureA.Create();
		glowRenderTextureB = new RenderTexture(origCamera.pixelWidth / factor, origCamera.pixelHeight / factor, 16, RenderTextureFormat.ARGB32);
		glowRenderTextureB.wrapMode = TextureWrapMode.Clamp;
		glowRenderTextureB.useMipMap = false;
		glowRenderTextureB.filterMode = FilterMode.Bilinear;
		glowRenderTextureB.Create();
		if (shaderCamera == null)
		{
			shaderCamera = new GameObject("Shadow Fire Effect", typeof(Camera)).GetComponent<Camera>();
			shaderCamera.gameObject.SetActive(false);
		}
	}

	public void OnPreRender()
	{
		shaderCamera.CopyFrom(origCamera);
		shaderCamera.backgroundColor = Color.clear;
		shaderCamera.clearFlags = CameraClearFlags.Depth;
		shaderCamera.renderingPath = RenderingPath.Forward;
		glowRenderTextureA.DiscardContents();
		shaderCamera.targetTexture = glowRenderTextureA;
		shaderCamera.rect = new Rect(0f, 0f, 1f, 1f);
		shaderCamera.cullingMask = fireEffectLayer;
		shaderCamera.RenderWithShader(fireBoundsShader, "ShadowFire");
		RenderWithShader(erosionShader, erosionSpread, fireLayer);
		outputMat.SetTexture("_Alpha", glowRenderTextureA);
	}

	private void RenderWithShader(Shader shader, float spread, LayerMask mask)
	{
		float num = 1f / (float)glowRenderTextureA.width;
		float num2 = 1f / (float)glowRenderTextureA.height;
		float num3 = num * spread;
		float num4 = num2 * spread;
		outputMat.SetVector("_HalfPixelOffset", new Vector4(num * 0.5f, (0f - num2) * 0.5f, 0f, 0f));
		outputMat.SetTexture("_MainTexGlobal", glowRenderTextureA);
		outputMat.SetVector("_Offset", new Vector4(num3, 0f, 2f * num3, 0f));
		glowRenderTextureB.DiscardContents();
		shaderCamera.cullingMask = mask;
		shaderCamera.targetTexture = glowRenderTextureB;
		shaderCamera.RenderWithShader(shader, null);
		outputMat.SetTexture("_MainTexGlobal", glowRenderTextureB);
		outputMat.SetVector("_Offset", new Vector4(0f, num4, 0f, 2f * num4));
		glowRenderTextureA.DiscardContents();
		shaderCamera.cullingMask = mask;
		shaderCamera.targetTexture = glowRenderTextureA;
		shaderCamera.RenderWithShader(shader, null);
	}

	private void OnDestroy()
	{
		glowRenderTextureA.Release();
		glowRenderTextureB.Release();
		GlobalLoad.Unload(glowRenderTextureA);
		GlobalLoad.Unload(glowRenderTextureB);
	}
}
