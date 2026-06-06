using System;
using System.Collections.Generic;
using UnityEngine;
using sf3DTO;

namespace Network.core.events
{
	public class NetworkEventManager
	{
		private static Dictionary<string, EventType> _subscription;

		private static List<string> _commonErrors;

		private static readonly int COMMON_ERROS_MIN;

		private static readonly int COMMON_ERROS_MAX_EXCLUDED;

		static NetworkEventManager()
		{
			COMMON_ERROS_MIN = 10000;
			COMMON_ERROS_MAX_EXCLUDED = 20000;
			Reset();
			_commonErrors = GetCommonErrors();
		}

		private static List<string> GetCommonErrors()
		{
			List<string> list = new List<string>();
			list.Add(Enum.GetName(typeof(ClientRequestError), ClientRequestError.ClientTimeout));
			foreach (int value in Enum.GetValues(typeof(RequestErrorCode)))
			{
				if (value >= COMMON_ERROS_MIN && value < COMMON_ERROS_MAX_EXCLUDED)
				{
					list.Add(Enum.GetName(typeof(RequestErrorCode), value));
				}
			}
			return list;
		}

		public static void Reset()
		{
			_subscription = new Dictionary<string, EventType>();
		}

		public static void Add(string name, Type type)
		{
			if (!_subscription.ContainsKey(name))
			{
				_subscription.Add(name, new EventType(type));
			}
		}

		public static void Add(string name, Type _sendType, Type _responseType, List<string> tags, List<string> possibleErrors)
		{
			if (!_subscription.ContainsKey(name))
			{
				_subscription.Add(name, new EventType(_sendType, _responseType, tags, possibleErrors));
			}
		}

		public static Type GetClass(string name)
		{
			if (_subscription.ContainsKey(name))
			{
				return _subscription[name].responseType;
			}
			Debug.LogError("NetworkEvent Error: can't find class with name: " + name);
			return null;
		}

		public static Type GetSendType(string cmd)
		{
			if (_subscription.ContainsKey(cmd))
			{
				return _subscription[cmd].sendType;
			}
			Debug.LogError("NetworkEvent Error: can't find class with name: " + cmd);
			return null;
		}

		public static bool isStateChanger(string name)
		{
			if (_subscription.ContainsKey(name))
			{
				return _subscription[name].isStateChanger;
			}
			Debug.LogError("NetworkEvent Error: can't find class with name: " + name);
			return false;
		}

		public static List<string> GetPossibleErrors(string name)
		{
			if (_subscription.ContainsKey(name))
			{
				List<string> list = new List<string>(_subscription[name].PossibleErrors);
				list.AddRange(_commonErrors);
				return list;
			}
			return new List<string>();
		}
	}
}
