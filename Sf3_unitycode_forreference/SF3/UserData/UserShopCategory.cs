using System;
using System.Collections.Generic;
using System.Linq;
using Nekki.Yaml;
using SF3.Items;
using UnityEngine;

namespace SF3.UserData
{
	public class UserShopCategory
	{
		private Action<bool> _OnDataUpdate = delegate
		{
		};

		private Mapping _categoryNode;

		public ShopCategoryType categoryType { get; private set; }

		public bool newItemNotification { get; private set; }

		public int newItemsCount { get; private set; }

		public List<ShopItem> items { get; private set; }

		public UserShopCategory(Mapping categoryNode, Action<bool> OnConfigurationUpdateCallback = null)
		{
			categoryType = ShopCategoryType.None;
			_OnDataUpdate = OnConfigurationUpdateCallback;
			_categoryNode = categoryNode;
			Scalar text = _categoryNode.GetText("Category");
			if (text != null)
			{
				categoryType = ComplianceUtils.GetShopCategoryTypeByName(text.text);
			}
			if (categoryType == ShopCategoryType.None)
			{
				Debug.LogError("Category is wrong or not setted!");
			}
			items = new List<ShopItem>();
			Sequence sequence = _categoryNode.GetSequence("Items");
			if (sequence != null)
			{
				foreach (Mapping item2 in sequence.nodesInside)
				{
					ShopItem item = new ShopItem(item2);
					items.Add(item);
				}
			}
			newItemsCount = CountNewItems(items);
			newItemNotification = false;
			Scalar text2 = _categoryNode.GetText("NewItemNotification");
			if (text2 != null)
			{
				newItemNotification = (text2.text.Equals("1") ? true : false);
			}
		}

		public UserShopCategory(Mapping categoryValue, ShopCategoryType dataCategoryType, List<ShopItem> dataItems, Action<bool> OnConfigurationUpdateCallback = null)
		{
			categoryType = dataCategoryType;
			items = dataItems;
			newItemNotification = true;
			newItemsCount = CountNewItems(items);
			_OnDataUpdate = OnConfigurationUpdateCallback;
			_categoryNode = categoryValue;
		}

		public List<BaseItem> GetItems()
		{
			return items.Select((ShopItem it) => it.item).ToList();
		}

		public void SetCategoryNotification(bool setNotification)
		{
			newItemNotification = setNotification;
			_categoryNode.SetOrAddText("NewItemNotification", (!newItemNotification) ? "0" : "1");
			_OnDataUpdate(true);
		}

		private int CountNewItems(List<ShopItem> shopItems)
		{
			int num = 0;
			foreach (ShopItem shopItem in shopItems)
			{
				if (shopItem.purchaseCount == 0)
				{
					num++;
				}
			}
			return num;
		}

		public void CalculateItemsCurrency()
		{
			items.ForEach(delegate(ShopItem it)
			{
				it.CalculateCurrency();
			});
		}
	}
}
