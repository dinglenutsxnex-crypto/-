using System;
using System.Collections.Generic;
using DG.Tweening;
using Nekki;
using Nekki.UI;
using SF3.Items;
using SF3.Settings;
using SF3.UserData;
using UnityEngine;
using sf3DTO;

public class CardItem : ReelItem
{
	public enum FlipPivot
	{
		LEFT = 0,
		CENTER = 1,
		RIGHT = 2
	}

	[SerializeField]
	private UISprite _lootSprite;

	[SerializeField]
	private UISprite _fraction;

	[SerializeField]
	private UITexture _lootTexture;

	[SerializeField]
	private UITexture _perkTexture;

	[SerializeField]
	private NekkiUILabel _perkLabel;

	[SerializeField]
	private NekkiUILabel _rewardStatusLbl;

	[SerializeField]
	private NekkiUILabel _requirementLabel;

	[SerializeField]
	private UILabel _saleLabel;

	[SerializeField]
	private UIButton _tryOnBtn;

	[SerializeField]
	private UISprite _tryOnBg;

	[SerializeField]
	private UITexture _shadowPerkSprite;

	[SerializeField]
	private UISprite _perkTypeIcon;

	[SerializeField]
	private UISprite _background;

	[SerializeField]
	private UISprite _backCloth;

	[SerializeField]
	private UISprite _shopIcon;

	[SerializeField]
	private GameObject _saleObject;

	[SerializeField]
	private GameObject _face;

	[SerializeField]
	private GameObject _back;

	[SerializeField]
	private GameObject _requirement;

	[SerializeField]
	private float _darkenForRequirement = 0.3f;

	[SerializeField]
	private UIAtlas _perkAtlas;

	[SerializeField]
	private UIAtlas _inventoryAtlas;

	[SerializeField]
	private Material _perkMaterial;

	private float _minX;

	private readonly Dictionary<RewardStatus, UnityEngine.Color> _rewardStatusVsColor = new Dictionary<RewardStatus, UnityEngine.Color>
	{
		{
			RewardStatus.NEW,
			new Color32(228, 184, 30, byte.MaxValue)
		},
		{
			RewardStatus.UPGRADE,
			UnityEngine.Color.white
		}
	};

	private Dictionary<SF3.Items.PerkType, string> _perkNames = new Dictionary<SF3.Items.PerkType, string>
	{
		{
			SF3.Items.PerkType.Enchantment,
			"PERK_DESC_POWER"
		},
		{
			SF3.Items.PerkType.Move,
			"PERK_DESC_SUPER_MOVE"
		},
		{
			SF3.Items.PerkType.Perk,
			"PERK_DESC_PERK"
		},
		{
			SF3.Items.PerkType.None,
			string.Empty
		}
	};

	private Dictionary<EquipmentType, string> _inventorySlotsNames = new Dictionary<EquipmentType, string>
	{
		{
			EquipmentType.Armor,
			"armor_icon"
		},
		{
			EquipmentType.Helmet,
			"head_icon"
		},
		{
			EquipmentType.Magic,
			"magic_icon"
		},
		{
			EquipmentType.Ranged,
			"knife_icon"
		},
		{
			EquipmentType.Weapon,
			"swords_icon"
		},
		{
			EquipmentType.None,
			string.Empty
		}
	};

	public Vector2 Border
	{
		get
		{
			return _background.border;
		}
	}

	public bool IsPerk
	{
		get
		{
			return base.Item is SF3.Items.Perk;
		}
	}

	public bool IsBackClothNow
	{
		get
		{
			return _back.activeSelf;
		}
		private set
		{
			_back.SetActive(value);
			_face.SetActive(!value);
		}
	}

	public event Action OnFlipEnded;

	private void Awake()
	{
		_transform = base.transform;
		_tweenScale = GetComponent<TweenScale>();
		if (_tweenScale != null)
		{
			_tweenScale.enabled = false;
		}
	}

	public override Vector2 LocalSize()
	{
		return _background.localSize;
	}

	private Material InitCutoutShader(float ImageWidth, float AtlasWidth)
	{
		Material material = new Material(Shader.Find("Nekki/UI/ReelItemTextureShader"));
		material.SetFloat("_minX", _minX);
		material.SetFloat("_imageWidth", ImageWidth + 2f);
		material.SetFloat("_atlasWidth", AtlasWidth);
		return material;
	}

