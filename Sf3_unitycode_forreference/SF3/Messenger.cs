using System;
using UnityEngine;

namespace SF3
{
	public class Messenger
	{
		public static void Error(string msg, object o)
		{
			Error(string.Format("<{0}> {1}", o.GetType().Name, msg));
		}

		public static void Error(string msg)
		{
			Message(msg, StringWrapperPurpose.Error);
		}

		public static void Warning(string msg, object o)
		{
			Warning(string.Format("<{0}> {1}", o.GetType().Name, msg));
		}

		public static void Warning(string msg)
		{
			Message(msg, StringWrapperPurpose.Warning);
		}

		public static void Log(string msg, object o)
		{
			Log(string.Format("<{0}> {1}", o.GetType().Name, msg));
		}

		public static void Log(string msg)
		{
			Message(msg, StringWrapperPurpose.Log);
		}

		private static void Message(string msg, StringWrapperPurpose purpose)
		{
			msg = StringWrapper.Create(purpose, msg).ToString();
			switch (purpose)
			{
			case StringWrapperPurpose.Log:
				Debug.Log(msg);
				break;
			case StringWrapperPurpose.Warning:
				Debug.LogWarning(msg);
				break;
			case StringWrapperPurpose.Error:
				Debug.LogError(msg);
				break;
			default:
				Debug.LogError(StringWrapper.Create(StringWrapperPurpose.Error, string.Format("The <{0}> type [{1}] is not supported by <{2}>", typeof(StringWrapperPurpose).Name, purpose.ToString(), typeof(Messenger).Name)).ToString());
				throw new ArgumentOutOfRangeException("purpose", purpose, null);
			}
		}
	}
}
