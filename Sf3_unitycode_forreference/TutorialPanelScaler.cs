using System;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanelScaler : MonoBehaviour
{
	[SerializeField]
	private ImageWrapper[] _targets;

	[SerializeField]
	private TextWrapper _textLabel;

	[SerializeField]
	private Vector2 _offset;

	private void Start()
	{
		TextWrapper textLabel = _textLabel;
		textLabel.onTextChage = (Action)Delegate.Combine(textLabel.onTextChage, new Action(DoScale));
		DoScale();
	}

	private void OnDestroy()
	{
		TextWrapper textLabel = _textLabel;
		textLabel.onTextChage = (Action)Delegate.Remove(textLabel.onTextChage, new Action(DoScale));
	}

	[ContextMenu("DoScale")]
	private void DoScale()
	{
		float preferredSize = LayoutUtility.GetPreferredSize(_textLabel.rectTransform, 0);
		float preferredSize2 = LayoutUtility.GetPreferredSize(_textLabel.rectTransform, 1);
		ImageWrapper[] targets = _targets;
		foreach (ImageWrapper imageWrapper in targets)
		{
			imageWrapper.rectTransform.sizeDelta = new Vector2(preferredSize / (float)_targets.Length + _offset.x, preferredSize2 + _offset.y);
		}
	}
}
