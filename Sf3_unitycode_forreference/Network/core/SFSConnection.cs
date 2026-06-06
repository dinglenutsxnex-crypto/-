using System;
using System.Collections;
using Google.Protobuf;
using Network.core.data;
using Network.core.events;
using Newtonsoft.Json.Linq;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities.Data;
using Sfs2X.Logging;
using Sfs2X.Requests;
using UnityEngine;

namespace Network.core
{
	public class SFSConnection : NetworkEventListner
	{
		public class Constants
		{
			public const char LoginErrorDelimiter = '_';
		}

		public Func<string, float> Timeout;

		private string _serverIP;

		private int _tcpServerPort;

		private int _udpServerPort;

		private string _zoneName;

		private int _version;

		private bool _loggedIn;

		protected SFSProtocol _protocol;

		protected SFSGameData _data;

		private SmartFox _sfs;

		private Coroutine timeOutCoroutine;

		private RequestHistory requestHistory = new RequestHistory();

		private bool ENABLE_DEBUG;

		private string[] ignoredCmdForLogging;

		public string Server
		{
			get
			{
				return _serverIP;
			}
			set
			{
				_serverIP = value;
			}
		}

		public int TCPPort
		{
			get
			{
				return _tcpServerPort;
			}
			set
			{
				_tcpServerPort = value;
			}
		}

		public int UDPPort
		{
			get
			{
				return _udpServerPort;
			}
			set
			{
				_udpServerPort = value;
			}
		}

		public string Zone
		{
			get
			{
				return _zoneName;
			}
			set
			{
				_zoneName = value;
			}
		}

		public int Version
		{
			get
			{
				return _version;
			}
			set
			{
				_version = value;
			}
		}

		public SFSGameData UserData
		{
			get
			{
				return _data;
			}
		}

		public string SessionToken
		{
			get
			{
				if (_sfs.IsConnected)
				{
					return _sfs.SessionToken;
				}
				return string.Empty;
			}
		}

		public bool IsLoggedIn
		{
			get
			{
				return _loggedIn;
			}
		}

		public event Action<int> onRequestFinished = delegate
		{
		};

		public SFSConnection()
		{
			_serverIP = string.Empty;
			_tcpServerPort = 0;
			_udpServerPort = 0;
			_zoneName = string.Empty;
		}

		public virtual bool IsConnectionActive()
		{
			return _sfs != null && _sfs.IsConnected;
		}

		public void CreateInstance()
		{
			_loggedIn = false;
			if (_sfs != null && IsConnectionActive())
			{
				Debug.LogError("<ERROR> This can be called only after we disconnected form outside.");
				return;
			}
			_sfs = new SmartFox(ENABLE_DEBUG);
			_sfs.ThreadSafeMode = true;
			_sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
			_sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
			_sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
			_sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
			_sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
			_sfs.AddEventListener(SFSEvent.LOGOUT, OnLogout);
			_sfs.AddEventListener(SFSEvent.SOCKET_ERROR, OnSocketError);
			_sfs.AddLogListener(LogLevel.ERROR, OnErrorLogMessage);
			_sfs.AddLogListener(LogLevel.WARN, OnErrorLogMessage);
			requestHistory.Clear();
		}

		private void OnWarnLogMessage(BaseEvent evt)
		{
			string text = (string)evt.Params["message"];
			Debug.LogWarning("[SFS2X Warning]" + text);
		}

		private void OnErrorLogMessage(BaseEvent evt)
		{
			string text = (string)evt.Params["message"];
			Debug.LogError("[SFS2X Error]" + text);
		}

		public virtual void Connect()
		{
			CreateInstance();
			_sfs.Connect(_serverIP, _tcpServerPort);
		}

		protected virtual void Disconnect()
		{
			Debug.Log("Disconnect");
			if (_sfs != null)
			{
				_sfs.RemoveAllEventListeners();
				if (_sfs.IsConnected)
				{
					_sfs.Disconnect();
					_sfs.HandleClientDisconnection("application_quit");
				}
				_sfs = null;
				_loggedIn = false;
			}
		}

		private void OnConnection(BaseEvent e)
		{
			if ((bool)e.Params["success"] && IsConnectionActive())
			{
				callEvent("connection", null, 0, string.Empty);
			}
			else
			{
				callEvent("connection_error", null, 0, string.Empty);
			}
		}

		private void OnConnectionLost(BaseEvent e)
		{
			callEvent("connection_lost", null, 0, string.Empty);
		}

		private void OnLogout(BaseEvent e)
		{
			callEvent("logout", null, 0, string.Empty);
		}

		private void OnSocketError(BaseEvent e)
		{
			callEvent("socket_error", null, 0, string.Empty);
		}

		private void OnLogin(BaseEvent e)
		{
			_loggedIn = true;
			callEvent("login", null, 0, UserData.Auth.Login);
		}

		private void OnLoginError(BaseEvent e)
		{
			string[] array = e.Params["errorMessage"].ToString().Split('_');
			if (array.Length < 2)
			{
				callEvent("login_error", null, 0, "LoginError: unknown server error (nothing to split)");
				return;
			}
			string name = "login_error";
			int code = int.Parse(array[0]);
			callEvent(name, null, code, string.Format("LoginError: {0} - {1}", array[0], array[1]));
		}

