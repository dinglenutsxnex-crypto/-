using System.Collections.Generic;
using SF3;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ConfigurableDialogModule))]
public class ConfigurableDialogContent : MonoBehaviour
{
	[SerializeField]
	private LocalizationText _title;

	[SerializeField]
	private GameObject _buttons;

	[SerializeField]
	private GameObject _buttonPrf;

	public List<LocalizationText> labels = new List<LocalizationText>();

	public List<Image> images = new List<Image>();

	private DialogConfig _config;

	private bool _pause = true;

	private float _showTime;

	private List<GameObject> _buttonsObject;

	public void Init(DialogConfig config)
	{
		_config = config;
		_buttonsObject = new List<GameObject>();
		if (_config == null)
		{
			Debug.LogError("ConfigurableDialogContent.Init: config is null!");
			return;
		}
		if (_pause)
		{
			BattleController.SystemPause();
		}
		InitTitle(config.title);
		InitLabels(config.labels);
		InitImages(config.images);
		InitButtons(config.buttons);
		ScreenTexture.Instance.SetTexture(base.name, config.outputCamera, ScreenTexture.TextureOutputFilter.Blur);
		UIBlocker.Instance.Block(_buttonsObject, UIBlocker.Priority.Dialogs);
		_showTime = Time.unscaledTime;
	}

	private void InitTitle(LabelConfig config)
	{
		if (!config.Empty && (bool)_title)
		{
			_title.SetAlias(config.Alias);
			if (config.Color.HasValue)
			{
				_title.gameObject.GetComponent<UnityEngine.UI.Text>().color = config.Color.Value;
			}
		}
	}

	private void InitLabels(List<LabelConfig> configs)
	{
		if (labels == null)
		{
			return;
		}
		for (int i = 0; i < configs.Count && i < labels.Count; i++)
		{
			if (!configs[i].Empty)
			{
				labels[i].SetAlias(configs[i].Alias, configs[i].Format);
				if (configs[i].Color.HasValue)
				{
					labels[i].gameObject.GetComponent<UnityEngine.UI.Text>().color = configs[i].Color.Value;
				}
			}
			labels[i].gameObject.SetActive(!configs[i].Empty);
		}
	}

	private void InitImages(List<ImageConfig> configs)
	{
		if (images == null)
		{
			return;
		}
		for (int i = 0; i < configs.Count && i < images.Count; i++)
		{
			Image image = images[i];
			if (!configs[i].Empty)
			{
				Sprite loadSprite = GlobalLoad.GetLoadSprite(configs[i].Texture);
				RectTransform component = image.GetComponent<RectTransform>();
				image.sprite = loadSprite;
				image.color = configs[i].Color;
				component.sizeDelta = new Vector2(component.rect.height * loadSprite.rect.width / loadSprite.rect.height, component.rect.height);
			}
			image.gameObject.SetActive(!configs[i].Empty);
		}
	}

	private void InitButtons(List<ButtonConfig> configs)
	{
		for (int i = 0; i < configs.Count; i++)
		{
			DialogButton dialogButton = CreateButton();
			if (!configs[i].Empty)
			{
				dialogButton.SetLabel(configs[i].Alias, configs[i].Arrow);
				dialogButton.SetColor(configs[i].Color);
				dialogButton.AddCallback(CloseThis, configs[i].Callback);
				_buttonsObject.Add(dialogButton.gameObject);
			}
			dialogButton.gameObject.SetActive(!configs[i].Empty);
		}
	}

	private DialogButton CreateButton()
	{
		GameObject gameObject = Object.Instantiate(_buttonPrf);
		gameObject.transform.SetParent(_buttons.transform, false);
		return gameObject.GetComponent<DialogButton>();
	}

	private void CloseThis(string buttonAlias, object obj)
	{
		ScreenTexture.Instance.Clear(base.name);
		int time = (int)((Time.unscaledTime - _showTime) * 1000f);
		SF3UiLogger.instance.AddDialogCloseEvent(GetAliasID(), buttonAlias, time);
		UIBlocker.Instance.Unblock(UIBlocker.Priority.Dialogs);
		SlideMenu.isEnabled = true;
		NekkiUIRootModules.Instance.UnmountModule(_config.type);
		if (_pause)
		{
			BattleController.SystemResume();
		}
		if (ConfigurableDialogModule.onDialogClosed != null)
		{
			ConfigurableDialogModule.onDialogClosed(obj);
		}
		PauseWindow.IncrementShowCounter();
	}

	private string GetAliasID()
	{
		string text = string.Empty;
		bool flag = true;
		foreach (LabelConfig label in _config.labels)
		{
			if (flag)
			{
				text += label.Alias;
				flag = false;
			}
			else
			{
				text = text + " " + label.Alias;
			}
		}
		return text;
	}
}
