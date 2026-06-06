using SF3.Items;

public interface IPerkTypeComplianceHolder
{
	PerkType GetPerkTypeByName(string name);

	string GetPerkNameByType(PerkType type);
}
