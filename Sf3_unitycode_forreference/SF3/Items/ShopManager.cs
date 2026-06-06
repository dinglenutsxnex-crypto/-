using System;
using System.Collections;
using System.Collections.Generic;
using Nekki;
using SF3.Audio;
using SF3.GameModels;
using SF3.Moves;
using SF3.Settings;
using SF3.UserData;
using UnityEngine;
using sf3DTO;

namespace SF3.Items
{
	public class ShopManager : UIModuleHolder
	{
		private enum EShopState
		{
			OPEN = 0,
			CLOSE = 1,
			OPENING = 2,
			CLOSING = 3
		}

		private class ShopCategoryInfo
		{
			private int _selectedIndex;

			private UserShopCategory _categoryData;

			private readonly GameObject _categoryObject;

			private readonly UIButton _categoryButton;

			private readonly UISprite _categorySprite;

			private readonly UISprite _categoryBg;

			private readonly GameObject _lockIcon;

			private readonly GameObject _newItemNotificationIcon;

			private readonly UILabel _newItemNumLabel;

			public int SelectedIndex
			{
				get
				{
					return _selectedIndex;
				}
				set
				{
					if (_categoryData != null)
					{
						List<BaseItem> items = _categoryData.GetItems();
						if (items.Count > 0)
						{
							_selectedIndex = Mathf.Clamp(value, 0, _categoryData.items.Count - 1);
							SelectedShopItem = _categoryData.items[_selectedIndex];
						}
					}
					else
					{
						_selectedIndex = 0;
						SelectedShopItem = null;
					}
				}
			}

			public List<BaseItem> Items
			{
				get
				{
					return (_categoryData == null) ? null : _categoryData.GetItems();
				}
			}

			public SF3.UserData.ShopItem SelectedShopItem { get; private set; }

			public ShopCategoryType CategoryType { get; private set; }

			public long NewItems { get; protected set; }

			public bool IsLock
			{
				get
				{
					return _lockIcon.activeSelf;
				}
				set
				{
					_lockIcon.SetActive(value);
					_categoryObject.GetComponent<Collider>().enabled = !value;
					if (value)
					{
						DisableCategory();
						_categorySprite.color = CategoryDisableColor;
					}
					else
					{
						UnSelectCategory();
					}
				}
			}

			public ShopCategoryInfo(ShopCategoryType categoryTypeValue, Transform tranfs, UserShopCategory categoryDataValue = null)
			{
				CategoryType = categoryTypeValue;
				_categoryObject = tranfs.gameObject;
				_categoryButton = _categoryObject.GetComponent<UIButton>();
				_newItemNotificationIcon = tranfs.GetChild(0).gameObject;
				_newItemNumLabel = _newItemNotificationIcon.GetComponentInChildren<UILabel>();
				_lockIcon = tranfs.GetChild(1).gameObject;
				_categoryBg = tranfs.GetChild(2).GetComponent<UISprite>();
				_categorySprite = tranfs.GetChild(3).GetComponent<UISprite>();
				UpdateCategoryData(categoryDataValue);
			}

			public void SelectCategory()
			{
				_categorySprite.color = CategoryPressedColor;
				_categoryBg.color = CategoryPressedColorBg;
				_categoryButton.defaultColor = CategoryPressedColor;
			}

			public void UnSelectCategory()
			{
				_categorySprite.color = CategoryDefaultColor;
				_categoryBg.color = CategoryDefaultColorBg;
				_categoryButton.defaultColor = CategoryDefaultColor;
			}

			private void DisableCategory()
			{
				SelectedIndex = 0;
			}

			public void UpdateCategoryData(UserShopCategory categoryDataValue)
			{
				_categoryData = categoryDataValue;
				if (_categoryData != null && _categoryData.items.Count > 0)
				{
					_categoryData.CalculateItemsCurrency();
					NewItems = UserBadgesManager.Instance.WhichItemsisNew(UserBadgesManager.BadgeTypes.Shop, categoryDataValue.items);
					_newItemNotificationIcon.SetActive(NewItems > 0);
					_newItemNumLabel.text = NewItems.ToString();
					IsLock = false;
				}
				else
				{
					_newItemNotificationIcon.SetActive(false);
					IsLock = true;
				}
				if (SelectedShopItem == null)
				{
					SelectedIndex = 0;
				}
			}

			public bool SelectItem(int itemID)
			{
				List<BaseItem> items = _categoryData.GetItems();
				for (int i = 0; i < items.Count; i++)
				{
					if (items[i].id.Equals(itemID))
					{
						SelectedIndex = i;
						return true;
					}
				}
				SelectedIndex = 0;
				return false;
			}

			public void RemoveBadges()
			{
				_newItemNotificationIcon.SetActive(false);
				_categoryData.SetCategoryNotification(false);
			}
		}

		private const float REFRESH_TIMEOUT = 3f;

		private static ShopManager _instance;

		[SerializeField]
		private string _onOpenSoundName = string.Empty;

		[SerializeField]
		private string _onCloseSoundName = string.Empty;

		[SerializeField]
		private UnityEngine.Color _categoryPressedColor;

		[SerializeField]
		private UnityEngine.Color _categoryPressedColorBg;

		[SerializeField]
		private UnityEngine.Color _categoryDefaultColor;

