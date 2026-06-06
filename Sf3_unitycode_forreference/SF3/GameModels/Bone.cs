using System;
using System.Collections.Generic;
using UnityEngine;

namespace SF3.GameModels
{
	public class Bone
	{
		private bool _isCollising;

		private Vector3 _lastPosition;

		private Transform _transform;

		public bool animatedThisFrame { get; private set; }

		public Transform transform
		{
			get
			{
				return _transform;
			}
		}

		public Vector3 lossyScale
		{
			get
			{
				return _transform.lossyScale;
			}
		}

		public Vector3 localScale
		{
			get
			{
				return _transform.localScale;
			}
		}

		public Vector3 localPosition
		{
			get
			{
				return _transform.localPosition;
			}
		}

		public Vector3 position
		{
			get
			{
				return _transform.position;
			}
		}

		public Quaternion rotation
		{
			get
			{
				return _transform.rotation;
			}
		}

		public Quaternion localRotation
		{
			get
			{
				return _transform.localRotation;
			}
		}

		public Vector3 previousPosition { get; private set; }

		public Vector3 previousLocalPosition { get; private set; }

		public Vector3 startPosition { get; private set; }

		public Vector3 startRotation { get; private set; }

		public float weight { get; private set; }

		public int boneID { get; private set; }

		public string boneName
		{
			get
			{
				return _transform.name;
			}
		}

		public Bone mirrorBone { get; private set; }

		public Bone parentBone { get; private set; }

		public List<Bone> childBones { get; private set; }

		public bool pseudoPhysics { get; private set; }

		public Rigidbody rigidBody { get; private set; }

		public event Action onPreviousPositionUpdate;

		public Bone(Transform transform, int newBoneID = -1, Bone newParentBone = null)
		{
			_transform = transform;
			childBones = new List<Bone>();
			startRotation = transform.localRotation.eulerAngles;
			startPosition = transform.localPosition;
			boneID = newBoneID;
			parentBone = newParentBone;
			animatedThisFrame = false;
			weight = 0f;
			pseudoPhysics = false;
			UpdatePreviousPosition();
		}

		public void SetPseudoPhysics(bool value)
		{
			rigidBody = _transform.GetComponent<Rigidbody>();
			pseudoPhysics = value;
		}

		public void SetWeight(float newWeight)
		{
			weight = newWeight;
		}

		public void SetBoneID(int newID)
		{
			boneID = newID;
		}

		public void Rotate(Vector3 angles)
		{
			Vector3 localEulerAngles = _transform.localEulerAngles;
			localEulerAngles += angles;
			_transform.localEulerAngles = angles;
		}

		public void SetPosition(Vector3 position, bool isPrevious = true)
		{
			if (isPrevious)
			{
				UpdatePreviousPosition();
			}
			_transform.position = position;
		}

		public void SetRotation(Quaternion value)
		{
			_transform.rotation = value;
		}

		public void SetLocalPosition(Vector3 position)
		{
			_transform.localPosition = position;
			animatedThisFrame = true;
		}

		public void SetLocalRotation(Vector3 vector)
		{
			_transform.localEulerAngles = vector;
		}

		public void SetLocalRotation(Quaternion rotation)
		{
			_transform.localRotation = rotation;
		}

		public void UpdatePreviousPosition()
		{
			if (this.onPreviousPositionUpdate != null)
			{
				this.onPreviousPositionUpdate();
			}
			previousPosition = _transform.position;
			previousLocalPosition = _transform.localPosition;
		}

		public void SetPreviousPosition(Vector3 position)
		{
			previousPosition = position;
		}

		public void ShiftPosition(Vector3 vector)
		{
			SetPosition(position + vector);
		}

		public void SetShiftRotation(Vector3 vector)
		{
		}

		public void ResetAnimated()
		{
			animatedThisFrame = false;
		}

		public void ResetPosition()
		{
			SetLocalPosition(startPosition);
			SetLocalRotation(startRotation);
		}
	}
}
