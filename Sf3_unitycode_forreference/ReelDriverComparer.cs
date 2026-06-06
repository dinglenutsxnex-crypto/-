using System;
using System.Collections.Generic;
using SF3.Items;
using SF3_Attributes;
using sf3DTO;

public abstract class ReelDriverComparer : IComparer<BaseItem>
{
	public int Compare(BaseItem x, BaseItem y)
	{
		Equipment equipment = x as Equipment;
		Equipment equipment2 = y as Equipment;
		if (equipment != null && equipment2 != null)
		{
			return CompareEquipment(equipment, equipment2);
		}
		SF3.Items.Perk perk = x as SF3.Items.Perk;
		SF3.Items.Perk perk2 = y as SF3.Items.Perk;
		if (perk != null && perk2 != null)
		{
			return ComparePerks(perk, perk2);
		}
		return 0;
	}

	protected virtual int CompareEquipment(Equipment first, Equipment second)
	{
		if (first.GetEquipmentType() != second.GetEquipmentType())
		{
			return 0;
		}
		AttributeType key = AttributeType.None;
		switch (first.GetEquipmentType())
		{
		case EquipmentType.Helmet:
			key = AttributeType.HeadDefense;
			break;
		case EquipmentType.Armor:
			key = AttributeType.BodyDefense;
			break;
		case EquipmentType.Weapon:
			key = AttributeType.WeaponDamage;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case EquipmentType.None:
		case EquipmentType.Ranged:
		case EquipmentType.Magic:
			break;
		}
		float num = GetattributeOf(first, key);
		float num2 = GetattributeOf(second, key);
		return (num > num2) ? 1 : ((num < num2) ? (-1) : 0);
	}

	private float GetattributeOf(Equipment equipment, AttributeType key)
	{
		return equipment.GetAttributesForCombatValue(key);
	}

	protected virtual int ComparePerks(SF3.Items.Perk first, SF3.Items.Perk second)
	{
		if (first.GetPerkType() != second.GetPerkType())
		{
			return 0;
		}
		int num = CompareRarity(first.GetRarityType(), second.GetRarityType());
		return (num != 0) ? num : CompareStackLevel(first.GetStackLevel(), second.GetStackLevel());
	}

	private int CompareStackLevel(double first, double second)
	{
		return (!first.Equals(second)) ? ((first > second) ? 1 : (-1)) : 0;
	}

	private int CompareRarity(Rarity first, Rarity second)
	{
		return (first != second) ? ((first > second) ? 1 : (-1)) : 0;
	}
}
