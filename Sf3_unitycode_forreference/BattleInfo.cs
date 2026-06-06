using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jint.Native;
using Jint.Native.Array;
using Jint.Runtime;
using Nekki;
using Nekki.Yaml;
using SF3;
using SF3.Settings;
using SimpleJSON;
using sf3DTO;

public class BattleInfo : IBattleInfo, ICloneable
{
	[Flags]
	public enum CompletionType
	{
		WIN = 1,
		LOSS = 2,
		SURRENDER = 4
	}

	protected int _id;

	private sf3DTO.BattleType _battleType = sf3DTO.BattleType.Test;

	protected string _name = string.Empty;

	protected string _alias = string.Empty;

	protected string _description = string.Empty;

	protected string _icon = string.Empty;

	protected string _image = string.Empty;

	protected string _stageIcon = FightSettings.defaultStageIcon;

	protected LocationInfo _location;

	protected List<FightInfo> _fights = new List<FightInfo>();

	private bool _available = true;

	private bool _hidden;

	private long _cooldownMilliseconds;

	private DateTime _expirationTime = DateTime.MinValue;

	private DateTime _genTime = DateTime.MinValue;

	private DateTime _finishTime = DateTime.MinValue;

	private int _currentFightIndex;

	private int[] _chapters;

	private CompletionType _completion = CompletionType.WIN;

	public int id
	{
		get
		{
			return _id;
		}
	}

	public sf3DTO.BattleType battleType
	{
		get
		{
			return _battleType;
		}
	}

	public string name
	{
		get
		{
			return _name;
		}
	}

	public string alias
	{
		get
		{
			return _alias;
		}
	}

	public string description
	{
		get
		{
			return _description;
		}
	}

	public string icon
	{
		get
		{
			return _icon;
		}
	}

	public string image
	{
		get
		{
			return _image;
		}
	}

	public string stageIcon
	{
		get
		{
			return _stageIcon;
		}
	}

	public LocationInfo location
	{
		get
		{
			return _location;
		}
	}

	public string battleTypeAlias
	{
		get
		{
			return "battle_type_" + _battleType.ToString().ToLower();
		}
	}

	public List<FightInfo> fights
	{
		get
		{
			return _fights;
		}
	}

	public int wonFights
	{
		get
		{
			return _currentFightIndex;
		}
	}

	protected BattleInfo()
	{
	}

	public BattleInfo(Dictionary<string, JsValue> battleData)
	{
		Parse(battleData);
	}

	public static BattleInfo Create(Mapping battleMap)
	{
		BattleInfo battleInfo = null;
		Scalar text = battleMap.GetText("ID");
		int battleID = int.Parse(text.text);
		battleInfo = BattlesManager.instance.GetJSBattle(battleID);
		battleInfo.MergeWith(battleMap);
		return battleInfo;
	}

	public static IBattleInfo Create(Battle battle)
	{
		if (battle.Battles.Count == 1)
		{
			return BattlesManager.instance.GetJSBattle(battle.Battles[0].ModelId);
		}
		return new GenericBattleInfo(battle.Battles.Select((GeneratedBattle bt) => BattlesManager.instance.GetJSBattle(bt.ModelId)).ToList());
	}

	protected virtual void Parse(Dictionary<string, JsValue> battleJSDictionary)
	{
		foreach (KeyValuePair<string, JsValue> item in battleJSDictionary)
		{
			switch (item.Key)
			{
			case "Name":
				_name = item.Value.ToString();
				break;
			case "ID":
				_id = int.Parse(item.Value.ToString());
				break;
			case "Type":
				_battleType = (sf3DTO.BattleType)Enum.Parse(typeof(sf3DTO.BattleType), item.Value.ToString(), true);
				break;
			case "Chapters":
			{
				ArrayInstance arrayInstance2 = item.Value.AsArray();
				_chapters = new int[arrayInstance2.Length];
				for (int j = 0; j < arrayInstance2.Length; j++)
				{
					_chapters[j] = arrayInstance2[j].AsDictionary()["ID"].AsInteger();
				}
				break;
			}
			case "Alias":
				_alias = item.Value.ToString();
				break;
			case "Icon":
				_icon = item.Value.ToString();
				break;
			case "StageIcon":
				_stageIcon = item.Value.ToString();
				break;
			case "Completion":
			{
				ArrayInstance arrayInstance = item.Value.AsArray();
				if (arrayInstance.Length == 0)
				{
					_completion = (CompletionType)0;
					break;
				}
				Dictionary<int, string> enumDictionary = JS.Instance.EnumsCompliancer.GetEnumDictionary("FightResult");
				_completion = (CompletionType)Enum.Parse(typeof(CompletionType), enumDictionary[arrayInstance[0].AsInteger()]);
				for (int i = 1; i < arrayInstance.Length; i++)
				{
					_completion |= (CompletionType)Enum.Parse(typeof(CompletionType), enumDictionary[arrayInstance[i].AsInteger()]);
				}
				break;
			}
			case "Location":
				_location = new LocationInfo(item.Value.AsDictionary());
				break;
			case "Image":
				_image = item.Value.ToString();
				break;
			case "Available":
				_available = item.Value.AsBoolean();
				break;
			case "Hidden":
				_hidden = item.Value.AsInteger() == 1;
				break;
			case "Cooldown":
				_cooldownMilliseconds = long.Parse(item.Value.ToString());
				break;
			}
		}
		_description = "desc_" + _alias;
		_name = ((_name.Length != 0) ? _name : _location.locationName);
		_image = ((_image.Length != 0) ? _image : _location.locationName);
		ParseFights(battleJSDictionary);
	}

