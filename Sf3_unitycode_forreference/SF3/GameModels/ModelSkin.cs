using System;
using System.Collections;
using System.Collections.Generic;
using SF3.Effects;
using UnityEngine;

namespace SF3.GameModels
{
	[Serializable]
	public class ModelSkin : MonoBehaviour
	{
		[SerializeField]
		private string _rootBoneName;

		[SerializeField]
		private string[] _bindBonesName;

		private SkinnedMeshRenderer _skinnedMesh;

		private bool? _mirroringDuplicate;

		private bool isShowCashed;

		private bool isMirroredCashed;

		private bool forceHide;

		public Material shadowFormMaterial;

		private Material dissolveMaterial;

		public Color enemyMaskColor = Color.white;

		public Color playerMaskColor = Color.white;

		[HideInInspector]
		public Material simpleMaterial;

		[HideInInspector]
		public Material matcapMaterial;

		private bool shadowFormMatInited;

		public GradientInfo gradientInfo;

		public bool droppable;

		private bool isTransparent;

		public bool isGradientSource;

		private bool updateGlowMeshConstantly;

		public bool updateGlowBoundsFromBone;

		public Vector3 glowBoudsMeshExpansion = Vector3.one;

		private int defaultMaterialDepth;

		private bool shaderOverrideEnabled;

		private float shaderOverrideTransitionTime;

		public Renderer glowBoundsMesh;

		private RenderTexture mainTexturePreRender;

		private RenderTexture shadowFormTexturePreRender;

		private bool isTrail;

		private Dictionary<string, Material> replacementMaterials;

		private float currentTransitionTime;

		public bool? mirroringDuplicate
		{
			get
			{
				return _mirroringDuplicate;
			}
		}

		public Material defaultMaterial { get; private set; }

		public SkinnedMeshRenderer skinnedMesh
		{
			get
			{
				if (_skinnedMesh == null)
				{
					_skinnedMesh = GetComponent<SkinnedMeshRenderer>();
				}
				return _skinnedMesh;
			}
		}

		public Mesh sharedMesh
		{
			get
			{
				return _skinnedMesh.sharedMesh;
			}
		}

		public bool IsNeedsMirrorSwitch(bool isMirrored)
		{
			bool? flag = _mirroringDuplicate;
			if (flag.HasValue)
			{
				if (isMirrored)
				{
					bool? flag2 = _mirroringDuplicate;
					if (!flag2.GetValueOrDefault() || !flag2.HasValue || _skinnedMesh.enabled)
					{
						bool? flag3 = _mirroringDuplicate;
						if (flag3.GetValueOrDefault() || !flag3.HasValue || !_skinnedMesh.enabled)
						{
							goto IL_00e3;
						}
					}
					return true;
				}
				bool? flag4 = _mirroringDuplicate;
				if (flag4.GetValueOrDefault() || !flag4.HasValue || _skinnedMesh.enabled)
				{
					bool? flag5 = _mirroringDuplicate;
					if (!flag5.GetValueOrDefault() || !flag5.HasValue || !_skinnedMesh.enabled)
					{
						goto IL_00e3;
					}
				}
				return true;
			}
			goto IL_00e3;
			IL_00e3:
			return false;
		}

		public void SetIsMirroringDupcicate(bool? isMirroringDuplicate)
		{
			_mirroringDuplicate = isMirroringDuplicate;
			forceHide = false;
		}

		public void Init()
		{
			_bindBonesName = new string[skinnedMesh.bones.Length];
			int num = 0;
			Transform[] bones = skinnedMesh.bones;
			foreach (Transform transform in bones)
			{
				_bindBonesName[num] = transform.name;
				num++;
			}
			_rootBoneName = skinnedMesh.rootBone.name;
		}

		public IEnumerator PreWarmMaterial()
		{
			if (shadowFormMaterial == null || dissolveMaterial == null || defaultMaterial == null)
			{
				Debug.LogWarning("could not find material");
			}
			Material currMaterial = _skinnedMesh.material;
			_skinnedMesh.sharedMaterial = shadowFormMaterial;
			yield return null;
			_skinnedMesh.sharedMaterial = dissolveMaterial;
			yield return null;
			_skinnedMesh.sharedMaterial = defaultMaterial;
			yield return null;
			_skinnedMesh.sharedMaterial = currMaterial;
		}

