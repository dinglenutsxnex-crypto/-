using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Nekki.Yaml;
using SF3.GameModels;
using SF3.Moves;
using SF3.Tactics;
using sf3DTO;

namespace SF3.Settings
{
	[Serializable]
	public class TacticsSettings
	{
		private class MovesException
		{
			public string name;

			public Dictionary<string, string> properties;
		}

		public class DelayResetEvent
		{
			public ETriggerEvents type;

			public string name;

			public Dictionary<string, string> properties;

			public DelayResetEvent(ETriggerEvents type)
			{
				this.type = type;
				properties = new Dictionary<string, string>();
			}
		}

		public enum ExceptionProperty
		{
			NonProvocation = 0,
			NonReaction = 1,
			IgnoreProvocation = 2,
			ReverseResult = 3,
			ForcedReaction = 4
		}

		public static readonly bool BOXING_DEBUG = false;

		public const string COMPLIANCE_TABLE_NAME = "COMPLIANCE.bytes";

		public static string TacticsPivot = "TacticsPivot";

		public static string TacticsMirroredPivot = "TacticsMirroredPivot";

		private static readonly Dictionary<string, string> Settings = new Dictionary<string, string>();

		private static readonly Dictionary<string, MovesException> Exceptions = new Dictionary<string, MovesException>();

		private static readonly Dictionary<AiMode, TacticsMode> TacticsModes = new Dictionary<AiMode, TacticsMode>();

		private static bool _initilized;

		private static bool? _hasTacticsModule;

		private static List<TriggerEvent> _delayEvents;

		public static Dictionary<AiMode, List<string>> GroupNoReactions;

		public static bool HasTacticsModule
		{
			get
			{
				bool? hasTacticsModule = _hasTacticsModule;
				if (!hasTacticsModule.HasValue)
				{
					_hasTacticsModule = File.Exists("Assets/Tactics/developer");
				}
				return _hasTacticsModule.Value;
			}
		}

		public static void Init()
		{
			if (!_initilized)
			{
				_initilized = true;
				string tacticsSettings = ConfigsSourceResolver.TacticsSettings;
				YamlDocumentNekki yamlDocumentNekki = YamlDocumentNekki.FromYamlContent(tacticsSettings);
				Mapping mapping = yamlDocumentNekki.GetRoot().GetMapping("Settings");
				InitMainSetting(mapping);
				InitMovesExceptions((Sequence)mapping.GetNode("MovesExceptions"));
				Node node = mapping.GetNode("Delay");
				if (node != null)
				{
					Mapping delaySettingsMapping = (Mapping)node;
					InitDelaySettings(delaySettingsMapping);
				}
				InitTacticsModes((Sequence)mapping.GetNode("TacticsModes"));
				InitGroupNoReactions(mapping.GetNode("GroupNoReactions"));
			}
		}

		private static void InitMainSetting(Mapping tacticsSettings)
		{
			foreach (Node item in tacticsSettings.nodesInside)
			{
				if (item is Scalar)
				{
					Settings.Add(item.key, (item as Scalar).text);
				}
			}
		}

		private static void InitMovesExceptions(Sequence movesExceptions)
		{
			foreach (Mapping item in movesExceptions.nodesInside)
			{
				List<Node> list = new List<Node>();
				Sequence sequence = null;
				sequence = item.GetSequence("Names");
				if (sequence != null)
				{
					list.AddRange(sequence.nodesInside);
				}
				sequence = item.GetSequence("Groups");
				if (sequence != null)
				{
					list.AddRange(sequence.nodesInside);
				}
				List<Node> nodesInside = item.GetSequence("Properties").nodesInside;
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				foreach (Mapping item2 in nodesInside)
				{
					dictionary[item2.nodesInside[0].key] = item2.nodesInside[0].ToString();
				}
				foreach (Scalar item3 in list)
				{
					string text = item3.text.ToLower();
					if (Exceptions.ContainsKey(text))
					{
						foreach (KeyValuePair<string, string> item4 in dictionary)
						{
							Exceptions[text].properties[item4.Key] = item4.Value;
						}
					}
					else
					{
						MovesException ex = new MovesException();
						ex.name = text;
						ex.properties = dictionary;
						MovesException value = ex;
						Exceptions[text] = value;
					}
				}
			}
		}

