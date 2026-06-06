using System;
using Nekki;
using SF3;
using SF3.GameModels;
using SF3.Items;
using SF3.UserData;
using UnityEngine;

public sealed class BattleInterface : ExtentionBehaviour, ISceneInitializationObject
{
	private static BattleInterface _instance;

	private ShadowPerksController _shadowPerks;

	private HUD _playerHud;

	private HUD _foeHud;

	private IModelHUD _playerModelInfo;

	private IModelHUD _foeModelInfo;

	private readonly string[] _debugModules = new string[3] { "DebugMenu", "Console", "Debug" };

	private UIModulesController _uiModulesController;

	private int _currentFrame;

	private int _totalFrame;

	private int _currentDelayFrame;

	private int _delayFrame;

	private Action _callBack;

	private Action _delayCall;

	private Action<RoundsUI.RoundMessageData> _delayCallWithMsg;

	private RoundTimer _roundTimer;

	private RoundsUI _roundsUI;

	private RoundsUI.RoundMessageData _roundMessageData;

	[SerializeField]
	private float TIME_GREAT = 0.75f;

	[SerializeField]
	private float TIME_PERFECT = 0.75f;

	[SerializeField]
	private float TIME_VS = 1f;

	[SerializeField]
	private float TIME_ROUND_START = 2.5f;

	[SerializeField]
	private float TIME_FIGHT_START = 1.5f;

	[SerializeField]
	private float TIME_ROUND_END = 0.75f;

	[SerializeField]
	private float TIME_GAME_END = 1f;

	[SerializeField]
	private float DELAY_GREAT;

	[SerializeField]
	private float DELAY_PERFECT;

	[SerializeField]
	private float DELAY_VS;

	[SerializeField]
	private float DELAY_FIRST_ROUND = 1f;

	[SerializeField]
	private float DELAY_ROUND_START;

	[SerializeField]
	private float DELAY_FIGHT_START;

	[SerializeField]
	private float DELAY_ROUND_END;

	[SerializeField]
	private float DELAY_GAME_END;

	[SerializeField]
	private UILabel _nickNameFullLable;

	private bool _timerActive;

	private bool _timerEnable;

	private int _timeSecond;

	public static BattleInterface Instance
	{
		get
		{
			return _instance;
		}
	}

	public float TimeCount { get; private set; }

	private void Awake()
	{
		_instance = this;
	}

	public void Initialize()
	{
		if (_uiModulesController == null)
		{
			_uiModulesController = new UIModulesController();
		}
		UIModulesController.Instance.InitializeModules(BattlesManager.currentBattleType);
		if (BattlesManager.currentBattleType != 0)
		{
			NekkiUIModule mountedModule = _uiModulesController.GetMountedModule(UIModulesController.EUIElements.PLAYER_HUD);
			if (mountedModule != null)
			{
				_playerHud = mountedModule.GetComponent<HUD>();
			}
			mountedModule = _uiModulesController.GetMountedModule(UIModulesController.EUIElements.FOE_HUD);
			if (mountedModule != null)
			{
				_foeHud = mountedModule.GetComponent<HUD>();
			}
			mountedModule = _uiModulesController.GetMountedModule(UIModulesController.EUIElements.SHADOW_PERKS);
			if (mountedModule != null)
			{
				_shadowPerks = mountedModule.GetComponent<ShadowPerksController>();
			}
			mountedModule = _uiModulesController.GetMountedModule(UIModulesController.EUIElements.ROUND_TIMER);
			if (mountedModule != null)
			{
				_roundTimer = mountedModule.GetComponent<RoundTimer>();
			}
			mountedModule = _uiModulesController.GetMountedModule(UIModulesController.EUIElements.ROUNDS_UI);
			if (mountedModule != null)
			{
				_roundsUI = mountedModule.GetComponent<RoundsUI>();
			}
		}
		if (NekkiUtils.IsDebug)
		{
			DebugMenu.Inctance.Initialize();
		}
		ResetBattleTimer();
	}

	public void DisposePreviousLocation()
	{
		_playerHud = null;
		_foeHud = null;
		_playerModelInfo = null;
		_foeModelInfo = null;
	}

	public void DisposeObject(GameObject obj)
	{
		if (obj != null)
		{
			GlobalLoad.Unload(obj);
		}
	}

	public void HPBarsEnable(bool isEnable)
	{
		_playerHud.HPBarEnable(isEnable);
		_foeHud.HPBarEnable(isEnable);
	}

	public void ScoreBarsEnable(bool isEnable)
	{
		_playerHud.ScoreBarEnable(isEnable);
		_foeHud.ScoreBarEnable(isEnable);
	}

	public void ShadowFormEnable(bool isEnable)
	{
		_playerHud.ShadowFormEnable(isEnable);
		_foeHud.ShadowFormEnable(isEnable);
		_shadowPerks.gameObject.SetActive(isEnable);
		if (_shadowPerks != null)
		{
			_shadowPerks.SetShadowPerksState(ShadowPerksState.Disable);
		}
		ActionButtons.Instance.ShadowButtonEnable(isEnable);
	}