		[SerializeField]
		private UnityEngine.Color _categoryDefaultColorBg;

		[SerializeField]
		private UnityEngine.Color _categoryDisableColor;

		private EShopState _shopState;

		private Vector3 _openRightPosition;

		private Vector3 _openLeftPosition;

		private Vector3 _closedRightPosition;

		private Vector3 _closedLeftPosition;

		[SerializeField]
		private float _openCloseTime = 60f;

		[SerializeField]
		private AnimationCurve _openCloseCurve;

		private float _nextOpenCloseTime;

		[SerializeField]
		private GameObject _shopHolder;

		private List<ReelItem> _reelItems;

		[SerializeField]
		private GameObject _contentPanel;

		[SerializeField]
		private GameObject _categoriesPanel;

		[SerializeField]
		private GameObject _cardsPanel;

		private BoxCollider _categoriesOverlapCollider;

		private BoxCollider _cardsOverlapCollider;

		[SerializeField]
		private BoxCollider _dragCardsBackCollider;

		[SerializeField]
		private GameObject _categories;

		private Dictionary<int, ShopCategoryInfo> _shopCategoriesSenders;

		[SerializeField]
		private GameObject _refreshBtn;

		[SerializeField]
		private UILabel _timeNow;

		[SerializeField]
		private GameObject _refreshLight;

		[SerializeField]
		private GameObject _itemCardPrefab;

		[SerializeField]
		private GameObject _itemBoosterPrefab;

		[SerializeField]
		private Transform _itemsParent;

		[SerializeField]
		private Vector2 _cardsOffset = new Vector2(50f, 50f);

		[SerializeField]
		private Vector2 _boosterOffset = new Vector2(10f, 10f);

		private bool _refreshInCooldown;

		[SerializeField]
		private readonly Vector3 _playerOffsetPosition = new Vector3(0f, 0f, -500f);

		private UIScrollViewCustom _cardsScroll;

		private Equipment _currentEquipmentSaved;

		private bool _tryOnActive;

		[SerializeField]
		private ShopItemDescription _itemDescription;

		[SerializeField]
		private GameObject _arrowUp;

		[SerializeField]
		private GameObject _arrowDown;

		[SerializeField]
		private float _cardScale = 0.6f;

		private int _columnsCount = 3;

		private UserShopConfiguration _currentShopConfiguration;

		private Dictionary<ShopCategoryType, ShopCategoryInfo> _shopCategories;

		private ShopCategoryInfo _currentCategoryInfo;

		private float _itemsPadding;

		[SerializeField]
		private float _timeToShowTryOn = 2f;

		[SerializeField]
		private float _itemDarken = 0.5f;

		[SerializeField]
		private string _buyIconSprite = "buy_icon";

		[SerializeField]
		private string _upIconSprite = "up-icon";

		[SerializeField]
		private float _upgradeAttrDuration = 0.3f;

		private const float BOOSTER_REQUEST_TIMEOUT = 10f;

		private IEnumerator _tryOnCorutine;

		private Coroutine _delayedSetScreenTextureCoroutine;

		private readonly string SERVER_WAIT_ALIAS = "server_response_wait";

		private Coroutine _dragColliderCrutchRoutine;

		private AiMode _aiModeCurrent;

		private ShopIntentModule _intent;

		private Action _callbackOnOpen;

		private Action _callbackOnClosed;

		public static ShopManager Instance
		{
			get
			{
				return _instance;
			}
		}

		public static UnityEngine.Color CategoryPressedColor
		{
			get
			{
				return Instance._categoryPressedColor;
			}
		}

		public static UnityEngine.Color CategoryPressedColorBg
		{
			get
			{
				return Instance._categoryPressedColorBg;
			}
		}

		public static UnityEngine.Color CategoryDefaultColor
		{
			get
			{
				return Instance._categoryDefaultColor;
			}
		}

		public static UnityEngine.Color CategoryDefaultColorBg
		{
			get
			{
				return Instance._categoryDefaultColorBg;
			}
		}

		public static UnityEngine.Color CategoryDisableColor
		{
			get
			{
				return Instance._categoryDisableColor;
			}
		}

		private Model _playerModel
		{
			get
			{
				return ModelsManager.Instance.Player;
			}
		}

		public event Action OnBuySuccessful;

		protected override void Awake()
		{
			base.Awake();
			_instance = this;
		}

