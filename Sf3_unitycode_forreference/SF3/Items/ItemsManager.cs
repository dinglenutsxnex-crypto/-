using System.Collections.Generic;
using SF3_Attributes;
using sf3DTO;

namespace SF3.Items
{
	public static class ItemsManager
	{
		private static IEquipmentHolder EquipmentHolder;

		private static IPerkHolder PerkHolder;

		public static void Init()
		{
			EquipmentHolder = SF3.Items.EquipmentHolder.Create(EquipmentHolderPurpose.FilledFromJs);
			PerkHolder = SF3.Items.PerkHolder.Create(PerkHolderPurpose.FilledFromJs);
		}

		public static bool TryGetEquipmentById(int id, out Equipment equipment)
		{
			equipment = EquipmentHolder.GetEquipmentByIdClone(id);
			return !IsNull(equipment);
		}

		public static List<Equipment> GetEquipmentDefaultByTypeAll(EquipmentType type)
		{
			return EquipmentHolder.GetEquipmentDefaultByTypeAllClone(type);
		}

		public static bool TryGetPerkById(int id, out Perk perk)
		{
			perk = PerkHolder.GetPerkByIdClone(id);
			return !IsNull(perk);
		}

		public static Perk GetPerkById(int id)
		{
			return PerkHolder.GetPerkByIdClone(id);
		}

		public static Equipment GetEquipmentById(int id)
		{
			return EquipmentHolder.GetEquipmentByIdClone(id);
		}

		public static BaseItem GetItemById(int idValue)
		{
			BaseItem equipmentById = GetEquipmentById(idValue);
			return equipmentById ?? GetPerkById(idValue);
		}

		public static BaseItem GetItem(ShopItem shopItem)
		{
			BaseItem itemById = GetItemById(shopItem.ModelId);
			if (itemById is IStackable)
			{
				((IStackable)itemById).SetStackLevel(shopItem.StackLevel);
			}
			return itemById;
		}

		public static ShopCategoryType GetItemShopCategoryType(BaseItem itemValue)
		{
			if (itemValue is Equipment)
			{
				return ComplianceUtils.GetShopCategoryTypeByName(((Equipment)itemValue).GetEquipmentType().ToString());
			}
			return ShopCategoryType.None;
		}

		public static List<Equipment> GetEquipmentById(IEnumerable<int> enumerable)
		{
			HashSet<int> hashSet = new HashSet<int>(enumerable);
			List<Equipment> list = new List<Equipment>();
			foreach (int item in hashSet)
			{
				Equipment equipment;
				if (TryGetEquipmentById(item, out equipment))
				{
					list.Add(equipment);
				}
			}
			return list;
		}

		private static bool IsNull(BaseItem toCheck)
		{
			return null == toCheck;
		}

		public static Attributes GetAttributesVisibleNextLevel(double stackLevel, int modelId)
		{
			Attributes attributes = new Attributes();
			attributes.SetAttributes(JsFunction.GetAttributesVisibleNextLevel(stackLevel, modelId));
			return attributes;
		}
	}
}
