using System.Collections.Generic;
using System.Text;
using Nekki.Yaml;
using UnityEngine;

public class DialogConfig
{
	public string type { get; protected set; }

	public LabelConfig title { get; protected set; }

	public ImageConfig background { get; protected set; }

	public ImageConfig shade { get; protected set; }

	public List<ImageConfig> images { get; protected set; }

	public List<LabelConfig> labels { get; protected set; }

	public List<ButtonConfig> buttons { get; protected set; }

	public string defaultCallback { get; protected set; }

	public bool pause { get; protected set; }

	public ScreenTexture.TextureOutputCamera outputCamera { get; set; }

	public DialogConfig()
	{
		type = string.Empty;
		defaultCallback = string.Empty;
		pause = true;
		title = new LabelConfig();
		background = new ImageConfig();
		shade = new ImageConfig();
		images = new List<ImageConfig>();
		labels = new List<LabelConfig>();
		buttons = new List<ButtonConfig>();
		outputCamera = ScreenTexture.TextureOutputCamera.Main;
	}

	public DialogConfig(Mapping configSource)
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
					case "Pause":
						pause = item2.value.ToString().ToLower().Equals("true");
						break;
					case "Title":
						title = new LabelConfig((Mapping)item2);
						break;
					case "Background":
						background = new ImageConfig((Mapping)item2);
						break;
					case "Shade":
						shade = new ImageConfig((Mapping)item2);
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
					case "Images":
						foreach (Node item5 in ((Sequence)item2).nodesInside)
						{
							images.Add(new ImageConfig((Mapping)item5));
						}
						break;
					}
				}
				break;
			}
		}
	}

	public static DialogConfig GetConfig(string purpose, Dictionary<string, string> data)
	{
		if (purpose != null && purpose == "SHOW_DIALOG")
		{
			return GetShowDialogConfig(data);
		}
		Debug.LogWarning("No config found for key: [" + purpose.ToString() + "]. Creating default config...");
		return new DialogConfig();
	}

	private static DialogConfig GetShowDialogConfig(Dictionary<string, string> data)
	{
		DialogConfig dialogConfig = new DialogConfig();
		dialogConfig.type = data[EShowDialogParams.Prefab.ToString()];
		dialogConfig.title = new LabelConfig(data[EShowDialogParams.Title.ToString()]);
		dialogConfig.images.Add(new ImageConfig(data[EShowDialogParams.Image.ToString()]));
		dialogConfig.labels.Add(new LabelConfig(data[EShowDialogParams.Label.ToString()]));
		dialogConfig.buttons.Add(new ButtonConfig(data[EShowDialogParams.ButtonAlias.ToString()]));
		return dialogConfig;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(string.Format("[Dlg:{0}({1}) [back: {2}]", type, title, background));
		stringBuilder.Append(" [labels: ");
		foreach (LabelConfig label in labels)
		{
			stringBuilder.Append(label);
		}
		stringBuilder.Append("] [images: ");
		foreach (ImageConfig image in images)
		{
			stringBuilder.Append(image);
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
