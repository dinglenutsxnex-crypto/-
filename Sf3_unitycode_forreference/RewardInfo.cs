using System.Collections.Generic;
using Nekki.UI;
using SF3;
using SF3.Items;
using SF3_Attributes;
using UnityEngine;

public class RewardInfo : MonoBehaviour
{
	[SerializeField]
	private NekkiUILabel itemName;

	[SerializeField]
	private GameObject progressBarPrf;

	[SerializeField]
	private Transform progressBarPlaceholder;

	[SerializeField]
	private AttributesDrawer attributeDrawer;

	[SerializeField]
	private NekkiUILabel description;

	private UIProgressBar _progressBar;

	private ProgressBarAnimation _progressAnimation;

	public UIProgressBar progressBar
	{
		get
		{
			return _progressBar;
		}
	}

	public ProgressBarAnimation progressAnimation
	{
		get
		{
			return _progressAnimation;
		}
	}

	public void Init()
	{
		CreateProgressBar();
		HideProgress();
		HideAttributes();
		HideItemName();
	}

	private void CreateProgressBar()
	{
		GameObject gameObject = Object.Instantiate(progressBarPrf);
		gameObject.transform.parent = progressBarPlaceholder;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		_progressBar = gameObject.GetComponent<UIProgressBar>();
		_progressAnimation = gameObject.GetComponent<ProgressBarAnimation>();
		gameObject.SetActive(false);
	}

	public void SetPosition(float x)
	{
		progressBar.transform.position = new Vector3(x, progressBar.transform.position.y);
		attributeDrawer.transform.position = new Vector3(x, attributeDrawer.transform.position.y);
		itemName.transform.position = new Vector3(x, itemName.transform.position.y);
		description.transform.position = new Vector3(x, description.transform.position.y);
		Vector3 localPosition = attributeDrawer.transform.localPosition;
		localPosition.x -= attributeDrawer.table.padding.x;
		attributeDrawer.transform.localPosition = localPosition;
	}

	public void ShowItemName(string nameAlias)
	{
		itemName.Alias = nameAlias;
		itemName.gameObject.SetActive(true);
	}

	public void HideItemName()
	{
		itemName.gameObject.SetActive(false);
	}

	public void ShowProgress(BaseItem item)
	{
		SetProgress(item);
		progressBar.gameObject.SetActive(true);
	}

	public void HideProgress()
	{
		progressBar.gameObject.SetActive(false);
	}

	public void SetProgress(BaseItem item)
	{
		IStackable stackable = item as IStackable;
		if (stackable != null)
		{
			progressBar.value = stackable.GetBarValue();
		}
	}

	public void ShowAttributes(BaseItem item, BaseItem diffItem = null)
	{
		IAttributable attributable = item as IAttributable;
		IAttributable attributable2 = ((diffItem == null) ? null : (diffItem as IAttributable));
		if (attributable != null)
		{
			if (attributable2 != null)
			{
				ShowAttributes(attributable.GetAttributesForDisplayData(), attributable2.GetAttributesForDisplayData());
			}
			else
			{
				ShowAttributes(attributable.GetAttributesForDisplayData());
			}
		}
	}

	public void ShowAttributes(SortedDictionary<AttributeType, float> attrs)
	{
		attributeDrawer.Draw(attrs);
	}

	public void ShowAttributes(SortedDictionary<AttributeType, float> attrs, SortedDictionary<AttributeType, float> equippedAttr)
	{
		attributeDrawer.Draw(attrs, equippedAttr);
	}

	public void HideAttributes()
	{
		attributeDrawer.gameObject.SetActive(false);
	}

	public void ShowDescription(BaseItem item)
	{
		IDescribed described = item as IDescribed;
		if (described != null)
		{
			ShowDescription(described.GetDescription());
		}
	}

	public void ShowDescription(string alias)
	{
		description.Alias = alias;
		description.gameObject.SetActive(true);
	}

	public void HideDescription()
	{
		description.gameObject.SetActive(false);
	}

	public void ShowAll(BaseItem item, BaseItem diffItem = null)
	{
		ShowItemName(item.alias);
		ShowProgress(item);
		ShowAttributes(item, diffItem);
		ShowDescription(item);
	}

	public void HideAll()
	{
		HideItemName();
		HideProgress();
		HideAttributes();
		HideDescription();
	}
}
