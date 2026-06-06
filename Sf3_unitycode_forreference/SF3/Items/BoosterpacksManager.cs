using System;
using System.Collections.Generic;
using DG.Tweening;
using DOTweenUtils;
using SF3.Settings;
using SF3.UserData;
using UnityEngine;
using sf3DTO;

namespace SF3.Items
{
	public class BoosterpacksManager : UIModuleHolder
	{
		private const float HIDE_MODELS_TIME = 1f;

		private const float BOOSTERPACK_FADE_TIME = 0.5f;

		private const float SLICE_SIMULATION_TIME = 1f;

		private const float SCROLL_FADE_TIME = 0.5f;

		private const float STACKED_CARD_FADEIN_TIME = 0.5f;

		private const float CARD_MOVE_AND_FLIP_TIME = 0.6f;

		private const float SHINING_ADDED_TIME = 0.2f;

		private const float TIME_FOR_FREE_SLICING = 2.5f;

		private const float TIME_BEFORE_SLICE_ENDING = 1f;

		private const float CELL_COLLIDER_WIDTH = 150f;

		private const float CELL_COLLIDER_HEIGHT = 190f;

		private const float CELL_BACKGROUND_HEIGHT = 165f;

		private const float OPEN_CARD_SCALE = 1.4f;

		private const float EXPLODE_FORCE = 5f;

		private const float EXPLODE_RADIUS = 100f;

		private const float CARD_Y_OFFSET = -35f;

		private const int BASE_CARD_DEPTH = 10;

		private const float CARD_MOVE_DURATION = 0.3f;

		private const float CARD_MOVE_DELAY = 0.2f;

		private const float CARD_MOVE_TIME_OFFSET = 0.1f;

		private readonly Vector3 _openedCardPosition = new Vector3(15f, 0f, 0f);

		private readonly Vector3 _selectedBoosterpackPosition = new Vector3(0f, 0f, 0f);

		private readonly Vector3 _firstCardPosition = new Vector3(-410f, -35f, 0f);

		private readonly Vector3 _stackCardPosition = new Vector3(115f, -100f, 400f);

		private readonly Vector3 _slicingStartPoint = new Vector3(-0.5f, -0.3f, 0f);

		private readonly Vector3 _slicingEndPoint = new Vector3(0.5f, 0.3f, 0f);

		[SerializeField]
		private float _cutForceMultiplier;

		[SerializeField]
		private float _slowingFactor;

		[SerializeField]
		private float _swipeLengthMultiplier;

		[SerializeField]
		private GameObject _slashPrf;

		[SerializeField]
		private UIScrollViewCustom _boosterpacksScroll;

		[SerializeField]
		private BoosterpackScrollItem _boosterpackScrollItem;

		[SerializeField]
		private Boosterpack _boosterpack;

		[SerializeField]
		private Transform _selectedBoosterpackAnchor;

		[SerializeField]
		private Transform _scrollAnchor;

		[SerializeField]
		private UISprite _scrollBG;

		[SerializeField]
		private SpriteSlicerSFManager _spriteSlicer;

		[SerializeField]
		private CardItem _reelItem;

		[SerializeField]
		private Transform _reelItems;

		[SerializeField]
		private RewardInfo _rewardInfo;

		[SerializeField]
		private UIButton _nextCardButton;

		[SerializeField]
		private UIPanel _unblockableInputPanel;

		[SerializeField]
		private GameObject _reelItemPlaceholder;

		[SerializeField]
		private GameObject _commonStackFx;

		[SerializeField]
		private GameObject _rareStackFx;

		[SerializeField]
		private GameObject _epicStackFx;

		[SerializeField]
		private GameObject _legendaryStackFx;

		private UIPanel _scrollerPanel;

		private CardItem _selectedCard;

		private BoosterpackItemProvider _selectedBoosterItem;

		private Boosterpack _selectedBoosterpack;

		private Booster _openedBooster;

		private CardItem _underlayedCard;

		private bool _isSpawnPlaying;

		private bool _scrollDragEnabled = true;

		private bool _firstSliceDone;

		private static bool _cardsCanBeOpen;

		private BoosterpackScrollItem _nextBoosterpack;

		private readonly List<CardItem> _cards = new List<CardItem>();

		private readonly List<BoosterpackScrollItem> _scrollItems = new List<BoosterpackScrollItem>();

		private BoosterIntentModule _intent;

		private Action _callbackOnOpen;

		private Action _callbackOnClosed;