		public override void Initialize()
		{
			UserShopManager.OnUserShopUpdated += UpdateShopData;
			_currentShopConfiguration = UserShopManager.Instance.shopConfiguration;
			ItemBuyingController.Instance.OnSuccessBuy += OnBuyItemSuccess;
			ItemBuyingController.Instance.OnFailureBuy += OnBuyItemFailure;
			_categoriesOverlapCollider = _categoriesPanel.GetComponent<BoxCollider>();
			_cardsOverlapCollider = _cardsPanel.GetComponent<BoxCollider>();
			_cardsScroll = _itemsParent.GetComponent<UIScrollViewCustom>();
			_itemsPadding = _cardsOffset.y;
			_cardsScroll.padding = new Vector2(0f, _itemsPadding);
			_refreshBtn.GetComponent<Collider>().enabled = true;
			InitShopCategories();
			_itemDescription.Initialize();
			_itemDescription.AddProgressAnimationCallback(OnDescriptionProgressAnimationEnd);
			InitShopPartsPositions();
			InitButtons();
			_shopState = EShopState.CLOSING;
			ChangeShopState(EShopState.CLOSE);
			AudioManager.Instance.LoadSound(_onOpenSoundName, _onCloseSoundName);
			_currentShopConfiguration.OnShopRefreshUpdated.Add(UpdateRefreshTimerState);
			_shopHolder.SetActive(false);
			base.gameObject.SetActive(false);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			UserShopManager.OnUserShopUpdated -= UpdateShopData;
			ItemBuyingController.Instance.OnSuccessBuy -= OnBuyItemSuccess;
			ItemBuyingController.Instance.OnFailureBuy -= OnBuyItemFailure;
			if (_currentShopConfiguration != null)
			{
				_currentShopConfiguration.OnShopRefreshUpdated.Remove(UpdateRefreshTimerState);
			}
		}

		public void UpdateShopData()
		{
			if (_shopState != 0 && _shopState != EShopState.OPENING)
			{
				return;
			}
			UpdateRefreshTimerState();
			foreach (ShopCategoryInfo value in _shopCategories.Values)
			{
				value.UpdateCategoryData(UserShopManager.Instance.GetShopCategoryData(value.CategoryType));
			}
			ShopCategoryType categoryType = _currentCategoryInfo.CategoryType;
			if (_currentCategoryInfo.IsLock)
			{
				foreach (ShopCategoryInfo value2 in _shopCategories.Values)
				{
					if (!value2.IsLock)
					{
						categoryType = value2.CategoryType;
						break;
					}
				}
			}
			else if (!_currentCategoryInfo.SelectItem(_currentShopConfiguration.selectedItem))
			{
				_currentShopConfiguration.SetSelectedItem(_currentCategoryInfo.SelectedShopItem.item.id);
			}
			SelectCategory(categoryType);
		}

		private IEnumerator DragColliderCrutch()
		{
			_dragCardsBackCollider.gameObject.SetActive(false);
			yield return new WaitForSeconds(1.5f);
			_dragCardsBackCollider.gameObject.SetActive(true);
		}

		private void InitShopCategories()
		{
			_shopCategories = new Dictionary<ShopCategoryType, ShopCategoryInfo>();
			_shopCategoriesSenders = new Dictionary<int, ShopCategoryInfo>();
			foreach (ShopCategoryType enumerator3 in EnumsCompliancer.GetEnumerators<ShopCategoryType>())
			{
				if (enumerator3 == ShopCategoryType.None)
				{
					continue;
				}
				string value = enumerator3.ToString().ToUpper();
				foreach (Transform item in _categories.transform)
				{
					if (item.name.ToUpper().Contains(value))
					{
						ShopCategoryInfo value2 = new ShopCategoryInfo(enumerator3, item);
						_shopCategories.Add(enumerator3, value2);
						_shopCategoriesSenders.Add(item.gameObject.GetInstanceID(), value2);
						break;
					}
				}
			}
			_currentCategoryInfo = _shopCategories[_currentShopConfiguration.currentCategory];
		}

		private void InitButtons()
		{
			_itemDescription.BuyForCoinsBtn.onClick.Add(new EventDelegate(delegate
			{
				BuyItem(CurrencyType.Coin);
			}));
			_itemDescription.BuyForBonusBtn.onClick.Add(new EventDelegate(delegate
			{
				BuyItem(CurrencyType.Bonus);
			}));
		}

		private void InitShopPartsPositions()
		{
			_cardsPanel.GetComponent<UICameraAnchor>().ForcedUpdate();
			_categoriesPanel.GetComponent<UICameraAnchor>().ForcedUpdate();
			_itemDescription.GetComponent<UICameraAnchor>().ForcedUpdate();
			InitCards(_currentCategoryInfo.CategoryType == ShopCategoryType.Booster);
			_openLeftPosition = _contentPanel.transform.localPosition;
			_openRightPosition = _itemDescription.transform.localPosition;
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(_contentPanel.transform);
			_closedLeftPosition = _openLeftPosition;
			_closedLeftPosition.x -= bounds.max.x - _contentPanel.transform.InverseTransformPoint(UICamera.mainCamera.ViewportToWorldPoint(new Vector2(0f, 0f))).x;
			bounds = NGUIMath.CalculateRelativeWidgetBounds(((Component)_itemDescription).transform);
			_closedRightPosition = _openRightPosition;
			_closedRightPosition.x += _itemDescription.transform.InverseTransformPoint(UICamera.mainCamera.ViewportToWorldPoint(new Vector2(1f, 1f))).x - bounds.min.x;
		}

