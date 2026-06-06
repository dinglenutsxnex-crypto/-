using System;
using System.Collections.Generic;
using System.Xml;
using SF3.Effects;
using SF3.Items;
using SF3.Moves;
using SF3.Settings;
using UnityEngine;
using sf3DTO;

namespace SF3.GameModels
{
	[Serializable]
	public class ModelObject : IBonesHolder
	{
		private XmlElement _modelObjectNode;

		private List<ModelSkin> gradientSkins;

		public GradientInfo gradientInfo;

		private RenderTexture _mainTexturePreRender;

		private RenderTexture _shadowFormPreRender;

		private static readonly string[] FOLDER_SEPARATOR;

		private const string pathGlobal = "models_folder";

		private static HashSet<string> avaibleTypeFolderName;

		public List<SkeletonObject> skeletons { get; private set; }

		public List<ModelSkin> skins { get; private set; }

		public Bone[] bones { get; private set; }

		public string name { get; private set; }

		public bool droppable { get; private set; }

		public ModelMaterials modelMaterials { get; private set; }

		public Equipment equipment { get; private set; }

		public ModelObject(Gender genderValue)
		{
			name = string.Empty;
			skeletons = new List<SkeletonObject>();
			skins = new List<ModelSkin>();
			bones = new Bone[0];
			modelMaterials = new ModelMaterials();
		}

		static ModelObject()
		{
			FOLDER_SEPARATOR = new string[1] { "__" };
			avaibleTypeFolderName = new HashSet<string>(new string[7] { "arm", "helm", "wpn", "rng", "head", "hair", "mgc" });
		}

		public void SetModelsRagdollSleepState(bool isSleep, int priority)
		{
			foreach (SkeletonObject skeleton in skeletons)
			{
				skeleton.skeletonRagdoll.SetRagdollSleepState(isSleep, priority);
			}
		}

		public void LoadModel(Equipment equipmentModel, Transform parent, Gender genderValue, bool destroyPreviousModel = true)
		{
			equipment = equipmentModel;
			if (!string.IsNullOrEmpty(equipmentModel.model))
			{
				LoadModel(equipment.model, parent, genderValue, destroyPreviousModel);
			}
		}

		public virtual void LoadModel(string modelName, Transform parent, Gender genderValue, bool destroyPreviousModel = true)
		{
			if (destroyPreviousModel)
			{
				DestroyItem();
			}
			name = modelName;
			droppable = false;
			skeletons = new List<SkeletonObject>();
			bones = new Bone[0];
			skins = new List<ModelSkin>();
			string text = name;
			if ((equipment != null && (equipment.GetEquipmentType() == EquipmentType.Armor || equipment.GetEquipmentType() == EquipmentType.Helmet)) || name.ToUpper().Contains("HEAD"))
			{
				switch (genderValue)
				{
				case Gender.Male:
					text += "_m";
					break;
				case Gender.Female:
					text += "_f";
					break;
				}
			}
			_modelObjectNode = LoadModelConfig(text);
			if (_modelObjectNode != null)
			{
				CreateModelBones(parent, genderValue);
				XmlElement xmlElement = (XmlElement)_modelObjectNode.SelectSingleNode("Materials");
				if (xmlElement != null)
				{
					modelMaterials.Parse(xmlElement);
				}
				return;
			}
			throw new Exception(string.Format("Cant load \"{0}\" modelObject config ^^", text));
		}

		public void SetEquipment(Equipment itemEquipment)
		{
			equipment = itemEquipment;
		}

		public int GetBonesCount()
		{
			return bones.Length;
		}

		public int[] GetBonesIDs()
		{
			int[] array = new int[bones.Length];
			for (int i = 0; i < bones.Length; i++)
			{
				array[i] = bones[i].boneID;
			}
			return array;
		}

		public void FillAnimatedTransforms(ref ModelAnimation.BlendAnimatedTransforms result)
		{
			for (int i = 0; i < bones.Length; i++)
			{
				if (bones[i].boneID != -1)
				{
					result.animatedTransforms[bones[i].boneID].animateThisFrame = true;
					result.animatedTransforms[bones[i].boneID].SetPosition(bones[i].localPosition);
					result.animatedTransforms[bones[i].boneID].SetRotation(bones[i].localRotation);
				}
			}
		}

