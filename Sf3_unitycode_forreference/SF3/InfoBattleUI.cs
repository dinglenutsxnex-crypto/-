using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using DOTweenUtils;
using Nekki.UI;
using SF3.GameModels;
using SF3.UserData;
using SF3_Attributes;
using UnityEngine;
using sf3DTO;

namespace SF3
{
	public class InfoBattleUI : MonoBehaviour
	{
		private const string BATTLE_COOLDOWN_ALIAS = "battle_cooldown";

		private const string CONNECTION_GET_BATTLE = "connection_get_battle";

		[SerializeField]
		private GameObject _stageSpritePrefab;

		[SerializeField]
		private UnityEngine.Color _stageCopmleteColor;

		[SerializeField]
		private UnityEngine.Color _stageNoComleteColor;

		[SerializeField]
		private GameObject _impossibleIcon;

		[SerializeField]
		private Transform _fightsParent;

		[SerializeField]
		private GameObject _fightsLabelObject;

		[SerializeField]
		private UISprite _infoBattleBackground;

		[SerializeField]
		private int _maxInRow;

		[SerializeField]
		private Vector2 _indent;

		private NekkiUILabel _fightsLabel;

		private List<GameObject> _battleStagesSprites;

		[SerializeField]
		private UITexture _image;

		[SerializeField]
		private NekkiUILabel _desctiptionLabel;

		[SerializeField]
		private NekkiUILabel _aliasLabel;

		[SerializeField]
		private GameObject _battleDataObject;

		[SerializeField]
		private NekkiUILabel _battleDataInfo;

		[SerializeField]
		private NekkiUILabel _levelDifficultyLabel;

		[SerializeField]
		private UITable _levelDifficultyTable;

		[SerializeField]
		private UISprite _levelDifficultySprite;

		[SerializeField]
		private UnityEngine.Color[] _difficultyColors = new UnityEngine.Color[5];

		[SerializeField]
		private GameObject _startFightButton;

		[SerializeField]
		private GameObject _rewardsObject;

		private UIWidget _rewardsWidget;

		[SerializeField]
		private UIWidget _stageWidget;

		[SerializeField]
		private UITable _rewardsTableCountable;

		[SerializeField]
		private UITable _rewardsTableItem;

		private Sequence _rewardSequence;

		private Tween _fadeTween;

		private UIWidget _itemWidget;

		private UIWidget _countableWidget;

		[SerializeField]
		private NekkiUILabel _questLabel;

		[SerializeField]
		private GameObject _countableRewardPrf;

		[SerializeField]
		private GameObject _itemRewardPrf;

		[SerializeField]
		private float _animationDuration = 0.5f;

		[SerializeField]
		private float _rewardDelay = 1f;

		[SerializeField]
		private AnimationCurve _inCurve;

		[SerializeField]
		private AnimationCurve _outCurve;

		private FightInfo _selectedFight;

		[SerializeField]
		private UnityEngine.Color _descriptionRulesColor;

		private sf3DTO.BattleType _battleType;

		public int backgroundWidth
		{
			get
			{
				return _infoBattleBackground.width;
			}
		}

		private void Start()
		{
			RepositionTabels();
		}

		public void Initialize()
		{
			_fightsLabel = _fightsLabelObject.GetComponent<NekkiUILabel>();
			_battleStagesSprites = new List<GameObject>();
			for (int i = 0; i < 24; i++)
			{
				GameObject gameObject = Object.Instantiate(_stageSpritePrefab);
				gameObject.transform.parent = _fightsParent;
				gameObject.transform.eulerAngles = Vector3.zero;
				gameObject.transform.localScale = Vector3.one;
				gameObject.SetActive(false);
				_battleStagesSprites.Add(gameObject);
			}
			_startFightButton.GetComponentInChildren<UIButton>().onClick.Add(new EventDelegate(OnFightPressed));
			InitializeRewardAnimation();
		}

