using Nekki.Yaml;
using SF3;
using SF3.Moves;

public class TriggerActionLose : TriggerAction
{
	public TriggerActionLose(Node yamlNode)
		: base(EActionType.LOSE, yamlNode)
	{
	}

	protected override void ApplyAction(object modelData)
	{
		RoundController.Instance.SetRoundWinner(ERoundResult.ENEMY_WIN);
	}
}