		private void InitCards(bool isBoosterpack)
		{
			if (_reelItems != null)
			{
				foreach (ReelItem reelItem in _reelItems)
				{
					UnityEngine.Object.Destroy(reelItem.gameObject);
				}
			}
			Vector2 vector = ((!isBoosterpack) ? _cardsOffset : _boosterOffset);
			_reelItems = new List<ReelItem>();
			Bounds bounds = NGUIMath.CalculateAbsoluteWidgetBounds(_categoriesPanel.transform);
			Vector3 vector2 = _shopHolder.transform.InverseTransformPoint(bounds.max);
			Bounds bounds2 = NGUIMath.CalculateAbsoluteWidgetBounds(((Component)_itemDescription).transform);
			Vector3 vector3 = _shopHolder.transform.InverseTransformPoint(bounds2.min);
			float num = vector3.x - vector2.x;
			float num2 = 0f;
			Vector2 vector4 = ((!isBoosterpack) ? _itemCardPrefab : _itemBoosterPrefab).GetComponent<ReelItem>().LocalSize() * _cardScale;
			_columnsCount = 0;
			while (num > num2)
			{
				num2 += vector4.x + vector.x;
				if (_columnsCount == 0)
				{
					num2 -= vector.x;
				}
				_columnsCount++;
			}
			num2 -= vector4.x + vector.x;
			_columnsCount--;
			_cardsScroll.cellSize = new Vector2(vector4.x + vector.x, vector4.y + vector.y);
			float num3 = vector3.x - vector2.x;
			Vector3 position = _cardsPanel.transform.position;
			position.x = bounds.max.x + (num3 - num2) / 2f * _cardsPanel.transform.lossyScale.x;
			_cardsPanel.transform.position = position;
			Vector3 size = _cardsOverlapCollider.size;
			size.x = num2;
			_cardsOverlapCollider.size = size;
			Vector3 center = _cardsOverlapCollider.center;
			center.x = size.x / 2f;
			_cardsOverlapCollider.center = center;
			size = _dragCardsBackCollider.size;
			size.x = num2;
			_dragCardsBackCollider.size = size;
			center = _dragCardsBackCollider.center;
			center.x = size.x / 2f;
			_dragCardsBackCollider.center = center;
			UIPanel component = _itemsParent.GetComponent<UIPanel>();
			component.SetRect(num2 / 2f + 5f, component.baseClipRegion.y, num2 + component.clipSoftness.x * 2f + 15f + _itemsPadding, vector4.y * 2f + vector.y * 2f + component.clipSoftness.y + _itemsPadding / 2f);
			int shopCategoryItemCount = UserShopManager.Instance.GetShopCategoryItemCount(_currentCategoryInfo.CategoryType);
			for (int i = 0; i < shopCategoryItemCount; i++)
			{
				CreateNewItem(isBoosterpack);
				_reelItems[i].UpdateDepth(i);
			}
			_cardsScroll.InvalidateBounds();
			_cardsScroll.UpdatePosition();
			Vector3 center2 = NGUIMath.CalculateRelativeWidgetBounds(_cardsPanel.transform).center;
			center2.y = _arrowUp.transform.localPosition.y;
			_arrowUp.transform.localPosition = center2;
			center2.y = _arrowDown.transform.localPosition.y;
			_arrowDown.transform.localPosition = center2;
		}

		private IEnumerator DisableRecalcBoundsOnNextFrame()
		{
			yield return new WaitForEndOfFrame();
			_cardsScroll.disableRecalcBounds = true;
		}

		private void CreateNewItem(bool isBoosterpack)
		{
			int num = _reelItems.Count / _columnsCount;
			int num2 = _reelItems.Count - num * _columnsCount;
			Vector2 vector = ((!isBoosterpack) ? _cardsOffset : _boosterOffset);
			GameObject gameObject = ((!isBoosterpack) ? UnityEngine.Object.Instantiate(_itemCardPrefab) : UnityEngine.Object.Instantiate(_itemBoosterPrefab));
			gameObject.transform.parent = _itemsParent;
			Vector3 vector2 = gameObject.GetComponent<ReelItem>().LocalSize() * _cardScale;
			gameObject.transform.localPosition = new Vector3(vector2.x * (float)num2 + vector.x * (float)num2 + vector2.x / 2f, 0f - (vector2.y * (float)num + vector.y * (float)num), 0f);
			int i = _reelItems.Count;
			gameObject.transform.localScale = new Vector3(_cardScale, _cardScale, _cardScale);
			ReelItem newItem = gameObject.GetComponent<ReelItem>();
			newItem.Button.gameObject.AddComponent<UIDragScrollView>();
			EventDelegate newCallBack = new EventDelegate(delegate
			{
				if (_currentCategoryInfo.SelectedIndex == i)
				{
					if (newItem is CardItem)
					{
						(newItem as CardItem).ShowTryOnBtn();
					}
				}
				else
				{
					SelectItemCard(i);
					SF3UiLogger.instance.AddShopSelectItemEvent(_reelItems[i].Item);
				}
			});
			newItem.AddClickCallback(newCallBack);
			if (newItem is CardItem)
			{
				CardItem cardItem = newItem as CardItem;
				cardItem.OnFlipEnded += OnCardFlipEnded;
				cardItem.AddTryOnCallback(new EventDelegate(TryOnItem));
				cardItem.UpdateShade(_itemDarken);
			}
			newItem.ActivateSelectionBorder(false);
			newItem.gameObject.SetActive(false);
			_reelItems.Add(newItem);
		}

		public void RefreshCategories()
		{
			SF3UiLogger.instance.AddButtonClickEvent(ConstantsSF3.ELocationSceneModule.Shop, "refresh");
			UserBadgesManager.Instance.Clear(UserBadgesManager.BadgeTypes.Shop);
			BlockRefreshScreenUI();
			UserDataController.Send_GenerateShop(3f);
			StartCoroutine(WaitingForResponce());
		}

