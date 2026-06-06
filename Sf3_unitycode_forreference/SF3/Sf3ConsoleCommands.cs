using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.WellKnownTypes;
using Jint.Native;
using Nekki;
using Network.core.events;
using SF3.Effects;
using SF3.GameModels;
using SF3.Moves;
using SF3.UserData;
using SF3_Attributes;
using UnityEngine;
using sf3DTO;

namespace SF3
{
	public class Sf3ConsoleCommands
	{
		private delegate bool ParamsPars<T>(string source, out T result);

		public static readonly string MsgArgParsingError;

		private static int defaultWidth;

		private static int defaultHeight;

		public static bool isActive { get; private set; }

		public static bool unityConsoleModeActive { get; private set; }

		static Sf3ConsoleCommands()
		{
			MsgArgParsingError = "Arguments parsing error";
			Application.logMessageReceived += ApplicationLogSubsctiption;
			unityConsoleModeActive = false;
		}

		private static void ApplicationLogSubsctiption(string condition, string stacktrace, LogType type)
		{
			if (unityConsoleModeActive)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("------------------------\n");
				switch (type)
				{
				case LogType.Log:
					stringBuilder.Append("UNITY_LOG: ");
					break;
				case LogType.Warning:
					stringBuilder.Append("UNITY_WARNING: ");
					break;
				case LogType.Error:
					stringBuilder.Append("UNITY_ERROR: ");
					break;
				case LogType.Assert:
					stringBuilder.Append("UNITY_ASSERT: ");
					break;
				case LogType.Exception:
					stringBuilder.Append("UNITY_EXCEPTION: ");
					break;
				}
				stringBuilder.AppendFormat("\n{0} \n {1}\n", condition, stacktrace);
				stringBuilder.Append("------------------------\n");
				NekkiConsolePanel.Log(stringBuilder.ToString());
			}
		}

		private static bool ParseCommandWithParams<T>(string command, string key, ParamsPars<T> paramsParser, ref T result)
		{
			string[] array = command.Split('[', ']');
			if (array.Length >= 2 && array[0].Trim() == key && paramsParser(array[1], out result))
			{
				return true;
			}
			return false;
		}

