using System;
using System.Collections.Generic;
using System.Linq;
using SF3.GameModels;
using UnityEngine;

public class SetupRagdoll : MonoBehaviour
{
	private class RagdollPartData
	{
		private CapsuleCollider _capsuleCollider;

		private BoxCollider _boxCollider;

		private SphereCollider _sphereCollider;

		private Rigidbody _rigidBody;

		private CharacterJoint _joint;

		private Vector3 _localPosition;

		private Vector3 _localAngles;

		private float _weight;

		public CapsuleCollider capsuleCollider
		{
			get
			{
				return _capsuleCollider;
			}
		}

		public BoxCollider boxCollider
		{
			get
			{
				return _boxCollider;
			}
		}

		public SphereCollider sphereCollider
		{
			get
			{
				return _sphereCollider;
			}
		}

		public Rigidbody rigidBody
		{
			get
			{
				return _rigidBody;
			}
		}

		public CharacterJoint joint
		{
			get
			{
				return _joint;
			}
		}

		public Vector3 localPosition
		{
			get
			{
				return _localPosition;
			}
		}

		public Vector3 localAngles
		{
			get
			{
				return _localAngles;
			}
		}

		public float weight
		{
			get
			{
				return _weight;
			}
		}

		public RagdollPartData(Transform boneTransform, IBonesHolder model)
		{
			_rigidBody = boneTransform.GetComponent<Rigidbody>();
			_joint = boneTransform.GetComponent<CharacterJoint>();
			Collider component = boneTransform.GetComponent<Collider>();
			if (component is CapsuleCollider)
			{
				_capsuleCollider = (CapsuleCollider)component;
			}
			else if (component is BoxCollider)
			{
				_boxCollider = (BoxCollider)component;
			}
			else if (component is SphereCollider)
			{
				_sphereCollider = (SphereCollider)component;
			}
			_localPosition = boneTransform.localPosition;
			_localAngles = boneTransform.localEulerAngles;
			_weight = model.GetBone(boneTransform.name).weight;
		}

		private void ActivateCollider(Collider collider, bool activate)
		{
			if (collider != null)
			{
				collider.enabled = activate;
			}
		}

		public void Activate()
		{
			ActivateCollider(_capsuleCollider, true);
			ActivateCollider(_boxCollider, true);
			ActivateCollider(_sphereCollider, true);
			if (_rigidBody != null)
			{
				_rigidBody.isKinematic = false;
				_rigidBody.useGravity = true;
			}
		}

		public void Disable()
		{
			ActivateCollider(_capsuleCollider, false);
			ActivateCollider(_boxCollider, false);
			ActivateCollider(_sphereCollider, false);
			if (_rigidBody != null)
			{
				_rigidBody.isKinematic = true;
				_rigidBody.useGravity = false;
			}
		}
	}

	private class BoneData
	{
		private GameObject _boneObject;

		private Vector3 _localPosition;

		private Vector3 _localAngles;

		public GameObject boneObject
		{
			get
			{
				return _boneObject;
			}
		}

		public BoneData(GameObject boneObject)
		{
			_boneObject = boneObject;
			_localPosition = _boneObject.transform.localPosition;
			_localAngles = _boneObject.transform.localEulerAngles;
		}

		public void ResetToStartPose()
		{
			_boneObject.transform.localPosition = _localPosition;
			_boneObject.transform.localEulerAngles = _localAngles;
		}
	}

	[SerializeField]
	private Transform _ragdollSourcePrefab;

	[SerializeField]
	private Transform _parentArmature;

	private Dictionary<string, BoneData> _modelBones;

	private Dictionary<string, RagdollPartData> _ragdollData;

	private IBonesHolder _bonesHolder;

	public void Initialize(IBonesHolder bonesHolder)
	{
		_bonesHolder = bonesHolder;
		CollectData();
		SetRagdollPose();
		ConfigureRagdoll();
	}

	private void CollectData()
	{
		_modelBones = new Dictionary<string, BoneData>();
		_ragdollData = new Dictionary<string, RagdollPartData>();
		for (int i = 0; i < _parentArmature.childCount; i++)
		{
			ParseModelHierarchy(_parentArmature.GetChild(i));
		}
		for (int j = 0; j < _ragdollSourcePrefab.childCount; j++)
		{
			ParseRagdollHierarchy(_ragdollSourcePrefab.GetChild(j));
		}
		for (int k = 0; k < _modelBones.Count; k++)
		{
			KeyValuePair<string, BoneData> keyValuePair = _modelBones.ElementAt(k);
			if (!_ragdollData.ContainsKey(keyValuePair.Key))
			{
				_modelBones.Remove(keyValuePair.Key);
				k--;
			}
		}
	}