		private IEnumerator WaitingForResponce()
		{
			while (UserDataController.waitingForGenerateShop)
			{
				yield return new WaitForEndOfFrame();
			}
			UnBlockRefreshScreenUI();
		}

		private void BlockRefreshScreenUI()
		{
			UIBlocker.Instance.Block(UIBlocker.Priority.Preloader);
			if (_delayedSetScreenTextureCoroutine != null)
			{
				StopCoroutine(_delayedSetScreenTextureCoroutine);
			}
			_delayedSetScreenTextureCoroutine = StartCoroutine(DelayedSetScreenTexture());
		}

		private void UnBlockRefreshScreenUI()
		{
			if (_delayedSetScreenTextureCoroutine == null)
			{
				LoadingIcon.Instance.DisableLoadingScreen(0.5f, delegate
				{
					UIBlocker.Instance.Unblock(UIBlocker.Priority.Preloader);
				}, SERVER_WAIT_ALIAS);
			}
			else
			{
				StopCoroutine(_delayedSetScreenTextureCoroutine);
				_delayedSetScreenTextureCoroutine = null;
				UIBlocker.Instance.Unblock(UIBlocker.Priority.Preloader);
			}
			SelectItemCard(0);
		}

		private IEnumerator DelayedSetScreenTexture()
		{
			yield return new WaitForSeconds(GameSettings.clientSettings.DelayForWaitResponseBeforeBlock);
			LoadingIcon.Instance.EnableLoadingScreen(SERVER_WAIT_ALIAS);
			_delayedSetScreenTextureCoroutine = null;
		}

		public override void ShowModule(IntentModule intent, Action callbackOnOpen)
		{
			_intent = (ShopIntentModule)intent;
			_callbackOnOpen = callbackOnOpen;
			if (ModelsManager.Instance.Enemy != null)
			{
				_aiModeCurrent = ModelsManager.Instance.Enemy.GetAiMode();
				ModelsManager.Instance.Enemy.SetAiMode(AiMode.NoneMode);
			}
			base.gameObject.SetActive(true);
			_shopHolder.SetActive(true);
			LocationColorAnimation.Instance.Animate(GameSettings.DojoSettings.LocationColorChangeTime, GameSettings.DojoSettings.DefaultLocationColor, GameSettings.DojoSettings.LocationColorInModule);
			LocationColorAnimation.Instance.EnableRays(false, GameSettings.DojoSettings.LocationColorChangeTime);
			if (ModelsManager.Instance.Player != null)
			{
				BattleCamera.MoveToPlayer(_playerOffsetPosition, null, _intent.IsInstant());
			}
			HolderModule.EnableControls(false);
			ModelsManager.Instance.HideModels(1f);
			ChangeShopState(EShopState.OPENING);
			UpdateShopData();
			UpdateModule(intent);
			AudioManager.Instance.PlaySound(_onOpenSoundName);
			StartCoroutine(DisableRecalcBoundsOnNextFrame());
		}

		public override void UpdateModule(IntentModule intent)
		{
			_intent = (ShopIntentModule)intent;
			if (_intent.Category != 0)
			{
				SelectCategory(_intent.Category);
				if (_intent.ItemId != -1 && _currentCategoryInfo.SelectedShopItem.item.id != _intent.ItemId)
				{
					UnselectCurrentItemCard();
					_currentCategoryInfo.SelectItem(_intent.ItemId);
					SelectItemCard(_currentCategoryInfo.SelectedIndex);
				}
			}
			else
			{
				SelectCategory(_currentCategoryInfo.CategoryType);
			}
			if (_shopState == EShopState.OPEN)
			{
				StoreOpened();
			}
		}

		private void StoreOpened()
		{
			_callbackOnOpen.InvokeSafe();
		}

		public override void HideModule(Action callbackOnClosed)
		{
			_callbackOnClosed = callbackOnClosed;
			LocationColorAnimation.Instance.Animate(GameSettings.DojoSettings.LocationColorChangeTime, GameSettings.DojoSettings.LocationColorInModule, GameSettings.DojoSettings.DefaultLocationColor);
			LocationColorAnimation.Instance.EnableRays(true, GameSettings.DojoSettings.LocationColorChangeTime);
			if (_tryOnActive)
			{
				EndTryOnItem();
			}
			ChangeShopState(EShopState.CLOSING);
			AudioManager.Instance.PlaySound(_onCloseSoundName);
			if (ModelsManager.Instance.Enemy != null)
			{
				ModelsManager.Instance.Enemy.SetAiMode(_aiModeCurrent);
			}
		}

		private void StoreClosed()
		{
			_shopHolder.SetActive(false);
			base.gameObject.SetActive(false);
			_callbackOnClosed.InvokeSafe();
		}

