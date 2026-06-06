using System;

public class TextWrapper : TextPic
{
	public Action onTextChage;

	public override string text
	{
		get
		{
			return base.text;
		}
		set
		{
			base.text = value;
			onTextChage.InvokeSafe();
		}
	}
}