		private List<BoosterpackItemProvider> _boosterItems;

		private EquipmentSaver _equipedItem = new EquipmentSaver();

		public static BoosterpacksManager Instance { get; private set; }

		public override void ShowModule(IntentModule intent, Action callbackOnOpen)
		{
			_intent = (BoosterIntentModule)intent;
			_callbackOnOpen = callbackOnOpen;
			if (_intent.IsInstant())
			{
				BattleCamera.Instance.changeClipPlane = true;
				BattleCamera.Instance.toClipPlane = 10f;
			}
			else
			{
				BattleCamera.Instance.MainCamera.nearClipPlane = 10f;
			}
			HolderModule.EnableControls(false);
			BattleCamera.Move(BattleCamera.Instance.boosterpackCameraPosition, null, _intent.IsInstant(), true);
			LocationColorAnimation.Instance.Animate(GameSettings.DojoSettings.LocationColorChangeTime, GameSettings.DojoSettings.DefaultLocationColor, GameSettings.DojoSettings.LocationColorInModule);
			LocationColorAnimation.Instance.EnableRays(false, GameSettings.DojoSettings.LocationColorChangeTime);
			ModelsManager.Instance.HideModels(1f);
			base.gameObject.SetActive(true);
			InitBoosterpacks();
			SelectNextBoosterpack(_selectedBoosterpackPosition);
			InitScroller();
			DONgui.Fade(_scrollerPanel, 1f, 0.5f);
			DONgui.Fade(_scrollBG, 1f, 0.5f);
			UpdateModule(intent);
			_nextCardButton.gameObject.SetActive(false);
		}

		public override void UpdateModule(IntentModule intent)
		{
			_intent = (BoosterIntentModule)intent;
			_callbackOnOpen.InvokeSafe();
		}

		public override void HideModule(Action callbackOnClosed)
		{
			_callbackOnClosed = callbackOnClosed;
			HolderModule.EnableControls(true);
			LocationColorAnimation.Instance.Animate(GameSettings.DojoSettings.LocationColorChangeTime, GameSettings.DojoSettings.LocationColorInModule, GameSettings.DojoSettings.DefaultLocationColor);
			LocationColorAnimation.Instance.EnableRays(true, GameSettings.DojoSettings.LocationColorChangeTime);
			ClearBoosterpacks();
			ClearCards();
			ResetFlags();
			base.gameObject.SetActive(false);
			_callbackOnClosed.InvokeSafe();
		}

		public override void Initialize()
		{
			Instance = this;
			SubtypeButton.Reset();
			base.gameObject.SetActive(false);
			InitSelectedBoosterpack();
			_rewardInfo.Init();
			_scrollerPanel = _boosterpacksScroll.gameObject.GetComponent<UIPanel>();
			InitSpriteSlicer();
			_nextCardButton.onClick.Add(new EventDelegate(OpenCard));
		}

		private void InitSelectedBoosterpack()
		{
			_selectedBoosterpack = UnityEngine.Object.Instantiate(_boosterpack);
			_selectedBoosterpack.transform.parent = _selectedBoosterpackAnchor;
			_selectedBoosterpack.transform.localScale = Vector3.one;
			_selectedBoosterpack.transform.localPosition = _selectedBoosterpackPosition;
			_selectedBoosterpack.gameObject.SetActive(false);
			_spriteSlicer.slashParent = _selectedBoosterpack.gameObject;
		}

		private void InitSpriteSlicer()
		{
			_spriteSlicer.cutForceMultiplier = _cutForceMultiplier;
			_spriteSlicer.slowingFactor = _slowingFactor;
			_spriteSlicer.swipeLengthMultiplier = _swipeLengthMultiplier;
			_spriteSlicer.slashPrf = _slashPrf;
			_spriteSlicer.slashParent = _selectedBoosterpack.gameObject;
			_spriteSlicer.onSliced = OnBoosterpackSliced;
		}

		private void InitBoosterpacks()
		{
			Dictionary<int, List<Booster>> boostersAll = UserManager.UserModelInfo.GetBoostersAll();
			foreach (KeyValuePair<int, List<Booster>> item in boostersAll)
			{
				InitBoosterpack(item.Key, item.Value.Count);
			}
		}

