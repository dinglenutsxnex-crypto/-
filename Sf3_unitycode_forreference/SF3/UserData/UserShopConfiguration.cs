using System;
using System.Collections.Generic;
using Nekki;
using Nekki.Yaml;
using SF3.Items;
using sf3DTO;

namespace SF3.UserData
{
	public class UserShopConfiguration
	{
		private Action<bool> _OnConfigurationUpdate = delegate
		{
		};

		private Mapping _shopConfigurationNode;

		public readonly List<Action> OnShopRefreshUpdated = new List<Action>();

		public DateTime lastGenerationTime { get; private set; }

		public DateTime nextRefreshAvailableTime { get; private set; }

		public ShopCategoryType currentCategory { get; private set; }

		public int selectedItem { get; private set; }

		public bool IsShopRefreshInCooldown
		{
			get
			{
				return nextRefreshAvailableTime != DateTime.MinValue;
			}
		}

		public UserShopConfiguration(Mapping shopConfigurationNode, Action<bool> OnConfigurationUpdateCallback = null)
		{
			_shopConfigurationNode = shopConfigurationNode;
			_OnConfigurationUpdate = OnConfigurationUpdateCallback;
			lastGenerationTime = DateTime.MinValue;
			nextRefreshAvailableTime = DateTime.MinValue;
			currentCategory = ShopCategoryType.Weapon;
			selectedItem = -1;
			Scalar text = _shopConfigurationNode.GetText("LastGenerationTime");
			if (text != null)
			{
				long num = long.Parse(text.text);
				if (num > 0)
				{
					lastGenerationTime = NekkiUtils.GetUnixDateTimeFromMilliseconds(num);
					int integerConstant = JS.Instance.GetIntegerConstant("shopCooldown");
					nextRefreshAvailableTime = lastGenerationTime.AddMilliseconds(integerConstant);
				}
			}
			Scalar text2 = _shopConfigurationNode.GetText("SelectedCategory");
			if (text2 != null)
			{
				currentCategory = ComplianceUtils.GetShopCategoryTypeByName(text2.text);
				Scalar text3 = _shopConfigurationNode.GetText("SelectedItem");
				if (text3 != null)
				{
					selectedItem = int.Parse(text3.text);
				}
			}
		}

		public void UpdateData(Shop shopData)
		{
			if (shopData != null)
			{
				DateTime unixDateTimeFromMilliseconds = NekkiUtils.GetUnixDateTimeFromMilliseconds(shopData.LastGenerationTime.Value);
				if (unixDateTimeFromMilliseconds != lastGenerationTime)
				{
					lastGenerationTime = unixDateTimeFromMilliseconds;
					_shopConfigurationNode.GetText("LastGenerationTime").SetText(shopData.LastGenerationTime.Value.ToString());
					SetSelectedItem(-1);
				}
			}
			if (lastGenerationTime.GetUnixTimeStampMilliseconds() > 0)
			{
				int integerConstant = JS.Instance.GetIntegerConstant("shopCooldown");
				nextRefreshAvailableTime = lastGenerationTime.AddMilliseconds(integerConstant);
				NetworkConnection.TimeDispatcher.removeDelegate(CooldownUpdated);
				NetworkConnection.TimeDispatcher.callDelegateAt(nextRefreshAvailableTime.GetUnixTimeStampMilliseconds(), CooldownUpdated);
			}
			else
			{
				CooldownUpdated(shopData);
			}
			_OnConfigurationUpdate(true);
		}

		private void CooldownUpdated(object data)
		{
			nextRefreshAvailableTime = DateTime.MinValue;
			foreach (Action item in OnShopRefreshUpdated)
			{
				item();
			}
		}

		public void SetSelectedItem(int selectedItemID)
		{
			selectedItem = selectedItemID;
			_shopConfigurationNode.SetOrAddText("SelectedItem", selectedItem.ToString());
			_OnConfigurationUpdate(false);
		}

		public void SetCurrentCategory(ShopCategoryType currentCategoryValue, int selectedItemID)
		{
			currentCategory = currentCategoryValue;
			_shopConfigurationNode.SetOrAddText("SelectedCategory", currentCategory.ToString());
			SetSelectedItem(selectedItemID);
			_OnConfigurationUpdate(false);
		}
	}
}
