using System;
using System.Collections.Generic;
using System.Linq;
using SF3.Items;
using SF3.Moves;
using SF3.UserData;
using SF3.Utils;
using UnityEngine;
using sf3DTO;

namespace SF3.GameModels
{
	[Serializable]
	public class ModelComponents : IBonesHolder, IShadowFormModel
	{
		private const string SHURIKEN_TAG = "IM_A_SHURIKEN";

		private Bone[] _bones;

		private Dictionary<int, Bone> _bonesByID;

		private Dictionary<string, Bone> _bonesByName;

		private List<Bone> _pseudoPhysicBones;

		public Dictionary<string, List<ModelMaterial>> capsulesMaterials;

		private Transform _parentTransform;

		private ISkinnedModel _skinnedModel;

		private BehaviourTimer.SingleTimer shadowFormBlendTimer;

		public List<ModelObject> commonEquippedItems { get; private set; }

		public Dictionary<EquipmentType, ModelObject> equippedItems { get; private set; }

		public Dictionary<EquipmentType, ModelObject> droppedItemsNew { get; private set; }

		public List<Bone> pseudoPhysicBones
		{
			get
			{
				return _pseudoPhysicBones;
			}
		}

		public BonesMirrorContainer mirrorBones { get; private set; }

		public Bone rootBone { get; private set; }

		public Bone armatureBone { get; private set; }

		public Bone pivotBone { get; private set; }

		public Bone leftBoneForMirror { get; private set; }

		public Bone rightBoneForMirror { get; private set; }

		public Bone centerOfMassBone { get; private set; }

		public ModelCapsules modelCapsules { get; private set; }

		public SFState sfState { get; private set; }

		public ModelInfo info { get; private set; }

		public bool IsVisible { get; set; }

		public ModelPhysics modelPhysics { get; private set; }

		private ModelObject headComponent
		{
			get
			{
				if (commonEquippedItems.Count < 2)
				{
					return null;
				}
				return commonEquippedItems[1];
			}
		}

		public float Transparency { get; private set; }

		public ModelComponents(ModelInfo info, Model skinnedModelValue, Transform parentTransform)
		{
			this.info = info;
			_skinnedModel = skinnedModelValue;
			commonEquippedItems = new List<ModelObject>();
			equippedItems = new Dictionary<EquipmentType, ModelObject>();
			capsulesMaterials = new Dictionary<string, List<ModelMaterial>>();
			modelCapsules = new ModelCapsules();
			droppedItemsNew = new Dictionary<EquipmentType, ModelObject>();
			_parentTransform = parentTransform;
			if (equippedItems.Count > 0 || commonEquippedItems.Count > 0)
			{
				DestroyAll();
			}
			if (!info.skeleton.IsNullOrEmpty())
			{
				CreateModelObject(info.skeleton);
			}
			if (!info.head.IsNullOrEmpty())
			{
				CreateModelObject(info.head);
			}
			CreateEquippedItems();
			if (equippedItems.Count == 0 && commonEquippedItems.Count == 0)
			{
				throw new Exception(string.Format("Cant create [{0}] model, havnt any equippedItems", info.alias));
			}
			centerOfMassBone = new Bone(new GameObject("centerOfMass").transform);
			centerOfMassBone.transform.parent = _parentTransform;
			InitializeEquipment();
			modelPhysics = new ModelPhysics(parentTransform.gameObject, this);
		}

		private void CreateEquippedItems()
		{
			Equipment[] array = info.GetEquippedItems();
			foreach (Equipment itemValue in array)
			{
				CreateModelObject(itemValue);
			}
		}

		private void DestroyAll()
		{
			DestroyCommonItems();
			DestroyEquippedItems();
		}

		private void DestroyEquippedItems()
		{
			foreach (ModelObject value in equippedItems.Values)
			{
				value.DestroyItem();
			}
			equippedItems.Clear();
		}

		private void DestroyCommonItems()
		{
			foreach (ModelObject commonEquippedItem in commonEquippedItems)
			{
				commonEquippedItem.DestroyItem();
			}
			commonEquippedItems.Clear();
		}

		private ModelObject CreateModelObject(Equipment itemValue)
		{
			EquipmentType equipmentType = itemValue.GetEquipmentType();
			ModelObject modelObject = ((equipmentType != EquipmentType.Ranged && equipmentType != EquipmentType.Magic) ? new ModelObject(info.gender) : new ModelObjectExtended(_skinnedModel.GetId(), info.gender, itemValue));
			if (!equippedItems.ContainsKey(equipmentType))
			{
				equippedItems.Add(equipmentType, modelObject);
			}
			else
			{
				equippedItems[equipmentType].DestroyItem();
				equippedItems[equipmentType] = modelObject;
			}
			modelObject.SetEquipped(itemValue);
			if ((equipmentType == EquipmentType.Ranged || equipmentType == EquipmentType.Magic) && !info.ContainsTag("IM_A_SHURIKEN"))
			{
				return modelObject;
			}
			try
			{
				modelObject.LoadModel(itemValue, _parentTransform, info.gender);
			}
			catch
			{
				Debug.LogError(string.Format("Cant find config for model [{0}]", itemValue.model));
				equippedItems.Remove(equipmentType);
				modelObject = null;
			}
			return modelObject;
		}

