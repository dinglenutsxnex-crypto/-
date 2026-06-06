using System.Collections.Generic;
using System.IO;
using Ionic.Crc;
using Ionic.Zip;
using Nekki.Yaml;
using SF3.GameModels;
using SF3.Moves;

public class SplitController
{
	private ZipFile _archive;

	private Dictionary<string, List<IConditionEqual>> _headerLocks;

	private Dictionary<string, MovesData> _movesData;

	private List<string> _movesLoad;

	public void Init()
	{
		_headerLocks = new Dictionary<string, List<IConditionEqual>>();
		_movesData = new Dictionary<string, MovesData>();
		_movesLoad = new List<string>();
		InitArchive();
		InitHeader();
	}

	private void InitArchive()
	{
		byte[] splitMoves = ConfigsSourceResolver.SplitMoves;
		MemoryStream zipStream = new MemoryStream(splitMoves);
		_archive = ZipFile.Read(zipStream);
	}

	private void InitHeader()
	{
		string fileFromArchive = GetFileFromArchive("HEADER.yaml");
		Mapping root = YamlDocumentNekki.FromYamlContent(fileFromArchive).GetRoot();
		Sequence sequence = root.GetSequence("SplitFiles");
		foreach (Mapping item in sequence.nodesInside)
		{
			string text = item.GetText("FileName").text;
			List<IConditionEqual> value = Condition.Parse(item.GetSequence("Locks"));
			_headerLocks.Add(text, value);
		}
	}

	public List<InfoTriggerSet> LoadTriggers(Model model)
	{
		List<InfoTriggerSet> list = new List<InfoTriggerSet>();
		foreach (KeyValuePair<string, List<IConditionEqual>> headerLock in _headerLocks)
		{
			if (TriggersVerification.CheckConditionsEqual(model, null, headerLock.Value))
			{
				string key = headerLock.Key;
				if (!_movesData.ContainsKey(key))
				{
					_movesData.Add(key, ParseSplit(key + ".yaml"));
				}
				if (!_movesLoad.Contains(key))
				{
					_movesLoad.Add(key);
				}
				list.AddRange(_movesData[key].Triggers.Values);
			}
		}
		return list;
	}

	public MovesData ParseSplit(string fileName)
	{
		string fileFromArchive = GetFileFromArchive(fileName);
		if (!fileFromArchive.IsNullOrEmpty())
		{
			MovesData movesData = new MovesData();
			MovesParser.ParseAllFromContent(fileName, fileFromArchive, movesData.Animations, movesData.Binaries, movesData.Triggers);
			return movesData;
		}
		return null;
	}

	public InfoAnimation GetAnimationByName(string name)
	{
		foreach (MovesData value in _movesData.Values)
		{
			if (value.Animations.ContainsKey(name))
			{
				return value.Animations[name];
			}
		}
		return null;
	}

	public AnimationBinaries GetBinariesByName(string name)
	{
		foreach (MovesData value in _movesData.Values)
		{
			if (value.Binaries.ContainsKey(name))
			{
				return value.Binaries[name];
			}
		}
		return null;
	}

	public List<InfoAnimation> GetAnimations()
	{
		List<InfoAnimation> list = new List<InfoAnimation>();
		foreach (MovesData value in _movesData.Values)
		{
			list.AddRange(value.Animations.Values);
		}
		return list;
	}

	public void UnloadUnusedTriggers()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, MovesData> movesDatum in _movesData)
		{
			if (!_movesLoad.Contains(movesDatum.Key))
			{
				list.Add(movesDatum.Key);
			}
		}
		foreach (string item in list)
		{
			_movesData.Remove(item);
		}
		_movesLoad.Clear();
		GlobalLoad.GCCollectImmediately();
	}

	private string GetFileFromArchive(string name)
	{
		if (_archive.ContainsEntry(name))
		{
			ZipEntry zipEntry = _archive[name];
			CrcCalculatorStream stream = zipEntry.OpenReader();
			using (StreamReader streamReader = new StreamReader(stream))
			{
				return streamReader.ReadToEnd();
			}
		}
		return null;
	}
}
