using Nekki.Yaml;
using SF3.Effects;

namespace SF3.Moves
{
	public class TriggerActionFreezeFrame : TriggerAction
	{
		private readonly RpnValue<int> _duration;

		public int duration
		{
			get
			{
				return _duration;
			}
		}

		public TriggerActionFreezeFrame(Node yamlNode)
			: base(EActionType.FREEZE_FRAME, yamlNode)
		{
			_duration = 0;
			int outResult;
			if (!(yamlNode is Scalar) && TryGetInt(out outResult, "Frames", 0, string.Empty, this))
			{
				_duration = outResult;
			}
		}

		protected override void ApplyAction(object modelData)
		{
			base.ApplyAction(modelData);
			EffectsManager.PlayEffect(base.name, this);
		}
	}
}
