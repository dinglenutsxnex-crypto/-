using System.Collections.Generic;
using Ionic.Zlib;
using Nekki.Yaml;
using Nekki.Zip;
using SF3.Moves;

public class SplitTriggers
{
	public const string HeaderName = "HEADER.yaml";

	public static readonly string SplitDir = GlobalPath.ApplicationPath + "/MovesSplit";

	public static readonly string SplitZipName = "Configs/splitmoves.bytes";

	private static Dictionary<string, Mapping> _animationsSettings;

	private static Dictionary<int, SplitData> _splitsData;

	private static string TriggersResult
	{
		get
		{
			return GetResultFile("GameSettings/Results/moves_triggers_result");
		}
	}

	private static string AnimationsSettingsFile
	{
		get
		{
			return GetResultFile("GameSettings/Results/moves_settings_result");
		}
	}

	public static void Split()
	{
		_animationsSettings = new Dictionary<string, Mapping>();
		_splitsData = new Dictionary<int, SplitData>();
		Condition.Init();
		TriggerEvent.Init();
		TriggerAction.Init();
		Parse();
		Save();
	}

	private static void Parse()
	{
		ParseAnimationSettings();
		string loadText = GlobalLoad.GetLoadText(TriggersResult);
		YamlDocumentNekki yamlDocumentNekki = YamlDocumentNekki.FromYamlContent(loadText);
		Sequence sequence = yamlDocumentNekki.GetRoot().GetSequence("TriggerSets");
		int num = 0;
		foreach (Mapping item in sequence.nodesInside)
		{
			string text = GetText(item, "Name");
			string text2 = GetText(item, "Type");
			if (!text.IsNullOrEmpty() && !text.Equals("Tactics") && (text2.IsNullOrEmpty() || !text2.Equals("Template")) && ParseLocks(item, "split_" + num))
			{
				num++;
			}
		}
	}

	private static void ParseAnimationSettings()
	{
		string loadText = GlobalLoad.GetLoadText(AnimationsSettingsFile);
		YamlDocumentNekki yamlDocumentNekki = YamlDocumentNekki.FromYamlContent(loadText);
		Sequence sequence = yamlDocumentNekki.GetRoot().GetSequence("Animations");
		foreach (Mapping item in sequence.nodesInside)
		{
			string key = GetText(item, "Animation").ToLower();
			if (!_animationsSettings.ContainsKey(key))
			{
				_animationsSettings.Add(key, item);
			}
		}
	}

	private static bool ParseLocks(Mapping node, string fileName)
	{
		bool result = false;
		Sequence sequence = node.GetSequence("Locks");
		if (sequence != null)
		{
			int key = HashLocks.CreateHash(sequence);
			string text = GetText(node, "Name");
			if (!_splitsData.ContainsKey(key))
			{
				_splitsData.Add(key, new SplitData(sequence, fileName));
				result = true;
			}
			Sequence sequence2 = node.GetSequence("Triggers");
			if (sequence2 != null)
			{
				Mapping mapping = new Mapping(string.Empty, new Scalar("Name", text));
				mapping.Add(sequence2);
				_splitsData[key].AddTriggerSet(text, mapping);
				ParseAnimations(_splitsData[key], sequence2);
			}
		}
		return result;
	}

	private static void ParseAnimations(SplitData splitData, Sequence triggers)
	{
		foreach (Mapping trigger in triggers)
		{
			Sequence sequence = trigger.GetSequence("Actions");
			if (sequence == null)
			{
				continue;
			}
			foreach (Node item in sequence)
			{
				Mapping mapping2 = item as Mapping;
				if (mapping2 == null)
				{
					continue;
				}
				if (mapping2.nodesInside[0].key.Equals("Animation"))
				{
					string text = GetText((Mapping)mapping2.GetNodesByIndex(0), "Name").ToLower();
					if (_animationsSettings.ContainsKey(text))
					{
						splitData.AddAnimation(text, _animationsSettings[text]);
					}
				}
				else
				{
					if (!mapping2.nodesInside[0].key.Equals("AnimationRandom"))
					{
						continue;
					}
					foreach (object item2 in (Mapping)mapping2.nodesInside[0])
					{
						Sequence sequence2 = item2 as Sequence;
						if (sequence2 == null)
						{
							continue;
						}
						for (int i = 0; i < sequence2.nodesInside.Count; i++)
						{
							string text2 = sequence2.nodesInside[i].value.ToString().ToLower();
							if (_animationsSettings.ContainsKey(text2))
							{
								splitData.AddAnimation(text2, _animationsSettings[text2]);
							}
						}
					}
				}
			}
		}
	}

	private static void Save()
	{
		FilesUtil.DeleteDirectory(SplitDir);
		SaveSplit();
		SaveHeader();
		PackingArchive();
	}

	private static void SaveSplit()
	{
		foreach (KeyValuePair<int, SplitData> splitsDatum in _splitsData)
		{
			YamlDocumentNekki yamlDocumentNekki = YamlDocumentNekki.FromYamlContent("Root:");
			Sequence triggersSet = splitsDatum.Value.GetTriggersSet();
			if (triggersSet != null)
			{
				yamlDocumentNekki.GetRoot().Add(triggersSet);
			}
			Sequence animations = splitsDatum.Value.GetAnimations();
			if (animations != null)
			{
				yamlDocumentNekki.GetRoot().Add(animations);
			}
			yamlDocumentNekki.SaveToFile(SplitDir + "/" + splitsDatum.Value.FileName + ".yaml");
		}
	}

	private static void SaveHeader()
	{
		YamlDocumentNekki yamlDocumentNekki = YamlDocumentNekki.FromYamlContent("Root:");
		List<Node> list = new List<Node>();
		foreach (SplitData value in _splitsData.Values)
		{
			Mapping mapping = new Mapping(string.Empty, new Scalar("FileName", value.FileName));
			mapping.Add(value.Locks);
			list.Add(mapping);
		}
		yamlDocumentNekki.GetRoot().Add(new Sequence("SplitFiles", list));
		yamlDocumentNekki.SaveToFile(SplitDir + "/HEADER.yaml");
	}

	private static void PackingArchive()
	{
		ZipUtils.SaveZipFromDirectory(SplitDir, GlobalPath.ApplicationPath + "/" + SplitZipName, CompressionLevel.None);
		FilesUtil.DeleteDirectory(SplitDir);
	}

	private static string GetText(Mapping node, string name)
	{
		Scalar text = node.GetText(name);
		if (text != null)
		{
			return text.text;
		}
		return null;
	}

	private static string GetResultFile(string result)
	{
		return GlobalPath.ExternalPath + "/" + result + ".yaml";
	}
}
