public class Analytics
{
	private static Analytics _instance;

	private SF3EventLogger logger;

	private LogHandler unityLogHandler;

	private CrashLogger crashLogger;

	public static Analytics current
	{
		get
		{
			if (_instance == null)
			{
				_instance = new Analytics();
			}
			return _instance;
		}
	}

	public static BaseEventLogger Logger
	{
		get
		{
			return current.logger;
		}
	}

	public void Init()
	{
		logger = new SF3EventLogger();
		logger.Init();
		unityLogHandler = new LogHandler(logger);
		crashLogger = CrashLogger.Create();
	}

	public void DestroySelf()
	{
		crashLogger.OnQuitOrRestart();
		logger.Cleanup();
		unityLogHandler.Unsibscribe();
		_instance = null;
	}

	internal void SetClientLoggingID(long clientLogID)
	{
		logger.SetEventIDIfHighter(clientLogID);
	}

	public void OnModuleChange(IntentModule intent)
	{
		crashLogger.OnModuleChange(intent.TransitionData.FromData.Name, intent.TransitionData.ToData.Name);
	}
}
