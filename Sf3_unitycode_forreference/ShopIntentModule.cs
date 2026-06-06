using Nekki.Yaml;
using SF3.Items;
using SimpleJSON;

public class ShopIntentModule : ItemIntentModule
{
	public ShopCategoryType Category { get; private set; }

	public override void Init(ConstantsSF3.ELocationSceneModule type, params object[] args)
	{
		base.Init(type, args);
		Category = ShopCategoryType.Weapon;
		if (HasProperties())
		{
			Category = GetCategory(Properties[0]);
		}
	}

	public override bool Equal(IntentModule value)
	{
		if (base.Equal(value))
		{
			ShopIntentModule shopIntentModule = (ShopIntentModule)value;
			return shopIntentModule.Category == Category && shopIntentModule.ItemId == base.ItemId;
		}
		return false;
	}

	public void SetCategory(ShopCategoryType categopry)
	{
		Category = categopry;
		Update();
	}

	private ShopCategoryType GetCategory(object categoryObject)
	{
		IntentParametrs intentParametrs = categoryObject as IntentParametrs;
		if (intentParametrs != null)
		{
			string text = intentParametrs.Tab as string;
			if (!text.IsNullOrEmpty())
			{
				return ComplianceUtils.GetShopCategoryTypeByName(text);
			}
			if (intentParametrs.Tab is ShopCategoryType)
			{
				return (ShopCategoryType)intentParametrs.Tab;
			}
		}
		return ShopCategoryType.Weapon;
	}

	public override Mapping ToYaml()
	{
		Mapping mapping = base.ToYaml();
		if (Category != 0)
		{
			mapping.Add(new Scalar("Tab", Category.ToString()));
		}
		return mapping;
	}

	public override JSONClass ToJSON()
	{
		JSONClass jSONClass = base.ToJSON();
		if (Category != 0)
		{
			jSONClass.Add("Tab", new JSONData(Category.ToString()));
		}
		return jSONClass;
	}
}