		public static string SafeCommandExecution(Func<string> coommandFunc)
		{
			try
			{
				return coommandFunc();
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}

		public static void AddCommands()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			defaultWidth = Screen.width;
			defaultHeight = Screen.height;
			NekkiConsole.RegisterCommand("setqualitypreset", SetPreset, "setqualitypreset - set quiality preset");
			NekkiConsole.RegisterCommand("qualitypreset", GetPreset, "qualitypreset - get current quiality preset");
			NekkiConsole.RegisterCommand("qualityavailable", GetAvailablePreset, "qualityavailable - get Availables quiality presets");
			NekkiConsole.RegisterCommand("booster", GetBooster, "booster [ID, count] - getBooster");
			NekkiConsole.RegisterCommand("allitems", GetAllItems, "allitems [double] - give ALL items to the user with specified stack level");
			NekkiConsole.RegisterCommand("apltrg", ApplyTriggerActions, "apltrg [me/enemy] [trigger_name] - apply trigger by name. To player me by default");
			NekkiConsole.RegisterCommand("block", BlockInput, "block [int] - blocks the input for seconds (default: 3)");
			NekkiConsole.RegisterCommand("blur", SetBlurIterations, string.Empty);
			NekkiConsole.RegisterCommand("bonew", SetBoneWeight, string.Empty);
			NekkiConsole.RegisterCommand("bonus", AddBonuses, "bonus [int] - add bonuses");
			NekkiConsole.RegisterCommand("capsules", Capsules, "capsules [bool] - show | hide capsules(def:false)");
			NekkiConsole.RegisterCommand("clear", Clear, "clear - clear console");
			NekkiConsole.RegisterCommand("clnusr", CleanUsers, "clnusr - clean users");
			NekkiConsole.RegisterCommand("cloth", ClothEnable, "cloth [bool] - enable | disable cloth(def:true)");
			NekkiConsole.RegisterCommand("clrbund", ClearBundles, string.Empty);
			NekkiConsole.RegisterCommand("clrusr", ClearUser, "clrusr - clear user config");
			NekkiConsole.RegisterCommand("coins", AddCoins, "coins [int] - add money");
			NekkiConsole.RegisterCommand("ctrl", SetControll, string.Empty);
			NekkiConsole.RegisterCommand("dc", TestDisconnect, "dc - disconnect");
			NekkiConsole.RegisterCommand("eai", SwitchAiModeEnemy, "eai [bool] - enable | disable enemy ai");
			NekkiConsole.RegisterCommand("ee", EnableEnemy, "ee [bool] - enable | disable enemy");
			NekkiConsole.RegisterCommand("elog", EmailLog, "elog [email] -send current logs to email");
			NekkiConsole.RegisterCommand("exp", AddExperience, "exp [int] - add experience or get exp if no params");
			NekkiConsole.RegisterCommand("fake_ui_event", FakeUIEvent, "fake_ui_event [string?] where string is a value that is sent");
			NekkiConsole.RegisterCommand("fcrt", CriticalChanceFactor, "fcrt [me/enemy] [float] - set player factor of critical chance");
			NekkiConsole.RegisterCommand("fdmg", DamageFactor, "fdmg [me/enemy] [float] - set player factor of inflicted damage");
			NekkiConsole.RegisterCommand("fog", ToggleFog, string.Empty);
			NekkiConsole.RegisterCommand("fss", FrameSkipShift, string.Empty);
			NekkiConsole.RegisterCommand("gdm", GetDeviceModel, string.Empty);
			NekkiConsole.RegisterCommand("gender", SetGender, "gender - switch current player gender\ngender [MALE|FEMALE] - set current player gender");
			NekkiConsole.RegisterCommand("ggvar", GetGlobalVariable, "ggvar [string] - get global variable with name");
			NekkiConsole.RegisterCommand("glow", SwitchGlow, "glow [bool] -mk glow switch");
			NekkiConsole.RegisterCommand("help", Help, "help - show all commands");
			NekkiConsole.RegisterCommand("item", GetEquip, "item [ID, stackLevel] - getItem");
			NekkiConsole.RegisterCommand("lbfs", LoadBattleFromServer, "lbfs [int] - get battle from server by id");
			NekkiConsole.RegisterCommand("lvl", SetLevel, "lvl [int] - set new user lvl");
			NekkiConsole.RegisterCommand("maxup", GetMaxUp, "maxup - max money and exp ");
			NekkiConsole.RegisterCommand("mfloat", Mfloat, "mfloat - float to module(mfloat help - details)");
			NekkiConsole.RegisterCommand("mjump", Mjump, "mjump - jump to module(mjump help - details)");
			NekkiConsole.RegisterCommand("nocloth", RemoveCloth, string.Empty);
			NekkiConsole.RegisterCommand("nophys", RemovePhysicsComponents, string.Empty);
			NekkiConsole.RegisterCommand("noshadow", NoShadow, string.Empty);
			NekkiConsole.RegisterCommand("noskin", RemoveSkin, string.Empty);
			NekkiConsole.RegisterCommand("pa", PlayAnimation, "pa [me/enemy] [animation name from moves_settings] - play animation on me/enemy with name");
			NekkiConsole.RegisterCommand("paf", PlayAnimationFixed, string.Empty);
			NekkiConsole.RegisterCommand("pai", SwitchAiModePlayer, "pai [bool] - enable | disable player ai");
			NekkiConsole.RegisterCommand("pe", EnablePlayer, "pe [bool] - enable | disable player");
			NekkiConsole.RegisterCommand("perk", GetPerk, "perk [ID, stackLevel] - getPerk");
			NekkiConsole.RegisterCommand("qqq", ExitGame, "qqq - exit the game Q.Q");
			NekkiConsole.RegisterCommand("rating", GetCurrentRating, "rating - prints current player rating");
			NekkiConsole.RegisterCommand("rgvar", RemoveGlobalVariable, "rgvar [string] - remove global variable with name");
			NekkiConsole.RegisterCommand("roundp", PauseRoundTime, "roundp [bool] - pause | resume round time (def:false)");
			NekkiConsole.RegisterCommand("setbt", SetBattleTimer, "setbt [int] - set battle timer by value");
			NekkiConsole.RegisterCommand("setres", SetResolution, string.Empty);
			NekkiConsole.RegisterCommand("sf", ShadowFormToogle, "sf [bool] - shadow form switch");
			NekkiConsole.RegisterCommand("sglow", SetSimpleGlow, string.Empty);
			NekkiConsole.RegisterCommand("sguid", SetGUID, "sguid [string] - sets guid to specified string (to share accounts)");
			NekkiConsole.RegisterCommand("sgvar", SetGlobalVariable, "sgvar [string] [string] - set global variable with name to value");
			NekkiConsole.RegisterCommand("shadow", ShadowEnadle, "shadow  - switch between shadow type");
			NekkiConsole.RegisterCommand("ssc", SetShadowCharge, "ssc [me/enemy] - set shadow charge to 1. To player me by default");
			NekkiConsole.RegisterCommand("stmesh", SkinToSimpleMesh, string.Empty);
			NekkiConsole.RegisterCommand("swmat", SwitchMaterials, "swmat - switch materials on char ");
			NekkiConsole.RegisterCommand("texqual", SetTextureQuality, string.Empty);
			NekkiConsole.RegisterCommand("timest", SetPhysicTimeStamp, string.Empty);
			NekkiConsole.RegisterCommand("tkc", TestKillConnection, "tkc - test kill connection");
			NekkiConsole.RegisterCommand("tlt", ToggleLocationTest, "tlt - toggle location testing mode");
			NekkiConsole.RegisterCommand("uload", SaveLoadUserConsoleCommands.LoadTestUser, "uload [string] - load user progress by name");
			NekkiConsole.RegisterCommand("unitycl", UnityConsole, "unitycl [bool] - activate unity-console mode and disable pause game in console");
			NekkiConsole.RegisterCommand("unlit", UnlitShader, string.Empty);
			NekkiConsole.RegisterCommand("usave", SaveLoadUserConsoleCommands.SaveTestUser, "usave - save current user progress");
			NekkiConsole.RegisterCommand("uua", TryUnloadUnusedAssets, "uua - UnloadUnusedAssets");
			NekkiConsole.RegisterCommand("vdebug", VisualDebug, string.Empty);
			NekkiConsole.RegisterCommand("vign", ToggleVignette, string.Empty);
			NekkiConsole.RegisterCommand("wtf", WinTheFight, "wtf [int] [int] - win $2 fights in chapter with ID $1");
			NekkiConsole.RegisterCommand("reload", Reload, "reload [int] - temp function to get reload dialog.");
			NekkiConsolePanel.OnConsoleActive += delegate(bool b)
			{
				isActive = b;
				if (!unityConsoleModeActive)
				{
					if (isActive)
					{
						BattleController.SystemPause();
					}
					else
					{
						BattleController.SystemResume();
					}
				}
			};
		}

