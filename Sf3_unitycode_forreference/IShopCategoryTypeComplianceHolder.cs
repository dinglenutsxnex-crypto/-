using SF3.Items;

public interface IShopCategoryTypeComplianceHolder
{
	ShopCategoryType GetShopCategoryTypeByName(string name);

	string GetShopCategoryNameByType(ShopCategoryType type);
}
