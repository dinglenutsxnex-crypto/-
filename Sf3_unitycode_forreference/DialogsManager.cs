using Nekki.Yaml;
using SF3;

public static class DialogsManager
{
	private static Mapping configSourceDelayedMap;

	private static string callbackDelayed;

	public static bool IsDialog { get; set; }

	private static void ShowDelayedMap()
	{
		if (configSourceDelayedMap != null)
		{
			ShowNow(configSourceDelayedMap);
			configSourceDelayedMap = null;
		}
	}

	internal static void Show(Mapping configSource)
	{
		if (LoadScreen.LoaderVisible)
		{
			configSourceDelayedMap = configSource;
			LoadScreen.Instance.SheduleOnHideLoader(ShowDelayedMap);
		}
		else
		{
			ShowNow(configSource);
		}
	}

	private static void ShowNow(Mapping configSource)
	{
		if (BaseModuleController.CurrentType == ConstantsSF3.ELocationSceneModule.Map)
		{
			MapController.Instance.ShowBattleInfo(false);
		}
		ConfigurableDialogModule.Show(new DialogConfig(configSource));
	}
}
