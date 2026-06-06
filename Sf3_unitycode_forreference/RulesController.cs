using System;
using System.Collections.Generic;
using Jint.Native;
using Nekki.Yaml;
using SF3;
using SF3.GameModels;
using SF3.Items;
using SF3_Attributes;
using UnityEngine;
using sf3DTO;

public class RulesController
{
	private Dictionary<string, string> _ruleAttributesMap;

	private Dictionary<string, Rule> _rules;

	private Dictionary<string, JsValue> _globalAttributes;

	private Dictionary<string, JsValue> _globalRules;

	private Dictionary<string, JsValue> _globalEnums;

	private ModelInfo _playerInfo;

	private ModelInfo _enemyInfo;

	private RoundInfo _round;

	public Action ClearRules;

	private static RulesController _instance;

	public static RulesController Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new RulesController();
			}
			return _instance;
		}
	}

	public RulesController()
	{
		_rules = new Dictionary<string, Rule>();
		_ruleAttributesMap = new Dictionary<string, string>();
		_globalEnums = JS.Instance.GetScope("Attributes").AsDictionary();
		_globalRules = JS.Instance.GetScope("RoundRules").AsDictionary();
		_globalAttributes = JS.Instance.GetScope("RoundRuleAttributes").AsDictionary();
		InitGlobalAttributes();
		InitGlobalRules();
	}

	public static void Clear()
	{
		_instance = null;
	}

	public void Initialize(RoundInfo round, ModelInfo playerInfo, ModelInfo enemyInfo)
	{
		_round = round;
		_playerInfo = playerInfo;
		_enemyInfo = enemyInfo;
		ApplyToRules();
		StartRules();
	}

	public bool HasActiveRule(string ruleName)
	{
		ruleName = ruleName.ToLower();
		return _round.rules.Find((Rule e) => e.Type == ruleName) != null;
	}

	private void ApplyToRules()
	{
		_playerInfo.rules.Clear();
		_enemyInfo.rules.Clear();
		foreach (Rule rule in _round.rules)
		{
			if (rule.playerType == EPlayerType.This)
			{
				_playerInfo.rules.Add(rule);
			}
			else
			{
				_enemyInfo.rules.Add(rule);
			}
		}
	}

	private void StartRules()
	{
		ClearRules = null;
		StartRulesModel(_playerInfo, 1);
		StartRulesModel(_enemyInfo, 2);
	}

	private void StartRulesModel(ModelInfo modelInfo, int modelID)
	{
		foreach (Rule rule in modelInfo.rules)
		{
			rule.Cache(modelInfo);
			ClearRules = (Action)Delegate.Combine(ClearRules, new Action(rule.Reset));
		}
		StartApplyGender(modelInfo);
		StartApplyAlias(modelInfo);
		StartApplyItem(modelInfo);
		StartApplyPerk(modelInfo);
		StartApplyTag(modelInfo);
		StartApplyFactor(modelInfo, modelID);
		StartApplyShadowless(modelInfo, modelID);
		StartApplyAppearence(modelInfo);
	}

	private void StartApplyAppearence(ModelInfo modelInfo)
	{
		List<Rule> rulesByType = modelInfo.GetRulesByType("SetAppearance");
		Dictionary<string, JsValue> dictionary = JS.Instance.GetScope("WarriorAppearencesMap").AsDictionary();
		foreach (Rule item in rulesByType)
		{
			string attributeStringByType = item.GetAttributeStringByType("ID");
			Dictionary<string, JsValue> dictionary2 = dictionary[attributeStringByType].AsDictionary();
			Dictionary<string, JsValue> dictionary3 = dictionary2["Head"].AsDictionary();
			modelInfo.SetHead(dictionary3["Head"].ToString());
			sf3DTO.Color colorData;
			modelInfo.ParseColor(dictionary2["HairColor"].AsDictionary(), out colorData);
			modelInfo.SetHairColor(colorData);
			sf3DTO.Color colorData2;
			modelInfo.ParseColor(dictionary2["SkinColor"].AsDictionary(), out colorData2);
			modelInfo.SetSkinColor(colorData2);
		}
	}

	private void StartApplyAlias(ModelInfo modelInfo)
	{
		List<Rule> rulesByType = modelInfo.GetRulesByType("SetAlias");
		foreach (Rule item in rulesByType)
		{
			modelInfo.SetAlias(item.GetAttributeStringByType("Alias"));
		}
	}

	private void StartApplyGender(ModelInfo modelInfo)
	{
		List<Rule> rulesByType = modelInfo.GetRulesByType("SetGender");
		foreach (Rule item in rulesByType)
		{
			modelInfo.SetGender((Gender)item.GetAttributeNumberByType("Gender").Value);
		}
	}

	private void StartApplyItem(ModelInfo modelInfo)
	{
		List<Rule> rulesByType = modelInfo.GetRulesByType("GiveItem");
		foreach (Rule item in rulesByType)
		{
			double? attributeNumberByType = item.GetAttributeNumberByType("ID");
			if (attributeNumberByType.HasValue)
			{
				Equipment equipment;
				if (ItemsManager.TryGetEquipmentById((int)attributeNumberByType.Value, out equipment))
				{
					modelInfo.EquipItemNotExisted(equipment);
				}
				else
				{
					Messenger.Error(string.Format("No equipment by ID: [{0}] found.", attributeNumberByType.ToString()));
				}
			}
		}
	}

	private void StartApplyPerk(ModelInfo modelInfo)
	{
		List<Rule> rulesByType = modelInfo.GetRulesByType("GivePerk");
		foreach (Rule item in rulesByType)
		{
			double? attributeNumberByType = item.GetAttributeNumberByType("ID");
			if (attributeNumberByType.HasValue)
			{
				SF3.Items.Perk perk;
				if (ItemsManager.TryGetPerkById((int)attributeNumberByType.Value, out perk))
				{
					modelInfo.AddPerkInCollection(perk);
				}
				else
				{
					Messenger.Error(string.Format("No perk with ID: [{0}]", attributeNumberByType.ToString()));
				}
			}
		}
	}

	private void StartApplyTag(ModelInfo modelInfo)
	{
		List<Rule> rulesByType = modelInfo.GetRulesByType("GiveTag");
		foreach (Rule item in rulesByType)
		{
			string attributeStringByType = item.GetAttributeStringByType("Tag");
			if (!attributeStringByType.Equals(string.Empty))
			{
				modelInfo.AddTag(attributeStringByType);
			}
		}
	}

	private void StartApplyFactor(ModelInfo modelInfo, int modelID)
	{
		List<Rule> rulesByType = modelInfo.GetRulesByType("ApplyFactor");
		foreach (Rule item in rulesByType)
		{
			foreach (KeyValuePair<string, JsValue> globalEnum in _globalEnums)
			{
				string text = globalEnum.Value.AsString();
				double? attributeNumberByType = item.GetAttributeNumberByType(text);
				if (attributeNumberByType.HasValue)
				{
					AttributeType attributeType = (AttributeType)Enum.Parse(typeof(AttributeType), text);
					ModelsAttributesController.Instance.AddAttributeFactorModifier(modelID, attributeType, (float)attributeNumberByType.Value);
				}
			}
		}
	}

	private void StartApplyShadowless(ModelInfo modelInfo, int modelID)
	{
	}

	private void InitGlobalRules()
	{
		foreach (KeyValuePair<string, JsValue> globalRule in _globalRules)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Dictionary<string, JsValue> dictionary2 = globalRule.Value.AsDictionary();
			string text = dictionary2["ID"].ToString();
			Dictionary<string, JsValue> dictionary3 = dictionary2["Attributes"].AsDictionary();
			foreach (KeyValuePair<string, JsValue> item in dictionary3)
			{
				string globalAttributeName = GetGlobalAttributeName(item.Key);
				dictionary.Add(globalAttributeName, item.Value.ToString());
			}
			string key = globalRule.Key;
			_rules.Add(text, Rule.RuleFabric(text, key, dictionary));
		}
	}

	private void InitGlobalAttributes()
	{
		foreach (KeyValuePair<string, JsValue> globalAttribute in _globalAttributes)
		{
			Dictionary<string, JsValue> dictionary = globalAttribute.Value.AsDictionary();
			string key = dictionary["ID"].ToString();
			_ruleAttributesMap.Add(key, globalAttribute.Key);
		}
	}

	public Rule GetGlobalRule(string ID)
	{
		if (_rules.ContainsKey(ID))
		{
			return _rules[ID];
		}
		Debug.LogError("Rule:" + ID + " Not Found");
		return null;
	}

	public string GetGlobalAttributeID(string name)
	{
		if (_globalAttributes.ContainsKey(name))
		{
			Dictionary<string, JsValue> dictionary = _globalAttributes[name].AsDictionary();
			return dictionary["ID"].ToString();
		}
		Debug.LogError("Rule Attribute:" + name + " Not Found");
		return string.Empty;
	}

	public string GetGlobalAttributeName(string ID)
	{
		if (_ruleAttributesMap.ContainsKey(ID))
		{
			return _ruleAttributesMap[ID];
		}
		Debug.LogError("Rule Attribute:" + ID + " Not Found");
		return string.Empty;
	}

	public List<RoundRule> ConvertYaml(Sequence rulesNode)
	{
		List<RoundRule> list = new List<RoundRule>();
		if (rulesNode != null)
		{
			foreach (Mapping item in rulesNode)
			{
				foreach (Mapping item2 in item)
				{
					RoundRule roundRule = new RoundRule();
					roundRule.RuleId = int.Parse(item2.key);
					foreach (Node item3 in item2.nodesInside)
					{
						RoundRuleAttribute roundRuleAttribute = new RoundRuleAttribute();
						roundRuleAttribute.AttrId = int.Parse(item3.key);
						roundRuleAttribute.AttrValue = item3.value.ToString();
						roundRule.Attrs.Add(roundRuleAttribute);
					}
					list.Add(roundRule);
				}
			}
		}
		return list;
	}
}
