using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugListContainerController : MonoBehaviour
{
	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UIGrid linesGrid;

	[SerializeField]
	private GameObject debugLinePrefab;

	[SerializeField]
	private GameObject debugButtonPrefab;

	public void setup(string title)
	{
		titleLabel.text = title;
	}

	public void setVisible(bool visible)
	{
		if (visible)
		{
			GetComponent<UIWidget>().alpha = 1f;
		}
		else
		{
			GetComponent<UIWidget>().alpha = 0f;
		}
	}

	private void Start()
	{
	}

	public void UpdateLines()
	{
		List<Transform> childList = linesGrid.GetChildList();
		foreach (Transform item in childList)
		{
			DebugLineController component = item.GetComponent<DebugLineController>();
			component.onUpdate();
		}
	}

	public DebugTextLineController AddLine(string name, Action<DebugTextLineController> onUpdate)
	{
		GameObject gameObject = NGUITools.AddChild(linesGrid.gameObject, debugLinePrefab);
		linesGrid.Reposition();
		DebugTextLineController component = gameObject.GetComponent<DebugTextLineController>();
		component.setup(name, onUpdate);
		return component;
	}

	public DebugGOLineController AddLine(GameObject prefab, Action<DebugGOLineController> onUpdate)
	{
		GameObject gameObject = NGUITools.AddChild(linesGrid.gameObject, prefab);
		linesGrid.Reposition();
		DebugGOLineController component = gameObject.GetComponent<DebugGOLineController>();
		component.setup(onUpdate);
		return component;
	}

	public DebugButtonLineController AddButton(string text, Action onClick)
	{
		GameObject gameObject = NGUITools.AddChild(linesGrid.gameObject, debugButtonPrefab);
		linesGrid.Reposition();
		DebugButtonLineController component = gameObject.GetComponent<DebugButtonLineController>();
		component.setup(text, onClick);
		return component;
	}
}
