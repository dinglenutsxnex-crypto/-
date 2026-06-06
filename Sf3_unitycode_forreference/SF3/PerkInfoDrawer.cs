using System.Collections.Generic;
using Nekki.UI;
using SF3.Items;
using UnityEngine;

namespace SF3
{
	public class PerkInfoDrawer : MonoBehaviour
	{
		public UITable perkTable;

		public NekkiUILabel bonusLabel;

		public UISprite bonusSprite;

		public GameObject perkInfoPrf;

		public GameObject perkInfoEmptyPrf;

		private UITable mainTable;

		private void Awake()
		{
			mainTable = GetComponent<UITable>();
		}

		public void Draw(BaseItem item)
		{
			Clear();
			ISlotable slotable = item as ISlotable;
			if (slotable != null)
			{
				List<ItemSlot> slotItems = slotable.GetSlotItems();
				DrawPerks(slotItems, slotable.GetSlotQuantity());
				if (slotItems.Count > 0)
				{
					DrawBonus();
				}
			}
			Reposition();
		}

		private void DrawPerks(List<ItemSlot> slotItems, int slotQuantity)
		{
			if (slotItems.Count == 0)
			{
				perkTable.gameObject.SetActive(false);
				return;
			}
			perkTable.gameObject.SetActive(true);
			for (int i = 0; i < slotItems.Count; i++)
			{
				if (slotItems[i].HasPerk())
				{
					DrawPerkCell(slotItems[i].GetImage());
				}
				else
				{
					CreatePerkCell(perkInfoEmptyPrf);
				}
			}
		}

		public void Clear()
		{
			foreach (Transform item in perkTable.gameObject.transform)
			{
				Object.Destroy(item.gameObject);
			}
			perkTable.gameObject.SetActive(false);
			bonusLabel.gameObject.SetActive(false);
			bonusSprite.gameObject.SetActive(false);
		}

		private GameObject DrawPerkCell(string name)
		{
			GameObject gameObject = CreatePerkCell(perkInfoPrf);
			UITexture component = gameObject.GetComponent<UITexture>();
			component.mainTexture = GlobalLoad.GetLoadTexture2D("UI/Perks/ICON_" + name);
			component.color = Color.white;
			return gameObject;
		}

		private void DrawBonus()
		{
			bonusLabel.gameObject.SetActive(true);
			string text = Localization.Get("Properties_nobonus");
			bonusLabel.text = text.ToUpper();
		}

		private GameObject CreatePerkCell(GameObject prf)
		{
			GameObject gameObject = Object.Instantiate(prf);
			gameObject.transform.parent = perkTable.gameObject.transform;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			return gameObject;
		}

		public void TintColor(Color col)
		{
			bonusLabel.color = col;
		}

		private void Reposition()
		{
			perkTable.Reposition();
			mainTable.Reposition();
		}
	}
}
