using Nekki.Yaml;
using SF3.GameModels;

namespace SF3.Moves
{
	public class TriggerActionSetHit : TriggerAction
	{
		private readonly RpnValue<bool> _dealsDamage;

		public TriggerActionSetHit(Node yamlNode)
			: base(EActionType.SET_HIT, yamlNode)
		{
			string outResult;
			if (TryGetString(out outResult, "DealsDamage", string.Empty, string.Empty, this))
			{
				_dealsDamage = outResult;
			}
		}

		protected override void ApplyAction(object modelData)
		{
			Model.hitResult.CanDealDamage = _dealsDamage;
			((Model)modelData).modelAnimation.ResumeForced();
		}
	}
}
