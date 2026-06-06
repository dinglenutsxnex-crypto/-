using Nekki.Yaml;
using SF3.Moves;
using UnityEngine;

public class TriggerEventQuestBattleSelectionStart : TriggerEvent
{
	private readonly int _battleId = -1;

	private readonly int _fightId = -1;

	private FightInfo _fightInfo;

	private IBattleInfo _battleInfo;

	public TriggerEventQuestBattleSelectionStart(Mapping eventMap)
		: this(eventMap, ETriggerEvents.QEVENT_BATTLE_SELECTION_START)
	{
	}

	protected TriggerEventQuestBattleSelectionStart(Mapping eventMap, ETriggerEvents type)
		: base(type, eventMap)
	{
		string outResult;
		int[] outResult2;
		if (TryGetString(out outResult, "FinishedFightID", string.Empty, string.Empty, null, false) && TryGetBattleIdentifier(out outResult2, outResult, string.Empty, this))
		{
			_battleId = outResult2[0];
			_fightId = outResult2[1];
		}
	}

	protected override void SetArguments(object[] args)
	{
		base.SetArguments(args);
		if (args != null && args.Length != 0)
		{
			_battleInfo = (IBattleInfo)arguments[0];
		}
	}

	protected override bool Equal()
	{
		if (_battleInfo == null)
		{
			return false;
		}
		_fightInfo = _battleInfo.GetCurrentFight();
		return _battleId == -1 || (_battleInfo.GetID() == _battleId && _fightInfo != null && _fightInfo.fightID == _fightId);
	}

	public override object GetArgument(string field)
	{
		int num = ((_fightInfo == null) ? (-1) : _fightInfo.fightID);
		switch (field)
		{
		case "BattleID":
			return _battleInfo.GetID();
		case "FightID":
			return num;
		case "ID":
		case "FinishedFightID":
			return _battleInfo.GetID() + "." + (num + 1);
		default:
			Debug.LogError(string.Format("GetArgument Unknown field: [{0}]", field));
			return base.GetArgument(field);
		}
	}
}
