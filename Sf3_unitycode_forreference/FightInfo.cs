using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jint.Native;
using Jint.Native.Array;
using Jint.Runtime;
using Nekki.Yaml;
using SimpleJSON;
using sf3DTO;

public class FightInfo : ICloneable
{
	private FightRewards _rewards;

	private List<RoundInfo> _rounds;

	public int battleID { get; protected set; }

	public int fightID { get; protected set; }

	private FightRewards rewards
	{
		get
		{
			return _rewards;
		}
	}

	public List<RoundInfo> rounds
	{
		get
		{
			return _rounds;
		}
	}

	public int roundTime { get; protected set; }

	public int roundsToWin { get; protected set; }

	public int roundsToLose { get; protected set; }

	public float hpRecovery { get; protected set; }

	protected FightInfo()
	{
		_rounds = new List<RoundInfo>();
		_rewards = new FightRewards();
	}

	public static FightInfo Create(int battleID, Dictionary<string, JsValue> battleData, int fightIndex)
	{
		FightInfo fightInfo = new FightInfo();
		fightInfo.battleID = battleID;
		fightInfo.fightID = fightIndex;
		JsValue jsValue = battleData["Fights"];
		ArrayInstance warriorsArray = null;
		ArrayInstance rulesArray = null;
		fightInfo.ParseFightProperties(battleData, ref rulesArray, ref warriorsArray);
		if (jsValue.Type != Types.Number)
		{
			Dictionary<string, JsValue> props = battleData["Fights"].AsArray()[fightInfo.fightID].AsDictionary();
			fightInfo.ParseFightProperties(props, ref rulesArray, ref warriorsArray);
		}
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < fightInfo.rounds.Capacity; i++)
		{
			RoundInfo roundInfo = RoundInfo.Create(warriorsArray[num].AsDictionary());
			if (num < warriorsArray.Length - 1)
			{
				num++;
			}
			if (rulesArray != null)
			{
				roundInfo.CreateRules(rulesArray, num2);
				if (num2 < rulesArray.Length - 1)
				{
					num2++;
				}
			}
			fightInfo.rounds.Add(roundInfo);
		}
		fightInfo.roundsToLose = ((fightInfo.roundsToLose != 0) ? fightInfo.roundsToLose : fightInfo.roundsToWin);
		return fightInfo;
	}

	private void ParseFightProperties(Dictionary<string, JsValue> props, ref ArrayInstance rulesArray, ref ArrayInstance warriorsArray)
	{
		if (props.ContainsKey("RoundTime"))
		{
			roundTime = props["RoundTime"].AsInteger();
		}
		if (props.ContainsKey("RoundsToWin"))
		{
			roundsToWin = props["RoundsToWin"].AsInteger();
			_rounds = new List<RoundInfo>(roundsToWin);
		}
		if (props.ContainsKey("RoundsToLose"))
		{
			roundsToLose = props["RoundsToLose"].AsInteger();
		}
		if (props.ContainsKey("HPRecovery"))
		{
			hpRecovery = (float)props["HPRecovery"].AsNumber();
		}
		if (props.ContainsKey("RewardIcon"))
		{
			ArrayInstance arrayInstance = props["RewardIcon"].AsArray();
			if (arrayInstance != null)
			{
				_rewards.SetRewardIcon(arrayInstance);
			}
		}
		if (props.ContainsKey("Rewards"))
		{
			ArrayInstance arrayInstance2 = props["Rewards"].AsArray();
			if (arrayInstance2 != null)
			{
				_rewards.SetRewards(arrayInstance2);
			}
		}
		if (props.ContainsKey("Rules"))
		{
			rulesArray = props["Rules"].AsArray();
		}
		if (props.ContainsKey("Warriors"))
		{
			warriorsArray = props["Warriors"].AsArray()[0].AsDictionary()["WarriorBlocks"].AsArray();
		}
	}

	public void MergeWith(Mapping battleMap)
	{
		Mapping mapping = battleMap.GetSequence("Fights").nodesInside[fightID] as Mapping;
		Sequence sequence = mapping.GetSequence("Rounds");
		if (sequence != null)
		{
			_rounds = new List<RoundInfo>(sequence.nodesInside.Count);
			sequence.nodesInside.ForEach(delegate(Node roundMapp)
			{
				RoundInfo item = RoundInfo.Create((Mapping)roundMapp);
				_rounds.Add(item);
			});
		}
		sequence = mapping.GetSequence("Rewards");
		if (sequence != null)
		{
			_rewards.SetRewards(sequence);
		}
	}

	public void Merge(GeneratedFight fightData)
	{
		_rewards.SetRewards(fightData.RewardsByRoundWins.RepeatedToList());
		_rounds.Clear();
		fightData.Rounds.RepeatedToList().ForEach(delegate(GeneratedRound roundValue)
		{
			RoundInfo item = RoundInfo.Create(roundValue);
			_rounds.Add(item);
		});
	}

	public object Clone()
	{
		FightInfo result = MemberwiseClone() as FightInfo;
		result._rounds = new List<RoundInfo>();
		_rounds.ForEach(delegate(RoundInfo roundValue)
		{
			result._rounds.Add(roundValue.Clone());
		});
		result._rewards = _rewards.Clone();
		return result;
	}

	public bool Equal(FightInfo info)
	{
		return battleID == info.battleID && fightID == info.fightID;
	}

	public RoundInfo GetRound(int roundNumber)
	{
		return (_rounds.Count < roundNumber) ? null : _rounds[roundNumber - 1];
	}

	public Mapping ToYAML()
	{
		Mapping mapping = new Mapping("FightInfo", new Scalar("ID", (fightID + 1).ToString()));
		if (_rounds.Count > 0)
		{
			mapping.Add(new Sequence("Rounds", ((IEnumerable<RoundInfo>)_rounds).Select((Func<RoundInfo, Node>)((RoundInfo roundValue) => roundValue.ToYAML())).ToList()));
		}
		if (_rewards != null && _rewards.HasRewards())
		{
			mapping.Add(_rewards.ToYAML());
		}
		return mapping;
	}

	public JSONClass ToJSON()
	{
		throw new NotImplementedException();
	}

	public FightRewards.RoundReward GetRewardsByRoundsWin(int roundsWin)
	{
		return _rewards.GetRewardsByRoundsWin(roundsWin);
	}

	public FightRewards.RoundReward GetRewardsMax()
	{
		return _rewards.GetRewardsByRoundsWin(_rounds.Count);
	}

	public long GetCurrencyByRoundsWin(int roundsWin, CurrencyType type)
	{
		return _rewards.GetCurrencyByRoundsWin(roundsWin, type);
	}

	public long GetCurrencyByRoundsWinForMultiplies(int roundsWin, CurrencyType type)
	{
		return _rewards.GetCurrencyByRoundsWinForMultiplies(roundsWin, type);
	}

	public List<BaseRewardInfo> GetRewardInfo()
	{
		return _rewards.rewardsIcon;
	}

	public int GetRoundsCount()
	{
		return _rounds.Count;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("roundTime {0}\nroundsToLose {1}\nroundsToWin {2}\nhpRecovery {3}", roundTime, roundsToLose, roundsToWin, hpRecovery);
		return stringBuilder.ToString();
	}
}