		private ModelObject CreateModelObject(string itemModel)
		{
			ModelObject modelObject = new ModelObject(info.gender);
			commonEquippedItems.Add(modelObject);
			try
			{
				modelObject.LoadModel(itemModel, _parentTransform, info.gender);
			}
			catch
			{
				Debug.LogError(string.Format("Cant load model object with name [{0}]", itemModel));
			}
			return modelObject;
		}

		public string GetHead()
		{
			string oldValue = ((UserManager.GetGender() != Gender.Female) ? "_m" : "_f");
			if (headComponent != null)
			{
				return headComponent.name.Replace(oldValue, string.Empty);
			}
			return string.Empty;
		}

		public void SetHead(string head)
		{
			try
			{
				if (headComponent != null)
				{
					headComponent.LoadModel(head, _parentTransform, info.gender);
				}
			}
			catch
			{
				Debug.Log(string.Format("Cant load model object with name [{0}]", head));
			}
			InitializeEquipment();
		}

		public void Initialize()
		{
			IsVisible = true;
			rootBone.SetLocalPosition(Vector3.zero);
			if (shadowFormBlendTimer != null)
			{
				shadowFormBlendTimer.ForceStop();
			}
			sfState = SFState.Disabled;
			UpdateShadowFormBlend(0f);
			if (droppedItemsNew.Count > 0)
			{
				foreach (KeyValuePair<EquipmentType, ModelObject> item in droppedItemsNew)
				{
					CreateModelObject(item.Value.equipment);
				}
				droppedItemsNew.Clear();
				InitializeEquipment();
			}
			bool flag = false;
			if (equippedItems.Count > 0)
			{
				List<ModelObject> list = equippedItems.Values.ToList();
				for (int i = 0; i < list.Count; i++)
				{
					ModelObject modelObject = list[i];
					bool flag2 = true;
					Equipment[] array = info.GetEquippedItems();
					foreach (Equipment equipment in array)
					{
						if (modelObject.equipment.GetEquipmentType() == equipment.GetEquipmentType())
						{
							flag2 = false;
							if (!modelObject.equipment.id.Equals(equipment.id))
							{
								CreateModelObject(equipment);
							}
							break;
						}
					}
					if (flag2)
					{
						modelObject.DestroyItem();
						equippedItems.Remove(modelObject.equipment.GetEquipmentType());
						flag = true;
					}
				}
			}
			else
			{
				Equipment[] array2 = info.GetEquippedItems();
				foreach (Equipment itemValue in array2)
				{
					flag = true;
					CreateModelObject(itemValue);
				}
			}
			if (flag)
			{
				InitializeEquipment();
			}
			modelPhysics.Initialize(this);
			if (info.ContainsTag("IM_A_SHURIKEN"))
			{
				modelPhysics.EnableColliders(true);
				modelPhysics.SetIsTriggerActive(true);
			}
			InitColors();
		}

		public void CreateModelObjects()
		{
			SetHead(GetHead());
			DestroyEquippedItems();
			CreateEquippedItems();
			InitializeEquipment();
			InitColors();
		}

		public void InitializeEquipment()
		{
			CollectModelBones();
			CheckAdditionalSkeletons();
			InitModelObjects();
			ConnectJoints();
			CollectCapsules();
		}

		private void CollectModelBones()
		{
			List<Bone> list = new List<Bone>();
			foreach (ModelObject commonEquippedItem in commonEquippedItems)
			{
				list.AddRange(commonEquippedItem.bones);
			}
			foreach (ModelObject value in equippedItems.Values)
			{
				if (value.bones != null)
				{
					list.AddRange(value.bones);
				}
			}
			if (list.Count == 0)
			{
				throw new Exception(string.Format("Model [{0}] havnt any bones", _parentTransform.name));
			}
			list.Add(centerOfMassBone);
			_bones = list.ToArray();
			_pseudoPhysicBones = new List<Bone>();
			Bone[] bones = _bones;
			foreach (Bone bone in bones)
			{
				if (bone.pseudoPhysics)
				{
					_pseudoPhysicBones.Add(bone);
				}
			}
			_bonesByID = new Dictionary<int, Bone>();
			_bonesByName = new Dictionary<string, Bone>();
			Bone[] bones2 = _bones;
			foreach (Bone bone2 in bones2)
			{
				if (bone2.boneID != -1)
				{
					_bonesByID.Add(bone2.boneID, bone2);
				}
				_bonesByName.Add(bone2.boneName, bone2);
			}
			armatureBone = GetBone("Armature");
			rootBone = armatureBone.childBones[0];
			Bone bone4 = (rightBoneForMirror = null);
			leftBoneForMirror = bone4;
			if (_bonesByName.ContainsKey("foot_l"))
			{
				leftBoneForMirror = _bonesByName["foot_l"];
			}
			if (_bonesByName.ContainsKey("foot_r"))
			{
				rightBoneForMirror = _bonesByName["foot_r"];
			}
		}

