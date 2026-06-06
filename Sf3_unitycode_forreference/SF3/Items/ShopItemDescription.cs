using System;
using System.Collections.Generic;
using DG.Tweening;
using DOTweenUtils;
using Jint.Native;
using Nekki.UI;
using SF3.Settings;
using SF3.UserData;
using SF3_Attributes;
using UnityEngine;
using sf3DTO;

namespace SF3.Items
{
	public class ShopItemDescription : MonoBehaviour
	{
		[Serializable]
		public class AttributesViewSettings
		{
			public float iconScale = 1f;

			public int valueFontSize = 50;

			public int nameFontSize = 20;

			public float positionX = -90f;
		}

		[SerializeField]
		private NekkiUILabel _itemNameLbl;

		[SerializeField]
		private GameObject _rarityObject;

		[SerializeField]
		private GameObject _attributesObject;

		[SerializeField]
		private GameObject _buyForCoinsBtn;

		[SerializeField]
		private GameObject _buyForBonusBtn;

		[SerializeField]
		private NekkiUILabel _restriction;

		[SerializeField]
		private NekkiUILabel _requirement;

		[SerializeField]
		private GameObject _subjectInformationObject;

		[SerializeField]
		private NekkiUILabel _subjectInformationTextLbl;

		private UISprite _rarityLine;

		private NekkiUILabel _rarityText;

		private List<GameObject> _boosterpackLootPics = new List<GameObject>();

		private UILabel _buyForCoinsLbl;

		private UILabel _buyForBonusLbl;

		private Vector3 _coinsBtnPos;

		private Vector3 _bonusBtnPos;

		[SerializeField]
		private AttributesViewSettings[] _attributesViewSettins;

		[SerializeField]
		private AttributesDrawer _attributes;

		[SerializeField]
		private AttributesDrawer _upgradeAttributes;

		[SerializeField]
		private GameObject _progressPlaceholder;

		[SerializeField]
		private GameObject _progressPrf;

		private MultiCardProgress _progressCard;

		[SerializeField]
		private NekkiUILabel _upgradeLabel;

		[SerializeField]
		private GameObject _itemDescription;

		[SerializeField]
		private GameObject _boosterpackDescription;

		[SerializeField]
		private NekkiUILabel _boosterpackDescriptionLabel;

		[SerializeField]
		private GameObject _boosterpackLoot;

		[SerializeField]
		private GameObject _lootIconPrefab;

		[SerializeField]
		private float _itemDescriptionPositionY;

		[SerializeField]
		private float _itemDescriptionUpgradePositionY;

		[SerializeField]
		private float _upgradeFadeDuration = 0.5f;

		[SerializeField]
		private float _upgradeShowAttrTime = 1f;

		private Sequence _upgradeAttributesSequence;

		private SF3.UserData.ShopItem _currentShopItem;

		public new GameObject gameObject { get; private set; }

		public new Transform transform { get; private set; }

		public Collider OverlapCollider { get; private set; }

		public UIButton BuyForCoinsBtn { get; private set; }

		public UIButton BuyForBonusBtn { get; private set; }

		private void Awake()
		{
			gameObject = base.gameObject;
			transform = base.transform;
			OverlapCollider = gameObject.GetComponent<Collider>();
			if (_attributesViewSettins.Length == 0)
			{
				_attributesViewSettins = new AttributesViewSettings[1]
				{
					new AttributesViewSettings()
				};
			}
		}

		public void Initialize()
		{
			BuyForCoinsBtn = _buyForCoinsBtn.GetComponent<UIButton>();
			BuyForBonusBtn = _buyForBonusBtn.GetComponent<UIButton>();
			_buyForCoinsLbl = _buyForCoinsBtn.GetComponentInChildren<UILabel>();
			_buyForBonusLbl = _buyForBonusBtn.GetComponentInChildren<UILabel>();
			_coinsBtnPos = _buyForCoinsBtn.transform.localPosition;
			_bonusBtnPos = _buyForBonusBtn.transform.localPosition;
			_rarityLine = _rarityObject.GetComponentInChildren<UISprite>();
			_rarityText = _rarityObject.GetComponentInChildren<NekkiUILabel>();
			_upgradeLabel.alpha = 0f;
			ClearDescription();
			CreateProgress();
			UIWidget component = _attributes.GetComponent<UIWidget>();
			UIWidget component2 = _upgradeAttributes.GetComponent<UIWidget>();
			_upgradeAttributesSequence = DONgui.TwoWidgetBlinkSequence(component, component2, _upgradeFadeDuration, _upgradeShowAttrTime);
			SetItemDescriptionYPosition(_itemDescriptionPositionY);
			UserManager.AddActionForCurrency(CurrencyType.Bonus, RefreshBuyForBonusButton);
			UserManager.AddActionForCurrency(CurrencyType.Coin, RefreshBuyForCoinsButton);
		}

