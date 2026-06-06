using System;

public class ModuleController : BaseModuleController
{
	public static IntentModule GoToDojo(Action callbackOpenModule = null)
	{
		return BaseModuleController.GoToModule(ConstantsSF3.ELocationSceneModule.DojoInterface, callbackOpenModule);
	}

	public static IntentModule GoToFight(int battleID, int fightID = 0)
	{
		IntentParametrs intentParametrs = new IntentParametrs();
		intentParametrs.ModuleType = ConstantsSF3.ELocationSceneModule.Fight;
		intentParametrs.BattleID = battleID;
		intentParametrs.FightID = fightID;
		IntentParametrs intentParametrs2 = intentParametrs;
		return BaseModuleController.GoToModule(intentParametrs2.ModuleType, intentParametrs2);
	}

	public static IntentModule GoToFight(FightInfo fightInfo)
	{
		return GoToFight(fightInfo.battleID, fightInfo.fightID);
	}

	public static IntentModule GoToShop(object tab = null, int itemID = -1)
	{
		IntentParametrs intentParametrs = new IntentParametrs();
		intentParametrs.ModuleType = ConstantsSF3.ELocationSceneModule.Shop;
		intentParametrs.Tab = tab;
		intentParametrs.ItemID = itemID;
		IntentParametrs intentParametrs2 = intentParametrs;
		return BaseModuleController.GoToModule(intentParametrs2.ModuleType, intentParametrs2);
	}

	public static IntentModule GoToMap(int battleID = -1)
	{
		IntentParametrs intentParametrs = new IntentParametrs();
		intentParametrs.ModuleType = ConstantsSF3.ELocationSceneModule.Map;
		intentParametrs.BattleID = battleID;
		IntentParametrs intentParametrs2 = intentParametrs;
		return BaseModuleController.GoToModule(intentParametrs2.ModuleType, intentParametrs2);
	}

	public static IntentModule GoToInventory(string tab = "", int itemID = -1, string subType = "")
	{
		IntentParametrs intentParametrs = new IntentParametrs();
		intentParametrs.ModuleType = ConstantsSF3.ELocationSceneModule.Inventory;
		intentParametrs.Tab = tab;
		intentParametrs.ItemID = itemID;
		intentParametrs.SubModule = subType;
		IntentParametrs intentParametrs2 = intentParametrs;
		return BaseModuleController.GoToModule(intentParametrs2.ModuleType, intentParametrs2);
	}
}
