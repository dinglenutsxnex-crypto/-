using System;
using System.Collections.Generic;
using System.Xml;
using SF3.GameModels;
using UnityEngine;

namespace SF3.VerlePhysics
{
	internal static class ElementsDeserialization
	{
		public static Vector3 LoadPositionFromXml(XmlElement nodeXml)
		{
			Vector3 result = default(Vector3);
			result[0] = Convert.ToSingle(nodeXml.GetAttribute("X"));
			result[1] = Convert.ToSingle(nodeXml.GetAttribute("Y"));
			result[2] = Convert.ToSingle(nodeXml.GetAttribute("Z"));
			return result;
		}

		public static Node CreateNode(XmlElement node, Model model)
		{
			string attribute = node.GetAttribute("BoneName");
			Bone bone = model.GetBone(attribute);
			Vector3 localPos = LoadPositionFromXml(node);
			Node node2 = new Node(bone, localPos);
			float mass = Convert.ToSingle(node.GetAttribute("Mass"));
			node2.mass = mass;
			return node2;
		}

		public static Edge CreateEdge(XmlElement node, List<Node> nodes)
		{
			int index = Convert.ToInt32(node.GetAttribute("FirstNodeIndex"));
			int index2 = Convert.ToInt32(node.GetAttribute("SecondNodeIndex"));
			Node node2 = nodes[index];
			Node node3 = nodes[index2];
			Edge edge = new Edge(node2, node3);
			float num = Convert.ToSingle(node.GetAttribute("Length"));
			if (num == 0f)
			{
				Debug.LogError("Zero length edges are bad for this engine!");
			}
			edge.length = num;
			return edge;
		}

		public static BoneController CreateController(XmlElement node, List<Node> nodes, Model model)
		{
			int index = Convert.ToInt32(node.GetAttribute("NodeIndex1"));
			int index2 = Convert.ToInt32(node.GetAttribute("NodeIndex2"));
			int index3 = Convert.ToInt32(node.GetAttribute("NodeIndex3"));
			Node node2 = nodes[index];
			Node node3 = nodes[index2];
			Node node4 = nodes[index3];
			string attribute = node.GetAttribute("BoneName");
			Bone bone = model.GetBone(attribute);
			return new BoneController(node2, node3, node4, bone);
		}

		public static ChainController CreateChainController(XmlElement xnode, List<Node> nodes, Model model)
		{
			XmlElement xmlElement = (XmlElement)xnode.SelectSingleNode("ChainLinks");
			XmlNodeList xmlNodeList = xmlElement.SelectNodes("ChainLink");
			List<Node> list = new List<Node>();
			List<Bone> list2 = new List<Bone>();
			string attribute = xnode.GetAttribute("BoneName");
			Bone bone = model.GetBone(attribute);
			foreach (XmlElement item3 in xmlNodeList)
			{
				int index = Convert.ToInt32(item3.GetAttribute("MyNodeIndex"));
				Node item = nodes[index];
				list.Add(item);
				string attribute2 = item3.GetAttribute("BoneName");
				Bone bone2 = model.GetBone(attribute2);
				list2.Add(bone2);
			}
			int index2 = Convert.ToInt32(xnode.GetAttribute("LastNodeId"));
			Node item2 = nodes[index2];
			list.Add(item2);
			return new ChainController(bone.transform, list, list2);
		}
	}
}