		private void OnDestroy()
		{
			UserManager.RemoveActionForCurrency(CurrencyType.Bonus, RefreshBuyForBonusButton);
			UserManager.RemoveActionForCurrency(CurrencyType.Coin, RefreshBuyForCoinsButton);
		}

		public void SetBuyButtonsEnabling(bool enabled)
		{
			BuyForCoinsBtn.enabled = enabled;
			_buyForBonusBtn.SetActive(enabled);
			BuyForBonusBtn.enabled = enabled;
			_buyForCoinsBtn.SetActive(enabled);
		}

		private void RefreshBuyForCoinsButton(long coinsInUser)
		{
			long currencyValue = _currentShopItem.GetCurrencyValue(CurrencyType.Coin);
			BuyForCoinsBtn.isEnabled = currencyValue <= coinsInUser;
		}

		private void RefreshBuyForBonusButton(long bonusInUser)
		{
			long currencyValue = _currentShopItem.GetCurrencyValue(CurrencyType.Bonus);
			BuyForBonusBtn.isEnabled = currencyValue <= bonusInUser;
		}

		public void ShowDescription(SF3.UserData.ShopItem itemShop, float animDuration = 0f)
		{
			_currentShopItem = itemShop;
			if (_currentShopItem != null)
			{
				if (_currentShopItem.item is Booster)
				{
					SetupBoosterpackDescription();
					SetupGuaranteedLootPictures();
				}
				else
				{
					SetupAttributes(animDuration);
				}
				SetupBuyButtons();
				SetupItemName();
				SetupRarity();
				SetupRequirement();
				SetupRestriction();
			}
		}

		private void SetupBuyButtons()
		{
			if (_currentShopItem != null)
			{
				long currencyValue = _currentShopItem.GetCurrencyValue(CurrencyType.Coin);
				long currencyValue2 = _currentShopItem.GetCurrencyValue(CurrencyType.Bonus);
				_buyForBonusBtn.SetActive(currencyValue2 > 0);
				if (_buyForBonusBtn.activeSelf)
				{
					_buyForBonusLbl.text = currencyValue2.ToString();
					BuyForBonusBtn.isEnabled = currencyValue2 <= UserManager.GetCurrencyValue(CurrencyType.Bonus);
				}
				_buyForCoinsBtn.SetActive(currencyValue > 0);
				if (_buyForCoinsBtn.activeSelf)
				{
					_buyForCoinsLbl.text = currencyValue.ToString();
					BuyForCoinsBtn.isEnabled = currencyValue <= UserManager.GetCurrencyValue(CurrencyType.Coin);
					_buyForCoinsBtn.transform.localPosition = ((!_buyForBonusBtn.activeSelf) ? _bonusBtnPos : _coinsBtnPos);
				}
			}
		}

		private void SetupRequirement()
		{
			Equipment equipment = _currentShopItem.item as Equipment;
			if (equipment != null)
			{
				_requirement.Format(equipment.level.ToString());
			}
			else
			{
				_requirement.Format("0");
			}
			_requirement.gameObject.SetActive(true);
		}

		private void SetupRestriction()
		{
			IFactionable factionable = _currentShopItem.item as IFactionable;
			if (factionable != null)
			{
				if (factionable.GetFactionType() != 0)
				{
					_restriction.gameObject.SetActive(true);
					string factionAlias = GameSettings.ItemSettings.GetFactionAlias(factionable.GetFactionType());
					_restriction.Format(Localization.Get(factionAlias).String);
				}
				else
				{
					_restriction.gameObject.SetActive(false);
				}
			}
		}

		private void SetupRarity()
		{
			IRarable rarable = _currentShopItem.item as IRarable;
			if (rarable != null)
			{
				_rarityLine.spriteName = GameSettings.ItemSettings.GetRarityDescriptionLine(rarable.GetRarityType());
				_rarityText.Alias = GameSettings.ItemSettings.GetRarityAlias(rarable.GetRarityType());
			}
			_rarityObject.SetActive(true);
		}

