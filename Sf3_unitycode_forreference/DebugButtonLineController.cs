using System;
using UnityEngine;

public class DebugButtonLineController : DebugLineController
{
	[SerializeField]
	public UIButton button;

	[SerializeField]
	private UILabel textLabel;

	private Action onClick;

	internal void setup(string text, Action _onClick)
	{
		onClick = _onClick;
		textLabel.text = text;
	}

	public void onButtonPressed()
	{
		onClick();
	}
}
