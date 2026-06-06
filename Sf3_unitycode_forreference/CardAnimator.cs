using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DOTweenUtils;
using Nekki.UI;
using SF3.Audio;
using SF3.Items;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using sf3DTO;

public class CardAnimator : MonoBehaviour, ICardAnimation
{
	private const string IN_ANIMATION_NAME = "in";

	private const string SWIPE_ANIMATION_NAME = "swipe";

	private const string STACK_ANIMATION_NAME = "stack";

	private const string SELECT_ANIMATION_NAME = "select";

	private const string DROP_SOUND_PREFIX = "ui/rewards/sound_drop_";

	private const string STACK_SOUND = "ui/rewards/sound_stack";

	public static readonly Quaternion RotationStackedItem = Quaternion.Euler(new Vector3(-45f, 0f, 0f));

	public static readonly Quaternion RotationNewItem = Quaternion.Euler(new Vector3(23f, -24f, 0f));

	[SerializeField]
	private GameObject _reelItemPrf;

	[SerializeField]
	private Transform _reelItemPlecholder;

	[SerializeField]
	private Camera _reelCamera;

	[SerializeField]
	private float _offset;

	[SerializeField]
	private float _cardXStartOffset;

	[SerializeField]
	private float _cardYStartPosition;

	[SerializeField]
	private float _cardZStartPosition;

	[SerializeField]
	private float _cardZStartStackPosition;

	[SerializeField]
	private float _cardZEndPosition;

	[SerializeField]
	private float _cardScale;

	[SerializeField]
	private float _showInfoTime;

	[SerializeField]
	private float _cardFadeDuration;

	[SerializeField]
	private float _rewardsShowingDuration;

	[SerializeField]
	private NekkiUILabel _itemName;

	[SerializeField]
	private float _stackStartAngle;

	[SerializeField]
	private float _stackDuration;

	[SerializeField]
	private GameObject _commonStackFx;

	[SerializeField]
	private GameObject _rareStackFx;

	[SerializeField]
	private GameObject _epicStackFx;

	[SerializeField]
	private GameObject _legendaryStackFx;

	private RewardInfo _rewardInfo;

	private List<RewardItemProvider> _items;

	private List<IReelItemAnimation> _reelItems;

	private int _currentCard;

	private Vector2 _cardSize;

	private bool _isPlay;

	private int _finishedCardAnimation;

	private UIProgressBar _progressBar;

	private IReelItemAnimation _stackCard;

	private ParticleSystem _stackParticle;

	private MotionBlur _motionBlur;

	private bool _useMotionBlur;

	private int _selectedId = -1;

	private int _selectAnimationCounter;

	private float _cardYOffset = 23f;

	private Sequence _stackAnimationSequence;

	private RewardDataProvider _dataProvider;

	private int _repeatProgressStackAnimation;

	public event CardAnimationEnd onAnimationEnd;

	public void Animate()
	{
		_isPlay = true;
		StartCardInAnimation();
	}

	public void Break()
	{
		_isPlay = false;
		StopAllCoroutines();
		_stackAnimationSequence.Kill(true);
		for (int i = 0; i < _reelItems.Count; i++)
		{
			IReelItemAnimation reelItemAnimation = _reelItems[i];
			reelItemAnimation.Stop();
			reelItemAnimation.item.gameObject.transform.localPosition = CalcEndPosition(i, _reelItems.Count - 1, _reelItems.Count);
			reelItemAnimation.item.gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
			reelItemAnimation.item.gameObject.transform.localScale = Vector3.one;
			reelItemAnimation.item.gameObject.SetActive(true);
			ShowRewardStatus(i);
		}
		if (_stackCard != null)
		{
			_stackCard.Stop();
			_stackCard.item.gameObject.SetActive(false);
		}
		if (_stackParticle != null && (bool)_stackParticle.gameObject)
		{
			UnityEngine.Object.Destroy(_stackParticle.gameObject);
		}
		_currentCard = _items.Count;
		HideInfo();
		AnimationEnd();
	}

