using Nekki.Yaml;
using SF3.GameModels;

namespace SF3.Moves
{
	public class TriggerActionSetVariable : TriggerAction
	{
		private readonly RpnParser.Formula _formula;

		private readonly RpnValue<int> _frames;

		public TriggerActionSetVariable(Node yamlNode)
			: base(EActionType.SET_VARIABLE, yamlNode)
		{
			string outResult;
			if (TryGetString(out outResult, "Value", string.Empty, string.Empty, this))
			{
				_formula = new RpnParser.Formula(outResult);
			}
			int outResult2;
			TryGetInt(out outResult2, "Frames", -1, string.Empty, null, false);
			_frames = outResult2;
		}

		protected override void ApplyAction(object modelData)
		{
			GameVariables.AddVariable(((Model)modelData).id, base.name, _formula.calculate(), _frames);
		}
	}
}
