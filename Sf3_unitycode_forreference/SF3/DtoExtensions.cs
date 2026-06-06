using SF3.Items;
using sf3DTO;

namespace SF3
{
	public static class DtoExtensions
	{
		public static sf3DTO.FightResult AsDto(this FightResult result)
		{
			switch (result.resultType)
			{
			case sf3DTO.FightResult.Win:
				return sf3DTO.FightResult.Win;
			case sf3DTO.FightResult.Loss:
				return sf3DTO.FightResult.Loss;
			default:
				return sf3DTO.FightResult.Surrender;
			}
		}

		public static Item AsDto(this Equipment equipment)
		{
			Item item = new Item();
			item.ModelId = equipment.id;
			item.Equipped = equipment.IsEquipped();
			Item item2 = item;
			item2.Perks.AddRange(equipment.GetPerkSlots());
			item2.StackLevel = equipment.stackLevel;
			return item2;
		}

		public static sf3DTO.Perk AsDto(this SF3.Items.Perk perk)
		{
			sf3DTO.Perk perk2 = new sf3DTO.Perk();
			perk2.ModelId = perk.GetId();
			perk2.StackLevel = perk.GetStackLevel();
			return perk2;
		}
	}
}
