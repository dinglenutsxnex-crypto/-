using System;
using System.Collections.Generic;
using Jint.Native;
using Nekki.UI;
using SF3;
using SF3.Items;
using SF3.UserData;
using UnityEngine;
using sf3DTO;

public class PropertiesPanel : UIModuleHolder
{
	[SerializeField]
	private ItemPerkCard itemPerkCard;

	private ItemSlotCtr _slotsController;

	private Equipment _selectedItem;

	private ReelDriver _reel;

	[SerializeField]
	private UIButton imbueBtn;

	[SerializeField]
	private Collider imbueBtnCollider;

	[SerializeField]
	private NekkiUILabel imbueBtnLbl;

	public static PropertiesPanel Instance { get; private set; }

	public override void Initialize()
	{
		Instance = this;
		itemPerkCard.Init();
		_slotsController = itemPerkCard.itemSlotDrawer;
		ItemSlotCtr slotsController = _slotsController;
		slotsController.onSelected = (ItemSlotCtr.ItemSelectedEventHandler)Delegate.Combine(slotsController.onSelected, new ItemSlotCtr.ItemSelectedEventHandler(OnSlotSelectionChanged));
		ItemSlotCtr slotsController2 = _slotsController;
		slotsController2.onInsertedItem = (ItemInsertEventHandler)Delegate.Combine(slotsController2.onInsertedItem, new ItemInsertEventHandler(OnItemInserted));
		ItemSlotCtr slotsController3 = _slotsController;
		slotsController3.onRemovedItem = (ItemRemoveEventHandler)Delegate.Combine(slotsController3.onRemovedItem, new ItemRemoveEventHandler(OnItemRemoved));
		ItemSlotCtr slotsController4 = _slotsController;
		slotsController4.onChangedItem = (ItemChangeEventHandler)Delegate.Combine(slotsController4.onChangedItem, new ItemChangeEventHandler(OnItemChanged));
		imbueBtnCollider = imbueBtn.GetComponent<Collider>();
		imbueBtn.onClick.Add(new EventDelegate(OnImbueButtonClick));
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		ItemSlotCtr slotsController = _slotsController;
		slotsController.onSelected = (ItemSlotCtr.ItemSelectedEventHandler)Delegate.Remove(slotsController.onSelected, new ItemSlotCtr.ItemSelectedEventHandler(OnSlotSelectionChanged));
		ItemSlotCtr slotsController2 = _slotsController;
		slotsController2.onInsertedItem = (ItemInsertEventHandler)Delegate.Remove(slotsController2.onInsertedItem, new ItemInsertEventHandler(OnItemInserted));
		ItemSlotCtr slotsController3 = _slotsController;
		slotsController3.onRemovedItem = (ItemRemoveEventHandler)Delegate.Remove(slotsController3.onRemovedItem, new ItemRemoveEventHandler(OnItemRemoved));
		ItemSlotCtr slotsController4 = _slotsController;
		slotsController4.onChangedItem = (ItemChangeEventHandler)Delegate.Remove(slotsController4.onChangedItem, new ItemChangeEventHandler(OnItemChanged));
	}

	public void UpdatePerksWheel()
	{
		List<ISlotItem> availablePerksForItem = UserManager.UserModelInfo.GetAvailablePerksForItem(_selectedItem);
		List<BaseItem> list = new List<BaseItem>();
		foreach (ISlotItem item in availablePerksForItem)
		{
			list.Add((BaseItem)item);
		}
		List<ItemSlot> slotItems = _selectedItem.GetSlotItems();
		List<BaseItem> list2 = new List<BaseItem>();
		foreach (ItemSlot item2 in slotItems)
		{
			if (item2.HasPerk())
			{
				list2.Add(item2.perk);
			}
		}
		list.Add(_selectedItem);
		InventoryManager.Instance.UpdateSubtypesIcons();
		list.Remove(_selectedItem);
		_reel.ChangeType(true, EquipmentType.None, list, list2, false);
		_reel.FocusOnCenter();
	}

	public void ShowProperties(ReelDriver reel, int selectedItemID)
	{
		_reel = reel;
		_selectedItem = (Equipment)UserManager.UserModelInfo.GetItemByID(selectedItemID);
		_reel.ItemSelected += OnReelItemChange;
		ChangeSelectedItem(_selectedItem);
	}