	public void ResetFlip()
	{
		_transform.eulerAngles = new Vector3(0f, 0f, 0f);
		IsBackClothNow = false;
	}

	public void RotateHorizontally(float angle, float animationTime = 0f, FlipPivot pivot = FlipPivot.CENTER)
	{
		if (animationTime.IsEqualByEpsilon(0f))
		{
			_transform.eulerAngles = new Vector3(0f, angle, 0f);
			return;
		}
		Sequence rotateSequence = GetRotateSequence(angle, animationTime, pivot);
		rotateSequence.Play();
	}

	public Sequence GetRotateSequence(float angle, float animationTime, FlipPivot pivot = FlipPivot.CENTER)
	{
		Sequence sequence = DOTween.Sequence();
		sequence.Append(RotateTween(FlipPivot.RIGHT, angle, animationTime));
		return sequence;
	}

	public void FlipHorizontally(float animationTime = 0f, FlipPivot pivot = FlipPivot.CENTER)
	{
		if (animationTime.IsEqualByEpsilon(0f))
		{
			if (IsBackClothNow)
			{
				_transform.eulerAngles = new Vector3(0f, 0f, 0f);
			}
			else
			{
				_transform.eulerAngles = new Vector3(0f, 180f, 0f);
			}
			IsBackClothNow = !IsBackClothNow;
		}
		else
		{
			Sequence flipSequence = GetFlipSequence(animationTime, pivot);
			flipSequence.Play();
		}
	}

	public Sequence GetFlipSequence(float animationTime, FlipPivot pivot = FlipPivot.CENTER)
	{
		Sequence sequence = DOTween.Sequence();
		Vector3 rotation = _transform.eulerAngles;
		float firstRotation = -90f;
		float secondRotation = -90f;
		sequence.Append(RotateTween(pivot, firstRotation, animationTime / 2f));
		sequence.AppendCallback(delegate
		{
			IsBackClothNow = !IsBackClothNow;
		});
		sequence.Append(RotateTween(pivot, secondRotation, animationTime / 2f).SetEase(Ease.OutQuad));
		sequence.OnComplete(delegate
		{
			if (this.OnFlipEnded != null)
			{
				this.OnFlipEnded();
			}
			_transform.localRotation = Quaternion.Euler(new Vector3(_transform.localRotation.x, rotation.y - (firstRotation + secondRotation), _transform.localRotation.z));
		});
		return sequence;
	}

	public Tween RotateTween(FlipPivot pivot, float rotateTo, float duration)
	{
		float oldRotationY = 0f;
		Vector3 rotationPoint = GetRotationPoint(pivot);
		SetFlipPivot(rotationPoint);
		return DOTween.To(() => _transform.localRotation.y, delegate(float rotationY)
		{
			_transform.RotateAround(GetFlipPivot().transform.position, Vector3.up, rotationY - oldRotationY);
			oldRotationY = rotationY;
		}, rotateTo, duration).SetEase(Ease.Linear);
	}

	private Vector3 GetRotationPoint(FlipPivot pivot)
	{
		Vector3 result = Vector3.zero;
		switch (pivot)
		{
		case FlipPivot.LEFT:
			result = new Vector3(-_background.width / 2, 0f, 0f);
			break;
		case FlipPivot.CENTER:
			result = base.transform.localPosition;
			break;
		case FlipPivot.RIGHT:
			result = new Vector3(_background.width / 2, 0f, 0f);
			break;
		}
		return result;
	}

	public void ShowBackCloth()
	{
		Vector3 eulerAngles = _transform.eulerAngles;
		_transform.eulerAngles = new Vector3(eulerAngles.x, 180f, eulerAngles.z);
		IsBackClothNow = true;
	}

