using System;
using System.Collections.Generic;
using Google.Protobuf;

namespace Network.core.events
{
	public class NetworkEventListner
	{
		private readonly Dictionary<string, List<Action<NetworkEvent>>> _subscription;

		public NetworkEventListner()
		{
			_subscription = new Dictionary<string, List<Action<NetworkEvent>>>();
		}

		public void callEvent(string name, IMessage extensible = null, int code = 0, string message = "", int requestID = 0, object data = null)
		{
			callEvent(new NetworkEvent(name, extensible, code, message, requestID, data));
		}

		public void callEvent(NetworkEvent e)
		{
			if (!_subscription.ContainsKey(e.name))
			{
				return;
			}
			List<Action<NetworkEvent>> list = new List<Action<NetworkEvent>>(_subscription[e.name]);
			foreach (Action<NetworkEvent> item in list)
			{
				item(e);
			}
		}

		public void addEventListener(string name, Action<NetworkEvent> callback)
		{
			if (_subscription.ContainsKey(name))
			{
				_subscription[name].Add(callback);
				return;
			}
			List<Action<NetworkEvent>> list = new List<Action<NetworkEvent>>();
			list.Add(callback);
			_subscription[name] = list;
		}

		public void removeEventListener(string name, Action<NetworkEvent> callback)
		{
			if (_subscription.ContainsKey(name))
			{
				_subscription[name].Remove(callback);
			}
		}

		public void removeAllEventListener()
		{
			_subscription.Clear();
		}
	}
}