		public void UpdateBonesPositions(Dictionary<int, AnimatedTransform> bonesAnimationTransforms)
		{
		}

		public void DestroyItem()
		{
			if (skins != null && skins.Count > 0)
			{
				for (int i = 0; i < skins.Count; i++)
				{
					if (skins[i] != null)
					{
						UnityEngine.Object.DestroyImmediate(skins[i].gameObject);
					}
				}
			}
			if (skeletons == null || skeletons.Count <= 0)
			{
				return;
			}
			for (int j = 0; j < skeletons.Count; j++)
			{
				if (skeletons[j] != null)
				{
					UnityEngine.Object.DestroyImmediate(skeletons[j].gameObject);
				}
			}
		}

		private void CreateModelBones(Transform parent, Gender genderValue)
		{
			XmlElement xmlElement = (XmlElement)_modelObjectNode.SelectSingleNode("Prefabs");
			if (xmlElement == null)
			{
				return;
			}
			XmlNodeList xmlNodeList = xmlElement.SelectNodes("Prefab");
			List<Bone> list = new List<Bone>();
			foreach (XmlElement item in xmlNodeList)
			{
				string attribute = item.GetAttribute("Name");
				string attribute2 = item.GetAttribute("ConfigName");
				string attribute3 = item.GetAttribute("MirroringDuplicate");
				SkeletonObject skeletonObject;
				if (attribute.Length == 0)
				{
					if (attribute2.Length == 0)
					{
						throw new Exception(string.Format("Cant create \"{0}\" skeleton ^^", name));
					}
					GameObject gameObject = new GameObject(attribute2);
					skeletonObject = gameObject.AddComponent<SkeletonObject>();
					gameObject.transform.parent = parent;
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localRotation = Quaternion.identity;
					gameObject.transform.localScale = Vector3.one;
				}
				else
				{
					GameObject gameObject = LoadBonesPrefab(attribute);
					gameObject.name = gameObject.name.Replace("Models^Bones^Prefabs^", string.Empty);
					skeletonObject = gameObject.GetComponent<SkeletonObject>();
					XmlAttribute attributeNode = item.GetAttributeNode("Droppable");
					skeletonObject.droppable = attributeNode != null && attributeNode.Value.Equals("1");
					droppable = droppable || skeletonObject.droppable;
					Vector3 localPosition = gameObject.transform.localPosition;
					Quaternion localRotation = gameObject.transform.localRotation;
					Vector3 localScale = gameObject.transform.localScale;
					gameObject.transform.parent = parent;
					gameObject.transform.localPosition = localPosition;
					gameObject.transform.localRotation = localRotation;
					gameObject.transform.localScale = localScale;
				}
				XmlDocument xmlDocument = null;
				xmlDocument = LoadBonesConfig(attribute2);
				XmlElement bonesDataNode = null;
				if (xmlDocument != null)
				{
					bonesDataNode = xmlDocument.DocumentElement;
				}
				skeletonObject.CreateSkeleton(skeletonObject.transform, bonesDataNode, attribute3);
				if (skeletonObject.bones.Length > 0)
				{
					list.AddRange(skeletonObject.bones);
				}
				skeletons.Add(skeletonObject);
			}
			bones = list.ToArray();
		}

		public void Init(IBonesHolder modelState, Transform parent)
		{
			foreach (SkeletonObject skeleton in skeletons)
			{
				skeleton.Init(modelState);
			}
			CreateModelSkins(parent, modelState);
			_modelObjectNode = null;
		}

