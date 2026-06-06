using System;
using Nekki.Yaml;

namespace SF3.GameModels
{
	public class ModelStatusPrototype
	{
		public string name { get; protected set; }

		public ModelStatus.EStatusType statusType { get; protected set; }

		public RpnValue<float> duration { get; protected set; }

		public bool stackable { get; protected set; }

		protected ModelStatusPrototype(ModelStatus.EStatusType type, Mapping mapping)
		{
			statusType = type;
			FromYAML(mapping);
		}

		public static ModelStatusPrototype CreateInstance(Mapping mapping)
		{
			Scalar text = mapping.GetText("Type");
			if (text != null)
			{
				ModelStatus.EStatusType eStatusType = (ModelStatus.EStatusType)Enum.Parse(typeof(ModelStatus.EStatusType), text.text, true);
				if (eStatusType == ModelStatus.EStatusType.MODHP)
				{
					return new ModelStatusHPPrototype(eStatusType, mapping);
				}
			}
			return null;
		}

		protected virtual void FromYAML(Mapping mapping)
		{
			Scalar text = mapping.GetText("Name");
			if (text != null)
			{
				name = text.text;
			}
			text = mapping.GetText("Frames");
			if (text != null)
			{
				duration = text.text;
			}
			else
			{
				duration = -1f;
			}
			stackable = false;
			text = mapping.GetText("Stackable");
			if (text != null)
			{
				stackable = int.Parse(text.text) == 1;
			}
		}
	}
}
