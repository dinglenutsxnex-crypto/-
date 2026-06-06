using System;
using System.Collections.Generic;
using Jint.Native;

public class DailyBattleInfo : BattleInfo
{
	private DateTime _orign = DateTime.MinValue;

	private double _period;

	public bool DailyUpdateTimePassed
	{
		get
		{
			return GetGenerationTime().Add(TimeSpan.FromMilliseconds(_period)) < NetworkConnection.current.getCurrentServerDateTime() && GetIsHidden();
		}
	}

	public DateTime NextDailyUpdateTime
	{
		get
		{
			double value = _period - (NetworkConnection.current.getCurrentServerDateTime() - _orign).TotalMilliseconds % _period;
			return NetworkConnection.current.getCurrentServerDateTime().Add(TimeSpan.FromMilliseconds(value));
		}
	}

	public DailyBattleInfo(Dictionary<string, JsValue> data)
		: base(data)
	{
		Parse(data);
	}

	protected override void Parse(Dictionary<string, JsValue> data)
	{
		base.Parse(data);
		foreach (KeyValuePair<string, JsValue> datum in data)
		{
			string key = datum.Key;
			if (key != null && key == "Schedule")
			{
				Dictionary<string, JsValue> dictionary = datum.Value.AsDictionary();
				if (dictionary != null)
				{
					_orign = DateTime.Parse(dictionary["Origin"].ToString());
					_period = dictionary["Period"].AsInteger();
				}
			}
		}
	}

	public override object Clone()
	{
		DailyBattleInfo result = MemberwiseClone() as DailyBattleInfo;
		result._fights = new List<FightInfo>();
		_fights.ForEach(delegate(FightInfo fightValue)
		{
			result._fights.Add(fightValue.Clone() as FightInfo);
		});
		return result;
	}
}
