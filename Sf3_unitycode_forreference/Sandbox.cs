public class Sandbox
{
	private static Sandbox _instance;

	public Sandbox()
	{
		AddConsoleCommand();
	}

	public static void Init()
	{
		if (_instance == null)
		{
			_instance = new Sandbox();
		}
	}

	private void AddConsoleCommand()
	{
	}
}