	public void AnimateSelectCard(ReelItem item)
	{
		_isPlay = true;
		int num = FindAnimationID(item);
		if (_selectedId == num)
		{
			return;
		}
		_selectedId = num;
		if (_selectedId == -1)
		{
			Debug.LogError("Item not found");
			return;
		}
		_selectAnimationCounter = _reelItems.Count;
		for (int i = 0; i < _reelItems.Count; i++)
		{
			Transform transform = _reelItems[i].item.transform;
			if (i == _selectedId)
			{
				_reelItems[i].Animate("select", CalcEndPosition(i, _selectedId, _items.Count), transform.localRotation.eulerAngles, new Vector3(_cardScale, _cardScale));
				continue;
			}
			_reelItems[i].Animate("select", CalcEndPosition(i, _selectedId, _items.Count), transform.localRotation.eulerAngles, Vector3.one);
			ShowRewardStatus(i);
		}
		HideInfo();
	}

	public void Init(RewardDataProvider rewardDataProvider, List<IReelItemAnimation> reelItemAnimations, RewardInfo info)
	{
		_rewardInfo = info;
		_items = rewardDataProvider.rewardItemProvider;
		_dataProvider = rewardDataProvider;
		_reelItems = new List<IReelItemAnimation>();
		_currentCard = 0;
		_cardSize = _reelItemPrf.GetComponent<CardItem>().LocalSize();
		_progressBar = _rewardInfo.progressBar;
		_reelItems = reelItemAnimations;
		InitCards();
		_motionBlur = _reelCamera.GetComponent<MotionBlur>();
		if (_motionBlur == null)
		{
			Debug.LogError("Not found MoutionBlur");
		}
		else
		{
			_motionBlur.enabled = false;
		}
		string text = SystemInfo.deviceModel.ToLower();
		_useMotionBlur = !text.Contains("iphone8") && !text.Contains("iphone9");
	}

	private void InitCards()
	{
		for (int i = 0; i < _reelItems.Count; i++)
		{
			ReelItem item = _reelItems[i].item;
			item.transform.localPosition = CalcStartPosition(i, i + 1);
			item.transform.localRotation = CalcStartRotation(i);
			item.transform.localScale = new Vector3(_cardScale, _cardScale);
			_reelItems[i].onAnimationEnd += OnCardAnimationEnd;
		}
	}

	private IReelItemAnimation CreateReelItem(BaseItem item, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(_reelItemPrf);
		gameObject.transform.parent = _reelItemPlecholder;
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localPosition = position;
		gameObject.transform.localRotation = rotation;
		ReelItem component = gameObject.GetComponent<ReelItem>();
		component.Init(item);
		gameObject.SetActive(false);
		return gameObject.GetComponent<IReelItemAnimation>();
	}

	private Vector3 CalcBaseCardPosition(int n, int count)
	{
		if (count == 1)
		{
			return new Vector3(0f, 0f, 0f);
		}
		float num = 1f / (float)(count - 1);
		float num2 = (_cardSize.x + _offset) * ((float)count - 1f);
		float x = 0f - num2 / 2f + (float)n * num * num2;
		return new Vector3(x, 0f, 0f);
	}

	private Vector3 CalcStartPosition(int n, int count)
	{
		float x = CalcBaseCardPosition(n, count).x + _cardXStartOffset;
		float y = ((_items[n].itemInInventory == null) ? _cardYStartPosition : 0f);
		float z = ((_items[n].itemInInventory == null) ? _cardZStartPosition : _cardZStartStackPosition);
		return new Vector3(x, y, z);
	}

	private Quaternion CalcStartRotation(int n)
	{
		if (_items[n].itemInInventory != null)
		{
			return RotationStackedItem;
		}
		return RotationNewItem;
	}

	private Vector3 CalcEndPosition(int cardId, int selectedCardId, int cardsCount)
	{
		Vector3 result = CalcBaseCardPosition(cardId, cardsCount);
		result.z = _cardZEndPosition;
		if (cardId == selectedCardId)
		{
			result.y += _cardYOffset;
			return result;
		}
		float num = CalcScaleHorizontalOffset(cardId, _cardScale);
		if (cardId < selectedCardId)
		{
			result.x -= num;
		}
		if (cardId > selectedCardId)
		{
			result.x += num;
		}
		return result;
	}