	public void ClearShadowPerkSlot(int modelId, EquipmentType equipmentType)
	{
		if (_shadowPerks != null)
		{
			_shadowPerks.ClearShadowPerkSlot(modelId, equipmentType);
		}
	}

	public void SetShadowPerkState(int modelId, ShadowPerksState state)
	{
		if (_shadowPerks != null)
		{
			_shadowPerks.SetShadowPerkState(modelId, state);
		}
	}

	public void ShadowPerksInvert(bool invert)
	{
		if (_shadowPerks != null)
		{
			_shadowPerks.Invert(invert);
		}
		StickHelper.Instance.UpdateDirection(invert);
	}

	public void ShadowPerksCooldown(int modelId, EquipmentType equipmentType, int framesDuration)
	{
		if (_shadowPerks != null)
		{
			_shadowPerks.StartCooldown(modelId, equipmentType, framesDuration);
		}
	}

	public void ColorPlayerRoundsUI(int roundsWinned)
	{
		_playerHud.ColorRoundsWinned(roundsWinned);
	}

	public void ColorEnemyRoundsUI(int roundsWinned)
	{
		_foeHud.ColorRoundsWinned(roundsWinned);
	}

	public void SetNickNameFull(string value)
	{
		_nickNameFullLable.text = value;
	}

	public void InitializeFightHud()
	{
		InitDebug(ModelsManager.Instance.Player, ModelsManager.Instance.Enemy);
		if (BattlesManager.currentBattleType == BattleType.Fight)
		{
			InitHud(ModelsManager.Instance.Player, _playerHud);
			InitHud(ModelsManager.Instance.Enemy, _foeHud);
			_shadowPerks.Init(ModelsManager.Instance.Player, ModelsManager.Instance.Enemy);
		}
		ActionButtons.Instance.InitializeButtons();
	}

	private void InitHud(Model model, HUD hud)
	{
		if (model.isPlayer)
		{
			_playerModelInfo = model;
			if (Localization.Contains(model.GetAlias(), true))
			{
				hud.Alias = model.GetAlias();
			}
			else
			{
				hud.Name = UserManager.GetName();
			}
		}
		else
		{
			_foeModelInfo = model;
			hud.Alias = model.GetAlias();
		}
		hud.SetModel(model);
	}

	private void InitDebug(params Model[] models)
	{
		foreach (Model model in models)
		{
			if (!NekkiUtils.IsDebug)
			{
				break;
			}
			TweenPosition tweenPosition = null;
			string moduleName = ((!model.modelInfo.isPlayer) ? "ModelDebugEnemy" : "ModelDebug");
			NekkiUIModule module = NekkiUIRootModules.Instance.GetModule(moduleName);
			tweenPosition = ((!(module == null)) ? module.GetComponent<TweenPosition>() : NekkiUIRootModules.Instance.MountNGUIModule(moduleName).GetComponent<TweenPosition>());
			if (tweenPosition != null)
			{
				tweenPosition.GetComponent<ModelDebugInfo>().Init(model);
			}
		}
	}

	internal void Update()
	{
		if (_playerModelInfo != null && !(_playerHud == null))
		{
			_playerHud.HP = _playerModelInfo.GetCurrentLife() / _playerModelInfo.GetMaxLife();
			_playerHud.Score = _playerModelInfo.GetScore();
			if (_foeModelInfo != null)
			{
				_foeHud.HP = _foeModelInfo.GetCurrentLife() / _foeModelInfo.GetMaxLife();
				_foeHud.Score = _foeModelInfo.GetScore();
			}
		}
	}

	public void ShowTextImage()
	{
		GameObject gameObject = NGUITools.AddChild(GameObject.Find("TOP_LEFT"), GlobalLoad.GetPrefab("gamedata/GUI/fight/TextImage"));
		gameObject.GetComponent<TweenPosition>().PlayForward();
	}

	public void ShowDebugUI(bool activate = true)
	{
		if (activate)
		{
			NekkiUIRootModules.Instance.EnableModules(_debugModules);
		}
		else
		{
			NekkiUIRootModules.Instance.DisableModules(_debugModules);
		}
	}

	public void InitBattleGUIManager(int roundTimeTotal)
	{
		if (_roundTimer != null)
		{
			_roundTimer.UpdateLabel(roundTimeTotal.ToString());
		}
	}

	public void ResetBattleGUIManager()
	{
		_totalFrame = 0;
		_delayFrame = 0;
		_currentFrame = -1;
		_currentDelayFrame = -1;
		_roundMessageData = null;
		Clear();
	}

	private void ShowEffect(Action _callBack, Action _effectCall, float delay, float duration)
	{
		_delayCall = _effectCall;
		_delayCallWithMsg = null;
		this._callBack = _callBack;
		SetEffectDuration(delay, duration);
	}