		private void SetupItemName()
		{
			_itemNameLbl.Alias = "shop_name_with_set_icon";
			_itemNameLbl.DefaultImageOffsetY = 3;
			_itemNameLbl.OffsetX = -22;
			_itemNameLbl.Format("set_icon", Localization.Get(_currentShopItem.item.alias).String);
		}

		private void SetupBoosterpackDescription()
		{
			Booster booster = _currentShopItem.item as Booster;
			_itemDescription.SetActive(false);
			_boosterpackDescription.SetActive(true);
			_progressPlaceholder.SetActive(false);
			_attributesObject.SetActive(false);
			_upgradeAttributes.gameObject.SetActive(false);
			_upgradeLabel.gameObject.SetActive(false);
			_boosterpackDescriptionLabel.Format(booster.Zone, JsFunction.GetBoosterSize(booster.id));
		}

		private void SetupGuaranteedLootPictures()
		{
			int id = _currentShopItem.item.id;
			int num = (int)JsFunction.GetBoosterSize(id);
			Dictionary<string, JsValue> boosterCardsRarities = JsFunction.GetBoosterCardsRarities(id);
			if (_boosterpackLootPics.Count > num)
			{
				DeleteGuaranteedLootPictures(_boosterpackLootPics.Count - num);
			}
			else if (_boosterpackLootPics.Count < num)
			{
				int count = _boosterpackLootPics.Count;
				for (int i = 0; i < num - count; i++)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(_lootIconPrefab);
					gameObject.transform.SetParent(_boosterpackLoot.transform);
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localScale = Vector3.one;
					_boosterpackLootPics.Add(gameObject);
				}
			}
			Dictionary<Rarity, int> dictionary = new Dictionary<Rarity, int>();
			foreach (KeyValuePair<string, JsValue> item in boosterCardsRarities)
			{
				switch (item.Key)
				{
				case "0":
					dictionary.Add(Rarity.UnknownRarity, item.Value.AsInteger());
					break;
				case "1":
					dictionary.Add(Rarity.Common, item.Value.AsInteger());
					break;
				case "2":
					dictionary.Add(Rarity.Rare, item.Value.AsInteger());
					break;
				case "3":
					dictionary.Add(Rarity.Epic, item.Value.AsInteger());
					break;
				case "4":
					dictionary.Add(Rarity.Legendary, item.Value.AsInteger());
					break;
				}
			}
			int num2 = 0;
			foreach (KeyValuePair<Rarity, int> item2 in dictionary)
			{
				for (int j = num2; j < item2.Value + num2; j++)
				{
					SetLootRarityColor(_boosterpackLootPics[_boosterpackLootPics.Count - 1 - j].GetComponent<NekkiUILabel>(), item2.Key);
				}
				num2 += item2.Value;
			}
		}

		private void SetLootRarityColor(NekkiUILabel label, Rarity rarity)
		{
			if (rarity == Rarity.Common)
			{
				label.enabled = true;
			}
			else
			{
				label.enabled = false;
			}
			UISprite component = label.gameObject.transform.GetChild(0).GetComponent<UISprite>();
			component.color = GameSettings.ItemSettings.GetRarityColor(rarity);
		}

		private void DeleteGuaranteedLootPictures(float cardsToDelete)
		{
			if (cardsToDelete != 0f)
			{
				for (int i = _boosterpackLootPics.Count - 1; (float)i > (float)(_boosterpackLootPics.Count - 1) - cardsToDelete; i++)
				{
					UnityEngine.Object.Destroy(_boosterpackLootPics[i].gameObject);
				}
			}
		}

