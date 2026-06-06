using System;
using System.Collections.Generic;
using SF3;
using UnityEngine;
using UnityEngine.UI;

public class LoginEmailDialogController : MonoBehaviour
{
	[SerializeField]
	private Button _sendButton;

	[SerializeField]
	private InputField _inputField;

	private NekkiUIModule _module;

	private bool _pauseScheduled;

	public event Action OnDialogClosed;

	private event Action<string> OnSendDelegate;

	private void Start()
	{
		_inputField.onValueChanged.AddListener(OnMessageChanged);
		_sendButton.onClick.AddListener(OnSendPressed);
		SetSendButtonActive(false);
	}

	private void OnDestroy()
	{
		if (_pauseScheduled)
		{
			LoadScreen.Instance.UnsheduleOnHideLoader(PauseBattle);
		}
	}

	public static LoginEmailDialogController ShowDialog(Action<string> getEmailDelegate)
	{
		ScreenTexture.Instance.SetTexture("LoginEmailDialogController", ScreenTexture.TextureOutputCamera.Main, ScreenTexture.TextureOutputFilter.Blur);
		NekkiUIModule nekkiUIModule = NekkiUIRootModules.Instance.MountNativeModule("LoginEmailDialog");
		LoginEmailDialogController component = nekkiUIModule.GetComponent<LoginEmailDialogController>();
		component._module = nekkiUIModule;
		component.OnSendDelegate = getEmailDelegate;
		if (LoadScreen.LoaderVisible)
		{
			LoadScreen.Instance.SheduleOnHideLoader(component.PauseBattle);
			component._pauseScheduled = true;
		}
		else
		{
			component.PauseBattle();
		}
		List<GameObject> list = new List<GameObject>();
		list.Add(component._inputField.gameObject);
		list.Add(component._sendButton.gameObject);
		List<GameObject> ignoredObj = list;
		UIBlocker.Instance.Block(ignoredObj, UIBlocker.Priority.LoginEmailDialog);
		return component;
	}

	private void PauseBattle()
	{
		if (BattleKeyManager.Instance != null && ModelsManager.Instance != null)
		{
			BattleController.SystemPause();
		}
	}

	private void OnMessageChanged(string text)
	{
		UpdateSendButton();
	}

	private void UpdateSendButton()
	{
		SetSendButtonActive(!_inputField.text.IsNullOrEmpty());
	}

	private void SetSendButtonActive(bool state)
	{
		_sendButton.interactable = state;
		if (state)
		{
			_sendButton.GetComponentInChildren<UnityEngine.UI.Text>().color = Color.white;
		}
		else
		{
			_sendButton.GetComponentInChildren<UnityEngine.UI.Text>().color = new Color(1f, 1f, 1f, 0.6f);
		}
	}

	private void OnSendPressed()
	{
		this.OnSendDelegate.InvokeSafe(_inputField.text.ToLower());
		CloseDialog();
	}

	public void CloseDialog()
	{
		this.OnDialogClosed.InvokeSafe();
		ScreenTexture.Instance.Clear("LoginEmailDialogController");
		NekkiUIRootModules.Instance.UnmountModule(_module);
		UIBlocker.Instance.Unblock(UIBlocker.Priority.LoginEmailDialog);
		if (BattleKeyManager.Instance != null && ModelsManager.Instance != null)
		{
			BattleController.SystemResume();
		}
	}
}
