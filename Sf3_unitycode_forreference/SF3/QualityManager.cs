using System;
using System.Collections.Generic;
using SF3.UserData;
using UnityEngine;

namespace SF3
{
	public class QualityManager
	{
		public enum ChangeType
		{
			NOT_FOUND = 0,
			NOT_AVAILABLE = 1,
			INSTALLED = 2,
			SUCCESS = 3
		}

		private Dictionary<string, DevicePreset> _availablePresets = new Dictionary<string, DevicePreset>();

		private DevicePreset _currentPreset;

		private readonly QualityDeviceTypes _currentDeviceType;

		private readonly QualityPresets _qualityPresets;

		private bool _shadowForcedOff;

		public Action onQualityLvlChange;

		public static DevicePreset Preset
		{
			get
			{
				return Instance._currentPreset;
			}
		}

		public static string PresetName
		{
			get
			{
				return Preset.name;
			}
		}

		public static QualityDeviceTypes DeviceType
		{
			get
			{
				return Instance._currentDeviceType;
			}
		}

		public static bool IsTablet
		{
			get
			{
				return DeviceType == QualityDeviceTypes.TABLET;
			}
		}

		public float ScaleFactor
		{
			get
			{
				return (!IsTablet) ? 1f : 0.7f;
			}
		}

		public static Dictionary<string, DevicePreset> Availables
		{
			get
			{
				return Instance._availablePresets;
			}
		}

		public static QualityManager Instance { get; private set; }

		public bool IsShadowForcedOff
		{
			get
			{
				return _shadowForcedOff;
			}
		}

		public QualityManager()
		{
			_shadowForcedOff = false;
			_qualityPresets = ConfigsSourceResolver.QualitySettings;
			_qualityPresets.InitPreset();
			DevicePreset presetByName = _qualityPresets.GetPresetByName(UserManager.GetPresetQuality());
			DeviceQuality device = _qualityPresets.GetDevice();
			DevicePreset conditionPreset = _qualityPresets.GetConditionPreset();
			_currentDeviceType = ((device == null) ? _qualityPresets.GetConditionDeviceType() : device.type);
			if (presetByName != null && !presetByName.IsPriority(conditionPreset))
			{
				_currentPreset = presetByName;
				_currentPreset.subType = SubTypes.USER;
			}
			else if (device != null)
			{
				_currentPreset = device.preset;
				_currentPreset.subType = SubTypes.DEVICE;
			}
			else
			{
				_currentPreset = conditionPreset;
			}
			SetAvailablePresets(_currentPreset);
			ApplyPreset();
		}

		public static void Init()
		{
			if (Instance == null)
			{
				Instance = new QualityManager();
			}
		}

		public static ChangeType ChangePreset(string name, bool forcibly = false, bool save = false)
		{
			if (PresetName.Equals(name))
			{
				return ChangeType.INSTALLED;
			}
			DevicePreset presetByName = Instance._qualityPresets.GetPresetByName(name);
			if (presetByName == null)
			{
				return ChangeType.NOT_FOUND;
			}
			DevicePreset availablePresetByName = Instance.GetAvailablePresetByName(name);
			if (!forcibly && availablePresetByName == null)
			{
				return ChangeType.NOT_AVAILABLE;
			}
			Instance._currentPreset = (forcibly ? presetByName : availablePresetByName);
			Instance.ApplyPreset();
			Instance.SetAvailablePresets(Instance._currentPreset);
			if (save)
			{
				Instance.SavePreset();
			}
			return ChangeType.SUCCESS;
		}

		public void SetShadowForcedOff(bool enable)
		{
			_shadowForcedOff = enable;
			onQualityLvlChange.InvokeSafe();
		}

		private void ApplyPreset()
		{
			QualitySettings.masterTextureLimit = (int)_currentPreset.textureResolution;
			QualitySettings.anisotropicFiltering = (_currentPreset.anisotropicFiltering ? AnisotropicFiltering.ForceEnable : AnisotropicFiltering.Disable);
			QualitySettings.antiAliasing = (_currentPreset.antiAliasing ? 2 : 0);
			if (onQualityLvlChange != null)
			{
				onQualityLvlChange();
			}
			Debug.Log("CurrentQualityPreset " + _currentPreset.name);
			Debug.Log("CurrentDeviceType " + _currentDeviceType);
		}

		private void SetAvailablePresets(DevicePreset conditionPreset)
		{
			_availablePresets.Clear();
			foreach (KeyValuePair<string, DevicePreset> preset in _qualityPresets.Presets)
			{
				if (preset.Value.priority <= conditionPreset.priority)
				{
					_availablePresets.Add(preset.Key, preset.Value);
				}
			}
		}

		private DevicePreset GetAvailablePresetByName(string name)
		{
			if (_availablePresets.ContainsKey(name))
			{
				return _availablePresets[name];
			}
			return null;
		}

		private void SavePreset()
		{
			UserManager.SetPresetQuality(_currentPreset.name);
		}
	}
}