	private float CalcScaleHorizontalOffset(int n, float scale)
	{
		if (_currentCard == 0)
		{
			return 0f;
		}
		ReelItem item = _reelItems[n].item;
		float num = _cardSize.x * scale;
		return (num - _cardSize.x) / 2f;
	}

	private Sequence GetStackSequence()
	{
		ReelItem card = DublicateCurrentCardAsStack().item;
		_stackParticle = CreateStackParticle();
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(delegate
		{
			_motionBlur.enabled = true && _useMotionBlur;
			card.UpdateDepth(9999);
		});
		sequence.Append(card.transform.DOLocalMove(CalcEndPosition(_currentCard, _currentCard, _currentCard + 1), _stackDuration).SetEase(Ease.InQuad));
		sequence.Join(card.transform.DOLocalRotate(Vector3.zero, _stackDuration).SetEase(Ease.InQuad));
		sequence.AppendCallback(delegate
		{
			card.gameObject.SetActive(false);
			HideRewardStatus(_currentCard);
		});
		BaseItem startStack = GetStartStackItem();
		RewardItemProvider.StackedItem currentStackedItem = _items[_currentCard].GetCurrentStackedItem();
		_rewardInfo.SetProgress(startStack);
		sequence.AppendCallback(delegate
		{
			_rewardInfo.ShowProgress(startStack);
			_rewardInfo.ShowAttributes(startStack, _dataProvider.equipedItems.GetEquipedAnalogByType(startStack));
			_rewardInfo.ShowDescription(startStack);
			AudioManager.Instance.PlaySound("ui/rewards/sound_stack");
			_stackParticle.transform.localPosition = _reelItems[_currentCard].item.transform.localPosition;
			_stackParticle.Play();
		});
		sequence.AppendInterval(_stackParticle.main.duration);
		sequence.Join(DONgui.GetProgressBarSequence(_progressBar, currentStackedItem.levelUpBar, currentStackedItem.levelups, _stackParticle.main.duration * 1.3f, UpdateStackedStats));
		sequence.AppendInterval(_rewardsShowingDuration);
		sequence.AppendCallback(delegate
		{
			_motionBlur.enabled = false;
			GlobalLoad.Unload(_stackParticle.gameObject);
		});
		return sequence;
	}

	private BaseItem GetStartStackItem()
	{
		RewardItemProvider.StackedItem prvStackedItem = _items[_currentCard].GetPrvStackedItem();
		if (prvStackedItem == null)
		{
			return (_items[_currentCard].itemInInventory == null) ? _items[_currentCard].originItem : _items[_currentCard].itemInInventory;
		}
		return prvStackedItem.itemStacked;
	}

	private ParticleSystem CreateStackParticle()
	{
		IRarable rarable = _items[_currentCard].originItem as IRarable;
		if (rarable == null)
		{
			Debug.LogError(string.Format("item {0} is not rarable", _items[_currentCard].originItem.id));
			return null;
		}
		GameObject gameObject = null;
		switch (rarable.GetRarityType())
		{
		case Rarity.Common:
			gameObject = NGUITools.AddChild(_reelItemPlecholder.gameObject, _commonStackFx);
			break;
		case Rarity.Rare:
			gameObject = NGUITools.AddChild(_reelItemPlecholder.gameObject, _rareStackFx);
			break;
		case Rarity.Epic:
			gameObject = NGUITools.AddChild(_reelItemPlecholder.gameObject, _epicStackFx);
			break;
		case Rarity.Legendary:
			gameObject = NGUITools.AddChild(_reelItemPlecholder.gameObject, _legendaryStackFx);
			break;
		}
		if (gameObject == null)
		{
			Debug.LogError(string.Format("item {0} has no fx", _items[_currentCard].originItem.id));
			return null;
		}
		gameObject.transform.rotation = Quaternion.Euler(0f, -180f, 0f);
		gameObject.transform.localScale = new Vector3(65f, 65f, 65f);
		return gameObject.GetComponent<ParticleSystem>();
	}

