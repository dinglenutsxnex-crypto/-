using System;

namespace SF3.Items
{
	public interface IEquipment : ICloneable, ISlotable, IAttributable, IRarable, IFactionable, IInformable, IStackable
	{
		EquipmentType GetEquipmentType();

		void SetEquipmentType(EquipmentType type);

		bool IsHidden();

		bool IsDefault();

		bool IsEquipped();

		void SetEquipped(bool equipped);
	}
}
