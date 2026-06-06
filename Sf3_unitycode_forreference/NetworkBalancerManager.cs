using System;
using System.Collections.Generic;
using UnityEngine;

public class NetworkBalancerManager
{
	public const string PREFS_CURRENT_SERVER = "current_server";

	public Action onSuccess = delegate
	{
	};

	public Action onFail = delegate
	{
	};

	private BalancerSettings balancerSettings;

	private Dictionary<string, string> balancers;

	private Dictionary<string, string> Balancers
	{
		get
		{
			if (balancers == null)
			{
				balancers = InternalSettingsSF3.Balancers;
			}
			return balancers;
		}
	}

	public NekkiWebRequest Download(string url, bool checkCert = true)
	{
		Action<NekkiWebRequest> onSuccessful = OnBalancerDownloaded;
		Action<NekkiWebRequest> onError = OnBalancerError;
		Action<NekkiWebRequest> onProgress = OnBalancerProgress;
		bool checkCert2 = checkCert;
		return NekkiWebHelper.SendRequest(url, onSuccessful, onError, onProgress, null, 5f, checkCert2);
	}

	private void OnBalancerDownloaded(NekkiWebRequest web)
	{
		BalancerSettings json = web.GetJson<BalancerSettings>();
		if (balancerSettings != null)
		{
			List<BalancerItem> balancerItems = balancerSettings.getBalancerItems();
			List<BalancerItem> balancerItems2 = json.getBalancerItems();
			foreach (BalancerItem item in balancerItems2)
			{
				foreach (BalancerItem item2 in balancerItems)
				{
					if (item.equal(item2))
					{
						item.attempted = item2.attempted;
					}
				}
			}
		}
		balancerSettings = json;
		if (balancerSettings.getBalancerItems().Count > 0)
		{
			onSuccess();
		}
		else
		{
			OnBalancerError(web);
		}
	}

	private void OnBalancerError(NekkiWebRequest web)
	{
		onFail();
	}

	private void OnBalancerProgress(NekkiWebRequest web)
	{
	}

	public BalancerItem getBalancerItem()
	{
		if (balancerSettings == null)
		{
			Debug.LogError("Error: Calling getBalancerItem before balancer was downloaded");
			return null;
		}
		return balancerSettings.getBalancerItem();
	}

	public NetworkSettings.ConfigVersion getConfigVersion()
	{
		if (balancerSettings == null)
		{
			Debug.LogError("Error: Calling getConfigVersion before balancer was downloaded");
			return null;
		}
		return new NetworkSettings.ConfigVersion(balancerSettings.version.currentVersion);
	}

	public string getConfigURL()
	{
		if (balancerSettings == null)
		{
			Debug.LogError("Error: Calling getConfigURL before balancer was downloaded");
			return null;
		}
		return balancerSettings.version.url;
	}

	public string GetBalancerUrl()
	{
		string text = InternalSettingsSF3.ForceBalancer;
		if (text.IsNullOrEmpty())
		{
			string @string = PlayerPrefs.GetString("current_server", string.Empty);
			text = ((@string.Length <= 0) ? InternalSettingsSF3.DefaultBalancer : @string);
		}
		string value;
		return (!Balancers.TryGetValue(text, out value)) ? string.Empty : value;
	}
}
