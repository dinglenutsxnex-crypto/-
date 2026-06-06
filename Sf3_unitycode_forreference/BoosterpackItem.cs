using Nekki.UI;
using SF3.Items;
using UnityEngine;
using sf3DTO;

public class BoosterpackItem : ReelItem
{
	[SerializeField]
	private UILabel _zoneLabel;

	[SerializeField]
	private UILabel _rarityLabel;

	[SerializeField]
	private UISprite _image;

	[SerializeField]
	private NekkiUILabel _rewardStatusLbl;

	private void Awake()
	{
		_transform = base.transform;
		_selectionBorderActive = false;
		_tweenScale = GetComponent<TweenScale>();
		if (_tweenScale != null)
		{
			_tweenScale.enabled = false;
		}
	}

	public override Vector2 LocalSize()
	{
		return _image.localSize;
	}

	private void InitRarity(BaseItem baseItem)
	{
		_zoneLabel.text = string.Format("CHAPTER {0}", JS.Instance.GetBoosterByID(baseItem.id).Zone);
		Rarity rarityType = JS.Instance.GetBoosterByID(baseItem.id).GetRarityType();
		switch (rarityType)
		{
		case Rarity.Common:
			_image.spriteName = "booster_common";
			_rarityLabel.text = "COMMON";
			break;
		case Rarity.Epic:
			_image.spriteName = "booster_epic";
			_rarityLabel.text = "EPIC";
			break;
		case Rarity.Rare:
			_image.spriteName = "booster_rare";
			_rarityLabel.text = "RARE";
			break;
		case Rarity.Legendary:
			_image.spriteName = "booster_legendary";
			_rarityLabel.text = "LEGENDARY";
			break;
		default:
			Debug.LogError(string.Concat("No rarity found for boosterpack. Rarity key: [", rarityType, "]"));
			break;
		}
	}

	public override void Init(BaseItem baseItem)
	{
		base.Item = baseItem;
		_transform = base.transform;
		FindMaxLocalDepth();
		InitRarity(baseItem);
		if (beginScale == Vector3.zero)
		{
			beginScale = base.transform.localScale;
		}
	}

	private void InitDefaults(BaseItem baseItem)
	{
		base.Item = baseItem;
	}

	public override void ShowRewardStatus(RewardStatus status)
	{
		_rewardStatusLbl.gameObject.SetActive(true);
	}

	public override void HideRewardStatus()
	{
		_rewardStatusLbl.gameObject.SetActive(false);
	}
}
