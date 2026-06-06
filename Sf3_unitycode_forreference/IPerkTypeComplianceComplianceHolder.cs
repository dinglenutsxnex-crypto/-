using SF3.Items;

public interface IPerkTypeComplianceComplianceHolder
{
	PerkType GetPerkTypeByName(string name);

	string GetPerkNameByType(PerkType type);
}
