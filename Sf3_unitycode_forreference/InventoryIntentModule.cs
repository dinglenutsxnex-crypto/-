using Nekki.Yaml;
using SF3;
using SF3.Items;
using SimpleJSON;

public class InventoryIntentModule : ItemIntentModule
{
	public enum InventorySubModuleType
	{
		None = 0,
		PerkScreen = 1
	}

	public InventorySubModuleType SubModule;

	public EquipmentType Category { get; private set; }

	public override void Init(ConstantsSF3.ELocationSceneModule type, params object[] args)
	{
		base.Init(type, args);
		Category = EquipmentType.None;
		SubModule = InventorySubModuleType.None;
		if (HasProperties())
		{
			SetCategory(GetCategory(Properties[0]));
			SetSubModule(GetSubModuleType(Properties[0]));
		}
	}

	public void SetCategory(EquipmentType category)
	{
		Category = category;
		Update();
	}

	public void SetSubModule(InventorySubModuleType subModule)
	{
		SubModule = subModule;
		Update();
	}

	public override bool Equal(IntentModule value)
	{
		if (!base.Equal(value))
		{
			return false;
		}
		InventoryIntentModule inventoryIntentModule = (InventoryIntentModule)value;
		return inventoryIntentModule.Category == Category && inventoryIntentModule.ItemId == base.ItemId && inventoryIntentModule.SubModule == SubModule;
	}

	private EquipmentType GetCategory(object categoryObject)
	{
		IntentParametrs intentParametrs = categoryObject as IntentParametrs;
		if (intentParametrs != null)
		{
			string text = intentParametrs.Tab as string;
			if (text != null)
			{
				return ComplianceUtils.GetEquipmentTypeByName(text);
			}
			if (intentParametrs.Tab is EquipmentType)
			{
				return (EquipmentType)intentParametrs.Tab;
			}
		}
		return EquipmentType.None;
	}

	private InventorySubModuleType GetSubModuleType(object categoryObject)
	{
		IntentParametrs intentParametrs = categoryObject as IntentParametrs;
		if (intentParametrs != null)
		{
			InventorySubModuleType outParam;
			SF3Utils.TryParseEnum(out outParam, intentParametrs.SubModule, InventorySubModuleType.None);
			return outParam;
		}
		return InventorySubModuleType.None;
	}

	public override Mapping ToYaml()
	{
		Mapping mapping = base.ToYaml();
		if (Category != 0)
		{
			mapping.Add(new Scalar("Tab", Category.ToString()));
		}
		if (SubModule != 0)
		{
			mapping.Add(new Scalar("SubModule", SubModule.ToString()));
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
		if (SubModule != 0)
		{
			jSONClass.Add("SubModule", new JSONData(SubModule.ToString()));
		}
		return jSONClass;
	}

	public override string ToString()
	{
		return "InventoryIntentModule [Category: " + Category.ToString() + " ItemId:" + base.ItemId + "]";
	}
}
