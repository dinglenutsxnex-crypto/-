using System;
using System.Collections.Generic;
using System.Linq;
using Nekki;
using Nekki.Yaml;
using SimpleJSON;
using UnityEngine;
using sf3DTO;

public class GenericBattleInfo : IBattleInfo, ICloneable
{
	private List<BattleInfo> _battles;

	private int _currentBattleIndex;

	private int _battleCounter;

	private bool _available = true;

	private bool _hidden;

	private DateTime _expirationTime = DateTime.MinValue;

	private DateTime _genTime = DateTime.MinValue;

	private GenericBattleInfo()
	{
		_battles = new List<BattleInfo>();
		_battleCounter = 0;
		_currentBattleIndex = 0;
	}

	public GenericBattleInfo(List<BattleInfo> battleArr)
		: this()
	{
		_battles = battleArr;
	}

	public static GenericBattleInfo Create(Mapping battleMap)
	{
		GenericBattleInfo genericBattleInfo = new GenericBattleInfo();
		Sequence sequence = battleMap.GetSequence("Battles");
		foreach (Mapping item2 in sequence.nodesInside)
		{
			BattleInfo item = BattleInfo.Create(item2);
			genericBattleInfo._battles.Add(item);
		}
		genericBattleInfo.MergeWith(battleMap);
		return genericBattleInfo;
	}

	private void MergeWith(Mapping battleMap)
	{
		Scalar text = battleMap.GetText("CurrentBattleIndex");
		if (text != null)
		{
			SetBattleCounter(int.Parse(text.text));
		}
		text = battleMap.GetText("Available");
		if (text != null)
		{
			_available = text.text.Equals("1");
		}
		text = battleMap.GetText("Hidden");
		if (text != null)
		{
			_hidden = text.text.Equals("1");
		}
		text = battleMap.GetText("BattleCounter");
		if (text != null)
		{
			SetBattleCounter(int.Parse(text.text));
		}
		_battles[_currentBattleIndex].SetCurrentFight(0);
		text = battleMap.GetText("ExpirationTime");
		if (text != null)
		{
			_expirationTime = NekkiUtils.GetUnixDateTimeFromMilliseconds(long.Parse(text.text));
			if (_expirationTime <= NetworkConnection.current.getCurrentServerDateTime())
			{
				_expirationTime = DateTime.MinValue;
				SetBattleAvailable(true);
			}
		}
		text = battleMap.GetText("GenTime");
		if (text != null && _genTime == DateTime.MinValue)
		{
			_genTime = NekkiUtils.GetUnixDateTimeFromMilliseconds(long.Parse(text.text));
		}
	}

	private void IncBattleCounter()
	{
		SetBattleCounter(_battleCounter + 1);
	}

	private void SetBattleCounter(int value)
	{
		_battleCounter = value;
		_currentBattleIndex = _battleCounter % _battles.Count;
	}

	public BattleInfo GetBattleInfo()
	{
		if (_battles.Count > 0)
		{
			return _battles[_currentBattleIndex];
		}
		return null;
	}

	public FightInfo GetCurrentFight()
	{
		return GetBattleInfo().GetCurrentFight();
	}

	public void SetTestBattleType()
	{
		foreach (BattleInfo battle in _battles)
		{
			battle.SetTestBattleType();
		}
	}

	public int GetID()
	{
		return _battles[_currentBattleIndex].id;
	}

	public BattleType GetBattleType()
	{
		return _battles[_currentBattleIndex].GetBattleType();
	}

	public List<FightInfo> GetFights()
	{
		return _battles[_currentBattleIndex].fights;
	}

	public int GetWonFights()
	{
		return _battles[_currentBattleIndex].wonFights;
	}

	public object Clone()
	{
		GenericBattleInfo genericBattleInfo = MemberwiseClone() as GenericBattleInfo;
		genericBattleInfo._battles = _battles.Select((BattleInfo bv) => bv.Clone() as BattleInfo).ToList();
		return genericBattleInfo;
	}