	public void ChangeSelectedItem(Equipment selectedItem)
	{
		_selectedItem = selectedItem;
		if (_selectedItem.HasSlots())
		{
			UpdatePerksWheel();
			itemPerkCard.SetItem(_selectedItem);
			if (!_slotsController.selected_slot.Empty)
			{
				_reel.FocusOnItem(_slotsController.selected_id, true);
			}
			if (_reel.Count > 0)
			{
				imbueBtn.enabled = true;
				return;
			}
		}
		imbueBtn.enabled = false;
	}

	private void OnDisable()
	{
		_reel.ItemSelected -= OnReelItemChange;
	}

	private void OnReelItemChange(BaseItem item)
	{
		itemPerkCard.SetPerkSelection(item);
		UpdateButtonsText(item);
	}

	private void SaveChangesAndReloadAnimation()
	{
		UserManager.UpdateEquipmentData(_selectedItem);
		UserManager.UserModelInfo.CollectAndCalculate();
		ModelsManager.Instance.Player.InitializeModelAnimation(true);
		ModelsManager.Instance.Enemy.InitializeModelAnimation(true);
	}

	public void OnImbueButtonClick()
	{
		SF3.Items.Perk perk = (SF3.Items.Perk)_reel.Selected;
		if (_selectedItem.HasPerk(perk))
		{
			SF3UiLogger.instance.AddRemovePerkEvent(_selectedItem, _reel.Selected);
			RemovePerk(_reel.Selected);
			UpdateButtonsText();
			return;
		}
		_slotsController.SetSelection(perk);
		if (_slotsController.selected_slot.Empty)
		{
			Imbue(_slotsController.selected_slot, _reel.Selected);
			SF3UiLogger.instance.AddImbueEvent(_selectedItem, null, _reel.Selected);
			return;
		}
		ScreenTexture.Instance.SetTexture(base.name, ScreenTexture.TextureOutputCamera.Both, ScreenTexture.TextureOutputFilter.Blur);
		string msgAlias = "replace_perk";
		if (_slotsController.selected_slot.IsPerkType(perk))
		{
			msgAlias = "replace_same_type_perk";
		}
		SF3.Items.Perk perk2 = _slotsController.selected_slot.perk;
		SF3.Items.Perk perk3 = _reel.Selected as SF3.Items.Perk;
		SystemMessage systemMessage = SystemMessage.ShowAlert(msgAlias, new string[2]
		{
			Localization.Get(perk2.alias).String,
			Localization.Get(perk3.alias).String
		}, perk3.alias);
		SF3UiLogger.instance.AddImbueEvent(_selectedItem, perk2, perk3);
		systemMessage.AddButton("ok", delegate
		{
			Imbue(_slotsController.selected_slot, _reel.Selected);
			ScreenTexture.Instance.Clear(base.name);
		});
		systemMessage.AddButton("cancel", delegate
		{
			ScreenTexture.Instance.Clear(base.name);
		});
		systemMessage.Show();
	}

	public void Imbue(SF3.Items.PerkSlot perkSlot, BaseItem itemPerk)
	{
		int num = _selectedItem.PerkOfTypeCount((SF3.Items.Perk)itemPerk);
		Dictionary<string, JsValue> perkTypeRestrictions = JsFunction.GetPerkTypeRestrictions(_selectedItem.id);
		int perkType = (int)((SF3.Items.Perk)itemPerk).GetPerkType();
		if (!perkTypeRestrictions.ContainsKey(perkType.ToString()) || num < perkTypeRestrictions[perkType.ToString()].AsInteger() || perkSlot.IsPerkType(itemPerk))
		{
			UpdateButtonsText();
			if (!perkSlot.Empty)
			{
				ChangePerk(perkSlot, perkSlot.perk, itemPerk);
			}
			else
			{
				AddPerk(perkSlot, itemPerk);
			}
		}
	}

	private void ChangePerk(SF3.Items.PerkSlot perkSlot, BaseItem itemToRemove, BaseItem itemToAdd)
	{
		perkSlot.ChangeItem(perkSlot, itemToRemove, itemToAdd);
	}

