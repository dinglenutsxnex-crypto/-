using System.Collections.Generic;
using Nekki;
using UnityEngine;

namespace SF3
{
	public class UIModulesController
	{
		public enum ModulesType
		{
			BASE = 0,
			LOCAL = 1,
			CONTROL = 2
		}

		public enum EUIElements
		{
			DEBUG = 0,
			FIGHT_BUTTONS = 1,
			JOYSTICK = 2,
			FOE_HUD = 3,
			PLAYER_HUD = 4,
			PAUSE_WINDOW = 5,
			PAUSE_BUTTON = 6,
			ROUND_TIMER = 7,
			ROUNDS_UI = 8,
			MODEL_DEBUG = 9,
			MODEL_DEBUG_ENEMY = 10,
			DEBUG_HIDER = 11,
			GAME_TIME_DEBUG = 12,
			DEBUG_MENU = 13,
			CURRENCY = 14,
			CONSOLE = 15,
			HOME_MENU = 16,
			SHADOW_PERKS = 17,
			UIBLOCKER = 18
		}

		private static readonly Dictionary<EUIElements, string> _uiModulesNamesCompliance;

		private static readonly EUIElements[] _fightUIModules;

		private static readonly EUIElements[] _dojoUIModules;

		private static readonly EUIElements[] _controlUIModules;

		private static UIModulesController _instance;

		private List<NekkiUIModule> _mountedBase;

		private List<NekkiUIModule> _mountedLocal;

		private List<NekkiUIModule> _mountedControl;

		public static UIModulesController Instance
		{
			get
			{
				return _instance;
			}
		}

		static UIModulesController()
		{
			_uiModulesNamesCompliance = new Dictionary<EUIElements, string>
			{
				{
					EUIElements.FOE_HUD,
					"FoeHUD"
				},
				{
					EUIElements.PLAYER_HUD,
					"PlayerHUD"
				},
				{
					EUIElements.PAUSE_WINDOW,
					"PauseWindow"
				},
				{
					EUIElements.PAUSE_BUTTON,
					"PauseButton"
				},
				{
					EUIElements.ROUND_TIMER,
					"RoundTimer"
				},
				{
					EUIElements.ROUNDS_UI,
					"RoundsUI"
				},
				{
					EUIElements.CONSOLE,
					"Console"
				},
				{
					EUIElements.HOME_MENU,
					"HomeMenu"
				},
				{
					EUIElements.MODEL_DEBUG,
					"ModelDebug"
				},
				{
					EUIElements.MODEL_DEBUG_ENEMY,
					"ModelDebugEnemy"
				},
				{
					EUIElements.DEBUG_HIDER,
					"DebugHider"
				},
				{
					EUIElements.GAME_TIME_DEBUG,
					"GameTimeDebug"
				},
				{
					EUIElements.DEBUG_MENU,
					"DebugMenu"
				},
				{
					EUIElements.CURRENCY,
					"Currency"
				},
				{
					EUIElements.DEBUG,
					"Debug"
				},
				{
					EUIElements.FIGHT_BUTTONS,
					"FightButtons"
				},
				{
					EUIElements.JOYSTICK,
					"Joystick"
				},
				{
					EUIElements.SHADOW_PERKS,
					"ShadowPerks"
				},
				{
					EUIElements.UIBLOCKER,
					"UIBlocker"
				}
			};
			_fightUIModules = new EUIElements[7]
			{
				EUIElements.FOE_HUD,
				EUIElements.PLAYER_HUD,
				EUIElements.PAUSE_WINDOW,
				EUIElements.PAUSE_BUTTON,
				EUIElements.ROUND_TIMER,
				EUIElements.ROUNDS_UI,
				EUIElements.SHADOW_PERKS
			};
			_dojoUIModules = new EUIElements[2]
			{
				EUIElements.CURRENCY,
				EUIElements.HOME_MENU
			};
			_controlUIModules = new EUIElements[2]
			{
				EUIElements.JOYSTICK,
				EUIElements.FIGHT_BUTTONS
			};
		}

		public UIModulesController()
		{
			_instance = this;
			_mountedBase = new List<NekkiUIModule>();
			if (NekkiUtils.IsDebug)
			{
				NekkiUIModule nekkiUIModule = NekkiUIRootModules.Instance.MountNGUIModule("Debug");
				_mountedBase.Add(nekkiUIModule);
				nekkiUIModule.Elements.GetButton("reset").onClick.Add(new EventDelegate(NekkiUtils.ClearAllApplication));
				_mountedBase.Add(NekkiUIRootModules.Instance.MountNGUIModule("DebugMenu"));
				_mountedBase.Add(NekkiUIRootModules.Instance.MountNGUIModule("Console"));
			}
			float scaleFactor = QualityManager.Instance.ScaleFactor;
			_mountedControl = MountModules(_controlUIModules, new Vector3(scaleFactor, scaleFactor, scaleFactor));
		}