	public void MergeWith(Battle battleData, int battleIndex)
	{
		if (battleData.Battles.Count != _battles.Count)
		{
			Debug.LogError(string.Format("Error in compliance battles for merge"));
			return;
		}
		for (int i = 0; i < battleData.Battles.Count; i++)
		{
			_battles[i].MergeWith(battleData, i);
		}
		SetBattleCounter(battleData.BattleCounter);
		_genTime = NekkiUtils.GetUnixDateTimeFromMilliseconds(battleData.GenTime.Value);
	}

	public int GetBattleCounter()
	{
		return _battleCounter;
	}

	public Mapping ToYAML()
	{
		Mapping mapping = new Mapping("ID", new Scalar("ID", GetID().ToString()));
		mapping.Add(new Scalar("CurrentBattleIndex", _currentBattleIndex.ToString()));
		mapping.Add(new Scalar("Hidden", (!_hidden) ? string.Empty : "1"));
		mapping.Add(new Scalar("Available", _available ? string.Empty : "0"));
		mapping.Add(new Scalar("BattleCounter", _battleCounter.ToString()));
		if (HasExpirationTime())
		{
			mapping.Add(new Scalar("ExpirationTime", _expirationTime.GetUnixTimeStampMilliseconds().ToString()));
		}
		mapping.Add(new Scalar("GenTime", _genTime.GetUnixTimeStampMilliseconds().ToString()));
		mapping.Add(new Sequence("Battles", ((IEnumerable<BattleInfo>)_battles).Select((Func<BattleInfo, Node>)((BattleInfo bt) => bt.ToYAML())).ToList()));
		return mapping;
	}

	public JSONClass ToJSON()
	{
		throw new NotImplementedException();
	}

	public void SetBattleHidden(bool value)
	{
		_hidden = value;
	}

	public void SetBattleAvailable(bool value)
	{
		_available = value;
	}

	public bool GetIsHidden()
	{
		return _hidden;
	}

	public bool GetIsAvailable()
	{
		return _available && !GetIsCompleted();
	}

	public LocationInfo GetLocation()
	{
		return _battles[_currentBattleIndex].location;
	}

	public long GetCooldown()
	{
		return _battles[_currentBattleIndex].GetCooldown();
	}

	public bool HasCooldown()
	{
		return _battles[_currentBattleIndex].HasCooldown();
	}

	public DateTime GetGenerationTime()
	{
		return _genTime;
	}

	public DateTime GetExpirationTime()
	{
		return _expirationTime;
	}

	public DateTime GetFinishTime()
	{
		return _battles[_currentBattleIndex].GetFinishTime();
	}

	public void SetExpirationTime()
	{
		DateTime dateTime = _genTime.AddMilliseconds(GetCooldown() * _battleCounter);
		_expirationTime = NetworkConnection.current.getCurrentServerDateTime().AddMilliseconds(GetCooldown());
		if (_expirationTime < dateTime)
		{
			_expirationTime = dateTime;
		}
	}

	public bool HasExpirationTime()
	{
		return _expirationTime != DateTime.MinValue;
	}

	public void ClearExpirationTime()
	{
		_expirationTime = DateTime.MinValue;
	}

	public void CompleteFight(FightResult resultFight)
	{
		_battles[_currentBattleIndex].CompleteFight(FightResult.Win);
		if (_battles[_currentBattleIndex].GetIsCompleted())
		{
			IncBattleCounter();
			int integerConstant = JS.Instance.GetIntegerConstant("maxSurvivalCountWithoutRegeneration");
			if (_battleCounter >= integerConstant)
			{
				_battleCounter = integerConstant;
			}
		}
	}

	public bool GetIsCompleted()
	{
		int integerConstant = JS.Instance.GetIntegerConstant("maxSurvivalCountWithoutRegeneration");
		return _battleCounter == integerConstant;
	}

	public int[] GetChapters()
	{
		return _battles[_currentBattleIndex].GetChapters();
	}

	public void SetCurrentFight(int fightIndex)
	{
		_battles[_currentBattleIndex].SetCurrentFight(fightIndex);
	}
}
