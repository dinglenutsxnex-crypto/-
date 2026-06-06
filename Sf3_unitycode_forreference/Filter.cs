using System.Linq;

public class Filter
{
	private readonly IRule rule;

	private readonly string[] tokens;

	public Filter(IRule rule, string[] tokens)
	{
		this.rule = rule;
		this.tokens = tokens;
	}

	public bool Filtered(string message)
	{
		return tokens.All((string token) => rule.Filtered(token, message));
	}
}