		private bool IsNative(EUIElements module)
		{
			switch (module)
			{
			case EUIElements.DEBUG:
				return false;
			case EUIElements.FIGHT_BUTTONS:
				return false;
			case EUIElements.JOYSTICK:
				return false;
			case EUIElements.FOE_HUD:
				return false;
			case EUIElements.PLAYER_HUD:
				return false;
			case EUIElements.MODEL_DEBUG:
				return false;
			case EUIElements.MODEL_DEBUG_ENEMY:
				return false;
			case EUIElements.DEBUG_HIDER:
				return false;
			case EUIElements.GAME_TIME_DEBUG:
				return false;
			case EUIElements.DEBUG_MENU:
				return false;
			case EUIElements.CONSOLE:
				return false;
			case EUIElements.PAUSE_WINDOW:
				return true;
			case EUIElements.PAUSE_BUTTON:
				return true;
			case EUIElements.ROUND_TIMER:
				return true;
			case EUIElements.ROUNDS_UI:
				return true;
			case EUIElements.CURRENCY:
				return true;
			case EUIElements.HOME_MENU:
				return true;
			case EUIElements.SHADOW_PERKS:
				return true;
			case EUIElements.UIBLOCKER:
				return true;
			default:
				return false;
			}
		}

		public void InitializeModules(BattleType locationType)
		{
			EnableModules(true, default(ModulesType));
			if (_mountedLocal != null)
			{
				foreach (NekkiUIModule item in _mountedLocal)
				{
					NekkiUIRootModules.Instance.UnmountModule(item);
				}
				_mountedLocal.Clear();
			}
			switch (locationType)
			{
			case BattleType.Fight:
				_mountedLocal = MountModules(_fightUIModules);
				break;
			case BattleType.Dojo:
				_mountedLocal = MountModules(_dojoUIModules);
				break;
			}
		}

		private List<NekkiUIModule> MountModules(EUIElements[] newModules, Vector3? scaleVector = null)
		{
			List<NekkiUIModule> list = new List<NekkiUIModule>();
			foreach (EUIElements eUIElements in newModules)
			{
				NekkiUIModule nekkiUIModule = ((!IsNative(eUIElements)) ? NekkiUIRootModules.Instance.MountNGUIModule(_uiModulesNamesCompliance[eUIElements]) : NekkiUIRootModules.Instance.MountNativeModule(_uiModulesNamesCompliance[eUIElements]));
				list.Add(nekkiUIModule);
				UIModuleHolder component = nekkiUIModule.GetComponent<UIModuleHolder>();
				component.Initialize();
				if (scaleVector.HasValue)
				{
					nekkiUIModule.transform.localScale = scaleVector.Value;
				}
			}
			return list;
		}

		public void EnableModules(bool enable, params ModulesType[] types)
		{
			for (int i = 0; i < types.Length; i++)
			{
				switch (types[i])
				{
				case ModulesType.BASE:
					EnableModulesInList(enable, _mountedBase);
					break;
				case ModulesType.LOCAL:
					EnableModulesInList(enable, _mountedLocal);
					break;
				case ModulesType.CONTROL:
					EnableModulesInList(enable, _mountedControl);
					break;
				}
			}
		}

		public void EnableAllModules(bool enable)
		{
			EnableModulesInList(enable, _mountedBase);
			EnableModulesInList(enable, _mountedLocal);
			EnableModulesInList(enable, _mountedControl);
		}

		private void EnableModulesInList(bool enable, List<NekkiUIModule> list)
		{
			if (list == null)
			{
				return;
			}
			foreach (NekkiUIModule item in list)
			{
				if (enable)
				{
					NekkiUIRootModules.Instance.EnableModule(item);
					continue;
				}
				NekkiUIRootModules.Instance.DisableModules(item.ModuleName);
			}
		}

		public NekkiUIModule GetMountedModule(EUIElements module)
		{
			string value = _uiModulesNamesCompliance[module];
			foreach (NekkiUIModule item in _mountedBase)
			{
				if (item.ModuleName.Equals(value))
				{
					return item;
				}
			}
			foreach (NekkiUIModule item2 in _mountedLocal)
			{
				if (item2.ModuleName.Equals(value))
				{
					return item2;
				}
			}
			return null;
		}
	}
}
