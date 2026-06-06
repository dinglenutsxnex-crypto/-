using System;
using System.Collections.Generic;
using Network.core.events;
using UnityEngine;

namespace Network.core
{
	public class RequestObject
	{
		public string cmd;

		public Action<NetworkEvent> callback;

		public object data;

		public Coroutine timeoutCoroutine;

		public void Invoke(NetworkEvent e)
		{
			callback.InvokeSafe(e);
		}

		public bool DeleteDelegates(Action<NetworkEvent> action)
		{
			return DeleteDelegates(new List<Delegate>(action.GetInvocationList()));
		}

		public bool DeleteDelegates(List<Delegate> callbacksToDelete, int[] counter)
		{
			Delegate[] invocationList = callback.GetInvocationList();
			foreach (Delegate @delegate in invocationList)
			{
				int num = callbacksToDelete.IndexOf(@delegate);
				if (num >= 0)
				{
					callback = (Action<NetworkEvent>)Delegate.Remove(callback, (Action<NetworkEvent>)@delegate);
					counter[num]++;
					break;
				}
			}
			return callback.IsNullOrEmpty();
		}

		public bool DeleteDelegates(object target)
		{
			Delegate[] invocationList = callback.GetInvocationList();
			foreach (Delegate @delegate in invocationList)
			{
				if (@delegate.Target == target)
				{
					callback = (Action<NetworkEvent>)Delegate.Remove(callback, (Action<NetworkEvent>)@delegate);
				}
			}
			return callback.IsNullOrEmpty();
		}

		public void StopTimeoutCoroutine()
		{
			if (timeoutCoroutine != null)
			{
				Routiner.Stop(timeoutCoroutine);
				timeoutCoroutine = null;
			}
		}
	}
}
