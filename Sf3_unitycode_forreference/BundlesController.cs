using System;
using System.Collections.Generic;
using UnityEngine;

public class BundlesController
{
	private static string _bundlesPath;

	private string _configPath;

	private BundleConfig _currentConfig;

	private static BundlesController _instance;

	public static BundlesController Instance
	{
		get
		{
			return _instance ?? (_instance = new BundlesController());
		}
	}

	public event Action OnSuccessfulBundles = delegate
	{
	};

	public event Action OnErrorBundles = delegate
	{
	};

	public void Init()
	{
		_bundlesPath = GlobalPath.ExternalPath + "/Bundles/";
		_configPath = _bundlesPath + "bundlesConfig.json";
	}

	public void LoadBundles()
	{
		this.OnSuccessfulBundles.InvokeSafe();
	}

	private void LoadCurrentBundles(List<BundlesLoaderData> notAvailables)
	{
		BundlesLoaderController bundlesLoaderController = new BundlesLoaderController(notAvailables);
		bundlesLoaderController.OnSuccessfulBundle += OnSuccessfulCurrentLoader;
		bundlesLoaderController.OnSuccessful += OnSuccessfulLoader;
		bundlesLoaderController.OnError += OnErrorLoader;
		bundlesLoaderController.Load();
	}

	private void OnSuccessfulCurrentLoader(BundlesLoaderData loaderData)
	{
		_currentConfig.SetAvailable(loaderData.Name, true);
		_currentConfig.Save(_configPath);
	}

	private void OnSuccessfulLoader()
	{
		Debug.Log("OnLoadBundles");
		_currentConfig.BundlesPath = _bundlesPath;
		BundlesUtil.InitConfig(_currentConfig);
		this.OnSuccessfulBundles.InvokeSafe();
	}

	private void OnErrorLoader()
	{
		Debug.LogError("OnErrorLoadBundles");
		this.OnErrorBundles.InvokeSafe();
		OnSuccessfulLoader();
	}

	private void SetCurrentConfig(string path)
	{
		BundleConfig currentConfig = BundleConfig.CreateFromFile(path);
		SetCurrentConfig(currentConfig);
	}

	private void SetCurrentConfig(BundleConfig config)
	{
		_currentConfig = config;
		if (_currentConfig != null)
		{
			_currentConfig.BundlesPath = _bundlesPath;
		}
	}
}
