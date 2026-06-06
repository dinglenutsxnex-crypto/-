using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class LogHandler
{
	private readonly Filters filters = new Filters();

	private readonly BaseEventLogger eventLogger;

	private bool isApplicationQuiting;

	private HashSet<byte[]> loggedErrors = new HashSet<byte[]>(new ByteArrayComparer());

	private MD5 md5;

	public LogHandler(BaseEventLogger _eventLogger)
	{
		eventLogger = _eventLogger;
		md5 = MD5.Create();
		Application.logMessageReceived += HandleLog;
		StaticObjectsManager.OnApplicationQuitEvent += delegate
		{
			isApplicationQuiting = true;
		};
		filters.Add(LogType.Error, StartsWith.Instance, "Server Request Error");
		filters.Add(LogType.Error, StartsWith.Instance, "[SFS2X Error]TCPSocketLayer: General error reading data from socket: Read failure");
		filters.Add(LogType.Error, StartsWith.Instance, "Cannot send before player was inited or we");
		filters.Add(LogType.Error, StartsWith.Instance, "[SFS2X Error]TCPSocketLayer: Connection error:");
		filters.Add(LogType.Error, EndsWith.Instance, "message=request canceled");
		filters.Add(LogType.Error, EndsWith.Instance, "cannot call right now");
		filters.Add(LogType.Error, EndsWith.Instance, "Connection closed by the remote side");
		filters.Add(LogType.Error, StartsWith.Instance, "Not all possible errors were handled");
		filters.Add(LogType.Error, StartsWith.Instance, "AddGeneralData");
		filters.Add(LogType.Error, ContainsAnyCase.Instance, "assets/gamedata/resources/NativeUI/");
	}

	~LogHandler()
	{
		Unsibscribe();
	}

	public void Unsibscribe()
	{
		Application.logMessageReceived -= HandleLog;
	}

	private void HandleLog(string message, string stackTrace, LogType type)
	{
		if (!InternalSettings.IsDebug && !isApplicationQuiting)
		{
			switch (type)
			{
			case LogType.Log:
				break;
			case LogType.Error:
				AddLog(LogType.Error, message, stackTrace);
				break;
			case LogType.Assert:
			case LogType.Exception:
				AddLog(LogType.Exception, message, stackTrace);
				break;
			case LogType.Warning:
				AddWarning(message);
				break;
			}
		}
	}

	private void AddLog(LogType type, string message, string trace)
	{
		if (!filters.Filtered(type, message))
		{
			AddEvent(GetEventNameByType(type), message, trace);
		}
	}

	private void AddEvent(string etype, string message, string trace)
	{
		byte[] item = md5.ComputeHash(Encoding.UTF8.GetBytes(string.Format("{0}\n{1}", message, trace)));
		if (!loggedErrors.Contains(item))
		{
			loggedErrors.Add(item);
			JObject jObject = new JObject();
			jObject.Add("msg", message);
			jObject.Add("trc", trace);
			JObject eventData = jObject;
			eventLogger.AddEvent(etype, eventData);
		}
	}

	private void AddWarning(string message)
	{
		if (InternalSettings.Local.HandleLogWarnings)
		{
			eventLogger.AddEvent("WARNING", "msg", message);
		}
	}

	private static string GetEventNameByType(LogType type)
	{
		switch (type)
		{
		case LogType.Error:
			return LogErrorTypes.Error;
		case LogType.Assert:
		case LogType.Exception:
			return LogErrorTypes.Crash;
		case LogType.Warning:
			return LogErrorTypes.Warning;
		default:
			return string.Empty;
		}
	}
}