		public void BoundToBones(IBonesHolder model)
		{
			_skinnedMesh = GetComponent<SkinnedMeshRenderer>();
			Transform[] array = new Transform[_bindBonesName.Length];
			for (int i = 0; i < array.Length; i++)
			{
				Bone bone = model.GetBone(_bindBonesName[i]);
				if (bone != null)
				{
					array[i] = bone.transform;
				}
				else
				{
					Debug.LogError(_bindBonesName[i] + " NONE");
				}
			}
			_skinnedMesh.bones = array;
			Bone bone2 = model.GetBone(_rootBoneName);
			if (bone2 != null)
			{
				_skinnedMesh.rootBone = bone2.transform;
			}
			InitMaterial(array);
		}

		private void InitMaterial(Transform[] bonesTransforms)
		{
			defaultMaterial = skinnedMesh.material;
			defaultMaterialDepth = ((!MaterialUtility.GetUseZBuffer(defaultMaterial)) ? 2250 : 2000);
			defaultMaterial.renderQueue = defaultMaterialDepth;
			shadowFormMaterial = new Material(shadowFormMaterial);
			shadowFormMaterial.renderQueue = defaultMaterialDepth;
			dissolveMaterial = new Material(shadowFormMaterial);
			dissolveMaterial.EnableKeyword("SF_USE_DISSOLVE");
			dissolveMaterial.name += "_dissolve";
			shadowFormMaterial.DisableKeyword("SF_USE_DISSOLVE");
			if (bonesTransforms.Length >= 1)
			{
				CreateBox(bonesTransforms[0]);
			}
			InitMaskColor();
			isTrail = defaultMaterial.IsKeywordEnabled("USE_PER_VERTEX_ALPHA");
			InitializeReplacementMaterials();
		}

		private void CreateBox(Transform bone)
		{
			if (MaterialUtility.GetRenderType(shadowFormMaterial) != MaterialUtility.RenderType.Glow)
			{
				return;
			}
			GameObject prefabInstanceInternal = GlobalLoad.GetPrefabInstanceInternal("models_folder", "glowBoundsMesh");
			if (prefabInstanceInternal == null)
			{
				Debug.LogError("could not load Models/glowBoundsMesh");
				return;
			}
			glowBoundsMesh = prefabInstanceInternal.GetComponent<Renderer>();
			glowBoundsMesh.name = base.name + "_blur";
			glowBoundsMesh.transform.parent = bone;
			UpdateGlowBounds();
			glowBoundsMesh.gameObject.SetActive(false);
			if (ModelsManager.Instance.BlurBounds == null)
			{
				ModelsManager.Instance.BlurBounds = new List<Renderer>();
			}
			if (base.gameObject.GetComponent<Cloth>() != null)
			{
				updateGlowMeshConstantly = true;
			}
			ModelsManager.Instance.BlurBounds.Add(glowBoundsMesh);
		}

		private void UpdateGlowBounds()
		{
			glowBoundsMesh.transform.localScale = skinnedMesh.localBounds.size * 1.1f;
			glowBoundsMesh.transform.position = skinnedMesh.bounds.center;
			glowBoundsMesh.transform.localRotation = Quaternion.identity;
		}

		public bool RequirePreRender()
		{
			return skinnedMesh.material.IsKeywordEnabled("NEEDS_PRERENDER");
		}

		public void GetPreRenderTextures(out RenderTexture mainTexture, out RenderTexture shadowTexture)
		{
			mainTexture = PreRenderManager.Instance.PreRenderTexture(skinnedMesh.material, "_MainTex");
			shadowFormMaterial.EnableKeyword("PRERENDER_FOR_SF");
			if (shadowFormMaterial.IsKeywordEnabled("USE_B_AS_WEAPON_COLOR") || shadowFormMaterial.IsKeywordEnabled("USE_R_AS_WEAPON_COLOR"))
			{
				shadowTexture = PreRenderManager.Instance.PreRenderTexture(shadowFormMaterial, "_MainTex");
			}
			else
			{
				shadowTexture = PreRenderManager.Instance.PreRenderTexture(shadowFormMaterial, "_ShadowForm");
			}
		}

