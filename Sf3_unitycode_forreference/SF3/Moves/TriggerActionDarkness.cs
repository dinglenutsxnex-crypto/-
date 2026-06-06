using System.Collections.Generic;
using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerActionDarkness : TriggerActionRoundResetable
	{
		private const string DarkFrames = "DarkFrames";

		private const string LightFrames = "LightFrames";

		private const string TransitionFrames = "TransitionFrames";

		private readonly string[] _paramNames = new string[3] { "DarkFrames", "LightFrames", "TransitionFrames" };

		private readonly Dictionary<string, string> _params;

		public TriggerActionDarkness(Node yamlNode)
			: base(EActionType.DARKNESS, yamlNode)
		{
			_params = new Dictionary<string, string>();
			string[] paramNames = _paramNames;
			foreach (string key in paramNames)
			{
				string outResult;
				TryGetString(out outResult, key, string.Empty, string.Empty);
				_params.Add(key, outResult);
			}
		}

		protected override void ApplyAction(object modelData)
		{
			base.ApplyAction(modelData);
			CameraDarkness.Instance.SetupDarkness(GetValue("DarkFrames"), GetValue("LightFrames"), GetValue("TransitionFrames"));
		}

		public override void Reset()
		{
			CameraDarkness.Instance.StopDarkness();
		}

		private float GetValue(string valueName)
		{
			return new RpnValue<float>(_params[valueName]);
		}
	}
}
