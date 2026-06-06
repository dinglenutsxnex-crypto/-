using SF3.UserData;

public class WaitEndOfTutorialNetworkState : TCPNetworkState
{
	public override void TCPStart(object data)
	{
		UserManager.OnStartingTutorialCompleted += OnStartingTutorialCompleted;
	}

	public override void TCPCleanup()
	{
		UserManager.OnStartingTutorialCompleted -= OnStartingTutorialCompleted;
	}

	public override void TCPStop()
	{
		Disconnect();
	}

	private void OnStartingTutorialCompleted()
	{
		OnSuccess(typeof(CreatePlayerNetworkState), null);
	}
}
