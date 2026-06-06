using System.Collections.Generic;
using UnityEngine;

public class VisualDebugUiUnit : MonoBehaviour
{
	private static List<VisualDebugUiUnit> _units = new List<VisualDebugUiUnit>();

	public UIButton btnExpand;

	public UIButton btnUnExpand;

	public UIButton btnUnit;

	public UILabel lblUnitName;

	private VisualDebugUI.Unit _unit;

	public static void Clear()
	{
		foreach (VisualDebugUiUnit unit in _units)
		{
			Object.Destroy(unit.gameObject);
		}
		_units.Clear();
	}

	public void SetUnit(VisualDebugUI.Unit unit)
	{
		_unit = unit;
		_units.Add(this);
	}

	public void OnClick()
	{
		VisualDebugUI.Instance.OnSelect(_unit);
	}
}
