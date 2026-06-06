using UnityEngine;

namespace SF3
{
	public class DebugInfoHider : UIModuleHolder
	{
		public enum EDebugButton
		{
			none = 1,
			playersStats = 2,
			leftTouch = 3,
			rightTouch = 4,
			enemyAI = 5
		}

		[SerializeField]
		private UIButton _btPlayersStats;

		public bool playersStatsShow;

		private GameObject playerStats;

		[SerializeField]
		private UIButton _btEnemyStats;

		public bool enemyStatsShow;

		private GameObject enemyStats;

		private bool _playerStatsActive;

		private bool _enemyStatsActive;

		[SerializeField]
		private UIButton _btSceneDebug;

		private GameObject sceneDebug;

		public bool sceneDebugShow;

		private bool _sceneDebugActive;

		public static GameObject setSceneDebugObj
		{
			set
			{
				Instance.sceneDebug = value;
			}
		}

		public static DebugInfoHider Instance { get; private set; }

		public static GameObject setPlayerStatsObj
		{
			set
			{
				Instance.playerStats = value;
			}
		}

		public static GameObject setEnemyStatsObj
		{
			set
			{
				Instance.enemyStats = value;
			}
		}

		public bool _collisionCapsulesActive { get; private set; }

		protected override void Awake()
		{
			base.Awake();
			Instance = this;
			_collisionCapsulesActive = false;
			_playerStatsActive = false;
			_enemyStatsActive = false;
			_collisionCapsulesActive = false;
			_sceneDebugActive = false;
		}

		private void Start()
		{
			if (_btPlayersStats != null)
			{
				_btPlayersStats.onClick.Add(new EventDelegate(OnPlayersStatsClick));
				if (playersStatsShow)
				{
					OnPlayersStatsClick();
				}
			}
			if (_btEnemyStats != null)
			{
				_btEnemyStats.onClick.Add(new EventDelegate(ShowHideEnemyStats));
				if (enemyStatsShow)
				{
					ShowHideEnemyStats();
				}
			}
			if (_btSceneDebug != null)
			{
				_btSceneDebug.onClick.Add(new EventDelegate(OnSceneDebugClick));
				if (sceneDebugShow)
				{
					OnSceneDebugClick();
				}
			}
		}

		private void ShowHideEnemyStats()
		{
			if (!(enemyStats == null) && enemyStats != null)
			{
				_enemyStatsActive = !_enemyStatsActive;
				enemyStats.SendMessage("ShowHide");
			}
		}

		private void OnPlayersStatsClick()
		{
			if (!(playerStats == null))
			{
				_playerStatsActive = !_playerStatsActive;
				playerStats.SendMessage("ShowHide");
			}
		}

		private void OnSceneDebugClick()
		{
			VisualDebugUI.Open();
			if (!(sceneDebug == null))
			{
				_sceneDebugActive = !_sceneDebugActive;
				sceneDebug.SendMessage("ShowHide");
			}
		}
	}
}