		private void SetupAttributes(float animDuration = 0f)
		{
			BaseItem item = _currentShopItem.item;
			if (!(item is IAttributable) || !(item is Equipment))
			{
				return;
			}
			bool flag = item is Booster;
			_attributesObject.SetActive(!flag);
			_itemDescription.SetActive(!flag);
			_boosterpackDescription.SetActive(flag);
			_progressPlaceholder.SetActive(!flag);
			_upgradeAttributes.gameObject.SetActive(!flag);
			_upgradeLabel.gameObject.SetActive(!flag);
			Equipment equipped = UserManager.UserModelInfo.GetEquipped((item as Equipment).GetEquipmentType());
			BaseItem itemByID = UserManager.UserModelInfo.GetItemByID(item.id);
			IAttributable attributable = itemByID as IAttributable;
			IStackable stackable = itemByID as IStackable;
			IStackable newItem = item as IStackable;
			_upgradeAttributesSequence.Rewind();
			if (itemByID != null && stackable != null && attributable != null)
			{
				BaseItem baseItem = itemByID.Clone() as BaseItem;
				(baseItem as IStackable).MergeSimilarItems(newItem);
				SortedDictionary<AttributeType, float> attributesForDisplayData = (baseItem as IAttributable).GetAttributesForDisplayData();
				_upgradeLabel.gameObject.SetActive(true);
				ShowProgress(stackable.GetBarValue(), 0f, 1);
				_upgradeLabel.alpha = 1f;
				_upgradeAttributes.Draw(attributesForDisplayData, equipped.GetAttributesForDisplayData());
				_upgradeAttributesSequence.Restart();
				_upgradeAttributesSequence.PlayForward();
				_attributes.Draw(attributable.GetAttributesForDisplayData(), equipped.GetAttributesForDisplayData());
			}
			else
			{
				_upgradeLabel.gameObject.SetActive(false);
				HideProgress();
				SortedDictionary<AttributeType, float> attributesForDisplayData2 = (item as IAttributable).GetAttributesForDisplayData();
				if (equipped == null)
				{
					_attributes.Draw(attributesForDisplayData2);
				}
				else
				{
					_attributes.Draw(attributesForDisplayData2, equipped.GetAttributesForDisplayData());
				}
			}
		}

		public void ClearDescription(bool isAllCardsPurchased = false)
		{
			_buyForCoinsBtn.SetActive(false);
			_buyForBonusBtn.SetActive(false);
			_itemNameLbl.Alias = "shop_name_without_set_icon";
			_itemNameLbl.Format(string.Empty);
			_rarityObject.SetActive(false);
			_attributesObject.SetActive(false);
			_restriction.gameObject.SetActive(false);
			_requirement.gameObject.SetActive(false);
			_itemDescription.SetActive(false);
			_progressPlaceholder.SetActive(false);
			_upgradeAttributes.gameObject.SetActive(false);
			_upgradeLabel.gameObject.SetActive(false);
			if (isAllCardsPurchased)
			{
				_subjectInformationObject.SetActive(true);
				_subjectInformationTextLbl.Alias = "all_cards_purchased";
			}
			else
			{
				_subjectInformationObject.SetActive(false);
			}
		}

		private void CreateProgress()
		{
			GameObject gameObject = NGUITools.AddChild(_progressPlaceholder, _progressPrf);
			gameObject.SetActive(false);
			_progressCard = gameObject.GetComponent<MultiCardProgress>();
		}

		private void ShowProgress(float progress, float addedProgress, int levelups)
		{
			if (levelups > 1)
			{
				levelups = 1;
			}
			_progressCard.SetProgress(progress, addedProgress, levelups);
			_progressCard.gameObject.SetActive(true);
			SetItemDescriptionYPosition(_itemDescriptionUpgradePositionY);
		}

		private void HideProgress()
		{
			_progressCard.gameObject.SetActive(false);
			SetItemDescriptionYPosition(_itemDescriptionPositionY);
		}

		private void SetItemDescriptionYPosition(float y)
		{
			Vector3 localPosition = _itemDescription.transform.localPosition;
			_itemDescription.transform.localPosition = new Vector3(localPosition.x, y, localPosition.z);
		}

		private List<float> DiffAttributes(SortedDictionary<AttributeType, float> oldAttrs, SortedDictionary<AttributeType, float> newAttrs)
		{
			List<float> list = new List<float>();
			foreach (KeyValuePair<AttributeType, float> newAttr in newAttrs)
			{
				list.Add(newAttrs[newAttr.Key] - oldAttrs[newAttr.Key]);
			}
			return list;
		}

		public void AnimateProgressOnSelectedItem()
		{
			if (_progressCard.isActiveAndEnabled)
			{
				_progressCard.AnimateProgress();
			}
			else if (_progressCard.onAnimationEnd != null)
			{
				_progressCard.onAnimationEnd();
			}
		}

		public void AnimateAddedProgressOnSelectedItem()
		{
			if (_progressCard.isActiveAndEnabled)
			{
				_progressCard.AnimateAddedProgress();
			}
		}

		public void AddProgressAnimationCallback(MultiCardProgressAnimationEnd callback)
		{
			MultiCardProgress progressCard = _progressCard;
			progressCard.onAnimationEnd = (MultiCardProgressAnimationEnd)Delegate.Combine(progressCard.onAnimationEnd, callback);
		}

		public void BreakProgressAnimation()
		{
			_progressCard.BreakAnimation();
		}
	}
}
