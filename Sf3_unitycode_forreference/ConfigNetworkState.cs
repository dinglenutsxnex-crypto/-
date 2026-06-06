using System;
using Nekki.Core;

public class ConfigNetworkState : TCPNetworkState
{
	private NetworkConfigsDownloader _configsDownloader = new NetworkConfigsDownloader();

	private NekkiWebRequest downloadCoroutine;

	private ConfigNetworkStateData stateData;

	public override void TCPStart(object data)
	{
		stateData = (ConfigNetworkStateData)data;
		_configsDownloader = new NetworkConfigsDownloader();
		NetworkConfigsDownloader configsDownloader = _configsDownloader;
		configsDownloader.onSuccess = (Action)Delegate.Combine(configsDownloader.onSuccess, new Action(OnNetworkConfigDownloaded));
		NetworkConfigsDownloader configsDownloader2 = _configsDownloader;
		configsDownloader2.onFail = (Action)Delegate.Combine(configsDownloader2.onFail, new Action(OnConfigFailed));
		downloadCoroutine = _configsDownloader.Download(stateData.configURL, string.Empty);
	}

	public override void TCPStop()
	{
		downloadCoroutine.AbortWWW();
	}

	public override void TCPCleanup()
	{
		NetworkConfigsDownloader configsDownloader = _configsDownloader;
		configsDownloader.onSuccess = (Action)Delegate.Remove(configsDownloader.onSuccess, new Action(OnNetworkConfigDownloaded));
		NetworkConfigsDownloader configsDownloader2 = _configsDownloader;
		configsDownloader2.onFail = (Action)Delegate.Remove(configsDownloader2.onFail, new Action(OnConfigFailed));
	}

	private void OnConfigFailed()
	{
		OnFail("Failed to download config");
	}

	private void OnNetworkConfigDownloaded()
	{
		NetworkSettings.Server server = NetworkConfigManager.UnzipToJson<NetworkSettings.Server>("server.json");
		GameInit.CanContinueInit = false;
		NetworkConnection.current.OnConfigVersionUpdate(server.validVersions, delegate
		{
			GameRestarter.ShowRestartDialog("configs_downloaded");
		});
	}
}
