using System.Text;
using SF3;
using SF3.Audio;
using SF3.UserData;
using UnityEngine;
using UnityEngine.UI;

public class PauseWindow : UIModuleHolder
{
	public delegate void PauseEnabledEventHandler();

	public delegate void PauseDisabledEventHandler();

	private const float DISABLE_ALPHA = 0.33f;

	[SerializeField]
	private UnityEngine.UI.Text _rulesLabel;

	[SerializeField]
	private Button _resumeBtn;

	[SerializeField]
	private Button _surrenderBtn;

	[SerializeField]
	private Button _soundBtn;

	[SerializeField]
	private Button _musicBtn;

	private readonly string[] _hideModules = new string[8] { "Joystick", "FightButtons", "PauseButton", "PlayerHUD", "FoeHUD", "RoundTimer", "RoundsUI", "ShadowPerks" };

	private static PauseWindow _instance;

	private static int _showCounter;

	private static NekkiUIModule _pauseButton;

	private static NekkiUIModule _roundTimer;

	private static TutorialComponent _pauseButtonTutorialComponent;

	private static TutorialComponent _roundTimerTutorialComponent;

	public static event PauseEnabledEventHandler OnPauseEnabled;

	public static event PauseDisabledEventHandler OnPauseDisabled;

	public static void ResetShowCounter()
	{
		_showCounter = 0;
		RefreshDependentOnShowCounter();
	}

	public static void IncrementShowCounter()
	{
		AddShowCounter(1);
	}

	public static void DecrementShowCounter()
	{
		AddShowCounter(-1);
	}

	private static void AddShowCounter(int delta)
	{
		_showCounter += delta;
		RefreshDependentOnShowCounter();
	}

	private static void RefreshDependentOnShowCounter()
	{
		if (!(_instance == null))
		{
			if (_pauseButton == null)
			{
				_pauseButton = NekkiUIRootModules.Instance.MountNativeModule("PauseButton");
			}
			if (_roundTimer == null)
			{
				_roundTimer = NekkiUIRootModules.Instance.MountNativeModule("RoundTimer");
			}
			if (_pauseButtonTutorialComponent == null)
			{
				_pauseButtonTutorialComponent = _pauseButton.GetComponent<TutorialComponent>();
			}
			if (_roundTimerTutorialComponent == null)
			{
				_roundTimerTutorialComponent = _roundTimer.GetComponent<TutorialComponent>();
			}
			_pauseButton.gameObject.SetActive(_pauseButtonTutorialComponent.GetVisible() && _showCounter > 0);
			_roundTimer.gameObject.SetActive(_pauseButtonTutorialComponent.GetVisible() && _showCounter > 0);
		}
	}

	public bool IsNative(string moduleName)
	{
		switch (moduleName)
		{
		case "Joystick":
			return false;
		case "FightButtons":
			return false;
		case "PlayerHUD":
			return false;
		case "FoeHUD":
			return false;
		case "RoundsUI":
			return false;
		case "PauseButton":
			return true;
		case "RoundTimer":
			return true;
		case "ShadowPerks":
			return true;
		default:
			return false;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		_resumeBtn.onClick.AddListener(OnResumeClick);
		if (UserManager.GetGlobalVariable("TUTORIAL") == "1")
		{
			_surrenderBtn.gameObject.SetActive(false);
		}
		else
		{
			_surrenderBtn.onClick.AddListener(OnSurrenderClick);
		}
		_soundBtn.onClick.AddListener(OnSoundClick);
		_musicBtn.onClick.AddListener(OnMusicClick);
		UpdateBtn(_soundBtn, AudioManager.volume);
		UpdateBtn(_musicBtn, AudioManager.musicVolume);
		base.gameObject.SetActive(false);
		_instance = this;
		ResetShowCounter();
	}

	private void OnSurrenderClick()
	{
		BattleController.Instance.fightController.SetFightResult(2, true);
		base.gameObject.SetActive(false);
	}

	private void OnResumeClick()
	{
		Pause(false);
	}

	private void OnSoundClick()
	{
		AudioManager.Instance.SetVolume((!(AudioManager.volume > 0f)) ? 1 : 0);
		UpdateBtn(_soundBtn, AudioManager.volume);
	}

	private void OnMusicClick()
	{
		AudioManager.Instance.SetMusicVolume((!(AudioManager.musicVolume > 0f)) ? 1 : 0);
		UpdateBtn(_musicBtn, AudioManager.musicVolume);
	}

	public static void Pause()
	{
		if (!(_instance == null))
		{
			_instance.Pause(true);
		}
	}

	public static void UnPause()
	{
		if (!(_instance == null))
		{
			_instance.Pause(false);
		}
	}

	public void Pause(bool isPause)
	{
		base.gameObject.SetActive(isPause);
		if (isPause)
		{
			BattleController.SystemPause();
			_rulesLabel.text = BuildRulesString();
			if (PauseWindow.OnPauseEnabled != null)
			{
				PauseWindow.OnPauseEnabled();
			}
		}
		else
		{
			BattleController.SystemResume();
			if (PauseWindow.OnPauseDisabled != null)
			{
				PauseWindow.OnPauseDisabled();
			}
		}
		ShowModules(!isPause);
		if (isPause)
		{
			ScreenTexture.Instance.SetTexture(base.name, ScreenTexture.TextureOutputCamera.Main, ScreenTexture.TextureOutputFilter.Blur);
		}
		else
		{
			ScreenTexture.Instance.Clear(base.name, 0f);
		}
	}

	private string BuildRulesString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (Rule allRule in ModelsManager.Instance.GetAllRules())
		{
			string attributeByType = allRule.GetAttributeByType("Description");
			if (!string.IsNullOrEmpty(attributeByType))
			{
				string @string = Localization.Get(attributeByType).String;
				if (!string.IsNullOrEmpty(@string))
				{
					stringBuilder.AppendLine(@string);
				}
			}
		}
		if (stringBuilder.Length > 0)
		{
			stringBuilder.Length--;
		}
		return stringBuilder.ToString();
	}

	private void ShowModules(bool show)
	{
		string[] hideModules = _hideModules;
		foreach (string moduleName in hideModules)
		{
			NekkiUIModule nekkiUIModule = ((!IsNative(moduleName)) ? NekkiUIRootModules.Instance.MountNGUIModule(moduleName) : NekkiUIRootModules.Instance.MountNativeModule(moduleName));
			TutorialComponent component = nekkiUIModule.GetComponent<TutorialComponent>();
			if (component != null)
			{
				nekkiUIModule.gameObject.SetActive(show && component.GetVisible());
			}
			else
			{
				nekkiUIModule.gameObject.SetActive(show);
			}
		}
	}

	internal void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Pause(!GameTimeController.gamePaused);
		}
	}

	private void UpdateBtn(Button button, float volume)
	{
		ColorBlock colors = button.colors;
		Color normalColor = colors.normalColor;
		Color highlightedColor = colors.highlightedColor;
		if (volume > 0f)
		{
			normalColor.a = (highlightedColor.a = 1f);
		}
		else
		{
			normalColor.a = (highlightedColor.a = 0.33f);
		}
		colors.normalColor = normalColor;
		colors.highlightedColor = highlightedColor;
		button.colors = colors;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		ScreenTexture.Instance.Clear(base.name, 0f);
		ResetShowCounter();
		_instance = null;
	}
}
