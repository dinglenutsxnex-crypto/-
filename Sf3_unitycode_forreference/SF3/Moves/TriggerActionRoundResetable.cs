using Nekki.Yaml;

namespace SF3.Moves
{
	public abstract class TriggerActionRoundResetable : TriggerAction
	{
		protected TriggerActionRoundResetable(EActionType type, Node node)
			: base(type, node)
		{
		}

		protected override void ApplyAction(object modelData)
		{
			base.ApplyAction(modelData);
			RoundResetableManager.Instance.AddRule(this);
		}

		public abstract void Reset();
	}
}
