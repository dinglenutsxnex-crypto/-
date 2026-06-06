using System;
using System.Linq;
using UnityEngine;
using sf3DTO;

namespace SF3.Settings
{
	public class ItemSettings : MonoBehaviour
	{
		[Serializable]
		private class ItemsRaritySettings
		{
			public Rarity rarity = Rarity.Common;

			public string cardBorder = string.Empty;

			public string descriptionLine = string.Empty;

			public string alias = string.Empty;

			public string spriteName = string.Empty;

			public UnityEngine.Color color = UnityEngine.Color.clear;

			public string boosterSpriteName = string.Empty;

			public UnityEngine.Color boosterTextColor = UnityEngine.Color.clear;
		}

		[Serializable]
		private class ItemsFactionSettings
		{
			public Faction faction;

			public string alias = string.Empty;
		}

		[SerializeField]
		private ItemsRaritySettings[] _itemsRaritySettings;

		[SerializeField]
		private ItemsFactionSettings[] _itemsFactionSettings;

		public UnityEngine.Color lowerAttributeColor;

		public UnityEngine.Color higherAttributeColor;

		public UnityEngine.Color normalAttributeColor;

		public UnityEngine.Color upgradeAttributeColor;

		public UnityEngine.Color upgradeArrowAttributeColor;

		private ItemsRaritySettings GetRaritySettings(Rarity rarity)
		{
			ItemsRaritySettings itemsRaritySettings = _itemsRaritySettings.FirstOrDefault((ItemsRaritySettings rarityValue) => rarityValue.rarity == rarity);
			if (itemsRaritySettings == null)
			{
				Debug.LogError(string.Format("Havnt any item settings for [{0}] rarity", rarity.ToString()));
				return new ItemsRaritySettings();
			}
			return itemsRaritySettings;
		}

		public string GetRarityCardBorder(Rarity rarity)
		{
			return GetRaritySettings(rarity).cardBorder;
		}

		public string GetRarityDescriptionLine(Rarity rarity)
		{
			return GetRaritySettings(rarity).descriptionLine;
		}

		public string GetRarityAlias(Rarity rarity)
		{
			return GetRaritySettings(rarity).alias;
		}

		public string GetRaritySpriteName(Rarity rarity)
		{
			return GetRaritySettings(rarity).spriteName;
		}

		public UnityEngine.Color GetRarityColor(Rarity rarity)
		{
			return GetRaritySettings(rarity).color;
		}

		public string GetBoosterSpriteName(Rarity rarity)
		{
			return GetRaritySettings(rarity).boosterSpriteName;
		}

		public UnityEngine.Color GetBoosterTextColor(Rarity rarity)
		{
			return GetRaritySettings(rarity).boosterTextColor;
		}

		public string GetFactionAlias(Faction faction)
		{
			string text = string.Empty;
			ItemsFactionSettings[] itemsFactionSettings = _itemsFactionSettings;
			foreach (ItemsFactionSettings itemsFactionSettings2 in itemsFactionSettings)
			{
				if (itemsFactionSettings2.faction == faction)
				{
					text = itemsFactionSettings2.alias;
					break;
				}
			}
			if (text.Length == 0)
			{
				Debug.LogError(string.Format("Havn't any item settings for [{0}] faction", faction.ToString()));
			}
			return text;
		}
	}
}
