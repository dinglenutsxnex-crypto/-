using SF3.Items;
using SF3_Attributes;

public class ComplianceUtils
{
	private static readonly IEquipmentTypeComplianceHolder EquipmentTypeCompliance;

	private static readonly IPerkTypeComplianceHolder PerkTypeCompliance;

	private static readonly IAttributeNameTypeComplianceHolder AttributeNameTypeCompliance;

	private static readonly IShopCategoryTypeComplianceHolder ShopCategoryTypeCompliance;

	static ComplianceUtils()
	{
		EquipmentTypeCompliance = new EquipmentTypeComplianceHolder();
		PerkTypeCompliance = new PerkTypeComplianceHolder();
		AttributeNameTypeCompliance = new AttributeNameTypeComplianceHolder();
		ShopCategoryTypeCompliance = new ShopCategoryTypeCompliance();
	}

	public static ShopCategoryType GetShopCategoryTypeByName(string name)
	{
		return ShopCategoryTypeCompliance.GetShopCategoryTypeByName(name);
	}

	public static string GetShopCategoryNameByType(ShopCategoryType type)
	{
		return ShopCategoryTypeCompliance.GetShopCategoryNameByType(type);
	}

	public static EquipmentType GetEquipmentTypeByName(string name)
	{
		return EquipmentTypeCompliance.GetEquipmentTypeByName(name);
	}

	public static string GetEquipmentNameByType(EquipmentType type)
	{
		return EquipmentTypeCompliance.GetEquipmentNameByType(type);
	}

	public static string GetPerkNameByType(PerkType type)
	{
		return PerkTypeCompliance.GetPerkNameByType(type);
	}

	public static PerkType GetPerkTypeByName(string name)
	{
		return PerkTypeCompliance.GetPerkTypeByName(name);
	}

	public static string GetAttributeNameByType(AttributeType type)
	{
		return AttributeNameTypeCompliance.GetAttributeNameByType(type);
	}

	public static AttributeType GetAttributeTypeByName(string name)
	{
		return AttributeNameTypeCompliance.GetAttributeTypeByName(name);
	}
}
