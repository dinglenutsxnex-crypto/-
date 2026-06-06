using System;
using UnityEngine;

namespace SF3.VerlePhysics
{
	[Serializable]
	public class Edge
	{
		public Node[] nodes = new Node[2];

		public Vector3[] ends = new Vector3[2];

		public float[] mk = new float[2];

		public float length;

		public Edge(Node node1, Node node2)
		{
			nodes[0] = node1;
			nodes[1] = node2;
			ends[0] = default(Vector3);
			ends[1] = default(Vector3);
			node1.AddEnd(ends[0]);
			node2.AddEnd(ends[1]);
			CalcMK();
			RecalculateLength();
		}

		public void CalcMK()
		{
			float num = nodes[0].nowMass + nodes[1].nowMass;
			for (int i = 0; i < 2; i++)
			{
				mk[i] = nodes[i].nowMass / num;
			}
		}

		public void RecalculateLength()
		{
			length = (nodes[0].position - nodes[1].position).magnitude;
			if (length == 0f)
			{
				length = 0f;
				Debug.LogError("Zero length edges are bad for this engine!");
			}
		}
	}
}
