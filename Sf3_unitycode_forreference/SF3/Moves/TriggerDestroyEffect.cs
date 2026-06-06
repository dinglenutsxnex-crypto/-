using Nekki.Yaml;
using SF3.Effects;
using SF3.GameModels;

namespace SF3.Moves
{
	public class TriggerDestroyEffect : TriggerAction
	{
		public TriggerDestroyEffect(Node yamlNode)
			: base(EActionType.DESTROY_EFFECT, yamlNode)
		{
		}

		protected override void ApplyAction(object modelData)
		{
			EffectsManager.StopAll(base.name, ((Model)modelData).id);
		}
	}
}
