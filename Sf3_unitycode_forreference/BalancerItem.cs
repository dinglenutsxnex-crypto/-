using System;
using UnityEngine;

[Serializable]
public class BalancerItem
{
	public int attempted;

	public string ip { get; set; }

	public int port { get; set; }

	public float load { get; set; }

	public BalancerItem(string _ipAndPort, float _load)
	{
		load = _load;
		string[] array = _ipAndPort.Split(':');
		if (array.Length == 2)
		{
			ip = array[0];
			port = int.Parse(array[1]);
		}
		else
		{
			Debug.LogError("Balancer Item doesn't have ip and port");
		}
	}

	public bool equal(BalancerItem other)
	{
		return ip == other.ip && port == other.port;
	}
}
