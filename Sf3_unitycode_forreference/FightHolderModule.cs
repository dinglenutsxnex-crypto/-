using System;
using SF3;
using SF3.UserData;

public class FightHolderModule : HolderModule
{
	public const float FadeDuration = 1f;

	public static bool CreateDojo;

	public override void Init(ConstantsSF3.ELocationSceneModule type, Action<HolderModule> onOpened, Action<HolderModule> onClosed)
	{
		base.Init(type, onOpened, onClosed);
		CreateDojo = false;
	}

	public override void Close(ConstantsSF3.ELocationSceneModule newType)
	{
		ExitDarkPocket();
		base.Close(newType);
		LoadingIcon instance = LoadingIcon.Instance;
		instance.OnEnableLoadingScreen = (Action)Delegate.Remove(instance.OnEnableLoadingScreen, new Action(PauseWindow.Pause));
	}

	protected override void OpenModule(IntentModule intent)
	{
		CreateDojo = false;
		FightIntentModule fightIntentModule = (FightIntentModule)intent;
		BattlesManager.EnterBattle(fightIntentModule.FightInfo, OpenedCallback);
	}

	public override bool IsCanOpen()
	{
		return base.IsCanOpen() && !UserDataController.waitingForRefreshBattles;
	}

	protected override void OpenedCallback()
	{
		EnterDarkPocket();
		base.OpenedCallback();
		ModelsManager.Instance.ShowModels(1f);
		HolderModule.EnableControls(true);
		LoadingIcon instance = LoadingIcon.Instance;
		instance.OnEnableLoadingScreen = (Action)Delegate.Remove(instance.OnEnableLoadingScreen, new Action(PauseWindow.Pause));
		LoadingIcon instance2 = LoadingIcon.Instance;
		instance2.OnEnableLoadingScreen = (Action)Delegate.Combine(instance2.OnEnableLoadingScreen, new Action(PauseWindow.Pause));
	}

	protected virtual void EnterDarkPocket()
	{
		NetworkConnection.current.EnterDarkPocket();
	}

	protected virtual void ExitDarkPocket()
	{
		NetworkConnection.current.ExitDarkPocket();
	}
}
