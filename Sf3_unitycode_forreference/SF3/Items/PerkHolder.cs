using System;
using System.Collections.Generic;
using System.Linq;
using Nekki;

namespace SF3.Items
{
	public class PerkHolder : ClassHolder<Perk>, IPerkHolder
	{
		public PerkHolderPurpose HolderType { get; private set; }

		private PerkHolder(PerkHolderPurpose purpose)
		{
			Init(purpose);
			Fill(purpose);
		}

		public static PerkHolder Create()
		{
			return new PerkHolder(PerkHolderPurpose.Empty);
		}

		public static PerkHolder Create(PerkHolderPurpose purpose)
		{
			return new PerkHolder(purpose);
		}

		public void Clear()
		{
			foreach (KeyValuePair<string, List<Perk>> item in Holder)
			{
				item.Value.Clear();
			}
		}

		private void Init(PerkHolderPurpose purpose)
		{
			HolderType = purpose;
			foreach (PerkType value in Enum.GetValues(typeof(PerkType)))
			{
				string enumeratorName = EnumsCompliancer.GetEnumeratorName<PerkType>((int)value);
				Holder.Add(enumeratorName, new List<Perk>());
			}
		}

		private void Fill(PerkHolderPurpose purpose)
		{
			if (purpose != PerkHolderPurpose.FilledFromJs)
			{
				return;
			}
			foreach (Perk value in JS.Instance.Perks.Values)
			{
				string enumeratorName = EnumsCompliancer.GetEnumeratorName<PerkType>((int)value.GetPerkType());
				Holder[enumeratorName].Add((Perk)value.Clone());
			}
		}

		public IEnumerable<Perk> GetPerkAll()
		{
			return from PerkType name in Enum.GetValues(typeof(PerkType))
				from perk in Holder[EnumsCompliancer.GetEnumeratorName<PerkType>((int)name)]
				select perk;
		}

		public Perk GetPerkById(int id)
		{
			return GetPerkAll().SingleOrDefault((Perk perk) => perk.id.Equals(id));
		}

		public Perk GetPerkByIdClone(int id)
		{
			Perk perkById = GetPerkById(id);
			return (perkById != null) ? ((Perk)perkById.Clone()) : null;
		}

		public void AddPerk(Perk perk)
		{
			string enumeratorName = EnumsCompliancer.GetEnumeratorName<PerkType>((int)perk.GetPerkType());
			Holder[enumeratorName].Add(perk);
		}

		public IEnumerable<Perk> GetAvailablePerksForItem(Equipment itemValue)
		{
			List<Perk> list = GetPerkAll().ToList();
			for (int i = 0; i < list.Count; i++)
			{
				if ((list[i].GetTargetFactionType() != itemValue.GetFactionType() && list[i].GetTargetFactionType() != 0) || (list[i].GetTargetItemType() != itemValue.GetEquipmentType() && list[i].GetTargetItemType() != 0) || (list[i].GetTargetTag() != null && !itemValue.HasTag(list[i].GetTargetTag())))
				{
					list.RemoveAt(i);
					i--;
				}
			}
			return list;
		}

		public IEnumerable<Perk> GetAvailablePerksForItemClone(Equipment itemValue)
		{
			return from perk in GetAvailablePerksForItem(itemValue)
				select (Perk)perk.Clone();
		}

		public IEnumerable<Perk> GetPerkByTargetType(EquipmentType type)
		{
			return from perk in GetPerkAll()
				where perk.GetTargetItemType() == type
				select perk;
		}

		public IEnumerable<Perk> GetPerkByEquipmentTypeClone(EquipmentType type)
		{
			return from perk in GetPerkByTargetType(type)
				select (Perk)perk.Clone();
		}
	}
}
