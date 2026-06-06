using System;
using Network.core.data;

public class SFSSF3Auth : AuthManager
{
	public SFSSF3Auth()
	{
		Init();
		if (Login == null || Login.Length == 0)
		{
			Login = Guid.NewGuid().ToString();
		}
	}
}
