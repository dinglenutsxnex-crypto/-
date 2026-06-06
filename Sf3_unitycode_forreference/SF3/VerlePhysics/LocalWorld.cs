using System.Collections.Generic;
using System.Xml;
using SF3.GameModels;

namespace SF3.VerlePhysics
{
	public class LocalWorld
	{
		protected List<Node> nodes = new List<Node>();

		protected List<Edge> edges = new List<Edge>();

		protected List<BoneController> controllers = new List<BoneController>();

		protected List<ChainController> chains = new List<ChainController>();

		public List<Node> getNodes
		{
			get
			{
				return nodes;
			}
		}

		public List<Edge> getEdges
		{
			get
			{
				return edges;
			}
		}

		public List<BoneController> getControllers
		{
			get
			{
				return controllers;
			}
		}

		public List<ChainController> getChains
		{
			get
			{
				return chains;
			}
		}

		public LocalWorld()
		{
		}

		public LocalWorld(XmlElement node, Model model)
		{
			if (node == null)
			{
				return;
			}
			XmlElement xmlElement = (XmlElement)node.SelectSingleNode("Nodes");
			XmlNodeList xmlNodeList = xmlElement.SelectNodes("Node");
			foreach (XmlElement item5 in xmlNodeList)
			{
				Node item = ElementsDeserialization.CreateNode(item5, model);
				nodes.Add(item);
			}
			XmlElement xmlElement2 = (XmlElement)node.SelectSingleNode("Edges");
			XmlNodeList xmlNodeList2 = xmlElement2.SelectNodes("Edge");
			foreach (XmlElement item6 in xmlNodeList2)
			{
				Edge item2 = ElementsDeserialization.CreateEdge(item6, nodes);
				edges.Add(item2);
			}
			XmlElement xmlElement3 = (XmlElement)node.SelectSingleNode("Controllers");
			XmlNodeList xmlNodeList3 = xmlElement3.SelectNodes("Controller");
			foreach (XmlElement item7 in xmlNodeList3)
			{
				BoneController item3 = ElementsDeserialization.CreateController(item7, nodes, model);
				controllers.Add(item3);
			}
			XmlElement xmlElement4 = (XmlElement)node.SelectSingleNode("Chains");
			XmlNodeList xmlNodeList4 = xmlElement4.SelectNodes("Controller");
			foreach (XmlElement item8 in xmlNodeList4)
			{
				ChainController item4 = ElementsDeserialization.CreateChainController(item8, nodes, model);
				chains.Add(item4);
			}
		}

		public void AddNode(Node newNode)
		{
			nodes.Add(newNode);
		}

		public void AddEdge(Edge newEdge)
		{
			edges.Add(newEdge);
		}

		public void AddController(BoneController newController)
		{
			controllers.Add(newController);
		}

		public void AddChainController(ChainController newChainController)
		{
			chains.Add(newChainController);
		}

		public void RecalculateEdgesLength()
		{
			foreach (Edge edge in edges)
			{
				edge.RecalculateLength();
			}
		}
	}
}