	private void ParseFights(Dictionary<string, JsValue> battleJSDictionary)
	{
		JsValue jsValue = battleJSDictionary["Fights"];
		if (jsValue.Type == Types.Number)
		{
			_fights = new List<FightInfo>(jsValue.AsInteger());
		}
		else
		{
			_fights = new List<FightInfo>((int)jsValue.AsArray().Length);
		}
		for (int i = 0; i < _fights.Capacity; i++)
		{
			_fights.Add(FightInfo.Create(_id, battleJSDictionary, i));
		}
	}

	private void MergeWith(Mapping battleMap)
	{
		Scalar text = battleMap.GetText("Available");
		if (text != null)
		{
			_available = text.text.Equals("1");
		}
		text = battleMap.GetText("Hidden");
		if (text != null)
		{
			_hidden = text.text.Equals("1");
		}
		text = battleMap.GetText("WonFights");
		if (text != null)
		{
			SetCurrentFight(int.Parse(text.text));
		}
		text = battleMap.GetText("ExpirationTime");
		if (text != null)
		{
			_expirationTime = NekkiUtils.GetUnixDateTimeFromMilliseconds(long.Parse(text.text));
			if (_expirationTime <= NetworkConnection.current.getCurrentServerDateTime())
			{
				_expirationTime = DateTime.MinValue;
			}
		}
		text = battleMap.GetText("GenTime");
		if (text != null && _genTime == DateTime.MinValue)
		{
			_genTime = NekkiUtils.GetUnixDateTimeFromMilliseconds(long.Parse(text.text));
		}
		Sequence sequence = battleMap.GetSequence("Fights");
		if (sequence != null)
		{
			for (int i = 0; i < sequence.nodesInside.Count; i++)
			{
				_fights[i].MergeWith(battleMap);
			}
		}
	}

	public void MergeWith(Battle battleValue, int battleIndex)
	{
		if (_battleType == sf3DTO.BattleType.Survival)
		{
			if (battleValue.BattleCounter % battleValue.Battles.Count == battleIndex)
			{
				SetCurrentFight(battleValue.CurrentFightIndex);
			}
			else
			{
				SetCurrentFight(0);
			}
		}
		else if (battleValue.BattleCounter >= battleValue.Battles.Count)
		{
			SetCurrentFight(_fights.Count);
		}
		else
		{
			SetCurrentFight(battleValue.CurrentFightIndex);
		}
		_genTime = NekkiUtils.GetUnixDateTimeFromMilliseconds(battleValue.GenTime.Value);
		if (battleValue.LastFightFinishTime != null)
		{
			_finishTime = NekkiUtils.GetUnixDateTimeFromMilliseconds(battleValue.LastFightFinishTime.Value);
		}
		GeneratedBattle generatedBattle = battleValue.Battles[battleIndex];
		for (int i = 0; i < generatedBattle.Fights.Count; i++)
		{
			_fights[i].Merge(generatedBattle.Fights[i]);
		}
		if (GetIsCompleted())
		{
			SetBattleHidden(true);
		}
	}

	public FightInfo GetCurrentFight()
	{
		FightInfo result = null;
		if (fights.Count > 0 && !GetIsCompleted())
		{
			result = fights[_currentFightIndex];
		}
		return result;
	}