		private void InitializeRewardAnimation()
		{
			_rewardsWidget = _rewardsObject.GetComponent<UIWidget>();
			_itemWidget = _rewardsTableItem.GetComponent<UIWidget>();
			_countableWidget = _rewardsTableCountable.GetComponent<UIWidget>();
			_rewardSequence = DOTween.Sequence();
			_rewardSequence.OnStart(delegate
			{
				_itemWidget.alpha = 0f;
			});
			_rewardSequence.AppendInterval(_rewardDelay);
			_rewardSequence.Append(DONgui.Fade(_countableWidget, 1f, 0f, _animationDuration).SetEase(_inCurve));
			_rewardSequence.Join(DONgui.Fade(_itemWidget, 0f, 1f, _animationDuration).SetEase(_outCurve));
			_rewardSequence.AppendInterval(_rewardDelay);
			_rewardSequence.Append(DONgui.Fade(_itemWidget, 1f, 0f, _animationDuration).SetEase(_inCurve));
			_rewardSequence.Join(DONgui.Fade(_countableWidget, 0f, 1f, _animationDuration).SetEase(_outCurve));
			_rewardSequence.SetLoops(-1);
		}

		private void ShowStages(BattleInfo battleValue)
		{
			foreach (GameObject battleStagesSprite in _battleStagesSprites)
			{
				battleStagesSprite.SetActive(false);
			}
			_stageWidget.alpha = 0f;
			DONgui.Fade(_stageWidget, 1f, _animationDuration);
			_fightsLabel.gameObject.SetActive(false);
			int count = battleValue.fights.Count;
			if (count <= 1)
			{
				return;
			}
			float num = Mathf.Ceil((float)count / (float)_maxInRow);
			Vector3 zero = Vector3.zero;
			if (count > 1)
			{
				if (count < _maxInRow)
				{
					zero.x = (0f - _indent.x) * ((float)count / 2f);
				}
				else
				{
					zero.x = (0f - _indent.x) * ((float)_maxInRow / 2f);
				}
				zero.x += _indent.x / 2f;
			}
			if (num > 1f)
			{
				zero.y = (0f - _indent.y) * (num / 2f);
				zero.y += _indent.y / 2f;
			}
			int num2 = 0;
			int num3 = -1;
			for (int i = 0; (float)i < num; i++)
			{
				int num4 = count - num2;
				if (num4 < _maxInRow)
				{
					zero.x = 0f;
					if (num4 > 1)
					{
						if (num4 < _maxInRow)
						{
							zero.x = (0f - _indent.x) * ((float)num4 / 2f);
						}
						zero.x += _indent.x / 2f;
					}
				}
				for (int j = 0; j < _maxInRow; j++)
				{
					UISprite component = _battleStagesSprites[num2].GetComponent<UISprite>();
					if (num2 >= battleValue.wonFights)
					{
						component.color = _stageNoComleteColor;
						if (num3 == -1)
						{
							num3 = num2;
						}
					}
					else
					{
						component.color = _stageCopmleteColor;
					}
					_battleStagesSprites[num2].transform.localPosition = zero + new Vector3((float)j * _indent.x, (float)i * _indent.y, 0f);
					_battleStagesSprites[num2].SetActive(true);
					num2++;
					if (num2 == count)
					{
						_fightsLabel.gameObject.SetActive(true);
						_fightsLabel.text = num3 + "/" + count;
						return;
					}
				}
			}
		}

		private void ShowRewards(FightRewards.RoundReward rewards, List<BaseRewardInfo> rewardItems)
		{
			ClearRewards();
			bool flag = false;
			if (rewards.currencies != null)
			{
				foreach (Currency currency in rewards.currencies)
				{
					CountableReward countableReward = CreateCountableReward();
					countableReward.InitCurrency(currency);
					flag = true;
				}
			}
			if (rewards.experience != 0)
			{
				CountableReward countableReward2 = CreateCountableReward();
				countableReward2.InitExp(rewards.experience);
				flag = true;
			}
			if (rewardItems != null)
			{
				foreach (BaseRewardInfo rewardItem in rewardItems)
				{
					ItemReward itemReward = CreateItemReward();
					itemReward.Init(rewardItem);
					flag = true;
				}
			}
			_rewardsObject.SetActive(flag);
			RepositionTabels();
			if (!flag)
			{
				return;
			}
			_fadeTween.Kill();
			_rewardSequence.Pause();
			_itemWidget.alpha = 0f;
			_countableWidget.alpha = 1f;
			_fadeTween = DONgui.Fade(_rewardsWidget, 0f, 1f, _animationDuration).OnComplete(delegate
			{
				if (rewardItems != null && rewardItems.Count > 0)
				{
					_rewardSequence.Restart();
					_rewardSequence.PlayForward();
				}
			});
		}

