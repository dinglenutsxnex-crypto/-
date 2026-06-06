using SF3.GameModels;
using UnityEngine;

namespace SF3.VerlePhysics
{
	public class BoneController
	{
		protected Node[] _nodes;

		protected Bone _bone;

		protected Matrix4x4 _bindingMatrix;

		public Node[] nodes
		{
			get
			{
				return _nodes;
			}
		}

		public virtual string BoneName
		{
			get
			{
				return _bone.boneName;
			}
		}

		public Matrix4x4 BindingMatrix
		{
			get
			{
				return _bindingMatrix;
			}
		}

		protected Matrix4x4 NodeMatrix
		{
			get
			{
				return ControllerUtils.GenerateMatrixFrom3Nodes(_nodes[0], _nodes[1], _nodes[2]);
			}
		}

		public BoneController(Node node1, Node node2, Node node3)
			: this(node1, node1, node3, null)
		{
		}

		public BoneController(Node node1, Node node2, Node node3, Bone bone)
		{
			_nodes = new Node[3];
			_nodes[0] = node1;
			_nodes[1] = node2;
			_nodes[2] = node3;
			_bone = bone;
			_bindingMatrix = NodeMatrix.inverse * bone.transform.localToWorldMatrix;
		}

		public void SetBonePosition()
		{
			if (!_bone.animatedThisFrame)
			{
				Matrix4x4 nodeMatrix = NodeMatrix;
				nodeMatrix *= _bindingMatrix;
				Quaternion rotation = default(Quaternion);
				Vector3 scale = default(Vector3);
				Vector3 position;
				ControllerUtils.ExtractTransform(nodeMatrix, out position, out rotation, out scale);
				_bone.transform.rotation = rotation;
				_bone.transform.position = position;
			}
		}
	}
}
