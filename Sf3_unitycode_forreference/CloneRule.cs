using System.Collections.Generic;
using Jint.Native;
using Nekki.Yaml;
using SF3;

public class CloneRule : FightRule
{
	public CloneRule(Mapping rule)
		: base(rule)
	{
		base.startOnStage = FightController.EFightStage.RoundStart;
		base.stopOnStage = FightController.EFightStage.RoundEnd;
	}

	public CloneRule(Dictionary<string, JsValue> rule)
		: base(rule)
	{
	}

	public override void StartRule()
	{
		base.StartRule();
	}

	public override void StopRule()
	{
		base.StopRule();
	}
}
