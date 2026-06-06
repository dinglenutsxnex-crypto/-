using System;
using System.Collections.Generic;
using System.Text;
using Jint;
using Jint.Native;
using Jint.Native.Array;
using SF3.Items;
using SF3.UserData;
using UnityEngine;

public class JS : MonoBehaviour
{
	public class JSEnumsCompliancer
	{
		private Dictionary<string, Dictionary<int, string>> _intStringEnums = new Dictionary<string, Dictionary<int, string>>();

		private Dictionary<string, Dictionary<string, int>> _stringIntEnums = new Dictionary<string, Dictionary<string, int>>();

		public Dictionary<int, string> GetEnumDictionary(string enumName)
		{
			if (!_intStringEnums.ContainsKey(enumName))
			{
				Dictionary<string, JsValue> dictionary = Instance.GetScope(enumName).AsDictionary();
				_intStringEnums.Add(enumName, new Dictionary<int, string>());
				foreach (KeyValuePair<string, JsValue> item in dictionary)
				{
					_intStringEnums[enumName].Add(item.Value.AsInteger(), item.Key);
				}
			}
			return _intStringEnums[enumName];
		}

		public Dictionary<string, int> GetEnumDictionaryInverted(string enumName)
		{
			if (!_stringIntEnums.ContainsKey(enumName))
			{
				Dictionary<string, JsValue> dictionary = Instance.GetScope(enumName).AsDictionary();
				_stringIntEnums.Add(enumName, new Dictionary<string, int>());
				foreach (KeyValuePair<string, JsValue> item in dictionary)
				{
					_stringIntEnums[enumName].Add(item.Key, item.Value.AsInteger());
				}
			}
			return _stringIntEnums[enumName];
		}

		public string GetEnumeratorName(string jsEnumName, int enumeratorValue)
		{
			Dictionary<int, string> enumDictionary = GetEnumDictionary(jsEnumName);
			if (enumDictionary.ContainsKey(enumeratorValue))
			{
				return enumDictionary[enumeratorValue];
			}
			return null;
		}

		public int? GetEnumeratorValue(string jsEnumName, string enumeratorName)
		{
			Dictionary<string, int> enumDictionaryInverted = GetEnumDictionaryInverted(jsEnumName);
			if (enumDictionaryInverted.ContainsKey(enumeratorName))
			{
				return enumDictionaryInverted[enumeratorName];
			}
			return null;
		}

		public T GetEnumerator<T>(string jsEnumName, int enumeratorValue) where T : struct, IConvertible
		{
			Dictionary<int, string> enumDictionary = GetEnumDictionary(jsEnumName);
			if (!enumDictionary.ContainsKey(enumeratorValue))
			{
				throw new NullReferenceException();
			}
			if (Enum.IsDefined(typeof(T), enumDictionary[enumeratorValue]))
			{
				return (T)Enum.Parse(typeof(T), enumDictionary[enumeratorValue]);
			}
			return default(T);
		}

		public T GetEnumerator<T>(string jsEnumName, string enumeratorName) where T : struct, IConvertible
		{
			Dictionary<string, int> enumDictionaryInverted = GetEnumDictionaryInverted(jsEnumName);
			if (!enumDictionaryInverted.ContainsKey(enumeratorName))
			{
				throw new NullReferenceException();
			}
			if (Enum.IsDefined(typeof(T), enumeratorName))
			{
				return (T)Enum.Parse(typeof(T), enumeratorName);
			}
			return default(T);
		}

		public void Clear()
		{
			_intStringEnums.Clear();
			_stringIntEnums.Clear();
		}
	}

	private JSEnumsCompliancer _enumsCompliancer = new JSEnumsCompliancer();

	public Dictionary<int, Equipment> _equipmentDict;

	public Dictionary<int, Perk> _perksDict;

	private Dictionary<int, Booster> _boostersDict;

	private Dictionary<int, ShopBooster> _shopBoosters;

	private static JS _instance;

	private const string _jsScriptsRoot = "scripts/";

	public JSEnumsCompliancer EnumsCompliancer
	{
		get
		{
			return _enumsCompliancer;
		}
	}

	public Dictionary<int, Equipment> Equipment
	{
		get
		{
			if (_equipmentDict == null)
			{
				Dictionary<string, JsValue> dictionary = GetScope("ItemModelsMap").AsDictionary();
				_equipmentDict = new Dictionary<int, Equipment>();
				foreach (KeyValuePair<string, JsValue> item in dictionary)
				{
					Equipment equipment = new Equipment(new KeyValuePair<string, JsValue>(item.Key, item.Value));
					_equipmentDict.Add(equipment.id, equipment);
				}
			}
			return _equipmentDict;
		}
	}

	public Dictionary<int, Perk> Perks
	{
		get
		{
			if (_perksDict == null)
			{
				Dictionary<string, JsValue> dictionary = GetScope("PerkModelsMap").AsDictionary();
				_perksDict = new Dictionary<int, Perk>();
				foreach (KeyValuePair<string, JsValue> item in dictionary)
				{
					Perk perk = new Perk(new KeyValuePair<string, JsValue>(item.Key, item.Value));
					_perksDict.Add(perk.id, perk);
				}
			}
			return _perksDict;
		}
	}

