using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Protobuf;
using Network.core.events;
using Org.BouncyCastle.Utilities.Encoders;
using SF3.UserData;
using UnityEngine;
using clientDTO;
using sf3DTO;

public class OfflineRequestQueue
{
	public class QueueItem
	{
		public OfflineRequestData data;

		public byte[] state;
	}

	public class QueueData
	{
		public List<QueueItem> queue = new List<QueueItem>();

		private const string STATE_ID_TAG = "STATE_ID";

		private long _currentID;

		public long currentID
		{
			get
			{
				return _currentID;
			}
			private set
			{
				_currentID = value;
				SaveStateID();
			}
		}

		public QueueData(long stateID)
		{
			currentID = stateID;
		}

		public void SaveStateID()
		{
			PlayerPrefs.SetString("STATE_ID", GetCurrentStateID().ToString());
			PlayerPrefs.Save();
		}

		public static long LoadStateID()
		{
			if (PlayerPrefs.HasKey("STATE_ID"))
			{
				return long.Parse(PlayerPrefs.GetString("STATE_ID"));
			}
			return 0L;
		}

		public long GetCurrentStateID()
		{
			return currentID;
		}

		public long GetNextStateID()
		{
			if (currentID == long.MaxValue)
			{
				Debug.LogError("Too many requests were created - int64 wasn't enough...");
			}
			return ++currentID;
		}

		public bool SetIDFromServer(long id)
		{
			if (currentID > id)
			{
				Debug.LogError("client ID went farther than the server id got from create or get player");
				currentID = id;
				return false;
			}
			currentID = id;
			return true;
		}

		public void Add(QueueItem o)
		{
			queue.Add(o);
		}

		public void RemoveAcceptedRequests(long lastAcceptedID)
		{
			if (queue.Count == 0)
			{
				return;
			}
			int num = -1;
			for (int i = 0; i < queue.Count; i++)
			{
				QueueItem queueItem = queue[i];
				if (queueItem.data.RequestID <= lastAcceptedID)
				{
					num = i;
					continue;
				}
				break;
			}
			if (num != -1)
			{
				queue.RemoveRange(0, num + 1);
			}
		}

		public bool RemoveInvalidRequests(List<string> validVersions)
		{
			for (int i = 0; i < queue.Count; i++)
			{
				if (!validVersions.Contains(queue[i].data.VersionFull))
				{
					queue.RemoveRange(i, queue.Count - i);
					return true;
				}
			}
			return false;
		}
	}

	private QueueData queueData;

	private List<QueueItem> queue
	{
		get
		{
			return queueData.queue;
		}
	}

	private static string folderPath
	{
		get
		{
			return GlobalPath.GameDataCombine("User/Offline/");
		}
	}

	private static string queueFilePath
	{
		get
		{
			return folderPath + "offline_queue.dat";
		}
	}

	private static string lastStateFilePath
	{
		get
		{
			return folderPath + "offline_state.dat";
		}
	}

	public OfflineRequestQueue()
	{
		FilesUtil.CreateDirectory(folderPath);
		Load();
	}

	public void Start()
	{
		SendFromQueue();
	}

	private void SendFromQueue()
	{
		if (queue.Count > 0)
		{
			SendRequest(GetOfflineBatch());
		}
	}

	public void Stop()
	{
		NetworkConnection.current.RemoveCallbacks(this);
	}

	public long GetCurrentStateID()
	{
		return queueData.GetCurrentStateID();
	}

	internal void RemoveAcceptedRequests(long lastAcceptedID)
	{
		queueData.RemoveAcceptedRequests(lastAcceptedID);
		SaveAll();
	}

	public bool SetIDFromServer(long id)
	{
		Debug.Log("Switched offline_state_id to: " + id);
		bool flag = queueData.SetIDFromServer(id);
		if (!flag)
		{
			Debug.LogError("Client added new requests while we did syncing with the server - need fix ASAP");
		}
		queueData.SaveStateID();
		return flag;
	}

	public OfflineRequestBatch GetOfflineBatch()
	{
		OfflineRequestBatch offlineRequestBatch = new OfflineRequestBatch();
		List<OfflineRequest> list = new List<OfflineRequest>();
		list.Resize(queue.Count, null);
		for (int i = 0; i < queue.Count; i++)
		{
			list[i] = ObjectToRequest(queue[i]);
		}
		offlineRequestBatch.Requests.AddRange(list);
		AddStateIfNeeded(offlineRequestBatch, LoadLastUserState());
		return offlineRequestBatch;
	}

	public static float GetTimeoutForBatchSize(int batchSize)
	{
		if (batchSize <= 0)
		{
			return NetworkConnection.Settings.DefaultRequestTimeout.ToSeconds();
		}
		float defaultRequestTimeout = NetworkConnection.Settings.DefaultRequestTimeout;
		int num = 1 + batchSize / (int)NetworkConnection.Settings.OfflineRequestTimeout.PerRequests;
		return (defaultRequestTimeout + (float)(NetworkConnection.Settings.OfflineRequestTimeout.ExtraTimeout * num)).ToSeconds();
	}

