using System.Collections.Generic;
using Jint.Native;
using Nekki.Yaml;
using SF3;
using SF3.GameModels;
using SF3.Moves;

public class LoseAnimationsRule : FightRule
{
	private int _targetID;

	public List<string> animations { get; private set; }

	public EPlayerType target { get; private set; }

	public EPlayerType applyTo { get; private set; }

	public LoseAnimationsRule(Mapping rule)
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
		target = Model.GetPlayerTypeByName(YamlUtils.GetText(rule, "Target", "Both"));
		applyTo = Model.GetPlayerTypeByName(YamlUtils.GetText(rule, "ApplyTo", "Me"));
	}

	public LoseAnimationsRule(Dictionary<string, JsValue> rule)
		: base(rule)
	{
	}

	public override void StartRule()
	{
		base.StartRule();
		_targetID = ((target == EPlayerType.Both) ? (-1) : ((target == EPlayerType.This) ? 1 : 2));
		BattleController.RegisterEventCallback(ETriggerEvents.EVENT_ANIMATION_END, CheckAnimationStart);
	}

	public override void StopRule()
	{
		base.StopRule();
		BattleController.RemoveEventCallback(ETriggerEvents.EVENT_ANIMATION_END, CheckAnimationStart);
	}

	private void CheckAnimationStart(BattleEventArgs args)
	{
		ModelInfoAnimation modelInfoAnimation = (ModelInfoAnimation)args.EventData;
		if (args.SenderID != _targetID || _targetID == -1)
		{
			return;
		}
		foreach (string animation in animations)
		{
			if (modelInfoAnimation.animation.name.Equals(animation))
			{
				ERoundResult winner = (((applyTo != EPlayerType.This || args.SenderID != 2) && (applyTo != EPlayerType.Enemy || args.SenderID != 1)) ? ERoundResult.ENEMY_WIN : ERoundResult.PLAYER_WIN);
				FightController.Instance.WinCurrentRound(winner);
				break;
			}
		}
	}
}
