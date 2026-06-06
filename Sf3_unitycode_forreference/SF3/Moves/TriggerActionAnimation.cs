using Nekki.Yaml;
using SF3.GameModels;

namespace SF3.Moves
{
	public class TriggerActionAnimation : TriggerAction
	{
		public TriggerActionAnimation(Node yamNode)
			: base(EActionType.ANIMATION, yamNode)
		{
			base.name = base.name.ToLower();
		}

		protected override void ApplyAction(object modelData)
		{
			((Model)modelData).modelAnimation.Play(base.name);
		}
	}
}
