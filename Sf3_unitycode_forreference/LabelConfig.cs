using System.Collections.Generic;
using Nekki;
using Nekki.Yaml;
using UnityEngine;

public class LabelConfig
{
	public string Alias { get; protected set; }

	public string[] Format { get; protected set; }

	public Color? Color { get; protected set; }

	public string Atlas { get; protected set; }

	public bool Empty
	{
		get
		{
			return string.IsNullOrEmpty(Alias);
		}
	}

	public LabelConfig()
	{
		Alias = string.Empty;
		Format = new string[0];
	}

	public LabelConfig(string alias)
		: this()
	{
		Alias = alias;
	}

	public LabelConfig(Mapping config)
		: this()
	{
		if (config == null)
		{
			return;
		}
		foreach (Node item in config.nodesInside)
		{
			switch (item.key)
			{
			case "Alias":
				Alias = item.value.ToString();
				break;
			case "Format":
			{
				Mapping mapping = (Mapping)item;
				List<string> list = new List<string>();
				foreach (Node item2 in mapping.nodesInside)
				{
					list.Add(item2.value.ToString());
				}
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].Contains("/"))
					{
						string[] array = list[i].Split('/');
						Atlas = array[0];
						list[i] = array[1];
					}
				}
				Format = list.ToArray();
				break;
			}
			case "Color":
				Color = NekkiUtils.HexToColor(item.value.ToString());
				break;
			}
		}
	}

	public override string ToString()
	{
		return (!Empty) ? ("{" + string.Format("Alias = {0}, Format = {1}, Color = {2}", Alias, ConcatFormat(), Color) + "}") : "{EMPTY}";
	}

	protected string ConcatFormat()
	{
		string text = "(";
		string[] format = Format;
		foreach (string text2 in format)
		{
			text = text + text2 + " ";
		}
		return text.Trim() + ")";
	}
}