		public void SetPreRenderTexture(RenderTexture mainTexture, RenderTexture shadowFormTexture)
		{
			mainTexturePreRender = mainTexture;
			shadowFormTexturePreRender = shadowFormTexture;
			skinnedMesh.material.SetTexture("_MainTexPreRender", mainTexturePreRender);
			shadowFormMaterial.SetTexture("_MainTexPreRender", mainTexturePreRender);
			shadowFormMaterial.SetTexture("_ShadowformPreRender", shadowFormTexturePreRender);
		}

		private void ReturnPreRenderTextures()
		{
			if (mainTexturePreRender != null)
			{
				PreRenderManager.Instance.ReturnTexture(mainTexturePreRender);
			}
			if (shadowFormTexturePreRender != null)
			{
				PreRenderManager.Instance.ReturnTexture(shadowFormTexturePreRender);
			}
		}

		private void UpdateGlowBoundsFromBones()
		{
			if (skinnedMesh.bones == null || skinnedMesh.bones.Length == 0)
			{
				return;
			}
			Vector3 position = skinnedMesh.bones[0].transform.position;
			Vector3 vector = position;
			Vector3 vector2 = position;
			Transform[] bones = skinnedMesh.bones;
			foreach (Transform transform in bones)
			{
				for (int j = 0; j < 3; j++)
				{
					float b = transform.transform.position[j];
					vector[j] = Mathf.Min(vector[j], b);
					vector2[j] = Mathf.Max(vector2[j], b);
				}
			}
			glowBoundsMesh.transform.parent = null;
			glowBoundsMesh.transform.position = (vector + vector2) / 2f;
			glowBoundsMesh.transform.localScale = vector2 - vector + glowBoudsMeshExpansion;
			glowBoundsMesh.transform.rotation = Quaternion.identity;
		}

		public void BoundToBones(ModelObject modelobject)
		{
			_skinnedMesh = GetComponent<SkinnedMeshRenderer>();
			Transform[] array = new Transform[_bindBonesName.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = modelobject.GetBone(_bindBonesName[i]).transform;
			}
			_skinnedMesh.bones = array;
			_skinnedMesh.rootBone = modelobject.GetBone(_rootBoneName).transform;
			InitMaterial(array);
		}

		private void InitMaskColor()
		{
			enemyMaskColor = defaultMaterial.GetColor("_MaskEnemyColor");
			playerMaskColor = defaultMaterial.GetColor("_MaskPlayerColor");
			defaultMaterial.SetColor("_MaskColor", playerMaskColor);
			dissolveMaterial.SetColor("_MaskColor", playerMaskColor);
		}

		public void ShowSkin(bool _isShow, bool isMirrored, bool cashing = true)
		{
			if (cashing)
			{
				isShowCashed = _isShow;
				isMirroredCashed = isMirrored;
			}
			bool flag = _isShow ^ forceHide;
			bool? flag2 = _mirroringDuplicate;
			if (!flag2.HasValue || !flag)
			{
				_skinnedMesh.enabled = flag;
			}
			else if (_mirroringDuplicate == true)
			{
				_skinnedMesh.enabled = isMirrored;
			}
			else
			{
				_skinnedMesh.enabled = !isMirrored;
			}
		}

		protected void OnDestroy()
		{
			if (ModelsManager.Instance.BlurBounds != null)
			{
				ModelsManager.Instance.BlurBounds.Remove(glowBoundsMesh);
			}
			ReturnPreRenderTextures();
			GlobalLoad.Unload(defaultMaterial);
			GlobalLoad.Unload(shadowFormMaterial);
			GlobalLoad.Unload(dissolveMaterial);
			if (replacementMaterials == null)
			{
				return;
			}
			foreach (KeyValuePair<string, Material> replacementMaterial in replacementMaterials)
			{
				GlobalLoad.Unload(replacementMaterial.Value);
			}
		}

		private void InitShadowFormMaterial()
		{
			if (!shadowFormMatInited)
			{
				shadowFormMatInited = true;
				shadowFormMaterial.SetColor("_MaskColor", defaultMaterial.GetColor("_MaskColor"));
				shadowFormMaterial.SetTexture("_MatCap", defaultMaterial.GetTexture("_MatCap"));
				shadowFormMaterial.SetTexture("_Mask", defaultMaterial.GetTexture("_Mask"));
			}
		}

