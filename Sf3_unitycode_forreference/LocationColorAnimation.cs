using System;
using System.Linq;
using DynamicShadowProjector;
using SF3;
using UnityEngine;
using UnityEngine.Rendering;

public class LocationColorAnimation : MonoBehaviour
{
	private enum GradientOrientation
	{
		Horizontal = 0,
		Vertical = 1
	}

	public Texture MatcapTexture;

	public Vector3 ProjectorPosition = new Vector3(-59.13672f, 47.67383f, 23.17383f);

	public Vector3 ProjectorRotation = new Vector3(33f, 53f, -7f);

	public Color ShadowColor = new Color(0f, 0f, 0f, 1f);

	public int MipMap = 4;

	public int BlurLevel;

	private float _timer;

	private float _animationTime;

	private bool _animateColor;

	private Color _from;

	private Color _to;

	public SpriteAlphaAnimation[] rays;

	private bool _animateShadowForm;

	private bool _enableShadowForm;

	private float _shadowFormTimer;

	private bool _animateDissolveWeapon;

	private bool _enablingDissolveWeapon;

	private const float dissolveWeaponTime = 0.45f;

	private float _dissolveWeaponTimer = 0.45f;

	private Action _dissolveWeaponOnFinishCallback;

	private ColorCorrectionEffect _vignette;

	public Light directionalLight;

	[Space(70f)]
	[Header("Basic Settings", order = 1)]
	[Space(10f)]
	public Color ambientColor = Color.white;

	public float ambientIntensity = 1f;

	[Space(20f)]
	public Color directionalLightColor = Color.white;

	public float directionalLightIntensity = 1f;

	[Space(20f)]
	public bool fogOnLocation = true;

	public Color fogColor;

	public float defaultFogStart;

	public float defaultFogEnd;

	[Space(70f)]
	[Header("Shadow Form Settings", order = 1)]
	[Space(10f)]
	public Color shadowAmbientColor = Color.white;

	public float shadowAmbientIntensity = 1f;

	[Space(20f)]
	public Color shadowDirectionalLightColor = Color.white;

	public float shadowDirectionalLightIntensity = 1f;

	[Space(20f)]
	public bool fogInShadowForm = true;

	public Color shadowFogColor;

	public float shadowFormFogStart;

	public float shadowFormFogEnd;

	[Space(20f)]
	public float vignetteIntensityInSF = 0.35f;

	[Space(20f)]
	public float shadowFormEnterTime;

	public AnimationCurve shadowEffectCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[Space(20f)]
	public GameObject[] disableInShadowForm;

	public GameObject[] activateInShadowForm;

	[Space(20f)]
	[SerializeField]
	private Gradient rimColorGradient;

	[SerializeField]
	private GradientOrientation gradientColorOrientation;

	[SerializeField]
	private float rimIntensity;

	private Renderer[] _renderers;

	public Material[] defaultMaterials;

	public Material[] dissolveMaterials;

	public static LocationColorAnimation Instance { get; private set; }

	private void CollectRenderers()
	{
		_renderers = base.transform.GetComponentsInChildren<Renderer>();
		defaultMaterials = _renderers.Select((Renderer x) => x.material).ToArray();
		dissolveMaterials = defaultMaterials.Select(delegate(Material x)
		{
			Material material = new Material(x);
			material.EnableKeyword("SF_USE_DISSOLVE");
			return material;
		}).ToArray();
	}

	private void Awake()
	{
		CollectRenderers();
		Instance = this;
		ApplyFogSettings();
		SetRimColorGradient();
		directionalLight.color = directionalLightColor;
		directionalLight.intensity = directionalLightIntensity;
		RenderSettings.ambientMode = AmbientMode.Flat;
		RenderSettings.ambientIntensity = ambientIntensity;
		RenderSettings.ambientLight = ambientColor;
		_vignette = UnityEngine.Object.FindObjectOfType<ColorCorrectionEffect>();
		_vignette.VignetteEnabled = false;
		GameObject[] array = activateInShadowForm;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
		SetDissolveBlend(0f);
		SetLocationColor(Color.white);
	}

	public void ToggleVignette(bool enabled)
	{
		if (!enabled && _vignette.VignetteEnabled)
		{
			_vignette.VignetteEnabled = false;
		}
	}

