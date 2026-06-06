using UnityEngine;

public class SystemAlertButton : MonoBehaviour
{
	[SerializeField]
	private LocalizationText label;

	public void SetLabel(string alias)
	{
		label.SetAlias(alias);
	}
}
