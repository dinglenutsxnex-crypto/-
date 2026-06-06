using System;
using System.Collections.Generic;
using Google.Protobuf;
using UnityEngine;
using common;
using sf3DTO;

namespace Network.core.events
{
	public class NetworkEvent
	{
		public const string CONNECTION = "connection";

		public const string CONNECTION_ERROR = "connection_error";

		public const string CONNECTION_LOST = "connection_lost";

		public const string LOGOUT = "logout";

		public const string SOCKET_ERROR = "socket_error";

		public const string LOGIN = "login";

		public const string LOGIN_ERROR = "login_error";

		public const string TIMEOUT_ERROR = "timeout_error";

		private bool didHandleError;

		public string name { get; private set; }

		private IMessage extensible { get; set; }

		private int errorCode { get; set; }

		public string message { get; private set; }

		public int requestID { get; private set; }

		public object data { get; set; }

		public List<string> UnhandledErrors { get; private set; }

		public bool success
		{
			get
			{
				return errorCode.IsEqual(common.RequestErrorCode.Ok);
			}
		}

		public bool WasErrorHandled
		{
			get
			{
				return success || didHandleError;
			}
		}

		public bool isClientError
		{
			get
			{
				return errorCode.IsEqual(ClientRequestError.ClientError);
			}
		}

		public bool isClientTimeout
		{
			get
			{
				return errorCode.IsEqual(ClientRequestError.ClientTimeout);
			}
		}

		public bool isClientCancel
		{
			get
			{
				return errorCode.IsEqual(ClientRequestError.ClientCanceled);
			}
		}

		public bool isAnyClientIssue
		{
			get
			{
				return isClientError || isClientTimeout || isClientCancel;
			}
		}

		public bool isAnyServerIssue
		{
			get
			{
				return errorCode > 0;
			}
		}

		public NetworkEvent(string name, IMessage extensible, int errorCode, string message, int requestID, object data)
		{
			this.name = name;
			this.extensible = extensible;
			this.errorCode = errorCode;
			this.message = message;
			this.requestID = requestID;
			this.data = data;
			UnhandledErrors = NetworkEventManager.GetPossibleErrors(name);
		}

		public T getExtensible<T>() where T : IMessage
		{
			return (T)extensible;
		}

		public bool HandleError(Enum _code)
		{
			int num = Convert.ToInt32(_code);
			UnhandledErrors.Remove(ErrorCodeAsString(num));
			if (errorCode == num)
			{
				didHandleError = true;
				return true;
			}
			return false;
		}

		public bool HandleError(params Enum[] _codes)
		{
			bool flag = false;
			foreach (Enum code in _codes)
			{
				flag |= HandleError(code);
			}
			return flag;
		}

		public void HandleErrorAsDialog(params Enum[] codes)
		{
			foreach (Enum code in codes)
			{
				string alias = ErrorCodeAsAlias(code);
				if (HandleError(code))
				{
					ShowError(alias);
				}
			}
		}

		private static string ErrorCodeAsAlias(Enum code)
		{
			return "error_" + ErrorCodeAsString(code);
		}

		public static void ShowError(Enum code)
		{
			ShowError(ErrorCodeAsAlias(code));
		}

		private static void ShowError(string alias)
		{
			SystemMessage systemMessage = SystemMessage.ShowAlert(alias, "error");
			systemMessage.SetTextColor(UnityEngine.Color.red);
			systemMessage.AddButton("ok");
			systemMessage.SetBlockPriority(UIBlocker.Priority.ServerSystemAlert);
			systemMessage.Show();
		}

		public static NetworkEvent createTimeoutEvent(string cmd, int requestID, object data, string timeout_source)
		{
			return new NetworkEvent(cmd, null, -100, "request timeout. source:" + timeout_source, requestID, data);
		}

		public static NetworkEvent createErrorEvent(string cmd, object data, string errorMessage)
		{
			return new NetworkEvent(cmd, null, -101, errorMessage, -1, data);
		}

		public static NetworkEvent createCancelEvent(string cmd, object data, string cancel_source = "request canceled")
		{
			return new NetworkEvent(cmd, null, -102, cancel_source, -1, data);
		}

		public string ErrorTypeAsString()
		{
			if (isAnyClientIssue)
			{
				return "Client Request Error";
			}
			if (isAnyServerIssue)
			{
				return "Server Request Error";
			}
			if (success)
			{
				return "No Error";
			}
			return "Unknown Error Type";
		}

		public string ErrorCodeAsString()
		{
			return ErrorCodeAsString(errorCode);
		}

		public static string ErrorCodeAsString(Enum code)
		{
			return ErrorCodeAsString(Convert.ToInt32(code));
		}

		public static string ErrorCodeAsString(int code)
		{
			string enumKey = GetEnumKey<ClientRequestError>(code);
			if (!string.IsNullOrEmpty(enumKey))
			{
				return enumKey;
			}
			enumKey = GetEnumKey<common.RequestErrorCode>(code);
			if (!string.IsNullOrEmpty(enumKey))
			{
				return enumKey;
			}
			enumKey = GetEnumKey<sf3DTO.RequestErrorCode>(code);
			if (!string.IsNullOrEmpty(enumKey))
			{
				return enumKey;
			}
			return code.ToString();
		}

		public static string GetEnumKey<T>(int number)
		{
			foreach (int value in Enum.GetValues(typeof(T)))
			{
				if (value == number)
				{
					return ((T)(object)number).ToString();
				}
			}
			return string.Empty;
		}
	}
}