	public Dictionary<int, Booster> Boosters
	{
		get
		{
			if (_boostersDict == null)
			{
				Dictionary<string, JsValue> dictionary = GetScope("BoosterModelsMap").AsDictionary();
				_boostersDict = new Dictionary<int, Booster>();
				foreach (KeyValuePair<string, JsValue> item in dictionary)
				{
					Booster booster = new Booster(new KeyValuePair<string, JsValue>(item.Key, item.Value));
					booster.SetModel(item.Value.AsDictionary()["Name"].AsString());
					_boostersDict.Add(booster.id, booster);
				}
			}
			return _boostersDict;
		}
	}

	public Dictionary<int, ShopBooster> ShopBoosters
	{
		get
		{
			if (_shopBoosters == null)
			{
				ArrayInstance arrayInstance = GetScope("ShopBoosterModels").AsArray();
				_shopBoosters = new Dictionary<int, ShopBooster>();
				for (int i = 0; i < arrayInstance.Length; i++)
				{
					int num = arrayInstance[i].AsDictionary()["ID"].AsInteger();
					Dictionary<string, JsValue> dictionary = arrayInstance[i].AsDictionary()["Booster"].AsDictionary();
					string alias = string.Empty;
					foreach (KeyValuePair<string, JsValue> item in dictionary)
					{
						if (item.Key == "Name")
						{
							alias = item.Value.AsString();
						}
					}
					Dictionary<string, JsValue> currencies = arrayInstance[i].AsDictionary()["Price"].AsDictionary();
					ShopBooster value = new ShopBooster(num, alias, currencies);
					_shopBoosters.Add(num, value);
				}
			}
			return _shopBoosters;
		}
	}

	public static string FolderPath
	{
		get
		{
			return (GlobalPath.ExternalPath + "/JS").Replace("\\", "/").Replace("//", "/");
		}
	}

	public static JS Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = UnityEngine.Object.FindObjectOfType<JS>();
				if (!_instance)
				{
					_instance = new GameObject("JS").AddComponent<JS>();
					StaticObjectsManager.AddObject(_instance.gameObject);
				}
			}
			return _instance;
		}
	}

	public Engine engine { get; private set; }

	public Booster GetBoosterByID(int id)
	{
		if (Boosters.ContainsKey(id))
		{
			return Boosters[id];
		}
		return null;
	}

	public void InitializeGameScripts(Action onFinish)
	{
		Dictionary<string, string> map = NetworkConfigManager.UnzipToStrings();
		Engine engine = CreateScript(map, "main.js");
		ArrayInstance arrayInstance = engine.Global.Get("main").AsArray();
		List<string> list = new List<string>();
		for (int i = 0; i < arrayInstance.Length; i++)
		{
			string item = arrayInstance.Get(i.ToString()).AsString();
			list.Add(item);
		}
		this.engine = CreateScript(map, list.ToArray());
		onFinish.InvokeSafe();
	}

	private Engine CreateScript(Dictionary<string, string> map, params string[] scriptNames)
	{
		string text = UserManager.GetABTag() + "/";
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string text2 in scriptNames)
		{
			string value;
			if (map.TryGetValue("scripts/" + text + text2, out value))
			{
				stringBuilder.Append(value + "\n");
			}
			else if (map.TryGetValue("scripts/" + text2, out value))
			{
				stringBuilder.Append(value + "\n");
			}
		}
		if (string.IsNullOrEmpty(stringBuilder.ToString()))
		{
			stringBuilder = new StringBuilder();
			foreach (string text3 in scriptNames)
			{
				stringBuilder.Append("[" + text3 + "]\n");
			}
			Debug.LogError("JS script creation failed. Result script is empty. Parameters for script : " + stringBuilder.ToString());
			return null;
		}
		return new Engine().Execute(stringBuilder.ToString());
	}

	public int GetIntegerConstant(string name)
	{
		return engine.Global.Get(name).AsInteger();
	}

	public double GetNumberConstant(string name)
	{
		return engine.Global.Get(name).AsNumber();
	}

	public string GetStringConstant(string name)
	{
		return engine.Global.Get(name).AsString();
	}

	public bool GetBoolConstant(string name)
	{
		JsValue jsValue = engine.Global.Get(name);
		return jsValue != JsValue.Undefined && jsValue.AsBoolean();
	}

	public JsValue GetScope(string scope)
	{
		return engine.Global.Get(scope);
	}

	public static JsValue CallFunction(string functionName, params object[] content)
	{
		List<JsValue> list = new List<JsValue>();
		List<string> list2 = new List<string>();
		if (content != null && content.Length > 0)
		{
			for (int i = 0; i < content.Length; i++)
			{
				string text = string.Format("valueFor_{0}_{1}", functionName, i);
				list2.Add(text);
				object obj = content[i];
				Instance.engine.SetValue(text, obj);
				list.Add(Instance.engine.Global.Get(text));
			}
		}
		JsValue result = Instance.Call(functionName, list.ToArray());
		foreach (string item in list2)
		{
			Instance.engine.Global.RemoveOwnProperty(item);
		}
		return result;
	}

	private JsValue Call(string functionName, params JsValue[] args)
	{
		try
		{
			JsValue value = engine.GetValue(functionName);
			JsValue jsValue = value.Invoke(args);
			if (jsValue == null)
			{
				Debug.LogError("Result of function [" + functionName + "] is null. Maybe function name is changed.");
			}
			return jsValue;
		}
		catch
		{
			Debug.LogError("JS Exception when calling: " + functionName + "\nException details follow in the re-thrown exception");
			throw;
		}
	}
}
