using System.Collections.Generic;
using SF3.KeyPressInfo;
using SF3.Moves;

namespace SF3
{
	public class BattleKeyManager
	{
		public static readonly Dictionary<EQuadrants, EQuadrants> INVERTION_MAP = new Dictionary<EQuadrants, EQuadrants>
		{
			{
				EQuadrants.Two,
				EQuadrants.Eight
			},
			{
				EQuadrants.Three,
				EQuadrants.Seven
			},
			{
				EQuadrants.Four,
				EQuadrants.Six
			},
			{
				EQuadrants.Six,
				EQuadrants.Four
			},
			{
				EQuadrants.Seven,
				EQuadrants.Three
			},
			{
				EQuadrants.Eight,
				EQuadrants.Two
			}
		};

		public static readonly Dictionary<EQuadrants, EQuadrants> CONFLICTING_MAP = new Dictionary<EQuadrants, EQuadrants>
		{
			{
				EQuadrants.Punch,
				EQuadrants.Kick
			},
			{
				EQuadrants.Kick,
				EQuadrants.Punch
			}
		};

		private static BattleKeyManager _instance;

		private List<PlayerBattleKeyManager> _keyManagers;

		private bool _paused;

		private bool _keyEventsEnabled;

		public static BattleKeyManager Instance
		{
			get
			{
				return _instance;
			}
		}

		public static bool paused
		{
			get
			{
				return _instance._paused;
			}
			private set
			{
				_instance._paused = value;
			}
		}

		public static bool keyEventsEnabled
		{
			get
			{
				return _instance._keyEventsEnabled;
			}
		}

		public BattleKeyManager()
		{
			_instance = this;
			_keyManagers = new List<PlayerBattleKeyManager>
			{
				new PlayerBattleKeyManager(1, OnBatteKeyStateChaged),
				new PlayerBattleKeyManager(2, OnBatteKeyStateChaged)
			};
		}

		public void Update()
		{
			float gameTimeDelta = ((!GameTimeController.gamePaused) ? GameTimeController.gameTimeDelta : 1f);
			for (int i = 0; i < _keyManagers.Count; i++)
			{
				_keyManagers[i].Update(gameTimeDelta);
			}
		}

		public void InitBattleKeys()
		{
			_paused = false;
			_keyEventsEnabled = false;
			for (int i = 0; i < _keyManagers.Count; i++)
			{
				_keyManagers[i].Initialize();
			}
		}

		public void DisposeKeyManagers()
		{
			for (int i = 0; i < _keyManagers.Count; i++)
			{
				_keyManagers[i].Dispose();
			}
		}

		public PlayerBattleKeyManager GetBattleKeysByModelID(int id)
		{
			for (int i = 0; i < _keyManagers.Count; i++)
			{
				if (_keyManagers[i].GetModelId() == id)
				{
					return _keyManagers[i];
				}
			}
			return null;
		}

		private void OnBatteKeyStateChaged(int modelID, KeyData keyState)
		{
			if (_keyEventsEnabled)
			{
				BattleController.ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_KEY_PRESSED, modelID, keyState));
			}
		}

		public void ActivateBattleKeys(bool val)
		{
			for (int i = 0; i < _keyManagers.Count; i++)
			{
				_keyManagers[i].Activate(val);
			}
		}

		public void ActivateBattleKey(EQuadrants quadrant, bool activate)
		{
			for (int i = 0; i < _keyManagers.Count; i++)
			{
				_keyManagers[i].Activate(quadrant, activate);
			}
		}

		public void EnableBattleKeysEvents(bool val)
		{
			_keyEventsEnabled = val;
		}

		public static void Unpause()
		{
			paused = false;
		}

		public static void Pause()
		{
			paused = true;
		}

		public bool SetIsControll(int modelID, bool isControll)
		{
			for (int i = 0; i < _keyManagers.Count; i++)
			{
				if (_keyManagers[i].GetModelId() == modelID)
				{
					_keyManagers[i].SetControll(isControll);
					return true;
				}
			}
			return false;
		}
	}
}
