using System.Collections.Generic;
using System.IO;
using Nekki.Zip;
using Newtonsoft.Json;
using UnityEngine;

public class NetworkConfigManager
{
	public const string CONFIG_NAME = "archive.bytes";

	public static ConfigsSourceResolver.ProxyObject ProxyObject
	{
		get
		{
			return new ConfigsSourceResolver.ProxyObject(UnzipToString, UnzipToBinary);
		}
	}

	public static string ConfigPath
	{
		get
		{
			return GlobalPath.ExternalPath + "/Configs/archive.bytes";
		}
	}

	public static string ApplicationConfigPath
	{
		get
		{
			return GlobalPath.PathToLoaderFolder + "/Configs/archive.bytes";
		}
	}

	public static NetworkSettings.ConfigVersion GetInBuildVersion()
	{
		byte[] loadBytes = GlobalLoad.GetLoadBytes("Configs/archive");
		if (loadBytes == null)
		{
			return null;
		}
		Stream stream = new MemoryStream(loadBytes);
		string value = ZipUtils.UnzipToString(stream, "version.json");
		return JsonConvert.DeserializeObject<NetworkSettings.ConfigVersion>(value);
	}

	private static bool isVersionInBuildGreater()
	{
		NetworkSettings.ConfigVersion configVersion = UnzipToJson<NetworkSettings.ConfigVersion>("version.json");
		NetworkSettings.ConfigVersion inBuildVersion = GetInBuildVersion();
		if (Application.isEditor || InternalSettings.IsDebug)
		{
			return inBuildVersion.serverName == configVersion.serverName && inBuildVersion.version > configVersion.version;
		}
		return inBuildVersion.serverName != configVersion.serverName || inBuildVersion.version > configVersion.version;
	}

	private static void CopyFromBuild()
	{
		byte[] loadBytes = GlobalLoad.GetLoadBytes("Configs/archive");
		FilesUtil.WriteFileBytes(ConfigPath, loadBytes);
	}

	private static bool CopyFromBuildIfNeeded()
	{
		if (!FilesUtil.IsFileExists(ConfigPath))
		{
			CopyFromBuild();
			return false;
		}
		if (isVersionInBuildGreater())
		{
			CopyFromBuild();
			return true;
		}
		return false;
	}

	public static void Init()
	{
		if (CopyFromBuildIfNeeded())
		{
			NetworkSettings.Server server = UnzipToJson<NetworkSettings.Server>("server.json");
			NetworkConnection.current.OnConfigVersionUpdate(server.validVersions);
		}
		SetSettings();
	}

	private static void SetSettings()
	{
		NetworkSettings networkSettings = new NetworkSettings();
		networkSettings.config = UnzipToJson<NetworkSettings.Config>("config.json");
		networkSettings.version = UnzipToJson<NetworkSettings.ConfigVersion>("version.json");
		NetworkConnection.current.setSettings(networkSettings);
		BundlesController.Instance.Init();
	}

	public static T UnzipToJson<T>(string filename) where T : class
	{
		string text = ZipUtils.UnzipToString(ConfigPath, filename);
		if (text == null)
		{
			return (T)null;
		}
		return JsonConvert.DeserializeObject<T>(text);
	}

	public static string UnzipToString(string filename)
	{
		return ZipUtils.UnzipToString(ConfigPath, filename);
	}

	public static byte[] UnzipToBinary(string filename)
	{
		return ZipUtils.UnzipToBinary(ConfigPath, filename);
	}

	public static Dictionary<string, string> UnzipToStrings(List<string> fileNames = null)
	{
		return ZipUtils.UnzipToStrings(ConfigPath, fileNames);
	}
}
