using UnityEngine;

[ExecuteInEditMode]
public class AdvancedUIScrollBar : UIScrollBar
{
	public bool disableKeys;

	protected new void OnKey(KeyCode key)
	{
		if (!disableKeys)
		{
			base.OnKey(key);
		}
	}
}