	private void ShowEffect(Action _callBack, Action<RoundsUI.RoundMessageData> _effectCall, float delay, float duration)
	{
		_delayCall = null;
		_delayCallWithMsg = _effectCall;
		this._callBack = _callBack;
		SetEffectDuration(delay, duration);
	}

	private void SetEffectDuration(float delay, float duration)
	{
		_currentFrame = 0;
		_currentDelayFrame = 0;
		_totalFrame = Mathf.RoundToInt(60f * (delay + duration));
		_delayFrame = Mathf.RoundToInt(60f * delay);
	}

	public void ShowGreat(Action _callBack = null)
	{
		ShowEffect(_callBack, _roundsUI.ShowGreat, DELAY_GREAT, TIME_GREAT);
	}

	public void ShowPerfect(Action _callBack = null)
	{
		ShowEffect(_callBack, _roundsUI.ShowPerfect, DELAY_PERFECT, TIME_PERFECT);
	}

	public void ShowVS(Action _callBack)
	{
		ShowEffect(_callBack, (Action)delegate
		{
			_roundsUI.ShowText(string.Concat(Localization.Get(UserManager.UserModelInfo.alias), " VS ", Localization.Get(ModelsManager.Instance.Enemy.modelInfo.alias)));
		}, DELAY_VS, TIME_VS);
	}

	public void ShowStartRound(Action _callBack, int currentRound)
	{
		_roundMessageData = new RoundsUI.RoundMessageData(currentRound);
		ShowEffect(_callBack, _roundsUI.ShowRoundStart, (currentRound != 1) ? DELAY_ROUND_START : DELAY_FIRST_ROUND, TIME_ROUND_START);
	}

	public void ShowStartRoundFight(Action _callBack)
	{
		ShowEffect(_callBack, _roundsUI.ShowFightStart, DELAY_FIGHT_START, TIME_FIGHT_START);
	}

	public void ShowEndRoundFight(Action _callBack, ERoundResult resultRound)
	{
		_roundMessageData = new RoundsUI.RoundMessageData(resultRound);
		ShowEffect(_callBack, _roundsUI.ShowRoundEnd, DELAY_ROUND_END, TIME_ROUND_END);
	}

	public void ShowEndRoundFight_PVP(Action _callBack, ERoundResult resultRound, string winnerName)
	{
		_roundMessageData = new RoundsUI.RoundMessageData(resultRound, winnerName);
		ShowEffect(_callBack, _roundsUI.ShowRoundEnd_PVP, DELAY_ROUND_END, TIME_ROUND_END);
	}

	public void ShowEndGame(Action _callBack, FightResult fightResult)
	{
		_roundMessageData = new RoundsUI.RoundMessageData(fightResult);
		ShowEffect(_callBack, _roundsUI.ShowFightEnd, DELAY_GAME_END, TIME_GAME_END);
	}

	public void Clear()
	{
		if (_roundsUI != null)
		{
			_roundsUI.Clear();
		}
	}

	public void UpdateRoundTimer()
	{
		if (_timerActive)
		{
			int timeSecond = _timeSecond;
			TimeCount -= GameTimeController.gameTimeDelta;
			_timeSecond = (int)(TimeCount / 60f);
			if (timeSecond != _timeSecond)
			{
				UpdateRoundTimer(_timeSecond);
			}
		}
	}

	public void ResetBattleTimer()
	{
		TimeCount = 0f;
		_timeSecond = 0;
		_timerActive = false;
		BattleTimerEnable(true);
	}

	public void SetBattleTimerFrames(float value)
	{
		TimeCount = value;
		_timeSecond = (int)(TimeCount / 60f);
		UpdateRoundTimer(_timeSecond);
	}

	private void UpdateRoundTimer(int currentSecond)
	{
		if (!(null == _roundTimer))
		{
			_roundTimer.UpdateLabel(Mathf.Max(0, currentSecond).ToString());
		}
	}

	public void SetBattleTimerSeconds(float value)
	{
		SetBattleTimerFrames(value * 60f);
	}

	public void BattleTimerActive(bool isActive)
	{
		if (_timerEnable)
		{
			_timerActive = isActive;
		}
	}

	public void BattleTimerEnable(bool isEnable)
	{
		_timerEnable = isEnable;
		if (!_timerEnable)
		{
			_timerActive = false;
			_roundTimer.UpdateLabel("∞");
		}
	}

	public void UpdateBattleGUIManager()
	{
		if (_currentDelayFrame > -1)
		{
			_currentDelayFrame++;
			if (_currentFrame >= _delayFrame)
			{
				if (_delayCall != null)
				{
					_delayCall();
				}
				else if (_delayCallWithMsg != null)
				{
					_delayCallWithMsg(_roundMessageData);
				}
				_currentDelayFrame = -1;
			}
		}
		if (_currentFrame <= -1)
		{
			return;
		}
		_currentFrame++;
		if (_currentFrame >= _totalFrame)
		{
			ResetBattleGUIManager();
			if (_callBack != null)
			{
				_callBack();
			}
		}
	}
}
