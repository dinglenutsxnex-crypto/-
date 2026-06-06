using System.Collections.Generic;
using Network.core.events;
using UnityEngine;

namespace Network.core
{
	public class RequestHistory
	{
		private readonly Dictionary<int, RequestObject> requestHistory = new Dictionary<int, RequestObject>();

		public void Clear()
		{
			requestHistory.Clear();
		}

		public RequestObject GetRequest(int id)
		{
			RequestObject value = null;
			requestHistory.TryGetValue(id, out value);
			return value;
		}

		public bool HasRequest(int id)
		{
			return GetRequest(id) != null;
		}

		public void AddRequest(int id, RequestObject obj)
		{
			if (!requestHistory.ContainsKey(id))
			{
				requestHistory.Add(id, obj);
			}
			else
			{
				Debug.LogError("Error: AddObjectToHistory repeated id");
			}
		}

		public void RemoveRequest(int id, bool stopTimeout = true)
		{
			RequestObject value;
			requestHistory.TryGetValue(id, out value);
			if (value != null)
			{
				if (stopTimeout)
				{
					value.StopTimeoutCoroutine();
				}
				requestHistory.Remove(id);
			}
		}

		public void RemoveCallbacks<T>(T source)
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, RequestObject> item in requestHistory)
			{
				if (item.Value.DeleteDelegates(source))
				{
					list.Add(item.Key);
				}
			}
			foreach (int item2 in list)
			{
				RemoveRequest(item2);
			}
		}

		public virtual void CancelAllRequests()
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, RequestObject> item in requestHistory)
			{
				list.Add(item.Key);
				RequestObject value = item.Value;
				value.Invoke(NetworkEvent.createCancelEvent(value.cmd, (value == null) ? null : value.data));
			}
			foreach (int item2 in list)
			{
				RemoveRequest(item2);
			}
		}
	}
}
