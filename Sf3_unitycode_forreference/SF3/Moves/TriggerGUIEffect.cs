using System;
using System.Collections.Generic;
using Nekki.Yaml;
using SF3.Effects;
using SF3.GameModels;

namespace SF3.Moves
{
	public class TriggerGUIEffect : TriggerAction
	{
		private readonly List<RpnParser.Formula> _formulas = new List<RpnParser.Formula>();

		private readonly string _alias;

		private string[] Values
		{
			get
			{
				string[] array = new string[_formulas.Count];
				for (int i = 0; i < _formulas.Count; i++)
				{
					array[i] = Convert.ToString(_formulas[i].calculate());
				}
				return array;
			}
		}

		public TriggerGUIEffect(Node yamlNode)
			: base(EActionType.GUI_EFFECT, yamlNode)
		{
			Mapping outResult;
			if (!TryGetMapping(out outResult, "Text", string.Empty, null, false))
			{
				return;
			}
			_alias = outResult.GetText("Alias").text;
			for (int i = 1; i < outResult.nodesInside.Count; i++)
			{
				Scalar text = outResult.GetText("Value" + i);
				if (text != null)
				{
					RpnParser.Formula item = new RpnParser.Formula(text.text);
					_formulas.Add(item);
				}
			}
		}

		protected override void ApplyAction(object modelData)
		{
			if (!Model.disableEffects)
			{
				EffectsManager.PlayEffect((Model)modelData, base.name, ((Model)modelData).moveControl.positionSign == -1, _alias, Values, ((Model)modelData).id);
			}
		}
	}
}
