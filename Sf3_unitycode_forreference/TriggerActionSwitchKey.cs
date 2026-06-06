using Nekki.Yaml;
using SF3;
using SF3.KeyPressInfo;
using SF3.Moves;

public class TriggerActionSwitchKey : TriggerAction
{
	private readonly EQuadrants[] _switchKeys;

	public bool Enable;

	public bool Hide;

	public TriggerActionSwitchKey(Node yamlNode)
		: base(EActionType.SWITCH_KEY, yamlNode)
	{
		Mapping mapping = ((Mapping)yamlNode).GetMapping("SwitchKey");
		Node node = mapping.GetNode("State");
		if (node != null)
		{
			Enable = node.ToString().Equals("On");
		}
		Node node2 = mapping.GetNode("Hide");
		if (node2 != null)
		{
			Hide = node2.ToString().Equals("1");
		}
		Sequence sequence = mapping.GetSequence("Keys");
		if (sequence != null)
		{
			_switchKeys = new EQuadrants[sequence.nodesInside.Count];
			for (int i = 0; i < sequence.nodesInside.Count; i++)
			{
				_switchKeys[i] = KeyData.GetBattleCodeByName(sequence.nodesInside[i].ToString());
			}
		}
	}

	protected override void ApplyAction(object modelData)
	{
		base.ApplyAction(modelData);
		for (int i = 0; i < _switchKeys.Length; i++)
		{
			BattleKeyManager.Instance.ActivateBattleKey(_switchKeys[i], Enable);
			ActionButtons.Instance.ActionButtonHide(_switchKeys[i], Hide);
		}
	}
}
