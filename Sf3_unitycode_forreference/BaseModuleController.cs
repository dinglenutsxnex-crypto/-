using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SF3;
using SF3.Moves;
using SF3.UserData;
using SimpleJSON;

public class BaseModuleController
{
	private IntentModule _defaultIntent;

	private HolderModule _currentModule;

	private IntentModule _currentIntent;

	private static BaseModuleController _instance;

	private Dictionary<ConstantsSF3.ELocationSceneModule, HolderModule> _registredModules;

	private Dictionary<ConstantsSF3.ELocationSceneModule, Type> _intents;

	public static IntentModule DefaultIntent
	{
		get
		{
			return Instance._defaultIntent;
		}
	}

	public static HolderModule CurrentModule
	{
		get
		{
			return Instance._currentModule;
		}
	}

	private static BaseModuleController Instance
	{
		get
		{
			return _instance ?? (_instance = new BaseModuleController());
		}
	}

	public static ConstantsSF3.ELocationSceneModule CurrentType
	{
		get
		{
			return (CurrentModule != null) ? CurrentModule.ModuleType : ConstantsSF3.ELocationSceneModule.None;
		}
	}

	public static string CurrentName
	{
		get
		{
			return CurrentType.ToString();
		}
	}

	public static void Clear()
	{
		_instance = null;
	}

	public static void Init()
	{
		Instance.Initialize();
	}

	private void Initialize()
	{
		_registredModules = new Dictionary<ConstantsSF3.ELocationSceneModule, HolderModule>();
		_intents = new Dictionary<ConstantsSF3.ELocationSceneModule, Type>();
		Register(ConstantsSF3.ELocationSceneModule.DojoInterface, typeof(DojoHolderModule), typeof(FightIntentModule));
		Register(ConstantsSF3.ELocationSceneModule.Fight, typeof(FightHolderModule), typeof(FightIntentModule));
		Register(ConstantsSF3.ELocationSceneModule.Shop, typeof(ShopHolderModule), typeof(ShopIntentModule));
		Register(ConstantsSF3.ELocationSceneModule.Inventory, typeof(InventoryHolderModule), typeof(InventoryIntentModule));
		Register(ConstantsSF3.ELocationSceneModule.Map, typeof(MapHolderModule), typeof(MapIntentModule));
		Register(ConstantsSF3.ELocationSceneModule.BoosterpacksScreen, typeof(BoosterHolderModule), typeof(BoosterIntentModule));
		ParseDefaultIntent();
	}

	private void Register(ConstantsSF3.ELocationSceneModule type, Type holder, Type intent)
	{
		HolderModule holderModule = (HolderModule)Activator.CreateInstance(holder);
		holderModule.Init(type, OnOpenModule, OnCloseModule);
		_registredModules.Add(type, holderModule);
		_intents.Add(type, intent);
	}

	private HolderModule GetModule(ConstantsSF3.ELocationSceneModule type)
	{
		return (!_registredModules.ContainsKey(type)) ? null : _registredModules[type];
	}

	public static IntentModule GoToModule(ConstantsSF3.ELocationSceneModule type, params object[] args)
	{
		return GoToModule(type, null, args);
	}

	public static IntentModule GoToModule(ConstantsSF3.ELocationSceneModule type, Action callbackOpenModule, params object[] args)
	{
		IntentModule intent = CreateIntent(type, args);
		return GoToModule(intent, callbackOpenModule);
	}

	public static IntentModule GoToModule(IntentModule intent, Action callbackOpenModule = null)
	{
		return Instance.RunModule(intent, callbackOpenModule);
	}

	private IntentModule RunModule(IntentModule intent, Action callbackOpenModule)
	{
		Analytics.current.OnModuleChange(intent);
		SendChangeScreenLog(intent);
		intent.CallbackOpenModule = callbackOpenModule;
		if (IsInterruptModule(intent))
		{
			intent.IsInterrupted = true;
			intent.RunCallbackOpenModule();
			return intent;
		}
		_currentIntent = intent;
		CloseCurrent(intent);
		return intent;
	}

