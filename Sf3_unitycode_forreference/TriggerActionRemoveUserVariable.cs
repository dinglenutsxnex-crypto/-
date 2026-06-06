using Nekki.Yaml;
using SF3.Moves;
using SF3.UserData;
using UnityEngine;

public class TriggerActionRemoveUserVariable : TriggerActionQuest
{
	[SerializeField]
	public string VariableName;

	public TriggerActionRemoveUserVariable(Node node)
		: base(EActionType.REMOVE_USER_VARIABLE, node)
	{
		TryGetString(out VariableName, "Name", string.Empty, string.Empty, null, false);
	}

	protected override void ApplyAction(object modelData)
	{
		base.ApplyAction(modelData);
		UserManager.RemoveGlobalVariable(VariableName.AsRpn());
	}
}
