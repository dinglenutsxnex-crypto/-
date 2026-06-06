using UnityEngine;

[RequireComponent(typeof(ConfigurableDialogContent))]
public class ConfigurableDialogModule : NekkiUIModule
{
	public delegate void DialogOpened(DialogConfig config);

	public delegate void DialogClosed(object obj);

	public static DialogOpened onDialogOpened;

	public static DialogClosed onDialogClosed;

	public static ConfigurableDialogModule Show(DialogConfig config)
	{
		if (string.IsNullOrEmpty(config.type))
		{
			Debug.LogError("ConfigurableDialog.Show: type name can't be empty");
			return null;
		}
		if (onDialogOpened != null)
		{
			onDialogOpened(config);
		}
		PauseWindow.DecrementShowCounter();
		if (BaseModuleController.CurrentType == ConstantsSF3.ELocationSceneModule.Map || BaseModuleController.CurrentType == ConstantsSF3.ELocationSceneModule.Shop || BaseModuleController.CurrentType == ConstantsSF3.ELocationSceneModule.Inventory)
		{
			config.outputCamera = ScreenTexture.TextureOutputCamera.Both;
		}
		return NekkiUIRootModules.Instance.MountNativeModule<ConfigurableDialogModule>(config.type, config);
	}

	public override void Setup(object settings)
	{
		base.Setup(settings);
		GetComponent<ConfigurableDialogContent>().Init((DialogConfig)settings);
	}
}