		public void SetShadowFormMaterial()
		{
			if (shadowFormMaterial != null)
			{
				if (glowBoundsMesh != null)
				{
					glowBoundsMesh.gameObject.SetActive(true);
				}
				InitShadowFormMaterial();
				shadowFormMaterial.SetFloat("_BlendValue", 0f);
				dissolveMaterial.SetFloat("_BlendValue", 0f);
			}
		}

		public void UpdateShadowFormBlend(float blendProgress)
		{
			if (shaderOverrideEnabled)
			{
				StartCoroutine(DisableOverride());
			}
			if (!(currentTransitionTime > 0f) && dissolveMaterial != null)
			{
				if (QualityManager.Preset.dissolve)
				{
					skinnedMesh.sharedMaterial = dissolveMaterial;
				}
				dissolveMaterial.SetFloat("_DissolveBlendValue", blendProgress);
				if (blendProgress <= 0f)
				{
					ReturnDefaultMaterial();
				}
			}
		}

		public void DisableDissolve()
		{
			_skinnedMesh.sharedMaterial = shadowFormMaterial;
		}

		public void ReturnDefaultMaterial()
		{
			if (_skinnedMesh != null)
			{
				skinnedMesh.sharedMaterial = defaultMaterial;
			}
			if (glowBoundsMesh != null)
			{
				glowBoundsMesh.gameObject.SetActive(false);
			}
		}

		public void GetMatcapTexture()
		{
			Texture matcapTexture = LocationColorAnimation.GetMatcapTexture();
			if (matcapTexture != null)
			{
				defaultMaterial.SetTexture("_MatCap", matcapTexture);
				dissolveMaterial.SetTexture("_MatCap", matcapTexture);
			}
		}

		public void ApplyMaskColor()
		{
			defaultMaterial.SetColor("_MaskColor", enemyMaskColor);
			dissolveMaterial.SetColor("_MaskColor", enemyMaskColor);
		}

		public void SetMainColor(Color color, bool useAlpha = true)
		{
			_skinnedMesh.sharedMaterial.SetColor("_MainColor", color);
		}

		public void SetOpaque()
		{
			Material sharedMaterial = _skinnedMesh.sharedMaterial;
			if (!isTrail)
			{
				if (isTransparent)
				{
					sharedMaterial.renderQueue = defaultMaterialDepth;
					MaterialUtility.SetMaterialBlend(sharedMaterial, MaterialUtility.MaterialBlendType.None);
				}
				isTransparent = false;
			}
		}

		public void SetDissolveWeaponMaterial()
		{
			isTransparent = false;
			SetTransparent(1f);
			Material sharedMaterial = _skinnedMesh.sharedMaterial;
			sharedMaterial.EnableKeyword("SF_DISSOLVE_WEAPON");
		}

		public void DissolveWeaponMeshrendererOff()
		{
			forceHide = true;
			ShowSkin(isShowCashed, isMirroredCashed, false);
			Material sharedMaterial = _skinnedMesh.sharedMaterial;
			sharedMaterial.DisableKeyword("SF_DISSOLVE_WEAPON");
		}

		public void DissolveWeaponMeshrendererOn()
		{
			Material sharedMaterial = _skinnedMesh.sharedMaterial;
			forceHide = false;
			ShowSkin(isShowCashed, isMirroredCashed, false);
			sharedMaterial.EnableKeyword("SF_DISSOLVE_WEAPON");
		}

		public void DissolveWeaponMaterialOff()
		{
			forceHide = false;
			ShowSkin(isShowCashed, isMirroredCashed, false);
			Material sharedMaterial = _skinnedMesh.sharedMaterial;
			sharedMaterial.DisableKeyword("SF_DISSOLVE_WEAPON");
			SetOpaque();
		}

		public void SetTransparent(float value)
		{
			Material sharedMaterial = _skinnedMesh.sharedMaterial;
			if (!isTransparent)
			{
				sharedMaterial.renderQueue = 4850;
				MaterialUtility.SetMaterialBlend(sharedMaterial, MaterialUtility.MaterialBlendType.AlphaBlending);
			}
			isTransparent = true;
			skinnedMesh.sharedMaterial.SetFloat("_Alpha", value);
		}