		private void InitModelObjects()
		{
			mirrorBones = new BonesMirrorContainer();
			foreach (ModelObject commonEquippedItem in commonEquippedItems)
			{
				commonEquippedItem.Init(this, _parentTransform);
				foreach (SkeletonObject skeleton in commonEquippedItem.skeletons)
				{
					mirrorBones.AddBoneMirroring(skeleton.boneMirrorContainer);
				}
			}
			foreach (ModelObject value in equippedItems.Values)
			{
				value.Init(this, _parentTransform);
				foreach (SkeletonObject skeleton2 in value.skeletons)
				{
					mirrorBones.AddBoneMirroring(skeleton2.boneMirrorContainer);
				}
			}
			InitGradient();
		}

		private void InitGradient()
		{
			foreach (ModelObject commonEquippedItem in commonEquippedItems)
			{
				commonEquippedItem.DisableGradient();
			}
			foreach (ModelObject value in equippedItems.Values)
			{
				value.DisableGradient();
			}
			GradientInfo gradientInfo = null;
			foreach (ModelObject commonEquippedItem2 in commonEquippedItems)
			{
				if (gradientInfo != null)
				{
					break;
				}
				gradientInfo = commonEquippedItem2.gradientInfo;
			}
			foreach (ModelObject value2 in equippedItems.Values)
			{
				if (gradientInfo != null)
				{
					break;
				}
				gradientInfo = value2.gradientInfo;
			}
			if (gradientInfo == null)
			{
				return;
			}
			foreach (ModelObject commonEquippedItem3 in commonEquippedItems)
			{
				commonEquippedItem3.SetGradient(gradientInfo);
			}
		}

		public int GetBonesCount()
		{
			return _bones.Length;
		}

		public CollisionVertex GetVertexByName(string vertexName)
		{
			foreach (ModelObject value in equippedItems.Values)
			{
				if (value.skeletons == null)
				{
					continue;
				}
				foreach (SkeletonObject skeleton in value.skeletons)
				{
					if (skeleton.vertexes == null)
					{
						continue;
					}
					foreach (CollisionVertex vertex in skeleton.vertexes)
					{
						if (vertex.name == vertexName)
						{
							return vertex;
						}
					}
				}
			}
			foreach (ModelObject commonEquippedItem in commonEquippedItems)
			{
				if (commonEquippedItem.skeletons == null)
				{
					continue;
				}
				foreach (SkeletonObject skeleton2 in commonEquippedItem.skeletons)
				{
					if (skeleton2.vertexes == null)
					{
						continue;
					}
					foreach (CollisionVertex vertex2 in skeleton2.vertexes)
					{
						if (vertex2.name == vertexName)
						{
							return vertex2;
						}
					}
				}
			}
			return null;
		}

		public void CollectModelsDataForTactics(List<ModelObject> weaponsObjects)
		{
			List<Bone> list = _bones.OfType<Bone>().ToList();
			foreach (ModelObject weaponsObject in weaponsObjects)
			{
				if (weaponsObject.bones == null)
				{
					continue;
				}
				Bone[] bones = weaponsObject.bones;
				foreach (Bone item in bones)
				{
					if (list.IndexOf(item) < 0)
					{
						list.Add(item);
					}
				}
			}
			if (list.Count == 0)
			{
				throw new Exception(string.Format("Model [{0}] havnt any bones", _parentTransform.name));
			}
			list.Add(centerOfMassBone);
			_bones = list.ToArray();
			_pseudoPhysicBones = new List<Bone>();
			Bone[] bones2 = _bones;
			foreach (Bone bone in bones2)
			{
				if (bone.pseudoPhysics)
				{
					_pseudoPhysicBones.Add(bone);
				}
			}
			_bonesByID = new Dictionary<int, Bone>();
			_bonesByName = new Dictionary<string, Bone>();
			Bone value = null;
			foreach (Bone item2 in list)
			{
				if (item2.boneID != -1 && !_bonesByID.TryGetValue(item2.boneID, out value))
				{
					_bonesByID.Add(item2.boneID, item2);
				}
				if (!_bonesByName.TryGetValue(item2.boneName, out value))
				{
					_bonesByName.Add(item2.boneName, item2);
				}
			}
			armatureBone = GetBone("Armature");
			rootBone = armatureBone.childBones[0];
		}

