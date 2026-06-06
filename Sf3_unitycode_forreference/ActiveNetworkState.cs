using Network.core.events;

public class ActiveNetworkState : TCPNetworkState
{
	public override void TCPStart(object data)
	{
	}

	public override void TCPCleanup()
	{
	}

	public override void TCPStop()
	{
		Disconnect();
	}

	private void OnConnectionLost(NetworkEvent e)
	{
		OnFail("Connection Lost");
	}
}