	public override void Init(BaseItem baseItem)
	{
		base.Item = baseItem;
		_transform = base.transform;
		InitDefaults(baseItem);
		InitFaction();
		FindMaxLocalDepth();
		HideTryOnBtn();
		HideRewardStatus();
		ShowRequirement();
		if (IsPerk)
		{
			string value = string.Empty;
			if (_perkNames.TryGetValue((baseItem as SF3.Items.Perk).GetPerkType(), out value))
			{
				_perkLabel.Alias = value;
			}
			else
			{
				_perkLabel.Alias = string.Empty;
			}
			SetPerkMask();
			InitPerkType(baseItem);
		}
		else
		{
			InitShadowPerks(baseItem);
		}
		InitRarity();
		if (beginScale == Vector3.zero)
		{
			beginScale = base.transform.localScale;
		}
		if (_tutorialComponent != null)
		{
			_tutorialComponent.SetId("card_" + baseItem.id);
			TutorialManager.Instance.onSet += delegate(string id)
			{
				if (id == "card_" + baseItem.id)
				{
					HideTryOnBtn();
				}
			};
			TutorialManager.Instance.onBlockSet += delegate
			{
				if (_selectionBorderActive && (bool)_tweenScale)
				{
					_tweenScale.enabled = false;
					_tweenScale.value = _tweenScale.to;
					base.transform.localScale = _tweenScale.to;
				}
			};
			TutorialManager.Instance.onBlockRelease += delegate
			{
				if (_selectionBorderActive && (bool)_tweenScale)
				{
					ActivateSelectionBorder(true);
				}
			};
		}
		if (_tweenScale != null)
		{
			_tweenScale.enabled = false;
		}
		HideSale();
		HideShopIcon();
		IsBackClothNow = false;
	}

	private void InitShadowPerks(BaseItem baseItem)
	{
		Equipment equipment = baseItem as Equipment;
		if (equipment != null)
		{
			if (!string.IsNullOrEmpty(equipment.ShadowMark))
			{
				_shadowPerkSprite.mainTexture = GlobalLoad.GetLoadTexture2DInternal("shadowPerksIcons", equipment.ShadowMark);
				_shadowPerkSprite.gameObject.SetActive(true);
			}
			else
			{
				_shadowPerkSprite.gameObject.SetActive(false);
			}
			_perkTypeIcon.gameObject.SetActive(false);
		}
	}

	private void InitPerkType(BaseItem baseItem)
	{
		SF3.Items.Perk perk = baseItem as SF3.Items.Perk;
		if (perk != null)
		{
			string value = string.Empty;
			if (_inventorySlotsNames.TryGetValue(perk.GetTargetItemType(), out value))
			{
				_perkTypeIcon.spriteName = value;
				_perkTypeIcon.gameObject.SetActive(!string.IsNullOrEmpty(value));
			}
			else
			{
				_perkTypeIcon.gameObject.SetActive(false);
			}
			_shadowPerkSprite.gameObject.SetActive(false);
		}
	}

	private void InitDefaults(BaseItem baseItem)
	{
		base.Item = baseItem;
		SetImage();
		_fraction.spriteName = "noFraction";
		_background.spriteName = "gray_card";
	}

	private void InitRarity()
	{
		IRarable rarable = base.Item as IRarable;
		if (rarable != null)
		{
			switch (rarable.GetRarityType())
			{
			case Rarity.Common:
				_background.spriteName = "common_card";
				break;
			case Rarity.Epic:
				_background.spriteName = "epic_card";
				break;
			case Rarity.Rare:
				_background.spriteName = "rare_card";
				break;
			case Rarity.Legendary:
				_background.spriteName = "legendary_card";
				break;
			default:
				Debug.LogError(string.Concat("No raraity found for card. Rarity key: [", rarable.GetRarityType(), "]"));
				break;
			}
		}
		if (IsPerk)
		{
			_perkTexture.material.color = GameSettings.ItemSettings.GetRarityColor(rarable.GetRarityType());
		}
	}

	private void InitFaction()
	{
		IFactionable factionable = base.Item as IFactionable;
		if (factionable != null)
		{
			_fraction.spriteName = GameSettings.ItemSettings.GetFactionAlias(factionable.GetFactionType());
		}
	}

	public void UpdateShade(float alpha)
	{
		if (!_requirement.activeSelf)
		{
			alpha = Mathf.Clamp01(alpha);
			_shade.alpha = alpha;
		}
	}

	public void SetBackClothSprite(string spriteName)
	{
		_backCloth.spriteName = spriteName;
	}

