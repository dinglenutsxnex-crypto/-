using System.Collections.Generic;
using System.Linq;
using Nekki.Yaml;

public class SplitData
{
	public readonly string FileName;

	public Sequence Locks;

	private readonly Dictionary<string, Mapping> _triggerSets;

	private readonly Dictionary<string, Mapping> _animationsSettings;

	public SplitData(Sequence locks, string fileName)
	{
		Locks = locks;
		FileName = fileName;
		_triggerSets = new Dictionary<string, Mapping>();
		_animationsSettings = new Dictionary<string, Mapping>();
	}

	public void AddTriggerSet(string name, Mapping value)
	{
		SetDictionary(_triggerSets, name, value);
	}

	public void AddAnimation(string name, Mapping value)
	{
		SetDictionary(_animationsSettings, name, value);
	}

	public Sequence GetTriggersSet()
	{
		if (_triggerSets.Count > 0)
		{
			return new Sequence("TriggerSets", _triggerSets.Values.ToArray());
		}
		return null;
	}

	public Sequence GetAnimations()
	{
		if (_animationsSettings.Count > 0)
		{
			return new Sequence("Animations", _animationsSettings.Values.ToArray());
		}
		return null;
	}

	private void SetDictionary(Dictionary<string, Mapping> dict, string name, Mapping value)
	{
		if (!dict.ContainsKey(name))
		{
			dict[name] = value;
		}
	}
}
