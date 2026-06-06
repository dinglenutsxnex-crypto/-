using SF3;

public class DojoHolderModule : FightHolderModule
{
	protected override void OpenModule(IntentModule intent)
	{
		if (!FightHolderModule.CreateDojo)
		{
			CurrencyUI.SetActive(true);
			base.OpenModule(intent);
		}
		else
		{
			OpenedCallback();
		}
		FightHolderModule.CreateDojo = true;
	}

	protected override void OpenedCallback()
	{
		CurrencyUI.SetActive(true);
		BattleCamera.MoveToDojo(base.OpenedCallback, base.Intent.IsInstant());
	}

	protected override void EnterDarkPocket()
	{
	}

	protected override void ExitDarkPocket()
	{
	}
}