	public static IntentModule CreateIntent(ConstantsSF3.ELocationSceneModule type, params object[] args)
	{
		if (IsIntent(type))
		{
			IntentModule intentModule = (IntentModule)Activator.CreateInstance(Instance._intents[type]);
			intentModule.Init(type, args);
			intentModule.CreateTransitionData(Instance._currentIntent);
			return intentModule;
		}
		return null;
	}

	public static IntentModule GoToDefault(Action callbackOpenModule = null)
	{
		return GoToModule(Instance._defaultIntent, callbackOpenModule);
	}

	private void ParseDefaultIntent()
	{
		object intentModule = UserManager.GetIntentModule();
		if (intentModule != null)
		{
			IntentParametrs intentParametrs = null;
			intentParametrs = new IntentParametrs(intentModule as JSONClass);
			_defaultIntent = CreateIntent(intentParametrs.ModuleType, intentParametrs);
		}
		else
		{
			_defaultIntent = CreateIntent(ConstantsSF3.ELocationSceneModule.DojoInterface);
		}
	}

	private bool IsInterruptModule(IntentModule intent)
	{
		bool flag = IsEqualToCurrent(intent) && _currentModule.ModuleType != ConstantsSF3.ELocationSceneModule.Fight;
		bool flag2 = _currentModule != null && _currentModule.IsCanOpen();
		bool flag3 = IsPreSceneChange(intent);
		return flag || flag2 || flag3;
	}

	public static bool IsEqualToCurrent(IntentModule intent)
	{
		return Instance._currentIntent != null && Instance._currentIntent.Equal(intent);
	}

	private bool IsPreSceneChange(IntentModule intent)
	{
		ETriggerEvents e = ETriggerEvents.QEVENT_PRE_SCENE_CHANGE;
		if (intent.TypeModule == ConstantsSF3.ELocationSceneModule.Fight)
		{
			e = ETriggerEvents.QEVENT_FIGHT_ENTER;
		}
		return QuestController.Instance.ThrowEvent(e, intent.TransitionData);
	}

	private void OnOpenModule(HolderModule module)
	{
		_currentModule = module;
		UIBlocker.Instance.Unblock();
		module.CallIntentOpen();
		BattleController.ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_MODULE_ENTER, -1, module.Intent));
		QuestController.Instance.ThrowEvent(ETriggerEvents.QEVENT_POST_SCENE_CHANGE, module.Intent.TransitionData);
	}

	private void OnCloseModule(HolderModule module)
	{
		Open();
	}

	private void Open()
	{
		HolderModule registerModule = GetRegisterModule(_currentIntent.TypeModule);
		if (registerModule != null)
		{
			if (CurrentModule != null)
			{
				BattleController.ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_MODULE_PRE_ENTER, -1, _currentIntent));
			}
			registerModule.Open(_currentIntent);
		}
	}

	private void CloseCurrent(IntentModule intent)
	{
		UIBlocker.Instance.Block();
		if (!intent.IsSkip)
		{
			if (_currentModule != null)
			{
				_currentModule.Close(intent.TypeModule);
			}
			else
			{
				Open();
			}
			return;
		}
		HolderModule registerModule = GetRegisterModule(_currentIntent.TypeModule);
		if (registerModule != null)
		{
			registerModule.SetIntent(intent);
			OnOpenModule(registerModule);
		}
	}

	private HolderModule GetRegisterModule(ConstantsSF3.ELocationSceneModule type)
	{
		return Instance.GetModule(type);
	}

	private static bool IsIntent(ConstantsSF3.ELocationSceneModule type)
	{
		if (Instance._intents.ContainsKey(type))
		{
			return true;
		}
		throw new Exception("Intent Not Found: " + type);
	}

	private void SendChangeScreenLog(IntentModule intent)
	{
		JObject jObject = new JObject();
		jObject["srcScreen"] = intent.TransitionData.FromData.Name;
		jObject["dstScreen"] = intent.TransitionData.ToData.Name;
		if (intent.TransitionData.ToData.Name == ConstantsSF3.ELocationSceneModule.Fight.ToString())
		{
			jObject["battleId"] = intent.TransitionData.ToData.BattleID;
		}
		Analytics.Logger.AddEvent("SCREEN_CHANGED", jObject);
	}
}
