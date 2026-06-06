using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Nekki;
using SF3.Moves;
using UnityEngine;

namespace SF3.GameModels
{
	[Serializable]
	public class SkeletonObject : ExtentionBehaviour, IBonesHolder
	{
		private class BoneData
		{
			public string id;

			public string mass;

			public bool pseudoPhysics;

			public BoneData(string id, string mass)
			{
				this.id = id;
				this.mass = mass;
				pseudoPhysics = false;
			}
		}

		private XmlElement _bonesDataNode;

		[SerializeField]
		private string _parentName;

		[SerializeField]
		private bool _additionalPart;

		public bool droppable;

		public BonesMirrorContainer boneMirrorContainer { get; private set; }

		public string parentName
		{
			get
			{
				return _parentName;
			}
		}

		public bool additionalPart
		{
			get
			{
				return _additionalPart;
			}
		}

		public List<Bone> rootBones { get; private set; }

		public Bone[] bones { get; private set; }

		public List<CollisionVertex> vertexes { get; private set; }

		public List<Capsule> capsules { get; private set; }

		public RepulsionRect repulsionRect { get; private set; }

		public List<Transform> floorHitTransforms { get; private set; }

		public SkeletonRagdoll skeletonRagdoll { get; private set; }

		public event Action<SkeletonObject, Collision> OnCollision = delegate
		{
		};

		public void CreateSkeleton(Transform parentBoneTransform, XmlElement bonesDataNode, string mirroringDuplicateIndex)
		{
			_bonesDataNode = bonesDataNode;
			boneMirrorContainer = new BonesMirrorContainer();
			bones = new Bone[0];
			floorHitTransforms = new List<Transform>();
			CreateBones(parentBoneTransform);
			SetupRagdoll component = base.gameObject.GetComponent<SetupRagdoll>();
			if (component != null)
			{
				component.Initialize(this);
			}
			skeletonRagdoll = base.gameObject.GetComponent<SkeletonRagdoll>();
			if (skeletonRagdoll != null)
			{
				skeletonRagdoll.Init(mirroringDuplicateIndex);
			}
		}

		public void Init(IBonesHolder modelState)
		{
			if (_bonesDataNode == null || modelState == null)
			{
				return;
			}
			XmlElement xmlElement = (XmlElement)_bonesDataNode.SelectSingleNode("Vertexes");
			XmlElement xmlElement2;
			if (xmlElement != null)
			{
				InitVertexes(modelState, xmlElement);
				xmlElement2 = (XmlElement)_bonesDataNode.SelectSingleNode("Capsules");
				if (xmlElement2 != null)
				{
					InitCapsules(xmlElement2);
				}
			}
			xmlElement2 = (XmlElement)_bonesDataNode.SelectSingleNode("Capsules");
			if (_bonesDataNode != null)
			{
				ParseMirroring(modelState);
			}
			repulsionRect = null;
			xmlElement2 = (XmlElement)_bonesDataNode.SelectSingleNode("RepulsionRect");
			if (xmlElement2 != null)
			{
				CreateRepulsionRect(xmlElement2);
			}
			_bonesDataNode = null;
		}

		private void CreateRepulsionRect(XmlElement rectRepulsionNode)
		{
			Dictionary<string, Transform> dictionary = new Dictionary<string, Transform>();
			foreach (XmlElement childNode in rectRepulsionNode.ChildNodes)
			{
				XmlAttribute attributeNode = childNode.GetAttributeNode("Bone");
				dictionary.Add(attributeNode.Value, GetBone(attributeNode.Value).transform);
			}
			XmlAttribute attributeNode2 = rectRepulsionNode.GetAttributeNode("WidthScale");
			XmlAttribute attributeNode3 = rectRepulsionNode.GetAttributeNode("HeightScale");
			XmlAttribute attributeNode4 = rectRepulsionNode.GetAttributeNode("MinimumSize");
			float widthScale = ((attributeNode2 == null) ? 1f : float.Parse(attributeNode2.Value, CultureInfo.InvariantCulture));
			float heightScale = ((attributeNode3 == null) ? 1f : float.Parse(attributeNode3.Value, CultureInfo.InvariantCulture));
			float value = ((attributeNode4 == null) ? 0f : float.Parse(attributeNode4.Value, CultureInfo.InvariantCulture));
			repulsionRect = new RepulsionRect(dictionary, widthScale, heightScale, value);
		}

