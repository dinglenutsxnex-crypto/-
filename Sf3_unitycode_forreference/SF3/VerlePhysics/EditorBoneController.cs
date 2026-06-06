using System;
using UnityEngine;

namespace SF3.VerlePhysics
{
	[Serializable]
	internal class EditorBoneController : BoneController
	{
		private Transform elementTransform;

		public override string BoneName
		{
			get
			{
				return elementTransform.name;
			}
		}

		public EditorBoneController(Node node1, Node node2, Node node3, Transform transform)
			: base(node1, node2, node3)
		{
			elementTransform = transform;
			RecalculateBindingMatrix();
		}

		public void RecalculateBindingMatrix()
		{
			Matrix4x4 nodeMatrix = base.NodeMatrix;
			_bindingMatrix = elementTransform.localToWorldMatrix * Matrix4x4.Inverse(nodeMatrix);
		}
	}
}
