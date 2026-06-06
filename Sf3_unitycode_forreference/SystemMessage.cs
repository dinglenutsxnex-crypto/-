using System.Collections.Generic;
using SF3.Items;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SystemMessage : MonoBehaviour
{
	private const string defaultTitleAlias = "system_message_title";

	[SerializeField]
	private LocalizationText _title;

	[SerializeField]
	private LocalizationText _message;

	[SerializeField]
	private GameObject _buttonPrf;

	[SerializeField]
	private RectTransform _buttonsContainer;

	private NekkiUIModule _module;

	private float _showTime;

	private List<GameObject> _buttons;

	private UIBlocker.Priority _priority = UIBlocker.Priority.Min;

	public static SystemMessage ShowAlert(string msgAlias, string titleAlias = "system_message_title")
	{
		SystemMessage systemAlert = GetSystemAlert();
		systemAlert.SetMessage(msgAlias);
		systemAlert.SetTitle(titleAlias);
		return systemAlert;
	}

	public static SystemMessage ShowAlert(string msgAlias, string[] replacement, string titleAlias = "system_message_title")
	{
		SystemMessage systemAlert = GetSystemAlert();
		systemAlert.SetMessage(msgAlias, replacement);
		systemAlert.SetTitle(titleAlias);
		return systemAlert;
	}

	public static SystemMessage ShowAlert(string msgAlias, string[] msgReplacement, string titleAlias, string[] titleReplacement)
	{
		SystemMessage systemAlert = GetSystemAlert();
		systemAlert.SetMessage(msgAlias, msgReplacement);
		systemAlert.SetTitle(titleAlias, titleReplacement);
		return systemAlert;
	}

	private static SystemMessage GetSystemAlert()
	{
		NekkiUIModule nekkiUIModule = NekkiUIRootModules.Instance.MountNativeModule("SystemAlert");
		SystemMessage component = nekkiUIModule.GetComponent<SystemMessage>();
		InventoryManager.DisableReelCameraIfExists();
		component.Init(nekkiUIModule);
		return component;
	}

	private void Init(NekkiUIModule module)
	{
		_module = module;
		base.gameObject.SetActive(false);
		_buttons = new List<GameObject>();
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
		UIBlocker.Instance.Block(_buttons, _priority);
		_showTime = Time.unscaledTime;
	}

	public SystemMessage AddButton(string textAlias, UnityAction callback = null)
	{
		GameObject gameObject = Object.Instantiate(_buttonPrf);
		gameObject.transform.SetParent(_buttonsContainer, false);
		Button component = gameObject.GetComponent<Button>();
		component.onClick.AddListener(delegate
		{
			OnCloseButtonClick(textAlias);
		});
		if (callback != null)
		{
			component.onClick.AddListener(callback);
		}
		SystemAlertButton component2 = gameObject.GetComponent<SystemAlertButton>();
		component2.SetLabel(textAlias);
		_buttons.Add(gameObject);
		return this;
	}

	public SystemMessage SetBlockPriority(UIBlocker.Priority priority)
	{
		_priority = priority;
		return this;
	}

	public void SetMessage(string alias)
	{
		_message.SetAlias(alias);
	}

	public void SetMessage(string alias, string[] replacement)
	{
		_message.SetAlias(alias, replacement);
	}

	public void SetTitle(string alias)
	{
		_title.SetAlias(alias);
	}

	public void SetTitle(string alias, string[] replacement)
	{
		_title.SetAlias(alias, replacement);
	}

	private void OnCloseButtonClick(string buttonAlias)
	{
		int time = (int)((Time.unscaledTime - _showTime) * 1000f);
		SF3UiLogger.instance.AddDialogCloseEvent(_message.GetAlias(), buttonAlias, time);
		NekkiUIRootModules.Instance.UnmountModule(_module);
		UIBlocker.Instance.Unblock(_priority);
		InventoryManager.EnableReelCameraIfExists();
	}

	public void SetTextColor(Color color)
	{
		_title.SetColor(color);
		_message.SetColor(color);
	}
}