		private void CreateBones(Transform parentBoneTransform)
		{
			Dictionary<string, BoneData> dictionary = new Dictionary<string, BoneData>();
			List<string> list = new List<string>();
			if (_bonesDataNode != null)
			{
				XmlElement xmlElement = (XmlElement)_bonesDataNode.SelectSingleNode("Bones");
				if (xmlElement == null)
				{
					return;
				}
				foreach (XmlElement childNode in xmlElement.ChildNodes)
				{
					string attribute = childNode.GetAttribute("Name");
					string attribute2 = childNode.GetAttribute("ID");
					string attribute3 = childNode.GetAttribute("Mass");
					if (childNode.GetAttribute("FloorHit") == "1")
					{
						list.Add(attribute2);
					}
					bool pseudoPhysics = (childNode.GetAttribute("Pseudo-physics").Equals("1") ? true : false);
					BoneData boneData = new BoneData(attribute2, attribute3);
					boneData.pseudoPhysics = pseudoPhysics;
					dictionary.Add(attribute, boneData);
				}
			}
			CreateBonesHierarchy(parentBoneTransform, dictionary);
			foreach (string item in list)
			{
				floorHitTransforms.Add(GetBone(int.Parse(item)).transform);
			}
		}

		private void ParseMirroring(IBonesHolder modelState)
		{
			XmlElement xmlElement = (XmlElement)_bonesDataNode.SelectSingleNode("Mirroring");
			if (xmlElement == null)
			{
				return;
			}
			foreach (XmlElement childNode in xmlElement.ChildNodes)
			{
				bool flag = childNode.Name.Equals("DualMirrorers");
				foreach (XmlElement childNode2 in childNode.ChildNodes)
				{
					XmlAttribute attributeNode = childNode2.GetAttributeNode("MirrorAxis");
					MirrorAxisID enumerator3 = EnumsCompliancer.GetEnumerator<MirrorAxisID>(attributeNode.Value);
					BoneMirrorerData boneMirrorerData = ((!flag) ? new BoneMirrorerData() : new DualBoneMirrorerData());
					boneMirrorerData.SetAxis(enumerator3);
					foreach (XmlElement childNode3 in childNode2.ChildNodes)
					{
						attributeNode = childNode3.GetAttributeNode("Name");
						int boneID = modelState.GetBone(attributeNode.Value).boneID;
						boneMirrorerData.AddMirroredID(boneID);
						if (flag)
						{
							attributeNode = childNode3.GetAttributeNode("MirrorBone");
							if (attributeNode == null)
							{
								throw new Exception(string.Format("Cant find MirrorBone for {0}", boneID));
							}
							int boneID2 = modelState.GetBone(attributeNode.Value).boneID;
							((DualBoneMirrorerData)boneMirrorerData).AddMirrorMirroredID(boneID2);
						}
					}
					boneMirrorContainer.AddBoneMirroring(boneMirrorerData);
				}
			}
		}

		private void CreateBonesHierarchy(Transform parentBoneTransform, Dictionary<string, BoneData> bonesIDData)
		{
			rootBones = new List<Bone>();
			int num = 0;
			Transform transform = null;
			if (additionalPart)
			{
				transform = parentBoneTransform;
			}
			else
			{
				Transform[] componentsInChildren = parentBoneTransform.GetComponentsInChildren<Transform>();
				Transform[] array = componentsInChildren;
				foreach (Transform transform2 in array)
				{
					if (transform2.name.Equals("Armature"))
					{
						transform = transform2;
						break;
					}
				}
			}
			if (transform == null)
			{
				throw new Exception(string.Format("Cant find root for create bones hierarchy in transform [{0}]", parentBoneTransform.name));
			}
			bones = new Bone[transform.GetComponentsInChildren<Transform>().Length];
			Bone bone = new Bone(transform);
			if (bonesIDData.ContainsKey(bone.boneName))
			{
				bone.SetBoneID(int.Parse(bonesIDData[bone.boneName].id));
				bone.SetWeight((bonesIDData[bone.boneName].mass.Length <= 0) ? 0f : float.Parse(bonesIDData[bone.boneName].mass, CultureInfo.InvariantCulture));
				bone.SetPseudoPhysics(bonesIDData[bone.boneName].pseudoPhysics);
			}
			rootBones.Add(bone);
			bones[num] = bone;
			num++;
			CreateBonesHierarchyRecursively(bone, transform, bonesIDData, ref num);
		}

