using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Network.core;
using Network.core.events;
using SF3;
using SF3.UserData;
using UnityEngine;
using common;
using sf3DTO;

public class NetworkConnection : NetworkConnectionBase
{
	internal static readonly bool shouldAutoConnect = true;

	private static NetworkConnection _instance;

	private NetworkInputBlocker inputBlocker;

	private OfflineRequestQueue offlineQueue = new OfflineRequestQueue();

	private GenericRequestQueue genericQueue;

	private GenericRequestQueue eventQueue;

	private List<GenericRequestQueue> queues;

	private TimeDispatcher timeDispatcher = new TimeDispatcher();

	private NetworkChain networkChain;

	private bool isPlayerInited;

	private readonly List<string> cmdToAllowBeforeInit = new List<string>();

	private readonly LayerableCommand _layerablePocket = LayerableCommand.Create(typeof(NetworkConnection).Name);

	private NetworkSettings settings = new NetworkSettings();

	public bool InsideDarkPocket
	{
		get
		{
			return _layerablePocket.IsAnyLayerActive;
		}
	}

	public static NetworkConnection current
	{
		get
		{
			if (_instance == null)
			{
				_instance = new NetworkConnection();
			}
			return _instance;
		}
	}

	public static SFSSF3Protocol protocol
	{
		get
		{
			return (SFSSF3Protocol)current._protocol;
		}
	}

	public static NetworkSettings.Config Settings
	{
		get
		{
			return current.settings.config;
		}
	}

	public static string BundlesBaseURL
	{
		get
		{
			return current.settings.config.BundlesBaseURL;
		}
	}

	public NetworkSettings.ConfigVersion CurrentConfigVersion
	{
		get
		{
			return settings.getCurrentVersion();
		}
	}

	public string CurrentMarketVersion
	{
		get
		{
			return settings.version.marketVersionToString;
		}
	}

	internal static TimeDispatcher TimeDispatcher
	{
		get
		{
			return current.timeDispatcher;
		}
	}

	internal static string CurrentStateName
	{
		get
		{
			return current.networkChain.GetCurrentStateName();
		}
	}

	internal long CurrentPlayerStateIDForLogging
	{
		get
		{
			return (offlineQueue == null) ? 0 : offlineQueue.GetCurrentStateID();
		}
	}

	internal EventListener<CallEventArgsBase<System.Type>, System.Type> OnStateStartEvent
	{
		get
		{
			return networkChain.OnStateStartEvent;
		}
	}

	public event Action OnNetworkEstablished = delegate
	{
	};

	public event Action OnNetworkCutoff = delegate
	{
	};

	public event Action<ExtendedPlayer> OnPlayerUpdate = delegate
	{
	};

	public NetworkConnection()
	{
		networkChain = new NetworkChain(base.Disconnect);
		inputBlocker = new NetworkInputBlocker(this);
	}

	public void EnterDarkPocket()
	{
		UnityEngine.Debug.Log("Entering dark pocket");
		_layerablePocket.AddLayer(delegate
		{
			System.Type currentStateType = current.networkChain.GetCurrentStateType();
			if (currentStateType != NetworkState.Active && currentStateType != null)
			{
				StopCurrentState();
			}
		});
	}

	public void ExitDarkPocket()
	{
		UnityEngine.Debug.Log("Exiting dark pocket");
		_layerablePocket.RemoveLayer(delegate
		{
			System.Type currentStateType = current.networkChain.GetCurrentStateType();
			if (currentStateType != NetworkState.Active && currentStateType != null)
			{
				RestartConnection("Dark pocket exti", false);
			}
		});
	}

	public static bool HasInstance()
	{
		return _instance != null;
	}

	public bool IsNetworkEstablished()
	{
		return IsConnectionActive() && networkChain.GetCurrentStateType() == NetworkState.Active;
	}

