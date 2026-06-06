using System.Collections.Generic;

namespace SF3.Items
{
	public interface ISlotable
	{
		int GetSlotQuantity();

		bool HasSlots();

		List<ItemSlot> GetSlotItems();

		bool CanAddSlotItem(ISlotItem item);

		bool HasPerk(Perk item);

		bool AddSlotItem(ISlotItem slotitem, int slotIndex);

		bool RemoveSlotItem(ISlotItem item, out int slotIndex);
	}
}
