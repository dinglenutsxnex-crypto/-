using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using Nekki.UI;
using SF3;
using SF3.Effects;
using SF3.Items;
using UnityEngine;
using sf3DTO;

public class RewardsWindow : NekkiUIModule
{
	private enum States
	{
		None = 0,
		CoinAnimation = 1,
		CoinAnimationFinish = 2,
		RewardAnimation = 3,
		RewardAnimationFinish = 4
	}

	public Action onCloseAction;

	[SerializeField]
	private UIButton _breakBtn;

	[Header("Base items prefab:")]
	[SerializeField]
	private RewardsWindowUnit _baseUnit;

	[Header("Continue:")]
	[SerializeField]
	private UIButton _btnContinue;

	[Header("Header (Win/Lose):")]
	[SerializeField]
	private NekkiUILabel _header;

	[Header("Window content:")]
	[SerializeField]
	private NekkiUILabel _experience;

	[SerializeField]
	private Transform _expBarPlaceholder;

	[SerializeField]
	private GameObject _expBarPrf;

	[SerializeField]
	private NekkiUILabel _totalGold;

	[SerializeField]
	private NekkiUILabel _totalBonus;

	[SerializeField]
	private GameObject _totalRewardObj;

	[Header("Reward coin:")]
	[SerializeField]
	private GameObject _rewardCoinPrf;

	[SerializeField]
	private GameObject _rewardBasePrf;

	[SerializeField]
	private UILabel _shadowCurrencyLbl;

	[SerializeField]
	private Transform _coinPlaceholder;

	[SerializeField]
	private float _coinStepX = 0.5f;

	[SerializeField]
	private float _coinStepY = 1f;

	[SerializeField]
	private float _coinStepYBasePadding = 10f;

	[Header("Reward item:")]
	[SerializeField]
	private GameObject _reelItemPrf;

	[SerializeField]
	private GameObject _boosterpackPrf;

	[SerializeField]
	private Transform _reelItemsPlaceholder;

	[SerializeField]
	private RewardInAnimation _rewardInAnimation;

	[SerializeField]
	private CustomSlideInput _customSlideInput;

	[SerializeField]
	private RewardInfo _rewardInfo;

	private float _xOffset;

	private float _yOffset;

	private IAnimatedExpBar _expBar;

	private List<RewardCoin> _rewardCoinList;

	private TweenAlpha _totalRewardAlpha;

	private States _state;

	private TweenAlpha _btnContinueAlphaAnimation;

	private TweenPosition _btnContinuePositionAnimation;

	private List<BaseItem> _rewardItem;

	private ICardAnimation _cardAnimation;

	private SF3.FightResult _fightResult;

	private Dictionary<string, long> _fightRewardBonus;

	private long _coins;

	private long _bonus;

	private long _totalCoins;

	private long _totalBonuses;

	private long _exp;

	private float _totalGoldAnimationDuration = 1.2f;

	private float _startCoinAnimationDuration = 0.5f;

	private Sequence _totalRewardSequence;

	private Sequence _infoSequence;

	private Camera _mainCamera;

	private static readonly Dictionary<string, string> RewardVsImageNames = new Dictionary<string, string>
	{
		{ "Reward_Bonus_HeadHit", "bonus_headhits" },
		{ "Reward_Bonus_FirstStrike", "bonus_first_strike" },
		{ "Reward_Bonus_Critical", "bonus_criticals" },
		{ "Reward_Bonus_Combo", "bonus_combo" },
		{ "Reward_Bonus_ShadowAbilities", "bonus_shadow_abilities" }
	};

	protected long totalGold
	{
		get
		{
			return _totalCoins;
		}
		set
		{
			_totalCoins = value;
			_totalGold.text = _totalCoins.ToString();
		}
	}

	protected long totalBonus
	{
		get
		{
			return _totalBonuses;
		}
		set
		{
			_totalBonuses = value;
			_totalBonus.text = _totalBonuses.ToString();
		}
	}

