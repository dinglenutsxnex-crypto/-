using Nekki.UI;
using SF3.UserData;
using UnityEngine;

namespace SF3.Items
{
	public class ItemPerkCard : MonoBehaviour
	{
		public Transform itemCardPosition;

		public GameObject reelItemPrf;

		public Transform progressPosition;

		public GameObject progressPrf;

		public NekkiUILabel itemNameLbl;

		public ItemSlotCtr itemSlotDrawer;

		private CardItem itemCard;

		private UIProgressBar _progress;

		public void Init()
		{
			Transform transform = Object.Instantiate(reelItemPrf).transform;
			transform.parent = itemCardPosition;
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
			GameObject gameObject = Object.Instantiate(progressPrf);
			_progress = gameObject.GetComponent<UIProgressBar>();
			_progress.gameObject.SetActive(false);
			Transform transform2 = _progress.transform;
			transform2.parent = progressPosition;
			transform2.localPosition = Vector3.zero;
			transform2.localScale = Vector3.one;
			itemCard = transform.GetComponent<CardItem>();
			itemCard.gameObject.SetActive(true);
		}

		public void SetItem(int id)
		{
			SetItem(UserManager.UserModelInfo.GetItemByID(id));
		}

		public void SetItem(BaseItem item)
		{
			SetBase(item);
			SetSlotable(item);
			SetEquipment(item);
		}

		public void SetPerkSelection(BaseItem item)
		{
			itemSlotDrawer.SetSelection(item as Perk);
		}

		private void SetBase(BaseItem item)
		{
			itemCard.Init(item);
			base.gameObject.SetActive(true);
			itemCard.UpdateShade(0f);
			itemNameLbl.Alias = item.alias;
			itemCard.SetImage();
		}

		private void SetSlotable(BaseItem item)
		{
			ISlotable slotable = item as ISlotable;
			if (slotable != null)
			{
				itemSlotDrawer.DrawSlots(slotable.GetSlotItems());
			}
		}

		private void SetEquipment(BaseItem item)
		{
			_progress.gameObject.SetActive(false);
			Equipment equipment = item as Equipment;
			if (equipment != null)
			{
				_progress.gameObject.SetActive(true);
				_progress.value = equipment.GetBarValue();
			}
		}
	}
}