	public void Init()
	{
		cmdToAllowBeforeInit.Add("get_player");
		cmdToAllowBeforeInit.Add("create_player");
		cmdToAllowBeforeInit.Add("ping");
		genericQueue = new GenericRequestQueue(send, delegate(Action<NetworkEvent> callback)
		{
			_003CRemoveCallbacks_003E__BaseCallProxy0(callback);
		}, delegate(object target)
		{
			_003CRemoveCallbacks_003E__BaseCallProxy1(target);
		}, pingManager);
		GenericRequestQueue genericRequestQueue = genericQueue;
		genericRequestQueue.OnRequestFinished = (Action<NetworkEvent>)Delegate.Combine(genericRequestQueue.OnRequestFinished, new Action<NetworkEvent>(OnGenericRequestFinished));
		eventQueue = new GenericRequestQueue(send, delegate(Action<NetworkEvent> callback)
		{
			_003CRemoveCallbacks_003E__BaseCallProxy0(callback);
		}, delegate(object target)
		{
			_003CRemoveCallbacks_003E__BaseCallProxy1(target);
		}, pingManager);
		queues = new List<GenericRequestQueue> { genericQueue, eventQueue };
		inputBlocker.Init();
		timeDispatcher.Start(pingManager);
		Init(new SFSSF3Protocol(), new SFSGameData(new SFSSF3Auth()), "sf3", 2, Settings.GetTimeoutForCmd);
		addEventListener("refresh_config_event", OnConfigRefreshed);
		addEventListener("kick", OnPlayerKicked);
		OnNetworkEstablished += OnNetworkEstablishedCallback;
		networkChain.OnStateSuccessEvent.addEventListener(NetworkState.JoinZone, OnJoinedZone);
		networkChain.OnStateStartEvent.addEventListener(NetworkState.Active, OnActiveStateStarted);
		networkChain.OnStateStartEvent.addEventListener(NetworkState.GetPlayer, OnGetPlayerStarted);
		networkChain.OnStateEndedWithoutSuccess += delegate
		{
			reset();
			this.OnNetworkCutoff();
		};
		SetupLogging();
		StartConnectionProcess();
	}

	private void SetupLogging()
	{
		SetIgnoredCMDForLogging(InternalSettingsSF3.LogIgnoreCmd);
	}

	private void OnActiveStateStarted(CallEventArgsBase<System.Type> obj)
	{
		this.OnNetworkEstablished();
	}

	private void OnJoinedZone(CallEventArgsBase<System.Type> obj)
	{
		joined_zone = true;
		pingManager.startPing();
	}

	private void OnConfigRefreshed(NetworkEvent e)
	{
		NetworkSettings.ConfigVersion configVersion = new NetworkSettings.ConfigVersion(e.getExtensible<StringValue>().Value);
		if (configVersion.version > settings.getCurrentVersion().version)
		{
			StopCurrentState();
		}
		else
		{
			UnityEngine.Debug.LogError("Error: server sending update with version equal or lower to the current client version");
		}
	}

	private void OnPlayerKicked(NetworkEvent e)
	{
		KickReason reason = e.getExtensible<KickEvent>().Reason;
		UnityEngine.Debug.Log("Player kicked with reason: " + reason);
		EnterDarkPocket();
		current.BlockInputUntilNetworkEstablished();
		SystemMessage systemMessage = SystemMessage.ShowAlert(("KICK_" + reason).ToLower());
		systemMessage.AddButton("ok", OnKickedButtonClick);
		systemMessage.SetBlockPriority(UIBlocker.Priority.ServerSystemAlert);
		systemMessage.Show();
		ScreenTexture.Instance.AddOverlay(systemMessage.gameObject);
	}

	private void OnKickedButtonClick()
	{
		ExitDarkPocket();
		BlockInputUntilNetworkEstablished();
	}

	internal void HandleUnrecoverableError()
	{
		UnityEngine.Debug.LogError("Unrecoverable Error occured - wiping all data and require a connect");
		UserManager.Instance.ClearUser();
		GameRestarter.ShowRestartDialog("unrecoverable_error", "restart_game", true);
	}

	private void StartConnectionProcess()
	{
		if (InsideDarkPocket)
		{
			this.OnNetworkCutoff();
		}
		networkChain.StartChain();
	}

	public void UpdateNetwork()
	{
		if (_instance != null && _instance._protocol != null)
		{
			_instance.Update();
		}
	}

	public void setSettings(NetworkSettings _settings)
	{
		settings = _settings;
		networkChain.Config = _settings.config;
	}

	public void RestartConnection(string reason, bool withTimeout = true)
	{
		UnityEngine.Debug.LogWarning("Restarting Connection. Reason: " + reason);
		StopCurrentState(withTimeout);
	}

	private void StopCurrentState(bool withTimeout = true)
	{
		UnityEngine.Debug.Log("StopCurrentState");
		networkChain.StopCurrentState(withTimeout);
		reset();
	}

