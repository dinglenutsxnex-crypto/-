public class ContainsExact : IRule
{
	private static IRule instance;

	public static IRule Instance
	{
		get
		{
			return instance ?? (instance = new ContainsExact());
		}
	}

	public bool Filtered(string token, string message)
	{
		return message.Contains(token);
	}
}
