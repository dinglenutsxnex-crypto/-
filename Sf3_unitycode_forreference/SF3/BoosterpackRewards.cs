using System.Collections.Generic;
using Nekki.Yaml;
using SF3.Items;
using sf3DTO;

namespace SF3
{
	public class BoosterpackRewards
	{
		public List<Equipment> equipments { get; private set; }

		public List<SF3.Items.Perk> perks { get; private set; }

		public List<BaseItem> RewardItems
		{
			get
			{
				List<BaseItem> list = new List<BaseItem>();
				foreach (Equipment equipment in equipments)
				{
					list.Add(equipment);
				}
				foreach (SF3.Items.Perk perk in perks)
				{
					list.Add(perk);
				}
				return list;
			}
		}

		private BoosterpackRewards()
		{
			equipments = new List<Equipment>();
			perks = new List<SF3.Items.Perk>();
		}

		public static BoosterpackRewards Create(Mapping lootData)
		{
			return new BoosterpackRewards();
		}

		public static BoosterpackRewards Create(Loot lootData)
		{
			return new BoosterpackRewards();
		}

		public static BoosterpackRewards Create()
		{
			return new BoosterpackRewards();
		}

		public static BoosterpackRewards Create(List<int> equipmentsIDs, List<int> perksIDs)
		{
			BoosterpackRewards boosterpackRewards = new BoosterpackRewards();
			foreach (int equipmentsID in equipmentsIDs)
			{
				boosterpackRewards.equipments.Add(ItemsManager.GetEquipmentById(equipmentsID));
			}
			foreach (int perksID in perksIDs)
			{
				boosterpackRewards.perks.Add(ItemsManager.GetPerkById(perksID));
			}
			return boosterpackRewards;
		}
	}
}