		private CountableReward CreateCountableReward()
		{
			GameObject gameObject = NGUITools.AddChild(_rewardsTableCountable.gameObject, _countableRewardPrf);
			return gameObject.GetComponent<CountableReward>();
		}

		private ItemReward CreateItemReward()
		{
			GameObject gameObject = NGUITools.AddChild(_rewardsTableItem.gameObject, _itemRewardPrf);
			return gameObject.GetComponent<ItemReward>();
		}

		private void ClearRewards()
		{
			foreach (Transform item in _rewardsTableItem.transform)
			{
				item.gameObject.SetActive(false);
				Object.Destroy(item.gameObject);
			}
			foreach (Transform item2 in _rewardsTableCountable.transform)
			{
				item2.gameObject.SetActive(false);
				Object.Destroy(item2.gameObject);
			}
		}

		public void LoadBattleInfo(IBattleInfo selectedBattle)
		{
			UserBadgesManager.Instance.Reset(UserBadgesManager.BadgeTypes.Map, (long)selectedBattle.GetID());
			_selectedFight = selectedBattle.GetCurrentFight();
			_battleType = selectedBattle.GetBattleType();
			BattleInfo battleInfo = selectedBattle.GetBattleInfo();
			if (_selectedFight != null && selectedBattle.GetIsAvailable())
			{
				_aliasLabel.Alias = battleInfo.alias;
				_desctiptionLabel.Alias = string.Empty;
				_desctiptionLabel.maxLineCount = 20;
				_desctiptionLabel.height = 200;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(Localization.Get(battleInfo.description));
				foreach (string item in GetFightRulesDescription(_selectedFight))
				{
					stringBuilder.Append("\n[");
					stringBuilder.Append(ColorUtility.ToHtmlStringRGBA(_descriptionRulesColor));
					stringBuilder.Append(']');
					stringBuilder.Append(item);
					stringBuilder.Append("[-]");
				}
				_desctiptionLabel.text = stringBuilder.ToString();
				SetBattleImage(battleInfo.image);
				float difficulty = CalcFightDifficulty(_selectedFight, _battleType);
				UpdateFightDifficulty(difficulty);
				ShowStages(battleInfo);
				ShowRewards(_selectedFight.GetRewardsMax(), _selectedFight.GetRewardInfo());
				_battleDataObject.SetActive(true);
				_battleDataInfo.gameObject.SetActive(false);
			}
			else
			{
				_aliasLabel.Alias = battleInfo.alias;
				_desctiptionLabel.Alias = battleInfo.description;
				SetBattleImage(battleInfo.image);
				_battleDataObject.SetActive(false);
				_battleDataInfo.gameObject.SetActive(true);
				if (selectedBattle.HasExpirationTime() || battleInfo is DailyBattleInfo)
				{
					_battleDataInfo.Alias = "battle_cooldown";
				}
				else
				{
					_battleDataInfo.Alias = "connection_get_battle";
				}
			}
			_questLabel.Alias = battleInfo.battleTypeAlias;
		}

		private List<string> GetFightRulesDescription(FightInfo fight)
		{
			List<string> list = new List<string>();
			foreach (RoundInfo round in fight.rounds)
			{
				foreach (Rule rule in round.rules)
				{
					string attributeByType = rule.GetAttributeByType("Description");
					if (!string.IsNullOrEmpty(attributeByType))
					{
						LocaleImport.LocaleString localeString = Localization.Get(attributeByType);
						if (!list.Contains(localeString))
						{
							list.Add(localeString);
						}
					}
				}
			}
			return list;
		}

