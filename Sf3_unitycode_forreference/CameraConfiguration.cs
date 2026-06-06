using System;
using UnityEngine;

public class CameraConfiguration : MonoBehaviour
{
	public enum AspectRatio
	{
		_4X3 = 0,
		_16X9 = 1
	}

	[Serializable]
	public class CameraSettings
	{
		public AspectRatio aspect;

		public float minXPosition;

		public float maxXPosition;

		public float minYPosition;

		public float maxYPosition;

		public float startFollowYUp;

		public float startFollowYBot;

		public float startRotateYUp;

		public float startRotateYBot;

		public float farClipPlane;

		public float botVerticalAngle;

		public float upVerticalAngle;

		public float leftHorizontalAngle;

		public float rightHorizontalAngle;

		public float camZOffset;
	}

	public CameraSettings[] settings;

	public static CameraSettings Current { get; private set; }

	private void Awake()
	{
		Current = settings[0];
		float aspect = Camera.main.aspect;
		if (aspect >= 1.5f)
		{
			Current = GetSettingsByAspect(AspectRatio._16X9);
		}
		else if (aspect >= 1.3f)
		{
			Current = GetSettingsByAspect(AspectRatio._4X3);
		}
		Debug.Log(string.Format("<color=red>Current camera aspect [{0}]</color>", Current.aspect));
	}

	private CameraSettings GetSettingsByAspect(AspectRatio aspect)
	{
		CameraSettings[] array = settings;
		foreach (CameraSettings cameraSettings in array)
		{
			if (cameraSettings.aspect == aspect)
			{
				return cameraSettings;
			}
		}
		return null;
	}
}