		private static string Reload(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				int num = 0;
				if (args.Length == 1)
				{
					num = int.Parse(args[0]);
				}
				switch (num)
				{
				case 0:
					NetworkConnection.current.HandleUnrecoverableError();
					break;
				case 1:
					GameRestarter.ShowRestartDialog("configs_downloaded");
					break;
				case 2:
					GameRestarter.ShowRestartDialog("player_updated");
					break;
				default:
					GameRestarter.ShowRestartDialog("user_cleared");
					break;
				}
				return string.Empty;
			});
		}

		private static string FrameSkipShift(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				FrameSkipController.MaximumFrameShift = int.Parse(args[0]);
				Debug.Log("Frame skip shift set to " + FrameSkipController.MaximumFrameShift);
				return string.Empty;
			});
		}

		private static string FakeUIEvent(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				string text = ((args.Length < 1) ? "test_value" : args[0]);
				Analytics.Logger.AddEvent("UI_EVENT", "fake_key", text);
				return string.Empty;
			});
		}

		private static string SetGUID(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				if (args.Length != 1)
				{
					return "need guid as argument";
				}
				NetworkConnection.current.UserData.Auth.SetLogin(args[0]);
				GameRestarter.Restart();
				return "guid set";
			});
		}

		private static string GetCurrentRating(string[] args)
		{
			return SafeCommandExecution(() => UserManager.GetRating().ToString());
		}

		private static string TryUnloadUnusedAssets(string[] args)
		{
			return SafeCommandExecution(() => string.Empty);
		}

		private static string WinTheFight(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				int num = 1;
				int battleID = int.Parse(args[0]);
				if (args.Length > 1)
				{
					num = int.Parse(args[1]);
				}
				IBattleInfo battle = BattlesManager.instance.GetBattle(battleID);
				for (int i = 0; i < num; i++)
				{
					if (battle.GetIsCompleted())
					{
						break;
					}
					FightInfo currentFight = battle.GetCurrentFight();
					FightResult result = new FightResult(battle, currentFight, currentFight.roundsToWin, currentFight.roundsToWin, false, null);
					BattlesManager.CompleteFight(result);
				}
				return string.Empty;
			});
		}

		private static string SetControll(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				string value = args[0];
				if (bool.Parse(value))
				{
					BattleKeyManager.Instance.SetIsControll(2, true);
				}
				return string.Empty;
			});
		}

		private static string GetGlobalVariable(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				string text = args[0];
				string globalVariable = UserManager.GetGlobalVariable(text);
				return (globalVariable == null) ? string.Format("Havnt global variable with name [{0}]", text) : globalVariable;
			});
		}

		private static string SetGlobalVariable(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				string text = args[0];
				string text2 = args[1];
				UserManager.SetGlobalVariable(text, text2);
				return string.Format("Global variable [{0}] new value is [{1}]", text, text2);
			});
		}

		private static string RemoveGlobalVariable(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				string text = args[0];
				return (!UserManager.RemoveGlobalVariable(text)) ? string.Format("Havnt global variable with name [{0}] to remove", text) : string.Format("Global variable with name [{0}] is removed", text);
			});
		}

		private static string LoadBattleFromServer(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				int bValue = 0;
				BattleInfo battle = null;
				if (int.TryParse(args[0], out bValue))
				{
					BattlesManager.instance.LoadBattleFromServerCHEAT(bValue, delegate(IBattleInfo b)
					{
						if (b != null)
						{
							battle = b.GetBattleInfo();
							MapController.Instance.AddBattle(battle, MapBattleButton.DecorationType.Available);
							string text = string.Format("Battle loaded succesfull. ID: [{0}]; Name: [{1}];", battle.id, battle.name);
							Debug.Log(text);
							NekkiConsolePanel.Log(text);
						}
						else
						{
							string text2 = string.Format("Cant get battle from server by id [{0}]", bValue);
							Debug.Log(text2);
							NekkiConsolePanel.Log(text2);
						}
					});
				}
				return string.Empty;
			});
		}

		private static string ApplyTriggerActions(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				Model model = ModelsManager.Instance.Player;
				string value = string.Empty;
				if (args.Length == 2)
				{
					string text = args[0];
					value = args[1];
					if (text.Equals("me"))
					{
						model = ModelsManager.Instance.Player;
					}
					else
					{
						if (!text.Equals("enemy"))
						{
							return "Wrong player type parameter";
						}
						model = ModelsManager.Instance.Enemy;
					}
				}
				else if (args.Length == 1)
				{
					value = args[0];
				}
				ShadowFormController.Instance.SetShadowCharge(model.id, 1f);
				foreach (InfoTrigger currentTrigger in model.modelMoves.currentTriggers)
				{
					if (currentTrigger.parentName.Equals(value))
					{
						foreach (ITriggerAction action in currentTrigger.actions)
						{
							action.Apply(model);
						}
					}
				}
				return string.Format("Player [{0}] is a mister shadow now. (o-_-o)", model.name);
			});
		}

		private static string SetShadowCharge(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				Model model = ModelsManager.Instance.Player;
				if (args.Length >= 1)
				{
					string text = args[0];
					if (text.Equals("me"))
					{
						model = ModelsManager.Instance.Player;
					}
					else
					{
						if (!text.Equals("enemy"))
						{
							return "Wrong player type parameter";
						}
						model = ModelsManager.Instance.Enemy;
					}
				}
				ShadowFormController.Instance.SetShadowCharge(model.id, 1f);
				return string.Format("Shadow Charge filled for [{0}]", model.name);
			});
		}

		private static string ToggleLocationTest(string[] args)
		{
			PauseRoundTime("true");
			SwitchAiModeEnemy(new string[1] { "none" });
			return "Location testing mode on";
		}

		private static string GetPreset(string[] args)
		{
			return "Preset " + QualityManager.PresetName;
		}

		private static string GetAvailablePreset(string[] args)
		{
			string text = string.Empty;
			foreach (KeyValuePair<string, DevicePreset> available in QualityManager.Availables)
			{
				text = text + available.Key + ", ";
			}
			return text;
		}

		private static string SetPreset(string[] args)
		{
			if (args.Length > 0)
			{
				string text = args[0].ToLower();
				bool save = args[1].ToLower().Equals("true");
				switch (QualityManager.ChangePreset(text, true, save))
				{
				case QualityManager.ChangeType.INSTALLED:
					return "Error Preset " + text + " already installed";
				case QualityManager.ChangeType.NOT_FOUND:
					return "Error Preset " + text + " not found";
				case QualityManager.ChangeType.NOT_AVAILABLE:
					return "Error Preset " + text + " not available";
				default:
					return "Preset " + text + " successfully installed";
				}
			}
			return "Error Preset No arguments";
		}

		private static string PlayAnimation(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				if (args.Length >= 2)
				{
					string text = args[0];
					string text2 = args[1];
					if (text.Equals("me"))
					{
						ModelsManager.Instance.Player.modelAnimation.Play(text2, true);
					}
					else
					{
						if (!text.Equals("enemy"))
						{
							return "Wrong player type parameter";
						}
						ModelsManager.Instance.Enemy.modelAnimation.Play(text2, true);
					}
					return string.Format("Player type = {0}; Animation name = {1}", text, text2);
				}
				return "Wrong parameters count";
			});
		}

		private static string PlayAnimationFixed(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				if (args.Length >= 2)
				{
					string text = args[0];
					string text2 = args[1];
					Model model = ((!text.Equals("me")) ? ModelsManager.Instance.Enemy : ModelsManager.Instance.Player);
					List<InfoAnimation> animations = MovesController.GetAnimations();
					ModelInfoAnimation modelInfoAnimation = null;
					foreach (InfoAnimation item in animations)
					{
						if (item != null && item.name != null && item.name.Equals(text2))
						{
							modelInfoAnimation = new ModelInfoAnimation(item, model);
						}
					}
					if (modelInfoAnimation != null)
					{
						model.modelAnimation.PlayFixed(modelInfoAnimation);
					}
					return string.Format("Player type = {0}; Animation name = {1}", text, text2);
				}
				return "Wrong parameters count";
			});
		}

		private static string SetGender(string[] args)
		{
			try
			{
				if (args.Length > 0)
				{
					Gender enumerator = EnumsCompliancer.GetEnumerator<Gender>(args[0]);
					UserManager.SetGender(enumerator);
				}
				else
				{
					UserManager.SetGender((UserManager.GetGender() != Gender.Male) ? Gender.Male : Gender.Female);
				}
				ModelsManager.Instance.Player.UpdateModelInfo();
				Appearance appearance = new Appearance();
				appearance.Gender = UserManager.GetGender();
				appearance.HeadId = UserManager.GetHeadId();
				appearance.HairColor = UserManager.GetHairColor();
				appearance.SkinColor = UserManager.GetSkinColor();
				UserDataController.Send_SetAppearance(appearance);
			}
			catch (Exception)
			{
				return "something goes wrong ^(";
			}
			return string.Format("done! character is {0} now", UserManager.GetGender());
		}

		private static string SetSimpleGlow(string[] args)
		{
			GlowEffectController.instance.SetSimpleGlow();
			return "SimpleGlow";
		}

		private static string SetBoneWeight(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				int result = 1;
				if (int.TryParse(args[0], out result))
				{
					switch (result)
					{
					case 1:
						QualitySettings.skinWeights = SkinWeights.OneBone;
						return "Blend weight now use 1 bones";
					case 2:
						QualitySettings.skinWeights = SkinWeights.TwoBones;
						return "Blend weight now use 2 bones";
					case 4:
						QualitySettings.skinWeights = SkinWeights.FourBones;
						return "Blend weight now use 4 bones";
					}
				}
				return "Can't parse";
			});
		}

		private static string SetTextureQuality(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				int result = 1;
				if (int.TryParse(args[0], out result))
				{
					QualitySettings.masterTextureLimit = result;
					return "masterTextureLimit weight now " + result;
				}
				return "Can't parse";
			});
		}

		private static string SetBlurIterations(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				int result = 1;
				if (int.TryParse(args[0], out result))
				{
					GlowEffectController.instance.SetBlurIterations(result);
					return "Glow blur iterations now is  " + result;
				}
				return "Can't parse";
			});
		}

		private static string SetResolution(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				float result = 1f;
				if (float.TryParse(args[0], out result))
				{
					Screen.SetResolution(Mathf.RoundToInt((float)defaultWidth * result), Mathf.RoundToInt((float)defaultHeight * result), true);
					return "Screen resolution changed";
				}
				return "Can't parse";
			});
		}

		private static string SetPhysicTimeStamp(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				float result = 1f;
				if (float.TryParse(args[0], out result))
				{
					GameTimeController.SetPhysicTimeStamp(result);
					return "Physic time stamp now is  " + result;
				}
				return "Can't parse";
			});
		}

		private static string ToggleFog(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				bool result = false;
				if (bool.TryParse(args[0], out result))
				{
					RenderSettings.fog = result;
					return "Fog now is " + result;
				}
				return "Can't parse";
			});
		}

		private static string ToggleVignette(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				bool result = false;
				if (bool.TryParse(args[0], out result))
				{
					LocationColorAnimation.Instance.ToggleVignette(result);
					return "Vignette now is " + result;
				}
				return "Can't parse";
			});
		}

		private static string SetBattleTimer(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				int result = 1;
				if (int.TryParse(args[0], out result))
				{
					BattleInterface.Instance.SetBattleTimerFrames(result * 60 + 1);
					return "Timer lowed by " + result;
				}
				return "Timer lowed by " + 0;
			});
		}

		private static string ShadowFormToogle(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				bool result = false;
				if (bool.TryParse(args[0], out result))
				{
					if (result)
					{
						ShadowFormController.Instance.ActivateShadowForm(ModelsManager.Instance.Player.id, "shadow_fire", true);
						return "Shadow Form Activated";
					}
					ShadowFormController.Instance.DisableShadowForm(ModelsManager.Instance.Player.id);
					return "Shadow Form Disactivated";
				}
				return MsgArgParsingError;
			});
		}

		private static string ClearUser(string[] args)
		{
			UserManager.Instance.ClearUser();
			GameRestarter.ShowRestartDialog("user_cleared");
			return "User is cleared";
		}

		private static string CleanUsers(string[] args)
		{
			NekkiUtils.ClearAllApplication();
			return "Users are cleaned succesfull";
		}

		private static string ExitGame(string[] args)
		{
			Application.Quit();
			return "Exit Game Succesfull";
		}

		private static string UnityConsole(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				bool result = false;
				if (bool.TryParse(args[0], out result))
				{
					if (result)
					{
						if (!unityConsoleModeActive)
						{
							unityConsoleModeActive = true;
							BattleController.SystemResume();
						}
						return "unity-console mode activated";
					}
					if (unityConsoleModeActive)
					{
						unityConsoleModeActive = false;
						BattleController.SystemPause();
					}
					return "unity-console mode disabled";
				}
				return MsgArgParsingError;
			});
		}

		private static string NoShadow(string[] args)
		{
			Projector[] array = UnityEngine.Object.FindObjectsOfType<Projector>();
			for (int i = 0; i < array.Length; i++)
			{
				UnityEngine.Object.Destroy(array[i].gameObject);
			}
			return "Shadow destroyed";
		}

		private static string RemoveCloth(string[] args)
		{
			Cloth[] array = UnityEngine.Object.FindObjectsOfType<Cloth>();
			for (int i = 0; i < array.Length; i++)
			{
				UnityEngine.Object.Destroy(array[i]);
			}
			return "Cloth removed";
		}

		private static string SkinToSimpleMesh(string[] args)
		{
			SkinnedMeshRenderer[] array = UnityEngine.Object.FindObjectsOfType<SkinnedMeshRenderer>();
			for (int i = 0; i < array.Length; i++)
			{
				if (!(array[i].GetComponent<Cloth>() != null))
				{
					array[i].gameObject.AddComponent<MeshFilter>().mesh = array[i].sharedMesh;
					array[i].gameObject.AddComponent<MeshRenderer>().material = array[i].material;
					UnityEngine.Object.Destroy(array[i]);
				}
			}
			return "Skins removed";
		}

		private static string RemoveSkin(string[] args)
		{
			RemoveCloth(new string[1] { string.Empty });
			SkinnedMeshRenderer[] array = UnityEngine.Object.FindObjectsOfType<SkinnedMeshRenderer>();
			for (int i = 0; i < array.Length; i++)
			{
				UnityEngine.Object.Destroy(array[i]);
			}
			return "Skinned meshes removed";
		}

		private static string RemovePhysicsComponents(string[] args)
		{
			Collider[] array = UnityEngine.Object.FindObjectsOfType<Collider>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].gameObject.layer != 31)
				{
					UnityEngine.Object.Destroy(array[i]);
				}
			}
			Joint[] array2 = UnityEngine.Object.FindObjectsOfType<Joint>();
			for (int j = 0; j < array2.Length; j++)
			{
				UnityEngine.Object.Destroy(array2[j]);
			}
			Rigidbody[] array3 = UnityEngine.Object.FindObjectsOfType<Rigidbody>();
			for (int k = 0; k < array3.Length; k++)
			{
				UnityEngine.Object.Destroy(array3[k]);
			}
			return "Physics components removed";
		}

		private static string UnlitShader(string[] args)
		{
			MeshRenderer[] array = UnityEngine.Object.FindObjectsOfType<MeshRenderer>();
			MeshRenderer[] array2 = array;
			foreach (MeshRenderer meshRenderer in array2)
			{
				meshRenderer.material.shader = Shader.Find("Unlit/Texture");
			}
			SkinnedMeshRenderer[] array3 = UnityEngine.Object.FindObjectsOfType<SkinnedMeshRenderer>();
			SkinnedMeshRenderer[] array4 = array3;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in array4)
			{
				skinnedMeshRenderer.material.shader = Shader.Find("Unlit/Texture");
			}
			return "SimpleShader";
		}

		private static string SwitchGlow(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				bool result = false;
				if (bool.TryParse(args[0], out result))
				{
					GlowEffectController.instance.ToggleGlow(result);
					return (!result) ? "glow disabled" : "glow enabled";
				}
				return MsgArgParsingError;
			});
		}

		private static string SwitchMaterials(string[] args)
		{
			return SafeCommandExecution(delegate
			{
				bool result = false;
				if (bool.TryParse(args[0], out result))
				{
					ModelSkin[] array = UnityEngine.Object.FindObjectsOfType<ModelSkin>();
					ModelSkin[] array2 = array;
					foreach (ModelSkin modelSkin in array2)
					{
						modelSkin.SwitchMaterial(result);
					}
					return (!result) ? "matcap materials" : "simple materials";
				}
				return MsgArgParsingError;
			});
		}

		private static string EmailLog(string[] args)
		{
			AdvLog.EmailLog(args[0], "shadow fight 3 - log system");
			return "logs send requested to " + args[0];
		}

		private static string ClearBundles(string[] args)
		{
			Resources.UnloadUnusedAssets();
			return "Unloaded";
		}

		private static string VisualDebug(string[] args)
		{
			VisualDebugUI.Open();
			NekkiConsolePanel.Instance.OnButton();
			return "goto visual debug mode";
		}

		private static string GetDeviceModel(string[] args)
		{
			return SystemInfo.deviceModel;
		}

		private static string Mjump(params string[] args)
		{
			if (args == null || args.Length < 1)
			{
				return "not enought paremeters (type: mjump help - for details)";
			}
			if (args[0] == "help")
			{
				string empty = string.Empty;
				empty += "jump to shop:\n";
				empty += "  mjump Shop [categoryName(Weapons|Perks|Helms|Armors...(calss:ConstantsSF3.ItemsCategories))]\n";
				empty += "jump to inventory:\n";
				empty += "  mjump Inventory [categoryName(HELMET|ARMOR|WEAPON|PERK...(enum:ItemSubtype))] [itemID]\n";
				empty += "jump to map:\n";
				empty += "  mjump Map [battleID]\n";
				empty += "jump to dojo:\n";
				empty += "  mjump DojoInterface\n";
				empty += "jump to fight:\n";
				return empty + "  mjump Fight [fightID]\n";
			}
			try
			{
				List<object> list = new List<object>(args);
				list.RemoveAt(0);
				return "jump process ...";
			}
			catch (Exception)
			{
				return "jump fails under mysterious circumstances";
			}
		}

		private static string Mfloat(params string[] args)
		{
			if (args == null || args.Length < 1)
			{
				return "not enought paremeters (type: mfloat help - for details)";
			}
			if (args[0] == "help")
			{
				string empty = string.Empty;
				empty += "float to shop:\n";
				empty += "  mfloat Shop [categoryName(Weapons|Perks|Helms|Armors...(calss:ConstantsSF3.ItemsCategories))]\n";
				empty += "float to inventory:\n";
				empty += "  mfloat Inventory [categoryName(HELMET|ARMOR|WEAPON|PERK...(enum:ItemSubtype))] [itemID]\n";
				empty += "float to map:\n";
				empty += "  mfloat Map [battleID]\n";
				empty += "float to dojo:\n";
				empty += "  mfloat DojoInterface\n";
				empty += "float to fight:\n";
				return empty + "  mfloat Fight [fightID]\n";
			}
			try
			{
				List<object> list = new List<object>(args);
				list.RemoveAt(0);
				return "float process ...";
			}
			catch (Exception)
			{
				return "float fails under mysterious circumstances";
			}
		}

		private static string CriticalChanceFactor(params string[] args)
		{
			return SafeCommandExecution(delegate
			{
				int id = ModelsManager.Instance.Player.id;
				float result = 1f;
				if (args.Length == 2)
				{
					string text = args[0];
					if (!float.TryParse(args[1], out result))
					{
						result = 1f;
					}
					if (text.Equals("me"))
					{
						id = ModelsManager.Instance.Player.id;
					}
					else
					{
						if (!text.Equals("enemy"))
						{
							return "Wrong player type parameter";
						}
						id = ModelsManager.Instance.Enemy.id;
					}
				}
				else if (!float.TryParse(args[0], out result))
				{
					result = 1f;
				}
				ModelsAttributesController.Instance.AddAttributeFactorModifier(id, AttributeType.CriticalChance, result, 9999999f);
				return "Critical chance factor changed: x" + result;
			});
		}

		private static string DamageFactor(params string[] args)
		{
			return SafeCommandExecution(delegate
			{
				int id = ModelsManager.Instance.Player.id;
				float result = 1f;
				if (args.Length == 2)
				{
					string text = args[0];
					if (!float.TryParse(args[1], out result))
					{
						result = 1f;
					}
					if (text.Equals("me"))
					{
						id = ModelsManager.Instance.Player.id;
					}
					else
					{
						if (!text.Equals("enemy"))
						{
							return "Wrong player type parameter";
						}
						id = ModelsManager.Instance.Enemy.id;
					}
				}
				else if (!float.TryParse(args[0], out result))
				{
					result = 1f;
				}
				ModelsAttributesController.Instance.AddAttributeFactorModifier(id, AttributeType.UnarmedDamage, result, 9999999f);
				ModelsAttributesController.Instance.AddAttributeFactorModifier(id, AttributeType.WeaponDamage, result, 9999999f);
				return "Damage factor changed: x" + result;
			});
		}

		private static string ShadowEnadle(params string[] args)
		{
			ModelShadow.UseRealShadow = !ModelShadow.UseRealShadow;
			return "useRealShadow is " + ModelShadow.UseRealShadow;
		}

		private static string ClothEnable(params string[] args)
		{
			return SafeCommandExecution(delegate
			{
				bool result = false;
				if (bool.TryParse(args[0], out result))
				{
					Cloth[] array = UnityEngine.Object.FindObjectsOfType<Cloth>();
					for (int i = 0; i < array.Length; i++)
					{
						array[i].enabled = result;
						array[i].transform.localPosition += new Vector3(0.0001f, 0f, 0f);
					}
					return "Cloth " + ((!result) ? "disabled" : "enabled");
				}
				Cloth[] array2 = UnityEngine.Object.FindObjectsOfType<Cloth>();
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j].enabled = true;
					array2[j].transform.localPosition += new Vector3(0.0001f, 0f, 0f);
				}
				return "Cloth enabled";
			});
		}

		private static string Clear(params string[] args)
		{
			return SafeCommandExecution(delegate
			{
				NekkiConsolePanel.Clear();
				return "Console cleared";
			});
		}

		private static string Help(params string[] args)
		{
			return SafeCommandExecution(delegate
			{
				if (args.Length > 0)
				{
					StringBuilder stringBuilder = new StringBuilder();
					string[] array = NekkiConsole.Descriptions.Split('\n');
					string[] array2 = array;
					foreach (string text in array2)
					{
						string[] array3 = args;
						foreach (string value in array3)
						{
							if (text.Contains(value))
							{
								stringBuilder.AppendLine(text);
							}
						}
					}
					return stringBuilder.ToString();
				}
				return NekkiConsole.Descriptions;
			});
		}

		private static string AddCoins(params string[] args)
		{
			return SafeCommandExecution(delegate
			{
				long result = 0L;
				if (long.TryParse(args[0], out result))
				{
					UserManager.AddCurrency(CurrencyType.Coin, result);
					NetworkConnection.Send("cheat_set_currency", new Currency
					{
						CurrencyType = CurrencyType.Coin,
						Value = UserManager.GetCurrency(CurrencyType.Coin).Value
					});
					return "Added " + result + " coins";
				}
				return MsgArgParsingError;
			});
		}

		private static string GetEquip(params string[] args)
		{
			return SafeCommandExecution(delegate
			{
				int modelId = int.Parse(args[0]);
				double stackLevel = ((args.Length != 1) ? double.Parse(args[1]) : 0.0);
				NetworkConnection.Send("cheat_add_item", new Item
				{
					ModelId = modelId,
					StackLevel = stackLevel
				}, delegate
				{
				});
				return modelId.ToString();
			});
		}

		private static string GetPerk(params string[] args)
		{
			return SafeCommandExecution(delegate
			{
				int modelId = int.Parse(args[0]);
				double stackLevel = ((args.Length != 1) ? double.Parse(args[1]) : 0.0);
				NetworkConnection.Send("cheat_add_perk", new Perk
				{
					ModelId = modelId,
					StackLevel = stackLevel
				}, delegate
				{
				});
				return modelId.ToString();
			});
		}

		private static string AddBonuses(params string[] args)
		{
			return SafeCommandExecution(delegate
			{
				long result = 0L;
				if (long.TryParse(args[0], out result))
				{
					UserManager.AddCurrency(CurrencyType.Bonus, result);
					NetworkConnection.Send("cheat_set_currency", new Currency
					{
						CurrencyType = CurrencyType.Bonus,
						Value = UserManager.GetCurrency(CurrencyType.Bonus).Value
					});
					return "Added " + result + " bonuses";
				}
				return MsgArgParsingError;
			});
		}

		private static string GetBooster(params string[] args)
		{
			return SafeCommandExecution(delegate
			{
				if (args.Length > 0)
				{
					int num = int.Parse(args[0]);
					double num2 = ((args.Length != 1) ? double.Parse(args[1]) : 1.0);
					Currency currency = new Currency
					{
						CurrencyType = CurrencyType.Coin
					};
					foreach (KeyValuePair<int, ShopBooster> shopBooster in JS.Instance.ShopBoosters)
					{
						if (shopBooster.Value.ID == num)
						{
							long value = 0L;
							shopBooster.Value.Currencies.TryGetValue(CurrencyType.Coin, out value);
							currency.Value = value;
							AddCoins(((double)currency.Value * num2).ToString());
							for (int i = 0; (double)i < num2; i++)
							{
								BuyBoosterRequest request = new BuyBoosterRequest
								{
									ShopBoosterModelId = shopBooster.Value.ID,
									Currency = currency
								};
								UserDataController.Send_BuyBooster(request, 10f);
							}
							return "Add " + num2 + " Boosters of type " + num;
						}
					}
					return "Booster with ID: " + num + " doesn't exist!";
				}
				string text = string.Empty;
				foreach (KeyValuePair<int, ShopBooster> shopBooster2 in JS.Instance.ShopBoosters)
				{
					text = text + shopBooster2.Value.ID + ", ";
				}
				return "Avaliable boosters IDs: " + text;
			});
		}

		private static string GetMaxUp(params string[] args)
		{
			AddBonuses("9999999");
			AddCoins("9999999");
			AddExperience("60");
			return "Take your money and run!";
		}

		private static string TestDisconnect(params string[] args)
		{
			UnityEngine.Object.FindObjectOfType<NekkiConsolePanel>().OnButton();
			NetworkConnection.current.RestartConnection("test disconnect", false);
			return "disconnect";
		}

		private static string TestKillConnection(params string[] args)
		{
			NetworkConnection.current.TestKillConnection();
			return "testing disconnect";
		}

		private static string AddExperience(params string[] args)
		{
			return SafeCommandExecution(delegate
			{
				int result = 0;
				int num = JsFunction.CalculateRealExperience(UserManager.GetLevel(), UserManager.GetExperience());
				if (args.Length == 0)
				{
					return "Exp: " + UserManager.GetExperience() + ". Real exp: " + num;
				}
				if (int.TryParse(args[0], out result))
				{
					num += result;
					Dictionary<string, JsValue> dictionary = JsFunction.CalculateLevelAndExperience(num);
					UserManager.SetExperience(dictionary["Experience"].AsInteger());
					UserManager.SetLevel(dictionary["Level"].AsInteger());
					UserManager.SetLevelExperience(dictionary["LevelExperience"].AsInteger());
					SendCheatExperience(num);
					return "Added " + result + " experience. Total: " + num;
				}
				return MsgArgParsingError;
			});
		}

		private static string SetLevel(params string[] args)
		{
			return SafeCommandExecution(delegate
			{
				int result = 0;
				if (int.TryParse(args[0], out result))
				{
					int realExp = JsFunction.CalculateRealExperience(result, 0L);
					UserManager.SetLevel(result);
					UserManager.SetExperience(0L);
					SendCheatExperience(realExp);
					return "New level " + result;
				}
				return MsgArgParsingError;
			});
		}

		private static void SendCheatExperience(int realExp)
		{
			NetworkConnection.Send("cheat_set_experience", new Int64Value
			{
				Value = realExp
			}, delegate(NetworkEvent e)
			{
				UserDataController.UpdatePlayerData(e.getExtensible<Player>());
			});
		}

		private static string GetAllItems(params string[] args)
		{
			double result = 0.0;
			if (args.Length > 0)
			{
				double.TryParse(args[0], out result);
			}
			DoubleValue doubleValue = new DoubleValue();
			doubleValue.Value = result;
			NetworkConnection.Send("cheat_set_all_items", doubleValue, delegate(NetworkEvent e)
			{
				if (e.success)
				{
					NetworkConnection.current.RestartConnection("allitems cheat", false);
					Debug.Log("GetAllItems Done!");
				}
				else
				{
					Debug.LogError("GetAllItems Failed");
				}
			});
			return string.Empty;
		}

		private static string PauseRoundTime(params string[] args)
		{
			return SafeCommandExecution(delegate
			{
				bool result = false;
				if (bool.TryParse(args[0], out result))
				{
					BattleInterface.Instance.BattleTimerActive(!result);
					return "Round time " + ((!result) ? "resumed" : "paused");
				}
				BattleInterface.Instance.BattleTimerActive(false);
				return "Round time resumed";
			});
		}

		private static string Capsules(params string[] args)
		{
			return SafeCommandExecution(delegate
			{
				bool result = false;
				string result2 = "Capsules hidden";
				if (bool.TryParse(args[0], out result))
				{
					result2 = "Capsules " + ((!result) ? "hidden" : "visible");
				}
				ShowHideCapsules(result);
				return result2;
			});
		}

		private static void ShowHideCapsules(bool enabled)
		{
			ModelCapsules.ShowCapsules = enabled;
			ModelsManager.Instance.Player.ShowSkins(!enabled);
			ModelsManager.Instance.Enemy.ShowSkins(!enabled);
			if (!enabled)
			{
				EffectsManager.StopAll("TestEffect");
			}
		}

		private static string EnableEnemy(string[] args)
		{
			return EnableModel(ModelsManager.Instance.Enemy, args);
		}

		private static string EnablePlayer(string[] args)
		{
			return EnableModel(ModelsManager.Instance.Player, args);
		}

		private static string EnableModel(Model model, string[] args)
		{
			if (null == model)
			{
				return "Model is null!";
			}
			return SafeCommandExecution(delegate
			{
				string msg = string.Format("Model [{0}] state changed to ", (!model.id.Equals(1)) ? "Enemy" : "Player");
				bool result;
				if (bool.TryParse(args[0], out result))
				{
					return SetActive(model, result, msg);
				}
				int result2;
				return int.TryParse(args[0], out result2) ? SetActive(model, result2 == 1, msg) : MsgArgParsingError;
			});
		}

		private static string SetActive(Model model, bool enabled, string msg)
		{
			model.gameObject.SetActive(enabled);
			return msg + enabled;
		}

		private static string SwitchAiModeEnemy(string[] args)
		{
			return SafeCommandExecution(() => SwitchAiMode(ModelsManager.Instance.Enemy, args));
		}

		private static string SwitchAiModePlayer(string[] args)
		{
			return SafeCommandExecution(() => SwitchAiMode(ModelsManager.Instance.Player, args));
		}

		private static string SwitchAiMode(Model model, string[] args)
		{
			if (null == model)
			{
				return "Model is null!";
			}
			if (args.Length == 0)
			{
				return "Choode Ai mode name!";
			}
			AiMode aiMode = AiMode.NoneMode;
			switch (args[0])
			{
			case "sensei":
			case "s":
				aiMode = AiMode.SenseiMode;
				break;
			case "regular":
			case "r":
				aiMode = AiMode.RegularMode;
				break;
			case "dojo":
			case "d":
				aiMode = AiMode.DojoMode;
				break;
			default:
				Messenger.Error(string.Format("Ai mode [{0}] is not supported.", args[0]));
				return MsgArgParsingError;
			case "none":
			case "n":
				break;
			}
			model.SetAiMode(aiMode);
			return string.Format("Ai mode changed to [{0}]", aiMode);
		}

		public static string BlockInput(string[] args)
		{
			int result = 3;
			if (args.Length > 0 && int.TryParse(args[0], out result))
			{
				result = ((result < 1) ? 1 : result);
			}
			NekkiConsolePanel.Instance.OnButton();
			Routiner.Go(BlockScreenForAWhile(result));
			return string.Format("Input is blocked for [{0}] seconds", result);
		}

		public static IEnumerator BlockScreenForAWhile(int seconds)
		{
			while (isActive)
			{
				yield return null;
			}
			UIBlocker.Instance.BlockNGUI();
			LoadingIcon.Instance.EnableLoadingScreen();
			yield return new WaitForSecondsRealtime(seconds);
			UIBlocker.Instance.UnblockNGUI();
			LoadingIcon.Instance.DisableLoadingScreen();
		}
	}
}