		private void CheckAdditionalSkeletons()
		{
			foreach (KeyValuePair<EquipmentType, ModelObject> equippedItem in equippedItems)
			{
				if (equippedItem.Value.skeletons == null)
				{
					continue;
				}
				foreach (SkeletonObject skeleton in equippedItem.Value.skeletons)
				{
					if (skeleton.additionalPart)
					{
						skeleton.gameObject.SetActive(false);
						Bone bone = GetBone(skeleton.parentName);
						if (bone == null)
						{
							new Exception(string.Format("Cant find parent bone for [{0}] skeleton. Model [{1}] havnt any bones with name [{2}].", skeleton.name, _parentTransform.name, skeleton.parentName));
						}
						Vector3 localPosition = skeleton.transform.localPosition;
						Quaternion localRotation = skeleton.transform.localRotation;
						Vector3 localScale = skeleton.transform.localScale;
						HingeJointLimit component = skeleton.GetComponent<HingeJointLimit>();
						if (component == null || component.anchor == null)
						{
							skeleton.transform.parent = bone.transform;
							skeleton.transform.localPosition = localPosition;
							skeleton.transform.localRotation = localRotation;
							skeleton.transform.localScale = localScale;
						}
						else if (component != null && component.anchor != null)
						{
							skeleton.transform.parent = null;
							skeleton.transform.position = component.anchor.transform.position;
							skeleton.transform.rotation = component.anchor.transform.rotation;
							skeleton.transform.localScale = component.anchor.transform.localScale;
						}
						bone.childBones.Add(skeleton.bones[0]);
						skeleton.gameObject.SetActive(true);
					}
				}
			}
		}

		private void ConnectJoints()
		{
			foreach (ModelObject value in equippedItems.Values)
			{
				foreach (SkeletonObject skeleton in value.skeletons)
				{
					HingeJoint component = skeleton.GetComponent<HingeJoint>();
					if (component != null)
					{
						Transform transform = GetBone(skeleton.parentName).transform;
						HingeJointLimit component2 = skeleton.GetComponent<HingeJointLimit>();
						if (component2 != null)
						{
							if (!string.IsNullOrEmpty(component2.boneStartName))
							{
								component2.boneStart = GetBone(component2.boneStartName).transform;
							}
							if (!string.IsNullOrEmpty(component2.boneEndName))
							{
								component2.boneEnd = GetBone(component2.boneEndName).transform;
							}
							component.connectedBody = GetBone(skeleton.parentName).transform.GetComponent<Rigidbody>();
						}
					}
					FixedJoint component3 = skeleton.GetComponent<FixedJoint>();
					if (component3 != null)
					{
						Transform transform = GetBone(skeleton.parentName).transform;
						if ((bool)transform)
						{
							component3.connectedBody = transform.GetComponent<Rigidbody>();
							continue;
						}
					}
					CharacterJoint component4 = skeleton.GetComponent<CharacterJoint>();
					if (component4 != null)
					{
						Transform transform = GetBone(skeleton.parentName).transform;
						if ((bool)transform)
						{
							component4.connectedBody = transform.GetComponent<Rigidbody>();
						}
					}
				}
			}
		}

		public void ConnectJointsForTactics(List<ModelObject> weaponObjects)
		{
			foreach (ModelObject weaponObject in weaponObjects)
			{
				foreach (SkeletonObject skeleton in weaponObject.skeletons)
				{
					HingeJoint component = skeleton.GetComponent<HingeJoint>();
					if (component != null)
					{
						Transform transform = GetBone(skeleton.parentName).transform;
						if ((bool)transform)
						{
							component.connectedBody = transform.GetComponent<Rigidbody>();
							continue;
						}
					}
					FixedJoint component2 = skeleton.GetComponent<FixedJoint>();
					if (component2 != null)
					{
						Transform transform = GetBone(skeleton.parentName).transform;
						if ((bool)transform)
						{
							component2.connectedBody = transform.GetComponent<Rigidbody>();
							continue;
						}
					}
					CharacterJoint component3 = skeleton.GetComponent<CharacterJoint>();
					if (component3 != null)
					{
						Transform transform = GetBone(skeleton.parentName).transform;
						if ((bool)transform)
						{
							component3.connectedBody = transform.GetComponent<Rigidbody>();
						}
					}
				}
			}
		}

