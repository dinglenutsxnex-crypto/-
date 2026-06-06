using System;
using System.Collections.Generic;
using System.Linq;
using Jint.Native.Array;
using Nekki;
using Nekki.Yaml;
using SF3.Items;
using UnityEngine;
using sf3DTO;

namespace SF3.UserData
{
	public class UserShopData
	{
		private Dictionary<ShopCategoryType, UserShopCategory> _userShopCategories;

		private Sequence _shopDataNode;

		private Action<bool> _OnDataUpdate = delegate
		{
		};

		public UserShopData(Sequence shopDataNodeValue, bool parseNode, Action<bool> OnConfigurationUpdateCallback = null)
		{
			_shopDataNode = shopDataNodeValue;
			_OnDataUpdate = OnConfigurationUpdateCallback;
			_userShopCategories = new Dictionary<ShopCategoryType, UserShopCategory>();
			if (!parseNode)
			{
				return;
			}
			foreach (Mapping item in _shopDataNode)
			{
				UserShopCategory userShopCategory = new UserShopCategory(item, _OnDataUpdate);
				_userShopCategories.Add(userShopCategory.categoryType, userShopCategory);
			}
		}

		public void UpdateData(Shop shopData)
		{
			List<sf3DTO.ShopItem> list = shopData.Items.RepeatedToList();
			Dictionary<ShopCategoryType, List<ShopItem>> dictionary = new Dictionary<ShopCategoryType, List<ShopItem>>();
			foreach (ShopCategoryType enumerator5 in EnumsCompliancer.GetEnumerators<ShopCategoryType>())
			{
				if (enumerator5 != 0)
				{
					dictionary.Add(enumerator5, new List<ShopItem>());
				}
			}
			foreach (sf3DTO.ShopItem item3 in list)
			{
				BaseItem item = ItemsManager.GetItem(item3);
				if (item != null)
				{
					ShopItem item2 = new ShopItem(item, item3.PurchaseCount);
					dictionary[ShopCategoryType.Equipment].Add(item2);
				}
			}
			Dictionary<int, ShopBooster> availableShopBoosters = GetAvailableShopBoosters();
			foreach (KeyValuePair<int, ShopBooster> item4 in availableShopBoosters)
			{
				ShopItem shopItem = new ShopItem(item4.Value, 10);
				if (shopItem.item == null)
				{
					Debug.LogWarning("Can't parse booster with name: " + item4.Value.Booster.model);
					return;
				}
				if (shopItem.HasPrice())
				{
					dictionary[ShopCategoryType.Booster].Add(shopItem);
				}
				else
				{
					Debug.LogError("Booster in shop doesn't have price (currencies)");
				}
			}
			_userShopCategories.Clear();
			_shopDataNode.RemoveNodes();
			foreach (KeyValuePair<ShopCategoryType, List<ShopItem>> item5 in dictionary)
			{
				if (item5.Value.Count != 0)
				{
					Mapping mapping = new Mapping("Category", new Scalar("Category", item5.Key.ToString()));
					mapping.Add(new Scalar("NewItemNotification", "1"));
					mapping.Add(new Sequence("Items", ((IEnumerable<ShopItem>)item5.Value).Select((Func<ShopItem, Node>)((ShopItem itemValue) => itemValue.ToYAML())).ToList()));
					_shopDataNode.AddNode(mapping);
					UserShopCategory userShopCategory = new UserShopCategory(mapping, item5.Key, item5.Value, _OnDataUpdate);
					_userShopCategories.Add(userShopCategory.categoryType, userShopCategory);
				}
			}
			_OnDataUpdate(false);
		}

		public UserShopCategory GetShopCategoryData(ShopCategoryType categoryType)
		{
			UserShopCategory result = null;
			if (_userShopCategories.ContainsKey(categoryType))
			{
				result = _userShopCategories[categoryType];
			}
			return result;
		}

		private Dictionary<int, ShopBooster> GetAvailableShopBoosters()
		{
			Dictionary<int, ShopBooster> dictionary = new Dictionary<int, ShopBooster>();
			int currentChapter = UserManager.Instance.GetCurrentChapter();
			ArrayInstance availableBoosters = JsFunction.GetAvailableBoosters(currentChapter);
			Dictionary<int, ShopBooster> shopBoosters = JS.Instance.ShopBoosters;
			for (int i = 0; i < availableBoosters.Length; i++)
			{
				int key = availableBoosters[i].AsInteger();
				if (shopBoosters.ContainsKey(key))
				{
					ShopBooster value = shopBoosters[key];
					dictionary.Add(key, value);
				}
			}
			return dictionary;
		}
	}
}
