using System.Linq;
using SF3;
using UnityEngine;

public class OptimizedGlow : MonoBehaviour
{
	private RenderTexture glowRenderTextureA;

	private RenderTexture glowRenderTextureB;

	private Camera origCamera;

	private Camera shaderCamera;

	private GameObject glowBounds;

	[SerializeField]
	private LayerMask ignorLayersForGlow;

	[SerializeField]
	private LayerMask ignoreLayersForPostEffect;

	[SerializeField]
	private Shader glowReplaceDissolve;

	[SerializeField]
	private Material blurMaterial;

	[SerializeField]
	private Material blend;

	[SerializeField]
	private Shader verticalShader;

	[SerializeField]
	private int factor = 2;

	[SerializeField]
	private float blurSpread = 1.25f;

	public int blurIterations = 1;

	[SerializeField]
	private FireEffect fireEffect;

	public void Awake()
	{
		origCamera = GetComponent<Camera>();
		glowBounds = GlobalLoad.GetPrefabInstanceInternal("models_folder", "glowAdditiveBounds");
		if (glowBounds == null)
		{
			Debug.LogError("could not load Models/glowAdditiveBounds");
			return;
		}
		OnEffectEnable();
		glowBounds.SetActive(false);
		blurMaterial.shader = verticalShader;
	}

	public void Start()
	{
		if (!SystemInfo.supportsImageEffects)
		{
			Debug.Log("Disabling the Glow Effect. Image effects are not supported (do you have Unity Pro?)");
			base.enabled = false;
		}
		blurMaterial.SetPass(0);
	}

	private void OnEnable()
	{
		glowBounds.SetActive(true);
		if (fireEffect != null)
		{
			fireEffect.enabled = true;
		}
	}

	private void OnDisable()
	{
		if (glowBounds != null)
		{
			glowBounds.SetActive(false);
		}
		if (fireEffect != null)
		{
			fireEffect.enabled = false;
		}
	}

	private void OnDestroy()
	{
		Object.Destroy(glowBounds);
		glowRenderTextureA.Release();
		glowRenderTextureB.Release();
		GlobalLoad.Unload(glowRenderTextureA);
		GlobalLoad.Unload(glowRenderTextureB);
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
			shaderCamera = new GameObject("Glow Effect", typeof(Camera)).GetComponent<Camera>();
			shaderCamera.gameObject.SetActive(false);
		}
	}

	public void OnPreRender()
	{
		shaderCamera.CopyFrom(origCamera);
		shaderCamera.backgroundColor = Color.clear;
		shaderCamera.clearFlags = CameraClearFlags.Color;
		shaderCamera.renderingPath = RenderingPath.Forward;
		shaderCamera.targetTexture = glowRenderTextureA;
		shaderCamera.rect = new Rect(0f, 0f, 1f, 1f);
		shaderCamera.cullingMask = ~(int)ignorLayersForGlow;
		shaderCamera.RenderWithShader(glowReplaceDissolve, "RenderType");
		for (int i = 0; i < blurIterations; i++)
		{
			shaderCamera.cullingMask = ~(int)ignoreLayersForPostEffect;
			float num = 1f / (float)glowRenderTextureA.width * blurSpread;
			float num2 = 1f / (float)glowRenderTextureA.height * blurSpread;
			blurMaterial.SetTexture("_MainTexGlobal", glowRenderTextureA);
			blurMaterial.SetVector("_Offset", new Vector4(num, 0f, 2f * num, 0f));
			shaderCamera.targetTexture = glowRenderTextureB;
			shaderCamera.RenderWithShader(verticalShader, null);
			blurMaterial.SetTexture("_MainTexGlobal", glowRenderTextureB);
			blurMaterial.SetVector("_Offset", new Vector4(0f, num2, 0f, 2f * num2));
			shaderCamera.targetTexture = glowRenderTextureA;
			shaderCamera.RenderWithShader(verticalShader, null);
		}
		blend.SetTexture("_AddTex", glowRenderTextureA);
	}

	private void GetGlowBoxBounds()
	{
		if (ModelsManager.Instance.BlurBounds == null)
		{
			return;
		}
		Renderer[] array = ModelsManager.Instance.BlurBounds.Where((Renderer x) => x.gameObject.activeSelf).ToArray();
		if (array.Length == 0)
		{
			return;
		}
		Vector3 min = array[0].bounds.min;
		Vector3 max = array[0].bounds.max;
		Renderer[] array2 = array;
		foreach (Renderer renderer in array2)
		{
			for (int j = 0; j < 3; j++)
			{
				min[j] = Mathf.Min(renderer.bounds.min[j], min[j]);
				max[j] = Mathf.Max(renderer.bounds.max[j], max[j]);
			}
		}
		glowBounds.transform.position = (min + max) / 2f;
		glowBounds.transform.localScale = max - min;
	}

	private void Update()
	{
		GetGlowBoxBounds();
	}
}
