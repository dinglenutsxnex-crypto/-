using Nekki.Yaml;
using SF3;
using SF3.Moves;
using UnityEngine;
using sf3DTO;

public class TriggerEventQuestFightEnd : TriggerEvent
{
	private readonly int _battleId;

	private readonly int _fightId;

	public sf3DTO.FightResult _result;

	private int battleID;

	private int fightID;

	private sf3DTO.FightResult result;

	public TriggerEventQuestFightEnd(Mapping eventMap)
		: base(ETriggerEvents.QEVENT_FIGHT_END, eventMap)
	{
		_battleId = -1;
		_fightId = -1;
		_result = sf3DTO.FightResult.UnknownFightResult;
		string outResult;
		if (TryGetString(out outResult, "Result", string.Empty, string.Empty, null, false))
		{
			SF3Utils.TryParseEnum(out _result, outResult, sf3DTO.FightResult.UnknownFightResult);
		}
		int[] outResult2;
		if (TryGetString(out outResult, "ID", string.Empty, string.Empty, null, false) && TryGetBattleIdentifier(out outResult2, outResult, string.Empty, this))
		{
			_battleId = outResult2[0];
			_fightId = outResult2[1];
		}
	}

	protected override void SetArguments(object[] args)
	{
		base.SetArguments(args);
		battleID = (int)arguments[0];
		fightID = (int)arguments[1];
		result = (sf3DTO.FightResult)arguments[2];
	}

	protected override bool Equal()
	{
		bool flag = _battleId == -1 || (battleID == _battleId && fightID == _fightId);
		bool flag2 = _result == sf3DTO.FightResult.UnknownFightResult || _result == result;
		return flag && flag2;
	}

	public override object GetArgument(string field)
	{
		switch (field)
		{
		case "BattleID":
			return battleID;
		case "FightID":
			return fightID;
		case "ID":
			return battleID + "." + (fightID + 1);
		case "Result":
			return result.ToString().ToLower();
		default:
			Debug.LogError("GetArgument Unknown field" + field);
			return base.GetArgument(field);
		}
	}
}