	private void ApplyFogSettings()
	{
		RenderSettings.fogColor = fogColor;
		RenderSettings.fogStartDistance = defaultFogStart;
		RenderSettings.fogEndDistance = defaultFogEnd;
		RenderSettings.fog = fogOnLocation;
	}

	public void SetShadow(ShadowTextureRenderer r)
	{
		r.transform.localPosition = ProjectorPosition;
		r.transform.localEulerAngles = ProjectorRotation;
		r.mipLevel = MipMap;
		r.shadowColor = ShadowColor;
	}

	public static Texture GetMatcapTexture()
	{
		if (Instance != null)
		{
			return Instance.MatcapTexture;
		}
		return null;
	}

	[ContextMenu("Set Rim Color Gradient")]
	private void SetRimColorGradient()
	{
		GradientColorKey[] colorKeys = rimColorGradient.colorKeys;
		if (colorKeys.Length != 2)
		{
			Debug.LogWarning("Incorrect number of gradient color keys. Must be 2");
			return;
		}
		Color color = colorKeys[0].color;
		color.a = colorKeys[0].time;
		Color color2 = colorKeys[1].color;
		color2.a = colorKeys[1].time;
		Shader.SetGlobalColor("_RimColorA", color);
		Shader.SetGlobalColor("_RimColorB", color2);
		SetGradientAlpha(1f);
		Vector2 vector = default(Vector2);
		vector.x = ((gradientColorOrientation != 0) ? 0f : 1f);
		vector.y = ((gradientColorOrientation != GradientOrientation.Vertical) ? 0f : 1f);
		Shader.SetGlobalVector("_RimGradientMask", vector);
		Shader.SetGlobalFloat("_RimPower", rimIntensity);
	}

	private void SetGradientAlpha(float multiplier)
	{
		GradientAlphaKey[] alphaKeys = rimColorGradient.alphaKeys;
		if (alphaKeys.Length != 2)
		{
			Debug.LogWarning("Incorrect number of gradient alpha keys. Must be  2");
			return;
		}
		Vector2 vector = default(Vector2);
		vector.x = alphaKeys[0].alpha * multiplier;
		vector.y = alphaKeys[0].time;
		Vector2 vector2 = default(Vector2);
		vector2.x = alphaKeys[1].alpha * multiplier;
		vector2.y = alphaKeys[1].time;
		Shader.SetGlobalVector("_RimAlphaA", vector);
		Shader.SetGlobalVector("_RimAlphaB", vector2);
	}

	public void EnableRays(bool enable, float time)
	{
		if (rays == null || rays.Length <= 0)
		{
			return;
		}
		SpriteAlphaAnimation[] array = rays;
		foreach (SpriteAlphaAnimation spriteAlphaAnimation in array)
		{
			if (enable)
			{
				spriteAlphaAnimation.Activate(time);
			}
			else
			{
				spriteAlphaAnimation.Disable(time);
			}
		}
	}

	public void Animate(float time, Color _to)
	{
		Animate(time, _from, _to);
	}

	public void Animate(float time, Color _from, Color _to)
	{
		_timer = 0f;
		_animationTime = time;
		_animateColor = true;
		this._from = _from;
		this._to = _to;
	}

	private void EnableDissolve()
	{
		if (QualityManager.Preset.dissolve)
		{
			for (int i = 0; i < _renderers.Length; i++)
			{
				_renderers[i].material = dissolveMaterials[i];
			}
		}
	}

	private void DisableDissolve()
	{
		if (QualityManager.Preset.dissolve)
		{
			for (int i = 0; i < _renderers.Length; i++)
			{
				_renderers[i].material = defaultMaterials[i];
			}
		}
	}