		public void CollectCapsules()
		{
			ClearAttackingCapsules();
			modelCapsules.Clear();
			Dictionary<string, Transform> dictionary = new Dictionary<string, Transform>();
			List<Transform> list = new List<Transform>();
			capsulesMaterials.Clear();
			float widthSc = 1f;
			float heightSc = 1f;
			float minSize = 0f;
			foreach (ModelObject value in equippedItems.Values)
			{
				if (value == null)
				{
					continue;
				}
				foreach (SkeletonObject skeleton in value.skeletons)
				{
					if (skeleton.capsules != null)
					{
						modelCapsules.AddCapsules(skeleton.capsules);
					}
					if (skeleton.repulsionRect == null)
					{
						continue;
					}
					foreach (KeyValuePair<string, Transform> point in skeleton.repulsionRect.points)
					{
						dictionary.Add(point.Key, point.Value);
					}
					list.AddRange(skeleton.floorHitTransforms);
					widthSc = skeleton.repulsionRect.widthScale;
					heightSc = skeleton.repulsionRect.heightScale;
				}
				if (value.modelMaterials == null)
				{
					continue;
				}
				ModelMaterial[] modelMaterials = value.modelMaterials.modelMaterials;
				foreach (ModelMaterial modelMaterial in modelMaterials)
				{
					if (!capsulesMaterials.ContainsKey(modelMaterial.capsuleName))
					{
						capsulesMaterials.Add(modelMaterial.capsuleName, new List<ModelMaterial>());
					}
					capsulesMaterials[modelMaterial.capsuleName].Add(modelMaterial);
				}
			}
			foreach (ModelObject commonEquippedItem in commonEquippedItems)
			{
				if (commonEquippedItem == null)
				{
					continue;
				}
				foreach (SkeletonObject skeleton2 in commonEquippedItem.skeletons)
				{
					if (skeleton2.capsules != null)
					{
						modelCapsules.AddCapsules(skeleton2.capsules);
					}
					if (skeleton2.repulsionRect == null)
					{
						continue;
					}
					foreach (KeyValuePair<string, Transform> point2 in skeleton2.repulsionRect.points)
					{
						dictionary.Add(point2.Key, point2.Value);
					}
					list.AddRange(skeleton2.floorHitTransforms);
					widthSc = skeleton2.repulsionRect.widthScale;
					heightSc = skeleton2.repulsionRect.heightScale;
					minSize = skeleton2.repulsionRect.minimumSize;
				}
				if (commonEquippedItem.modelMaterials == null)
				{
					continue;
				}
				ModelMaterial[] modelMaterials2 = commonEquippedItem.modelMaterials.modelMaterials;
				foreach (ModelMaterial modelMaterial2 in modelMaterials2)
				{
					if (!capsulesMaterials.ContainsKey(modelMaterial2.capsuleName))
					{
						capsulesMaterials.Add(modelMaterial2.capsuleName, new List<ModelMaterial>());
					}
					capsulesMaterials[modelMaterial2.capsuleName].Add(modelMaterial2);
				}
			}
			modelCapsules.SetRepulsionRect(dictionary, widthSc, heightSc, minSize);
			if (list.Count > 0)
			{
				modelCapsules.SetFloorRepulsion(list, 1f, 1f);
				return;
			}
			modelCapsules.SetFloorRepulsion(new List<Transform> { rootBone.transform }, 1f, 1f);
		}

		public string GetModelMaterialAtPointInCapsule(string capsuleName, Vector3 point)
		{
			if (capsulesMaterials.ContainsKey(capsuleName))
			{
				float num = 0f;
				float num2 = 0f;
				string result = "none";
				bool flag = true;
				{
					foreach (ModelMaterial item in capsulesMaterials[capsuleName])
					{
						string[] vertexNames = item.vertexNames;
						foreach (string vertexName in vertexNames)
						{
							CollisionVertex vertexByName = GetVertexByName(vertexName);
							if (vertexByName == null)
							{
								continue;
							}
							Vector3 position = vertexByName.position;
							if (flag)
							{
								num2 = (point - position).sqrMagnitude;
								num = num2;
								result = item.materialName;
								flag = false;
								continue;
							}
							num2 = (point - position).sqrMagnitude;
							if (num2 < num)
							{
								num = num2;
								result = item.materialName;
							}
						}
					}
					return result;
				}
			}
			return "none";
		}

		public void ClearAttackingCapsules()
		{
			modelCapsules.ClearAttackingCapsules();
		}

		public void ClearAttackingCapsules(int intervalID)
		{
			modelCapsules.ClearAttackingCapsules(intervalID);
		}

		public void AddAttackingCapsules(int intervalID, List<Capsule> capsules)
		{
			modelCapsules.AddAttackingCapsules(intervalID, capsules);
		}

		public void UpdateCenterOffMass(bool isPrevious = false)
		{
			Vector3 zero = Vector3.zero;
			Vector3 position = _bones[0].position;
			if (_bones.Length > 1)
			{
				float num = 0f;
				Bone[] bones = _bones;
				foreach (Bone bone in bones)
				{
					zero += bone.position * bone.weight;
					num += bone.weight;
				}
				if (num == 0f)
				{
					throw new Exception(string.Format("Model [{0}] bone weights sum is 0!", _parentTransform.name));
				}
				position = zero / num;
			}
			centerOfMassBone.SetPosition(position, false);
			if (isPrevious)
			{
				centerOfMassBone.SetPreviousPosition(centerOfMassBone.position);
			}
		}

