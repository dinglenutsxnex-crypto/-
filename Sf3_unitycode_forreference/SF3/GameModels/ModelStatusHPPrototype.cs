using Nekki.Yaml;

namespace SF3.GameModels
{
	public class ModelStatusHPPrototype : ModelStatusPrototype
	{
		public RpnValue<float> hpPerFrame { get; protected set; }

		public RpnValue<float> step { get; protected set; }

		public ModelStatusHPPrototype(ModelStatus.EStatusType type, Mapping mapping)
			: base(type, mapping)
		{
		}

		protected override void FromYAML(Mapping mapping)
		{
			base.FromYAML(mapping);
			Scalar text = mapping.GetText("HPPerFrame");
			if (text != null)
			{
				hpPerFrame = text.text;
			}
			text = mapping.GetText("Step");
			if (text != null)
			{
				step = text.text;
			}
			else
			{
				step = 0f;
			}
		}
	}
}
