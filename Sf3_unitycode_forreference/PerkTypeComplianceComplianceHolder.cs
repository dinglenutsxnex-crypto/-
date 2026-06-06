using SF3.Items;

public class PerkTypeComplianceComplianceHolder : EnumCompliantHolder<PerkType>, IPerkTypeComplianceComplianceHolder
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
