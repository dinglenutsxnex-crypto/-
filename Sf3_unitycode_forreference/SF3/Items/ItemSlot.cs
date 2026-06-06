using System;
using System.Collections.Generic;
using Nekki.Yaml;
using SimpleJSON;
using sf3DTO;

namespace SF3.Items
{
	public class ItemSlot : ISlotItem, ICloneable
	{
		public int slotIndex { get; private set; }

		public Perk perk { get; private set; }

		public ItemSlot(int slotIndexValue, Perk perkValue = null)
		{
			slotIndex = slotIndexValue;
			perk = perkValue;
		}

		public static explicit operator BaseItem(ItemSlot d)
		{
			return d.perk;
		}

		public void SetPerk(Perk perkValue)
		{
			perk = perkValue;
		}

		public void RemovePerk()
		{
			perk = null;
		}

		public bool HasPerk()
		{
			return perk != null;
		}

		public bool HasPerk(int perkID)
		{
			return perk != null && perk.GetId() == perkID;
		}

		public string GetImage()
		{
			return (perk != null) ? perk.GetImage() : string.Empty;
		}

		public int GetId()
		{
			return (perk != null) ? perk.GetId() : (-1);
		}

		public string GetAlias()
		{
			return (perk != null) ? perk.GetAlias() : string.Empty;
		}

		public EquipmentType GetTargetItemType()
		{
			return (perk != null) ? perk.GetTargetItemType() : EquipmentType.None;
		}

		public Faction GetTargetFactionType()
		{
			return (perk != null) ? perk.GetTargetFactionType() : Faction.UnknownFaction;
		}

		public List<string> GetTags()
		{
			return (perk != null) ? perk.GetTags() : new List<string>();
		}

		public List<Node> ToYaml()
		{
			return (perk != null) ? perk.ToYaml() : null;
		}

		public JSONClass ToJSON()
		{
			return (perk != null) ? perk.ToJSON() : null;
		}

		public object Clone()
		{
			ItemSlot itemSlot = MemberwiseClone() as ItemSlot;
			if (itemSlot.perk != null)
			{
				itemSlot.perk = perk.Clone() as Perk;
			}
			return itemSlot;
		}
	}
}
