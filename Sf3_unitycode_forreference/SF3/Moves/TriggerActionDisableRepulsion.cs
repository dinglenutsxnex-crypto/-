using Nekki.Yaml;
using SF3.GameModels;

namespace SF3.Moves
{
	public class TriggerActionDisableRepulsion : TriggerAction
	{
		private readonly bool _disableRepulsion;

		public TriggerActionDisableRepulsion(Node yamlNode)
			: base(EActionType.REPULSION_TYPE, yamlNode)
		{
			TryGetBool(out _disableRepulsion, "State", false, string.Empty);
		}

		protected override void ApplyAction(object modelData)
		{
			((Model)modelData).modelCollisions.EnableRepulsionCollisions(!_disableRepulsion);
		}
	}
}
