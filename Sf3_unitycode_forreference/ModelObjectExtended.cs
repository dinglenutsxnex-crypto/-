using System.Collections.Generic;
using System.Linq;
using SF3;
using SF3.GameModels;
using SF3.Items;
using SF3.Utils;
using UnityEngine;
using sf3DTO;

public class ModelObjectExtended : ModelObject
{
	private readonly int _framesTotal;

	private readonly int _id;

	private readonly int _usageCountTotal;

	private int _usageCountCurrent;

	private bool _timerWorks;

	private int _framesPassed;

	private readonly Queue<int> _counters;

	public ModelObjectExtended(int parentId, Gender genderValue, Equipment equipment)
		: base(genderValue)
	{
		_usageCountCurrent = 0;
		_counters = new Queue<int>();
		_id = parentId;
		_usageCountTotal = equipment.UsageQuantity;
		_framesTotal = SF3Utils.SecondsToFrames(equipment.CooldownSeconds);
		_timerWorks = false;
		_framesPassed = 0;
		Subscribe();
		Validate();
	}

	private void Subscribe()
	{
		GameVariables.Subscribe(_id, "RangedCreated", Check);
		GameVariables.AddVariable(_id, "RangedUsagesLeft", _usageCountTotal);
	}

	private void Validate()
	{
		bool flag = false;
		StringWrapper stringWrapper = StringWrapper.Create(StringWrapperPurpose.Error);
		if (_usageCountTotal == 0)
		{
			flag = true;
			stringWrapper.Wrap("UsageCountTotal is 0.");
		}
		if (_framesTotal == 0)
		{
			flag = true;
			stringWrapper.Wrap("FramesTotal is 0");
		}
		if (flag)
		{
			stringWrapper.Wrap("Check equipment data.\n" + base.equipment.ToString());
			Debug.LogError(stringWrapper.ToString());
		}
	}

	private void Check(int id, string varName)
	{
		if (varName != null && varName == "RangedCreated")
		{
			TryUse();
		}
	}

	private void TryUse()
	{
		if (_usageCountCurrent < _usageCountTotal)
		{
			_usageCountCurrent++;
			UpdateVariables();
			_counters.Enqueue(0);
			LaunchTimer();
		}
	}

	private void UpdateVariables()
	{
		SetVariable(_id, "RangedUsagesLeft", _usageCountTotal - _usageCountCurrent);
	}

	private void SetVariable(int id, string varName, int value)
	{
		GameVariables.LocalVariable variable = GameVariables.GetVariable(id, varName);
		if (variable == null)
		{
			Debug.LogError("Variable found by key : [" + varName + "] is null");
		}
		else
		{
			variable.SetValue(value);
		}
	}

	private void LaunchTimer()
	{
		if (_counters.Count > 0 && !_timerWorks)
		{
			_timerWorks = true;
			BehaviourTimer.CreateGameFramesTimer(_framesTotal, OnTimerUpdate, null, OnTimerEnd);
		}
	}

	private void OnTimerUpdate(float value)
	{
		_framesPassed = _counters.Dequeue();
		_framesPassed++;
		_counters.Enqueue(_framesPassed);
		if (_counters.Count > 1)
		{
			for (int i = 0; i < _counters.Count - 1; i++)
			{
				_counters.Enqueue(_counters.Dequeue());
			}
		}
	}

	private void OnTimerEnd(object o)
	{
		_usageCountCurrent--;
		UpdateVariables();
		_timerWorks = false;
		_counters.Dequeue();
		LaunchTimer();
	}

	public float GetCooldownSecondsLeft()
	{
		if (_counters.Count == 0)
		{
			return 0f;
		}
		return (float)(_framesTotal - _counters.Peek()) / 60f;
	}

	public float GetCooldownSecondsLeftTotal()
	{
		return (float)(_counters.Count * _framesTotal - _counters.Sum()) / 60f;
	}

	private void Print()
	{
		Debug.LogError(string.Format("UsagesTotal: {0}, UsagesCurrent: {1}, RangedUsagesLeft: {2}, TimerCount: {3}", _usageCountTotal, _usageCountCurrent, (int)GameVariables.GetVariable(_id, "RangedUsagesLeft").value, _counters.Count));
	}
}