		private void InitBoosterpack(int modelID, int count)
		{
			if (count > 0)
			{
				BoosterpackScrollItem boosterpack = UnityEngine.Object.Instantiate(_boosterpackScrollItem);
				EventDelegate onClick = new EventDelegate(delegate
				{
					OnBoosterpackSelected(boosterpack);
				});
				boosterpack.Init(modelID, count, onClick);
				boosterpack.transform.parent = _boosterpacksScroll.transform;
				boosterpack.transform.localPosition = Vector3.zero;
				boosterpack.transform.localScale = Vector3.one;
				boosterpack.DragScrollView.scrollView = _boosterpacksScroll;
				_scrollItems.Add(boosterpack);
			}
		}

		private void InitScroller()
		{
			_boosterpacksScroll.cellSize = new Vector2(150f, 190f);
			RefreshScroller();
		}

		private void RefreshScroller()
		{
			Transform transform = _boosterpacksScroll.transform;
			int num = 0;
			int i = 0;
			for (int childCount = transform.childCount; i < childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child.gameObject.activeSelf)
				{
					num++;
					child.localPosition = new Vector3(0f, 190f - (float)num * 190f, 0f);
				}
			}
			_boosterpacksScroll.RecalculateBounds();
			bool flag = num > 3;
			if (_scrollDragEnabled != flag)
			{
				_scrollDragEnabled = flag;
				if (!_scrollDragEnabled)
				{
					_boosterpacksScroll.ResetPosition();
				}
				_boosterpacksScroll.enabled = _scrollDragEnabled;
			}
		}

		private void MoveScrollToMakeVisible(BoosterpackScrollItem scrollItem)
		{
			switch (_boosterpacksScroll.GetVisibilityForCell(scrollItem.transform, new Vector2(150f, 165f)))
			{
			case UIScrollViewCustom.CellVisibility.TOP_PART:
				_boosterpacksScroll.ScrollDownByCell();
				break;
			case UIScrollViewCustom.CellVisibility.BOTTOM_PART:
				_boosterpacksScroll.ScrollUpByCell();
				break;
			}
		}

		private void EnableSlicer(bool enabled)
		{
			if (!enabled)
			{
				_spriteSlicer.ClearPositions();
			}
			_spriteSlicer.gameObject.SetActive(enabled);
		}

		private void SelectNextBoosterpack(Vector3 moveToPosition)
		{
			if (_scrollItems.Count != 0)
			{
				if (_nextBoosterpack != null)
				{
					SpawnBoosterpack(_nextBoosterpack, moveToPosition);
				}
				else
				{
					SpawnBoosterpack(_scrollItems[0], moveToPosition);
				}
			}
		}

		private void OnBoosterpackSelected(BoosterpackScrollItem scrollItem)
		{
			if (_isSpawnPlaying)
			{
				return;
			}
			MoveScrollToMakeVisible(scrollItem);
			if (!(_selectedBoosterpack.ScrollItem == scrollItem))
			{
				_isSpawnPlaying = true;
				Sequence sequence = DOTween.Sequence();
				sequence.Append(DONgui.Fade(_selectedBoosterpack.SlicableTexture, 0f, 0.5f));
				sequence.AppendCallback(delegate
				{
					SpawnBoosterpack(scrollItem, _selectedBoosterpackPosition);
					RefreshScroller();
				});
				sequence.Append(DONgui.Fade(_selectedBoosterpack.SlicableTexture, 1f, 0.5f));
				sequence.OnComplete(delegate
				{
					_isSpawnPlaying = false;
				});
				sequence.Play();
			}
		}

		private void SpawnBoosterpack(BoosterpackScrollItem scrollItem, Vector3 moveToPosition)
		{
			_selectedBoosterpack.Init(scrollItem, SimulateSlicing);
			_selectedBoosterpack.transform.parent = _selectedBoosterpackAnchor;
			_selectedBoosterpack.transform.localScale = Vector3.one;
			_selectedBoosterpack.gameObject.SetActive(true);
			_selectedBoosterpack.transform.localPosition = moveToPosition;
		}

		private void SimulateSlicing()
		{
			if (!_firstSliceDone && !_isSpawnPlaying)
			{
				_spriteSlicer.SimulateSlicing(_slicingStartPoint, _slicingEndPoint);
			}
		}

