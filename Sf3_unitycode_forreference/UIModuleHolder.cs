using System;
using UnityEngine;

[RequireComponent(typeof(NekkiUIModule))]
public abstract class UIModuleHolder : ExtentionBehaviour
{
	protected NekkiUIModule _nekkiUIModule;

	protected virtual void Awake()
	{
		_nekkiUIModule = GetComponent<NekkiUIModule>();
	}

	public virtual void Initialize()
	{
	}

	public virtual void ShowModule(IntentModule intent, Action openedCallback)
	{
	}

	public virtual void HideModule(Action closedCallback)
	{
	}

	public virtual void UpdateModule(IntentModule intent)
	{
	}
}
