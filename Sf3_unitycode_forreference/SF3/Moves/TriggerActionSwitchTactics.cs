using Nekki.Yaml;
using sf3DTO;

namespace SF3.Moves
{
	public class TriggerActionSwitchTactics : TriggerAction
	{
		private const string Enemy = "Enemy";

		private readonly AiMode _mode;

		private readonly string _target;

		public TriggerActionSwitchTactics(Node yamlNode)
			: base(EActionType.SWITCH_TACTICS, yamlNode)
		{
			string outResult;
			if (TryGetString(out outResult, "Mode", string.Empty, string.Empty, this))
			{
				AiMode outParam;
				if (!SF3Utils.TryParseEnum(out outParam, outResult, AiMode.NoneMode))
				{
					Messenger.Error(string.Format("Failed to parse tactics mode for trigger. Parsed value [{0}] is not supported.", outResult), this);
				}
				_mode = outParam;
			}
			TryGetString(out _target, "Target", "Enemy", string.Empty, null, false);
		}

		protected override void ApplyAction(object modelData)
		{
			base.ApplyAction(modelData);
			if ("Enemy".Equals(_target))
			{
				ModelsManager.Instance.Enemy.SetAiMode(_mode);
			}
			else
			{
				ModelsManager.Instance.Player.SetAiMode(_mode);
			}
		}
	}
}
