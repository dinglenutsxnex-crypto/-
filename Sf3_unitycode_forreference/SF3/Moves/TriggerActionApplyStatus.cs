using Nekki.Yaml;
using SF3.GameModels;

namespace SF3.Moves
{
	public class TriggerActionApplyStatus : TriggerAction
	{
		private readonly ModelStatusPrototype _modelStatusPrototype;

		public TriggerActionApplyStatus(Node yamlNode)
			: base(EActionType.APPLY_STATUS, yamlNode)
		{
			_modelStatusPrototype = ModelStatusPrototype.CreateInstance(BaseMapping);
		}

		protected override void ApplyAction(object modelData)
		{
			((Model)modelData).statusControl.ApplyStatus(_modelStatusPrototype);
		}
	}
}
