using System;
using System.Collections;
using Network.core.events;
using UnityEngine;
using common;

public class PingManager
{
	private const string LAST_SERVER_TIME_KEY = "lastServerTime";

	private const string LAST_UPDATE_TIME_KEY = "lastUpdateTime";

	private long lastServerTime;

	private long lastUpdateTime;

	private bool running;

	private int failedPings;

	private Coroutine delayPingCor;

	private int lastPing = -1;

	public bool PingInProcess { get; private set; }

	private long LastServerTime
	{
		get
		{
			return (lastServerTime <= 0) ? GetLong("lastServerTime") : lastServerTime;
		}
		set
		{
			lastServerTime = value;
			PlayerPrefs.SetString("lastServerTime", lastServerTime.ToString());
			LastUpdateTime = DateTime.Now.GetUnixTimeStampMilliseconds();
		}
	}

	private long LastUpdateTime
	{
		get
		{
			return (!((float)lastUpdateTime > 0f)) ? GetLong("lastUpdateTime") : lastUpdateTime;
		}
		set
		{
			lastUpdateTime = value;
			PlayerPrefs.SetString("lastUpdateTime", lastUpdateTime.ToString());
			PlayerPrefs.Save();
		}
	}

	public event Action<bool> OnPingFinished;

	public PingManager()
	{
		addListeners();
	}

	private static long GetLong(string key)
	{
		return long.Parse(PlayerPrefs.GetString(key, "-1"));
	}

	public void startPing()
	{
		stopPing();
		running = true;
		Routiner.Go(onPingDelayElapsed());
	}

	private IEnumerator onPingDelayElapsed()
	{
		SendPing();
		yield return 0;
	}

	private void SendPing()
	{
		delayPingCor = null;
		PingInProcess = true;
		NetworkConnection.WithoutQueueSend("ping", new Timestamp
		{
			Value = getCurrentLocalTime()
		}, onPingResponse);
	}

	public void ForceSendPing()
	{
		Debug.LogWarning("Forcing a ping");
		Routiner.Stop(delayPingCor);
		SendPing();
	}

	private void onPingResponse(NetworkEvent e)
	{
		e.HandleError(ClientRequestError.ClientTimeout);
		PingInProcess = false;
		this.OnPingFinished.InvokeSafe(e.success);
		if (running)
		{
			if (e.success)
			{
				PingResponse extensible = e.getExtensible<PingResponse>();
				LastServerTime = extensible.ServerTimestamp.Value;
				lastPing = (int)(getCurrentLocalTime() - extensible.ClientTimestamp.Value);
				resetFailedPings();
			}
			else
			{
				incrementFailedPings();
			}
			delayPingCor = Routiner.GoDelayed(onPingDelayElapsed(), NetworkConnection.Settings.Ping.Delay.ToSeconds());
		}
	}

	public void stopPing()
	{
		resetFailedPings();
		running = false;
		lastPing = -1;
		PingInProcess = false;
		NetworkConnection.current.RemoveCallbacks(onPingResponse);
		Routiner.Stop(delayPingCor);
		delayPingCor = null;
	}

	public Timestamp getCurrentServerTime()
	{
		Timestamp timestamp;
		if (LastServerTime <= 0 || LastUpdateTime <= 0)
		{
			Debug.LogError("Client time used instead of server time.");
			timestamp = new Timestamp();
			timestamp.Value = getCurrentLocalTime();
			return timestamp;
		}
		long num = DateTime.Now.GetUnixTimeStampMilliseconds() - LastUpdateTime;
		timestamp = new Timestamp();
		timestamp.Value = LastServerTime + num;
		return timestamp;
	}

	private static long getCurrentLocalTime()
	{
		return TimeManager.GetLocalMS();
	}

	private void resetFailedPings()
	{
		failedPings = 0;
	}

	private void incrementFailedPings()
	{
		failedPings++;
		if (failedPings >= NetworkConnection.Settings.Ping.FailAttempts)
		{
			NetworkConnection.current.RestartConnection("Multiple Pings failed");
			resetFailedPings();
		}
	}

	private void addListeners()
	{
		removeEventListeners();
		StaticObjectsManager.OnApplicationResumeEvent += OnApplicationResume;
	}

	private void OnApplicationResume()
	{
		if (running)
		{
			startPing();
		}
	}

	private void removeEventListeners()
	{
		StaticObjectsManager.OnApplicationResumeEvent -= OnApplicationResume;
	}

	public int getPing()
	{
		return lastPing;
	}
}
