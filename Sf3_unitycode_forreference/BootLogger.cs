using System;
using Nekki;
using UnityEngine;
using UnityEngine.Profiling;

public static class BootLogger
{
	private static string _file;

	private static string AllocatedMemory
	{
		get
		{
			return ((float)Profiler.GetTotalAllocatedMemory() / 1024f / 1024f).ToString("0.0");
		}
	}

	private static string HeapMemory
	{
		get
		{
			return ((float)Profiler.GetMonoHeapSize() / 1024f / 1024f).ToString("0.0");
		}
	}

	private static string UsedMemory
	{
		get
		{
			return ((float)Profiler.GetMonoUsedSize() / 1024f / 1024f).ToString("0.0");
		}
	}

	public static void Init()
	{
		if (NekkiUtils.IsDebug)
		{
			_file = string.Format("{0}/BootLog_{1}.log", GlobalPath.LogsPath, NekkiUtils.CurrentTimeAsString);
			FilesUtil.AppendFileText(_file, string.Format("Device            : {0}\n", SystemInfo.deviceName));
			FilesUtil.AppendFileText(_file, string.Format("Model             : {0}\n", SystemInfo.deviceModel));
			FilesUtil.AppendFileText(_file, string.Format("Type              : {0}\n", SystemInfo.deviceType));
			FilesUtil.AppendFileText(_file, string.Format("System            : {0}\n", SystemInfo.operatingSystem));
			FilesUtil.AppendFileText(_file, string.Format("ProcessorCount    : {0}\n", SystemInfo.processorCount));
			FilesUtil.AppendFileText(_file, string.Format("Graphics memory   : {0}\n", SystemInfo.graphicsMemorySize));
			FilesUtil.AppendFileText(_file, string.Format("System memory     : {0}\n", SystemInfo.systemMemorySize));
			FilesUtil.AppendFileText(_file, "\n");
		}
	}

	public static void Call(Action action, string name)
	{
		DateTime now = DateTime.Now;
		action();
		if (NekkiUtils.IsDebug)
		{
			double totalSeconds = (DateTime.Now - now).TotalSeconds;
			string text = string.Format("[Time:{0:0.0000} UsedMemory:{1}, HeapMemory:{2}, AllocatedMemory:{3}] {4}\n", totalSeconds, UsedMemory, HeapMemory, AllocatedMemory, name);
			FilesUtil.AppendFileText(_file, text);
			Debug.Log("BootLogger Call " + text);
		}
	}

	public static void Write(string log)
	{
		if (NekkiUtils.IsDebug)
		{
			FilesUtil.AppendFileText(_file, string.Format("{0}\n", log));
		}
	}
}
