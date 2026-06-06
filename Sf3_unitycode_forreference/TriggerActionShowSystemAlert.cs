using Nekki.Yaml;
using SF3.Moves;

public class TriggerActionShowSystemAlert : TriggerActionQuest
{
	private readonly Mapping _data;

	public TriggerActionShowSystemAlert(Node yamlNode)
		: base(EActionType.SHOW_SYSTEM_ALERT, yamlNode, false)
	{
		_data = ((Mapping)yamlNode).GetMapping("SystemAlert");
	}

	protected override void ApplyAction(object modelData)
	{
		base.ApplyAction(modelData);
		SystemAlertManager.Show(_data, base.CloseAction);
	}
}
