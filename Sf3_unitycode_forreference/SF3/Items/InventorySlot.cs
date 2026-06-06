using System;
using SF3.Settings;
using UnityEngine;
using sf3DTO;

namespace SF3.Items
{
	[Serializable]
	public class InventorySlot
	{
		[SerializeField]
		private UISprite _slotEmptyIcon;

		[SerializeField]
		private UISprite _itemInSlotBack;

		[SerializeField]
		private EquipmentType _TypeEquipment;

		[SerializeField]
		private UISprite _CategoryAlert;

		[SerializeField]
		private UISprite _CategoryLock;

		[SerializeField]
		private GameObject _perkInfo;

		[SerializeField]
		private GameObject _perkInfoMarker;

		[SerializeField]
		private UITexture _itemTexture;

		[SerializeField]
		private UISprite _rarityBar;

		private UnityEngine.Color _fullColorBg = new Color32(242, 233, 218, byte.MaxValue);

		private UnityEngine.Color _fullDarkenColor = new Color32(180, 180, 180, byte.MaxValue);

		private UnityEngine.Color _fullDarkenColorBg = new Color32(195, 191, 183, byte.MaxValue);

		private UnityEngine.Color _emptyColorBg = new Color32(16, 16, 16, byte.MaxValue);

		private UnityEngine.Color _emptyLockColorIcon = new Color32(27, 27, 27, byte.MaxValue);

		private UnityEngine.Color _emptyDarkenColorIcon = new Color32(90, 89, 87, byte.MaxValue);

		private UnityEngine.Color _emptyColorIcon = new Color32(173, 172, 168, byte.MaxValue);

		private bool _isLock;

		private UISprite _perkMarker;

		private UISprite _slotEmpty;

		private bool _empty = true;

		private string _alias;

		public EquipmentType TypeEquipment
		{
			get
			{
				return _TypeEquipment;
			}
		}

		public UISprite CategoryAlert
		{
			get
			{
				return _CategoryAlert;
			}
		}

		public UITexture Texture
		{
			get
			{
				return _itemTexture;
			}
		}

		public void SlotEmpty(bool empty)
		{
			_empty = empty;
			_itemTexture.enabled = !empty;
			_slotEmptyIcon.gameObject.SetActive(empty);
			if (!empty)
			{
				_itemInSlotBack.color = _fullColorBg;
				return;
			}
			_itemInSlotBack.color = _emptyColorBg;
			_slotEmptyIcon.color = ((!_isLock) ? _emptyColorIcon : _emptyLockColorIcon);
		}

		public void SetRarity(IRarable rarable)
		{
			if (rarable != null)
			{
				if (_empty)
				{
					_rarityBar.spriteName = GameSettings.ItemSettings.GetRaritySpriteName(Rarity.Common);
					return;
				}
				Rarity rarityType = rarable.GetRarityType();
				_rarityBar.spriteName = GameSettings.ItemSettings.GetRaritySpriteName(rarityType);
			}
		}

		public void UnsetRarity()
		{
			_rarityBar.spriteName = GameSettings.ItemSettings.GetRaritySpriteName(Rarity.Common);
		}

		public void DarkenSlot(bool darken = true)
		{
			if (!_isLock)
			{
				if (darken)
				{
					Draken(_fullDarkenColor, (!_empty) ? _fullDarkenColorBg : _emptyColorBg, _emptyDarkenColorIcon);
				}
				else
				{
					Draken(UnityEngine.Color.white, (!_empty) ? _fullColorBg : _emptyColorBg, _emptyColorIcon);
				}
			}
		}

		private void Draken(UnityEngine.Color itemTextureColor, UnityEngine.Color itemInSlotBackColor, UnityEngine.Color slotEmptyIconColor)
		{
			_itemTexture.color = itemTextureColor;
			_itemInSlotBack.color = itemInSlotBackColor;
			_slotEmptyIcon.color = slotEmptyIconColor;
		}

		public void LockSlot(bool isLock)
		{
			_isLock = isLock;
			_CategoryLock.gameObject.SetActive(isLock);
			if (isLock)
			{
				_itemInSlotBack.color = _emptyColorBg;
				_slotEmptyIcon.color = _emptyLockColorIcon;
			}
			else
			{
				DarkenSlot();
			}
		}

		private UISprite GetEmptySprite()
		{
			if (_slotEmpty == null)
			{
				_slotEmpty = _slotEmptyIcon.GetComponent<UISprite>();
			}
			return _slotEmpty;
		}

		public void SetImage(string alias)
		{
			_alias = alias;
			_itemTexture.mainTexture = GlobalLoad.GetLoadTexture2D("UI/Inventory/" + alias);
		}

		public void RefreshImage()
		{
			if (!string.IsNullOrEmpty(_alias) && !_empty)
			{
				_itemTexture.gameObject.SetActive(false);
				_itemTexture.mainTexture = GlobalLoad.GetLoadTexture2D("UI/Inventory/" + _alias);
				_itemTexture.gameObject.SetActive(true);
			}
		}
	}
}
