using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerEventPostSceneChange : TriggerEvent
	{
		private readonly string _from;

		private readonly string _to;

		public TriggerEventPostSceneChange(Mapping eventMap)
			: base(ETriggerEvents.QEVENT_POST_SCENE_CHANGE, eventMap)
		{
			if (eventMap != null)
			{
				if (TryGetString(out _from, "From", string.Empty, string.Empty, this))
				{
					_from = _from.ToLower();
				}
				if (TryGetString(out _to, "To", string.Empty, string.Empty, this))
				{
					_to = _to.ToLower();
				}
			}
		}

		protected override bool Equal()
		{
			IntentTransitionData intentTransitionData = (IntentTransitionData)arguments[0];
			string text = intentTransitionData.FromData.Name.ToLower();
			string text2 = intentTransitionData.ToData.Name.ToLower();
			if (!string.IsNullOrEmpty(_from) && !text.Equals(_from))
			{
				return false;
			}
			return string.IsNullOrEmpty(_to) || text2.Equals(_to);
		}
	}
}
