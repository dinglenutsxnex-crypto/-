using SF3.Items;

public class EquipmentTypeComplianceHolder : EnumCompliantHolder<EquipmentType>, IEquipmentTypeComplianceHolder
{
	public EquipmentType GetEquipmentTypeByName(string name)
	{
		return GetValueByKey(name);
	}

	public string GetEquipmentNameByType(EquipmentType type)
	{
		return GetKeyByType(type);
	}

	protected override EquipmentType GetValueDefault()
	{
		return EquipmentType.None;
	}
}
