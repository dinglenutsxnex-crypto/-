using System;
using UnityEngine;

public class NetworkChain
{
	public EventListener<CallEventArgsBase<Type>, Type> OnStateSuccessEvent = new EventListener<CallEventArgsBase<Type>, Type>();

	public EventListener<CallEventArgsBase<Type>, Type> OnStateStartEvent = new EventListener<CallEventArgsBase<Type>, Type>();

	private float? timeout;

	private NetworkStateBase currentState;

	private readonly Action _disconnectDelegate;

	private float NextTimeout
	{
		get
		{
			NetworkSettings.Config.TimeoutParamsInfo timeoutParams = Config.TimeoutParams;
			float? num = timeout;
			float value;
			if (!num.HasValue)
			{
				value = timeoutParams.Min.ToSeconds();
			}
			else
			{
				float a = timeoutParams.Max.ToSeconds();
				float? num2 = timeout;
				value = Mathf.Min(a, num2.Value * timeoutParams.Multiplier);
			}
			timeout = value;
			float? num3 = timeout;
			return num3.Value;
		}
	}

	public NetworkSettings.Config Config { get; set; }

	public event Action OnStateEndedWithoutSuccess = delegate
	{
	};

	public NetworkChain(Action disconnectDelegate)
	{
		_disconnectDelegate = disconnectDelegate;
		OnStateStartEvent.addEventListener(typeof(ActiveNetworkState), OnActiveNetworkState);
	}

	private void ResetTimeout()
	{
		timeout = null;
	}

	private void OnActiveNetworkState(CallEventArgsBase<Type> obj)
	{
		ResetTimeout();
	}

	public void StartChain()
	{
		if (NetworkConnection.current.InsideDarkPocket)
		{
			ResetTimeout();
			SwitchToTimeoutState();
		}
		else if (currentState == null || currentState.GetType() == typeof(TimeoutNetworkState))
		{
			SwitchToState(typeof(BalancerNetworkState), null);
		}
		else
		{
			Debug.LogError("StartChain can only be called when timeout ended or this is the first call of the app");
		}
	}

	public Type GetCurrentStateType()
	{
		return (currentState != null) ? currentState.GetType() : null;
	}

	public string GetCurrentStateName()
	{
		return (currentState != null) ? currentState.ToString() : "Null";
	}

	public void StopCurrentState(bool withTimeout = true)
	{
		Debug.Log("Stopping state: " + GetCurrentStateName());
		if (currentState != null)
		{
			NetworkStateBase networkStateBase = currentState;
			currentState = null;
			networkStateBase.Cleanup();
			networkStateBase.Stop();
			networkStateBase.SetCallbacks(delegate
			{
			}, delegate
			{
			}, delegate
			{
			});
			this.OnStateEndedWithoutSuccess();
		}
		else
		{
			Debug.LogError("State null was stopped. This should not happen.");
		}
		if (withTimeout)
		{
			SwitchToTimeoutState();
		}
		else
		{
			StartChain();
		}
	}

	private void OnStateSuccess(Type nextStateType, object dataForNextState)
	{
		OnStateSuccessEvent.callEvent(currentState.GetType());
		SwitchToState(nextStateType, dataForNextState);
	}

	private void OnStateFail(string failReason)
	{
		Debug.LogWarning("State Failed: " + GetCurrentStateName() + " with Error: " + failReason);
		StopCurrentState();
	}

	private void SwitchToState(Type nextStateType, object data, Action<Type, object> OnSuccessCallback = null)
	{
		Debug.Log(string.Format("Switching network state: {0}->{1}", GetCurrentStateName(), nextStateType));
		if (OnSuccessCallback == null)
		{
			OnSuccessCallback = OnStateSuccess;
		}
		CleanupCurrentState();
		currentState = CreateStateOfType(nextStateType);
		currentState.SetCallbacks(OnSuccessCallback, OnStateFail, _disconnectDelegate);
		currentState.Start(data);
		OnStateStartEvent.callEvent(currentState.GetType(), currentState);
	}

	private void SwitchToTimeoutState()
	{
		SwitchToState(typeof(TimeoutNetworkState), NextTimeout, OnTimeoutStateFinished);
	}

	private void CleanupCurrentState()
	{
		if (currentState != null)
		{
			currentState.Cleanup();
			currentState = null;
		}
	}

	private void OnTimeoutStateFinished(Type nextStateType, object dataForNextState)
	{
		StartChain();
	}

	private static NetworkStateBase CreateStateOfType(Type type)
	{
		NetworkStateBase networkStateBase = Activator.CreateInstance(type) as NetworkStateBase;
		if (networkStateBase == null)
		{
			Debug.LogError(string.Concat("Error: ", type, " is not derived from NetworkStateBase"));
		}
		return networkStateBase;
	}
}
