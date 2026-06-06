using UnityEngine;

namespace SF3.VerlePhysics
{
	internal static class ElementIterator
	{
		public static void IterateNodes(Node[] nodes)
		{
			foreach (Node node in nodes)
			{
				if (node.HasEnds)
				{
					Vector3 vector = default(Vector3);
					for (int j = 0; j < node.endArray.Length; j++)
					{
						vector += node.endArray[j];
					}
					if (float.IsNaN(vector[0]))
					{
					}
					node.position = 1f / (float)node.endArray.Length * vector;
					for (int k = 0; k < node.endArray.Length; k++)
					{
						node.endArray[k] = node.position;
					}
				}
			}
		}

		public static void IterateEdges(Edge[] edges)
		{
			foreach (Edge edge in edges)
			{
				Vector3 vector = edge.mk[0] * edge.nodes[0].position + edge.mk[1] * edge.nodes[1].position;
				float magnitude = (edge.nodes[0].position - edge.nodes[1].position).magnitude;
				float num = ((!(magnitude > 0f)) ? 1f : (edge.length / magnitude));
				Vector3 vector2 = (1f - num) * vector;
				edge.ends[0] = num * edge.nodes[0].position + vector2;
				edge.ends[1] = num * edge.nodes[1].position + vector2;
				if (Mathf.Abs(edge.ends[0][1]) > 100000f || float.IsNaN(edge.ends[0][1]))
				{
				}
			}
		}
	}
}