		private void UpdateShadowFormBlend(float progress)
		{
			foreach (ModelObject value in equippedItems.Values)
			{
				value.UpdateShadowFormBlend(progress);
			}
			foreach (ModelObject commonEquippedItem in commonEquippedItems)
			{
				commonEquippedItem.UpdateShadowFormBlend(progress);
			}
		}

		public void DisalbeDissolve()
		{
			foreach (ModelObject value in equippedItems.Values)
			{
				value.ChangeToNonDissolveShadowForm();
			}
			foreach (ModelObject commonEquippedItem in commonEquippedItems)
			{
				commonEquippedItem.ChangeToNonDissolveShadowForm();
			}
		}

		public void ReturnDefaultMaterial()
		{
			foreach (ModelObject value in equippedItems.Values)
			{
				value.ReturnDefaultMaterial();
			}
			foreach (ModelObject commonEquippedItem in commonEquippedItems)
			{
				commonEquippedItem.ReturnDefaultMaterial();
			}
		}

		public void ActivateShadowForm(bool instant = false)
		{
			if (sfState == SFState.Activated)
			{
				return;
			}
			if (shadowFormBlendTimer != null)
			{
				shadowFormBlendTimer.Stop();
			}
			foreach (ModelObject value in equippedItems.Values)
			{
				value.ActivateShadowFormEffect();
			}
			foreach (ModelObject commonEquippedItem in commonEquippedItems)
			{
				commonEquippedItem.ActivateShadowFormEffect();
			}
			sfState = SFState.InActivating;
			if (instant)
			{
				UpdateShadowFormBlend(0.999f);
				return;
			}
			shadowFormBlendTimer = BehaviourTimer.CreateGameFramesTimer(45, UpdateShadowFormBlend, null, delegate
			{
				shadowFormBlendTimer = null;
				sfState = SFState.Activated;
				DisalbeDissolve();
			});
		}

		public void ResetBonesPosition()
		{
			foreach (KeyValuePair<EquipmentType, ModelObject> equippedItem in equippedItems)
			{
				if (equippedItem.Key == EquipmentType.Weapon)
				{
					equippedItem.Value.ResetBonesPosition();
				}
			}
			if (_bonesByName.ContainsKey("weapon_l"))
			{
				_bonesByName["weapon_l"].ResetPosition();
			}
			if (_bonesByName.ContainsKey("weapon_r"))
			{
				_bonesByName["weapon_r"].ResetPosition();
			}
		}

		public void DisableShadowForm()
		{
			if (sfState != 0)
			{
				if (shadowFormBlendTimer != null)
				{
					shadowFormBlendTimer.Stop();
				}
				sfState = SFState.InDisabling;
				shadowFormBlendTimer = BehaviourTimer.CreateGameFramesTimer(45, delegate(float progress)
				{
					UpdateShadowFormBlend(1f - progress);
				}, null, delegate
				{
					shadowFormBlendTimer = null;
					sfState = SFState.Disabled;
					ReturnDefaultMaterial();
				});
			}
		}

		public bool GetShadowFormActive()
		{
			return false;
		}

		public void UpdateShadowForm()
		{
		}

		public void UpdateBonesPositions(Dictionary<int, AnimatedTransform> bonesAnimationTransforms)
		{
			for (int i = 0; i < _bones.Length; i++)
			{
				_bones[i].UpdatePreviousPosition();
			}
			foreach (KeyValuePair<int, AnimatedTransform> bonesAnimationTransform in bonesAnimationTransforms)
			{
				_bonesByID[bonesAnimationTransform.Key].ResetAnimated();
				if (bonesAnimationTransform.Value.animateThisFrame)
				{
					_bonesByID[bonesAnimationTransform.Key].SetLocalPosition(bonesAnimationTransform.Value.position);
					_bonesByID[bonesAnimationTransform.Key].SetLocalRotation(bonesAnimationTransform.Value.rotation);
				}
			}
		}

		public void FillAnimatedTransforms(ref ModelAnimation.BlendAnimatedTransforms result)
		{
			for (int i = 0; i < _bones.Length; i++)
			{
				if (_bones[i].boneID != -1)
				{
					result.animatedTransforms[_bones[i].boneID].animateThisFrame = true;
					result.animatedTransforms[_bones[i].boneID].SetPosition(_bones[i].localPosition);
					result.animatedTransforms[_bones[i].boneID].SetRotation(_bones[i].localRotation);
				}
			}
		}

		public int[] GetBonesIDs()
		{
			int[] array = new int[_bones.Length];
			for (int i = 0; i < _bones.Length; i++)
			{
				array[i] = _bones[i].boneID;
			}
			return array;
		}

