using System;
using UnityEngine;

public class DisconnectOnExit : MonoBehaviour
{
	private NetworkConnection _connection;

	public static void Init(NetworkConnection connection)
	{
		DisconnectOnExit disconnectOnExit = new GameObject("_networkDisconnector").AddComponent<DisconnectOnExit>();
		StaticObjectsManager.AddObject(disconnectOnExit.gameObject);
		disconnectOnExit._connection = connection;
	}

	private void OnDestroy()
	{
		try
		{
			if (NetworkConnection.HasInstance())
			{
				_connection.EnterDarkPocket();
				_connection.RestartConnection("Game Closing");
			}
		}
		catch (Exception)
		{
		}
	}
}
