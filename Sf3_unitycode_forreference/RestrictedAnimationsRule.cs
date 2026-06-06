using System.Collections.Generic;
using Jint.Native;
using Nekki.Yaml;
using SF3;
using SF3.GameModels;

public class RestrictedAnimationsRule : FightRule
{
	public List<string> animations { get; private set; }

	public EPlayerType applyTo { get; private set; }

	public RestrictedAnimationsRule(Mapping rule)
		: base(rule)
	{
		base.startOnStage = FightController.EFightStage.RoundStart;
		base.stopOnStage = FightController.EFightStage.RoundEnd;
		animations = new List<string>();
		Sequence sequence = rule.GetSequence("Animations");
		foreach (Scalar item in sequence.nodesInside)
		{
			animations.Add(item.text.ToLower());
		}
		applyTo = Model.GetPlayerTypeByName(YamlUtils.GetText(rule, "ApplyTo", "Me"));
	}

	public RestrictedAnimationsRule(Dictionary<string, JsValue> rule)
		: base(rule)
	{
	}

	public override void StartRule()
	{
		base.StartRule();
		List<Model> list = new List<Model>();
		if (applyTo == EPlayerType.Both)
		{
			list.Add(ModelsManager.Instance.Player);
			list.Add(ModelsManager.Instance.Enemy);
		}
		else if (applyTo == EPlayerType.This)
		{
			list.Add(ModelsManager.Instance.Player);
		}
		else if (applyTo == EPlayerType.Enemy)
		{
			list.Add(ModelsManager.Instance.Enemy);
		}
		foreach (Model item in list)
		{
			item.RestrictAnimations(animations);
		}
	}
}