	public void Init(SF3.FightResult result, RewardDataProvider rewardDataProvider)
	{
		if (_customSlideInput == null)
		{
			Debug.LogError("Slide Input not found");
			return;
		}
		_customSlideInput.onPress += CardSelect;
		ScreenTexture.Instance.SetTexture(base.name, ScreenTexture.TextureOutputCamera.Main, ScreenTexture.TextureOutputFilter.Blur, null, -1);
		_fightResult = result;
		_fightRewardBonus = _fightResult.GetFightRewardBonus();
		_exp = _fightResult.GetRewardExperience();
		_coins = _fightResult.GetRewardCurrency(CurrencyType.Coin);
		_bonus = _fightResult.GetRewardCurrency(CurrencyType.Bonus);
		_rewardItem = new List<BaseItem>();
		_rewardItem.AddRange(((IEnumerable<Equipment>)_fightResult.GetRewardEquipment()).Select((Func<Equipment, BaseItem>)((Equipment equipment) => equipment)));
		_rewardItem.AddRange(((IEnumerable<SF3.Items.Perk>)_fightResult.GetRewardPerks()).Select((Func<SF3.Items.Perk, BaseItem>)((SF3.Items.Perk equipment) => equipment)));
		_rewardItem.AddRange(((IEnumerable<SF3.Items.Booster>)_fightResult.GetRewardBoosters()).Select((Func<SF3.Items.Booster, BaseItem>)((SF3.Items.Booster equipment) => equipment)));
		_rewardInfo.Init();
		_cardAnimation = GetComponent<CardAnimator>();
		_cardAnimation.onAnimationEnd += delegate
		{
			_state = States.RewardAnimationFinish;
			_breakBtn.gameObject.SetActive(false);
		};
		_cardAnimation.Init(rewardDataProvider, CreateCardsForAnimator(rewardDataProvider.rewardItemProvider), _rewardInfo);
		FillHeader(result);
		_rewardInAnimation.onAnimationEnd += StartInfoAnimation;
		_expBar = CreateExpBar();
		_expBar.CurrentLevel = rewardDataProvider.startLevel;
		_expBar.FromExp = rewardDataProvider.startExp;
		_expBar.AddedExp = _exp;
		_expBar.onAnimationEnd += StartContinueBtnAnimation;
		_expBar.Hide();
		_btnContinueAlphaAnimation = _btnContinue.GetComponent<TweenAlpha>();
		_btnContinuePositionAnimation = _btnContinue.GetComponent<TweenPosition>();
		_btnContinue.gameObject.SetActive(false);
		long rewardCurrency = _fightResult.GetRewardCurrency(CurrencyType.Shadow);
		if (rewardCurrency > 0)
		{
			ShowShadowCurrency(rewardCurrency);
		}
		_rewardCoinList = new List<RewardCoin> { CreateBaseReward("base_reward", _coins.ToString(), _bonus.ToString()) };
		foreach (string value in RewardMultipyerCounter.Instance.RewardMultipliersMap.Values)
		{
			long num = 0L;
			if (_fightRewardBonus.ContainsKey(value))
			{
				num = _fightRewardBonus[value];
			}
			if (!RewardVsImageNames.ContainsKey(value))
			{
				Debug.LogError(string.Format("No key: [{0}] found in <RewardVsImageNames>!", value));
			}
			_rewardCoinList.Add(CreateRewardCoin(RewardVsImageNames[value], num.ToString(), (int)result.GetRewardMultiplierUsagesQuantity(value)));
			_coins += num;
		}
		totalGold = _coins;
		totalBonus = _bonus;
		_totalRewardAlpha = _totalRewardObj.GetComponent<TweenAlpha>();
		_totalRewardObj.SetActive(false);
		_mainCamera = Camera.main;
		_btnContinue.onClick.Add(new EventDelegate(delegate
		{
			switch (_state)
			{
			case States.CoinAnimationFinish:
				_mainCamera.enabled = false;
				if (_rewardItem.Count > 0)
				{
					StartItemAnimation();
					_state = States.RewardAnimation;
				}
				else
				{
					Quit();
				}
				break;
			case States.RewardAnimationFinish:
				Quit();
				break;
			}
		}));
		_breakBtn.onClick.Add(new EventDelegate(OnBreakBtnClick));
	}

	private void FillHeader(SF3.FightResult result)
	{
		List<HeaderFiller> list = new List<HeaderFiller>();
		list.Add(new FightFiller());
		list.Add(new SurvivalFiller());
		List<HeaderFiller> list2 = list;
		foreach (HeaderFiller item in list2)
		{
			item.Set(result, _header);
			if (item.IsFilled())
			{
				return;
			}
		}
		Messenger.Error("Header has not been filled!", this);
	}

