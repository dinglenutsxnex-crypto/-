using SF3;

public class IntentTransitionData
{
	public class TransitionData
	{
		public string Name = "None";

		public string Category = string.Empty;

		public int ItemID = -1;

		public int FightID;

		public int BattleID;

		public override string ToString()
		{
			return "TransitionData [Name:" + Name + " Category:" + Category + " ItemID:" + ItemID + " FightID:" + FightID + " BattleID:" + BattleID + "]";
		}
	}

	public TransitionData FromData;

	public TransitionData ToData;

	public IntentTransitionData(IntentModule fromIntent, IntentModule toIntent)
	{
		FromData = Parse(fromIntent);
		ToData = Parse(toIntent);
	}

	public TransitionData Parse(IntentModule value)
	{
		TransitionData transitionData = new TransitionData();
		if (value != null)
		{
			transitionData.Name = value.TypeModule.ToString();
			switch (value.TypeModule)
			{
			case ConstantsSF3.ELocationSceneModule.Fight:
			case ConstantsSF3.ELocationSceneModule.DojoInterface:
			{
				FightIntentModule fightIntentModule = (FightIntentModule)value;
				if (fightIntentModule.FightInfo != null)
				{
					transitionData.BattleID = fightIntentModule.FightInfo.battleID;
					transitionData.FightID = fightIntentModule.FightInfo.fightID;
				}
				break;
			}
			case ConstantsSF3.ELocationSceneModule.Shop:
			{
				ShopIntentModule shopIntentModule = (ShopIntentModule)value;
				transitionData.Category = shopIntentModule.Category.ToString();
				transitionData.ItemID = shopIntentModule.ItemId;
				break;
			}
			case ConstantsSF3.ELocationSceneModule.Inventory:
			{
				InventoryIntentModule inventoryIntentModule = (InventoryIntentModule)value;
				transitionData.Category = inventoryIntentModule.Category.ToString();
				transitionData.ItemID = inventoryIntentModule.ItemId;
				break;
			}
			case ConstantsSF3.ELocationSceneModule.Map:
			{
				MapIntentModule mapIntentModule = (MapIntentModule)value;
				IBattleInfo battleInfo = ((mapIntentModule.Battle == null) ? BattlesManager.instance.GetDefaultBattle() : mapIntentModule.Battle);
				transitionData.BattleID = battleInfo.GetID();
				break;
			}
			}
		}
		return transitionData;
	}

	public override string ToString()
	{
		return string.Concat("TransitionData [From - ", FromData.ToString(), " To - ", ToData, "]");
	}
}