		private void OnBoosterpackSliced()
		{
			if (!_firstSliceDone)
			{
				_firstSliceDone = true;
				EnableScreenBlock(true);
				_nextCardButton.gameObject.SetActive(true);
				DONgui.Fade(_scrollerPanel, 0f, 0.5f);
				DONgui.Fade(_scrollBG, 0f, 0.5f);
				OnBoosterpackUsed();
				_spriteSlicer.EnableTrailParticles(false);
				Sequence sequence = DOTween.Sequence();
				sequence.AppendInterval(2.5f);
				sequence.AppendCallback(delegate
				{
					_spriteSlicer.Explode(_selectedBoosterpackAnchor.position, 100f, 5f);
					_selectedBoosterpack.Explode();
					EnableSlicer(false);
				});
				sequence.Append(_spriteSlicer.SharedMaterial.DOFade(0f, 1f));
				sequence.OnComplete(delegate
				{
					RecreateSelectedBoosterpack();
					CreateCards();
				});
				sequence.Play();
			}
		}

		private void OnBoosterpackUsed()
		{
			_openedBooster = UserManager.UserModelInfo.GetBoosterToSpend(_selectedBoosterpack.ModelID);
			_equipedItem.SaveEquipedItems();
			_boosterItems = BoosterpackItemProvider.CreateFromList(_openedBooster.AllItems);
			UserManager.GiveRewards(_openedBooster);
			SpendBoosterpack();
		}

		private void SpendBoosterpack()
		{
			if (_selectedBoosterpack.ScrollItem.WillBeDeletedAfterSpend())
			{
				_scrollItems.Remove(_selectedBoosterpack.ScrollItem);
				_nextBoosterpack = null;
			}
			else
			{
				_nextBoosterpack = _selectedBoosterpack.ScrollItem;
			}
			_selectedBoosterpack.ScrollItem.SpendBoosterpack();
			UserManager.UserModelInfo.SpendBooster(_openedBooster);
			UserManager.RemoveBoosterFromYAML(_openedBooster);
			UserBadgesManager.Instance.Reset(UserBadgesManager.BadgeTypes.Boosters, _openedBooster);
			OpenBoosterRequest openBoosterRequest = new OpenBoosterRequest();
			openBoosterRequest.InstanceId = _openedBooster.instance_id;
			OpenBoosterRequest request = openBoosterRequest;
			UserDataController.Send_OpenBooster(request);
		}

		private void RecreateSelectedBoosterpack()
		{
			_selectedBoosterpack.gameObject.SetActive(false);
			UnityEngine.Object.Destroy(_selectedBoosterpack.gameObject);
			_selectedBoosterpack = UnityEngine.Object.Instantiate(_boosterpack);
			_selectedBoosterpack.transform.parent = _selectedBoosterpackAnchor;
			_selectedBoosterpack.transform.localScale = Vector3.one;
			_selectedBoosterpack.transform.localPosition = _selectedBoosterpackPosition;
			_selectedBoosterpack.gameObject.SetActive(false);
			_spriteSlicer.slashParent = _selectedBoosterpack.gameObject;
		}

		private void OnOpenCardsSequenceEnded()
		{
			_firstSliceDone = false;
			_isSpawnPlaying = true;
			_cardsCanBeOpen = false;
			EnableSlicer(true);
			Sequence sequence = DOTween.Sequence();
			sequence.AppendCallback(delegate
			{
				SelectNextBoosterpack(_selectedBoosterpackPosition);
				RefreshScroller();
				DONgui.Fade(_scrollerPanel, 1f, 0.5f);
				DONgui.Fade(_scrollBG, 1f, 0.5f);
			});
			Transform target = _selectedBoosterpack.transform;
			Vector3 selectedBoosterpackPosition = _selectedBoosterpackPosition;
			sequence.Append(target.DOLocalMoveX(selectedBoosterpackPosition.x, 0.5f));
			sequence.OnComplete(delegate
			{
				_isSpawnPlaying = false;
				EnableScreenBlock(false);
				_nextCardButton.gameObject.SetActive(false);
			});
			sequence.Play();
		}

		private void CreateCards()
		{
			_cards.Clear();
			_cardsCanBeOpen = false;
			List<BaseItem> allItems = _openedBooster.AllItems;
			List<CardItem> list = new List<CardItem>();
			for (int i = 0; i < allItems.Count; i++)
			{
				CardItem cardItem = CreateCard(allItems[i], _openedCardPosition);
				cardItem.UpdateDepth(i + 10);
				list.Add(cardItem);
			}
			Sequence t = DOTween.Sequence();
			for (int j = 0; j < list.Count; j++)
			{
				Vector3 firstCardPosition = _firstCardPosition;
				float x = firstCardPosition.x + -35f * (float)(list.Count - j);
				Vector3 firstCardPosition2 = _firstCardPosition;
				float y = firstCardPosition2.y;
				Vector3 firstCardPosition3 = _firstCardPosition;
				Vector3 endValue = new Vector3(x, y, firstCardPosition3.z);
				float interval = 0.2f + 0.1f * (float)j;
				CardItem cardItem2 = list[j];
				t = DOTween.Sequence().AppendInterval(interval).Append(cardItem2.transform.DOLocalMove(endValue, 0.7f).SetEase(Ease.InQuad))
					.Join(cardItem2.transform.DOScale(Vector3.one, 0.7f).SetEase(Ease.InQuad));
				t.Play();
			}
			t.OnComplete(delegate
			{
				_cardsCanBeOpen = true;
			});
		}

