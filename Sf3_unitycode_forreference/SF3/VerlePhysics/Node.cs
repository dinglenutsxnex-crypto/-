using System.Collections.Generic;
using SF3.GameModels;
using UnityEngine;

namespace SF3.VerlePhysics
{
	public class Node
	{
		public Vector3 position;

		public Vector3 prevPosition;

		public Vector4 localPosition;

		public float mass;

		public float nowMass;

		public float tenuation = 1f;

		public Bone bone;

		private List<Vector3> ends = new List<Vector3>();

		public Vector3[] endArray = new Vector3[0];

		private bool hasEnds;

		public virtual bool HasEnds
		{
			get
			{
				return hasEnds;
			}
		}

		public virtual string getBoneName
		{
			get
			{
				return bone.boneName;
			}
		}

		public virtual Transform getBoneTransform
		{
			get
			{
				return bone.transform;
			}
		}

		public Node(Bone myBone, Vector3 localPos)
		{
			bone = myBone;
			localPosition = localPos;
			localPosition[3] = 1f;
			position = bone.transform.localToWorldMatrix * localPosition;
			prevPosition = position;
			mass = 1f;
			nowMass = mass;
		}

		public Node(Bone myBone, Vector3 localPos, float mass)
			: this(myBone, localPos)
		{
			this.mass = mass;
			nowMass = mass;
		}

		public void AddEnd(Vector3 newEnd)
		{
			newEnd = position;
			ends.Add(newEnd);
			endArray = ends.ToArray();
			hasEnds = true;
		}
	}
}