		public int GetBoneMirrorID(int normalID)
		{
			int num = -1;
			foreach (ModelObject value in equippedItems.Values)
			{
				foreach (SkeletonObject skeleton in value.skeletons)
				{
					num = skeleton.GetBoneMirrorID(normalID);
					if (num != -1)
					{
						return num;
					}
				}
			}
			foreach (ModelObject commonEquippedItem in commonEquippedItems)
			{
				foreach (SkeletonObject skeleton2 in commonEquippedItem.skeletons)
				{
					num = skeleton2.GetBoneMirrorID(normalID);
					if (num != -1)
					{
						return num;
					}
				}
			}
			return -1;
		}

		public Bone GetMirrorBone(Bone currentBone)
		{
			int num = -1;
			foreach (ModelObject value in equippedItems.Values)
			{
				foreach (SkeletonObject skeleton in value.skeletons)
				{
					num = skeleton.GetBoneMirrorID(currentBone.boneID);
					if (num != -1)
					{
						return GetBone(num);
					}
				}
			}
			foreach (ModelObject commonEquippedItem in commonEquippedItems)
			{
				foreach (SkeletonObject skeleton2 in commonEquippedItem.skeletons)
				{
					num = skeleton2.GetBoneMirrorID(currentBone.boneID);
					if (num != -1)
					{
						return GetBone(num);
					}
				}
			}
			return currentBone;
		}

		public void SetPivotBone(string boneName, bool isMirrored)
		{
			pivotBone = GetBone(boneName);
			if (isMirrored)
			{
				pivotBone = GetMirrorBone(pivotBone);
			}
		}

		public Bone GetBone(string boneName)
		{
			if (_bonesByName.ContainsKey(boneName))
			{
				return _bonesByName[boneName];
			}
			Debug.LogWarning("missing bone  " + boneName);
			return null;
		}

		public Bone GetBone(int boneID)
		{
			return _bonesByID[boneID];
		}

		public bool IsNeedsMirroringModelObjectSwitch(bool isMirrored)
		{
			foreach (ModelObject value in equippedItems.Values)
			{
				foreach (ModelSkin skin in value.skins)
				{
					if (skin.mirroringDuplicate.HasValue)
					{
						return skin.IsNeedsMirrorSwitch(isMirrored);
					}
				}
			}
			return false;
		}

		public void ShowSkins(bool isShow, bool isMirrored)
		{
			foreach (ModelObject value in equippedItems.Values)
			{
				value.ShowSkins(isShow, isMirrored);
			}
			foreach (ModelObject commonEquippedItem in commonEquippedItems)
			{
				commonEquippedItem.ShowSkins(isShow, isMirrored);
			}
		}

		public void ChangeSkinsColor(UnityEngine.Color color, bool useAlpha = true)
		{
			foreach (ModelObject value in equippedItems.Values)
			{
				value.SetMainColor(color, useAlpha);
			}
			foreach (ModelObject commonEquippedItem in commonEquippedItems)
			{
				commonEquippedItem.SetMainColor(color, useAlpha);
			}
		}

		public void SetOpaque()
		{
			foreach (ModelObject value in equippedItems.Values)
			{
				value.SetOpaque();
			}
			foreach (ModelObject commonEquippedItem in commonEquippedItems)
			{
				commonEquippedItem.SetOpaque();
			}
		}

		public void SetDissolveWeaponMaterial()
		{
			foreach (KeyValuePair<EquipmentType, ModelObject> equippedItem in equippedItems)
			{
				if (equippedItem.Key == EquipmentType.Weapon)
				{
					equippedItem.Value.SetDissolveWeaponMaterial();
				}
			}
		}

		public void DissolveWeaponMeshrendererOff()
		{
			foreach (KeyValuePair<EquipmentType, ModelObject> equippedItem in equippedItems)
			{
				if (equippedItem.Key == EquipmentType.Weapon)
				{
					equippedItem.Value.DissolveWeaponMeshrendererOff();
				}
			}
		}

		public void DissolveWeaponMeshrendererOn()
		{
			foreach (KeyValuePair<EquipmentType, ModelObject> equippedItem in equippedItems)
			{
				if (equippedItem.Key == EquipmentType.Weapon)
				{
					equippedItem.Value.DissolveWeaponMeshrendererOn();
				}
			}
		}

		public void DissolveWeaponMaterialOff()
		{
			foreach (KeyValuePair<EquipmentType, ModelObject> equippedItem in equippedItems)
			{
				if (equippedItem.Key == EquipmentType.Weapon)
				{
					equippedItem.Value.DissolveWeaponMaterialOff();
				}
			}
		}

		public void SetTransparent(float value)
		{
			Transparency = value;
			foreach (ModelObject value2 in equippedItems.Values)
			{
				value2.SetTransparent(value);
			}
			foreach (ModelObject commonEquippedItem in commonEquippedItems)
			{
				commonEquippedItem.SetTransparent(value);
			}
		}

