using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class CrashLogger
{
	private class AppState
	{
		public bool wasSuspended = true;

		public bool wasClosed = true;

		public string lastTransition = string.Empty;

		public float memoryAllocated;

		public float memoryMonoHeap;

		public float memoryMonoUsed;

		public void Reset()
		{
			wasClosed = false;
			wasSuspended = false;
			lastTransition = string.Empty;
		}

		public void Refresh()
		{
			memoryAllocated = SystemProperties.AllocatedMemory;
			memoryMonoHeap = SystemProperties.MonoHeapSize;
			memoryMonoUsed = SystemProperties.MonoUsedSize;
		}
	}

	private AppState state;

	private static string filePath
	{
		get
		{
			return GlobalPath.GameDataCombine("User/crash_log.dat");
		}
	}

	private CrashLogger()
	{
		StaticObjectsManager.OnApplicationPauseEvent += OnApplicationPauseEvent;
		StaticObjectsManager.OnApplicationResumeEvent += OnApplicationResumeEvent;
		StaticObjectsManager.OnApplicationQuitEvent += OnQuitOrRestart;
		state = LoadState();
		if (!state.wasSuspended && !state.wasClosed)
		{
			LogUnknownCrash();
		}
		ResetState();
		state.Refresh();
	}

	public static CrashLogger Create()
	{
		return new CrashLogger();
	}

	private void ResetState()
	{
		state.Reset();
		SaveState();
	}

	public void OnQuitOrRestart()
	{
		state.wasClosed = true;
		SaveState();
		StaticObjectsManager.OnApplicationPauseEvent -= OnApplicationPauseEvent;
		StaticObjectsManager.OnApplicationResumeEvent -= OnApplicationResumeEvent;
		StaticObjectsManager.OnApplicationQuitEvent -= OnQuitOrRestart;
	}

	private void LogUnknownCrash()
	{
		JObject jObject = new JObject();
		jObject.Add("msg", "Unknown Crash Happened");
		jObject.Add("trns", state.lastTransition);
		jObject.Add("alloc", state.memoryAllocated);
		jObject.Add("h_size", state.memoryMonoHeap);
		jObject.Add("h_used", state.memoryMonoUsed);
		Analytics.Logger.AddEvent(LogErrorTypes.Crash, jObject);
	}

	private void OnApplicationResumeEvent()
	{
		state.wasSuspended = false;
		SaveState();
	}

	private void OnApplicationPauseEvent()
	{
		state.wasSuspended = true;
		SaveState();
	}

	private void SaveState()
	{
		state.Refresh();
		FilesUtil.WriteFileText(filePath, JsonConvert.SerializeObject(state, Formatting.None));
	}

	private static AppState LoadState()
	{
		if (FilesUtil.IsFileExists(filePath))
		{
			return JsonConvert.DeserializeObject<AppState>(FilesUtil.ReadFileText(filePath));
		}
		return new AppState();
	}

	public void OnModuleChange(string from, string to)
	{
		string text = from + "->" + to;
		state.lastTransition = text ?? string.Empty;
		SaveState();
	}
}
