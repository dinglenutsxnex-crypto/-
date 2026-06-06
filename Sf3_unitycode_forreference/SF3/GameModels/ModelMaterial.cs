using System.Collections.Generic;
using System.Xml;

namespace SF3.GameModels
{
	public class ModelMaterial
	{
		public string capsuleName { get; private set; }

		public string[] vertexNames { get; private set; }

		public string materialName { get; private set; }

		public ModelMaterial(XmlElement materialNode)
		{
			materialName = materialNode.GetAttribute("Name");
			capsuleName = materialNode.GetAttribute("CapsuleName");
			List<string> list = new List<string>();
			foreach (XmlElement childNode in materialNode.ChildNodes)
			{
				list.Add(childNode.GetAttribute("Name"));
			}
			vertexNames = list.ToArray();
		}
	}
}