	private void StartStackAnimation()
	{
		if (_isPlay)
		{
			_stackAnimationSequence = GetStackSequence().AppendCallback(OnProgressAnimationEnd);
			_stackAnimationSequence.PlayForward();
		}
	}

	private void OnProgressAnimationEnd()
	{
		_repeatProgressStackAnimation--;
		if (_repeatProgressStackAnimation < 0)
		{
			UpdateStackedStats();
			StartCoroutine(ShowCardInfoAnimationCorutine(SwitchNextCard));
			return;
		}
		_progressBar.value = 0f;
		if (_repeatProgressStackAnimation == 0)
		{
			UpdateStackedStats();
		}
	}

	private void UpdateStackedStats()
	{
		RewardItemProvider.StackedItem currentStackedItem = _items[_currentCard].GetCurrentStackedItem();
		_rewardInfo.ShowAttributes(currentStackedItem.itemStacked, _dataProvider.equipedItems.GetEquipedAnalogByType(currentStackedItem.itemStacked));
		_rewardInfo.ShowDescription(currentStackedItem.itemStacked);
	}

	private Vector3 CalcStartStackPosition(int n, int count)
	{
		return new Vector3(CalcBaseCardPosition(n, count).x + _cardXStartOffset, _cardYStartPosition, _cardZStartPosition);
	}

	private Quaternion CalcStartStackRotation()
	{
		return Quaternion.Euler(new Vector3(_stackStartAngle, 0f, 0f));
	}

	private IReelItemAnimation DublicateCurrentCardAsStack()
	{
		_stackCard = CreateReelItem(_items[_currentCard].originItem, CalcStartStackPosition(_currentCard, _currentCard + 1), CalcStartStackRotation());
		_stackCard.item.transform.localScale = new Vector3(_cardScale, _cardScale);
		_stackCard.onAnimationEnd += OnCardAnimationEnd;
		_stackCard.item.gameObject.SetActive(true);
		return _stackCard;
	}

	private bool NeedStack(int id)
	{
		return _items[id].itemsStacked.Count > 0;
	}

	private void StartCardInAnimation()
	{
		if (_isPlay)
		{
			_motionBlur.enabled = true && _useMotionBlur;
			Vector3 moveTo = CalcEndPosition(_currentCard, _currentCard, _currentCard + 1);
			FadeIn();
			_reelItems[_currentCard].Animate("in", moveTo, Vector3.zero);
			PlayDropSound(_items[_currentCard].originItem);
		}
	}

	public void FadeIn()
	{
		UIWidget component = _reelItems[_currentCard].item.GetComponent<UIWidget>();
		component.alpha = 0f;
		DONgui.Fade(component, 1f, _cardFadeDuration).OnComplete(delegate
		{
			_reelItems[_currentCard].item.GetComponent<UIButton>().defaultColor = new UnityEngine.Color(1f, 1f, 1f, 1f);
		});
	}

	public void FadeOut()
	{
		UIWidget component = _reelItems[_currentCard].item.GetComponent<UIWidget>();
		DONgui.Fade(component, 1f, 0f, _cardFadeDuration).OnComplete(delegate
		{
			_reelItems[_currentCard].item.GetComponent<UIButton>().defaultColor = new UnityEngine.Color(1f, 1f, 1f, 0f);
		});
	}

	private void ShowCardInfoAnimation()
	{
		if (_isPlay)
		{
			_rewardInfo.SetPosition(ConvertCardPosition(_currentCard).x);
			ShowRewardStatus(_currentCard);
			if (NeedStack(_currentCard))
			{
				StartCoroutine(ShowCardInfoAnimationCorutine(StartStackAnimation));
			}
			else
			{
				StartCoroutine(ShowCardInfoAnimationCorutine(SwitchNextCard));
			}
		}
	}

	private IEnumerator ShowCardInfoAnimationCorutine(Action callback)
	{
		yield return new WaitForSeconds(_showInfoTime);
		callback();
	}

	private void SwitchNextCard()
	{
		if (_items[_currentCard].SwitchToNextStackItem() != null)
		{
			StartStackAnimation();
			return;
		}
		ShowRewardStatus(_currentCard);
		HideInfo();
		if (_currentCard <= _items.Count - 1)
		{
			StartMoveAnimation();
		}
	}

