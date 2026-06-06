using System;
using UnityEngine;

namespace SF3.Audio
{
	[Serializable]
	public class AudioSourceSettings
	{
		[Serializable]
		public class AudioCurve
		{
			public bool apply;

			public AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 0f);
		}

		public float dopplerLevel = 1f;

		public float spread;

		public AudioRolloffMode volumeRolloff;

		public float minDistance = 1f;

		public float maxDistance = 500f;

		public AudioCurve volumeCurve;

		public AudioCurve spatialCurve;

		public AudioCurve spreadCurve;

		public AudioCurve reverbCurve;

		public void ApplySettings(AudioSource applyTo)
		{
			applyTo.dopplerLevel = dopplerLevel;
			applyTo.spread = spread;
			applyTo.rolloffMode = volumeRolloff;
			applyTo.maxDistance = maxDistance;
			applyTo.minDistance = minDistance;
			if (volumeCurve.apply)
			{
				applyTo.SetCustomCurve(AudioSourceCurveType.CustomRolloff, volumeCurve.curve);
			}
			if (spatialCurve.apply)
			{
				applyTo.SetCustomCurve(AudioSourceCurveType.SpatialBlend, spatialCurve.curve);
			}
			if (spreadCurve.apply)
			{
				applyTo.SetCustomCurve(AudioSourceCurveType.Spread, spreadCurve.curve);
			}
			if (reverbCurve.apply)
			{
				applyTo.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, reverbCurve.curve);
			}
		}
	}
}