		private string FormatRefreshTime()
		{
			TimeSpan timeSpan = _currentShopConfiguration.nextRefreshAvailableTime - NetworkConnection.current.getCurrentServerDateTime();
			string text = null;
			if (timeSpan.Days > 0)
			{
				return string.Format("{0}d {1:00}:{2:00}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes);
			}
			if (timeSpan.Hours > 0)
			{
				return string.Format("{0:00}:{1:00}", timeSpan.Hours, timeSpan.Minutes);
			}
			return string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
		}

		private void UpdateRefreshTimerState()
		{
			_refreshInCooldown = UserShopManager.Instance.shopConfiguration.IsShopRefreshInCooldown;
			_refreshBtn.GetComponent<Collider>().enabled = !_refreshInCooldown;
			UIButton component = _refreshBtn.GetComponent<UIButton>();
			UnityEngine.Color defaultColor = component.defaultColor;
			if (_refreshInCooldown)
			{
				_timeNow.enabled = true;
				FormatRefreshTime();
				_refreshLight.SetActive(false);
				defaultColor.a = 0.5f;
			}
			else
			{
				_timeNow.enabled = false;
				_refreshLight.SetActive(true);
				defaultColor.a = 1f;
			}
			component.defaultColor = defaultColor;
			component.pressed = defaultColor;
			component.hover = defaultColor;
			component.disabledColor = defaultColor;
		}

		private void Update()
		{
			UpdateRefreshTimer();
			UpdateShopState();
		}

		private void UpdateRefreshTimer()
		{
			if (_refreshInCooldown)
			{
				_timeNow.text = FormatRefreshTime();
			}
		}

		private void UpdateShopState()
		{
			if (_shopState == EShopState.OPENING)
			{
				float num = (_nextOpenCloseTime - GameTimeController.battleTime) / _openCloseTime;
				if (num <= 0f)
				{
					ChangeShopState(EShopState.OPEN);
					StoreOpened();
				}
				else
				{
					_contentPanel.transform.localPosition = GetNewPosition(_closedLeftPosition, _openLeftPosition, _openCloseCurve.Evaluate(num));
					_itemDescription.transform.localPosition = GetNewPosition(_closedRightPosition, _openRightPosition, _openCloseCurve.Evaluate(num));
				}
			}
			else
			{
				if (_shopState != EShopState.CLOSING)
				{
					return;
				}
				float num2 = (_nextOpenCloseTime - GameTimeController.battleTime) / _openCloseTime;
				if (num2 <= 0f)
				{
					ChangeShopState(EShopState.CLOSE);
					if (!_tryOnActive)
					{
						StoreClosed();
					}
				}
				else
				{
					_contentPanel.transform.localPosition = GetNewPosition(_openLeftPosition, _closedLeftPosition, _openCloseCurve.Evaluate(num2));
					_itemDescription.transform.localPosition = GetNewPosition(_openRightPosition, _closedRightPosition, _openCloseCurve.Evaluate(num2));
				}
			}
		}

		private Vector3 GetNewPosition(Vector3 firstPosition, Vector3 secondPosition, float newTime)
		{
			return Vector3.Lerp(firstPosition, secondPosition, newTime);
		}

		private void ChangeShopState(EShopState newState)
		{
			if (_shopState == newState)
			{
				return;
			}
			switch (newState)
			{
			case EShopState.OPENING:
			case EShopState.CLOSING:
				if (_shopState == EShopState.OPENING || _shopState == EShopState.CLOSING)
				{
					_nextOpenCloseTime = _openCloseTime - (_nextOpenCloseTime - GameTimeController.battleTime) + GameTimeController.battleTime;
				}
				else
				{
					_nextOpenCloseTime = GameTimeController.battleTime + _openCloseTime;
				}
				EnableColliders(true);
				break;
			case EShopState.OPEN:
				_contentPanel.transform.localPosition = _openLeftPosition;
				_itemDescription.transform.localPosition = _openRightPosition;
				EnableColliders(false);
				break;
			case EShopState.CLOSE:
				_contentPanel.transform.localPosition = _closedLeftPosition;
				_itemDescription.transform.localPosition = _closedRightPosition;
				EnableColliders(false);
				break;
			}
			_shopState = newState;
		}

		private void EnableColliders(bool enable)
		{
			_categoriesOverlapCollider.enabled = enable;
			_cardsOverlapCollider.enabled = enable;
			_itemDescription.OverlapCollider.enabled = enable;
		}

		private void UnselectCurrentItemCard()
		{
			if (_currentCategoryInfo.SelectedShopItem != null)
			{
				if (_currentCategoryInfo.SelectedIndex >= _reelItems.Count)
				{
					return;
				}
				_reelItems[_currentCategoryInfo.SelectedIndex].ActivateSelectionBorder(false);
				if (_reelItems[_currentCategoryInfo.SelectedIndex] is CardItem)
				{
					CardItem cardItem = _reelItems[_currentCategoryInfo.SelectedIndex] as CardItem;
					cardItem.HideTryOnBtn();
					cardItem.UpdateShade(_itemDarken);
				}
			}
			_itemDescription.ClearDescription();
			if (_tryOnCorutine != null)
			{
				StopCoroutine(_tryOnCorutine);
			}
		}

		private void SelectCategory(ShopCategoryType category)
		{
			if (_dragColliderCrutchRoutine != null)
			{
				StopCoroutine(_dragColliderCrutchRoutine);
			}
			_dragColliderCrutchRoutine = StartCoroutine(DragColliderCrutch());
			List<BaseItem> items = _shopCategories[category].Items;
			if (items == null || items.Count == 0)
			{
				foreach (ShopCategoryInfo value in _shopCategories.Values)
				{
					List<BaseItem> items2 = value.Items;
					if (items2 != null && items2.Count > 0)
					{
						category = value.CategoryType;
						break;
					}
				}
			}
			UnselectCurrentItemCard();
			_currentCategoryInfo.UnSelectCategory();
			_currentCategoryInfo = _shopCategories[category];
			_currentCategoryInfo.SelectCategory();
			UserBadgesManager.Instance.Reset(UserBadgesManager.BadgeTypes.Shop, items);
			_currentCategoryInfo.RemoveBadges();
			_currentShopConfiguration.SetCurrentCategory(category, _currentCategoryInfo.SelectedShopItem.item.id);
			UpdateIntent(category, _currentCategoryInfo.SelectedShopItem.item.id);
			UpdateItemCards(category == ShopCategoryType.Booster);
		}

		private void UpdateIntent(ShopCategoryType category, int itemId)
		{
			if (_intent.Category != category)
			{
				_intent.SetCategory(category);
				_intent.SetItemId(itemId);
			}
			else if (_intent.ItemId != itemId)
			{
				_intent.SetItemId(itemId);
			}
		}

		private void SelectItemCard(int itemCardIndex)
		{
			_itemDescription.BreakProgressAnimation();
			UnselectCurrentItemCard();
			_currentCategoryInfo.SelectedIndex = itemCardIndex;
			_reelItems[_currentCategoryInfo.SelectedIndex].ActivateSelectionBorder(true);
			_itemDescription.ShowDescription(_currentCategoryInfo.SelectedShopItem);
			if (_reelItems[_currentCategoryInfo.SelectedIndex] is CardItem)
			{
				CardItem cardItem = _reelItems[_currentCategoryInfo.SelectedIndex] as CardItem;
				if (!cardItem.IsBackClothNow)
				{
					cardItem.UpdateShade(0f);
				}
			}
			_cardsScroll.ScrollVerticalToCell((int)Mathf.Floor(_currentCategoryInfo.SelectedIndex / _columnsCount));
			_currentShopConfiguration.SetSelectedItem(_currentCategoryInfo.SelectedShopItem.item.id);
			UpdateIntent(_currentCategoryInfo.CategoryType, _currentCategoryInfo.SelectedShopItem.item.id);
			_tryOnCorutine = TryOnAutoShow();
			StartCoroutine(_tryOnCorutine);
		}

		private IEnumerator TryOnAutoShow()
		{
			yield return new WaitForSeconds(_timeToShowTryOn);
			if (_reelItems[_currentCategoryInfo.SelectedIndex] is CardItem)
			{
				(_reelItems[_currentCategoryInfo.SelectedIndex] as CardItem).ShowTryOnBtn();
			}
		}

		public void BuyItem(CurrencyType currencyType)
		{
			if (_currentCategoryInfo.SelectedShopItem != null)
			{
				if (_currentCategoryInfo.SelectedShopItem.item is Booster)
				{
					Currency currency = new Currency();
					currency.CurrencyType = currencyType;
					currency.Value = _currentCategoryInfo.SelectedShopItem.GetCurrencyValue(currencyType);
					BuyBoosterRequest buyBoosterRequest = new BuyBoosterRequest();
					buyBoosterRequest.ShopBoosterModelId = _currentCategoryInfo.SelectedShopItem.BoosterpackID;
					buyBoosterRequest.Currency = currency;
					BuyBoosterRequest request = buyBoosterRequest;
					UserDataController.Send_BuyBooster(request, 10f);
				}
				else
				{
					ItemBuyingController.Instance.BuyItem(_currentCategoryInfo.SelectedShopItem, currencyType);
				}
			}
		}

		internal void OnBuyItemSuccess()
		{
			if (_currentCategoryInfo != null)
			{
				if (ModelsManager.Instance.Player != null && !ModelsManager.Instance.Player.IsEquipmentEquipped(_currentCategoryInfo.SelectedShopItem.item.id))
				{
					ModelsManager.Instance.Player.EquipItem(_currentCategoryInfo.SelectedShopItem.item.id);
				}
				else
				{
					UserManager.UserModelInfo.EquipItem(_currentCategoryInfo.SelectedShopItem.item.id);
				}
				GlobalLoad.UnloadUnusedAssets();
				this.OnBuySuccessful.InvokeSafe();
				_itemDescription.AnimateProgressOnSelectedItem();
				if (_reelItems[_currentCategoryInfo.SelectedIndex] is CardItem)
				{
					(_reelItems[_currentCategoryInfo.SelectedIndex] as CardItem).ShowShopIcon(_upIconSprite);
				}
				else
				{
					SystemMessage systemMessage = SystemMessage.ShowAlert("shop_boosterBuySuccessful");
					systemMessage.AddButton("continue");
					systemMessage.Show();
				}
				_reelItems[_currentCategoryInfo.SelectedIndex].UpdateItem();
			}
		}

		internal void OnBuyItemFailure()
		{
		}

		private void OnDescriptionProgressAnimationEnd()
		{
			_itemDescription.BreakProgressAnimation();
			_itemDescription.ShowDescription(_currentCategoryInfo.SelectedShopItem, _upgradeAttrDuration);
			_itemDescription.AnimateAddedProgressOnSelectedItem();
		}

		private void OnCardFlipEnded()
		{
			if (_currentCategoryInfo.SelectedShopItem != null && !_cardsScroll.isScrolling)
			{
				_itemDescription.SetBuyButtonsEnabling(true);
			}
		}

		public void TryOnItem()
		{
			SF3UiLogger.instance.AddButtonClickEvent(ConstantsSF3.ELocationSceneModule.Shop, "try_on");
			if (!_tryOnActive)
			{
				ModelsManager.Instance.ShowPlayer(1f);
				_tryOnActive = true;
				_currentEquipmentSaved = _playerModel.GetEquippedForType((_currentCategoryInfo.SelectedShopItem.item as Equipment).GetEquipmentType());
				_playerModel.EquipItemNotExisted(_currentCategoryInfo.SelectedShopItem.item as Equipment, false);
				BattleController.ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_TRY_ON, ModelsManager.Instance.Player.id, (Equipment)_currentCategoryInfo.SelectedShopItem.item));
				ChangeShopState(EShopState.CLOSING);
				if (!UserManager.IsTutorialComplete())
				{
					UIBlocker.Instance.Block();
				}
			}
		}

