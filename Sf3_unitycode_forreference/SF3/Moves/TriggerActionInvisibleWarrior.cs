using Nekki.Yaml;
using SF3.GameModels;

namespace SF3.Moves
{
	public class TriggerActionInvisibleWarrior : TriggerActionRoundResetable
	{
		public TriggerActionInvisibleWarrior(Node yamlNode)
			: base(EActionType.INVISIBLE_WARRIOR, yamlNode)
		{
		}

		protected override void ApplyAction(object modelData)
		{
			((Model)modelData).Fade();
		}

		public override void Reset()
		{
			ModelsManager.Instance.ShowModels();
		}
	}
}
