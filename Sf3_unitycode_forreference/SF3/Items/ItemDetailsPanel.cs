using Nekki.UI;
using SF3.UserData;
using UnityEngine;

namespace SF3.Items
{
	public class ItemDetailsPanel : MonoBehaviour
	{
		public Transform itemCardPosition;

		public NekkiUILabel itemNameLbl;

		public GameObject itemCardPrefab;

		public UIButton detailsButton;

		public string normalButtonName;

		public string pressedButtonName;

		public AttributesDrawer attributeDrawer;

		public Transform progressPosition;

		public GameObject progressPrf;

		public Transform perkInfoBarPosition;

		public GameObject perkInfoBarPrf;

		public GameObject perkInfoBarEmtyCellPrf;

		private PerkInfoDrawer _perkInfroDrawer;

		private UIProgressBar _progress;

		private CardItem itemCard;

		public bool active
		{
			get
			{
				return base.gameObject.activeSelf;
			}
		}

		private void SwitchButtonImage(string spriteName)
		{
			if (detailsButton != null)
			{
				detailsButton.GetComponent<UISprite>().spriteName = spriteName;
				detailsButton.normalSprite = spriteName;
				detailsButton.pressedSprite = spriteName;
				detailsButton.hoverSprite = spriteName;
			}
		}

		public void Init()
		{
			Transform transform = Object.Instantiate(itemCardPrefab).transform;
			transform.parent = itemCardPosition;
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
			itemCard = transform.GetComponent<CardItem>();
			itemCard.gameObject.SetActive(true);
			GameObject gameObject = Object.Instantiate(progressPrf);
			_progress = gameObject.GetComponent<UIProgressBar>();
			_progress.gameObject.SetActive(false);
			Transform transform2 = _progress.transform;
			transform2.parent = progressPosition;
			transform2.localPosition = Vector3.zero;
			transform2.localScale = Vector3.one;
			GameObject gameObject2 = Object.Instantiate(perkInfoBarPrf);
			gameObject2.transform.parent = perkInfoBarPosition.transform;
			gameObject2.transform.localScale = Vector3.one;
			gameObject2.transform.localPosition = Vector3.zero;
			_perkInfroDrawer = gameObject2.GetComponent<PerkInfoDrawer>();
			_perkInfroDrawer.TintColor(new Color(0.3529412f, 1f / 3f, 4f / 15f));
			_perkInfroDrawer.perkInfoEmptyPrf = perkInfoBarEmtyCellPrf;
		}

		public void DisablePanel()
		{
			SwitchButtonImage(normalButtonName);
			base.gameObject.SetActive(false);
		}

		public void ActivatePanel(BaseItem item)
		{
			SwitchButtonImage(pressedButtonName);
			base.gameObject.SetActive(true);
			itemCard.Init(item);
			itemCard.UpdateShade(0f);
			itemNameLbl.Alias = item.alias;
			itemCard.SetImage();
			IAttributable attributable = item as IAttributable;
			if (attributable != null)
			{
				attributeDrawer.Draw(attributable.GetAttributesForDisplayData());
			}
			IStackable stackable = item as IStackable;
			if (stackable != null)
			{
				_progress.gameObject.SetActive(true);
				_progress.value = stackable.GetBarValue();
			}
			else
			{
				_progress.gameObject.SetActive(false);
			}
			_perkInfroDrawer.Draw(item);
		}

		public void ActivatePanel(int id)
		{
			ActivatePanel(UserManager.UserModelInfo.GetItemByID(id));
		}

		public void Refresh(BaseItem item)
		{
			SwitchButtonImage(pressedButtonName);
			itemCard.Init(item);
			itemNameLbl.Alias = item.alias;
			itemCard.SetImage();
		}
	}
}
