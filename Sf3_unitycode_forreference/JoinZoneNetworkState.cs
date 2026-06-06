using System.Collections;
using Network.core.events;
using UnityEngine;
using common;

public class JoinZoneNetworkState : TCPNetworkState
{
	private Coroutine JoinZoneTimeoutCoroutine;

	public override void TCPStart(object data)
	{
		NetworkConnection.current.addEventListener("join_zone", OnJoinZone);
		JoinZoneTimeoutCoroutine = Routiner.GoDelayed(OnJoinZoneTimeout(), NetworkConnection.Settings.JoinZoneTimeout.ToSeconds());
	}

	public override void TCPCleanup()
	{
		cancelJoinZoneTimeout();
		NetworkConnection.current.removeEventListener("join_zone", OnJoinZone);
	}

	public override void TCPStop()
	{
		Disconnect();
	}

	private void cancelJoinZoneTimeout()
	{
		if (JoinZoneTimeoutCoroutine != null)
		{
			Routiner.Stop(JoinZoneTimeoutCoroutine);
			JoinZoneTimeoutCoroutine = null;
		}
	}

	private IEnumerator OnJoinZoneTimeout()
	{
		cancelJoinZoneTimeout();
		OnFail("OnJoinZoneTimeout");
		yield return 0;
	}

	private void OnJoinZone(NetworkEvent e)
	{
		cancelJoinZoneTimeout();
		JoinZoneEvent extensible = e.getExtensible<JoinZoneEvent>();
		Debug.Log(string.Format("OnJoinZone Player Exists = {0}", extensible.PlayerExists));
		if (extensible.PlayerExists)
		{
			OnSuccess(typeof(GetPlayerNetworkState), null);
		}
		else if (!NetworkConnection.current.canCreatePlayer())
		{
			OnSuccess(typeof(WaitEndOfTutorialNetworkState), null);
		}
		else
		{
			OnSuccess(typeof(CreatePlayerNetworkState), null);
		}
	}
}
