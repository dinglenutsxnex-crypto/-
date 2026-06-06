using Nekki.Yaml;
using SF3;
using SF3.UserData;
using SimpleJSON;

public class FightIntentModule : IntentModule
{
	public FightInfo FightInfo { get; private set; }

	public override void Init(ConstantsSF3.ELocationSceneModule type, params object[] args)
	{
		base.Init(type, args);
		if (HasProperties())
		{
			FightInfo = GetFightInfo(Properties);
			return;
		}
		base.TypeModule = ConstantsSF3.ELocationSceneModule.DojoInterface;
		FightInfo = UserManager.CurrentDojoFight;
	}

	public override bool Equal(IntentModule value)
	{
		if (base.Equal(value))
		{
			FightInfo fightInfo = ((FightIntentModule)value).FightInfo;
			return FightInfo.battleID == fightInfo.battleID && FightInfo.fightID == fightInfo.fightID;
		}
		return false;
	}

	private FightInfo GetFightInfo(object[] args)
	{
		IntentParametrs intentParametrs = args[0] as IntentParametrs;
		if (intentParametrs != null && intentParametrs.BattleID != -1)
		{
			return BattlesManager.GetFightInfo(intentParametrs.BattleID, intentParametrs.FightID);
		}
		return UserManager.CurrentDojoFight;
	}

	public override Mapping ToYaml()
	{
		Scalar entry = new Scalar("Module", ConstantsSF3.ELocationSceneModule.DojoInterface.ToString());
		return new Mapping("Intent", entry);
	}

	public override JSONClass ToJSON()
	{
		JSONClass jSONClass = new JSONClass();
		jSONClass.Add("Module", new JSONData(ConstantsSF3.ELocationSceneModule.DojoInterface.ToString()));
		return jSONClass;
	}

	public override string ToString()
	{
		return string.Concat(base.TypeModule, " [ BattleID:", FightInfo.battleID, " FightID:", FightInfo.fightID, "]");
	}
}
