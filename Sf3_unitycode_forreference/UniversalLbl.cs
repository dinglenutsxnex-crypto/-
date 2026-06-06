using System;
using Nekki.UI;
using UnityEngine.UI;

[Serializable]
public class UniversalLbl
{
	public NekkiUILabel label;

	public Text uiLabel;

	public string Text
	{
		get
		{
			if (label != null)
			{
				return label.text;
			}
			if (uiLabel != null)
			{
				return uiLabel.text;
			}
			return string.Empty;
		}
		set
		{
			if (label != null)
			{
				label.text = value;
			}
			else if (uiLabel != null)
			{
				uiLabel.text = value;
			}
		}
	}
}
