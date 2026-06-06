using UnityEngine;

public class VisualDebugUIExpandHolder : MonoBehaviour
{
	private VisualDebugUI.Unit _unit;

	public void SetUnit(VisualDebugUI.Unit unit)
	{
		_unit = unit;
	}

	private void Start()
	{
	}

	private void OnClick()
	{
		VisualDebugUI.Instance.OnExpand(_unit);
	}
}