		private void OnExtensionResponse(BaseEvent e)
		{
			string text = (string)e.Params["cmd"];
			SFSObject sFSObject = (SFSObject)e.Params["params"];
			SFSProtocolResponse sFSProtocolResponse = _protocol.DecodeResponse(sFSObject);
			Type @class = NetworkEventManager.GetClass(text);
			if (@class == null)
			{
				return;
			}
			IMessage message = null;
			if (sFSProtocolResponse.Code == 0)
			{
				byte[] array = sFSObject.GetByteArray("b").Bytes;
				try
				{
					if (NetworkEventManager.isStateChanger(text))
					{
						array = PreprocessExtensionResponse(text, array);
					}
					message = array.FromBinary(@class);
				}
				catch (Exception ex)
				{
					Debug.LogError(string.Format("Error: Cannot parse proto for cmd: {0} Proto on Client or Server is very likely out of date. Or request should have [state_change] in sf3.proto\nOriginal Error: {1}", text, ex.Message));
					Debug.LogException(ex);
				}
			}
			int num = ((!sFSObject.ContainsKey("id")) ? (-1) : sFSObject.GetInt("id"));
			RequestObject request = requestHistory.GetRequest(num);
			requestHistory.RemoveRequest(num, false);
			NetworkEvent networkEvent = new NetworkEvent(text, message, sFSProtocolResponse.Code, sFSProtocolResponse.Message, num, (request == null) ? null : request.data);
			if (num == -1)
			{
				LogEvent(text, @class, message);
				callEvent(networkEvent);
			}
			else
			{
				if (request == null)
				{
					return;
				}
				request.StopTimeoutCoroutine();
				LogRequest("Server Response", text, "session_request_id:" + num, @class, message);
				try
				{
					request.Invoke(networkEvent);
				}
				catch (Exception)
				{
					throw;
				}
				finally
				{
					HandleCommonErrors(networkEvent);
					if (networkEvent.success)
					{
					}
					this.onRequestFinished(num);
				}
			}
		}

		protected virtual void HandleCommonErrors(NetworkEvent e)
		{
		}

		protected virtual byte[] PreprocessExtensionResponse(string cmd, byte[] bytes)
		{
			return bytes;
		}

		public virtual void Update()
		{
			if (_sfs != null)
			{
				_sfs.ProcessEvents();
			}
		}

		public virtual void RequestLogin(JObject extraData = null)
		{
			string login = UserData.Auth.Login;
			SFSObject loginRequest = _protocol.GetLoginRequest(SessionToken, login, _version, extraData);
			_sfs.Send(new LoginRequest(string.Empty, string.Empty, _zoneName, loginRequest));
		}

		public virtual void WebRequestLogin()
		{
		}

		protected virtual int send(string cmd, IMessage e, float timeout, Action<NetworkEvent> callback = null, object data = null)
		{
			SFSObject sfs = _protocol.PackObject(e);
			return send(cmd, sfs, timeout, callback, data);
		}

		private int send(string cmd, SFSObject sfs, float timeout, Action<NetworkEvent> callback = null, object data = null)
		{
			int @int = sfs.GetInt("id");
			Coroutine timeoutCoroutine = Routiner.GoDelayed(onRequestTimeout(@int), timeout);
			requestHistory.AddRequest(@int, new RequestObject
			{
				cmd = cmd,
				callback = callback,
				data = data,
				timeoutCoroutine = timeoutCoroutine
			});
			ExtensionRequest request = new ExtensionRequest(cmd, sfs);
			_sfs.Send(request);
			return @int;
		}

		private IEnumerator onRequestTimeout(int requestID)
		{
			RequestObject requestData = requestHistory.GetRequest(requestID);
			requestHistory.RemoveRequest(requestID, false);
			if (requestData != null)
			{
				if (!IsIgnoreingCMDForLogging(requestData.cmd))
				{
					Debug.LogError(string.Format("Request timeout cmd={0} id={1}", requestData.cmd, requestID.ToString()));
				}
				requestData.Invoke(NetworkEvent.createTimeoutEvent(requestData.cmd, requestID, (requestData == null) ? null : requestData.data, "SFS.onRequestTimeout"));
				this.onRequestFinished(requestID);
			}
			yield return 0;
		}

		public virtual void RemoveCallbacks(Action<NetworkEvent> callback)
		{
			requestHistory.RemoveCallbacks(callback);
		}

		public virtual void RemoveCallbacks(object target)
		{
			requestHistory.RemoveCallbacks(target);
		}

		public virtual void cancelAllRequests()
		{
			requestHistory.CancelAllRequests();
		}

		public void TestKillConnection()
		{
			if (_sfs != null)
			{
				_sfs.KillConnection();
			}
		}

		public void SetIgnoredCMDForLogging(string[] ignores)
		{
			ignoredCmdForLogging = ignores;
		}

		private bool IsIgnoreingCMDForLogging(string cmd)
		{
			if (ignoredCmdForLogging == null)
			{
				return false;
			}
			return Array.IndexOf(ignoredCmdForLogging, cmd) >= 0;
		}

		private void LogEvent(string cmd, Type type, IMessage e)
		{
			if (!IsIgnoreingCMDForLogging(cmd))
			{
				Debug.Log(string.Format("[Server Event] cmd:<b>{0}</b> type:{1} extensible:\n{2}", cmd, type.ToString(), e.ToDebugJSON()));
			}
		}

		public void LogRequest(string source, string cmd, string extraInfo, Type type, IMessage e)
		{
			if (!IsIgnoreingCMDForLogging(cmd))
			{
				Debug.Log(string.Format("[{0}] cmd:<b>{1}</b> type:{2} {3} extensible:\n{4}", source, cmd, type.ToString(), extraInfo, e.ToDebugJSON()));
			}
		}
	}
}
