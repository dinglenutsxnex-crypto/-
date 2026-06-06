using System;

public static class NetworkState
{
	public static Type Active
	{
		get
		{
			return typeof(ActiveNetworkState);
		}
	}

	public static Type Balancer
	{
		get
		{
			return typeof(BalancerNetworkState);
		}
	}

	public static Type Config
	{
		get
		{
			return typeof(ConfigNetworkState);
		}
	}

	public static Type Connection
	{
		get
		{
			return typeof(ConnectionNetworkState);
		}
	}

	public static Type Login
	{
		get
		{
			return typeof(LoginNetworkState);
		}
	}

	public static Type CreatePlayer
	{
		get
		{
			return typeof(CreatePlayerNetworkState);
		}
	}

	public static Type GetPlayer
	{
		get
		{
			return typeof(GetPlayerNetworkState);
		}
	}

	public static Type JoinZone
	{
		get
		{
			return typeof(JoinZoneNetworkState);
		}
	}

	public static Type RefreshBattles
	{
		get
		{
			return typeof(RefreshBattlesNetworkState);
		}
	}

	public static Type Timeout
	{
		get
		{
			return typeof(TimeoutNetworkState);
		}
	}

	public static Type WaitEndOfTutorial
	{
		get
		{
			return typeof(WaitEndOfTutorialNetworkState);
		}
	}
}
