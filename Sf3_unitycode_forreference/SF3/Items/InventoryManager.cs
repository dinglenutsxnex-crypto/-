using System;
using System.Collections.Generic;
using Nekki.UI;
using SF3.Audio;
using SF3.GameModels;
using SF3.Settings;
using SF3.UserData;
using UnityEngine;

namespace SF3.Items
{
	public class InventoryManager : UIModuleHolder
	{
		[SerializeField]
		private string _onOpenSoundName = string.Empty;

		[SerializeField]
		private string _onCloseSoundName = string.Empty;

		[SerializeField]
		private UISprite _abilitiesBadge;

		[SerializeField]
		private UILabel _abilitiesBadgeLabel;

		public InventorySlot[] inventorySlots;

		private UIButton _equipItemButton;

		private UILabel _equipItemLable;

		private GameObject _itemPropertiesBtn;

		public GameObject compareItemsBtn;

		public GameObject equippedLbl;

		public GameObject equippedFrame;

		public Vector3 equippedLblOffset;

		public Vector3 offsetFromChar;

		public GameObject inventory;

		public GameObject properties;

		public GameObject descriptionPanel;

		public ItemDetailsPanel detailsPanel;

		public SetInfoPanel setInfoPanel;

		public ReelDriver inventoryReelDriver;

		public Transform progressPosition;

		public GameObject progressPrf;

		private UIProgressBar _progress;

		public NekkiUILabel nameLbl;

		public AttributesDrawer attributesDrawer;

		public Transform perkInfoBarPosition;

		public GameObject perkInfoBarPrf;

		private PerkInfoDrawer _perkInfroDrawer;

		public NekkiUILabel perkNameLbl;

		public NekkiUILabel perkDescriptionLbl;

		public NekkiUILabel NoPerksLabel;

		private BaseItem _lastSelectedElementReel;

		private EquipmentType _categoryTypeCurrent;

		public Transform subTypeSelectionFrame;

		public SubtypeButton[] subTypeButtons;

		public GameObject inventoryReelBack;

		private UIButton _propertiesButton;

		private Action onSceneChanged;

		private GameObject _lastClickedSlot;

		private long _abilitiesBadgeCount;

		private InventoryIntentModule _intent;

		private Action _callbackOnOpen;

		private Action _callbackOnClosed;

		public static InventoryManager Instance { get; private set; }

		public PropertiesPanel PropPanel { get; private set; }

		public static BaseItem CurrentItemData { get; private set; }

		private ISkinnedModel _playerModel
		{
			get
			{
				return ModelsManager.Instance.Player;
			}
		}

