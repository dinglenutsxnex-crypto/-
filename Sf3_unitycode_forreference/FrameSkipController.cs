using System;
using UnityEngine;

public static class FrameSkipController
{
	private const int MaximumStepsPerFrame = 2;

	public static int MaximumFrameShift = 10;

	public static Action UpdateFrame;

	public static int CurrentFrame;

	public static void MoveToNextFrame()
	{
		int num = 0;
		if (GetFrameDelta() > MaximumFrameShift)
		{
			SyncFrameCountToTime();
		}
		do
		{
			num++;
			CurrentFrame++;
			UpdateFrame();
		}
		while (GetFrameDelta() >= 1 && num <= 2);
	}

	private static int GetFrameDelta()
	{
		return (int)(Time.realtimeSinceStartup * 60f - (float)CurrentFrame);
	}

	public static void SyncFrameCountToTime()
	{
		CurrentFrame = (int)(Time.realtimeSinceStartup * 60f);
	}
}
