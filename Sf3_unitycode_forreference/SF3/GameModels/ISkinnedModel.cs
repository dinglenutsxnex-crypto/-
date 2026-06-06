using SF3.Items;

namespace SF3.GameModels
{
	public interface ISkinnedModel
	{
		int GetEquippedIDForType(EquipmentType type);

		Bone GetCenterOfMassBone();

		void EquipItem(int equippedItemID, bool throwEventEquip = true);

		void UnEquipItem(EquipmentType type, bool throwEventEquip = true);

		int GetId();
	}
}
