using SF3.Settings;
using UnityEngine;
using sf3DTO;

public class BoosterpackScrollItem : MonoBehaviour
{
	[SerializeField]
	private UIDragScrollView _dragScrollView;

	[SerializeField]
	private UILabel _countLabel;

	[SerializeField]
	private UILabel _zoneLabel;

	[SerializeField]
	private UILabel _rarityLabel;

	[SerializeField]
	private UIButton _button;

	[SerializeField]
	private UISprite _Image;

	private int _modelID;

	private int _count;

	private Rarity _rarity;

	public UIDragScrollView DragScrollView
	{
		get
		{
			return _dragScrollView;
		}
	}

	public int ModelID
	{
		get
		{
			return _modelID;
		}
	}

	public Rarity Rarity
	{
		get
		{
			return _rarity;
		}
	}

	public void Init(int modelID, int count, EventDelegate onClick = null)
	{
		_modelID = modelID;
		_count = count;
		_rarity = JS.Instance.GetBoosterByID(_modelID).GetRarityType();
		if (onClick != null)
		{
			_button.onClick.Add(onClick);
		}
		RefreshGraphics();
	}

	public bool WillBeDeletedAfterSpend()
	{
		return _count <= 1;
	}

	public void SpendBoosterpack()
	{
		_count--;
		if (_count > 0)
		{
			RefreshCountLabel();
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void RefreshGraphics()
	{
		RefreshCountLabel();
		RefreshZoneLabel();
		RefreshRarityLabel();
		RefreshImage();
	}

	private void RefreshCountLabel()
	{
		if (_count > 1)
		{
			_countLabel.enabled = true;
			_countLabel.text = string.Format("X{0}", _count);
		}
		else
		{
			_countLabel.enabled = false;
		}
	}

	private void RefreshZoneLabel()
	{
		_zoneLabel.text = string.Format("CHAPTER {0}", JS.Instance.GetBoosterByID(_modelID).Zone);
		_zoneLabel.color = GameSettings.ItemSettings.GetBoosterTextColor(_rarity);
	}

	private void RefreshRarityLabel()
	{
		_rarityLabel.text = string.Format("{0}", _rarity.ToString());
		_rarityLabel.color = GameSettings.ItemSettings.GetBoosterTextColor(_rarity);
	}

	private void RefreshImage()
	{
		_Image.spriteName = GameSettings.ItemSettings.GetBoosterSpriteName(_rarity);
	}
}
