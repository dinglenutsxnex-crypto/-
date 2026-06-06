using System.Collections.Generic;
using SF3.Moves;

public class MovesData
{
	public Dictionary<string, InfoAnimation> Animations = new Dictionary<string, InfoAnimation>();

	public Dictionary<string, AnimationBinaries> Binaries = new Dictionary<string, AnimationBinaries>();

	public Dictionary<string, InfoTriggerSet> Triggers = new Dictionary<string, InfoTriggerSet>();
}
