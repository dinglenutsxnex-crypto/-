using System.Collections.Generic;

namespace SF3.Items
{
	internal interface IPerkHolder
	{
		Perk GetPerkById(int id);

		Perk GetPerkByIdClone(int id);

		IEnumerable<Perk> GetAvailablePerksForItem(Equipment itemValue);

		IEnumerable<Perk> GetAvailablePerksForItemClone(Equipment itemValue);
	}
}
