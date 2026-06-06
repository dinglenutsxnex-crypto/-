using System;
using System.Collections.Generic;
using Jint.Native;
using Nekki.Yaml;
using SF3;
using SF3.GameModels;
using SimpleJSON;
using sf3DTO;

public class Rule
{
	private const string PLAYER = "Player";

	private const string ENEMY = "Enemy";

	private const string PLAYER_TYPE_ATTRIBUTE = "ApplyTo";

	public const string SET_APPEARANCE = "SetAppearance";

	public const string SET_ALIAS = "SetAlias";

	public const string SET_GENDER = "SetGender";

	public const string APPLY_FACTOR = "ApplyFactor";

	public const string GIVE_ITEM = "GiveItem";

	public const string GIVE_PERK = "GivePerk";

	public const string GIVE_TAG = "GiveTag";

	public const string SHADOWLESS = "Shadowless";

	private Dictionary<string, string> _attributesRule = new Dictionary<string, string>();

	protected ModelInfo _modelInfo;

	public EPlayerType playerType = EPlayerType.This;

	public string typeRule;

	public string idRule;

	public string Type
	{
		get
		{
			return typeRule.ToLower();
		}
	}

	protected Rule(string id, string type, Dictionary<string, string> attributes, EPlayerType playerType = EPlayerType.This)
	{
		idRule = id;
		typeRule = type;
		this.playerType = playerType;
		_attributesRule = attributes;
		ApplyTo();
	}

	public static Rule RuleFabric(Dictionary<string, JsValue> ruleData)
	{
		string text = ruleData["ID"].AsInteger().ToString();
		Dictionary<string, JsValue> dictionary = ruleData["Attributes"].AsDictionary();
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		Rule globalRule = RulesController.Instance.GetGlobalRule(text);
		EPlayerType ePlayerType = globalRule.playerType;
		foreach (KeyValuePair<string, string> item in globalRule._attributesRule)
		{
			dictionary2.Add(item.Key, item.Value);
		}
		foreach (KeyValuePair<string, JsValue> item2 in dictionary)
		{
			AddAttribute(item2.Key, item2.Value.ToString(), dictionary2);
		}
		return RuleFabric(text, RulesController.Instance.GetGlobalRule(text).typeRule, dictionary2, ePlayerType);
	}

	public static Rule RuleFabric(RoundRule rule)
	{
		string text = rule.RuleId.ToString();
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		Rule globalRule = RulesController.Instance.GetGlobalRule(text);
		EPlayerType ePlayerType = globalRule.playerType;
		foreach (KeyValuePair<string, string> item in globalRule._attributesRule)
		{
			dictionary.Add(item.Key, item.Value);
		}
		foreach (RoundRuleAttribute attr in rule.Attrs)
		{
			AddAttribute(attr.AttrId.ToString(), attr.AttrValue, dictionary);
		}
		return RuleFabric(text, RulesController.Instance.GetGlobalRule(text).typeRule, dictionary, ePlayerType);
	}

	public static Rule RuleFabric(string id, string name, Dictionary<string, string> attributesMap, EPlayerType playerType = EPlayerType.This)
	{
		switch (name)
		{
		case "SetAlias":
			return new RuleAlias(id, name, attributesMap, playerType);
		case "SetAppearance":
			return new RuleAppearance(id, name, attributesMap, playerType);
		case "SetGender":
			return new RuleGender(id, name, attributesMap, playerType);
		default:
			return new Rule(id, name, attributesMap, playerType);
		}
	}

	public double? GetAttributeNumberByType(string nameAttr)
	{
		string attributeByType = GetAttributeByType(nameAttr);
		if (!attributeByType.Equals(string.Empty))
		{
			return double.Parse(attributeByType);
		}
		return null;
	}

	public string GetAttributeStringByType(string nameAttr)
	{
		return GetAttributeByType(nameAttr);
	}

	public string GetAttributeByType(string nameAttr)
	{
		if (_attributesRule.ContainsKey(nameAttr))
		{
			return _attributesRule[nameAttr];
		}
		return string.Empty;
	}

	private void GlobalMerge(string ID)
	{
		Rule globalRule = RulesController.Instance.GetGlobalRule(ID);
		playerType = globalRule.playerType;
		idRule = globalRule.idRule;
		typeRule = globalRule.typeRule;
		foreach (KeyValuePair<string, string> item in globalRule._attributesRule)
		{
			_attributesRule.Add(item.Key, item.Value);
		}
	}

	private static void AddAttribute(string ID, string value, Dictionary<string, string> dict)
	{
		string globalAttributeName = RulesController.Instance.GetGlobalAttributeName(ID);
		if (dict.ContainsKey(globalAttributeName))
		{
			dict[globalAttributeName] = value;
		}
		else
		{
			dict.Add(globalAttributeName, value);
		}
	}

	private void AddAttribute(string ID, string value)
	{
		AddAttribute(ID, value, _attributesRule);
	}

	private void ApplyTo()
	{
		string attributeStringByType = GetAttributeStringByType("ApplyTo");
		if (attributeStringByType.Equals("Enemy"))
		{
			playerType = EPlayerType.Enemy;
		}
	}

	public Mapping ToYaml()
	{
		List<Node> list = new List<Node>();
		foreach (KeyValuePair<string, string> item2 in _attributesRule)
		{
			string globalAttributeID = RulesController.Instance.GetGlobalAttributeID(item2.Key);
			Scalar item = new Scalar(globalAttributeID, item2.Value);
			list.Add(item);
		}
		Mapping entry = new Mapping(idRule, list);
		return new Mapping(idRule, entry);
	}

	public JSONClass ToJSON()
	{
		throw new NotImplementedException();
	}

	public virtual void Cache(ModelInfo modelInfo)
	{
		_modelInfo = modelInfo;
	}

	public virtual void Reset()
	{
	}

	public string ToStringAttributes()
	{
		string text = string.Empty;
		int num = 0;
		int count = _attributesRule.Count;
		foreach (KeyValuePair<string, string> item in _attributesRule)
		{
			string text2 = text;
			text = text2 + item.Key + ":" + item.Value + ((num >= count - 1) ? string.Empty : ", ");
			num++;
		}
		return text;
	}

	public override string ToString()
	{
		return "Rule [name:" + typeRule + " {" + ToStringAttributes() + "} ]";
	}
}