		private void InitColors()
		{
			if (ColorManager.Instance == null)
			{
				Debug.LogWarning("Missing color manager");
				return;
			}
			sf3DTO.Color hairColor = info.hairColor;
			sf3DTO.Color skinColor = info.skinColor;
			SetHairColor(hairColor);
			SetSkinColor(skinColor);
			PreRenderMainTexture();
		}

		private void PreRenderMainTexture()
		{
			foreach (KeyValuePair<EquipmentType, ModelObject> equippedItem in equippedItems)
			{
				equippedItem.Value.PreRenderTexture();
			}
			if (headComponent != null)
			{
				headComponent.PreRenderTexture();
			}
		}

		private void SetHairColor(sf3DTO.Color colorData)
		{
			ColorPreset hairColor = ColorManager.Instance.GetHairColor(colorData.ColorId);
			if (hairColor != null)
			{
				UnityEngine.Color color = hairColor.GetColor((float)colorData.Value);
				SetHairColor(color);
			}
			else
			{
				Debug.LogWarning("Missing  hair color " + colorData.ColorId);
			}
		}

		public void SetHairColor(UnityEngine.Color col)
		{
			foreach (KeyValuePair<EquipmentType, ModelObject> equippedItem in equippedItems)
			{
				if (equippedItem.Key == EquipmentType.Helmet)
				{
					equippedItem.Value.SetSkinColor(col);
				}
			}
		}

		private void SetSkinColor(sf3DTO.Color colorData)
		{
			ColorPreset skinColor = ColorManager.Instance.GetSkinColor(colorData.ColorId);
			if (skinColor != null)
			{
				UnityEngine.Color color = skinColor.GetColor((float)colorData.Value);
				SetSkinColor(color);
			}
			else
			{
				Debug.LogWarning("Missing color " + colorData.ColorId);
			}
		}

		public void SetSkinColor(UnityEngine.Color col)
		{
			foreach (KeyValuePair<EquipmentType, ModelObject> equippedItem in equippedItems)
			{
				if (equippedItem.Key != EquipmentType.Helmet)
				{
					equippedItem.Value.SetSkinColor(col);
				}
			}
			if (headComponent != null)
			{
				headComponent.SetSkinColor(col);
			}
		}

		public bool DropItems(bool shadowFormActive, float? multiplayer, bool freeze)
		{
			bool flag = false;
			for (int i = 0; i < equippedItems.Count; i++)
			{
				KeyValuePair<EquipmentType, ModelObject> keyValuePair = equippedItems.ElementAt(i);
				if (keyValuePair.Value.droppable)
				{
					droppedItemsNew.Add(keyValuePair.Key, keyValuePair.Value);
					keyValuePair.Value.DropModelObject(Model.hitResult.Impulse, sfState, multiplayer, freeze);
					equippedItems[keyValuePair.Key] = new ModelObject(info.gender);
					equippedItems[keyValuePair.Key].LoadModel(Equipment.GetDefaultEquipment(keyValuePair.Key), _parentTransform, info.gender, false);
					flag = true;
				}
			}
			if (flag)
			{
				InitializeEquipment();
			}
			return flag;
		}

		public Bone[] GetDeepBonesCopy()
		{
			Bone[] array = new Bone[_bones.Length];
			Array.Copy(_bones, array, _bones.Length);
			return array;
		}

		public void ApplyMaskColor(EquipmentType itemType)
		{
			if (equippedItems.ContainsKey(itemType))
			{
				equippedItems[itemType].ApplyMaskColor();
			}
		}

		public ModelObject GetEquipedModelObject(EquipmentType eqvType)
		{
			if (equippedItems.ContainsKey(eqvType))
			{
				return equippedItems[eqvType];
			}
			return null;
		}

		public void EquipItem(EquipmentType type, Equipment itemValue, bool isMirrored)
		{
			ModelObject modelObject = CreateModelObject(itemValue);
			if (modelObject != null)
			{
				InitializeEquipment();
				InitColors();
				ShowSkins(true, isMirrored);
				if (Transparency >= 0.999f)
				{
					modelObject.SetOpaque();
				}
				else
				{
					modelObject.SetTransparent(Transparency);
				}
			}
		}

		public void UnEquipItem(EquipmentType type, bool isMirrored)
		{
			if (equippedItems.ContainsKey(type))
			{
				equippedItems[type].DestroyItem();
				equippedItems.Remove(type);
				InitializeEquipment();
				ShowSkins(true, isMirrored);
			}
		}

		public void OverrideMaterial(string materialName, bool value, float transitionTime)
		{
			foreach (ModelObject value2 in equippedItems.Values)
			{
				value2.OverrideMaterial(materialName, value, transitionTime);
			}
			foreach (ModelObject commonEquippedItem in commonEquippedItems)
			{
				commonEquippedItem.OverrideMaterial(materialName, value, transitionTime);
			}
		}
	}
}
