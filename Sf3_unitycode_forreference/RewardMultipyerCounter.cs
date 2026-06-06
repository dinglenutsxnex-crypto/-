using System.Collections.Generic;
using Jint.Native;
using SF3;
using UnityEngine;

public class RewardMultipyerCounter
{
	private readonly List<RewardMultipyerCounterUnit> _counters = new List<RewardMultipyerCounterUnit>();

	private Dictionary<int, string> _rewardsMultipliersMapIntString;

	private Dictionary<string, int> _rewardsMultipliersMapStringInt;

	public static RewardMultipyerCounter Instance { get; private set; }

	public Dictionary<int, string> RewardMultipliersMap
	{
		get
		{
			ReadRewardMultipliers();
			return _rewardsMultipliersMapIntString;
		}
	}

	public RewardMultipyerCounter()
	{
		Instance = this;
	}

	private void ReadRewardMultipliers()
	{
		if (_rewardsMultipliersMapIntString != null && _rewardsMultipliersMapStringInt != null)
		{
			return;
		}
		_rewardsMultipliersMapIntString = new Dictionary<int, string>();
		_rewardsMultipliersMapStringInt = new Dictionary<string, int>();
		Dictionary<string, JsValue> dictionary = JS.Instance.GetScope("RewardMultipliersMap").AsDictionary();
		foreach (KeyValuePair<string, JsValue> item in dictionary)
		{
			int num = int.Parse(item.Key);
			string text = item.Value.AsObject().GetOwnProperty("Name").Value.ToString();
			_rewardsMultipliersMapIntString.Add(num, text);
			_rewardsMultipliersMapStringInt.Add(text, num);
		}
	}

	public string GetCounterName(int id)
	{
		ReadRewardMultipliers();
		if (_rewardsMultipliersMapIntString.ContainsKey(id))
		{
			return _rewardsMultipliersMapIntString[id];
		}
		return string.Empty;
	}

	public int GetCounterID(string key)
	{
		ReadRewardMultipliers();
		if (_rewardsMultipliersMapStringInt.ContainsKey(key))
		{
			return _rewardsMultipliersMapStringInt[key];
		}
		return 0;
	}

	public void Clear()
	{
		_counters.Clear();
	}

	public List<RewardMultipyerCounterUnit> GetMultipliers()
	{
		return _counters;
	}

	public void PrintAllData(string headSentence)
	{
		StringWrapper stringWrapper = StringWrapper.Create(StringWrapperPurpose.Log);
		stringWrapper.Head(string.IsNullOrEmpty(headSentence) ? "Counter Data" : headSentence);
		foreach (RewardMultipyerCounterUnit counter in _counters)
		{
			stringWrapper.Wrap("<Name: [" + counter.Name + "], Value: [" + counter.Value + "], ID: [" + counter.ID + "]>\n");
		}
		Debug.Log(stringWrapper.ToString());
	}

	public void Set(string key, double value, bool rewrite)
	{
		RewardMultipyerCounterUnit counter = GetCounter(key);
		if (counter == null)
		{
			counter = new RewardMultipyerCounterUnit(key, GetCounterID(key), value);
			_counters.Add(counter);
		}
		else if (rewrite)
		{
			counter.SetValue(value);
		}
		else
		{
			counter.Add(value);
		}
	}

	private RewardMultipyerCounterUnit GetCounter(string key)
	{
		foreach (RewardMultipyerCounterUnit counter in _counters)
		{
			if (counter != null && counter.Name == key)
			{
				return counter;
			}
		}
		return null;
	}
}
