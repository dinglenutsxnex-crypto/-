using SF3.Items;

public class PerkTypeComplianceHolder : EnumCompliantHolder<PerkType>, IPerkTypeComplianceHolder
{
	public PerkType GetPerkTypeByName(string name)
	{
		return GetValueByKey(name);
	}

	public string GetPerkNameByType(PerkType type)
	{
		return GetKeyByType(type);
	}

	protected override PerkType GetValueDefault()
	{
		return PerkType.None;
	}
}
