using System;
using SF3.Settings;
using UnityEngine;

public class Boosterpack : MonoBehaviour
{
	private BoosterpackScrollItem _scrollItem;

	[SerializeField]
	private UITexture _slicableTexture;

	[SerializeField]
	private UISprite _graphicsSprite;

	[SerializeField]
	private UILabel _zoneLabel;

	[SerializeField]
	private UILabel _rarityLabel;

	private int _modelID;

	[SerializeField]
	private NekkiUICamera _RTTCamera;

	[SerializeField]
	private ParticleSystem _shining;

	[SerializeField]
	private ParticleSystem _explosion;

	private Action _onClick;

	public BoosterpackScrollItem ScrollItem
	{
		get
		{
			return _scrollItem;
		}
	}

	public UITexture SlicableTexture
	{
		get
		{
			return _slicableTexture;
		}
	}

	public int ModelID
	{
		get
		{
			return _modelID;
		}
	}

	private void OnEnable()
	{
		RefreshBoosterTexture();
	}

	public void Init(BoosterpackScrollItem _parentScrollItem, Action onClick = null)
	{
		_scrollItem = _parentScrollItem;
		_modelID = _parentScrollItem.ModelID;
		_onClick = onClick;
		RefreshGraphics();
		RefreshBoosterTexture();
	}

	public void RefreshGraphics()
	{
		RefreshGraphicsSprite();
		RefreshZoneLabel();
		RefreshRarityLabel();
	}

	public void OnClick()
	{
		if (_onClick != null)
		{
			_onClick();
		}
	}

	public void ShineOn()
	{
		if (!_shining.IsAlive())
		{
			_shining.Play();
		}
	}

	public void Explode()
	{
		_shining.Stop();
		_explosion.Play();
	}

	public void SetShiningLifetime(float value)
	{
		ParticleSystem.MainModule main = _shining.main;
		main.startLifetime = value;
	}

	private void RefreshGraphicsSprite()
	{
		_graphicsSprite.spriteName = GameSettings.ItemSettings.GetBoosterSpriteName(_scrollItem.Rarity);
	}

	private void RefreshZoneLabel()
	{
		_zoneLabel.text = string.Format("CHAPTER {0}", JS.Instance.GetBoosterByID(_modelID).Zone);
		_zoneLabel.color = GameSettings.ItemSettings.GetBoosterTextColor(_scrollItem.Rarity);
	}

	private void RefreshRarityLabel()
	{
		_rarityLabel.text = string.Format("{0}", _scrollItem.Rarity.ToString());
		_rarityLabel.color = GameSettings.ItemSettings.GetBoosterTextColor(_scrollItem.Rarity);
	}

	private void RefreshBoosterTexture()
	{
		if (_RTTCamera.gameObject.activeInHierarchy)
		{
			_RTTCamera.RenderAndDisable();
		}
	}
}
