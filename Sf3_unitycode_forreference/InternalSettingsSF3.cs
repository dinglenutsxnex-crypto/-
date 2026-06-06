using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class InternalSettingsSF3 : InternalSettings
{
	[Serializable]
	public class DefaultBalancerObject
	{
		public const string FileConfig = "Configs/localServerSettings";

		public string defaultBalancer = string.Empty;

		public string forceBalancer = string.Empty;

		public string[] logIgnoreCmd = new string[0];

		public static DefaultBalancerObject CreateBalancer()
		{
			return GlobalLoad.GetLoadJson<DefaultBalancerObject>("Configs/localServerSettings");
		}

		public string ToJSONString()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}

	private static DefaultBalancerObject _defaultBalancer;

	private static DefaultBalancerObject DefaultBalancerInstanceWrapper
	{
		get
		{
			if (_defaultBalancer == null)
			{
				_defaultBalancer = DefaultBalancerObject.CreateBalancer();
			}
			return _defaultBalancer;
		}
	}

	public static Dictionary<string, string> Balancers
	{
		get
		{
			return InternalSettings.GetServerObject<Dictionary<string, string>>("Balancers");
		}
	}

	public static string[] LogIgnoreCmd
	{
		get
		{
			return DefaultBalancerInstanceWrapper.logIgnoreCmd;
		}
	}

	public static string DefaultBalancer
	{
		get
		{
			return DefaultBalancerInstanceWrapper.defaultBalancer;
		}
	}

	public static string ForceBalancer
	{
		get
		{
			return DefaultBalancerInstanceWrapper.forceBalancer;
		}
	}

	public static void Init()
	{
		InternalSettings.Init(ConfigsSourceResolver.InternalSettings);
		_defaultBalancer = null;
	}
}
