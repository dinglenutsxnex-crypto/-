using System;
using System.Collections.Generic;
using Nekki;
using Newtonsoft.Json;
using UnityEngine;

public class NetworkSettings
{
	public class ConfigVersion
	{
		[JsonIgnore]
		public string full;

		[JsonIgnore]
		public VariedVersionContainer version;

		[JsonIgnore]
		public string serverName;

		[JsonIgnore]
		public VariedVersionContainer min_version;

		[JsonIgnore]
		public string marketVersionToString { get; private set; }

		[JsonConstructor]
		public ConfigVersion(string full, string min = null)
		{
			this.full = full;
			string[] array = full.Split('-');
			version = new VariedVersionContainer(array[0]);
			serverName = array[1];
			marketVersionToString = version.GetVersionToString(3);
			if (min != null)
			{
				min_version = new VariedVersionContainer(min);
			}
		}

		public override string ToString()
		{
			return "NetworkSettings.ConfigVersion [full version: " + full + "]";
		}
	}

	[Serializable]
	public class Config
	{
		public class PingInfo
		{
			public float Delay;

			public int FailAttempts;
		}

		public class OfflineRequestTimeoutInfo
		{
			public float PerRequests = 5f;

			public int ExtraTimeout = 250;
		}

		public class TimeoutParamsInfo
		{
			public float Min = 5000f;

			public float Multiplier = 1f;

			public float Max = 5000f;
		}

		public string[] Balancers;

		public float DefaultRequestTimeout;

		public float JoinZoneTimeout;

		public float ConnectTimeout = 5000f;

		public float LoginTimeout = 5000f;

		public string BundlesBaseURL;

		public PingInfo Ping;

		public OfflineRequestTimeoutInfo OfflineRequestTimeout;

		public Dictionary<string, float> Timeouts;

		public TimeoutParamsInfo TimeoutParams;

		[JsonIgnore]
		private List<string> BalancersToTry = new List<string>();

		public bool CheckEmail;

		public float GetTimeoutForCmd(string cmd)
		{
			if (Timeouts == null || !Timeouts.ContainsKey(cmd))
			{
				return DefaultRequestTimeout.ToSeconds();
			}
			return Timeouts[cmd].ToSeconds();
		}

		public string getRandomBalancerURL()
		{
			if (Balancers.Length == 0)
			{
				Debug.LogError("No Balancer to choose from");
				return string.Empty;
			}
			if (BalancersToTry.Count == 0)
			{
				string[] balancers = Balancers;
				foreach (string item in balancers)
				{
					BalancersToTry.Add(item);
				}
			}
			int index = NekkiMath.randomInt(0, BalancersToTry.Count);
			string result = BalancersToTry[index];
			BalancersToTry.RemoveAt(index);
			return result;
		}
	}

	public class Server
	{
		[JsonProperty("all_versions")]
		public List<string> validVersions;
	}

	public ConfigVersion version;

	public Config config;

	internal ConfigVersion getCurrentVersion()
	{
		if (version == null || version.full == null)
		{
			Debug.LogError("Version wasn't found in file");
			return new ConfigVersion("0.0.0.0.0-unknown");
		}
		return version;
	}
}
