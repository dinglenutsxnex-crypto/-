using Network.core.events;
using SF3.UserData;
using sf3DTO;

public class CreatePlayerNetworkState : TCPNetworkState
{
	public override void TCPStart(object data)
	{
		CreatePlayerRequest createPlayerRequestData = UserDataController.GetCreatePlayerRequestData();
		createPlayerRequestData.Version = NetworkConnection.current.CurrentConfigVersion.full;
		NetworkConnection.Send("create_player", createPlayerRequestData, OnCreatePlayer);
	}

	public override void TCPCleanup()
	{
		NetworkConnection.current.RemoveCallbacks(OnCreatePlayer);
	}

	private void OnCreatePlayer(NetworkEvent e)
	{
		e.HandleErrorAsDialog(RequestErrorCode.InvalidDisplayName);
		if (e.success)
		{
			UserManager.SetPlayerID(e);
			if (NetworkConnection.current.ParsePlayer(e))
			{
				OnSuccess(typeof(RefreshBattlesNetworkState), null);
			}
			else
			{
				OnFail("Parse Player error");
			}
		}
		else
		{
			OnFail("CreatePlayer error");
		}
	}

	public override void TCPStop()
	{
		Disconnect();
	}
}