		public void EndTryOnItem()
		{
			if (_tryOnActive)
			{
				ModelsManager.Instance.HidePlayer(1f);
				_tryOnActive = false;
				_playerModel.EquipItem(_currentEquipmentSaved.id, false);
				_currentEquipmentSaved = null;
				ChangeShopState(EShopState.OPENING);
				GlobalLoad.UnloadUnusedAssets();
			}
		}

		public void ScrollCardsUp()
		{
			_cardsScroll.ScrollUpByCell();
		}

		public void ScrollCardsDown()
		{
			_cardsScroll.ScrollDownByCell();
		}

		public void ShowCategory(GameObject senderObject)
		{
			int instanceID = senderObject.GetInstanceID();
			if (_shopCategoriesSenders.ContainsKey(instanceID) && _currentCategoryInfo.CategoryType != _shopCategoriesSenders[instanceID].CategoryType)
			{
				string srcSlot = _currentCategoryInfo.CategoryType.ToString();
				string dstSlot = _shopCategoriesSenders[instanceID].CategoryType.ToString();
				long newItems = _shopCategoriesSenders[instanceID].NewItems;
				SF3UiLogger.instance.AddShopSlotChangeEvent(dstSlot, srcSlot, newItems);
				SelectCategory(_shopCategoriesSenders[instanceID].CategoryType);
			}
		}

