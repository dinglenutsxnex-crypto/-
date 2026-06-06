using Nekki.Yaml;
using UnityEngine.Events;

public static class SystemAlertManager
{
	private static UnityAction _callback;

	internal static void Show(Mapping configSource, UnityAction callback)
	{
		_callback = callback;
		SystemAlertConfig systemAlertConfig = new SystemAlertConfig(configSource);
		SystemMessage systemMessage = SystemMessage.ShowAlert(systemAlertConfig.labels[0].Alias, systemAlertConfig.title.Alias);
		foreach (ButtonConfig button in systemAlertConfig.buttons)
		{
			systemMessage.AddButton(button.Alias, OnSystemAlertClose);
		}
		systemMessage.Show();
	}

	private static void OnSystemAlertClose()
	{
		if (_callback != null)
		{
			_callback();
		}
	}
}
