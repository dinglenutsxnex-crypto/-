using System;

public class NetworkInitializer
{
	private bool _inited;

	private static NetworkInitializer _instance;

	public bool Inited
	{
		get
		{
			return _inited;
		}
	}

	public static NetworkInitializer current
	{
		get
		{
			return _instance ?? (_instance = new NetworkInitializer());
		}
	}

	public event Action<bool> onInitFinished = delegate
	{
	};

	public void DestroySelf()
	{
		_instance = null;
	}

	public void Init()
	{
		NetworkConnection.current.OnNetworkEstablished += delegate
		{
			OnInitFinished(true);
		};
		NetworkConnection.current.OnNetworkCutoff += delegate
		{
			OnInitFinished(false);
		};
		NetworkConnection.current.OnStateStartEvent.addEventListener(NetworkState.WaitEndOfTutorial, delegate
		{
			OnInitFinished(false);
		});
		NetworkConnection.current.OnStateStartEvent.addEventListener(NetworkState.Timeout, delegate
		{
			OnInitFinished(false);
		});
		NetworkConnection.current.Init();
	}

	private void OnInitFinished(bool networkFullyEstablished)
	{
		if (!_inited)
		{
			_inited = true;
			this.onInitFinished(networkFullyEstablished);
		}
	}
}
