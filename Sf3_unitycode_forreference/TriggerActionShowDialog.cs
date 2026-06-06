using System;
using Nekki.Yaml;
using SF3.Moves;

public class TriggerActionShowDialog : TriggerActionQuest
{
	private readonly Mapping _data;

	public TriggerActionShowDialog(Node yamlNode)
		: base(EActionType.SHOW_DIALOG, yamlNode, false)
	{
		_data = ((Mapping)yamlNode).GetMapping("Dialog");
	}

	protected override void ApplyAction(object modelData)
	{
		base.ApplyAction(modelData);
		ConfigurableDialogModule.onDialogClosed = (ConfigurableDialogModule.DialogClosed)Delegate.Combine(ConfigurableDialogModule.onDialogClosed, new ConfigurableDialogModule.DialogClosed(OnDialogClose));
		ConfigurableDialogModule.onDialogOpened = (ConfigurableDialogModule.DialogOpened)Delegate.Combine(ConfigurableDialogModule.onDialogOpened, new ConfigurableDialogModule.DialogOpened(OnDialogOpen));
		DialogsManager.Show(_data);
	}

	private void OnDialogClose(object obj)
	{
		ConfigurableDialogModule.onDialogClosed = (ConfigurableDialogModule.DialogClosed)Delegate.Remove(ConfigurableDialogModule.onDialogClosed, new ConfigurableDialogModule.DialogClosed(OnDialogClose));
		DialogsManager.IsDialog = false;
		if (obj != null)
		{
			string text = (string)obj;
			if (!text.IsNullOrEmpty())
			{
				QuestController.Instance.CallEvent(text);
			}
		}
		CloseAction();
	}

	private void OnDialogOpen(object obj)
	{
		ConfigurableDialogModule.onDialogOpened = (ConfigurableDialogModule.DialogOpened)Delegate.Remove(ConfigurableDialogModule.onDialogOpened, new ConfigurableDialogModule.DialogOpened(OnDialogOpen));
		DialogsManager.IsDialog = true;
	}
}
