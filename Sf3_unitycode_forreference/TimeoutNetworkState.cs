using System;
using UnityEngine;

public class TimeoutNetworkState : NetworkStateBase
{
	private Coroutine _timeoutRoutine;

	private float _reconnectTimeout;

	private bool _warningShown;

	public override void Start(object data)
	{
		if (data == null)
		{
			Debug.LogError("Error: TimeoutNetworkState should only be created from NetworkChain where it passes the timeout in");
			throw new ArgumentException();
		}
		_reconnectTimeout = (float)data;
		_timeoutRoutine = Routiner.GoDelayed(OnTimeoutFinished, _reconnectTimeout);
	}

	public override void Cleanup()
	{
	}

	public override void Stop()
	{
		if (_timeoutRoutine != null)
		{
			Routiner.Stop(_timeoutRoutine);
		}
	}

	private void OnTimeoutFinished()
	{
		if (NetworkConnection.current.InsideDarkPocket)
		{
			Start(_reconnectTimeout);
			OneTimeWarning("Restarting timeout inside dark pocket");
		}
		else
		{
			OnSuccess(null, null);
		}
	}

	private void OneTimeWarning(string text)
	{
		if (!_warningShown)
		{
			Debug.LogWarning(text);
			_warningShown = true;
		}
	}

	public override string ToString()
	{
		return string.Format("{0} ({1} s.)", base.ToString(), _reconnectTimeout);
	}
}
