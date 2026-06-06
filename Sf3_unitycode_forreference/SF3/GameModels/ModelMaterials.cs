using System.Collections.Generic;
using System.Xml;

namespace SF3.GameModels
{
	public class ModelMaterials
	{
		public ModelMaterial[] modelMaterials { get; private set; }

		public ModelMaterials()
		{
			modelMaterials = new ModelMaterial[0];
		}

		public void Parse(XmlNode materialsNode)
		{
			List<ModelMaterial> list = new List<ModelMaterial>();
			foreach (XmlNode childNode in materialsNode.ChildNodes)
			{
				list.Add(new ModelMaterial((XmlElement)childNode));
			}
			modelMaterials = list.ToArray();
		}
	}
}
