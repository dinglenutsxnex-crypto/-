using System;
using Google.Protobuf;
using Nekki;
using Network.core;
using Network.core.data;
using Network.core.events;
using UnityEngine;
using common;

public abstract class NetworkConnectionBase : SFSConnection
{
	protected bool joined_zone;

	protected PingManager pingManager = new PingManager();

	public NetworkConnectionBase()
	{
	}

	public void Init(SFSProtocol protocol, SFSGameData data, string zoneName, int version, Func<string, float> timeOut)
	{
		SFCMD.Init();
		_protocol = protocol;
		_data = data;
		base.Zone = zoneName;
		base.Version = version;
		Timeout = timeOut;
	}

	public void Connect(string serverIP, int port)
	{
		base.Server = serverIP;
		base.TCPPort = port;
		Debug.Log(string.Format("Start Connecting IP:{0} Port:{1}", base.Server, base.TCPPort));
		base.Connect();
	}

	public int GetPing()
	{
		return pingManager.getPing();
	}

	protected override int send(string cmd, IMessage e, float timeout, Action<NetworkEvent> callback = null, object data = null)
	{
		if (joined_zone)
		{
			return base.send(cmd, e, timeout, callback, data);
		}
		Debug.LogError("Cannot send request before join_zone cmd=" + cmd);
		if (callback != null)
		{
			callback(NetworkEvent.createErrorEvent(cmd, data, "cannot call before join_zone"));
		}
		return -1;
	}

	public Timestamp getCurrentServerTime()
	{
		return pingManager.getCurrentServerTime();
	}

	public DateTime getCurrentServerDateTime()
	{
		return NekkiUtils.GetUnixDateTimeFromMilliseconds(getCurrentServerTime().Value);
	}
}
