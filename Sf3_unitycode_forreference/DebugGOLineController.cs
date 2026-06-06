using System;
using UnityEngine;

public class DebugGOLineController : DebugLineController
{
	private Action<DebugGOLineController> updateAction;

	[HideInInspector]
	public GameObject go;

	internal virtual void setup(Action<DebugGOLineController> onUpdate)
	{
		go = base.gameObject;
		updateAction = onUpdate;
	}

	internal override void onUpdate()
	{
		if (updateAction != null)
		{
			updateAction(this);
		}
	}
}
