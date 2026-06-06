using Nekki.Yaml;
using SF3;
using SF3.UserData;
using SimpleJSON;

public class MapIntentModule : IntentModule
{
	public static readonly int BattleIdDefault;

	public IBattleInfo Battle { get; private set; }

	public int BattleID
	{
		get
		{
			return (Battle == null) ? BattleIdDefault : Battle.GetID();
		}
	}

	public override void Init(ConstantsSF3.ELocationSceneModule type, params object[] args)
	{
		base.Init(type, args);
		SetBattle((!HasProperties()) ? BattlesManager.instance.GetBattle(UserManager.GetSelectedBattleID()) : Properties[0]);
	}

	public override bool Equal(IntentModule value)
	{
		if (base.Equal(value))
		{
			MapIntentModule mapIntentModule = (MapIntentModule)value;
			return mapIntentModule.BattleID == BattleID;
		}
		return false;
	}

	public void SetBattle(object value)
	{
		Parse(value);
		Update();
	}

	private void Parse(object value)
	{
		Battle = UserManager.CurrentDojoBattle;
		IBattleInfo battleInfo = value as IBattleInfo;
		if (battleInfo != null)
		{
			Battle = battleInfo;
		}
		else if (value is int)
		{
			SetBattleID((int)value);
		}
		else if (value is IntentParametrs)
		{
			SetBattleID(((IntentParametrs)value).BattleID);
		}
	}

	private void SetBattleID(int value)
	{
		IBattleInfo battle = BattlesManager.instance.GetBattle(value);
		if (battle != null)
		{
			Battle = battle;
		}
	}

	public override Mapping ToYaml()
	{
		Mapping mapping = base.ToYaml();
		mapping.Add(new Scalar("BattleID", BattleID.ToString()));
		return mapping;
	}

	public override JSONClass ToJSON()
	{
		JSONClass jSONClass = base.ToJSON();
		jSONClass.Add("BattleID", new JSONData(BattleID));
		return jSONClass;
	}

	public override string ToString()
	{
		return string.Concat(base.TypeModule, " [BattleID:", BattleID, "]");
	}
}
