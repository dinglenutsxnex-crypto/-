using SF3;
using SF3.UserData;

public abstract class HolderModuleManager : HolderModule
{
	protected UIModuleHolder CurrentManager;

	protected ConstantsSF3.ELocationSceneModule NewType;

	private IntentModule intent;

	private string _nameMount;

	private bool isReopen;

	protected override void OpenModule(IntentModule value)
	{
		intent = value;
		if (!FightHolderModule.CreateDojo)
		{
			CurrencyUI.SetActive(true);
			FightHolderModule.CreateDojo = true;
			BattlesManager.EnterBattle(UserManager.CurrentDojoFight, OpenManager);
		}
		else
		{
			OpenManager();
		}
	}

	protected virtual void Mount(string name = "")
	{
		if (CurrentManager == null)
		{
			_nameMount = name;
			UIModuleHolder component = NekkiUIRootModules.Instance.MountNGUIModule(_nameMount).GetComponent<UIModuleHolder>();
			component.Initialize();
			CurrentManager = component;
		}
	}

	protected virtual void UnMount()
	{
		NekkiUIRootModules.Instance.UnmountModule(_nameMount);
		CurrentManager = null;
	}

	private void OpenManager()
	{
		if (!isReopen)
		{
			Mount(string.Empty);
			ShowModule();
		}
		else
		{
			UpdateModule();
		}
	}

	private void ShowModule()
	{
		if (CurrentManager != null)
		{
			CurrentManager.ShowModule(intent, OpenedCallback);
		}
	}

	private void UpdateModule()
	{
		if (CurrentManager != null)
		{
			CurrentManager.UpdateModule(intent);
		}
	}

	protected override void CloseModule(IntentModule intent)
	{
		if (!isReopen)
		{
			UnMount();
		}
	}

	public override void Close(ConstantsSF3.ELocationSceneModule newType)
	{
		NewType = newType;
		isReopen = intent.TypeModule == NewType;
		if (!isReopen)
		{
			CurrentManager.HideModule(OnCloseCallback);
		}
		else
		{
			OnCloseCallback();
		}
	}

	protected virtual void OnCloseCallback()
	{
		base.Close(NewType);
	}
}
