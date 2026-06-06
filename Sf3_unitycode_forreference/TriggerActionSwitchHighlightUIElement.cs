using System.Collections.Generic;
using Nekki.Yaml;
using SF3.Moves;
using UnityEngine;

public class TriggerActionSwitchHighlightUIElement : TriggerActionQuest
{
	private List<string> _ids;

	public TriggerActionSwitchHighlightUIElement(Node yamlNode)
		: base(EActionType.SWITCH_HIGHLIGHT_UI_ELEMENT, yamlNode)
	{
		_ids = new List<string>();
		Mapping mapping = yamlNode as Mapping;
		if (mapping == null)
		{
			return;
		}
		Mapping mapping2 = mapping.GetMapping("SwitchHighlightUIElement");
		Scalar text = mapping2.GetText("ID");
		if (text != null)
		{
			_ids.Add(text.text);
			return;
		}
		Sequence sequence = mapping2.GetSequence("ID");
		if (sequence == null)
		{
			return;
		}
		foreach (Node item in sequence.nodesInside)
		{
			_ids.Add(item.ToString());
		}
	}

	protected override void ApplyAction(object modelData)
	{
		if (_ids.Count > 0)
		{
			TutorialManager.Instance.SetLayer(new TutorialManager.SetLayerCallData(_ids, null, false, new Vector2(0f, 0f), null));
		}
		else
		{
			TutorialManager.Instance.Release(null);
		}
	}
}
