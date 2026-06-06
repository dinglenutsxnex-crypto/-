using Network.core.events;
using UnityEngine;

public class ConnectionNetworkState : TCPNetworkState
{
	private Coroutine timeoutRoutine;

	public override void TCPStart(object data)
	{
		BalancerItem balancerItem = (BalancerItem)data;
		if (balancerItem != null)
		{
			NetworkConnection.current.addEventListener("connection", OnConnectionSuccess);
			NetworkConnection.current.addEventListener("connection_error", OnConnectionFail);
			NetworkConnection.current.Connect(balancerItem.ip, balancerItem.port);
			timeoutRoutine = Routiner.GoDelayed(delegate
			{
				OnFail("Timeout");
				timeoutRoutine = null;
			}, NetworkConnection.Settings.ConnectTimeout.ToSeconds());
		}
		else
		{
			OnFail("Balancer is null!");
		}
	}

	public override void TCPCleanup()
	{
		NetworkConnection.current.removeEventListener("connection", OnConnectionSuccess);
		NetworkConnection.current.removeEventListener("connection_error", OnConnectionFail);
		Routiner.Stop(timeoutRoutine);
	}

	private void OnConnectionSuccess(NetworkEvent e)
	{
		Debug.Log("OnConnectionSuccess");
		OnSuccess(typeof(LoginNetworkState), null);
	}

	private void OnConnectionFail(NetworkEvent e)
	{
		OnFail("Connection Failed");
	}

	public override void TCPStop()
	{
		Disconnect();
	}
}
