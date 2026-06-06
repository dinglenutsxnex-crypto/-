public class StartsWith : IRule
{
	private static IRule instance;

	public static IRule Instance
	{
		get
		{
			return instance ?? (instance = new StartsWith());
		}
	}

	public bool Filtered(string token, string message)
	{
		return message.StartsWith(token);
	}
}
