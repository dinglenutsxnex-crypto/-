namespace SF3.Tactics
{
	public class TacticsLogger
	{
		private static TacticsLogger _instance;

		public static bool LogMessagesEnabled;

		public static bool WarningMessagesEnabled;

		public static TacticsLogger Instance
		{
			get
			{
				return _instance ?? (_instance = new TacticsLogger());
			}
		}

		private TacticsLogger()
		{
			LogMessagesEnabled = false;
			WarningMessagesEnabled = false;
		}

		public void Log(string msg, object o)
		{
			if (LogMessagesEnabled)
			{
				Messenger.Log(msg, o);
			}
		}

		public void Warning(string msg, object o)
		{
			if (LogMessagesEnabled || WarningMessagesEnabled)
			{
				Messenger.Warning(msg, o);
			}
		}

		public void Error(string msg, object o)
		{
			Messenger.Error(msg, o);
		}
	}
}
