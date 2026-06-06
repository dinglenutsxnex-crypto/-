using System;
using SF3;
using UnityEngine;

public class ColorCorrectionEffect : MonoBehaviour
{
	public Material colorCorrectionMaterial;

	public Material depthOnlyMaterial;

	private Texture _placeholder;

	public float mipmapBias = -0.45f;

	public float saturation = 1.2f;

	[SerializeField]
	private bool EditGlobalShaderVariables;

	[SerializeField]
	private Texture dissolveMask;

	private bool _needUpdate;

	private Camera _camera;

	public ColorCorrectionEffect instance;

	private LayerMask _previousCameraLayer;

	private bool _mainCameraEnabledPrevious;

	public float _Gamma = 1f;

	public bool VignetteEnabled { get; set; }

	public void SetIntensity(float value)
	{
		colorCorrectionMaterial.SetFloat("_Intensity", value);
	}

	public void SetSaturation(float val)
	{
		saturation = val;
		_needUpdate = true;
	}

	private void Awake()
	{
		VignetteEnabled = false;
		_placeholder = new Texture2D(1, 1);
		SetMipmapBias();
		if (dissolveMask == null)
		{
			Debug.LogError("Dissolve mask wasn't assigned for BattleCamera. Location shadowform transition won't work as intended");
		}
		Shader.SetGlobalTexture("_DissolveMask", dissolveMask);
		if (QualityManager.Preset.gamma)
		{
			EnableGamma();
		}
		else
		{
			DisableGamma();
		}
		instance = this;
		_camera = GetComponent<Camera>();
		_mainCameraEnabledPrevious = true;
		Input.gyro.enabled = false;
		SetQualityLevel();
		QualityManager qualityManager = QualityManager.Instance;
		qualityManager.onQualityLvlChange = (Action)Delegate.Combine(qualityManager.onQualityLvlChange, new Action(SetQualityLevel));
	}

	private void OnDestroy()
	{
		QualityManager qualityManager = QualityManager.Instance;
		qualityManager.onQualityLvlChange = (Action)Delegate.Remove(qualityManager.onQualityLvlChange, new Action(SetQualityLevel));
	}

	private static void SetQualityLevel()
	{
		if (QualityManager.Preset.fpsCap > 0)
		{
			Application.targetFrameRate = QualityManager.Preset.fpsCap;
		}
		switch (QualityManager.Preset.name)
		{
		case "low":
			Shader.EnableKeyword("LOW_Q");
			break;
		case "medium":
			Shader.EnableKeyword("MED_Q");
			break;
		case "high":
			Shader.EnableKeyword("HIGH_Q");
			break;
		case "ultra":
			Shader.EnableKeyword("ULTRA_Q");
			break;
		}
	}

	private void EnableGamma()
	{
		Shader.EnableKeyword("USE_SATURATE");
	}

	private void DisableGamma()
	{
		Shader.DisableKeyword("USE_SATURATE");
	}

	private void SetMipmapBias()
	{
		Shader.SetGlobalFloat("_MipMapBias", mipmapBias);
		Shader.SetGlobalFloat("_Saturation", saturation);
	}

	private void EnableSaturation()
	{
		Shader.EnableKeyword("USE_SATURATE");
	}

	private void DisableSaturation()
	{
		Shader.DisableKeyword("USE_SATURATE");
	}

	public void EnableMatcap()
	{
		if (QualityManager.Preset.matcap)
		{
			Shader.EnableKeyword("USE_MATCAP_GLOBAL");
		}
	}

	public void DisableMatcap()
	{
		Shader.DisableKeyword("USE_MATCAP_GLOBAL");
	}

	private void EnableMainCameraRendering()
	{
		_camera.cullingMask = _previousCameraLayer;
	}

	private void DisableMainCameraRendering()
	{
		_previousCameraLayer = Camera.main.cullingMask;
		_camera.cullingMask = 0;
	}

	private void OnPreRender()
	{
		if (!(ScreenTexture.Instance == null))
		{
			if (ScreenTexture.Instance.MainCameraEnabled && !_mainCameraEnabledPrevious)
			{
				_mainCameraEnabledPrevious = true;
				EnableMainCameraRendering();
			}
			else if (!ScreenTexture.Instance.MainCameraEnabled && _mainCameraEnabledPrevious)
			{
				_mainCameraEnabledPrevious = false;
				DisableMainCameraRendering();
			}
			if (!ScreenTexture.Instance.MainCameraEnabled)
			{
				Graphics.Blit((Texture)ScreenTexture.Instance.renderTexture, (RenderTexture)null);
			}
		}
	}

	private void OnPostRender()
	{
		if (VignetteEnabled)
		{
			Graphics.Blit(_placeholder, colorCorrectionMaterial);
		}
	}

	private void Update()
	{
		if (EditGlobalShaderVariables || _needUpdate)
		{
			SetMipmapBias();
			_needUpdate = false;
		}
		Shader.SetGlobalFloat("_Gamma", 1f / _Gamma);
	}
}