	public void SetBackClothDepth(int depth)
	{
		_backCloth.depth = depth;
	}

	private void SetPerkMask()
	{
		_lootSprite.gameObject.SetActive(false);
		_lootTexture.gameObject.SetActive(false);
		_perkTexture.gameObject.SetActive(true);
		_perkLabel.gameObject.SetActive(true);
		Texture2D loadTexture2D = GlobalLoad.GetLoadTexture2D("UI/Inventory/" + base.Item.Image);
		if (_perkTexture.material == null)
		{
			_perkTexture.material = new Material(_perkMaterial);
		}
		_perkTexture.material.SetTexture("_Mask", loadTexture2D);
	}

	public void SetImage(UIAtlas atlas)
	{
		_lootSprite.atlas = atlas;
		bool flag = false;
		foreach (UISpriteData sprite in atlas.spriteList)
		{
			if (sprite.name == base.Item.Image)
			{
				flag = true;
				break;
			}
		}
		_lootSprite.spriteName = ((!flag) ? "no_image" : base.Item.Image);
		_perkTexture.gameObject.SetActive(false);
		_lootSprite.gameObject.SetActive(!IsPerk);
		_lootTexture.gameObject.SetActive(false);
		_perkLabel.gameObject.SetActive(IsPerk);
	}

	public void SetImage()
	{
		Texture2D loadTexture2D = GlobalLoad.GetLoadTexture2D("UI/Inventory/" + base.Item.Image);
		_lootSprite.gameObject.SetActive(false);
		_lootTexture.gameObject.SetActive(true);
		_perkTexture.gameObject.SetActive(false);
		_perkLabel.gameObject.SetActive(false);
		_lootTexture.material = InitCutoutShader(_lootTexture.width, _lootTexture.width);
		float value = Mathf.Abs(1f - _lootTexture.localSize.x / _background.localSize.x) + 0.03f;
		_lootTexture.material.SetFloat("_minX", value);
		_lootTexture.mainTexture = loadTexture2D;
	}

	public void ShowTryOnBtn()
	{
		if ((bool)_tryOnBtn)
		{
			_requirement.SetActive(false);
			_tryOnBtn.gameObject.SetActive(true);
		}
	}

	public void HideTryOnBtn()
	{
		if ((bool)_tryOnBtn)
		{
			ShowRequirement();
			_tryOnBtn.gameObject.SetActive(false);
		}
	}

	public void AddTryOnCallback(EventDelegate callback)
	{
		_tryOnBtn.onClick.Add(callback);
	}

	public void ShowSale(string value)
	{
		if ((bool)_saleObject && (bool)_saleLabel)
		{
			_saleObject.SetActive(true);
			_saleLabel.text = value;
		}
	}

	public void HideSale()
	{
		if ((bool)_saleObject)
		{
			_saleObject.SetActive(false);
		}
	}

	public void ShowShopIcon(string icon)
	{
		if ((bool)_shopIcon)
		{
			_shopIcon.gameObject.SetActive(true);
			_shopIcon.spriteName = icon;
		}
	}

	public void HideShopIcon()
	{
		if ((bool)_shopIcon)
		{
			_shopIcon.gameObject.SetActive(false);
		}
	}

	public override void ShowRewardStatus(RewardStatus status)
	{
		if (status == RewardStatus.NEW)
		{
			_rewardStatusLbl.Alias = ((!(base.Item is SF3.Items.Perk)) ? "reward_new_item" : "reward_new_ability");
		}
		else
		{
			_rewardStatusLbl.Alias = "reward_upgrade";
		}
		_rewardStatusLbl.color = _rewardStatusVsColor[status];
		_rewardStatusLbl.gameObject.SetActive(true);
	}

	public override void HideRewardStatus()
	{
		_rewardStatusLbl.gameObject.SetActive(false);
	}

	private void ShowRequirement()
	{
		Equipment equipment = base.Item as Equipment;
		if (equipment != null && equipment.level > UserManager.GetLevel())
		{
			_requirementLabel.Format(equipment.level);
			UpdateShade(_darkenForRequirement);
			_requirement.SetActive(true);
		}
		else
		{
			_requirement.SetActive(false);
			UpdateShade(0f);
		}
	}
}
