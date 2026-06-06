using System;
using System.Collections.Generic;

public class BundlesLoaderController
{
	private readonly List<BundlesLoaderData> _loadBundles;

	private readonly List<NekkiWebRequest> _requests;

	private int _currentLoad;

	private bool _isError;

	public event Action<BundlesLoaderData> OnSuccessfulBundle = delegate
	{
	};

	public event Action OnSuccessful = delegate
	{
	};

	public event Action OnError = delegate
	{
	};

	public BundlesLoaderController(List<BundlesLoaderData> loadBundles)
	{
		_loadBundles = loadBundles;
		_requests = new List<NekkiWebRequest>();
	}

	public void Load()
	{
		foreach (BundlesLoaderData loadBundle in _loadBundles)
		{
			NekkiWebRequest item = NekkiWebHelper.DownloadFile(loadBundle.Url, loadBundle.SavePath, OnLoadBundle, OnErrorBundle, null, loadBundle);
			_requests.Add(item);
		}
	}

	private void OnLoadBundle(NekkiWebRequest request)
	{
		BundlesLoaderData externalData = request.GetExternalData<BundlesLoaderData>();
		this.OnSuccessfulBundle.InvokeSafe(externalData);
		Update();
	}

	private void OnErrorBundle(NekkiWebRequest request)
	{
		_isError = true;
		Update();
	}

	private void Update()
	{
		_currentLoad++;
		if (_currentLoad >= _loadBundles.Count)
		{
			if (!_isError)
			{
				this.OnSuccessful.InvokeSafe();
			}
			else
			{
				this.OnError.InvokeSafe();
			}
		}
	}
}