	private void Quit()
	{
		_mainCamera.enabled = true;
		ScreenTexture.Instance.Clear(base.name);
		NekkiUIRootModules.Instance.UnmountModule(this);
		BattleController.SystemResume();
		if (onCloseAction != null)
		{
			onCloseAction();
		}
	}

	private void Start()
	{
		StartAnimation();
	}

	public static void Show(SF3.FightResult result, RewardDataProvider rewardDataProvider, Action onClose)
	{
		UIModulesController.Instance.EnableAllModules(false);
		RewardsWindow rewardsWindow = NekkiUIRootModules.Instance.MountNGUIModule<RewardsWindow>("Rewards");
		if (onClose != null)
		{
			rewardsWindow.onCloseAction = onClose;
		}
		rewardsWindow.Init(result, rewardDataProvider);
		EffectsManager.Reset();
	}

	private RewardCoin CreateRewardCoin(string name, string value, int count)
	{
		RewardCoin rewardCoin = CreateEmptyRewardField(_rewardCoinPrf);
		rewardCoin.SetCoins(name, value, count);
		return rewardCoin;
	}

	private RewardCoin CreateBaseReward(string name, string coinsValue, string bonusValue)
	{
		RewardCoin rewardCoin = CreateEmptyRewardField(_rewardBasePrf);
		rewardCoin.Set(name, coinsValue, bonusValue);
		_yOffset += _coinStepYBasePadding;
		return rewardCoin;
	}

