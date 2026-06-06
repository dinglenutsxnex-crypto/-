using Nekki.Yaml;
using SF3;
using SF3.Moves;

public class TriggerActionSetFightSettings : TriggerAction
{
	private readonly bool _showFightStart;

	private readonly bool _isHpFight;

	private readonly bool _isScoreFight;

	private readonly int _scoreCount;

	private readonly bool _isTimeoutWin;

	public TriggerActionSetFightSettings(Node node)
		: base(EActionType.SET_FIGHT_SETTINGS, node)
	{
		TryGetBool(out _showFightStart, "ShowFightStart", true, string.Empty, null, false);
		TryGetBool(out _isHpFight, "IsHpFight", true, string.Empty, null, false);
		TryGetBool(out _isScoreFight, "IsScoreFight", false, string.Empty, null, false);
		TryGetInt(out _scoreCount, "ScoreCount", 0, string.Empty, null, false);
		TryGetBool(out _isTimeoutWin, "IsTimeoutWin", false, string.Empty, null, false);
	}

	protected override void ApplyAction(object modelData)
	{
		base.ApplyAction(modelData);
		FightController.Settings.ShowFightStart = _showFightStart;
		FightController.Settings.IsHpFight = _isHpFight;
		FightController.Settings.IsScoreFight = _isScoreFight;
		FightController.Settings.ScoreCount = _scoreCount;
		FightController.Settings.IsTimeoutWin = _isTimeoutWin;
	}
}
