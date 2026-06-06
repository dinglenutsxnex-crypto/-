using Network.core.events;
using SF3.UserData;

public class RefreshBattlesNetworkState : TCPNetworkState
{
	public override void TCPStart(object data)
	{
		RefreshBattlesProcessing.RefreshBattles(OnRefreshBattles);
	}

	private void OnRefreshBattles(NetworkEvent e)
	{
		if (e.success)
		{
			OnSuccess(typeof(ActiveNetworkState), null);
		}
		else
		{
			OnFail("Failed to refresh battles");
		}
	}

	public override void TCPCleanup()
	{
		NetworkConnection.current.RemoveCallbacks(OnRefreshBattles);
	}

	public override void TCPStop()
	{
		Disconnect();
	}
}
