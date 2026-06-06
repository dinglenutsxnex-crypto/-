using SF3.UserData;
using UnityEngine;

public class NetworkInputBlocker
{
	private readonly string WAIT_CONNECTION_ALIAS = "server_connection_wait_mandatory";

	private bool blockingScreen;

	private NetworkConnection _connection;

	public NetworkInputBlocker(NetworkConnection n)
	{
		_connection = n;
		_connection.OnNetworkEstablished += OnNetworkEstablished;
	}

	public void Init()
	{
		if (UsersGetInputBlocked())
		{
			BlockInput();
		}
	}

	private void UsersSetInputBlocked(bool inputBlocked)
	{
		UserManager.SetGlobalVariable("InputBlocked", inputBlocked.ToString());
	}

	private bool UsersGetInputBlocked()
	{
		string globalVariable = UserManager.GetGlobalVariable("InputBlocked");
		if (globalVariable == null)
		{
			return false;
		}
		return bool.Parse(globalVariable);
	}

	public void BlockInputUntilNetworkEstablished()
	{
		UsersSetInputBlocked(true);
		BlockInput();
	}

	private void OnNetworkEstablished()
	{
		if (UsersGetInputBlocked())
		{
			UsersSetInputBlocked(false);
			UnblockInput();
		}
	}

	private void BlockInput()
	{
		if (!blockingScreen)
		{
			if (_connection.IsNetworkEstablished())
			{
				Debug.LogError("Blocking Input when we have a connection");
			}
			blockingScreen = true;
			UIBlocker.Instance.Block(UIBlocker.Priority.Preloader);
			LoadingIcon.Instance.EnableLoadingScreen(WAIT_CONNECTION_ALIAS);
		}
	}

	private void UnblockInput()
	{
		blockingScreen = false;
		UIBlocker.Instance.Unblock(UIBlocker.Priority.Preloader);
		LoadingIcon instance = LoadingIcon.Instance;
		string wAIT_CONNECTION_ALIAS = WAIT_CONNECTION_ALIAS;
		instance.DisableLoadingScreen(0.5f, null, wAIT_CONNECTION_ALIAS);
	}
}
