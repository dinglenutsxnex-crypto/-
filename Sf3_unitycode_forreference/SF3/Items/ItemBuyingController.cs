using System;
using SF3.Moves;
using SF3.UserData;
using UnityEngine;
using sf3DTO;

namespace SF3.Items
{
	public class ItemBuyingController
	{
		private static ItemBuyingController _instance;

		public static ItemBuyingController Instance
		{
			get
			{
				return _instance ?? (_instance = new ItemBuyingController());
			}
		}

		public event Action OnSuccessBuy;

		public event Action OnFailureBuy;

		public void BuyItem(SF3.UserData.ShopItem itemShop, CurrencyType currencyType)
		{
			ShopTransaction.NewTransaction(itemShop, new Currency
			{
				CurrencyType = currencyType,
				Value = itemShop.GetCurrencyValue(currencyType)
			});
			if (!QuestController.Instance.ThrowEvent(ETriggerEvents.QEVENT_PRE_PURCHASE, ShopTransaction.Current))
			{
				CompleteCurrentTransaction();
			}
		}

		public void CompleteCurrentTransaction()
		{
			if (ShopTransaction.Current != null)
			{
				if (TryBuyOnClient())
				{
					BuyItemRequest buyItemRequest = new BuyItemRequest();
					buyItemRequest.ModelId = ShopTransaction.Current.ItemShop.item.id;
					buyItemRequest.Currency = ShopTransaction.Current.Currency;
					BuyItemRequest request = buyItemRequest;
					UserDataController.Send_BuyItem(request);
					this.OnSuccessBuy.InvokeSafe();
				}
				else
				{
					this.OnFailureBuy.InvokeSafe();
				}
				ShopTransaction.Clear();
			}
		}

		private bool TryBuyOnClient()
		{
			if (ShopTransaction.Current.Currency.Value <= UserManager.GetCurrencyValue(ShopTransaction.Current.Currency.CurrencyType))
			{
				UserManager.SubtractCurrency(ShopTransaction.Current.Currency);
				UserManager.AddItem(ShopTransaction.Current.ItemShop.item.Clone() as BaseItem, false);
				ShopTransaction.Current.ItemShop.Buy();
				return true;
			}
			Debug.Log("<color=red>Not enough money.</color>");
			return false;
		}
	}
}
