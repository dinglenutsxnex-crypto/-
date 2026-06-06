using Nekki.Yaml;
using SF3.KeyPressInfo;
using SF3.Moves;

public class TriggerActionHighlightKey : TriggerAction
{
	private readonly bool _toggleOn;

	private readonly EQuadrants[] _highlightKeys;

	public TriggerActionHighlightKey(Node yamlNode)
		: base(EActionType.HIHGLIGHT_KEY, yamlNode)
	{
		Mapping mapping = ((Mapping)yamlNode).GetMapping("HighlightKey");
		Sequence sequence = mapping.GetSequence("Keys");
		Scalar text = mapping.GetText("Toggle");
		if (sequence != null)
		{
			_highlightKeys = new EQuadrants[sequence.nodesInside.Count];
			for (int i = 0; i < sequence.nodesInside.Count; i++)
			{
				_highlightKeys[i] = KeyData.GetBattleCodeByName(sequence.nodesInside[i].ToString());
			}
		}
		if (text != null)
		{
			_toggleOn = text.text.Equals("On");
		}
	}

	protected override void ApplyAction(object modelData)
	{
		base.ApplyAction(modelData);
		Tutorial.SetTutor(_highlightKeys, _toggleOn);
	}
}
