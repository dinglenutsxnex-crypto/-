using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugDevelopmentServerChooser : DebugGOLineController
{
	[SerializeField]
	private UIGrid _grid;

	[SerializeField]
	private DebugDeveloperServerButton _choseButtonPrototype;

	[SerializeField]
	private int _servers;

	private List<DebugDeveloperServerButton> _buttons = new List<DebugDeveloperServerButton>();

	public event Action<int> DevServerChosen;

	internal override void setup(Action<DebugGOLineController> onUpdate)
	{
		base.setup(onUpdate);
	}

	private void Start()
	{
		for (int i = 2; i <= _servers; i++)
		{
			DebugDeveloperServerButton debugDeveloperServerButton = UnityEngine.Object.Instantiate(_choseButtonPrototype);
			debugDeveloperServerButton.Init(i, ChooseServer);
			debugDeveloperServerButton.transform.SetParent(_choseButtonPrototype.transform.parent);
			debugDeveloperServerButton.transform.localScale = _choseButtonPrototype.transform.localScale;
			debugDeveloperServerButton.gameObject.SetActive(true);
			_buttons.Add(debugDeveloperServerButton);
		}
		NetworkConnection.current.OnNetworkEstablished += OnNetworkEstablished;
		NetworkConnection.current.OnNetworkCutoff += OnNetworkCutoff;
		OnNetworkEstablished();
		_grid.Reposition();
	}

	private void OnNetworkCutoff()
	{
		Select(NetworkConnection.current.CurrentConfigVersion.serverName);
	}

	private void OnNetworkEstablished()
	{
		Select(NetworkConnection.current.CurrentConfigVersion.serverName);
	}

	public void Select(string server)
	{
		foreach (DebugDeveloperServerButton button in _buttons)
		{
			button.HighLight(server);
		}
	}

	private void ChooseServer(int number)
	{
		if (this.DevServerChosen != null)
		{
			this.DevServerChosen(number);
		}
	}
}
