using System.Collections.Generic;
using System.Linq;
using SF3;
using UnityEngine;

public class DefaultCounter : ICounter
{
	private readonly Dictionary<string, double> _counters;

	public DefaultCounter()
	{
		_counters = new Dictionary<string, double>();
	}

	public double GetValue(string key)
	{
		return (!_counters.ContainsKey(key)) ? 0.0 : _counters[key];
	}

	public List<string> GetKeyList()
	{
		return _counters.Keys.ToList();
	}

	public void Set(string key, double value, bool rewrite = false)
	{
		if (_counters.ContainsKey(key))
		{
			if (rewrite)
			{
				_counters[key] = value;
			}
			else
			{
				_counters[key] += value;
			}
		}
		else
		{
			_counters.Add(key, value);
		}
	}

	public void SetAll(double value, bool rewrite = false)
	{
		foreach (KeyValuePair<string, double> counter in _counters)
		{
			Set(counter.Key, value, rewrite);
		}
	}

	public void SetAllDefault()
	{
		SetAll(0.0, true);
	}

	public void Increment(string key)
	{
		Increment(key, 1.0);
	}

	public void Increment(string key, double value)
	{
		Set(key, value);
	}

	public void Decrement(string key)
	{
		Increment(key, -1.0);
	}

	public void Decrement(string key, double value)
	{
		Increment(key, 0.0 - value);
	}

	public void Clear()
	{
		_counters.Clear();
	}

	public void PrintAllData(string headSentence = null)
	{
		StringWrapper stringWrapper = StringWrapper.Create(StringWrapperPurpose.Log);
		stringWrapper.Head(string.IsNullOrEmpty(headSentence) ? "Counter Data" : headSentence);
		foreach (KeyValuePair<string, double> counter in _counters)
		{
			stringWrapper.Wrap("<Key: [" + counter.Key + "], Value: [" + counter.Value + "]>\n");
		}
		Debug.Log(stringWrapper.ToString());
	}
}