		private void CreateModelSkins(Transform parent, IBonesHolder modelState = null)
		{
			if (_modelObjectNode == null)
			{
				return;
			}
			skins = new List<ModelSkin>();
			gradientSkins = new List<ModelSkin>();
			XmlNodeList xmlNodeList;
			if (modelState == null)
			{
				XmlElement xmlElement = (XmlElement)_modelObjectNode.SelectSingleNode("Prefabs");
				if (xmlElement == null)
				{
					return;
				}
				xmlNodeList = xmlElement.SelectNodes("Mesh");
				if (xmlNodeList.Count == 0)
				{
					return;
				}
			}
			else
			{
				XmlElement xmlElement2 = (XmlElement)_modelObjectNode.SelectSingleNode("Meshes");
				if (xmlElement2 == null)
				{
					return;
				}
				xmlNodeList = xmlElement2.SelectNodes("Mesh");
			}
			if (xmlNodeList.Count == 0)
			{
				return;
			}
			gradientInfo = null;
			foreach (XmlElement item in xmlNodeList)
			{
				XmlAttribute attributeNode = item.GetAttributeNode("Name");
				if (attributeNode == null)
				{
					throw new Exception(string.Format("Object [{0}] mesh name field cant be empty", _modelObjectNode.GetAttribute("Name")));
				}
				string value = attributeNode.Value;
				GameObject gameObject = LoadSkinPrefab(value);
				if (gameObject == null)
				{
					throw new Exception(string.Format("Cant load skin object mesh with name [{0}]", value));
				}
				gameObject.layer = LayerMask.NameToLayer("CharacterAndFoe");
				ModelSkin componentInChildren = gameObject.GetComponentInChildren<ModelSkin>();
				attributeNode = item.GetAttributeNode("MirroringDuplicate");
				if (attributeNode != null)
				{
					componentInChildren.SetIsMirroringDupcicate(attributeNode.Value.Equals("1") ? true : false);
				}
				skins.Add(componentInChildren);
				if (value.Contains("head"))
				{
					gradientSkins.Add(componentInChildren);
				}
				if (componentInChildren.isGradientSource)
				{
					gradientInfo = componentInChildren.gradientInfo;
				}
				componentInChildren.name = componentInChildren.name.Replace("(Clone)", string.Empty);
				componentInChildren.transform.parent = parent;
				if (modelState == null || modelState.GetBonesCount() == 0)
				{
					if (bones.Length > 0)
					{
						componentInChildren.BoundToBones(this);
					}
					else
					{
						Debug.LogError(string.Format("Cant bound \"{0}\" skin, model havnt bones ^^", name));
					}
				}
				else
				{
					componentInChildren.BoundToBones(modelState);
				}
				XmlAttribute attributeNode2 = item.GetAttributeNode("Droppable");
				componentInChildren.droppable = attributeNode2 != null && (attributeNode2.Value.Equals("1") ? true : false);
				droppable = droppable || componentInChildren.droppable;
			}
			InitMatcap();
		}

		public void SetGradient(GradientInfo gradientInfo)
		{
			foreach (ModelSkin gradientSkin in gradientSkins)
			{
				gradientSkin.gradientInfo = gradientInfo;
				gradientSkin.EnableGradient();
			}
		}

		public void ResetBonesPosition()
		{
			Bone[] array = bones;
			foreach (Bone bone in array)
			{
				if (bone.boneID != -1 && !bone.pseudoPhysics)
				{
					bone.ResetPosition();
				}
			}
		}

		public void ShowSkins(bool isShow, bool isMirrored)
		{
			foreach (ModelSkin skin in skins)
			{
				skin.ShowSkin(isShow, isMirrored);
			}
		}

