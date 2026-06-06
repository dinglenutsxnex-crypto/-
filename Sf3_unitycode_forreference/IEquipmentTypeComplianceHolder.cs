using SF3.Items;

public interface IEquipmentTypeComplianceHolder
{
	EquipmentType GetEquipmentTypeByName(string name);

	string GetEquipmentNameByType(EquipmentType type);
}
