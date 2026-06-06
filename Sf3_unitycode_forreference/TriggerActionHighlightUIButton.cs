using System.Collections.Generic;
using Nekki.Yaml;
using SF3.Moves;
using UnityEngine;

internal class TriggerActionHighlightUIButton : TriggerActionQuest
{
	private List<string> _ids;

	private List<string> _notClickable;

	private string _draggableFrom = string.Empty;

	private string _draggableTo = string.Empty;

	private string _tag;

	private string _description;

	private Vector2 _offset;

	private string _pannelType;

	private string _animation;

	public TriggerActionHighlightUIButton(Node yamlNode)
		: base(EActionType.HIGHLIGHT_UI_BUTTON, yamlNode, false)
	{
		_ids = new List<string>();
		_notClickable = new List<string>();
		Mapping mapping = ((Mapping)yamlNode).GetMapping("HighlightUI");
		Sequence sequence = mapping.GetSequence("Clickable");
		if (sequence != null)
		{
			foreach (Node item in sequence.nodesInside)
			{
				_ids.Add(item.ToString());
			}
		}
		Sequence sequence2 = mapping.GetSequence("NonClickable");
		if (sequence2 != null)
		{
			foreach (Node item2 in sequence2.nodesInside)
			{
				_ids.Add(item2.ToString());
				_notClickable.Add(item2.ToString());
			}
		}
		Node node = mapping.GetNode("Draggable");
		if (node != null)
		{
			Mapping mapping2 = (Mapping)node;
			_draggableFrom = mapping2.GetText("From").ToString();
			_draggableTo = mapping2.GetText("To").ToString();
			if (!string.IsNullOrEmpty(_draggableFrom) && !string.IsNullOrEmpty(_draggableTo))
			{
				_ids.Add(_draggableFrom);
				_ids.Add(_draggableTo);
			}
		}
		Scalar text = mapping.GetText("Tag");
		if (text != null)
		{
			_tag = text.ToString();
		}
		TryGetVector2(out _offset, "Offset", string.Empty, null, false);
		TryGetString(out _pannelType, "Type", string.Empty, string.Empty);
		TryGetString(out _animation, "Animation", string.Empty, string.Empty, null, false);
		_description = mapping.GetText("Description").ToString();
	}

	protected override void ApplyAction(object modelData)
	{
		base.ApplyAction(modelData);
		TutorialManager.Instance.SetReleaseLayerCallback(base.CloseAction);
		TutorialManager.Instance.ShowPanel(new TutorialManager.OpenTutorialCallData(_pannelType, _description, new string[0], _animation, _offset));
		List<string> idTargets = ((!string.IsNullOrEmpty(_tag)) ? TutorialManager.Instance.GetIdsByTag(_tag) : _ids);
		TutorialManager.Instance.SetLayer(new TutorialManager.SetLayerCallData(idTargets, _description, true, _offset, _notClickable));
		TutorialManager.Instance.SetUnclickable(_notClickable);
	}
}
