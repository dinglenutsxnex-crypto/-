using System;
using System.Collections.Generic;
using Nekki.Utils;

public class TimeDispatcher
{
	private class DelegateObject
	{
		public Action<object> action;

		public object data;
	}

	private List<KeyValuePair<long, DelegateObject>> delegates = new List<KeyValuePair<long, DelegateObject>>();

	private PingManager _pingManager;

	public void Start(PingManager pingManager)
	{
		_pingManager = pingManager;
		GlobalTimer.Instnce.addEventListener(1, onTick);
	}

	public void callDelegateAt(long timestamp, Action<object> _delegate, object _data = null)
	{
		DelegateObject delegateObject = new DelegateObject();
		delegateObject.action = _delegate;
		delegateObject.data = _data;
		DelegateObject value = delegateObject;
		delegates.Add(new KeyValuePair<long, DelegateObject>(timestamp, value));
	}

	public void removeDelegate(Action<object> _delegate)
	{
		for (int i = 0; i < delegates.Count; i++)
		{
			if (_delegate == delegates[i].Value.action)
			{
				delegates.RemoveAt(i);
				break;
			}
		}
	}

	private void onTick(CallEventArgs callEventArgs)
	{
		for (int num = delegates.Count - 1; num >= 0; num--)
		{
			long key = delegates[num].Key;
			Action<object> action = delegates[num].Value.action;
			if (_pingManager.getCurrentServerTime().Value >= key)
			{
				action(delegates[num].Value.data);
				delegates.RemoveAt(num);
			}
		}
	}
}
