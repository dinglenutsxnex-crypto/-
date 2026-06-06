using SF3.Items;

public class ShopCategoryTypeCompliance : EnumCompliantHolder<ShopCategoryType>, IShopCategoryTypeComplianceHolder
{
	public ShopCategoryType GetShopCategoryTypeByName(string name)
	{
		return GetValueByKey(name);
	}

	public string GetShopCategoryNameByType(ShopCategoryType type)
	{
		return GetKeyByType(type);
	}

	protected override ShopCategoryType GetValueDefault()
	{
		return ShopCategoryType.None;
	}
}