		public void SetSkinColor(Color color)
		{
			Material sharedMaterial = _skinnedMesh.sharedMaterial;
			sharedMaterial.SetColor("_SkinColor", color);
			shadowFormMaterial.SetColor("_SkinColor", color);
			Color color2 = color - sharedMaterial.GetColor("_SkinColorBase");
			sharedMaterial.SetVector("_SkinColorDifference", color2);
			shadowFormMaterial.SetVector("_SkinColorDifference", color2);
			dissolveMaterial.SetVector("_SkinColorDifference", color2);
		}

		public void SwitchMaterial(bool toSimple)
		{
			Material material = ((!toSimple) ? matcapMaterial : simpleMaterial);
			if (material != null)
			{
				skinnedMesh.material = material;
				defaultMaterial = skinnedMesh.material;
			}
		}

		public void EnableGradient()
		{
			skinnedMesh.sharedMaterial.EnableKeyword("USE_GRADIENT");
			skinnedMesh.sharedMaterial.SetColor("_GradientStartColor", gradientInfo.gradientStartColor);
			skinnedMesh.sharedMaterial.SetColor("_GradientEndColor", gradientInfo.gradientEndColor);
			skinnedMesh.sharedMaterial.SetFloat("_GradientStart", gradientInfo.gradientStart);
			skinnedMesh.sharedMaterial.SetFloat("_GradientEnd", gradientInfo.gradientEnd);
		}

		public void DisableGradient()
		{
			skinnedMesh.sharedMaterial.SetColor("_GradientStartColor", Color.white);
			skinnedMesh.sharedMaterial.SetColor("_GradientEndColor", Color.white);
			dissolveMaterial.SetColor("_GradientStartColor", Color.white);
			dissolveMaterial.SetColor("_GradientEndColor", Color.white);
		}

		private void Update()
		{
			if (updateGlowMeshConstantly)
			{
				UpdateGlowBounds();
			}
			else if (updateGlowBoundsFromBone)
			{
				UpdateGlowBoundsFromBones();
			}
		}

		public void OverrideMaterial(string materialName, bool value, float transitionTime)
		{
			shaderOverrideTransitionTime = transitionTime;
			if (replacementMaterials.ContainsKey(materialName))
			{
				if (!value)
				{
					StartCoroutine(DisableOverride());
				}
				else
				{
					StartCoroutine(EnableOverride(materialName));
				}
			}
		}

		private void SetOverrideShaderTransition(string materialName, float time)
		{
			skinnedMesh.sharedMaterial.SetFloat("_EffectTransition", time / shaderOverrideTransitionTime);
		}

		private IEnumerator EnableOverride(string materialName)
		{
			skinnedMesh.sharedMaterial = replacementMaterials[materialName];
			if (!shaderOverrideEnabled && shaderOverrideTransitionTime > 0f)
			{
				for (currentTransitionTime = 0f; currentTransitionTime < shaderOverrideTransitionTime; currentTransitionTime += Time.deltaTime)
				{
					SetOverrideShaderTransition(materialName, currentTransitionTime);
					yield return null;
				}
			}
			shaderOverrideEnabled = true;
			yield return null;
		}

		private IEnumerator DisableOverride()
		{
			if (shaderOverrideEnabled && shaderOverrideTransitionTime > 0f)
			{
				for (currentTransitionTime = shaderOverrideTransitionTime; currentTransitionTime > 0f; currentTransitionTime -= Time.deltaTime)
				{
					SetOverrideShaderTransition("_EffectTransition", currentTransitionTime);
					yield return null;
				}
			}
			shaderOverrideEnabled = false;
			skinnedMesh.sharedMaterial = shadowFormMaterial;
			yield return null;
		}

		private void InitializeReplacementMaterials()
		{
			replacementMaterials = new Dictionary<string, Material>();
			MaterialUtility.CharacterSkinType @int = (MaterialUtility.CharacterSkinType)defaultMaterial.GetInt("_MaterialType");
			foreach (EffectsManager.ShaderReplacementEffect shaderReplacementEffect in EffectsManager.Instance.shaderReplacementEffects)
			{
				if (shaderReplacementEffect.types.Contains(@int))
				{
					AddReplacementMaterial(shaderReplacementEffect.material, shaderReplacementEffect.name);
				}
			}
		}

		private void AddReplacementMaterial(Material mat, string name)
		{
			Material material = new Material(mat);
			Texture texture = shadowFormMaterial.GetTexture("_ShadowForm");
			material.SetTexture("_ShadowForm", texture);
			replacementMaterials.Add(name, material);
		}
	}
}