	private void AddPerk(SF3.Items.PerkSlot perkSlot, BaseItem item)
	{
		perkSlot.SetItem(item, false, EffectType.Blink);
	}

	private void RemovePerk(BaseItem item)
	{
		SF3.Items.Perk perk = item as SF3.Items.Perk;
		if (perk != null)
		{
			RemovePerk(_slotsController.GetSlotWithPerk(perk));
		}
	}

	private void RemovePerk(SF3.Items.PerkSlot perkSlot)
	{
		if (!(perkSlot == null))
		{
			perkSlot.RemoveItem();
		}
	}

	public void OnSlotSelectionChanged(SF3.Items.PerkSlot slot)
	{
		if (_selectedItem.HasPerk(slot.id))
		{
			_reel.FocusOnItem(slot.id, false);
		}
		UpdateButtonsText();
	}

	public bool OnItemInserted(BaseItem item)
	{
		if (_selectedItem.AddSlotItem((ISlotItem)item, _slotsController.selected_slot.index))
		{
			SaveChangesAndReloadAnimation();
			InventoryManager.Instance.UpdateSlot(_selectedItem.GetEquipmentType());
			InsertPerkRequest insertPerkRequest = new InsertPerkRequest();
			insertPerkRequest.Insert = true;
			insertPerkRequest.ItemModelId = _selectedItem.id;
			insertPerkRequest.PerkModelId = item.id;
			insertPerkRequest.SlotIndex = _slotsController.selected_slot.index;
			UserDataController.Send_InsertPerk(insertPerkRequest);
			UpdateButtonsText();
			return true;
		}
		return false;
	}

	public bool OnItemChanged(BaseItem form, BaseItem to)
	{
		int slotId;
		bool flag = _selectedItem.RemoveSlotItem((ISlotItem)form, out slotId);
		bool flag2 = _selectedItem.AddSlotItem((ISlotItem)to, _slotsController.selected_slot.index);
		if (flag2 || flag)
		{
			InventoryManager.Instance.UpdateSlot(_selectedItem.GetEquipmentType());
			SaveChangesAndReloadAnimation();
			UpdateButtonsText();
			InsertPerkRequest insertPerkRequest = new InsertPerkRequest();
			insertPerkRequest.Insert = true;
			insertPerkRequest.ItemModelId = _selectedItem.id;
			insertPerkRequest.PerkModelId = to.id;
			insertPerkRequest.SlotIndex = _slotsController.selected_slot.index;
			UserDataController.Send_InsertPerk(insertPerkRequest);
			if (flag2)
			{
				return true;
			}
		}
		return false;
	}

	public void OnItemRemoved(BaseItem item)
	{
		int slotId;
		if (_selectedItem.RemoveSlotItem((ISlotItem)item, out slotId))
		{
			SaveChangesAndReloadAnimation();
			InventoryManager.Instance.UpdateSlot(_selectedItem.GetEquipmentType());
			InsertPerkRequest insertPerkRequest = new InsertPerkRequest();
			insertPerkRequest.Insert = false;
			insertPerkRequest.ItemModelId = _selectedItem.id;
			insertPerkRequest.PerkModelId = item.id;
			insertPerkRequest.SlotIndex = slotId;
			UserDataController.Send_InsertPerk(insertPerkRequest);
			UpdateButtonsText();
		}
	}

	private void UpdateButtonsText(BaseItem item = null)
	{
		if (_reel.Selected != null)
		{
			bool flag = _selectedItem.HasPerk(_reel.Selected.id);
			SF3.Items.PerkSlot selected_slot = _slotsController.selected_slot;
			if (flag)
			{
				UpdateButton("Properties_remove", true);
			}
			else
			{
				UpdateButton("Properties_imbue", true);
			}
		}
		else
		{
			imbueBtn.gameObject.SetActive(false);
		}
	}

	private void UpdateButton(string alias, bool enabled)
	{
		imbueBtn.gameObject.SetActive(true);
		imbueBtnLbl.Alias = alias;
		imbueBtnCollider.enabled = enabled;
		imbueBtn.SetState((!enabled) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal, true);
	}
}
