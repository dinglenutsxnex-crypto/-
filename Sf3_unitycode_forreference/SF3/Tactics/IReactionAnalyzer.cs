using System.Collections.Generic;
using SF3.Moves;

namespace SF3.Tactics
{
	public interface IReactionAnalyzer
	{
		List<InfoTrigger> GetReaction();
	}
}
