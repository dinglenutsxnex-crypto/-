using Network.core.events;

public abstract class TCPNetworkState : NetworkStateBase
{
	public sealed override void Start(object data)
	{
		NetworkConnection.current.addEventListener("connection_lost", OnError);
		NetworkConnection.current.addEventListener("socket_error", OnError);
		NetworkConnection.current.addEventListener("logout", OnError);
		TCPStart(data);
	}

	public sealed override void Cleanup()
	{
		NetworkConnection.current.removeEventListener("connection_lost", OnError);
		NetworkConnection.current.removeEventListener("socket_error", OnError);
		NetworkConnection.current.removeEventListener("logout", OnError);
		TCPCleanup();
	}

	public sealed override void Stop()
	{
		TCPStop();
	}

	public abstract void TCPStart(object data);

	public abstract void TCPStop();

	public virtual void TCPCleanup()
	{
	}

	private void OnError(NetworkEvent e)
	{
		OnFail("TCP Connection error");
	}
}