		private static float CalcFightDifficulty(FightInfo fight, sf3DTO.BattleType battleType)
		{
			if (battleType == sf3DTO.BattleType.Brawler)
			{
				return -1f;
			}
			if (fight == null)
			{
				Debug.LogError("Cannot calculate difficulty for 'null'");
				return -1f;
			}
			ModelAttributes attributes = fight.GetRound(1).warrior.attributes;
			float summaryAttribute = attributes.GetSummaryAttribute(AttributeType.WeaponDamage);
			float summaryAttribute2 = attributes.GetSummaryAttribute(AttributeType.UnarmedDamage);
			float summaryAttribute3 = attributes.GetSummaryAttribute(AttributeType.BodyDefense);
			float summaryAttribute4 = attributes.GetSummaryAttribute(AttributeType.HeadDefense);
			float summaryAttribute5 = attributes.GetSummaryAttribute(AttributeType.MagicPower);
			ModelAttributes attributes2 = UserManager.UserModelInfo.attributes;
			float summaryAttribute6 = attributes2.GetSummaryAttribute(AttributeType.WeaponDamage);
			float summaryAttribute7 = attributes2.GetSummaryAttribute(AttributeType.UnarmedDamage);
			float summaryAttribute8 = attributes2.GetSummaryAttribute(AttributeType.BodyDefense);
			float summaryAttribute9 = attributes2.GetSummaryAttribute(AttributeType.HeadDefense);
			float summaryAttribute10 = attributes2.GetSummaryAttribute(AttributeType.MagicPower);
			return JsFunction.CalculateFightDifficulty(summaryAttribute6, summaryAttribute7, summaryAttribute10, summaryAttribute8, summaryAttribute9, summaryAttribute, summaryAttribute2, summaryAttribute5, summaryAttribute3, summaryAttribute4);
		}

		private void UpdateFightDifficulty(float difficulty)
		{
			_impossibleIcon.SetActive(false);
			if (difficulty < 0f)
			{
				_levelDifficultyLabel.Alias = "difficulty_unknown";
			}
			else if ((double)difficulty <= 0.82)
			{
				_levelDifficultyLabel.Alias = "difficulty_easy";
				_levelDifficultyLabel.color = _difficultyColors[0];
			}
			else if ((double)difficulty <= 1.3)
			{
				_levelDifficultyLabel.Alias = "difficulty_normal";
				_levelDifficultyLabel.color = _difficultyColors[1];
			}
			else if ((double)difficulty <= 2.6)
			{
				_levelDifficultyLabel.Alias = "difficulty_hard";
				_levelDifficultyLabel.color = _difficultyColors[2];
			}
			else if ((double)difficulty <= 5.2)
			{
				_levelDifficultyLabel.Alias = "difficulty_insane";
				_levelDifficultyLabel.color = _difficultyColors[3];
			}
			else
			{
				_levelDifficultyLabel.Alias = "difficulty_impossible";
				_levelDifficultyLabel.color = _difficultyColors[4];
				_impossibleIcon.SetActive(true);
			}
			_levelDifficultyTable.Reposition();
		}

		private void OnFightPressed()
		{
			sf3DTO.BattleType battleType = _battleType;
			if (battleType == sf3DTO.BattleType.Brawler)
			{
				LoadBrawlerFight();
			}
			else
			{
				LoadFight();
			}
		}

		private void LoadFight()
		{
			if (_selectedFight != null)
			{
				LoadScreen.ShowLoader(delegate
				{
					ModuleController.GoToFight(_selectedFight);
					FoggingController.Instance.ShowFogging();
				});
			}
		}

		private void LoadBrawlerFight()
		{
			LoadingIcon.Instance.EnableLoadingScreen("wait_brawler");
			UIBlocker.Instance.Block(UIBlocker.Priority.Preloader);
			BrawlerHelper.GetNewBrawlerFightAsync(delegate(BrawlerFight fight)
			{
				LoadingIcon.Instance.DisableLoadingScreen(0.5f, null, "wait_brawler");
				UIBlocker.Instance.Unblock();
				if (fight != null)
				{
					_selectedFight.rounds.Clear();
					for (int i = 0; i < _selectedFight.roundsToWin; i++)
					{
						_selectedFight.rounds.Add(RoundInfo.Create(fight.Enemy));
					}
					LoadFight();
				}
			});
		}

		public void ShowBattleInfo(bool show = true)
		{
			base.gameObject.SetActive(show);
		}

		private void SetBattleImage(string img)
		{
			_image.mainTexture = GlobalLoad.GetLoadTexture2D("UI/InfoBattle/Images/" + img);
		}

		public void RepositionTabels()
		{
			_rewardsTableCountable.repositionNow = true;
			_rewardsTableItem.repositionNow = true;
		}
	}
}
