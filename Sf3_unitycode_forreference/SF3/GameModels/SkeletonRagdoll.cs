using System.Collections.Generic;
using UnityEngine;

namespace SF3.GameModels
{
	public class SkeletonRagdoll : ExtentionBehaviour
	{
		private const int NONE_MIRRORING_DUPLICATE = -1;

		public bool activeOnStart;

		public string[] ignoredTransforms;

		private HashSet<string> _ignoredTransformsSet;

		private CharacterJoint[] _joints;

		private Collider[] _colliders;

		private Rigidbody[] _rigidBodies;

		private bool _hasIgnoredColliders;

		private Vector3[] _lastImpulse;

		private Vector3[] _lastTorque;

		private Vector3 _forceAwake;

		private Vector3 _torqueAwake;

		private bool _isSleeping;

		private int _sleepPriority;

		private bool isFrozen;

		public bool isActive { get; private set; }

		public int mirroringDuplicateIndex { get; private set; }

		public void Init(string mirroringDuplicateIndex)
		{
			isActive = false;
			_isSleeping = false;
			_sleepPriority = -1;
			_ignoredTransformsSet = new HashSet<string>();
			string[] array = ignoredTransforms;
			foreach (string item in array)
			{
				_ignoredTransformsSet.Add(item);
			}
			ignoredTransforms = null;
			CollectJoints();
			CollectColliders();
			CollectRigidBodies();
			SetActive(activeOnStart);
			this.mirroringDuplicateIndex = ((!string.IsNullOrEmpty(mirroringDuplicateIndex)) ? int.Parse(mirroringDuplicateIndex) : (-1));
		}

		private void CollectJoints()
		{
			List<CharacterJoint> components = new List<CharacterJoint>(base.gameObject.GetComponentsInChildren<CharacterJoint>());
			_joints = ApplyFilterIgnored(components).ToArray();
		}

		private void CollectColliders()
		{
			List<Collider> components = new List<Collider>(base.gameObject.GetComponentsInChildren<Collider>());
			_colliders = ApplyFilterIgnored(components).ToArray();
		}

		private void CollectRigidBodies()
		{
			List<Rigidbody> components = new List<Rigidbody>(base.gameObject.GetComponentsInChildren<Rigidbody>());
			_rigidBodies = ApplyFilterIgnored(components).ToArray();
		}

		private List<T> ApplyFilterIgnored<T>(List<T> components) where T : Component
		{
			if (_ignoredTransformsSet.Count > 0)
			{
				for (int i = 0; i < components.Count; i++)
				{
					HashSet<string> ignoredTransformsSet = _ignoredTransformsSet;
					T val = components[i];
					if (ignoredTransformsSet.Contains(val.transform.name))
					{
						components.RemoveAt(i);
						i--;
					}
				}
			}
			return components;
		}

		public void AddForce(Vector3 impulse, string capsuleName)
		{
			if (capsuleName.Length == 0 || impulse == Vector3.zero)
			{
				return;
			}
			for (int i = 0; i < _rigidBodies.Length; i++)
			{
				if (_rigidBodies[i].gameObject.name.Equals(capsuleName))
				{
					Rigidbody[] rigidBodies = _rigidBodies;
					foreach (Rigidbody rigidbody in rigidBodies)
					{
						rigidbody.velocity = new Vector3(0f, 0f, 0f);
					}
					_rigidBodies[i].AddForce(impulse, ForceMode.Impulse);
					return;
				}
			}
			Debug.Log(string.Format("Can't find rigidbody with capsule name [{0}]", capsuleName));
		}

		public void AddForceAwake(Vector3 impulse)
		{
			_forceAwake = impulse;
		}

		public void AddTorqueAwake(Vector3 torque)
		{
			_torqueAwake = torque;
		}

		public void AddForce(Vector3 impulse, int index = 0)
		{
			if (!isActive)
			{
				SetActive(true);
			}
			_rigidBodies[index].AddForce(impulse, ForceMode.Impulse);
		}

		public void AddTorque(Vector3 torque, int index = 0)
		{
			if (!isActive)
			{
				SetActive(true);
			}
			_rigidBodies[index].AddTorque(torque, ForceMode.Acceleration);
		}

		public void EnableColliders(bool enable)
		{
			if (!isFrozen)
			{
				Collider[] colliders = _colliders;
				foreach (Collider collider in colliders)
				{
					collider.enabled = enable;
				}
			}
		}

		public void SetIsTriggerActive(bool active)
		{
			Collider[] colliders = _colliders;
			foreach (Collider collider in colliders)
			{
				collider.isTrigger = active;
			}
		}

		public void SetActive(bool isActiveVal)
		{
			isActive = isActiveVal;
			EnableColliders(isActive);
			CharacterJoint[] joints = _joints;
			foreach (CharacterJoint characterJoint in joints)
			{
				characterJoint.enableCollision = isActive;
			}
			Rigidbody[] rigidBodies = _rigidBodies;
			foreach (Rigidbody rigidbody in rigidBodies)
			{
				rigidbody.isKinematic = !isActive;
			}
		}

		public void FreezeObject(bool freeze)
		{
			isFrozen = freeze;
			RigidbodyConstraints constraints = (freeze ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.None);
			Rigidbody[] rigidBodies = _rigidBodies;
			foreach (Rigidbody rigidbody in rigidBodies)
			{
				rigidbody.constraints = constraints;
			}
		}

		public void SetRagdollSleepState(bool isSleep, int priority)
		{
			if (_isSleeping == isSleep || _sleepPriority > priority)
			{
				return;
			}
			_isSleeping = isSleep;
			if (_isSleeping)
			{
				_lastImpulse = new Vector3[_rigidBodies.Length];
				_lastTorque = new Vector3[_rigidBodies.Length];
				for (int i = 0; i < _rigidBodies.Length; i++)
				{
					_lastImpulse[i] = _rigidBodies[i].velocity;
					_lastTorque[i] = _rigidBodies[i].angularVelocity;
					_rigidBodies[i].Sleep();
				}
			}
			else
			{
				for (int j = 0; j < _rigidBodies.Length; j++)
				{
					_rigidBodies[j].WakeUp();
					_rigidBodies[j].velocity = _lastImpulse[j];
					_lastImpulse[j] = Vector3.zero;
					_rigidBodies[j].angularVelocity = _lastTorque[j];
					_lastTorque[j] = Vector3.zero;
				}
				AddForce(_forceAwake);
				_forceAwake = Vector3.zero;
				AddTorque(_torqueAwake);
				_torqueAwake = Vector3.zero;
			}
			if (_isSleeping)
			{
				_sleepPriority = priority;
			}
			else
			{
				_sleepPriority = -1;
			}
		}

		public void IgnoreCollisionWithRagdoll(SkeletonRagdoll skelRagdoll)
		{
			if (_hasIgnoredColliders || skelRagdoll._hasIgnoredColliders || mirroringDuplicateIndex == skelRagdoll.mirroringDuplicateIndex || mirroringDuplicateIndex == -1 || skelRagdoll.mirroringDuplicateIndex == -1)
			{
				return;
			}
			Collider[] colliders = _colliders;
			foreach (Collider collider in colliders)
			{
				Collider[] colliders2 = skelRagdoll._colliders;
				foreach (Collider collider2 in colliders2)
				{
					Physics.IgnoreCollision(collider, collider2);
				}
			}
			_hasIgnoredColliders = true;
		}
	}
}