		private static void InitDelaySettings(Mapping delaySettingsMapping)
		{
			_delayEvents = null;
			Node node = delaySettingsMapping.GetNode("ResetOnEvents");
			if (node == null)
			{
				return;
			}
			string[] names = Enum.GetNames(typeof(AiMode));
			foreach (string name in names)
			{
				Node node2 = ((Mapping)node).GetNode(name);
				if (node2 != null)
				{
					Sequence nodes = (Sequence)node2;
					_delayEvents = TriggerEvent.Parse(nodes);
				}
			}
		}

		public static bool IsDelaySettings(BattleEventArgs currentData)
		{
			if (_delayEvents != null)
			{
				foreach (TriggerEvent delayEvent in _delayEvents)
				{
					if (delayEvent.type == currentData.EventType && delayEvent.Equal(currentData))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static void InitTacticsModes(Sequence tacticsModesSequence)
		{
			foreach (Mapping item in tacticsModesSequence.nodesInside)
			{
				foreach (Sequence item2 in item.nodesInside)
				{
					TacticsMode tacticsMode = new TacticsMode(item2);
					if (!TacticsModes.ContainsKey(tacticsMode.GetAiMode()))
					{
						TacticsModes.Add(tacticsMode.GetAiMode(), tacticsMode);
					}
					else
					{
						Messenger.Error(string.Format("<{0}> Failed to add tactics mode to global settings dictionary. This mode type alreadi exists. Check mode parsed correctly. Mode: {1}", typeof(TacticsSettings).Name, tacticsMode.ToString()));
					}
				}
			}
		}

		private static void InitGroupNoReactions(Node movesNoReactions)
		{
			GroupNoReactions = new Dictionary<AiMode, List<string>>();
			foreach (AiMode value in Enum.GetValues(typeof(AiMode)))
			{
				GroupNoReactions.Add(value, new List<string>());
				Node node = ((Mapping)movesNoReactions).GetNode(value.ToString());
				if (node == null)
				{
					continue;
				}
				foreach (Scalar item in ((Sequence)node).nodesInside)
				{
					GroupNoReactions[value].Add(item.GetText().ToLower());
				}
			}
		}

		public static string GetParamByName(string name)
		{
			return (!Settings.ContainsKey(name)) ? null : Settings[name];
		}

		public static T GetParamByName<T>(string name)
		{
			if (Settings.ContainsKey(name))
			{
				return (T)Convert.ChangeType(Settings[name], typeof(T), CultureInfo.InvariantCulture);
			}
			throw new Exception("Tactics Settings don't have parametrs with name " + name);
		}

		public static Bone GetPivotBoneForModel(Model model)
		{
			if (!Settings.ContainsKey(TacticsPivot))
			{
				return model.GetPivotBone();
			}
			return (!model.moveControl.mirrored) ? GetBone(model, TacticsPivot) : GetBone(model, TacticsMirroredPivot);
		}

		private static Bone GetBone(Model model, string pivot)
		{
			return model.modelComponents.GetBone(Settings[pivot]);
		}

		public static TacticsMode GetTacticsMode(AiMode tacticsMode)
		{
			if (!TacticsModes.ContainsKey(tacticsMode))
			{
				throw new KeyNotFoundException("No tactics profile by name [" + tacticsMode.ToString() + "] found.");
			}
			return TacticsModes[tacticsMode];
		}

		public static bool IsGroupNoReactions(string[] animations, AiMode mode)
		{
			foreach (string item in GroupNoReactions[mode])
			{
				foreach (string value in animations)
				{
					if (item.Equals(value))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