	private RewardCoin CreateEmptyRewardField(GameObject prefab)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
		gameObject.transform.parent = _coinPlaceholder;
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localPosition = new Vector3(_xOffset, _yOffset, 0f);
		gameObject.SetActive(false);
		_xOffset += _coinStepX;
		_yOffset += _coinStepY;
		return gameObject.GetComponent<RewardCoin>();
	}

	private IAnimatedExpBar CreateExpBar()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(_expBarPrf);
		gameObject.transform.parent = _expBarPlaceholder.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		return gameObject.GetComponent<IAnimatedExpBar>();
	}

	private void StartAnimation()
	{
		_rewardInAnimation.Play();
	}

	private void StartInfoAnimation()
	{
		_infoSequence = DOTween.Sequence();
		_infoSequence.AppendCallback(StartCoinAnimation).AppendInterval(_startCoinAnimationDuration).AppendCallback(StartTotalRewardAnimation)
			.AppendInterval(_totalGoldAnimationDuration)
			.AppendCallback(StartExpAnimation);
		_infoSequence.Play();
	}

	private void StartContinueBtnAnimation()
	{
		_state = States.CoinAnimationFinish;
		_btnContinue.gameObject.SetActive(true);
		_btnContinueAlphaAnimation.PlayForward();
		_btnContinuePositionAnimation.PlayForward();
		_breakBtn.gameObject.SetActive(false);
	}

	private void StartExpAnimation()
	{
		_state = States.CoinAnimation;
		_expBar.Show();
		_expBar.AnimateExp();
	}

	private void StartCoinAnimation()
	{
		if (_rewardCoinList.Count == 0)
		{
			return;
		}
		foreach (RewardCoin rewardCoin in _rewardCoinList)
		{
			rewardCoin.gameObject.SetActive(true);
			rewardCoin.Animate();
		}
	}

	public long getGold()
	{
		return this.totalGold;
	}
	public long getBonus()
	{
		return this.totalBonus;
	}

    private void StartTotalRewardAnimation()
    {
		RewardsWindow e = this;
        this._totalRewardObj.SetActive(true);
        this.totalGold = 0L;
        this.totalBonus = 0L;
        this._totalRewardSequence = DOTween.Sequence();
        TweenSettingsExtensions.AppendInterval(TweenSettingsExtensions.AppendCallback(this._totalRewardSequence, delegate ()
        {
            this._totalRewardAlpha.PlayForward();
        }), this._totalRewardAlpha.duration + this._totalRewardAlpha.delay);
        if (this._coins > 0L)
        {
            TweenSettingsExtensions.AppendCallback(this._totalRewardSequence, delegate ()
            {
                DOTween.To(new DOGetter<long>(() => this.totalGold), delegate (long n)
                {
                    this.totalGold = n;
                }, this._coins, this._totalGoldAnimationDuration);
            });
        }
        if (this._bonus > 0L)
        {
            TweenSettingsExtensions.AppendCallback(this._totalRewardSequence, delegate ()
            {
                DOTween.To(new DOGetter<long>(() => this.totalBonus), delegate (long n)
                {
                    this.totalBonus = n;
                }, this._bonus, this._totalGoldAnimationDuration);
            });
        }
        TweenExtensions.Play<Sequence>(this._totalRewardSequence);
    }

    private void StartItemAnimation()
	{
		_coinPlaceholder.gameObject.SetActive(false);
		_totalRewardObj.SetActive(false);
		HideShadowCurrency();
		_cardAnimation.Animate();
		_breakBtn.gameObject.SetActive(true);
	}

	private void BreakAllAnimation()
	{
		_rewardInAnimation.Break();
		if (_infoSequence != null)
		{
			_infoSequence.Pause();
		}
		_expBar.BreakAnimation();
		_expBar.Show();
		foreach (RewardCoin rewardCoin in _rewardCoinList)
		{
			rewardCoin.gameObject.SetActive(true);
			rewardCoin.BreakAnimate();
		}
		if (_totalRewardSequence != null)
		{
			_totalRewardSequence.Pause();
		}
		totalGold = _coins;
		totalBonus = _bonus;
		_totalRewardObj.SetActive(true);
		_totalRewardAlpha.enabled = false;
		_totalRewardAlpha.value = _totalRewardAlpha.to;
		_btnContinue.gameObject.SetActive(true);
		_btnContinuePositionAnimation.enabled = false;
		_btnContinuePositionAnimation.value = _btnContinuePositionAnimation.to;
		_btnContinueAlphaAnimation.enabled = false;
		_btnContinueAlphaAnimation.value = _btnContinueAlphaAnimation.to;
	}

	private List<IReelItemAnimation> CreateCardsForAnimator(List<RewardItemProvider> itemProvider)
	{
		List<IReelItemAnimation> list = new List<IReelItemAnimation>();
		foreach (RewardItemProvider item in itemProvider)
		{
			GameObject gameObject;
			if (item.originItem is SF3.Items.Booster)
			{
				gameObject = UnityEngine.Object.Instantiate(_boosterpackPrf);
				BoosterpackItem component = gameObject.GetComponent<BoosterpackItem>();
				component.Init(item.originItem);
			}
			else
			{
				gameObject = UnityEngine.Object.Instantiate(_reelItemPrf);
				CardItem component2 = gameObject.GetComponent<CardItem>();
				component2.Init(item.originItem);
			}
			gameObject.transform.parent = _reelItemsPlaceholder;
			gameObject.transform.localScale = Vector3.one;
			gameObject.SetActive(false);
			IReelItemAnimation component3 = gameObject.GetComponent<IReelItemAnimation>();
			if (component3 != null)
			{
				list.Add(component3);
			}
		}
		return list;
	}

	private void CardSelect(GameObject target)
	{
		ReelItem component = target.GetComponent<ReelItem>();
		if (component != null && _state == States.RewardAnimationFinish)
		{
			_cardAnimation.AnimateSelectCard(component);
		}
		else if (_state != States.RewardAnimationFinish)
		{
			OnBreakBtnClick();
		}
	}

	private void ShowShadowCurrency(long value)
	{
		_shadowCurrencyLbl.text = value.ToString();
		_shadowCurrencyLbl.transform.parent.gameObject.SetActive(true);
	}

	private void HideShadowCurrency()
	{
		_shadowCurrencyLbl.transform.parent.gameObject.SetActive(false);
	}

	private void OnBreakBtnClick()
	{
		switch (_state)
		{
		case States.None:
		case States.CoinAnimation:
			BreakAllAnimation();
			_state = States.CoinAnimationFinish;
			break;
		case States.RewardAnimation:
			_cardAnimation.Break();
			_state = States.RewardAnimationFinish;
			break;
		}
		_breakBtn.gameObject.SetActive(false);
	}
}
