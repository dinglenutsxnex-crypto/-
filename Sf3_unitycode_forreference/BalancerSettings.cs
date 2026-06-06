using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class BalancerSettings
{
	public class ConfigVersion
	{
		[JsonProperty("cur")]
		public string currentVersion;

		public string url;
	}

	private Dictionary<string, float> _load;

	private List<BalancerItem> balancerItems = new List<BalancerItem>();

	public ConfigVersion version;

	public Dictionary<string, float> load
	{
		get
		{
			return _load;
		}
		set
		{
			_load = value;
			balancerItems.Clear();
			foreach (KeyValuePair<string, float> item in _load)
			{
				balancerItems.Add(new BalancerItem(item.Key, item.Value));
			}
		}
	}

	public List<BalancerItem> getBalancerItems()
	{
		return balancerItems;
	}

	public BalancerItem getBalancerItem()
	{
		if (balancerItems.Count == 0)
		{
			Debug.LogError("No balancers found");
			return null;
		}
		BalancerItem balancerItem = null;
		int num = int.MaxValue;
		float num2 = float.MaxValue;
		for (int i = 0; i < balancerItems.Count; i++)
		{
			BalancerItem balancerItem2 = balancerItems[i];
			if (balancerItem2.attempted <= num && (balancerItem2.load < num2 || balancerItem2.attempted > 0))
			{
				balancerItem = balancerItem2;
				num = balancerItem2.attempted;
				num2 = balancerItem2.load;
			}
		}
		balancerItem.attempted++;
		return balancerItem;
	}
}
