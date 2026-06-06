using System;
using UnityEngine;

namespace SF3
{
	public class MapBattleButton : MonoBehaviour
	{
		public enum DecorationType
		{
			Available = 0,
			Unavailable = 1,
			NewButtonNotification = 2
		}

		private const string NOT_AVAILABLE_PREFIX = "_gray";

		private const int BASE_DEPTH = -990;

		private const float AVAILABLE_BATTLE_ALPHA = 1f;

		private const float NOT_AVAILABLE_BATTLE_ALPHA = 0.4f;

		private int _battleID;

		private readonly Vector3 _zoomedSize = new Vector3(120f, 120f, 1f);

		private readonly Vector3 _defaultSize = new Vector3(60f, 120f, 1f);

		[SerializeField]
		private UISprite _battleIcon;

		[SerializeField]
		private TweenAlpha _battleIconTween;

		[SerializeField]
		private UISprite _newBattleIconNotification;

		[SerializeField]
		private TweenAlpha _newBattleIconNotificationTween;

		[SerializeField]
		private UIButton _battleButton;

		[SerializeField]
		private UILabel _timeLabel;

		[SerializeField]
		private UISprite _selector;

		[SerializeField]
		private BoxCollider _collider;

		[SerializeField]
		private BattleBadgeUnit _badge;

		[SerializeField]
		private GameObject _draggablePannel;

		private bool _hasTimer;

		private DateTime _expire;

		private string _normallIconName;

		private DecorationType _currentDecorationType;

		public Transform TargetAnchor { get; private set; }

		private void Awake()
		{
			_newBattleIconNotification.depth = _battleIcon.depth + 1;
			_badge.onUpdateState += delegate(bool activeState)
			{
				if (activeState)
				{
					UpdateBattleIcon(DecorationType.NewButtonNotification);
				}
				else if (_currentDecorationType == DecorationType.NewButtonNotification)
				{
					UpdateBattleIcon(DecorationType.Available);
				}
			};
		}

		public void Initialize(int idBattle, Transform anchor, string battleName, string battleIcon, DecorationType decorationType, GameObject draggablePannel)
		{
			_battleID = idBattle;
			_draggablePannel = draggablePannel;
			base.transform.parent = anchor.parent;
			TargetAnchor = anchor;
			base.gameObject.name = battleName;
			base.transform.localScale = Vector3.one;
			ChangeBattleIcon(battleIcon);
			UpdateBattleIcon(decorationType);
			for (int i = 0; i < _battleButton.onClick.Count; i++)
			{
				if (_battleButton.onClick[i].target == this && _battleButton.onClick[i].methodName.Equals("OnMapButtonClick"))
				{
					_battleButton.onClick.RemoveAt(i);
					i--;
				}
			}
			_battleButton.onClick.Add(new EventDelegate(this, "OnMapButtonClick"));
			_badge.SetId(idBattle);
			TutorialComponent component = GetComponent<TutorialComponent>();
			if (component != null)
			{
				component.SetId("BattleID_" + idBattle);
			}
		}

		public void SetTimer(DateTime expireDate)
		{
			_expire = expireDate;
			_timeLabel.gameObject.SetActive(true);
			_hasTimer = true;
		}

		public void Update()
		{
			if (_hasTimer)
			{
				if (_expire.Subtract(TimeSpan.FromSeconds(1.0)) < NetworkConnection.current.getCurrentServerDateTime())
				{
					_timeLabel.text = string.Empty;
					_hasTimer = false;
				}
				else
				{
					_timeLabel.text = FormatTimeSpan(_expire - NetworkConnection.current.getCurrentServerDateTime());
				}
			}
		}

		public int GetBattleId()
		{
			return _battleID;
		}

		private void OnMapButtonClick()
		{
			string dstId = _battleID.ToString();
			string srcId = ((MapController.SelectedBattleUI == null) ? "none" : MapController.SelectedBattleUI.GetID().ToString());
			bool dialog = _currentDecorationType == DecorationType.NewButtonNotification;
			SF3UiLogger.instance.AddMapSelectBattleEvent(dstId, srcId, dialog);
			MapController.Instance.SelectBattleByClick(_battleID);
		}

		public void EnableSelector(bool isEnable)
		{
			_selector.enabled = isEnable;
		}

		public void ChangeBattleIcon(string iconName)
		{
			_normallIconName = iconName;
		}

		public void UpdateBattleIcon(DecorationType type)
		{
			switch (type)
			{
			case DecorationType.Available:
				UpdateSprite(_normallIconName, 1f);
				StopNotificationTween();
				break;
			case DecorationType.Unavailable:
				UpdateSprite(_normallIconName + "_gray", 0.4f);
				StopNotificationTween();
				break;
			case DecorationType.NewButtonNotification:
				UpdateSprite(_normallIconName, 1f);
				StartNotificationTween();
				break;
			default:
				Messenger.Error(string.Format("Type [{0}] is not supported", type.ToString()), this);
				throw new ArgumentOutOfRangeException("type", type, null);
			}
			_currentDecorationType = type;
		}

		private void StopNotificationTween()
		{
			_battleIconTween.enabled = false;
			_newBattleIconNotificationTween.enabled = false;
			_newBattleIconNotification.alpha = 0f;
		}

		private void StartNotificationTween()
		{
			_newBattleIconNotificationTween.PlayForward();
		}

		private void UpdateSprite(string name, float alpha)
		{
			_battleIcon.alpha = alpha;
			_battleButton.normalSprite = name;
			_battleIcon.spriteName = name;
		}

		public void EnableButton(bool isEnable)
		{
			_battleButton.enabled = isEnable;
		}

		private string FormatTimeSpan(TimeSpan date)
		{
			if (date.Days > 0)
			{
				return string.Format("{0}d {1:00}:{2:00}", date.Days, date.Hours, date.Minutes);
			}
			return string.Format("{0:00}:{1:00}:{2:00}", date.Hours, date.Minutes, date.Seconds);
		}

		private void OnPress(bool press)
		{
			_draggablePannel.SendMessage("OnPress", press, SendMessageOptions.DontRequireReceiver);
		}

		private void OnDrag(Vector2 delta)
		{
			_draggablePannel.SendMessage("OnDrag", delta, SendMessageOptions.DontRequireReceiver);
		}

		public void SetOrder(int index)
		{
			int num = index * 2 + -990;
			_battleIcon.depth = num;
			_selector.depth = num - 1;
			_battleIcon.depth = num;
			_timeLabel.depth = num;
			_badge.SetDepth(num + 1);
			_newBattleIconNotification.depth = num + 1;
		}

		public void SetActiveCollider(bool active, bool zoomed)
		{
			_collider.enabled = active;
			_collider.size = ((!zoomed) ? _defaultSize : _zoomedSize);
		}

		public Vector2 GetSize()
		{
			return _battleIcon.localSize;
		}
	}
}