	public void ClearOfflineQueue()
	{
		offlineQueue.Clear();
	}

	private void reset()
	{
		isPlayerInited = false;
		joined_zone = false;
		pingManager.stopPing();
		cancelAllRequests();
		offlineQueue.Stop();
	}

	protected override void Disconnect()
	{
		reset();
		base.Disconnect();
	}

	public bool ParsePlayer(NetworkEvent e)
	{
		ExtendedPlayer extensible = e.getExtensible<ExtendedPlayer>();
		Analytics.current.SetClientLoggingID(extensible.PrimaryPlayer.LogData.ClientLogId);
		if (!offlineQueue.SetIDFromServer(extensible.PrimaryPlayer.OfflineStateId))
		{
			HandleUnrecoverableError();
			return false;
		}
		this.OnPlayerUpdate(extensible);
		isPlayerInited = true;
		ReopenCurrentModule();
		if (extensible.PrimaryPlayer.BrawlerFight != null)
		{
			BrawlerHelper.ResendBrawlerFinish(extensible.PrimaryPlayer.BrawlerFight);
		}
		return true;
	}

	private void ReopenCurrentModule()
	{
		ConstantsSF3.ELocationSceneModule currentType = BaseModuleController.CurrentType;
		if (currentType == ConstantsSF3.ELocationSceneModule.Fight || currentType == ConstantsSF3.ELocationSceneModule.Preloader || currentType == ConstantsSF3.ELocationSceneModule.DojoInterface || currentType == ConstantsSF3.ELocationSceneModule.None)
		{
			return;
		}
		UIBlocker.Instance.Block(UIBlocker.Priority.Preloader);
		LoadingIcon.Instance.EnableLoadingScreen();
		ModuleController.GoToDojo(delegate
		{
			BaseModuleController.GoToModule(currentType, delegate
			{
				UIBlocker.Instance.Unblock(UIBlocker.Priority.Preloader);
				LoadingIcon.Instance.DisableLoadingScreen();
			});
		});
	}

	private void OnNetworkEstablishedCallback()
	{
		offlineQueue.Start();
	}

	public bool isReconnecting()
	{
		return !IsNetworkEstablished() && networkChain.GetCurrentStateType() == NetworkState.Timeout;
	}

	public bool canCreatePlayer()
	{
		return UserManager.IsStartingTutorialCompleted;
	}

	public static void SendOffline(string cmd, IMessage e)
	{
		if (UserManager.IsStartingTutorialCompleted)
		{
			current.offlineQueue.AddRequest(cmd, e);
		}
	}

	protected override int send(string cmd, IMessage e, float timeout, Action<NetworkEvent> callback = null, object data = null)
	{
		if (current.isPlayerInited || cmdToAllowBeforeInit.Contains(cmd))
		{
			return base.send(cmd, e, timeout, callback, data);
		}
		UnityEngine.Debug.LogError("Cannot send before player was inited or we are not connected.\nWait for the end of tutorial and get an internet connection\nCMD=" + cmd);
		if (callback != null)
		{
			callback(NetworkEvent.createErrorEvent(cmd, data, "cannot call right now"));
		}
		return -1;
	}

	public static void Send(string cmd, IMessage e, Action<NetworkEvent> callback = null, object data = null, float? timeout = null, bool next = false, bool useTimeoutInQueue = true)
	{
		if (current.queues == null || current.queues.IsEmpty())
		{
			UnityEngine.Debug.LogError("Trying to Send before Init() was called");
		}
		else
		{
			current.genericQueue.AddRequest(cmd, e, callback, data, GetTimeout(cmd, timeout), next, useTimeoutInQueue);
		}
	}

	public static void SendEvent(string cmd, IMessage e, Action<NetworkEvent> callback = null, object data = null, float? timeout = null, bool next = false, bool useTimeoutInQueue = true)
	{
		if (current.queues == null || current.queues.IsEmpty())
		{
			UnityEngine.Debug.LogError("Trying to Send before Init() was called");
		}
		else
		{
			current.eventQueue.AddRequest(cmd, e, callback, data, GetTimeout(cmd, timeout), next, useTimeoutInQueue);
		}
	}

	public static void WithoutQueueSend(string cmd, IMessage e, Action<NetworkEvent> callback = null, object data = null, float? timeout = null)
	{
		current.send(cmd, e, GetTimeout(cmd, timeout), callback, data);
	}

