public class EndsWith : IRule
{
	private static IRule instance;

	public static IRule Instance
	{
		get
		{
			return instance ?? (instance = new EndsWith());
		}
	}

	public bool Filtered(string token, string message)
	{
		return message.EndsWith(token);
	}
}