		private void CreateBonesHierarchyRecursively(Bone parentBone, Transform parentTransf, Dictionary<string, BoneData> bonesIDData, ref int num)
		{
			foreach (Transform item in parentTransf)
			{
				Transform transform2 = item;
				Bone bone = new Bone(transform2, -1, parentBone);
				if (bonesIDData.ContainsKey(bone.boneName))
				{
					bone.SetBoneID(int.Parse(bonesIDData[bone.boneName].id));
					string mass = bonesIDData[bone.boneName].mass;
					bone.SetWeight((!(mass != string.Empty)) ? 0f : float.Parse(mass, CultureInfo.InvariantCulture));
					bone.SetPseudoPhysics(bonesIDData[bone.boneName].pseudoPhysics);
				}
				parentBone.childBones.Add(bone);
				bones[num] = bone;
				num++;
				CreateBonesHierarchyRecursively(bone, item, bonesIDData, ref num);
			}
		}

		private void InitVertexes(IBonesHolder modelState, XmlElement vertexesNode)
		{
			vertexes = new List<CollisionVertex>();
			foreach (XmlElement childNode in vertexesNode.ChildNodes)
			{
				Bone bone = modelState.GetBone(childNode.GetAttribute("BoneName"));
				if (bone != null)
				{
					Vector3 newOffset = Vector3.zero;
					XmlAttribute attributeNode = childNode.GetAttributeNode("LocalPosition");
					if (attributeNode != null)
					{
						newOffset = Vector3D.GetFromString(attributeNode.Value);
					}
					CollisionVertex item = new CollisionVertex(bone, newOffset, childNode.GetAttribute("Name"));
					vertexes.Add(item);
				}
				else
				{
					Debug.Log(string.Format("Vertex with name - {0} does not exist!!", childNode.GetAttribute("BoneName")));
				}
			}
		}

		private void InitCapsules(XmlElement capsulesNode)
		{
			capsules = new List<Capsule>();
			foreach (XmlElement childNode in capsulesNode.ChildNodes)
			{
				CollisionVertex vertexByName = GetVertexByName(childNode.GetAttribute("Vertex1"));
				CollisionVertex vertexByName2 = GetVertexByName(childNode.GetAttribute("Vertex2"));
				if (vertexByName != null && vertexByName2 != null)
				{
					Capsule capsule = new Capsule(vertexByName, vertexByName2, float.Parse(childNode.GetAttribute("Radius"), CultureInfo.InvariantCulture), childNode.GetAttribute("Name"), childNode.GetAttribute("ParentRigidbody"));
					XmlAttribute attributeNode = childNode.GetAttributeNode("Collisible");
					if (attributeNode != null)
					{
						capsule.SetCollisible(attributeNode.Value == "1");
					}
					attributeNode = childNode.GetAttributeNode("Repulsive");
					if (attributeNode != null)
					{
						capsule.SetRepulsive(attributeNode.Value == "1");
					}
					attributeNode = childNode.GetAttributeNode("Defense");
					if (attributeNode != null)
					{
						capsule.SetDefense(ComplianceUtils.GetAttributeTypeByName(attributeNode.Value));
					}
					capsules.Add(capsule);
				}
			}
			foreach (XmlElement childNode2 in capsulesNode.ChildNodes)
			{
				XmlAttribute attributeNode = childNode2.GetAttributeNode("LeftMirrorCapsule");
				if (attributeNode == null)
				{
					continue;
				}
				XmlAttribute attributeNode2 = childNode2.GetAttributeNode("Name");
				foreach (Capsule capsule2 in capsules)
				{
					if (!capsule2.name.Equals(attributeNode2.Value))
					{
						continue;
					}
					foreach (Capsule capsule3 in capsules)
					{
						if (capsule3.name.Equals(attributeNode.Value))
						{
							capsule2.SetLeftMirror(capsule3);
							capsule3.SetLeftMirror(capsule2);
							break;
						}
					}
					break;
				}
			}
		}

		public void SetAsAdditional(string newParentName)
		{
			_additionalPart = true;
			_parentName = newParentName;
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
			return null;
		}

		public Bone GetBone(int boneID)
		{
			Bone[] array = bones;
			foreach (Bone bone in array)
			{
				if (bone.boneID == boneID)
				{
					return bone;
				}
			}
			return null;
		}

		public CollisionVertex GetVertexByName(string nameVertex)
		{
			foreach (CollisionVertex vertex in vertexes)
			{
				if (vertex.name.Equals(nameVertex))
				{
					return vertex;
				}
			}
			return null;
		}

		public int GetBoneMirrorID(int normalID)
		{
			return boneMirrorContainer.GetMirrorID(normalID);
		}

		public BoneMirrorerData GetBoneMirrorerData(int boneID)
		{
			return boneMirrorContainer.GetBoneMirrorerData(boneID);
		}

		private void OnCollisionEnter(Collision collision)
		{
			this.OnCollision(this, collision);
		}
	}
}
