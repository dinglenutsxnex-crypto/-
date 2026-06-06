using Newtonsoft.Json.Linq;
using SF3;
using UnityEngine;

public class SF3DeviceInfoLogger
{
	private static SF3DeviceInfoLogger _logger;

	public static SF3DeviceInfoLogger instance
	{
		get
		{
			if (_logger == null)
			{
				_logger = new SF3DeviceInfoLogger();
			}
			return _logger;
		}
	}

	public void SendLog()
	{
		JObject jObject = new JObject();
		jObject["device"] = SystemInfo.deviceModel;
		jObject["OS"] = SystemInfo.operatingSystem;
		jObject["RAM"] = SystemInfo.systemMemorySize;
		jObject["CPU"] = SystemInfo.processorCount;
		jObject["screenWidth"] = Screen.width;
		jObject["screenHeight"] = Screen.height;
		jObject["qualityPreset"] = QualityManager.PresetName;
		jObject["systemLanguage"] = Application.systemLanguage.ToString();
		jObject["subtype"] = "device_info";
		Analytics.Logger.AddEvent("APPLICATION_OPEN", jObject);
	}
}
