using System;
using System.Collections.Generic;
using System.Linq;
using Jint.Native;
using Jint.Native.Array;
using Nekki.Yaml;
using SF3.GameModels;
using SimpleJSON;
using sf3DTO;

public class RoundInfo
{
	public List<Rule> rules { get; private set; }

	public ModelInfo warrior { get; private set; }

	private RoundInfo()
	{
		rules = new List<Rule>();
	}

	public static RoundInfo Create(Mapping roundData)
	{
		RoundInfo roundInfo = new RoundInfo();
		roundInfo.warrior = new ModelInfo(roundData.GetMapping("ModelInfo"));
		roundInfo.CreateRules(RulesController.Instance.ConvertYaml(roundData.GetSequence("Rules")));
		return roundInfo;
	}

	public static RoundInfo Create(GeneratedRound roundData)
	{
		RoundInfo roundInfo = new RoundInfo();
		roundInfo.warrior = new ModelInfo(roundData.Warrior);
		roundInfo.CreateRules(roundData.Rules.RepeatedToList());
		return roundInfo;
	}

	public static RoundInfo Create(Dictionary<string, JsValue> roundData)
	{
		RoundInfo roundInfo = new RoundInfo();
		roundInfo.warrior = new ModelInfo(roundData);
		return roundInfo;
	}

	public void CreateRules(ArrayInstance rulesData, int ruleNumber)
	{
		rules.Clear();
		for (int i = 0; i < rulesData.Length; i++)
		{
			if (rulesData[i].AsDictionary()["RuleBlocks"].AsArray().Length > ruleNumber)
			{
				Dictionary<string, JsValue> ruleData = rulesData[i].AsDictionary()["RuleBlocks"].AsArray()[ruleNumber].AsDictionary();
				rules.Add(Rule.RuleFabric(ruleData));
			}
		}
	}

	public static RoundInfo Create(BrawlerEnemy enenmyData)
	{
		RoundInfo roundInfo = new RoundInfo();
		roundInfo.warrior = new ModelInfo(enenmyData);
		return roundInfo;
	}

	private void CreateRules(List<RoundRule> roundRules)
	{
		foreach (RoundRule roundRule in roundRules)
		{
			rules.Add(Rule.RuleFabric(roundRule));
		}
	}

	public Mapping ToYAML()
	{
		List<Node> list = new List<Node>();
		list.Add(warrior.ToYAML());
		if (rules.Count > 0)
		{
			list.Add(new Sequence("Rules", ((IEnumerable<Rule>)rules).Select((Func<Rule, Node>)((Rule ruleValue) => ruleValue.ToYaml())).ToList()));
		}
		return new Mapping("Round", list);
	}

	public JSONClass ToJSON()
	{
		throw new NotImplementedException();
	}

	public RoundInfo Clone()
	{
		RoundInfo result = MemberwiseClone() as RoundInfo;
		result.rules = new List<Rule>();
		rules.ForEach(delegate(Rule ruleValue)
		{
			result.rules.Add(ruleValue);
		});
		return result;
	}
}
