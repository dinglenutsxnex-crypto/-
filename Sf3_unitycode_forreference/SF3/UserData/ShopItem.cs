using System;
using System.Collections.Generic;
using Jint.Native;
using Nekki.Yaml;
using SF3.Items;
using SimpleJSON;
using sf3DTO;

namespace SF3.UserData
{
	public class ShopItem
	{
		private BaseItem _item;

		private int _boosterpackID;

		private CurrencyHolder _currencyHolder;

		private int _purchaseCount;

		private Mapping _shopItemNode;

		public BaseItem item
		{
			get
			{
				return _item;
			}
		}

		public int BoosterpackID
		{
			get
			{
				return _boosterpackID;
			}
		}

		public int purchaseCount
		{
			get
			{
				return _purchaseCount;
			}
		}

		public ShopItem()
		{
			_currencyHolder = new CurrencyHolder();
		}

		public ShopItem(Mapping itemNode)
			: this()
		{
			_shopItemNode = itemNode;
			_item = Equipment.Create(_shopItemNode);
			_purchaseCount = 0;
			Scalar text = _shopItemNode.GetText("PurchaseCount");
			if (text != null)
			{
				_purchaseCount = int.Parse(text.text);
			}
		}

		public ShopItem(ShopBooster shopBooster, int purchaseCountValue = 0)
		{
			_item = shopBooster.Booster;
			Dictionary<CurrencyType, long> currencies = shopBooster.Currencies;
			_boosterpackID = shopBooster.ID;
			_purchaseCount = purchaseCountValue;
			_currencyHolder = new CurrencyHolder();
			long value = 0L;
			currencies.TryGetValue(CurrencyType.Coin, out value);
			long value2 = 0L;
			currencies.TryGetValue(CurrencyType.Bonus, out value2);
			_currencyHolder.SetCurrency(CurrencyType.Coin, value);
			_currencyHolder.SetCurrency(CurrencyType.Bonus, value2);
		}

		public ShopItem(BaseItem itemValue, int purchaseCountValue = 0)
		{
			_item = itemValue;
			_purchaseCount = purchaseCountValue;
			_currencyHolder = new CurrencyHolder();
			CalculateCurrency();
		}

		public bool HasPrice()
		{
			return _currencyHolder.HasAnyCurrency();
		}

		public Mapping ToYAML()
		{
			if (_item != null)
			{
				_shopItemNode = new Mapping("Item", _item.ToYaml());
			}
			_shopItemNode.Add(new Scalar("PurchaseCount", _purchaseCount.ToString()));
			return _shopItemNode;
		}

		public JSONClass ToJSON()
		{
			throw new NotImplementedException();
		}

		public long GetCurrencyValue(CurrencyType type)
		{
			return _currencyHolder.GetCurrencyValue(type);
		}

		public void Buy(int addValue = 1)
		{
			_purchaseCount += addValue;
			CalculateCurrency();
			_shopItemNode.SetOrAddText("PurchaseCount", _purchaseCount.ToString());
			UserShopManager.Instance.SaveFileShop();
		}

		public void CalculateCurrency()
		{
			if (_item == null || _item is SF3.Items.Booster)
			{
				return;
			}
			IStackable stackable = UserManager.UserModelInfo.GetItemByID(_item.id) as IStackable;
			double stackLevel = 0.0;
			if (stackable != null)
			{
				stackLevel = stackable.GetStackLevel();
			}
			Dictionary<string, JsValue> dictionary = JsFunction.CalculateShopitemParameters(UserManager.UserModelInfo.level, _item.id, stackLevel);
			Dictionary<string, JsValue> dictionary2 = dictionary["Price"].AsDictionary();
			foreach (string key in dictionary2.Keys)
			{
				CurrencyType outParam;
				if (SF3Utils.TryParseEnum(out outParam, key, CurrencyType.Coin))
				{
					_currencyHolder.SetCurrency(outParam, (long)dictionary2[key].AsNumber());
				}
			}
			IStackable stackable2 = item as IStackable;
			if (stackable2 != null)
			{
				double stackLevel2 = dictionary["SL"].AsNumber();
				stackable2.SetStackLevel(stackLevel2);
			}
		}
	}
}
