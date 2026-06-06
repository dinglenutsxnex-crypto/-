using System;
using System.Collections.Generic;
using System.Globalization;
using Jint.Native;
using Jint.Native.Array;
using Jint.Native.Object;
using Nekki.Yaml;
using SF3.UserData;
using SimpleJSON;
using UnityEngine;
using sf3DTO;

namespace SF3.Items
{
	public class Perk : BattleItem, IPerk, ISlotItem, IFactionable, IRarable, IStackable, IDescribed, ICloneable
	{
		private const string ALIAS_KEY = "Alias";

		private const string ID_KEY = "ID";

		private const string STACK_LEVEL_KEY = "StackLevel";

		private PerkType _perkType;

		private EquipmentType _targetItemType;

		private Faction _targetFaction;

		private readonly Rarity _rarity;

		private string _targetTag;

		private string _description;

		private readonly string _name;

		private double _stackLevel;

		private Dictionary<string, float> _numericalVariables;

		private Dictionary<string, string> _stringVariables;

		public double stackLevel
		{
			get
			{
				return _stackLevel;
			}
		}

		public Perk(KeyValuePair<string, JsValue> keyValuePair)
			: base(keyValuePair)
		{
			Init();
			Dictionary<string, JsValue> dic = keyValuePair.Value.AsDictionary();
			_targetTag = GetNodeSafe(dic, "TargetTag", null);
			_name = GetNodeSafe(dic, "Name", "error_no_name");
			TryParseEnum(out _targetItemType, GetNodeSafe(dic, "TargetItemType", "NONE"), EquipmentType.None);
			TryParseEnum(out _targetFaction, GetNodeSafe(dic, "TargetFaction", "NONE"), Faction.UnknownFaction);
			TryParseEnum(out _rarity, GetNodeSafe(dic, "Rarity", "COMMON"), Rarity.Common);
			TryParseEnum(out _perkType, GetNodeSafe(dic, "PerkType", "MOVE"), PerkType.Move);
			UpdateStackLevelDependent();
		}

		private void Init()
		{
			_numericalVariables = new Dictionary<string, float>();
			_stringVariables = new Dictionary<string, string>();
			_targetTag = null;
			_targetFaction = Faction.UnknownFaction;
		}

		public static Perk Create(Mapping perkMapp)
		{
			int perkID = int.Parse(perkMapp.GetText("ID").text);
			Perk perk = Create(perkID);
			Scalar text = perkMapp.GetText("StackLevel");
			if (text != null)
			{
				perk.SetStackLevel(double.Parse(text.text));
			}
			return perk;
		}

		public static Perk Create(JSONClass perkClass)
		{
			int asInt = perkClass["ID"].AsInt;
			Perk perk = Create(asInt);
			if (perkClass.ContainsKey("StackLevel"))
			{
				perk.SetStackLevel(perkClass["StackLevel"].AsDouble);
			}
			return perk;
		}

		public static Perk Create(int perkID)
		{
			Perk perk = null;
			ItemsManager.TryGetPerkById(perkID, out perk);
			return perk;
		}

		public static Perk Create(sf3DTO.Perk perkData)
		{
			Perk perk = Create(perkData.ModelId);
			perk.SetStackLevel(perkData.StackLevel);
			return perk;
		}

		public static List<BaseItem> Create(List<sf3DTO.Perk> perksData)
		{
			List<BaseItem> list = new List<BaseItem>();
			foreach (sf3DTO.Perk perksDatum in perksData)
			{
				list.Add(Create(perksDatum));
			}
			return list;
		}

		public override List<Node> ToYaml()
		{
			List<Node> list = new List<Node>();
			list.Add(new Scalar("ID", base.id.ToString()));
			list.Add(new Scalar("StackLevel", _stackLevel.ToString(CultureInfo.InvariantCulture)));
			return list;
		}

		public override JSONClass ToJSON()
		{
			JSONClass jSONClass = new JSONClass();
			jSONClass.Add("ID", new JSONData(base.id));
			jSONClass.Add("StackLevel", new JSONData(stackLevel));
			return jSONClass;
		}

		public string GetImage()
		{
			return base.Image;
		}

		public int GetId()
		{
			return base.id;
		}

		public string GetAlias()
		{
			return base.alias;
		}

		public EquipmentType GetTargetItemType()
		{
			return _targetItemType;
		}

		public Faction GetTargetFactionType()
		{
			return _targetFaction;
		}

		public Faction GetFactionType()
		{
			return _targetFaction;
		}

		public Rarity GetRarityType()
		{
			return _rarity;
		}

		public double GetStackLevel()
		{
			return _stackLevel;
		}

