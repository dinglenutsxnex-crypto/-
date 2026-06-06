using System;
using UnityEngine;

public class DebugCurrentServerController : DebugGOLineController
{
	[SerializeField]
	private Color normalColor;

	[SerializeField]
	private Color selectedColor;

	[SerializeField]
	private UIButton[] buttons;

	private DebugUI debugUI;

	internal override void setup(Action<DebugGOLineController> onUpdate)
	{
		base.setup(onUpdate);
		NetworkConnection.current.OnNetworkEstablished -= OnNetworkEstablished;
		NetworkConnection.current.OnNetworkEstablished += OnNetworkEstablished;
		NetworkConnection.current.OnNetworkCutoff -= OnNetworkCutoff;
		NetworkConnection.current.OnNetworkCutoff += OnNetworkCutoff;
		OnNetworkEstablished();
		string @string = PlayerPrefs.GetString("current_server", string.Empty);
		if (@string.Length > 0)
		{
			ChangeButtonColor(GetButton(buttons, GetButtonNum(@string)));
		}
		debugUI = GetComponentInParent<DebugUI>();
	}

	private static int GetButtonNum(string name)
	{
		int num = -1;
		switch (name)
		{
		case "prod":
			return 0;
		case "stable":
			return 1;
		case "qa":
			return 2;
		case "dev":
		case "develop":
			return 3;
		default:
			if (name.Contains("dev") && name.Length > 3)
			{
				return 4;
			}
			return -1;
		}
	}

	public UIButton GetButton(UIButton[] buttons, int number)
	{
		return (0 > number || number > buttons.Length) ? null : buttons[number];
	}

	public void OnProdPressed()
	{
		ChangeCurrentServer("prod");
	}

	public void OnStablePressed()
	{
		ChangeCurrentServer("stable");
	}

	public void OnQAPressed()
	{
		ChangeCurrentServer("qa");
	}

	public void OnDevPressed()
	{
		ChangeCurrentServer("dev");
	}

	public void OnDevNPressed()
	{
		debugUI.ToggleDevServerChooser();
	}

	public void OnDevServerChosen(int number)
	{
		ChangeButtonColor(GetButton(buttons, GetButtonNum("devN")));
		PlayerPrefs.SetString("current_server", "dev" + number);
		NetworkConnection.current.RestartConnection("Switching server", false);
	}

	private void disableAllButtons()
	{
		UIButton[] array = buttons;
		foreach (UIButton uIButton in array)
		{
			uIButton.defaultColor = normalColor;
		}
	}

	private void ChangeCurrentServer(string name)
	{
		ChangeButtonColor(GetButton(buttons, GetButtonNum(name)));
		PlayerPrefs.SetString("current_server", name);
		NetworkConnection.current.RestartConnection("changing server", false);
	}

	private void ChangeButtonColor(UIButtonColor button)
	{
		disableAllButtons();
		if (button != null)
		{
			button.defaultColor = selectedColor;
		}
	}

	private void OnNetworkEstablished()
	{
		string serverName = NetworkConnection.current.CurrentConfigVersion.serverName;
		ChangeButtonColor(GetButton(buttons, GetButtonNum(serverName)));
	}

	private void OnNetworkCutoff()
	{
		disableAllButtons();
	}

	private void OnDestroy()
	{
		NetworkConnection.current.OnNetworkEstablished -= OnNetworkEstablished;
		NetworkConnection.current.OnNetworkCutoff -= OnNetworkCutoff;
	}
}
