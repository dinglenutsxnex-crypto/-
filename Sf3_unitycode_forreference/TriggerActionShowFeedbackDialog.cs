using System.Collections.Generic;
using Nekki.Yaml;
using SF3.Moves;

public class TriggerActionShowFeedbackDialog : TriggerActionQuest
{
	private List<string> aliases = new List<string>();

	public TriggerActionShowFeedbackDialog(Node yamlNode)
		: base(EActionType.FEEDBACK_DIALOG, yamlNode, false)
	{
		Mapping mapping = ((Mapping)yamlNode).GetMapping("FeedbackDialog");
		Sequence sequence = mapping.GetSequence("Checkboxes");
		foreach (Mapping item in sequence)
		{
			aliases.Add(item.GetText("Alias").ToString());
		}
	}

	protected override void ApplyAction(object modelData)
	{
		base.ApplyAction(modelData);
		FeedbackDialogController feedbackDialogController = FeedbackDialogController.ShowDialog();
		foreach (string alias in aliases)
		{
			feedbackDialogController.AddCheckBox(alias);
		}
		feedbackDialogController.OnDialogClosed += OnDialogClose;
		feedbackDialogController.Show();
	}

	private void OnDialogClose()
	{
		CloseAction();
	}
}
