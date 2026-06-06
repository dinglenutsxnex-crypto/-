using System;
using UnityEngine;

namespace SF3
{
	public class LocationQualitySettings : MonoBehaviour
	{
		[Serializable]
		public class ToggleQualLvl
		{
			public string quality;

			public bool enabled;
		}

		[Serializable]
		public class ParticlesQualLvl
		{
			public string quality;

			public float maxParticlesPercent;
		}

		[Serializable]
		public class ToggleObjSettingsLvls
		{
			public GameObject gameObject;

			public ToggleQualLvl[] settingsLevels;

			public void ApplyQuality()
			{
				ToggleQualLvl[] array = settingsLevels;
				foreach (ToggleQualLvl toggleQualLvl in array)
				{
					if (toggleQualLvl.quality.ToLower() == QualityManager.PresetName)
					{
						gameObject.SetActive(toggleQualLvl.enabled);
						return;
					}
				}
				gameObject.SetActive(true);
			}
		}

		[Serializable]
		public class ParticlesSettingsLvls
		{
			public ParticleSystem particleSystem;

			public ParticlesQualLvl[] settingsLevels;

			private bool _defaultState;

			private int _defaultMaxParticles;

			public void Init()
			{
				_defaultState = particleSystem.gameObject.activeSelf;
				_defaultMaxParticles = particleSystem.main.maxParticles;
			}

			public void ApplyQuality()
			{
				ParticlesQualLvl[] array = settingsLevels;
				foreach (ParticlesQualLvl particlesQualLvl in array)
				{
					if (!(particlesQualLvl.quality.ToLower() == QualityManager.PresetName))
					{
						continue;
					}
					if (particlesQualLvl.maxParticlesPercent > 0f)
					{
						if (!particleSystem.gameObject.activeSelf && _defaultState)
						{
							particleSystem.gameObject.SetActive(true);
						}
						SetMaxParticles(particleSystem, Mathf.RoundToInt((float)_defaultMaxParticles * particlesQualLvl.maxParticlesPercent));
					}
					else
					{
						particleSystem.gameObject.SetActive(false);
					}
				}
				particleSystem.gameObject.SetActive(_defaultState);
				SetMaxParticles(particleSystem, _defaultMaxParticles);
			}

			private void SetMaxParticles(ParticleSystem system, int count)
			{
				ParticleSystem.MainModule main = system.main;
				main.maxParticles = count;
			}
		}

		public Cloth[] clothComponents;

		public ToggleObjSettingsLvls[] gameObjectQualityLvls;

		public ParticlesSettingsLvls[] particlesQualityLvls;

		public Material[] preRenderMaterials;

		private RenderTexture prerenderTexture;

		private void Start()
		{
			if (particlesQualityLvls != null)
			{
				ParticlesSettingsLvls[] array = particlesQualityLvls;
				foreach (ParticlesSettingsLvls particlesSettingsLvls in array)
				{
					particlesSettingsLvls.Init();
				}
			}
			OnQualityLvlChange();
			PreRenderMainTexture();
		}

		private void PreRenderMainTexture()
		{
			if (preRenderMaterials.Length != 0 && (QualityManager.Preset.dissolve || QualityManager.Preset.saturation))
			{
				Texture texture = preRenderMaterials[0].GetTexture("_MainTex");
				prerenderTexture = new RenderTexture(texture.width, texture.height, 0);
				prerenderTexture.filterMode = FilterMode.Bilinear;
				prerenderTexture.useMipMap = true;
				prerenderTexture.autoGenerateMips = false;
				prerenderTexture.Create();
				Material mat = new Material(Shader.Find("Hidden/ColorCorrection"));
				Graphics.Blit(texture, prerenderTexture, mat);
				prerenderTexture.GenerateMips();
				Material[] array = preRenderMaterials;
				foreach (Material material in array)
				{
					material.EnableKeyword("PRE_RENDER");
					material.SetTexture("_MainTexPreRender", prerenderTexture);
				}
			}
		}

		private void OnDestroy()
		{
			if (prerenderTexture != null)
			{
				GlobalLoad.Unload(prerenderTexture);
			}
		}

		private void OnQualityLvlChange()
		{
			if (clothComponents != null)
			{
				Cloth[] array = clothComponents;
				foreach (Cloth cloth in array)
				{
					cloth.enabled = QualityManager.Preset.cloth;
				}
			}
			if (gameObjectQualityLvls != null)
			{
				ToggleObjSettingsLvls[] array2 = gameObjectQualityLvls;
				foreach (ToggleObjSettingsLvls toggleObjSettingsLvls in array2)
				{
					toggleObjSettingsLvls.ApplyQuality();
				}
			}
			if (particlesQualityLvls != null)
			{
				ParticlesSettingsLvls[] array3 = particlesQualityLvls;
				foreach (ParticlesSettingsLvls particlesSettingsLvls in array3)
				{
					particlesSettingsLvls.ApplyQuality();
				}
			}
		}
	}
}
