using System;
using System.Collections.Generic;
using Jint.Native;
using Nekki.Yaml;
using SF3;
using SimpleJSON;
using UnityEngine;

public abstract class FightRule
{
	public enum FightRuleTypes
	{
		None = 0,
		ShortRound = 1,
		Darkness = 2,
		InvertedJoystick = 3,
		RestrictedAnimations = 4,
		GiveItem = 5,
		LoseAnimations = 6,
		ScoreFight = 7,
		WithoutHP = 8,
		RingOut = 9,
		InvisibleWarrior = 10,
		ApplyFactor = 11,
		WithoutSF = 12,
		WithoutTime = 13,
		RandomEquipment = 14,
		Clone = 15
	}

	public int id { get; protected set; }

	public string Name { get; protected set; }

	public FightRuleTypes Type { get; protected set; }

	public FightController.EFightStage startOnStage { get; protected set; }

	public FightController.EFightStage stopOnStage { get; protected set; }

	protected FightRule()
	{
		startOnStage = FightController.EFightStage.RoundStart;
		stopOnStage = FightController.EFightStage.RoundEnd;
	}

	protected FightRule(Mapping rule)
		: this()
	{
	}

	protected FightRule(Dictionary<string, JsValue> rule)
		: this()
	{
	}

	public static void Create(Mapping rule)
	{
	}

	public static FightRule Create(KeyValuePair<string, JsValue> battleData)
	{
		FightRule fightRule = null;
		Dictionary<string, JsValue> dictionary = battleData.Value.AsDictionary();
		string key = battleData.Key;
		FightRuleTypes fightRuleTypes = (FightRuleTypes)Enum.Parse(typeof(FightRuleTypes), key);
		switch (fightRuleTypes)
		{
		case FightRuleTypes.RestrictedAnimations:
			fightRule = new RestrictedAnimationsRule(dictionary);
			break;
		case FightRuleTypes.GiveItem:
			fightRule = new GiveItemRule(dictionary);
			break;
		case FightRuleTypes.LoseAnimations:
			fightRule = new LoseAnimationsRule(dictionary);
			break;
		case FightRuleTypes.RandomEquipment:
			fightRule = new RandomEquipmentRule(dictionary);
			break;
		case FightRuleTypes.Clone:
			fightRule = new CloneRule(dictionary);
			break;
		}
		fightRule.id = int.Parse(dictionary["ID"].ToString());
		fightRule.Type = fightRuleTypes;
		fightRule.Name = key;
		return fightRule;
	}

	public virtual void StartRule()
	{
		Debug.Log(string.Format("---[StartRule] Name: [{0}]; Type: [{1}]", Name, Type));
	}

	public virtual void StopRule()
	{
		Debug.Log(string.Format("---[StopRule] Name: [{0}]; Type: [{1}]", Name, Type));
	}

	public Mapping ToYaml()
	{
		List<Node> list = new List<Node>();
		list.Add(new Scalar("ID", id.ToString()));
		List<Node> entries = list;
		return new Mapping(Name, entries);
	}

	public JSONClass ToJSON()
	{
		throw new NotImplementedException();
	}

	public virtual FightRule Clone()
	{
		return MemberwiseClone() as FightRule;
	}
}
