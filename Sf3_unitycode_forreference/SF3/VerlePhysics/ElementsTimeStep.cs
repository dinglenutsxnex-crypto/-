using UnityEngine;

namespace SF3.VerlePhysics
{
	internal static class ElementsTimeStep
	{
		private const float FixedMassMult = 100000f;

		public static void physicNodeTimeStep(Node[] nodes, float oldDelta, float nowDelta, float gravity)
		{
			float num = 0f;
			if (oldDelta > 0f)
			{
				num = nowDelta / oldDelta;
			}
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			foreach (Node node in nodes)
			{
				zero = node.position;
				zero2 = (node.position - node.prevPosition) * num * node.tenuation;
				node.position = zero + zero2 + gravity * nowDelta * Vector3.down;
				if (float.IsNaN(node.position[0]))
				{
				}
				node.nowMass = node.mass;
				node.prevPosition = zero;
			}
		}

		public static void jointNodeTimeStep(Node[] nodes)
		{
			for (int i = 0; i < nodes.Length; i++)
			{
				if (nodes[i].bone.animatedThisFrame)
				{
					Node node = nodes[i];
					node.position = node.bone.transform.localToWorldMatrix * node.localPosition;
					node.nowMass = 100000f * node.mass;
				}
			}
		}

		public static void edgeTimeStep(Edge[] edges)
		{
			for (int i = 0; i < edges.Length; i++)
			{
				edges[i].CalcMK();
			}
		}

		public static void jointTimeStep(BoneController[] controllers)
		{
			for (int i = 0; i < controllers.Length; i++)
			{
				controllers[i].SetBonePosition();
			}
		}

		public static void jointTimeStep(ChainController[] controllers)
		{
			for (int i = 0; i < controllers.Length; i++)
			{
				controllers[i].SetBonesPositions();
			}
		}
	}
}