	private void ParseModelHierarchy(Transform currentRootTransform)
	{
		if (_modelBones.ContainsKey(currentRootTransform.name))
		{
			throw new Exception(string.Format("Parent skeleton already has bone with name [{0}] ", currentRootTransform.name));
		}
		_modelBones.Add(currentRootTransform.name, new BoneData(currentRootTransform.gameObject));
		if (currentRootTransform.childCount > 0)
		{
			for (int i = 0; i < currentRootTransform.childCount; i++)
			{
				ParseModelHierarchy(currentRootTransform.GetChild(i));
			}
		}
	}

	private void ParseRagdollHierarchy(Transform currentRootTransform)
	{
		if (_modelBones.ContainsKey(currentRootTransform.name))
		{
			RagdollPartData value = new RagdollPartData(currentRootTransform, _bonesHolder);
			_ragdollData.Add(currentRootTransform.name, value);
		}
		if (currentRootTransform.childCount > 0)
		{
			for (int i = 0; i < currentRootTransform.childCount; i++)
			{
				ParseRagdollHierarchy(currentRootTransform.GetChild(i));
			}
		}
	}

	public void SetRagdollPose()
	{
		foreach (string key in _ragdollData.Keys)
		{
			_modelBones[key].boneObject.transform.localPosition = _ragdollData[key].localPosition;
			_modelBones[key].boneObject.transform.localEulerAngles = _ragdollData[key].localAngles;
		}
	}

	public void SetTPose()
	{
		foreach (BoneData value in _modelBones.Values)
		{
			value.ResetToStartPose();
		}
	}

	private void ConfigureRagdoll()
	{
		foreach (string key in _ragdollData.Keys)
		{
			CharacterJoint joint = _ragdollData[key].joint;
			if (joint != null)
			{
				CharacterJoint characterJoint = _modelBones[key].boneObject.GetComponent<CharacterJoint>();
				if (characterJoint == null)
				{
					characterJoint = _modelBones[key].boneObject.AddComponent<CharacterJoint>();
				}
				characterJoint.anchor = joint.anchor;
				characterJoint.axis = joint.axis;
				characterJoint.autoConfigureConnectedAnchor = false;
				characterJoint.connectedAnchor = joint.connectedAnchor;
				characterJoint.enablePreprocessing = false;
				characterJoint.enableProjection = joint.enableProjection;
				characterJoint.projectionDistance = joint.projectionDistance;
				characterJoint.projectionAngle = joint.projectionAngle;
				characterJoint.twistLimitSpring = joint.twistLimitSpring;
				characterJoint.swingAxis = joint.swingAxis;
				characterJoint.lowTwistLimit = joint.lowTwistLimit;
				characterJoint.highTwistLimit = joint.highTwistLimit;
				characterJoint.swingLimitSpring = joint.swingLimitSpring;
				characterJoint.swing1Limit = joint.swing1Limit;
				characterJoint.swing2Limit = joint.swing2Limit;
				if (_modelBones.ContainsKey(joint.connectedBody.name))
				{
					characterJoint.connectedBody = _modelBones[joint.connectedBody.name].boneObject.GetComponent<Rigidbody>();
				}
			}
			if (_ragdollData[key].rigidBody != null)
			{
				Rigidbody rigidbody = _modelBones[key].boneObject.GetComponent<Rigidbody>();
				if (rigidbody == null)
				{
					rigidbody = _modelBones[key].boneObject.AddComponent<Rigidbody>();
				}
				rigidbody.interpolation = _ragdollData[key].rigidBody.interpolation;
				rigidbody.mass = _ragdollData[key].weight;
				rigidbody.drag = _ragdollData[key].rigidBody.drag;
			}
			if (_ragdollData[key].sphereCollider != null)
			{
				SphereCollider sphereCollider = _modelBones[key].boneObject.GetComponent<SphereCollider>();
				if (sphereCollider == null)
				{
					sphereCollider = _modelBones[key].boneObject.AddComponent<SphereCollider>();
				}
				sphereCollider.radius = _ragdollData[key].sphereCollider.radius;
				sphereCollider.center = _ragdollData[key].sphereCollider.center;
			}
			else if (_ragdollData[key].boxCollider != null)
			{
				BoxCollider boxCollider = _modelBones[key].boneObject.GetComponent<BoxCollider>();
				if (boxCollider == null)
				{
					boxCollider = _modelBones[key].boneObject.AddComponent<BoxCollider>();
				}
				boxCollider.size = _ragdollData[key].boxCollider.size;
				boxCollider.center = _ragdollData[key].boxCollider.center;
			}
			else if (_ragdollData[key].capsuleCollider != null)
			{
				CapsuleCollider capsuleCollider = _modelBones[key].boneObject.GetComponent<CapsuleCollider>();
				if (capsuleCollider == null)
				{
					capsuleCollider = _modelBones[key].boneObject.AddComponent<CapsuleCollider>();
				}
				capsuleCollider.direction = _ragdollData[key].capsuleCollider.direction;
				capsuleCollider.center = _ragdollData[key].capsuleCollider.center;
				capsuleCollider.height = _ragdollData[key].capsuleCollider.height;
				capsuleCollider.radius = _ragdollData[key].capsuleCollider.radius;
			}
		}
	}
}
