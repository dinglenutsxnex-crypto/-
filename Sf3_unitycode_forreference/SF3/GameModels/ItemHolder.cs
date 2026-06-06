using System.Collections.Generic;
using System.Linq;
using SF3.Items;

namespace SF3.GameModels
{
	public class ItemHolder
	{
		public readonly PerkHolder _perksAll;

		public ItemHolder()
		{
			_perksAll = PerkHolder.Create();
		}

		public List<Perk> GetPerkByTargetType(EquipmentType type)
		{
			return _perksAll.GetPerkByTargetType(type).ToList();
		}

		public List<ISlotItem> GetAvailablePerksForItemClone(Equipment itemValue)
		{
			return _perksAll.GetAvailablePerksForItemClone(itemValue).Cast<ISlotItem>().ToList();
		}

		public List<ISlotItem> GetPerksByEquipmentTypeClone(EquipmentType type)
		{
			return _perksAll.GetPerkByEquipmentTypeClone(type).Cast<ISlotItem>().ToList();
		}

		public Perk GetPerkByID(int prkID)
		{
			return _perksAll.GetPerkById(prkID);
		}

		public void AddPerks(List<Perk> perksLoaded)
		{
			foreach (Perk item in perksLoaded)
			{
				AddPerk(item);
			}
		}

		public void AddPerk(Perk perkLoaded)
		{
			_perksAll.AddPerk(perkLoaded);
		}

		internal List<Perk> GetAll()
		{
			return _perksAll.GetPerkAll().ToList();
		}
	}
}
