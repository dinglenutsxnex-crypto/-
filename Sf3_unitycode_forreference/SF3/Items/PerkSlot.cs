using System.Collections;
using DG.Tweening;
using DOTweenUtils;
using Nekki.UI;
using SF3.Settings;
using UnityEngine;
using sf3DTO;

namespace SF3.Items
{
	public class PerkSlot : MonoBehaviour
	{
		public GameObject perkIcon;

		public UISprite background;

		public NekkiUISprite selectBorder;

		public NekkiUISprite insertionEffect;

		public Perk perk;

		public ItemInsertEventHandler onInsertedItem;

		public ItemRemoveEventHandler onRemovedItem;

		public ItemChangeEventHandler onChangedItem;

		public int id;

		public int index;

		public float dragDelay;

		public float dragScale;

		private UITexture _perkIconTexture;

		private bool _selected;

		public bool Selected
		{
			get
			{
				return _selected;
			}
			set
			{
				_selected = value;
				selectBorder.enabled = _selected;
				selectBorder.gameObject.SetActive(_selected);
			}
		}

		public bool Empty
		{
			get
			{
				return perk == null;
			}
		}

		private void Awake()
		{
			_perkIconTexture = perkIcon.GetComponent<UITexture>();
		}

		private void SetSprite(string name, Rarity rarity, EffectType effectType)
		{
			background.color = GameSettings.ItemSettings.GetRarityColor(rarity);
			string text = "ICON_" + name;
			_perkIconTexture.mainTexture = GlobalLoad.GetLoadTexture2D("UI/Perks/" + text);
			perkIcon.SetActive(true);
			background.gameObject.SetActive(true);
			switch (effectType)
			{
			case EffectType.None:
				_perkIconTexture.alpha = 1f;
				background.alpha = 1f;
				break;
			case EffectType.Blink:
				_perkIconTexture.alpha = 0f;
				background.alpha = 0f;
				ShowPerkSelection();
				break;
			}
		}

		private void ResetSprite()
		{
			perkIcon.SetActive(false);
			background.gameObject.SetActive(false);
		}

		public bool SetItem<T>(T item, bool quite = false, EffectType effectType = EffectType.None) where T : BaseItem
		{
			Perk perk = item as Perk;
			if (perk == null)
			{
				return false;
			}
			Perk perk2 = this.perk;
			int num = id;
			this.perk = perk;
			id = perk.id;
			if (!quite && onInsertedItem != null && !onInsertedItem(perk))
			{
				id = num;
				this.perk = perk2;
				return false;
			}
			SetSprite(this.perk.Image, this.perk.GetRarityType(), effectType);
			return true;
		}

		public void RemoveItem()
		{
			if (onRemovedItem != null)
			{
				onRemovedItem(perk);
			}
			perk = null;
			id = -1;
			ResetSprite();
		}

		public bool ChangeItem(PerkSlot perkSlot, BaseItem itemToRemove, BaseItem itemToAdd)
		{
			this.perk = null;
			id = -1;
			ResetSprite();
			Perk perk = itemToAdd as Perk;
			if (perk == null)
			{
				return false;
			}
			this.perk = perk;
			id = perk.id;
			if (onChangedItem != null && !onChangedItem(itemToRemove, itemToAdd))
			{
				id = -1;
				this.perk = null;
				return false;
			}
			SetSprite(this.perk.Image, this.perk.GetRarityType(), EffectType.Blink);
			return true;
		}

		public bool IsPerkType<T>(T item) where T : BaseItem
		{
			Perk perk = item as Perk;
			return this.perk != null && perk != null && this.perk.GetPerkType().Equals(perk.GetPerkType());
		}

		private IEnumerator ReturnItemToSlot()
		{
			for (int i = 0; i < 20; i++)
			{
				yield return new WaitForEndOfFrame();
				perkIcon.transform.position = Vector3.Lerp(perkIcon.transform.position, base.transform.position, (float)i / 20f);
				perkIcon.transform.localScale = Vector3.Lerp(perkIcon.transform.localScale, base.transform.localScale, (float)i / 20f);
			}
			perkIcon.transform.parent = base.transform;
			perkIcon.transform.localScale = base.transform.localScale;
		}

		private void ShowPerkSelection()
		{
			float num = 0f;
			float num2 = 1f;
			float duration = 0.3f;
			Sequence sequence = DOTween.Sequence();
			sequence.Append(DONgui.Fade(insertionEffect, num, num2, duration).SetEase(Ease.InCubic));
			sequence.Append(DONgui.Fade(insertionEffect, num2, num, duration).SetEase(Ease.InCubic));
			sequence.Join(DONgui.Fade(_perkIconTexture, num, 1f, duration));
			sequence.Join(DONgui.Fade(background, num, 1f, duration));
			sequence.Play();
		}
	}
}
