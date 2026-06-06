using System;
using System.Collections.Generic;
using Nekki.UI;
using UnityEngine;

namespace SF3.Items
{
	public class ItemSlotCtr : MonoBehaviour
	{
		public delegate void ItemSelectedEventHandler(PerkSlot slot);

		public GameObject slotPrf;

		public float slotOffset = 10f;

		public ItemInsertEventHandler onInsertedItem;

		public ItemRemoveEventHandler onRemovedItem;

		public ItemChangeEventHandler onChangedItem;

		private NekkiUISprite _slotSprite;

		private int _selected;

		private bool _quiet;

		private List<PerkSlot> _slotsObj = new List<PerkSlot>();

		public ItemSelectedEventHandler onSelected;

		public PerkSlot selected_slot
		{
			get
			{
				if (_selected < 0 || _selected >= _slotsObj.Count)
				{
					return null;
				}
				return _slotsObj[_selected];
			}
		}

		public int selected_id
		{
			get
			{
				if (_slotsObj == null || _selected < 0 || _selected >= _slotsObj.Count)
				{
					return -1;
				}
				return _slotsObj[_selected].id;
			}
		}

		public int selected
		{
			get
			{
				return _selected;
			}
			set
			{
				foreach (PerkSlot item in _slotsObj)
				{
					if (item.index == value)
					{
						item.Selected = true;
					}
					else
					{
						item.Selected = false;
					}
				}
				_selected = value;
				if (!_quiet && onSelected != null)
				{
					onSelected(_slotsObj[_selected]);
				}
				_quiet = false;
			}
		}

		public void DrawSlots(List<ItemSlot> slotItems)
		{
			_slotsObj.Clear();
			foreach (Transform item in base.transform)
			{
				PerkSlot component = item.GetComponent<PerkSlot>();
				component.onInsertedItem = (ItemInsertEventHandler)Delegate.Remove(component.onInsertedItem, onInsertedItem);
				PerkSlot component2 = item.GetComponent<PerkSlot>();
				component2.onRemovedItem = (ItemRemoveEventHandler)Delegate.Remove(component2.onRemovedItem, onRemovedItem);
				PerkSlot component3 = item.GetComponent<PerkSlot>();
				component3.onChangedItem = (ItemChangeEventHandler)Delegate.Remove(component3.onChangedItem, onChangedItem);
				UnityEngine.Object.Destroy(item.gameObject);
			}
			if (slotItems.Count == 0)
			{
				return;
			}
			_slotSprite = slotPrf.GetComponent<NekkiUISprite>();
			float num = 0f;
			num = ((slotItems.Count % 2 != 0) ? (0f - ((slotOffset + (float)_slotSprite.width) * Mathf.Ceil((float)slotItems.Count / 2f) - (float)_slotSprite.width - slotOffset)) : (0f - ((slotOffset + (float)_slotSprite.width) * (float)slotItems.Count / 2f - (float)(_slotSprite.width / 2) - slotOffset / 2f)));
			for (int i = 0; i < slotItems.Count; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(slotPrf);
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = new Vector3(num + (slotOffset + (float)_slotSprite.width) * (float)i, 0f, 0f);
				gameObject.transform.localScale = base.transform.localScale;
				PerkSlot component4 = gameObject.GetComponent<PerkSlot>();
				component4.index = i;
				component4.id = i;
				component4.onInsertedItem = (ItemInsertEventHandler)Delegate.Combine(component4.onInsertedItem, onInsertedItem);
				component4.onRemovedItem = (ItemRemoveEventHandler)Delegate.Combine(component4.onRemovedItem, onRemovedItem);
				component4.onChangedItem = (ItemChangeEventHandler)Delegate.Combine(component4.onChangedItem, onChangedItem);
				TutorialComponent component5 = gameObject.GetComponent<TutorialComponent>();
				if (component5 != null)
				{
					component5.SetId(base.gameObject.transform.parent.name + "_slot_" + (i + 1));
				}
				EventDelegate eventDelegate = new EventDelegate();
				eventDelegate.target = this;
				eventDelegate.methodName = "OnSlotClick";
				EventDelegate eventDelegate2 = eventDelegate;
				EventDelegate.Parameter value = new EventDelegate.Parameter(component4, "slot");
				eventDelegate2.parameters.SetValue(value, 0);
				gameObject.GetComponent<UIButton>().onClick.Add(eventDelegate2);
				_slotsObj.Add(component4);
			}
			for (int j = 0; j < slotItems.Count; j++)
			{
				if (slotItems[j].HasPerk())
				{
					_slotsObj[j].SetItem(slotItems[j].perk, true);
				}
			}
			selected = 0;
		}

		private void OnSlotClick(PerkSlot slot)
		{
			SF3UiLogger.instance.AddButtonClickEvent(ConstantsSF3.ELocationSceneModule.Inventory, "ability_slot_" + (slot.index + 1));
			selected = slot.index;
		}

		public PerkSlot GetSlotWithPerk(Perk perk)
		{
			foreach (PerkSlot item in _slotsObj)
			{
				if (item != null && item.perk != null && item.perk.id.Equals(perk.id))
				{
					return item;
				}
			}
			return null;
		}

		public void SetSelection(Perk perk)
		{
			if (perk == null || _slotsObj.Count <= 1)
			{
				return;
			}
			foreach (PerkSlot item in _slotsObj)
			{
				if (item.IsPerkType(perk))
				{
					SelectSlotQuiet(item.index);
					return;
				}
			}
			if (selected_slot.Empty)
			{
				return;
			}
			foreach (PerkSlot item2 in _slotsObj)
			{
				if (item2.Empty)
				{
					SelectSlotQuiet(item2.index);
					break;
				}
			}
		}

		private void SelectSlotQuiet(int index)
		{
			_quiet = true;
			selected = index;
		}

		public PerkSlot GetSlot()
		{
			if (selected >= _slotsObj.Count)
			{
				return null;
			}
			return _slotsObj[selected];
		}

		public bool HasSelectedSlotPerkOfType<T>(T item) where T : BaseItem
		{
			return _slotsObj[selected].IsPerkType(item);
		}
	}
}
