using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Filters
{
	private readonly Dictionary<LogType, HashSet<Filter>> filtersByType = new Dictionary<LogType, HashSet<Filter>>();

	public Filters()
	{
		filtersByType.Add(LogType.Warning, new HashSet<Filter>());
		filtersByType.Add(LogType.Error, new HashSet<Filter>());
		filtersByType.Add(LogType.Exception, new HashSet<Filter>());
	}

	public void Add(LogType type, IRule rule, string token)
	{
		filtersByType[type].Add(new Filter(rule, new string[1] { token }));
	}

	public void Add(LogType type, IRule rule, string[] tokens)
	{
		filtersByType[type].Add(new Filter(rule, tokens));
	}

	public bool Filtered(LogType type, string message)
	{
		LogType key = type;
		switch (type)
		{
		case LogType.Assert:
			key = LogType.Exception;
			break;
		case LogType.Log:
			return true;
		}
		return filtersByType[key].Any((Filter filter) => filter.Filtered(message));
	}
}