	private DateTime GetDailyTimer()
	{
		Dictionary<string, JsValue> dictionary = JS.Instance.GetScope("Battles").AsDictionary();
		foreach (KeyValuePair<string, JsValue> item in dictionary)
		{
			Dictionary<string, JsValue> dictionary2 = item.Value.AsDictionary();
			if (!(dictionary2["ID"] == GetID()) || !dictionary2.ContainsKey("Schedule"))
			{
				continue;
			}
			Dictionary<string, JsValue> dictionary3 = dictionary2["Schedule"].AsDictionary();
			if (dictionary3 != null)
			{
				DateTime dateTime = DateTime.Parse(dictionary3["Origin"].ToString());
				int num = dictionary3["Period"].AsInteger();
				double value = (double)num - (NetworkConnection.current.getCurrentServerDateTime() - dateTime).TotalMilliseconds % (double)num;
				if (GetGenerationTime() < dateTime.AddMilliseconds(num))
				{
				}
				return NetworkConnection.current.getCurrentServerDateTime().Add(TimeSpan.FromMilliseconds(value));
			}
			return DateTime.MinValue;
		}
		return DateTime.MinValue;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("BattleInfo:");
		stringBuilder.AppendFormat("ID: [{0}]; Type: [{1}]; Available: [{2}]; Hidden: [{3}]; Won_fights: [{4}]; Expire_date_time: [{5}];", _id, _battleType, _available, _hidden, _currentFightIndex, _expirationTime);
		stringBuilder.AppendFormat("alias {0}\ndescription {1}\nicon {2}\nimage {3}\nstageIcon {4}\nlocation {5}", alias, description, icon, image, stageIcon, location);
		stringBuilder.AppendLine("\n Fights:");
		foreach (FightInfo fight in fights)
		{
			stringBuilder.Append("[ ");
			stringBuilder.Append(fight.ToString());
			stringBuilder.Append(" ]\n");
		}
		return stringBuilder.ToString();
	}

	public BattleInfo GetBattleInfo()
	{
		return this;
	}

	public void SetTestBattleType()
	{
		_battleType = sf3DTO.BattleType.Test;
	}

	public int GetID()
	{
		return id;
	}

	public sf3DTO.BattleType GetBattleType()
	{
		return _battleType;
	}

	public bool GetIsCompleted()
	{
		return _currentFightIndex >= fights.Count;
	}

	public List<FightInfo> GetFights()
	{
		return fights;
	}

	public int GetWonFights()
	{
		return wonFights;
	}

	public virtual object Clone()
	{
		BattleInfo result = MemberwiseClone() as BattleInfo;
		result._fights = new List<FightInfo>();
		_fights.ForEach(delegate(FightInfo fightValue)
		{
			result._fights.Add(fightValue.Clone() as FightInfo);
		});
		return result;
	}

	public DateTime GetFinishTime()
	{
		return _finishTime;
	}

	public int GetBattleCounter()
	{
		return GetIsCompleted() ? 1 : 0;
	}

	public Mapping ToYAML()
	{
		Mapping mapping = new Mapping(_id.ToString(), new Scalar("ID", _id.ToString()));
		mapping.Add(new Scalar("Hidden", (!_hidden) ? string.Empty : "1"));
		mapping.Add(new Scalar("Available", _available ? string.Empty : "0"));
		mapping.Add(new Scalar("WonFights", _currentFightIndex.ToString()));
		if (HasExpirationTime())
		{
			mapping.Add(new Scalar("ExpirationTime", _expirationTime.GetUnixTimeStampMilliseconds().ToString()));
		}
		mapping.Add(new Scalar("GenTime", _genTime.GetUnixTimeStampMilliseconds().ToString()));
		if (_fights.Count > 0)
		{
			Mapping[] mappingInside = _fights.Select((FightInfo fightNodes) => fightNodes.ToYAML()).ToArray();
			Sequence entry = new Sequence("Fights", mappingInside);
			mapping.Add(entry);
		}
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
		return _available;
	}

	public LocationInfo GetLocation()
	{
		return _location;
	}

	public long GetCooldown()
	{
		return _cooldownMilliseconds;
	}

	public bool HasCooldown()
	{
		return _cooldownMilliseconds != 0;
	}

	public DateTime GetGenerationTime()
	{
		return _genTime;
	}

	public DateTime GetExpirationTime()
	{
		return _expirationTime;
	}

	public void SetExpirationTime()
	{
		DateTime dateTime = _genTime.AddMilliseconds(GetCooldown());
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

	public void CompleteFight(sf3DTO.FightResult resultFight)
	{
		if (!GetIsCompleted() && ((uint)_completion & (uint)resultFight) == (uint)resultFight)
		{
			_currentFightIndex++;
			if (_currentFightIndex >= _fights.Count && _battleType == sf3DTO.BattleType.Test)
			{
				SetCurrentFight(0);
			}
		}
	}

	public int[] GetChapters()
	{
		return _chapters;
	}

	public void SetCurrentFight(int fightIndex)
	{
		_currentFightIndex = fightIndex;
	}
}
