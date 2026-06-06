using System;

public abstract class NetworkStateBase
{
	protected Action<Type, object> OnSuccess;

	protected Action<string> OnFail;

	protected Action Disconnect;

	public void SetCallbacks(Action<Type, object> OnSuccessCallback, Action<string> OnFailCallback, Action disconnectDelegate)
	{
		OnSuccess = OnSuccessCallback;
		OnFail = OnFailCallback;
		Disconnect = disconnectDelegate;
	}

	public abstract void Start(object data);

	public abstract void Stop();

	public virtual void Cleanup()
	{
	}
}
