using System;
using UnityEngine;

namespace SF3.GameModels
{
	[Serializable]
	public class CollisionVertex
	{
		public Vector3 _offset;

		public Bone bone { get; private set; }

		public Transform getTransform
		{
			get
			{
				return bone.transform;
			}
		}

		public string name { get; private set; }

		public Vector3 previousPosition { get; private set; }

		public Vector3 position
		{
			get
			{
				if (_offset == Vector3.zero)
				{
					return bone.position;
				}
				return bone.transform.TransformPoint(_offset);
			}
		}

		public CollisionVertex(Bone newBone, Vector3 newOffset, string newName)
		{
			name = newName;
			bone = newBone;
			_offset = newOffset;
			bone.onPreviousPositionUpdate += delegate
			{
				if (_offset == Vector3.zero)
				{
					previousPosition = bone.position;
				}
				else
				{
					previousPosition = bone.transform.TransformPoint(_offset);
				}
			};
		}

		public CollisionVertex(Bone newBone)
			: this(newBone, Vector3.zero, "CollisionVertex")
		{
		}

		public CollisionVertex(Bone newBone, Vector3 newOffset)
			: this(newBone, newOffset, "CollisionVertex")
		{
		}
	}
}