		public InteractiveModelObject DropModelObject(Vector3 attackImpulse, SFState sfState, float? forceMultiplier = 1f, bool freeze = false)
		{
			InteractiveModelObject interactiveModelObject = new GameObject().AddComponent<InteractiveModelObject>();
			interactiveModelObject.gameObject.name = "Interactive_" + name;
			ResetBonesPosition();
			Transform transform = interactiveModelObject.transform;
			FightSettings.DisarmTrajectory disarmTrajectory = (FightSettings.DisarmTrajectory)FightSettings.GetParamsByName("Disarm");
			Vector3 vector = Vector3.Lerp(disarmTrajectory.directionMin, disarmTrajectory.directionMax, UnityEngine.Random.value);
			vector.x *= Mathf.Sign(attackImpulse.x);
			Vector3 vector2 = Vector3.Lerp(disarmTrajectory.torqueMin, disarmTrajectory.torqueMax, UnityEngine.Random.value);
			float num = UnityEngine.Random.Range(disarmTrajectory.impulseMin, disarmTrajectory.impulseMax);
			int num2 = 0;
			List<SkeletonObject> list = new List<SkeletonObject>();
			foreach (SkeletonObject skeleton in skeletons)
			{
				if (skeleton.droppable)
				{
					skeleton.bones[0].transform.parent = transform;
					if (!freeze)
					{
						float num3 = (forceMultiplier.HasValue ? forceMultiplier.Value : 1f);
						if (!EffectsManager.freezeFrameActive)
						{
							skeleton.skeletonRagdoll.AddForce(num3 * vector * num);
							skeleton.skeletonRagdoll.AddTorque(num3 * vector2);
						}
						skeleton.skeletonRagdoll.AddForceAwake(num3 * vector * num);
						skeleton.skeletonRagdoll.AddTorqueAwake(num3 * vector2);
					}
					else
					{
						foreach (SkeletonObject skeleton2 in skeletons)
						{
							skeleton2.skeletonRagdoll.SetActive(false);
							skeleton2.skeletonRagdoll.FreezeObject(true);
						}
					}
					if (skins[num2].GetComponent<SkinnedMeshRenderer>().enabled)
					{
						skeleton.OnCollision += interactiveModelObject.OnCollision;
					}
					list.Add(skeleton);
				}
				num2++;
			}
			IgnoreColliders(list);
			foreach (ModelSkin skin in skins)
			{
				if (skin.droppable)
				{
					skin.transform.parent = transform;
				}
			}
			switch (sfState)
			{
			case SFState.InDisabling:
				interactiveModelObject.DisableShadowForm();
				break;
			case SFState.InActivating:
				interactiveModelObject.ActivateShadowForm();
				break;
			}
			interactiveModelObject.SetModelObject(this, sfState == SFState.Activated || sfState == SFState.InActivating);
			return interactiveModelObject;
		}

		public void SetMainColor(UnityEngine.Color color, bool useAlpha = true)
		{
			foreach (ModelSkin skin in skins)
			{
				skin.SetMainColor(color, useAlpha);
			}
		}

		public void SetOpaque()
		{
			foreach (ModelSkin skin in skins)
			{
				skin.SetOpaque();
			}
		}

		public void SetDissolveWeaponMaterial()
		{
			foreach (ModelSkin skin in skins)
			{
				skin.SetDissolveWeaponMaterial();
			}
		}

		public void DissolveWeaponMeshrendererOff()
		{
			foreach (ModelSkin skin in skins)
			{
				skin.DissolveWeaponMeshrendererOff();
			}
		}

		public void DissolveWeaponMeshrendererOn()
		{
			foreach (ModelSkin skin in skins)
			{
				skin.DissolveWeaponMeshrendererOn();
			}
		}

		public void DissolveWeaponMaterialOff()
		{
			foreach (ModelSkin skin in skins)
			{
				skin.DissolveWeaponMaterialOff();
			}
		}

		public void SetTransparent(float value)
		{
			foreach (ModelSkin skin in skins)
			{
				skin.SetTransparent(value);
			}
		}

		public void SetSkinColor(UnityEngine.Color color)
		{
			foreach (ModelSkin skin in skins)
			{
				skin.SetSkinColor(color);
			}
		}

		public void InitMatcap()
		{
			foreach (ModelSkin skin in skins)
			{
				skin.GetMatcapTexture();
			}
		}

		public void ApplyMaskColor()
		{
			foreach (ModelSkin skin in skins)
			{
				skin.ApplyMaskColor();
			}
		}

		public void DisableGradient()
		{
			foreach (ModelSkin skin in skins)
			{
				skin.DisableGradient();
			}
		}

		public void UpdateShadowFormBlend(float progress)
		{
			foreach (ModelSkin skin in skins)
			{
				skin.UpdateShadowFormBlend(progress);
			}
		}

