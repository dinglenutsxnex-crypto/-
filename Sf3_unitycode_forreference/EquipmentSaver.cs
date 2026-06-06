using System;
using System.Collections.Generic;
using SF3.Items;
using SF3.UserData;

public class EquipmentSaver
{
	private Dictionary<EquipmentType, BaseItem> equipedItems;

	public EquipmentSaver()
	{
		equipedItems = new Dictionary<EquipmentType, BaseItem>();
	}

	public void SaveEquipedItems()
	{
		equipedItems.Clear();
		foreach (EquipmentType value in Enum.GetValues(typeof(EquipmentType)))
		{
			Equipment equipped = UserManager.UserModelInfo.GetEquipped(value);
			equipedItems[value] = ((equipped == null) ? null : (equipped.Clone() as BaseItem));
		}
	}

	public BaseItem GetEquipedAnalogByType(BaseItem analog)
	{
		Equipment equipment = analog as Equipment;
		if (equipment == null)
		{
			return null;
		}
		EquipmentType equipmentType = equipment.GetEquipmentType();
		if (equipedItems.ContainsKey(equipmentType))
		{
			return equipedItems[equipmentType];
		}
		return null;
	}
}
