using System;
using UnityEngine;

public class DebugTextLineController : DebugLineController
{
	[SerializeField]
	public UILabel nameLabel;

	[SerializeField]
	public UILabel value;

	[SerializeField]
	public UILabel separator;

	private Action<DebugTextLineController> updateAction;

	internal void setup(string name, Action<DebugTextLineController> onUpdate)
	{
		nameLabel.text = name;
		updateAction = onUpdate;
	}

	internal override void onUpdate()
	{
		if (updateAction != null)
		{
			updateAction(this);
		}
	}

	internal void HideSeparator()
	{
		separator.alpha = 0f;
	}
}
