using SF3;
using SF3.Audio;
using UnityEngine;

public class DebugMenu : UIModuleHolder, ISceneInitializationObject
{
	private static DebugMenu _inctance;

	public GameObject scrollView;

	public string[] commands;

	public ModuleInfo[] debugModules;

	private bool show;

	private float savedTime;

	public UIButton showHideBtn;

	public UIButton hideUIButton;

	public UIButton backGroundMusic;

	public UIButton soundsBtn;

	private UILabel musicLbl;

	private UILabel soundsLbl;

	private UILabel changeEffectLbl;

	public GameObject menuObj;

	private bool _uiHidden;

	private bool _musicOn = true;

	private bool _soundsOn = true;

	public float secondsResume;

	private float _timeToResume;

	public static DebugMenu Inctance
	{
		get
		{
			return _inctance;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		_inctance = this;
		showHideBtn.onClick.Add(new EventDelegate(ShowHide));
		hideUIButton.onClick.Add(new EventDelegate(ShowHideUI));
		backGroundMusic.onClick.Add(new EventDelegate(PauseResumeMusic));
		soundsBtn.onClick.Add(new EventDelegate(PauseResumeSounds));
		soundsLbl = soundsBtn.GetComponentInChildren<UILabel>();
		musicLbl = backGroundMusic.GetComponentInChildren<UILabel>();
		for (int i = 0; i < debugModules.Length; i++)
		{
			debugModules[i].Init();
		}
		string[] array = commands;
		foreach (string com in array)
		{
			GameObject gameObject = Object.Instantiate(hideUIButton.gameObject);
			gameObject.transform.parent = scrollView.transform;
			gameObject.GetComponent<UIButton>().onClick.Clear();
			gameObject.GetComponent<UIButton>().onClick.Add(new EventDelegate(delegate
			{
				NekkiConsole.ExecuteCommand(com);
			}));
			gameObject.GetComponentInChildren<UILabel>().text = com;
		}
		menuObj.SetActive(false);
	}

	public override void Initialize()
	{
		_musicOn = AudioManager.musicVolume > 0f;
		_soundsOn = AudioManager.volume > 0f;
		soundsLbl.text = ((!_soundsOn) ? "Resume " : "Pause ") + " SOUNDS";
		musicLbl.text = ((!_musicOn) ? "Resume " : "Pause ") + " MUSIC";
		if (!_soundsOn)
		{
			AudioManager.Instance.SetVolume(0f);
		}
		if (!_musicOn)
		{
			LocationAudioSettings.SetVolume(0f);
		}
	}

	public void DisposePreviousLocation()
	{
	}

	private void PauseResumeMusic()
	{
		_musicOn = !_musicOn;
		musicLbl.text = ((!_musicOn) ? "Resume " : "Pause ") + " MUSIC";
		AudioManager.Instance.SetMusicVolume((!_musicOn) ? 0f : 1f);
	}

	private void PauseResumeSounds()
	{
		_soundsOn = !_soundsOn;
		soundsLbl.text = ((!_soundsOn) ? "Resume " : "Pause ") + " SOUNDS";
		AudioManager.Instance.SetVolume((!_soundsOn) ? 0f : 1f, true);
	}

	private void ShowHideUI()
	{
		_uiHidden = !_uiHidden;
		UICamera.currentCamera.enabled = !_uiHidden;
	}

	public void ShowHide()
	{
		show = !show;
		menuObj.SetActive(show);
		if (show)
		{
			BattleController.PauseGame();
		}
		else
		{
			BattleController.ResumeGame();
		}
	}

	private void Update()
	{
		if (_uiHidden)
		{
			if (Input.GetMouseButtonDown(0))
			{
				_timeToResume = Time.realtimeSinceStartup + secondsResume;
			}
			if (Input.GetMouseButton(0) && _timeToResume <= Time.realtimeSinceStartup)
			{
				ShowHideUI();
			}
		}
	}
}