		public override void Initialize()
		{
			Instance = this;
			inventoryReelBack.SetActive(false);
			offsetFromChar.x *= (float)Screen.width / (float)Screen.height / 1.33f;
			subTypeSelectionFrame.gameObject.SetActive(false);
			_categoryTypeCurrent = EquipmentType.Weapon;
			NekkiUIElements component = GetComponent<NekkiUIElements>();
			_lastClickedSlot = null;
			component.Get<UIButton>("propertiesBackBtn").onClick.Add(new EventDelegate(delegate
			{
				SF3UiLogger.instance.AddButtonClickEvent(ConstantsSF3.ELocationSceneModule.Inventory, "inventory");
				ShowInventory();
				_intent.SetSubModule(InventoryIntentModule.InventorySubModuleType.None);
			}));
			_propertiesButton = component.Get<UIButton>("propertiesBtn");
			_propertiesButton.onClick.Add(new EventDelegate(delegate
			{
				SF3UiLogger.instance.AddAbilitesClickEvent(inventoryReelDriver.Selected, _abilitiesBadgeCount);
				ShowPerkScreen();
				_intent.SetSubModule(InventoryIntentModule.InventorySubModuleType.PerkScreen);
			}));
			_itemPropertiesBtn = _propertiesButton.gameObject;
			_itemPropertiesBtn.SetActive(true);
			_equipItemButton = component.Get<UIButton>("equipItemButton");
			_equipItemButton.Collider.enabled = false;
			_equipItemButton.gameObject.SetActive(true);
			_equipItemButton.onClick.Add(new EventDelegate(Equip));
			_equipItemLable = _equipItemButton.GetComponentInChildren<UILabel>();
			inventory.SetActive(false);
			properties.SetActive(false);
			detailsPanel.Init();
			PropPanel = properties.GetComponent<PropertiesPanel>();
			PropPanel.Initialize();
			inventoryReelDriver.Cleared += delegate
			{
				ShowPerkDescription(null);
			};
			inventoryReelDriver.ItemSelected += delegate(BaseItem b)
			{
				UpdateAbilitiesBadge(b);
				PreviewItemStats(b);
				ChangeButtonsState();
			};
			inventoryReelDriver.ItemStartChanging += delegate
			{
				_propertiesButton.isEnabled = false;
				_equipItemButton.isEnabled = false;
			};
			inventoryReelDriver.ItemSwiped += PreviewItemStats;
			SubtypeButton.Reset();
			SubtypeButton[] array = subTypeButtons;
			foreach (SubtypeButton inventoryButton in array)
			{
				inventoryButton.Init();
				EquipmentType buttonType = inventoryButton.TypeEquipment;
				UIButton component2 = inventoryButton.subTypeButton.GetComponent<UIButton>();
				component2.onClick.Add(new EventDelegate(delegate
				{
					if (_categoryTypeCurrent != buttonType)
					{
						LodChageSlot(_categoryTypeCurrent.ToString(), buttonType.ToString(), inventoryButton.NewItems);
						ChangeSubType(buttonType);
					}
				}));
				inventoryButton.UpateBadge();
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(perkInfoBarPrf);
			gameObject.transform.parent = perkInfoBarPosition.transform;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			_perkInfroDrawer = gameObject.GetComponent<PerkInfoDrawer>();
			AudioManager.Instance.LoadSound(_onOpenSoundName, _onCloseSoundName);
			base.gameObject.SetActive(false);
		}

		public static void EnableReelCameraIfExists()
		{
			if (!(Instance == null) && !(Instance.inventoryReelDriver == null))
			{
				Instance.inventoryReelDriver.EnableReelCamera();
			}
		}

		public static void DisableReelCameraIfExists()
		{
			if (!(Instance == null) && !(Instance.inventoryReelDriver == null))
			{
				Instance.inventoryReelDriver.DisableReelCamera();
			}
		}

		private void UpdateAbilitiesBadge(BaseItem item)
		{
			_abilitiesBadgeCount = 0L;
			if (item != null && item is Equipment)
			{
				_abilitiesBadgeCount = UserBadgesManager.Instance.GetNewPerksFor(item);
				_abilitiesBadge.gameObject.SetActive(_abilitiesBadgeCount > 0);
				_abilitiesBadgeLabel.text = _abilitiesBadgeCount.ToString();
			}
		}

		public void UpdateSubtypesIcons()
		{
			InventorySlot[] array = inventorySlots;
			foreach (InventorySlot inventorySlot in array)
			{
				inventorySlot.RefreshImage();
			}
		}

		private void PreviewItemStats(BaseItem element)
		{
			if (CurrentItemData == element)
			{
				return;
			}
			if (properties.activeSelf)
			{
				ShowPerkDescription(element);
				CurrentItemData = element;
			}
			else
			{
				_lastSelectedElementReel = element;
				CurrentItemData = element;
				if (_playerModel != null)
				{
					ShowItemDescription(element);
				}
			}
			Equipment equipment = CurrentItemData as Equipment;
			if (equipment != null)
			{
				UpdateIntent(equipment.GetEquipmentType(), equipment.id);
			}
		}

		private void ChangeButtonsState()
		{
			Equipment equipment = CurrentItemData as Equipment;
			if (equipment != null)
			{
				bool flag = equipment.level <= UserManager.GetLevel();
				_equipItemButton.state = ((!flag && !equipment.IsEquipped()) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
				_equipItemButton.isEnabled = flag || equipment.IsEquipped();
				_equipItemLable.text = ((!equipment.IsEquipped()) ? "EQUIP" : "UNEQUIP");
			}
			_propertiesButton.isEnabled = false;
			ISlotable slotable = (ISlotable)UserManager.UserModelInfo.GetItemByID(CurrentItemData.id);
			if (slotable != null && slotable.HasSlots())
			{
				_propertiesButton.isEnabled = true;
			}
		}

		private void ShowItemDescription(BaseItem selected)
		{
			BaseItem itemByID = UserManager.UserModelInfo.GetItemByID(UserManager.UserModelInfo.GetEquippedIDForType(_categoryTypeCurrent));
			nameLbl.Alias = selected.alias;
			IAttributable attributable = selected as IAttributable;
			IAttributable attributable2 = itemByID as IAttributable;
			if (attributable != null && attributable2 != null)
			{
				attributesDrawer.Draw(attributable.GetAttributesForDisplayData(), attributable2.GetAttributesForDisplayData());
			}
			_perkInfroDrawer.Draw(selected);
		}

		private void ShowPerkDescription(BaseItem selectedBase)
		{
			bool flag = selectedBase != null;
			perkNameLbl.gameObject.SetActive(flag);
			perkDescriptionLbl.gameObject.SetActive(flag);
			NoPerksLabel.gameObject.SetActive(!flag);
			if (flag)
			{
				Perk perk = (Perk)selectedBase;
				perkNameLbl.Alias = perk.alias;
				perkDescriptionLbl.Alias = perk.GetDescription();
			}
			else
			{
				NoPerksLabel.Alias = "NO_PERKS";
			}
		}

		private void ShowSlotDescription(BaseItem item)
		{
			perkNameLbl.Alias = ((item != null) ? item.alias : "Empty Slot");
		}

		public void ToggleDetails()
		{
			SF3UiLogger.instance.AddButtonClickEvent(ConstantsSF3.ELocationSceneModule.Inventory, "compare");
			if (detailsPanel.active)
			{
				detailsPanel.DisablePanel();
			}
			else
			{
				detailsPanel.ActivatePanel(UserManager.UserModelInfo.GetEquippedIDForType(_categoryTypeCurrent));
			}
		}

		public void ToggleSetInfo()
		{
			if (setInfoPanel.active)
			{
				setInfoPanel.DisablePanel();
			}
			else
			{
				setInfoPanel.ActivatePanel();
			}
		}

		public int GetIdOfLastSelectedElementInReel()
		{
			return (_lastSelectedElementReel != null) ? _lastSelectedElementReel.id : (-1);
		}

		public void EnableInventory()
		{
			if (properties.activeSelf)
			{
				properties.SetActive(false);
			}
			inventory.SetActive(true);
			CheckInventoryCategoryIsActive();
		}

		public override void ShowModule(IntentModule intent, Action callbackOnOpen)
		{
			_intent = (InventoryIntentModule)intent;
			_callbackOnOpen = callbackOnOpen;
			detailsPanel.DisablePanel();
			InventorySlot[] array = inventorySlots;
			foreach (InventorySlot currentSlot in array)
			{
				UpdateSlot(currentSlot);
			}
			if (_intent.IsInstant())
			{
				BattleCamera.Instance.changeClipPlane = true;
				BattleCamera.Instance.toClipPlane = 10f;
			}
			else
			{
				BattleCamera.Instance.MainCamera.nearClipPlane = 10f;
			}
			if (ModelsManager.Instance.Player != null)
			{
				BattleCamera.MoveToObject(_playerModel.GetCenterOfMassBone().transform, offsetFromChar, delegate
				{
					BattleCamera.ActivateInventoryCamera(offsetFromChar);
					base.gameObject.SetActive(true);
					inventoryReelBack.SetActive(true);
					UpdateModule(intent);
				}, _intent.IsInstant(), true);
			}
			HolderModule.EnableControls(false);
			ModelsManager.Instance.HideEnemy(1f);
			LocationColorAnimation.Instance.Animate(GameSettings.DojoSettings.LocationColorChangeTime, GameSettings.DojoSettings.DefaultLocationColor, GameSettings.DojoSettings.LocationColorInModule);
			LocationColorAnimation.Instance.EnableRays(false, GameSettings.DojoSettings.LocationColorChangeTime);
			AudioManager.Instance.PlaySound(_onOpenSoundName);
		}

		public override void HideModule(Action callbackOnClosed)
		{
			_callbackOnClosed = callbackOnClosed;
			if (PropPanel.isActiveAndEnabled)
			{
				EnableInventory();
			}
			LocationColorAnimation.Instance.Animate(GameSettings.DojoSettings.LocationColorChangeTime, GameSettings.DojoSettings.LocationColorInModule, GameSettings.DojoSettings.DefaultLocationColor);
			LocationColorAnimation.Instance.EnableRays(true, GameSettings.DojoSettings.LocationColorChangeTime);
			_lastClickedSlot = null;
			inventoryReelDriver.gameObject.SetActive(false);
			inventoryReelBack.SetActive(false);
			AudioManager.Instance.PlaySound(_onCloseSoundName);
			inventory.SetActive(false);
			base.gameObject.SetActive(false);
			CurrentItemData = null;
			_callbackOnClosed.InvokeSafe();
		}

		public override void UpdateModule(IntentModule intent)
		{
			_intent = (InventoryIntentModule)intent;
			ShowInventory();
			ShowInventorySubModule();
			_callbackOnOpen.InvokeSafe();
		}

		private void ShowInventory()
		{
			EnableInventory();
			if (_intent.Category != 0)
			{
				ChangeSubType(_intent.Category, _intent.ItemId);
			}
			else
			{
				ChangeSubType(_categoryTypeCurrent, GetIdOfLastSelectedElementInReel());
			}
			ModelsManager.Instance.ShowPlayer(1f);
		}

		private void ShowInventorySubModule()
		{
			InventoryIntentModule.InventorySubModuleType subModule = _intent.SubModule;
			if (subModule == InventoryIntentModule.InventorySubModuleType.PerkScreen)
			{
				ShowPerkScreen();
			}
		}

		private void ShowPerkScreen()
		{
			UserBadgesManager.Instance.Reset(UserBadgesManager.BadgeTypes.Perks, inventoryReelDriver.Selected);
			_abilitiesBadge.gameObject.SetActive(false);
			inventory.SetActive(false);
			properties.SetActive(true);
			PropPanel.ShowProperties(inventoryReelDriver, inventoryReelDriver.Selected.id);
			ModelsManager.Instance.HidePlayer(1f);
		}

		private void CheckInventoryCategoryIsActive()
		{
			compareItemsBtn.SetActive(true);
			for (int i = 0; i < subTypeButtons.Length; i++)
			{
				List<Equipment> equipmentOfType = UserManager.UserModelInfo.GetEquipmentOfType(subTypeButtons[i].TypeEquipment);
				InventorySlot inventorySlotByType = GetInventorySlotByType(subTypeButtons[i].TypeEquipment);
				bool flag = equipmentOfType == null || equipmentOfType.Count == 0;
				subTypeButtons[i].Collider.enabled = !flag;
				inventorySlotByType.LockSlot(flag);
				bool equippedIsHidden = UserManager.UserModelInfo.GetEquippedIsHidden(inventorySlotByType.TypeEquipment);
				if (equippedIsHidden && _categoryTypeCurrent == subTypeButtons[i].TypeEquipment)
				{
					compareItemsBtn.SetActive(false);
				}
				inventorySlotByType.SlotEmpty(equippedIsHidden);
			}
		}

		private void SelectSubtypeButton(EquipmentType type)
		{
			SubtypeButton subtypeButton = null;
			SubtypeButton[] array = subTypeButtons;
			foreach (SubtypeButton subtypeButton2 in array)
			{
				if (subtypeButton2.TypeEquipment == type)
				{
					subtypeButton2.Select();
					subtypeButton = subtypeButton2;
					GetInventorySlotByType(subtypeButton2.TypeEquipment).DarkenSlot(false);
				}
				else
				{
					GetInventorySlotByType(subtypeButton2.TypeEquipment).DarkenSlot();
				}
				subtypeButton2.UpateBadge();
			}
			if (_lastClickedSlot != null)
			{
				_lastClickedSlot.GetComponent<Collider>().enabled = true;
			}
			if (subtypeButton != null)
			{
				_lastClickedSlot = subtypeButton.subTypeButton.gameObject;
				_lastClickedSlot.GetComponent<Collider>().enabled = false;
			}
			_categoryTypeCurrent = type;
		}

		private void UpdateIntent(EquipmentType type, int itemId)
		{
			if (_intent.Category != type)
			{
				_intent.SetCategory(type);
				_intent.SetItemId(itemId);
			}
			else if (_intent.ItemId != itemId)
			{
				_intent.SetItemId(itemId);
			}
		}

		public void ChangeSubType(EquipmentType newSubType, int itemID = -1)
		{
			UserBadgesManager.Instance.Reset(UserBadgesManager.BadgeTypes.Inventory, newSubType);
			SelectSubtypeButton(newSubType);
			Equipment equipment = (Equipment)(_lastSelectedElementReel = ((Equipment)UserManager.UserModelInfo.GetItemByID(itemID)) ?? UserManager.UserModelInfo.GetEquipped(_categoryTypeCurrent, true));
			if (PropPanel.isActiveAndEnabled)
			{
				if (equipment != null)
				{
					PropPanel.ChangeSelectedItem(equipment);
					UpdateIntent(_categoryTypeCurrent, equipment.id);
					return;
				}
				EnableInventory();
				UpdateIntent(_categoryTypeCurrent, itemID);
			}
			inventoryReelDriver.ChangeType(false, _categoryTypeCurrent);
			if (inventoryReelDriver.Count > 0)
			{
				descriptionPanel.SetActive(true);
				if (equipment != null)
				{
					inventoryReelDriver.FocusOnItem(equipment.id, true);
					if (detailsPanel.isActiveAndEnabled)
					{
						detailsPanel.ActivatePanel(equipment);
					}
				}
				else
				{
					inventoryReelDriver.FocusOnMostSuitableThing();
				}
			}
			else
			{
				descriptionPanel.SetActive(false);
				if (detailsPanel.active)
				{
					detailsPanel.DisablePanel();
				}
			}
			bool equippedIsHidden = UserManager.UserModelInfo.GetEquippedIsHidden(newSubType);
			compareItemsBtn.SetActive(!equippedIsHidden);
		}

		private void Equip()
		{
			if (inventoryReelDriver.Moving)
			{
				return;
			}
			if (_playerModel != null && inventoryReelDriver.Selected != null)
			{
				if (_playerModel.GetEquippedIDForType(_categoryTypeCurrent) == inventoryReelDriver.Selected.id)
				{
					SF3UiLogger.instance.AddUnequipEvent(inventoryReelDriver.Selected);
					_playerModel.UnEquipItem(_categoryTypeCurrent);
					_equipItemLable.text = "EQUIP";
					if (detailsPanel.active)
					{
						detailsPanel.DisablePanel();
					}
				}
				else
				{
					SF3UiLogger.instance.AddEquipEvent(inventoryReelDriver.Selected);
					Equipment equipment = (Equipment)inventoryReelDriver.Selected;
					_playerModel.EquipItem(equipment.id);
					_equipItemLable.text = "UNEQUIP";
					if (detailsPanel.active)
					{
						detailsPanel.ActivatePanel(inventoryReelDriver.Selected);
					}
				}
				GlobalLoad.UnloadUnusedAssets();
				UpdateSlot(_categoryTypeCurrent);
			}
			PreviewItemStats(CurrentItemData);
		}

		public void UpdateSlot(InventorySlot currentSlot)
		{
			if (currentSlot == null)
			{
				return;
			}
			bool equippedIsHidden = UserManager.UserModelInfo.GetEquippedIsHidden(currentSlot.TypeEquipment);
			currentSlot.SlotEmpty(equippedIsHidden);
			if (!equippedIsHidden)
			{
				BaseItem itemByID = UserManager.UserModelInfo.GetItemByID(UserManager.UserModelInfo.GetEquippedIDForType(currentSlot.TypeEquipment));
				currentSlot.SetImage(UserManager.UserModelInfo.GetEquippedImage(currentSlot.TypeEquipment));
				currentSlot.SetRarity(itemByID as IRarable);
				currentSlot.DarkenSlot(false);
			}
			else
			{
				currentSlot.UnsetRarity();
			}
			if (currentSlot.TypeEquipment == _categoryTypeCurrent)
			{
				compareItemsBtn.SetActive(!equippedIsHidden);
			}
			currentSlot.CategoryAlert.gameObject.SetActive(false);
			List<Equipment> equipmentOfType = UserManager.UserModelInfo.GetEquipmentOfType(currentSlot.TypeEquipment);
			if (equipmentOfType == null || equipmentOfType.Count <= 0)
			{
				return;
			}
			foreach (Equipment item in equipmentOfType)
			{
				IInformable informable = item;
				if (informable != null && informable.IsNew())
				{
					currentSlot.CategoryAlert.gameObject.SetActive(true);
					break;
				}
			}
		}

		public void UpdateSlot(EquipmentType type)
		{
			InventorySlot inventorySlotByType = GetInventorySlotByType(type);
			UpdateSlot(inventorySlotByType);
		}

		private InventorySlot GetInventorySlotByType(EquipmentType subType)
		{
			for (int i = 0; i < inventorySlots.Length; i++)
			{
				if (inventorySlots[i].TypeEquipment == subType)
				{
					return inventorySlots[i];
				}
			}
			return null;
		}

		private void LodChageSlot(string srcSlot, string dstSlot, long count)
		{
			string mode = "items";
			if (detailsPanel.isActiveAndEnabled)
			{
				mode = "compare";
			}
			if (PropertiesPanel.Instance.isActiveAndEnabled)
			{
				mode = "abilities";
			}
			SF3UiLogger.instance.AddInventoryChangeSlotEvent(srcSlot, dstSlot, mode, count);
		}
	}
}
