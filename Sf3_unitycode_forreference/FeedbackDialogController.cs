using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SF3;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackDialogController : MonoBehaviour
{
	[SerializeField]
	private Button _laterButton;

	[SerializeField]
	private Button _sendButton;

	[SerializeField]
	private InputField _inputField;

	[SerializeField]
	private RectTransform _checkboxContainer;

	[SerializeField]
	private RectTransform _verticalContainer;

	[SerializeField]
	private GameObject _checkboxPrefab;

	private NekkiUIModule _module;

	private List<GameObject> _blockIgnoredObjects;

	private bool _show;

	public event Action OnDialogClosed;

	public static FeedbackDialogController ShowDialog()
	{
		ScreenTexture.Instance.SetTexture("FeedbackDialogController", ScreenTexture.TextureOutputCamera.Main, ScreenTexture.TextureOutputFilter.Blur);
		NekkiUIModule nekkiUIModule = NekkiUIRootModules.Instance.MountNativeModule("FeedbackDialog");
		FeedbackDialogController component = nekkiUIModule.GetComponent<FeedbackDialogController>();
		component.Init(nekkiUIModule);
		return component;
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
		_blockIgnoredObjects.Add(_inputField.gameObject);
		_blockIgnoredObjects.Add(_laterButton.gameObject);
		_blockIgnoredObjects.Add(_sendButton.gameObject);
		if (LoadScreen.LoaderVisible)
		{
			LoadScreen.Instance.SheduleOnHideLoader(BlockAndPause);
		}
		else
		{
			BlockAndPause();
		}
		_show = true;
	}

	public void AddCheckBox(string alias)
	{
		if (_show)
		{
			Debug.LogError("The dialog has already been created");
			return;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(_checkboxPrefab);
		gameObject.transform.SetParent(_checkboxContainer, false);
		gameObject.GetComponentInChildren<LocalizationText>().SetAlias(alias);
		gameObject.GetComponentInChildren<Toggle>().onValueChanged.AddListener(OnChecked);
		_blockIgnoredObjects.Add(gameObject);
	}

	private void OnChecked(bool isChecked)
	{
		UpdateSendButton();
	}

	private void Init(NekkiUIModule module)
	{
		_module = module;
		_blockIgnoredObjects = new List<GameObject>();
		base.gameObject.SetActive(false);
	}

	private void BlockAndPause()
	{
		UIBlocker.Instance.Block(_blockIgnoredObjects, UIBlocker.Priority.Feedback);
		BattleController.SystemPause();
	}

	private void Start()
	{
		_inputField.onValueChanged.AddListener(OnMessageChanged);
		_laterButton.onClick.AddListener(CloseDialog);
		_sendButton.onClick.AddListener(OnSendPressed);
		SetSendButtonActive(false);
	}

	private void OnMessageChanged(string text)
	{
		UpdateSendButton();
	}

	private void UpdateSendButton()
	{
		bool flag = !_inputField.text.IsNullOrEmpty();
		flag |= IsAnythingChecked();
		SetSendButtonActive(flag);
	}

	private bool IsAnythingChecked()
	{
		Toggle[] componentsInChildren = _checkboxContainer.GetComponentsInChildren<Toggle>();
		Toggle[] array = componentsInChildren;
		foreach (Toggle toggle in array)
		{
			if (toggle.isOn)
			{
				return true;
			}
		}
		return false;
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

	private JObject GetCheckedAliases()
	{
		JObject jObject = new JObject();
		Toggle[] componentsInChildren = _checkboxContainer.GetComponentsInChildren<Toggle>();
		Toggle[] array = componentsInChildren;
		foreach (Toggle toggle in array)
		{
			jObject[toggle.GetComponentInChildren<LocalizationText>().GetAlias()] = toggle.isOn;
		}
		return jObject;
	}

	private void OnSendPressed()
	{
		JObject jObject = new JObject();
		jObject["message"] = _inputField.text;
		jObject["checked"] = GetCheckedAliases();
		Analytics.Logger.AddEvent("CLIENT_FEEDBACK", jObject);
		CloseDialog();
	}

	private void CloseDialog()
	{
		this.OnDialogClosed.InvokeSafe();
		ScreenTexture.Instance.Clear("FeedbackDialogController");
		NekkiUIRootModules.Instance.UnmountModule(_module);
		UIBlocker.Instance.Unblock(UIBlocker.Priority.Feedback);
		BattleController.SystemResume();
	}
}
