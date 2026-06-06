using System;
using UnityEngine;

public class DebugDeveloperServerButton : MonoBehaviour
{
	[SerializeField]
	private UILabel _serverName;

	private int _serverNum;

	private Action<int> _choseServerAction;

	private UIButton _button;

	[SerializeField]
	private Color _normalColor;

	[SerializeField]
	private Color _selectedColor;

	private void OnClick()
	{
		if (_choseServerAction != null)
		{
			_choseServerAction(_serverNum);
		}
	}

	public void Init(int serverNum, Action<int> chooseServer)
	{
		_serverNum = serverNum;
		base.gameObject.name = "DEV " + _serverNum.ToString("00");
		_serverName.text = base.gameObject.name;
		_choseServerAction = chooseServer;
		_button = GetComponent<UIButton>();
	}

	public void HighLight(string server)
	{
		bool flag = server == "dev" + _serverNum || server == "develop" + _serverNum;
		if (_button != null)
		{
			_button.defaultColor = ((!flag) ? _normalColor : _selectedColor);
		}
	}
}