		public void ChangeToNonDissolveShadowForm()
		{
			foreach (ModelSkin skin in skins)
			{
				skin.DisableDissolve();
			}
		}

		public void PreRenderTexture()
		{
			if (skins.Count == 0)
			{
				return;
			}
			ModelSkin modelSkin = skins[0];
			if (!modelSkin.RequirePreRender())
			{
				return;
			}
			if (_mainTexturePreRender == null || _shadowFormPreRender == null)
			{
				modelSkin.GetPreRenderTextures(out _mainTexturePreRender, out _shadowFormPreRender);
			}
			foreach (ModelSkin skin in skins)
			{
				skin.SetPreRenderTexture(_mainTexturePreRender, _shadowFormPreRender);
			}
		}

		public void ActivateShadowFormEffect()
		{
			foreach (ModelSkin skin in skins)
			{
				skin.SetShadowFormMaterial();
			}
		}

		public void ReturnDefaultMaterial()
		{
			foreach (ModelSkin skin in skins)
			{
				skin.ReturnDefaultMaterial();
			}
		}

		public Bone GetBone(string nameBone)
		{
			Bone[] array = bones;
			foreach (Bone bone in array)
			{
				if (bone.boneName.Equals(nameBone))
				{
					return bone;
				}
			}
			Debug.LogError(string.Format("Havnt any bone with name [{0}]", nameBone));
			return null;
		}

		public Bone GetBone(int idBone)
		{
			Bone[] array = bones;
			foreach (Bone bone in array)
			{
				if (bone.boneID == idBone)
				{
					return bone;
				}
			}
			Debug.LogError(string.Format("Havnt any bone with id [{0}]", idBone));
			return null;
		}

		private void IgnoreColliders(List<SkeletonObject> skeletons)
		{
			foreach (SkeletonObject skeleton in skeletons)
			{
				foreach (SkeletonObject skeleton2 in skeletons)
				{
					if (skeleton != skeleton2 && skeleton.skeletonRagdoll != null && skeleton2.skeletonRagdoll != null)
					{
						skeleton.skeletonRagdoll.IgnoreCollisionWithRagdoll(skeleton2.skeletonRagdoll);
					}
				}
			}
		}

		public void SetEquipped(Equipment itemValue)
		{
			equipment = itemValue;
		}

		public void OverrideMaterial(string materialName, bool value, float transitionTime)
		{
			foreach (ModelSkin skin in skins)
			{
				skin.OverrideMaterial(materialName, value, transitionTime);
			}
		}

		public static XmlElement LoadModelConfig(string nameModel)
		{
			string loadTextInternal = GlobalLoad.GetLoadTextInternal("models_folder", "Configs/" + nameModel);
			if (!loadTextInternal.IsNullOrEmpty())
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(loadTextInternal);
				return (XmlElement)xmlDocument.SelectSingleNode("ModelObject");
			}
			return null;
		}

		public static GameObject LoadBonesPrefab(string namePrefab)
		{
			return GlobalLoad.GetPrefabInstanceInternal("models_folder", "Bones/Prefabs/" + namePrefab);
		}

		public static XmlDocument LoadBonesConfig(string nameConfig)
		{
			string loadTextInternal = GlobalLoad.GetLoadTextInternal("models_folder", "Bones/Configs/" + nameConfig);
			if (!loadTextInternal.IsNullOrEmpty())
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(loadTextInternal);
				return xmlDocument;
			}
			return null;
		}

		public static GameObject LoadSkinPrefab(string namePrefab)
		{
			string[] array = namePrefab.Split(FOLDER_SEPARATOR, StringSplitOptions.None);
			if ((array.Length != 3 && array.Length != 2) || !avaibleTypeFolderName.Contains(array[0]))
			{
				throw new Exception("Invalid skin prefab name " + namePrefab);
			}
			string text = array[0] + "/" + array[1] + "/" + namePrefab;
			return GlobalLoad.GetPrefabInstanceInternal("models_folder", "Skins/Meshes/" + text);
		}
	}
}
