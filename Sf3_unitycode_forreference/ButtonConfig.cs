using Nekki.Yaml;

public class ButtonConfig : LabelConfig
{
	public ImageConfig Background { get; protected set; }

	public bool Arrow { get; protected set; }

	public string Callback { get; protected set; }

	public new bool Empty
	{
		get
		{
			return base.Empty && Background.Empty;
		}
	}

	public ButtonConfig(string buttonAlias)
	{
		Init();
		base.Alias = buttonAlias;
	}

	public ButtonConfig(Mapping config)
		: base(config)
	{
		Init();
		if (config == null)
		{
			return;
		}
		foreach (Node item in config.nodesInside)
		{
			switch (item.key)
			{
			case "Background":
				Background = new ImageConfig((Mapping)item);
				break;
			case "Arrow":
				Arrow = bool.Parse(item.value.ToString());
				break;
			case "CallID":
				Callback = item.value.ToString();
				break;
			}
		}
	}

	private void Init()
	{
		Background = new ImageConfig();
		Arrow = true;
		Callback = string.Empty;
	}

	public override string ToString()
	{
		return (!Empty) ? ("{" + base.ToString().Trim('{', '}') + string.Format(" Background = {0}, Callback = {1}", Background, Callback) + "}") : "{EMPTY}";
	}

	public void SetCallback(string defaultCallback)
	{
		Callback = defaultCallback;
	}
}
