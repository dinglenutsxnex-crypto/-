using System;
using Nekki;
using UnityEngine;

[Serializable]
public class ModuleInfo
{
	public string moduleName;

	public bool visibleAtStart;

	public UIButton moduleBtn;

	private GameObject moduleObj;

	public void Init()
	{
		if (!NekkiUtils.IsDebug)
		{
			return;
		}
		moduleObj = NekkiUIRootModules.Instance.MountNGUIModule(moduleName).gameObject;
		if (moduleObj != null)
		{
			moduleBtn.onClick.Add(new EventDelegate(delegate
			{
				moduleObj.SendMessage("ShowHide");
			}));
			if (!visibleAtStart)
			{
				moduleObj.SendMessage("ShowHide");
			}
		}
	}
}