	private OfflineRequest ObjectToRequest(QueueItem o)
	{
		OfflineRequest offlineRequest = new OfflineRequest();
		offlineRequest.NewStateId = o.data.RequestID;
		offlineRequest.Cmd = o.data.Cmd;
		offlineRequest.ConfigVersion = o.data.VersionFull;
		offlineRequest.Data = o.data.Binary;
		return offlineRequest;
	}

	public void AddRequest(string cmd, IMessage e)
	{
		QueueItem queueItem = new QueueItem();
		queueItem.data = new OfflineRequestData();
		QueueItem queueItem2 = queueItem;
		queueItem2.data.RequestID = queueData.GetNextStateID();
		queueItem2.data.Cmd = cmd;
		queueItem2.data.VersionFull = NetworkConnection.current.CurrentConfigVersion.full;
		queueItem2.data.Binary = e.ToByteString();
		queueItem2.state = UserDataController.GetClientState(queueItem2.data.RequestID).ToBinary();
		AddToQueue(queueItem2, queueItem2.state);
	}

	private void AddToQueue(QueueItem request, byte[] userState)
	{
		queueData.Add(request);
		AppendRequestToFile(request.data);
		SaveLastUserState(userState);
		queueData.SaveStateID();
		SendRequest(request);
	}

	private void SendRequest(QueueItem obj)
	{
		if (NetworkConnection.current.IsNetworkEstablished())
		{
			OfflineRequestBatch offlineRequestBatch = new OfflineRequestBatch();
			offlineRequestBatch.Requests.Add(ObjectToRequest(obj));
			AddStateIfNeeded(offlineRequestBatch, obj.state);
			SendRequest(offlineRequestBatch);
		}
	}

	private static void AddStateIfNeeded(OfflineRequestBatch batch, byte[] state)
	{
		if (JS.Instance.GetBoolConstant("compareOfflineStateAfterCommandExecution") && state != null)
		{
			batch.ClientState = state.FromBinary<MutableOfflineState>();
		}
	}

	private void SendRequest(OfflineRequestBatch batch, bool next = false)
	{
		NetworkConnection.Send("process_offline_batch", batch, OnOfflineResponse, batch, GetTimeoutForBatchSize(batch.Requests.Count), next, false);
	}

	private void OnOfflineResponse(NetworkEvent e)
	{
		if (e.HandleError(RequestErrorCode.UnrecoverableOfflineRequestError))
		{
			NetworkConnection.current.HandleUnrecoverableError();
		}
		else if (e.HandleError(RequestErrorCode.RecoverableOfflineRequestError, ClientRequestError.ClientTimeout))
		{
			SendRequest((OfflineRequestBatch)e.data, true);
		}
		else if (!e.HandleError(ClientRequestError.ClientCanceled, ClientRequestError.ClientError))
		{
			if (e.HandleError(RequestErrorCode.InvalidConfigVersion))
			{
				NetworkConnection.current.RestartConnection(e.ErrorTypeAsString(), false);
			}
			else if (!e.WasErrorHandled)
			{
				Debug.LogError("Unknown offline request error: Undefined behaviour");
				NetworkConnection.current.HandleUnrecoverableError();
			}
		}
	}

	internal bool RemoveInvalidRequests(List<string> validVersions)
	{
		bool flag = queueData.RemoveInvalidRequests(validVersions);
		if (flag)
		{
			SaveAll();
		}
		return flag;
	}

	private void SaveLastUserState(byte[] stateBytes)
	{
		FilesUtil.WriteFileBytes(lastStateFilePath, stateBytes);
	}

	private byte[] LoadLastUserState()
	{
		return FilesUtil.ReadFileBytes(lastStateFilePath);
	}

	private void SaveAll()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (QueueItem item in queue)
		{
			stringBuilder.Append(RequestToString(item.data)).Append("\n");
		}
		FilesUtil.WriteFileText(queueFilePath, stringBuilder.ToString());
		queueData.SaveStateID();
	}

	private void AppendRequestToFile(OfflineRequestData request)
	{
		FilesUtil.AppendFileText(queueFilePath, RequestToString(request) + "\n");
	}

	private string RequestToString(OfflineRequestData r)
	{
		return Base64.ToBase64String(r.ToBinary());
	}

	private OfflineRequestData RequestFromString(string requestString)
	{
		return Base64.Decode(requestString).FromBinary<OfflineRequestData>();
	}

	private void Load()
	{
		queueData = new QueueData(QueueData.LoadStateID());
		if (FilesUtil.IsFileExists(queueFilePath))
		{
			string[] array = FilesUtil.ReadFileLines(queueFilePath);
			queueData.queue.Resize(array.Count(), null);
			for (int i = 0; i < array.Count(); i++)
			{
				queueData.queue[i] = new QueueItem
				{
					data = RequestFromString(array[i])
				};
			}
		}
	}

	public void Clear()
	{
		FilesUtil.DeleteDirectory(folderPath);
		FilesUtil.CreateDirectory(folderPath);
		queueData = new QueueData(0L);
		SaveAll();
	}
}
