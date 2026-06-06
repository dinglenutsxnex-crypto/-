using System.Collections.Generic;

namespace SF3.Items
{
	internal interface IEquipmentHolder
	{
		Equipment GetEquipmentById(int id);

		Equipment GetEquipmentByIdClone(int id);

		void AddEquipment(Equipment equipment);

		void AddEquipment(List<Equipment> equipment);

		List<Equipment> GetEquipmentByTypeAll(EquipmentType type);

		List<Equipment> GetEquipmentByTypeAllSafe(EquipmentType type);

		List<Equipment> GetEquipmentDefaultByTypeAll(EquipmentType type);

		List<Equipment> GetEquipmentDefaultByTypeAllClone(EquipmentType type);
	}
}
