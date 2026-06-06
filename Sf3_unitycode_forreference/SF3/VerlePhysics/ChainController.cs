using System.Collections.Generic;
using SF3.GameModels;
using UnityEngine;

namespace SF3.VerlePhysics
{
	public class ChainController
	{
		public class ChainLink
		{
			public Node myNode;

			public Node nextNode;

			public Bone myBone;

			public Transform myTransform;

			public Matrix4x4 bindingMatrix;

			public Quaternion calculatedRotation;

			public Vector3 calculatedPosition;

			public ChainLink(Node node1, Node node2, Bone bone)
			{
				myNode = node1;
				nextNode = node2;
				myBone = bone;
				myTransform = bone.transform;
			}
		}

		public Transform baseRotationBone;

		private List<ChainLink> links = new List<ChainLink>();

		private bool initializedCorrectly;

		public List<ChainLink> Links
		{
			get
			{
				return links;
			}
		}

		public string MainBoneName
		{
			get
			{
				return baseRotationBone.name;
			}
		}

		public ChainController(Transform basicBone, List<Node> nodes, List<Bone> bones)
		{
			baseRotationBone = basicBone;
			int count = nodes.Count;
			int count2 = bones.Count;
			if (count2 == count - 1)
			{
				for (int i = 0; i < bones.Count; i++)
				{
					ChainLink item = new ChainLink(nodes[i], nodes[i + 1], bones[i]);
					links.Add(item);
				}
				RecalculateBindingMatrices();
				initializedCorrectly = true;
			}
		}

		protected void RecalculateBindingMatrices()
		{
			CalculateLinksFromNodes();
			Matrix4x4 m = default(Matrix4x4);
			for (int i = 0; i < links.Count; i++)
			{
				m.SetTRS(links[i].calculatedPosition, links[i].calculatedRotation, Vector3.one);
				links[i].bindingMatrix = Matrix4x4.Inverse(m) * links[i].myTransform.localToWorldMatrix;
			}
		}

		private void CalculateLinksFromNodes()
		{
			Vector3 fromDirection = baseRotationBone.TransformDirection(Vector3.right);
			Quaternion quaternion = baseRotationBone.rotation;
			for (int i = 0; i < links.Count; i++)
			{
				ChainLink chainLink = links[i];
				Vector3 vector = chainLink.nextNode.position - chainLink.myNode.position;
				Quaternion quaternion2 = Quaternion.FromToRotation(fromDirection, vector);
				Quaternion quaternion3 = quaternion2 * quaternion;
				chainLink.calculatedPosition = chainLink.myNode.position;
				chainLink.calculatedRotation = quaternion3;
				fromDirection = vector;
				quaternion = quaternion3;
			}
		}

		public void SetBonesPositions()
		{
			if (!initializedCorrectly)
			{
				return;
			}
			for (int i = 0; i < links.Count; i++)
			{
				if (links[i].myBone.animatedThisFrame)
				{
					return;
				}
			}
			CalculateLinksFromNodes();
			Matrix4x4 matrix4x = default(Matrix4x4);
			for (int j = 0; j < links.Count; j++)
			{
				matrix4x.SetTRS(links[j].calculatedPosition, links[j].calculatedRotation, Vector3.one);
				Matrix4x4 m = matrix4x * links[j].bindingMatrix;
				Vector3 position;
				Quaternion rotation;
				Vector3 scale;
				ControllerUtils.ExtractTransform(m, out position, out rotation, out scale);
				links[j].myTransform.rotation = rotation;
				links[j].myTransform.position = position;
			}
		}
	}
}