		private void UpdateItemCards(bool changeToBoosterpack)
		{
			List<BaseItem> items = _currentCategoryInfo.Items;
			for (int i = 0; i < _reelItems.Count; i++)
			{
				_reelItems[i].gameObject.SetActive(false);
			}
			UnselectCurrentItemCard();
			if (items == null || items.Count == 0)
			{
				return;
			}
			if ((_reelItems[0] is CardItem && changeToBoosterpack) || (_reelItems[0] is BoosterpackItem && !changeToBoosterpack))
			{
				InitCards(changeToBoosterpack);
			}
			else if (_reelItems.Count < items.Count)
			{
				int num = _currentCategoryInfo.Items.Count - _reelItems.Count;
				for (int j = 0; j < num; j++)
				{
					CreateNewItem(changeToBoosterpack);
					_reelItems[j].UpdateDepth(j);
				}
			}
			for (int k = 0; k < _reelItems.Count; k++)
			{
				_reelItems[k].gameObject.SetActive(true);
				if (_reelItems[k] is CardItem)
				{
					CardItem cardItem = _reelItems[k] as CardItem;
					cardItem.ResetFlip();
					if (k < items.Count)
					{
						cardItem.Init(items[k]);
						cardItem.ActivateSelectionBorder(false);
						cardItem.ActivateButton(true);
						BaseItem itemByID = UserManager.UserModelInfo.GetItemByID(items[k].id);
						if (itemByID != null)
						{
							cardItem.ShowShopIcon(_upIconSprite);
						}
						else
						{
							cardItem.ShowShopIcon(_buyIconSprite);
						}
						cardItem.SetImage();
					}
					else
					{
						cardItem.FlipHorizontally();
						cardItem.HideShopIcon();
						cardItem.HideSale();
					}
				}
				else
				{
					BoosterpackItem boosterpackItem = _reelItems[k] as BoosterpackItem;
					if (k < items.Count)
					{
						boosterpackItem.Init(items[k]);
						boosterpackItem.ActivateButton(true);
						boosterpackItem.ActivateSelectionBorder(false);
					}
				}
			}
			SelectItemCard(_currentCategoryInfo.SelectedIndex);
			_cardsScroll.ResetPosition();
			_cardsScroll.InvalidateBounds();
			if (_cardsScroll.panel.baseClipRegion.w > _cardsScroll.bounds.size.y)
			{
				_cardsScroll.enabled = false;
				_arrowUp.SetActive(false);
				_arrowDown.SetActive(false);
			}
			else
			{
				_cardsScroll.enabled = true;
				_arrowUp.SetActive(true);
				_arrowDown.SetActive(true);
			}
		}
	}
}