	public void ShadowForm(bool enable, bool instant = false)
	{
		GameObject[] array = activateInShadowForm;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(enable);
		}
		GameObject[] array2 = disableInShadowForm;
		foreach (GameObject gameObject2 in array2)
		{
			gameObject2.SetActive(!enable);
		}
		if (instant)
		{
			_enableShadowForm = enable;
			_animateShadowForm = false;
			SetShaderParams(enable ? 1 : 0);
			FinishAnimation();
			return;
		}
		_animateShadowForm = _enableShadowForm != enable;
		if (_animateShadowForm)
		{
			EnableDissolve();
		}
		_enableShadowForm = enable;
		_shadowFormTimer = 0f;
		if (_enableShadowForm)
		{
			_vignette.SetIntensity(0f);
			_vignette.VignetteEnabled = true;
			RenderSettings.fog = fogInShadowForm;
			Shader.EnableKeyword("SF_GLOBAL");
		}
	}

	private void SetShaderParams(float progress)
	{
		_vignette.SetIntensity(Mathf.Lerp(0f, vignetteIntensityInSF, progress));
		RenderSettings.fogColor = Color.Lerp(fogColor, shadowFogColor, progress);
		RenderSettings.fogStartDistance = Mathf.Lerp(defaultFogStart, shadowFormFogStart, progress);
		RenderSettings.fogEndDistance = Mathf.Lerp(defaultFogEnd, shadowFormFogEnd, progress);
		RenderSettings.ambientIntensity = Mathf.Lerp(ambientIntensity, shadowAmbientIntensity, progress);
		RenderSettings.ambientLight = Color.Lerp(ambientColor, shadowAmbientColor, progress);
		directionalLight.color = Color.Lerp(directionalLightColor, shadowDirectionalLightColor, progress);
		directionalLight.intensity = Mathf.Lerp(directionalLightIntensity, shadowDirectionalLightIntensity, progress);
		SetGradientAlpha(1f - progress);
	}

	private void FinishAnimation()
	{
		_animateShadowForm = false;
		DisableDissolve();
		if (!_enableShadowForm)
		{
			Shader.DisableKeyword("SF_GLOBAL");
			_vignette.VignetteEnabled = false;
			RenderSettings.fog = fogOnLocation;
		}
	}

	public void SetDissolveWeaponIn(Action onFinishCallback)
	{
		_enablingDissolveWeapon = true;
		_animateDissolveWeapon = true;
		_dissolveWeaponOnFinishCallback = onFinishCallback;
	}

	public void SetDissolveWeaponOut(Action onFinishCallback)
	{
		_enablingDissolveWeapon = false;
		_animateDissolveWeapon = true;
		_dissolveWeaponOnFinishCallback = onFinishCallback;
	}

	private void Update()
	{
		if (_animateShadowForm)
		{
			_shadowFormTimer += GameTimeController.deltaTime;
			float num = _shadowFormTimer / shadowFormEnterTime;
			if (!_enableShadowForm)
			{
				num = 1f - num;
			}
			num = shadowEffectCurve.Evaluate(num);
			SetDissolveBlend(num);
			SetShaderParams(num);
			if (_shadowFormTimer >= shadowFormEnterTime)
			{
				FinishAnimation();
			}
		}
		if (_animateColor)
		{
			_timer += GameTimeController.deltaTime;
			Color locationColor = Color.Lerp(_from, _to, _timer / _animationTime);
			SetLocationColor(locationColor);
			if (_timer >= _animationTime)
			{
				_animateColor = false;
			}
		}
		if (!_animateDissolveWeapon)
		{
			return;
		}
		_dissolveWeaponTimer -= GameTimeController.deltaTime;
		float num2 = _dissolveWeaponTimer / 0.45f;
		float time = ((!_enablingDissolveWeapon) ? (1f - num2) : num2);
		time = shadowEffectCurve.Evaluate(time);
		if (num2 <= 0f)
		{
			_dissolveWeaponTimer = 0.45f;
			time = ((!_enablingDissolveWeapon) ? 1f : 0f);
			_animateDissolveWeapon = false;
			if (_enableShadowForm || !_enablingDissolveWeapon)
			{
				Action dissolveWeaponOnFinishCallback = _dissolveWeaponOnFinishCallback;
				_dissolveWeaponOnFinishCallback = null;
				dissolveWeaponOnFinishCallback.InvokeSafe();
			}
		}
		SetDissolveWeaponBlend(time);
	}

	private static void SetLocationColor(Color value)
	{
		Shader.SetGlobalColor("_MainLocationColor", value);
	}

	private static void SetDissolveBlend(float value)
	{
		Shader.SetGlobalFloat("_DissolveBlendValue", value);
	}

	private static void SetDissolveWeaponBlend(float value)
	{
		Shader.SetGlobalFloat("_DissolveWeaponBlendValue", value);
	}
}