		private CardItem CreateCard(BaseItem item, Vector3 position)
		{
			CardItem cardItem = UnityEngine.Object.Instantiate(_reelItem);
			Transform transform = cardItem.gameObject.transform;
			transform.parent = _reelItems;
			transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
			transform.localPosition = position;
			transform.localRotation = Quaternion.identity;
			cardItem.gameObject.SetActive(false);
			cardItem.Init(item);
			cardItem.ActivateButton(false);
			cardItem.ActivateCollider(false);
			cardItem.SetBackClothDepth(100);
			cardItem.ShowBackCloth();
			cardItem.gameObject.SetActive(true);
			_cards.Add(cardItem);
			UIWidget widget = cardItem.gameObject.GetComponent<UIWidget>();
			Sequence sequence = DOTween.Sequence();
			sequence.AppendCallback(delegate
			{
				widget.alpha = 0f;
			});
			sequence.Append(DONgui.Fade(widget, 1f, 0.5f));
			sequence.AppendCallback(delegate
			{
				widget.alpha = 1f;
			});
			sequence.Play();
			return cardItem;
		}

		private ParticleSystem CreateStackParticle(CardItem item)
		{
			IRarable rarable = item.Item as IRarable;
			if (rarable == null)
			{
				Debug.LogError(string.Format("item {0} is not rarable", item.gameObject.name));
				return null;
			}
			GameObject gameObject = null;
			switch (rarable.GetRarityType())
			{
			case Rarity.Common:
				gameObject = NGUITools.AddChild(item.gameObject, _commonStackFx);
				break;
			case Rarity.Rare:
				gameObject = NGUITools.AddChild(item.gameObject, _rareStackFx);
				break;
			case Rarity.Epic:
				gameObject = NGUITools.AddChild(item.gameObject, _epicStackFx);
				break;
			case Rarity.Legendary:
				gameObject = NGUITools.AddChild(item.gameObject, _legendaryStackFx);
				break;
			}
			if (gameObject == null)
			{
				Debug.LogError(string.Format("item {0} has no fx", item.gameObject.name));
				return null;
			}
			gameObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
			gameObject.transform.localScale = new Vector3(53f, 53f, 53f);
			return gameObject.GetComponent<ParticleSystem>();
		}

		private void OpenCard()
		{
			if (!_cardsCanBeOpen)
			{
				return;
			}
			_rewardInfo.HideAll();
			if (_selectedCard != null)
			{
				_selectedCard.gameObject.SetActive(false);
				UnityEngine.Object.Destroy(_selectedCard.gameObject);
				_selectedCard = null;
			}
			if (_cards.Count == 0)
			{
				OnOpenCardsSequenceEnded();
				return;
			}
			_cardsCanBeOpen = false;
			_selectedCard = _cards[_cards.Count - 1];
			_selectedBoosterItem = _boosterItems[_cards.Count - 1];
			_cards.Remove(_selectedCard);
			_rewardInfo.transform.localPosition = _openedCardPosition;
			_selectedCard.gameObject.SetActive(true);
			Sequence sequence = DOTween.Sequence();
			if (_selectedBoosterItem.ownedItem != null)
			{
				AppendSelectedCardAnimation(sequence);
				JoinUnderlayedCardAnimation(sequence);
				AppendStackAnimation(sequence);
			}
			else
			{
				AppendSelectedCardAnimation(sequence);
			}
			sequence.AppendCallback(delegate
			{
				if (_selectedBoosterItem.ownedItem == null)
				{
					ShowSelectedCardInfo();
				}
				_cardsCanBeOpen = true;
			});
			sequence.Play();
		}

