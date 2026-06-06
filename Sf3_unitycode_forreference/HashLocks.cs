using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nekki.Yaml;
using UnityEngine;

public class HashLocks
{
	public static int CreateHash(Sequence sequence)
	{
		Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();
		CollectValues(sequence.nodesInside, data, string.Empty);
		return BuildHash(data);
	}

	private static void CollectValues(List<Node> nodes, Dictionary<string, List<string>> data, string currentKey)
	{
		for (int i = 0; i < nodes.Count; i++)
		{
			Scalar scalar = nodes[i] as Scalar;
			if (scalar != null)
			{
				if (!data.ContainsKey(currentKey + scalar.key))
				{
					data.Add(currentKey + scalar.key, new List<string>());
				}
				data[currentKey + scalar.key].Add(scalar.text);
				continue;
			}
			Sequence sequence = nodes[i] as Sequence;
			if (sequence != null)
			{
				CollectValues(sequence.nodesInside, data, currentKey + nodes[i].key);
				continue;
			}
			Mapping mapping = nodes[i] as Mapping;
			if (mapping != null)
			{
				CollectValues(mapping.nodesInside, data, currentKey + nodes[i].key);
			}
			else
			{
				Debug.LogError(string.Format("Unsupported type found! type: [{0}]", nodes[i].GetType().Name));
			}
		}
	}

	private static int BuildHash(Dictionary<string, List<string>> data)
	{
		StringBuilder stringBuilder = new StringBuilder();
		List<string> list = data.Keys.ToList();
		list.Sort();
		for (int i = 0; i < list.Count; i++)
		{
			stringBuilder.Append(list[i]);
			data[list[i]].Sort();
			for (int j = 0; j < data[list[i]].Count; j++)
			{
				stringBuilder.Append(data[list[i]][j]);
			}
		}
		return stringBuilder.ToString().GetHashCode();
	}
}
