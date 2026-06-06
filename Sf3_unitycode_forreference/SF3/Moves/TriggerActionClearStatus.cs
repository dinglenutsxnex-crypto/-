using Nekki.Yaml;
using SF3.GameModels;

namespace SF3.Moves
{
	public class TriggerActionClearStatus : TriggerAction
	{
		public TriggerActionClearStatus(Node yamlNode)
			: base(EActionType.CLEAR_STATUS, yamlNode)
		{
		}

		protected override void ApplyAction(object modelData)
		{
			((Model)modelData).statusControl.ClearStatus(base.name);
		}
	}
}
