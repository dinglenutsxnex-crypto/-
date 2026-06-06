using SF3;
using SimpleJSON;

public class IntentParametrs
{
	public object Tab;

	public string SubModule;

	public string ID;

	public int BattleID;

	public int FightID;

	public int ItemID;

	public ConstantsSF3.ELocationSceneModule ModuleType;

	public IntentModule.ModuleTransitionType TransitionType;

	private readonly JSONClass BaseClass;

	public IntentParametrs()
	{
		ModuleType = ConstantsSF3.ELocationSceneModule.DojoInterface;
		TransitionType = IntentModule.ModuleTransitionType.Float;
		BattleID = -1;
		FightID = 0;
		ItemID = -1;
	}

	public IntentParametrs(JSONClass jsonClass)
		: this()
	{
		BaseClass = jsonClass;
		Parse();
	}

	private void Parse()
	{
		string nodeValueString = GetNodeValueString("Module");
		string nodeValueString2 = GetNodeValueString("TransitionType");
		Tab = GetNodeValueString("Tab");
		ID = GetNodeValueString("ID");
		SubModule = GetNodeValueString("SubModule");
		SF3Utils.TryParseEnum(out ModuleType, nodeValueString, ConstantsSF3.ELocationSceneModule.DojoInterface);
		SF3Utils.TryParseEnum(out TransitionType, nodeValueString2, IntentModule.ModuleTransitionType.Float);
		if (ModuleType == ConstantsSF3.ELocationSceneModule.Fight)
		{
			int[] result;
			if (SF3Utils.TryParseBattleIdentifier(out result, ID.AsRpn()))
			{
				BattleID = result[0];
				FightID = result[1];
			}
			else
			{
				Messenger.Error("Invalid battle/fight data! Current data: " + ID, this);
			}
		}
	}

	protected string GetNodeValueString(string key)
	{
		string text = ((!(BaseClass == null) && BaseClass.ContainsKey(key)) ? BaseClass[key].Value : string.Empty);
		if (!text.IsNullOrEmpty())
		{
			text = text.AsRpn();
		}
		return text;
	}

	public override string ToString()
	{
		return string.Concat("IntentParametrs [ ModuleType:", ModuleType, " ", TransitionType, " Tab:", Tab, " SubModule:", SubModule, " ID:", ID, " BattleID:", BattleID, " FightID:", FightID, " ItemID:", ItemID, "]");
	}
}
