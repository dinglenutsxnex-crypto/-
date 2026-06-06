using System;
using Network.core.events;
using common;
using sf3DTO;

public class GetPlayerNetworkState : TCPNetworkState
{
	public event Action<NetworkEvent> OnGotPlayer = delegate
	{
	};

	public override void TCPStart(object data)
	{
		GetPlayerRequest getPlayerRequest = NetworkConnection.current.CreateGetPlayerRequest();
		NetworkConnection.Send("get_player", getPlayerRequest, OnGetPlayer, null, OfflineRequestQueue.GetTimeoutForBatchSize(getPlayerRequest.OfflineRequestBatch.Requests.Count));
		NetworkConnection.current.BlockInputUntilNetworkEstablished();
	}

	public override void TCPCleanup()
	{
		this.OnGotPlayer = null;
		NetworkConnection.current.RemoveCallbacks(OnGetPlayer);
	}

	private void OnGetPlayer(NetworkEvent e)
	{
		this.OnGotPlayer.InvokeSafe(e);
		if (e.HandleError(sf3DTO.RequestErrorCode.UnrecoverableOfflineRequestError))
		{
			NetworkConnection.current.HandleUnrecoverableError();
			OnFail("GetPlayer unrecoverable error");
		}
		else if (e.HandleError(sf3DTO.RequestErrorCode.RecoverableOfflineRequestError) || e.isAnyClientIssue)
		{
			OnFail("GetPlayer client or recoverable error");
		}
		else if (e.HandleError(sf3DTO.RequestErrorCode.InvalidConfigVersion))
		{
			string text = e.ErrorCodeAsString();
			OnFail("GetPlayer " + text + " error");
			NetworkConnection.current.RestartConnection(text, false);
		}
		else if (e.HandleError(common.RequestErrorCode.Error) || !e.WasErrorHandled)
		{
			NetworkConnection.current.HandleUnrecoverableError();
			OnFail("GetPlayer undefined error code - Undefined Behaviour./nServer should not send this error code: ");
		}
		else if (e.success)
		{
			if (NetworkConnection.current.ParsePlayer(e))
			{
				OnSuccess(typeof(RefreshBattlesNetworkState), null);
			}
			else
			{
				OnFail("Parse Player error");
			}
		}
	}

	public override void TCPStop()
	{
		Disconnect();
	}
}