	private static float GetTimeout(string cmd, float? timeout)
	{
		return (!timeout.HasValue) ? current.Timeout(cmd) : timeout.Value;
	}

	public override void RemoveCallbacks(Action<NetworkEvent> callback)
	{
		foreach (GenericRequestQueue queue in queues)
		{
			queue.RemoveCallbacks(callback);
		}
		base.RemoveCallbacks(callback);
	}

	public override void RemoveCallbacks(object target)
	{
		foreach (GenericRequestQueue queue in queues)
		{
			queue.RemoveCallbacks(target);
		}
		base.RemoveCallbacks(target);
	}

	public override void cancelAllRequests()
	{
		foreach (GenericRequestQueue queue in queues)
		{
			queue.CancelAllRequests();
		}
		base.cancelAllRequests();
	}

	internal GetPlayerRequest CreateGetPlayerRequest()
	{
		GetPlayerRequest getPlayerRequest = new GetPlayerRequest();
		getPlayerRequest.Version = CurrentConfigVersion.full;
		getPlayerRequest.OfflineRequestBatch = offlineQueue.GetOfflineBatch();
		return getPlayerRequest;
	}

	private void OnGetPlayerStarted(CallEventArgsBase<System.Type> obj)
	{
		GetPlayerNetworkState getPlayerNetworkState = (GetPlayerNetworkState)obj.Content;
		getPlayerNetworkState.OnGotPlayer += OnGetPlayer;
	}

	private void OnGetPlayer(NetworkEvent e)
	{
		if (e.success)
		{
			long offlineStateId = e.getExtensible<ExtendedPlayer>().PrimaryPlayer.OfflineStateId;
			offlineQueue.RemoveAcceptedRequests(offlineStateId);
		}
	}

	public void OnConfigVersionUpdate(List<string> validVersions, Action OnCheckFinished = null)
	{
		BlockInputUntilNetworkEstablished();
		if (offlineQueue.RemoveInvalidRequests(validVersions))
		{
			SystemMessage systemMessage = SystemMessage.ShowAlert("server_connection_data_loss_possible");
			systemMessage.AddButton("continue", delegate
			{
				OnCheckFinished.InvokeSafe();
			});
			systemMessage.SetBlockPriority(UIBlocker.Priority.ServerSystemAlert);
			systemMessage.Show();
		}
		else
		{
			OnCheckFinished.InvokeSafe();
		}
	}

	public void BlockInputUntilNetworkEstablished()
	{
		inputBlocker.BlockInputUntilNetworkEstablished();
	}

	internal void DestoySelf()
	{
		StopCurrentState();
		NetworkEventManager.Reset();
		_instance = null;
	}

	protected override byte[] PreprocessExtensionResponse(string cmd, byte[] bytes)
	{
		ResponseAndStateId responseAndStateId = bytes.FromBinary<ResponseAndStateId>();
		if (cmd != "process_offline_batch")
		{
			offlineQueue.SetIDFromServer(responseAndStateId.OfflineStateId);
		}
		offlineQueue.RemoveAcceptedRequests(responseAndStateId.OfflineStateId);
		return responseAndStateId.Response.ToByteArray();
	}

	private void OnGenericRequestFinished(NetworkEvent e)
	{
		if (e.HandleError(common.RequestErrorCode.Error, ClientRequestError.ClientTimeout) && e.name != "process_offline_batch" && IsNetworkEstablished())
		{
			RestartConnection("Server Error Or Client Timeout");
			BlockInputUntilNetworkEstablished();
		}
	}

	protected override void HandleCommonErrors(NetworkEvent e)
	{
		if (e.HandleError(sf3DTO.RequestErrorCode.PlayerNotFound))
		{
			RestartConnection("PlayerNotFound, try again");
			BlockInputUntilNetworkEstablished();
		}
		if (e.HandleError(sf3DTO.RequestErrorCode.InvalidConfigVersion))
		{
			RestartConnection("Need to download new configs");
		}
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private void _003CRemoveCallbacks_003E__BaseCallProxy0(Action<NetworkEvent> callback)
	{
		base.RemoveCallbacks(callback);
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private void _003CRemoveCallbacks_003E__BaseCallProxy1(object target)
	{
		base.RemoveCallbacks(target);
	}
}
