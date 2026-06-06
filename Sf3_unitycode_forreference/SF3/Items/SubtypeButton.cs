using System;
using SF3.UserData;
using UnityEngine;

namespace SF3.Items
{
	[Serializable]
	public class SubtypeButton
	{
		public UISprite Badge;

		public UILabel BadgeLabel;

		public EquipmentType TypeEquipment;

		public Transform subTypeButton;

		public static SubtypeButton CurrentlySelected;

		private TweenPosition tweenPosition;

		private bool inited;

		public Collider Collider { get; private set; }

		public long NewItems { get; private set; }

		public static void Reset()
		{
			CurrentlySelected = null;
		}

		public void Init()
		{
			if (!inited)
			{
				inited = true;
				tweenPosition = subTypeButton.gameObject.AddComponent<TweenPosition>();
				tweenPosition.duration = 0.15f;
				tweenPosition.from = subTypeButton.localPosition;
				tweenPosition.to = tweenPosition.from + new Vector3(14f, 0f, 0f);
				tweenPosition.enabled = false;
				Collider = subTypeButton.GetComponent<Collider>();
			}
		}

		public void UpateBadge()
		{
			NewItems = UserBadgesManager.Instance.WhichItemsisNew(UserBadgesManager.BadgeTypes.Inventory, UserManager.UserModelInfo.GetEquipmentOfType(TypeEquipment));
			Badge.gameObject.SetActive(NewItems > 0);
			BadgeLabel.text = NewItems.ToString();
		}

		public void ResetBadge()
		{
			Badge.gameObject.SetActive(false);
		}

		public void Select()
		{
			if (!inited)
			{
				Init();
			}
			if (CurrentlySelected != null)
			{
				if (CurrentlySelected == this)
				{
					return;
				}
				CurrentlySelected.Unselect();
			}
			CurrentlySelected = this;
			ResetBadge();
		}

		public void Unselect()
		{
			if (!inited)
			{
				Init();
			}
		}
	}
}