		private void AppendSelectedCardAnimation(Sequence parent)
		{
			parent.Append(_selectedCard.GetFlipSequence(0.6f, CardItem.FlipPivot.LEFT));
			Transform target = _selectedCard.transform;
			Vector3 openedCardPosition = _openedCardPosition;
			parent.Join(target.DOLocalMoveX(openedCardPosition.x, 0.6f).SetEase(Ease.InOutCubic));
			Transform target2 = _selectedCard.transform;
			Vector3 openedCardPosition2 = _openedCardPosition;
			parent.Join(target2.DOLocalMoveY(openedCardPosition2.y, 0.6f).SetEase(Ease.InOutCubic));
			parent.Join(_selectedCard.transform.DOScale(1.4f, 0.3f));
		}

		private void JoinUnderlayedCardAnimation(Sequence parent)
		{
			_underlayedCard = DuplicateCard(_selectedCard);
			UIWidget component = _underlayedCard.GetComponent<UIWidget>();
			parent.Join(_underlayedCard.transform.DOLocalRotateQuaternion(Quaternion.Euler(new Vector3(0f, 0f, 0f)), 0.5f).SetEase(Ease.InQuad));
			parent.Join(_underlayedCard.transform.DOLocalMove(_openedCardPosition, 0.5f).SetEase(Ease.InQuad));
			parent.Join(DONgui.Fade(component, 0f, 1f, 0.5f));
		}

		private void AppendStackAnimation(Sequence parent)
		{
			ParticleSystem stackParticles = CreateStackParticle(_selectedCard);
			_rewardInfo.SetProgress(_selectedBoosterItem.ownedItem);
			parent.AppendCallback(delegate
			{
				UnityEngine.Object.Destroy(_underlayedCard.gameObject);
				_rewardInfo.ShowAll(_selectedBoosterItem.ownedItem, _equipedItem.GetEquipedAnalogByType(_selectedBoosterItem.ownedItem));
				stackParticles.Play();
			});
			parent.AppendInterval(stackParticles.main.duration);
			parent.Join(DONgui.GetProgressBarSequence(_rewardInfo.progressBar, _selectedBoosterItem.levelUpBar, _selectedBoosterItem.levelups, stackParticles.main.duration * 1.3f, delegate
			{
				_rewardInfo.ShowAttributes(_selectedBoosterItem.itemStacked, _equipedItem.GetEquipedAnalogByType(_selectedBoosterItem.ownedItem));
				_rewardInfo.ShowDescription(_selectedBoosterItem.itemStacked);
			}));
			parent.AppendCallback(delegate
			{
				UnityEngine.Object.Destroy(stackParticles.gameObject);
			});
		}

		private void ShowSelectedCardInfo()
		{
			if (_selectedCard.IsPerk)
			{
				_rewardInfo.ShowAll(_selectedCard.Item);
				return;
			}
			Equipment equipped = UserManager.UserModelInfo.GetEquipped((_selectedCard.Item as Equipment).GetEquipmentType());
			_rewardInfo.ShowAll(_selectedCard.Item, equipped);
		}

		private CardItem DuplicateCard(CardItem card)
		{
			CardItem cardItem = UnityEngine.Object.Instantiate(card);
			cardItem.transform.parent = card.transform.parent.transform;
			cardItem.transform.localPosition = _stackCardPosition;
			cardItem.transform.localScale = Vector3.one * 1.4f;
			cardItem.UpdateDepth(1);
			cardItem.FlipHorizontally();
			cardItem.transform.localRotation = CardAnimator.RotationStackedItem;
			return cardItem;
		}

		private void ClearBoosterpacks()
		{
			List<GameObject> list = new List<GameObject>();
			foreach (Transform item in _boosterpacksScroll.transform)
			{
				list.Add(item.gameObject);
			}
			list.ForEach(delegate(GameObject child)
			{
				UnityEngine.Object.Destroy(child);
			});
			_scrollItems.Clear();
		}

		private void ClearCards()
		{
			foreach (CardItem card in _cards)
			{
				UnityEngine.Object.Destroy(card.gameObject);
			}
			if (_selectedCard != null)
			{
				UnityEngine.Object.Destroy(_selectedCard.gameObject);
			}
			_cards.Clear();
		}

		private void ResetFlags()
		{
			_isSpawnPlaying = false;
			_scrollDragEnabled = true;
			_firstSliceDone = false;
			_cardsCanBeOpen = false;
		}

		private void EnableScreenBlock(bool enabled)
		{
			if (enabled)
			{
				UIBlocker.Instance.Block();
				UIBlocker.Instance.AddIgnoreObject(_unblockableInputPanel);
			}
			else
			{
				UIBlocker.Instance.Unblock();
			}
		}
	}
}
