using UnityEngine;

public class ButtonLabelHighlight : MonoBehaviour
{
	public UILabel label;

	public Color highlightColor;

	private string selfComponentId;

	private Color oldColor;

	private bool setColor;

	private void Start()
	{
		TutorialManager.Instance.onSet += Highlight;
		TutorialManager.Instance.onRelease += Unhighlight;
		selfComponentId = GetComponent<TutorialComponent>().id;
	}

	public void Highlight(string id)
	{
		if (!(id != selfComponentId))
		{
			oldColor = label.color;
			label.color = highlightColor;
			setColor = true;
		}
	}

	public void Unhighlight(string id)
	{
		if (setColor)
		{
			label.color = oldColor;
			setColor = false;
		}
	}
}