		public void SetStackLevel(double value)
		{
			_stackLevel = value;
			UpdateStackLevelDependent();
		}

		public void MergeSimilarItems(IStackable toMerge)
		{
			Perk perk = toMerge as Perk;
			if (perk == null)
			{
				Debug.LogError("Something wrong.. This is not instance of Perk. ");
				return;
			}
			double num = JsFunction.MergeStackLevelsPerks(stackLevel, perk.stackLevel, (int)_rarity, UserManager.UserModelInfo.level);
			SetStackLevel(num);
		}

		public int GetLevelUpCount(IStackable newItem)
		{
			return JsFunction.GetLevelUpCountPerks(stackLevel, newItem.GetStackLevel(), (int)_rarity);
		}

		public float GetBarValue()
		{
			return JsFunction.GetBarPerks(stackLevel, (int)_rarity);
		}

		public float GetLevelUpBar(IStackable newItem)
		{
			return JsFunction.GetLevelUpBarPerks(stackLevel, newItem.GetStackLevel(), (int)_rarity);
		}

		public string GetDescription()
		{
			return _description;
		}

		private void UpdateStackLevelDependent()
		{
			ObjectInstance perkLeveling = JsFunction.GetPerkLeveling(base.id, _stackLevel);
			if (perkLeveling.HasOwnProperty("Tags"))
			{
				ArrayInstance arrayInstance = perkLeveling["Tags"].AsArray();
				for (int i = 0; i < arrayInstance.Length; i++)
				{
					AddTag(arrayInstance[i].AsString());
				}
			}
			if (perkLeveling.HasProperty("Attributes"))
			{
				Dictionary<string, JsValue> dictionary = perkLeveling["Attributes"].AsDictionary();
				foreach (KeyValuePair<string, JsValue> item in dictionary)
				{
					string text = item.Value.ToString();
					float result;
					if (float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
					{
						_numericalVariables[item.Key] = result;
					}
					else
					{
						_stringVariables[item.Key] = text;
					}
				}
			}
			if (perkLeveling.HasProperty("Alias"))
			{
				base.alias = GetNodeSafe(perkLeveling.AsDictionary(), "Alias", "error_no_alias");
			}
			if (perkLeveling.HasProperty("Description"))
			{
				_description = GetNodeSafe(perkLeveling.AsDictionary(), "Description", "error_no_description");
			}
		}

		public object GetAttributeValue(string varName)
		{
			return (!_numericalVariables.ContainsKey(varName)) ? _stringVariables[varName] : ((object)_numericalVariables[varName]);
		}

		public override object Clone()
		{
			Perk perk = (Perk)MemberwiseClone();
			perk.SetTags(GetTags().GetRange(0, GetTags().Count));
			perk._numericalVariables = new Dictionary<string, float>();
			foreach (KeyValuePair<string, float> numericalVariable in _numericalVariables)
			{
				perk._numericalVariables.Add(numericalVariable.Key, numericalVariable.Value);
			}
			perk._stringVariables = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> stringVariable in _stringVariables)
			{
				perk._stringVariables.Add(stringVariable.Key, stringVariable.Value);
			}
			return perk;
		}

		public override string ToString()
		{
			StringWrapper stringWrapper = StringWrapper.Create();
			stringWrapper.Add(base.ToString());
			stringWrapper.Head("Perk data");
			stringWrapper.Wrap("PerkType", _perkType.ToString());
			stringWrapper.Wrap("StackLevel", _stackLevel);
			stringWrapper.Wrap("EquipmentType", _targetItemType.ToString());
			stringWrapper.Wrap("FactionType", _targetFaction.ToString());
			stringWrapper.Wrap("RarityType", _rarity.ToString());
			stringWrapper.Wrap("TargetTag", _targetTag);
			stringWrapper.Wrap("Description", _description);
			stringWrapper.Wrap("Name", _name);
			foreach (KeyValuePair<string, float> numericalVariable in _numericalVariables)
			{
				stringWrapper.Wrap("NumericalVariable", "Key: " + numericalVariable.Key + ", Value: " + numericalVariable.Value);
			}
			foreach (KeyValuePair<string, string> stringVariable in _stringVariables)
			{
				stringWrapper.Wrap("StringVariable", "Key: " + stringVariable.Key + ", Value: " + stringVariable.Value);
			}
			stringWrapper.Separator();
			return stringWrapper.ToString();
		}

		public PerkType GetPerkType()
		{
			return _perkType;
		}

		public string GetTargetTag()
		{
			return _targetTag;
		}

		public string GetName()
		{
			return _name;
		}
	}
}
