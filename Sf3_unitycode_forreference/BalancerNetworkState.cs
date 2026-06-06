using Nekki.Core;
using UnityEngine;

public class BalancerNetworkState : TCPNetworkState
{
	private NetworkBalancerManager balancerManager = new NetworkBalancerManager();

	private NekkiWebRequest webRequest;

	public override void TCPStart(object data)
	{
		balancerManager.onSuccess = delegate
		{
			OnBalancerDownloaded();
		};
		balancerManager.onFail = delegate
		{
			OnFail("Config Manager failed");
		};
		webRequest = balancerManager.Download(balancerManager.GetBalancerUrl());
	}

	public override void TCPCleanup()
	{
		balancerManager.onSuccess = null;
		balancerManager.onFail = null;
	}

	private void OnBalancerDownloaded()
	{
		NetworkSettings.ConfigVersion currentConfigVersion = NetworkConnection.current.CurrentConfigVersion;
		NetworkSettings.ConfigVersion configVersion = balancerManager.getConfigVersion();
		Debug.Log(string.Concat("ServerVersion - ", configVersion, " ---  CurrentVersion - ", currentConfigVersion));
		if (currentConfigVersion.serverName != configVersion.serverName)
		{
			OnSuccess(typeof(ConfigNetworkState), new ConfigNetworkStateData
			{
				configURL = balancerManager.getConfigURL()
			});
		}
		else if (configVersion.version > currentConfigVersion.version)
		{
			if (VariedVersionContainer.Compare(configVersion.version, currentConfigVersion.version, 3) != 0)
			{
				openNewVersionAlert();
				return;
			}
			OnSuccess(typeof(ConfigNetworkState), new ConfigNetworkStateData
			{
				configURL = balancerManager.getConfigURL()
			});
		}
		else
		{
			OnSuccess(typeof(ConnectionNetworkState), balancerManager.getBalancerItem());
		}
	}

	public override void TCPStop()
	{
		webRequest.AbortWWW();
	}

	private void openNewVersionAlert()
	{
		GameInit.CanContinueInit = false;
		SystemMessage systemMessage = SystemMessage.ShowAlert("download_new_version");
		string storeUrl = InternalSettings.StoreUrl;
		if (storeUrl.IsNullOrEmpty())
		{
			systemMessage.AddButton("ok", delegate
			{
				openNewVersionAlert();
			});
		}
		else
		{
			systemMessage.AddButton("download", delegate
			{
				Application.OpenURL(storeUrl);
				openNewVersionAlert();
			});
		}
		systemMessage.SetBlockPriority(UIBlocker.Priority.ServerSystemAlert);
		systemMessage.Show();
	}
}
