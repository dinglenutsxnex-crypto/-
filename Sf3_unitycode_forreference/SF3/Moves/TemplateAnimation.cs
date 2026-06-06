using System;
using System.Collections.Generic;
using System.Xml;

namespace SF3.Moves
{
	[Serializable]
	public class TemplateAnimation
	{
		private string _name;

		private List<InfoAnimation> _children;

		public TemplateAnimation()
		{
			_children = new List<InfoAnimation>();
			_name = string.Empty;
		}

		public string getName()
		{
			return _name;
		}

		public List<InfoAnimation> getChildren()
		{
			return _children;
		}

		public void setName(string name)
		{
			_name = name;
		}

		public void addChild(InfoAnimation animation)
		{
			if (!_children.Contains(animation))
			{
				_children.Add(animation);
			}
		}

		public void parse(XmlElement node)
		{
			XmlAttribute attributeNode = node.GetAttributeNode("Name");
			string name = string.Empty;
			if (attributeNode != null)
			{
				name = attributeNode.Value;
			}
			setName(name);
		}
	}
}
