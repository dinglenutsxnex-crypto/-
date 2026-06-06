using System;

public class NetworkConfigsDownloader
{
	public Action onSuccess = delegate
	{
	};

	public Action onFail = delegate
	{
	};

	public NekkiWebRequest Download(string url, string savePath = "", bool checkCert = true)
	{
		if (savePath.IsNullOrEmpty())
		{
			savePath = NetworkConfigManager.ConfigPath;
		}
		string path = savePath;
		Action<NekkiWebRequest> onSuccessful = OnConfigDownloaded;
		Action<NekkiWebRequest> onError = OnConfigError;
		bool checkCert2 = checkCert;
		return NekkiWebHelper.DownloadFile(url, path, onSuccessful, onError, null, null, 5f, checkCert2);
	}

	private void OnConfigDownloaded(NekkiWebRequest web)
	{
		NetworkConfigManager.Init();
		onSuccess.InvokeSafe();
	}

	private void OnConfigError(NekkiWebRequest web)
	{
		NetworkConfigManager.Init();
		onFail.InvokeSafe();
	}
}
