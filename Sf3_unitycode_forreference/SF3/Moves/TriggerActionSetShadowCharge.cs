using System;
using Nekki.Yaml;
using SF3.GameModels;

namespace SF3.Moves
{
	public class TriggerActionSetShadowCharge : TriggerAction
	{
		private readonly RpnParser.Formula _formula;

		public TriggerActionSetShadowCharge(Node yamlNode)
			: base(EActionType.SET_SHADOW_CHARGE, yamlNode)
		{
			string outResult;
			if (TryGetString(out outResult, "Value", string.Empty, string.Empty, this))
			{
				_formula = new RpnParser.Formula(outResult);
			}
		}

		protected override void ApplyAction(object modelData)
		{
			ShadowFormController.Instance.SetShadowCharge(((Model)modelData).id, Convert.ToSingle(_formula.calculate()));
		}
	}
}
