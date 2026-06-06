using System;
using System.Collections.Generic;
using Google.Protobuf;
using Network.core;
using Network.core.events;
using UnityEngine;

public class GenericRequestQueue
{
	private class GenericRequest : RequestObject
	{
		public IMessage e;

		public float timeout;
	}

	public Action<NetworkEvent> OnRequestFinished;

	private readonly SendDelegate sendDelegate;

	private GenericRequest currentRequest;

	private readonly List<GenericRequest> queue = new List<GenericRequest>();

	private readonly Action<Action<NetworkEvent>> removeCallbacksByAction;

	private readonly Action<object> removeCallbacksByTarget;

	private PingManager pingManager;

	private bool lastPingSuccessful = true;

	public GenericRequestQueue(SendDelegate _sendDelegate, Action<Action<NetworkEvent>> removeCallbacksByAction, Action<object> removeCallbacksByTarget, PingManager pingManager)
	{
		sendDelegate = _sendDelegate;
		this.removeCallbacksByAction = removeCallbacksByAction;
		this.removeCallbacksByTarget = removeCallbacksByTarget;
		this.pingManager = pingManager;
		pingManager.OnPingFinished += OnPingFinished;
		NetworkConnection.current.OnNetworkCutoff += delegate
		{
			lastPingSuccessful = true;
		};
	}

	private void OnPingFinished(bool success)
	{
		lastPingSuccessful = success;
		if (success)
		{
			if (!isRequestInProgress())
			{
				SendNextRequestFromQueue();
			}
		}
		else if (queue.Count > 0 && !isRequestInProgress())
		{
			NetworkConnection.current.RestartConnection("testing ping failed - disconect to cancel all requests, so that will not fail with timeout and block the game until connection");
		}
	}

	public bool isRequestInProgress()
	{
		return currentRequest != null;
	}

	public void AddRequest(string cmd, IMessage e, Action<NetworkEvent> callback, object data, float timeout, bool next = false, bool useTimeoutInQueue = true)
	{
		GenericRequest r = new GenericRequest
		{
			cmd = cmd,
			e = e,
			callback = callback,
			data = data,
			timeout = timeout
		};
		if (useTimeoutInQueue)
		{
			r.timeoutCoroutine = Routiner.GoDelayed(delegate
			{
				CancelRequestWithTimeout(r);
			}, r.timeout);
		}
		if (next)
		{
			queue.Insert(0, r);
		}
		else
		{
			queue.Add(r);
		}
		if (!isRequestInProgress())
		{
			SendNextRequestFromQueue();
		}
	}

	private void RemoveRequest(int id)
	{
		queue[id].StopTimeoutCoroutine();
		queue.RemoveAt(id);
	}

	public void RemoveCallbacks(Action<NetworkEvent> action)
	{
		List<Delegate> list = new List<Delegate>(action.GetInvocationList());
		int[] counter = new int[list.Count];
		for (int num = queue.Count - 1; num >= 0; num--)
		{
			if (queue[num].DeleteDelegates(list, counter))
			{
				RemoveRequest(num);
			}
		}
		if (currentRequest != null && currentRequest.DeleteDelegates(action))
		{
			removeCallbacksByAction(OnRequestFinishedCallback);
			currentRequest = null;
			SendNextRequestFromQueue();
		}
	}

	public void RemoveCallbacks(object target)
	{
		for (int num = queue.Count - 1; num >= 0; num--)
		{
			if (queue[num].DeleteDelegates(target))
			{
				RemoveRequest(num);
			}
		}
		if (currentRequest != null && currentRequest.DeleteDelegates(target))
		{
			removeCallbacksByTarget(this);
			currentRequest = null;
			SendNextRequestFromQueue();
		}
	}

	private void CancelRequestWithTimeout(GenericRequest request)
	{
		if (queue.Remove(request))
		{
			request.callback(NetworkEvent.createCancelEvent(request.cmd, request.data, "canceled inside GenericQueue"));
		}
		else
		{
			Debug.LogError("CancelRequestWithTimeout called with not found. Request cmd:" + request.cmd);
		}
	}

	internal void CancelAllRequests()
	{
		while (queue.Count > 0)
		{
			GenericRequest genericRequest = queue[0];
			RemoveRequest(0);
			genericRequest.callback(NetworkEvent.createCancelEvent(genericRequest.cmd, genericRequest.data));
		}
	}

	private void SendNextRequestFromQueue()
	{
		if (queue.Count == 0)
		{
			return;
		}
		if (isRequestInProgress())
		{
			Debug.LogError("Error: checking to send from queue while we have a current request");
		}
		else if (!pingManager.PingInProcess)
		{
			if (!lastPingSuccessful)
			{
				pingManager.ForceSendPing();
				return;
			}
			GenericRequest r = queue[0];
			RemoveRequest(0);
			sendRequest(r);
		}
	}

	private void sendRequest(GenericRequest r)
	{
		if (isRequestInProgress())
		{
			Debug.LogError("Error: can't send multiple requests at the same time");
			return;
		}
		currentRequest = r;
		sendDelegate(r.cmd, r.e, r.timeout, (Action<NetworkEvent>)Delegate.Combine(new Action<NetworkEvent>(OnRequestFinishedCallback), r.callback), r.data);
	}

	private void OnRequestFinishedCallback(NetworkEvent e)
	{
		currentRequest = null;
		OnRequestFinished.InvokeSafe(e);
		SendNextRequestFromQueue();
	}
}
