using System;
using UnityEngine;

namespace SF3.GameModels
{
	[Serializable]
	public class ModelPhysics
	{
		private SkeletonRagdoll _mainRagdoll;

		private bool _ragdollActive;

		private SetupRagdoll _setupRagdoll;

		public bool ragdollActive
		{
			get
			{
				return _ragdollActive;
			}
		}

		public ModelPhysics(GameObject modelObject, IBonesHolder bonesGolder)
		{
			_mainRagdoll = null;
			_ragdollActive = false;
			_setupRagdoll = modelObject.GetComponentInChildren<SetupRagdoll>();
		}

		public void Initialize(ModelComponents modelComps)
		{
			_mainRagdoll = null;
			_ragdollActive = false;
			foreach (ModelObject commonEquippedItem in modelComps.commonEquippedItems)
			{
				if (commonEquippedItem != null)
				{
					foreach (SkeletonObject skeleton in commonEquippedItem.skeletons)
					{
						if (skeleton.skeletonRagdoll != null)
						{
							_mainRagdoll = skeleton.skeletonRagdoll;
							break;
						}
					}
				}
				if (_mainRagdoll != null)
				{
					break;
				}
			}
			if (_mainRagdoll == null)
			{
				foreach (ModelObject value in modelComps.equippedItems.Values)
				{
					if (value == null)
					{
						continue;
					}
					foreach (SkeletonObject skeleton2 in value.skeletons)
					{
						if (skeleton2.skeletonRagdoll != null && !skeleton2.additionalPart)
						{
							_mainRagdoll = skeleton2.skeletonRagdoll;
						}
					}
				}
			}
			_mainRagdoll.SetRagdollSleepState(false, 2);
			_mainRagdoll.SetActive(false);
		}

		public void SetToRagdollPose()
		{
			if (_setupRagdoll != null)
			{
				_setupRagdoll.SetRagdollPose();
			}
		}

		public void SetToTPose()
		{
			if (_setupRagdoll != null)
			{
				_setupRagdoll.SetTPose();
			}
		}

		public void EnableColliders(bool enable)
		{
			_mainRagdoll.EnableColliders(enable);
		}

		public void SetRagdollActive(bool isActive)
		{
			if (_ragdollActive != isActive)
			{
				_ragdollActive = isActive;
				_mainRagdoll.SetActive(_ragdollActive);
			}
		}

		public void SetRagdollSleepState(bool isSleep, int priority)
		{
			if (_ragdollActive)
			{
				_mainRagdoll.SetRagdollSleepState(isSleep, priority);
			}
		}

		public void AddForce(Vector3 impulse, string capsuleName)
		{
			_mainRagdoll.AddForce(impulse, capsuleName);
		}

		public void SetIsTriggerActive(bool activate)
		{
			_mainRagdoll.SetIsTriggerActive(activate);
		}
	}
}
