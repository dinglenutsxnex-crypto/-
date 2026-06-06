public class ContainsAnyCase : IRule
{
	private static IRule instance;

	public static IRule Instance
	{
		get
		{
			return instance ?? (instance = new ContainsAnyCase());
		}
	}

	public bool Filtered(string token, string message)
	{
		return message.ToLower().Contains(token.ToLower());
	}
}
