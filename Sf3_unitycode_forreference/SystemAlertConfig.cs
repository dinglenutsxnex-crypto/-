using System.Collections.Generic;
using System.Text;
using Nekki.Yaml;

public class SystemAlertConfig
{
	public string type { get; protected set; }

	public LabelConfig title { get; protected set; }

	public List<LabelConfig> labels { get; protected set; }

	public List<ButtonConfig> buttons { get; protected set; }

	public string defaultCallback { get; protected set; }

	public bool pause { get; protected set; }

	public SystemAlertConfig()
	{
		type = string.Empty;
		defaultCallback = string.Empty;
		title = new LabelConfig();
		labels = new List<LabelConfig>();
		buttons = new List<ButtonConfig>();
	}

	public SystemAlertConfig(Mapping configSource)
		: this()
	{
		if (configSource == null)
		{
			return;
		}
		foreach (Node item in configSource.nodesInside)
		{
			switch (item.key)
			{
			case "Type":
				type = item.value.ToString();
				break;
			case "Content":
				foreach (Node item2 in ((Mapping)item).nodesInside)
				{
					switch (item2.key)
					{
					case "Title":
						title = new LabelConfig((Mapping)item2);
						break;
					case "Buttons":
						foreach (Node item3 in ((Sequence)item2).nodesInside)
						{
							buttons.Add(new ButtonConfig((Mapping)item3));
						}
						break;
					case "Labels":
						foreach (Node item4 in ((Sequence)item2).nodesInside)
						{
							labels.Add(new LabelConfig((Mapping)item4));
						}
						break;
					}
				}
				break;
			}
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(string.Format("[Dlg:{0}({1})", type, title));
		stringBuilder.Append(" [labels: ");
		foreach (LabelConfig label in labels)
		{
			stringBuilder.Append(label);
		}
		stringBuilder.Append("] [buttons: ");
		foreach (ButtonConfig button in buttons)
		{
			stringBuilder.Append(button);
		}
		stringBuilder.Append("];");
		return stringBuilder.ToString();
	}
}