	private void StartMoveAnimation()
	{
		if (!_isPlay)
		{
			return;
		}
		_currentCard++;
		_finishedCardAnimation = 0;
		if (_currentCard != _items.Count)
		{
			for (int i = 0; i < _currentCard; i++)
			{
				StartSwipeAnimation(i, _currentCard + 1);
			}
		}
		else
		{
			AnimationEnd();
		}
	}

	private void StartSwipeAnimation(int i, int n)
	{
		if (_isPlay)
		{
			_reelItems[i].Animate("swipe", CalcEndPosition(i, n - 1, n), Vector3.zero, Vector3.one);
		}
	}

	private void OnCardAnimationEnd(string name, IReelItemAnimation target)
	{
		if (!_isPlay)
		{
			return;
		}
		switch (name)
		{
		case "in":
			_motionBlur.enabled = false;
			ShowCardInfoAnimation();
			break;
		case "swipe":
			_finishedCardAnimation++;
			if (_currentCard < _items.Count && _finishedCardAnimation == _currentCard)
			{
				StartCardInAnimation();
			}
			else if (_currentCard == _items.Count)
			{
				AnimationEnd();
			}
			break;
		case "select":
			_selectAnimationCounter--;
			if (_selectAnimationCounter == 0)
			{
				_rewardInfo.SetPosition(ConvertCardPosition(_selectedId).x);
				RewardItemProvider.StackedItem lastStackedItem = _items[_selectedId].GetLastStackedItem();
				BaseItem baseItem = null;
				if (lastStackedItem != null)
				{
					baseItem = lastStackedItem.itemStacked;
					_rewardInfo.ShowProgress(baseItem);
					HideRewardStatus(_selectedId);
				}
				else
				{
					baseItem = _items[_selectedId].originItem;
				}
				_rewardInfo.ShowItemName(baseItem.alias);
				_rewardInfo.ShowAttributes(baseItem, _dataProvider.equipedItems.GetEquipedAnalogByType(baseItem));
				_rewardInfo.ShowDescription(baseItem);
			}
			break;
		}
	}

	private void AnimationEnd()
	{
		AnimateSelectCard(_reelItems[_reelItems.Count - 1].item);
		if (this.onAnimationEnd != null)
		{
			this.onAnimationEnd();
		}
	}

	private int FindAnimationID(ReelItem item)
	{
		for (int i = 0; i < _reelItems.Count; i++)
		{
			if (_reelItems[i].item == item)
			{
				return i;
			}
		}
		return -1;
	}

	private Vector3 ConvertCardPosition(int id)
	{
		Vector3 position = _reelItems[id].item.transform.position;
		Vector3 position2 = _reelCamera.WorldToViewportPoint(position);
		return NekkiUIRoot.Camera.ViewportToWorldPoint(position2);
	}

	private void HideInfo()
	{
		_rewardInfo.HideItemName();
		_rewardInfo.HideProgress();
		_rewardInfo.HideAttributes();
		_rewardInfo.HideDescription();
	}

	private void PlayDropSound(BaseItem item)
	{
		IRarable rarable = item as IRarable;
		if (rarable != null)
		{
			AudioManager.Instance.PlaySound("ui/rewards/sound_drop_" + rarable.GetRarityType());
		}
	}

	private void ShowRewardStatus(int id)
	{
		ReelItem item = _reelItems[id].item;
		if (item == null)
		{
			Debug.LogError("Can't show card's reward status!");
			return;
		}
		if (_items[id].itemInInventory == null)
		{
			item.ShowRewardStatus(ReelItem.RewardStatus.NEW);
			return;
		}
		_rewardInfo.HideProgress();
		item.ShowRewardStatus(ReelItem.RewardStatus.UPGRADE);
	}

	private void HideRewardStatus(int id)
	{
		ReelItem item = _reelItems[id].item;
		if (item == null)
		{
			Debug.LogError("Can't hide card's reward status!");
		}
		else
		{
			item.HideRewardStatus();
		}
	}
}
