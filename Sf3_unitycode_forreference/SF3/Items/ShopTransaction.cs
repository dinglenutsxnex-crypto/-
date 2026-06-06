using System;
using SF3.UserData;
using sf3DTO;

namespace SF3.Items
{
	public class ShopTransaction
	{
		public static ShopTransaction Current { get; private set; }

		public SF3.UserData.ShopItem ItemShop { get; private set; }

		public Currency Currency { get; private set; }

		public Action onSuccess
		{
			get
			{
				return ShopManager.Instance.OnBuyItemSuccess;
			}
		}

		public Action onFailure
		{
			get
			{
				return ShopManager.Instance.OnBuyItemFailure;
			}
		}

		private ShopTransaction(SF3.UserData.ShopItem item, Currency currency)
		{
			ItemShop = item;
			Currency = currency;
			Current = this;
		}

		public static void NewTransaction(SF3.UserData.ShopItem item, Currency currency)
		{
			Current = new ShopTransaction(item, currency);
		}

		public static void Clear()
		{
			Current = null;
		}
	}
}
