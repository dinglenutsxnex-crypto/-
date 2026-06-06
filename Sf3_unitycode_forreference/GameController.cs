using System.Collections;
using System.Linq;
using UnityEngine;

public class GameController : ExtentionBehaviour
{
	private bool _punchEnabled;

	private bool _kickEnabled;

	private bool _stickEnabled = true;

	private bool _isStartController;

	public const int onControlPressed = 0;

	public const int onControlReleased = 1;

	private static GameController _instance;

	private bool _quadrantsInverted;

	public static GameController Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = new GameObject("gameController").AddComponent<GameController>();
				_instance.InitController(true, true, true);
			}
			return _instance;
		}
	}

	private bool ActiveController
	{
		get
		{
			return _isStartController;
		}
		set
		{
			_isStartController = value;
			Stick.Instance.gameObject.SetActive(value);
			KeyboardController.Instance.gameObject.SetActive(value);
		}
	}

	public void InitController(bool punchEnabled, bool kickEnabled, bool joystickEnabled)
	{
		_punchEnabled = punchEnabled;
		_kickEnabled = kickEnabled;
		if (!AssemblyController.market.isSteam)
		{
			AddActionButtons(_punchEnabled, _kickEnabled);
			AddNavigator();
		}
		AddKeyboard();
		AddGamepad();
		_quadrantsInverted = false;
	}

	private void AddGamepad()
	{
		if (Input.GetJoystickNames().ToList().Exists((string name) => name != null))
		{
			GamepadController.Instance.removeEventListener(0, OnControlQuadrantPress);
			GamepadController.Instance.removeEventListener(1, OnControlQuadrantRelease);
			GamepadController.Instance.addEventListener(0, OnControlQuadrantPress);
			GamepadController.Instance.addEventListener(1, OnControlQuadrantRelease);
			GamepadController.Instance.AddTrakedKey(KeyCode.JoystickButton8, EQuadrants.Zero);
			GamepadController.Instance.AddTrakedKey(KeyCode.Joystick1Button0, EQuadrants.Kick, 1);
			GamepadController.Instance.AddTrakedKey(KeyCode.Joystick1Button1, EQuadrants.Punch, 1);
			GamepadController.Instance.AddTrakedKey(KeyCode.Joystick1Button2, EQuadrants.Magic, 1);
			GamepadController.Instance.AddTrakedKey(KeyCode.Joystick1Button3, EQuadrants.Missile, 1);
			GamepadController.Instance.AddTrakedKey(KeyCode.Joystick2Button0, EQuadrants.Kick, 2);
			GamepadController.Instance.AddTrakedKey(KeyCode.Joystick2Button1, EQuadrants.Punch, 2);
			GamepadController.Instance.AddTrakedKey(KeyCode.Joystick2Button2, EQuadrants.Magic, 2);
			GamepadController.Instance.AddTrakedKey(KeyCode.Joystick2Button3, EQuadrants.Missile, 2);
		}
	}

	public void startController()
	{
		ActiveController = true;
	}

	private void StopController()
	{
		ActiveController = false;
	}

	private void AddActionButtons(bool punchEnabled, bool kickEnabled)
	{
		StartCoroutine(AddActionButtonsRoutine(punchEnabled, kickEnabled));
	}

	private IEnumerator AddActionButtonsRoutine(bool punchEnabled, bool kickEnabled)
	{
		int trys = 0;
		while (!ActionButtons.Instance)
		{
			yield return new WaitForSeconds(0.1f);
			if (trys++ > 20)
			{
				yield break;
			}
		}
		if ((bool)ActionButtons.Instance)
		{
			ActionButtons.Instance.removeEventListener(0, OnControlQuadrantPress);
			ActionButtons.Instance.removeEventListener(1, OnControlQuadrantRelease);
			ActionButtons.Instance.addEventListener(0, OnControlQuadrantPress);
			ActionButtons.Instance.addEventListener(1, OnControlQuadrantRelease);
		}
		else
		{
			Debug.LogWarning("ActionButtons has no instance");
		}
	}

	private void AddKeyboard()
	{
		StartCoroutine(AddKeysboardRoutine());
	}

	private IEnumerator AddKeysboardRoutine()
	{
		while (!KeyboardController.Instance)
		{
			yield return new WaitForSeconds(0.1f);
		}
		KeyboardController.Instance.removeEventListener(0, OnControlQuadrantPress);
		KeyboardController.Instance.removeEventListener(1, OnControlQuadrantRelease);
		KeyboardController.Instance.addEventListener(0, OnControlQuadrantPress);
		KeyboardController.Instance.addEventListener(1, OnControlQuadrantRelease);
		KeyboardController.Instance.AddTrakedKey(KeyCode.Alpha0, EQuadrants.Zero);
		AddKeysModels();
		KeyboardController.Instance.AddTrakedKey(KeyCode.RightArrow, EQuadrants.NextFrameButton);
		KeyboardController.Instance.AddTrakedKey(KeyCode.Alpha1, EQuadrants.WinRoundButton);
		KeyboardController.Instance.AddTrakedKey(KeyCode.Alpha2, EQuadrants.WinFightButton);
		KeyboardController.Instance.AddTrakedKey(KeyCode.Alpha3, EQuadrants.ResetRoundButton);
		KeyboardController.Instance.AddTrakedKey(KeyCode.Alpha4, EQuadrants.ResetFightButton);
		KeyboardController.Instance.AddTrakedKey(KeyCode.Alpha5, EQuadrants.LossRoundButton);
		KeyboardController.Instance.AddTrakedKey(KeyCode.Alpha6, EQuadrants.LossFightButton);
		KeyboardController.Instance.AddTrakedKey(KeyCode.F1, EQuadrants.SlowModeKey);
		KeyboardController.Instance.AddTrakedKey(KeyCode.F3, EQuadrants.ShowEdgesButton);
		KeyboardController.Instance.AddTrakedKey(KeyCode.F4, EQuadrants.PauseButton);
		KeyboardController.Instance.AddTrakedKey(KeyCode.F5, EQuadrants.ShowDebugPerksButton);
		KeyboardController.Instance.AddTrakedKey(KeyCode.F10, EQuadrants.FullscreenMode);
		KeyboardController.Instance.AddTrakedKey(KeyCode.F11, EQuadrants.EnableMinScale);
		KeyboardController.Instance.AddTrakedKey(KeyCode.F12, EQuadrants.TestTactic);
		KeyboardController.Instance.AddTrakedKey(KeyCode.M, EQuadrants.SoundMuteButton);
		KeyboardController.Instance.AddTrakedKey(KeyCode.B, EQuadrants.StartBenchmarkKey);
		KeyboardController.Instance.AddTrakedKey(KeyCode.U, EQuadrants.StartSuper);
	}

	private void AddKeysModels()
	{
		StartCoroutine(addKeysModelsRoutine());
	}

	private IEnumerator addKeysModelsRoutine()
	{
		while (!KeyboardController.Instance)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (!AssemblyController.market.isSteam)
		{
			KeyboardController.Instance.AddTrakedKey(KeyCode.W, EQuadrants.One, 1);
			KeyboardController.Instance.AddTrakedKey(KeyCode.E, EQuadrants.Two, 1);
			KeyboardController.Instance.AddTrakedKey(KeyCode.D, EQuadrants.Three, 1);
			KeyboardController.Instance.AddTrakedKey(KeyCode.C, EQuadrants.Four, 1);
			KeyboardController.Instance.AddTrakedKey(KeyCode.X, EQuadrants.Five, 1);
			KeyboardController.Instance.AddTrakedKey(KeyCode.Z, EQuadrants.Six, 1);
			KeyboardController.Instance.AddTrakedKey(KeyCode.A, EQuadrants.Seven, 1);
			KeyboardController.Instance.AddTrakedKey(KeyCode.Q, EQuadrants.Eight, 1);
			KeyboardController.Instance.AddTrakedKey(KeyCode.Keypad8, EQuadrants.One, 2);
			KeyboardController.Instance.AddTrakedKey(KeyCode.Keypad9, EQuadrants.Two, 2);
			KeyboardController.Instance.AddTrakedKey(KeyCode.Keypad6, EQuadrants.Three, 2);
			KeyboardController.Instance.AddTrakedKey(KeyCode.Keypad3, EQuadrants.Four, 2);
			KeyboardController.Instance.AddTrakedKey(KeyCode.Keypad2, EQuadrants.Five, 2);
			KeyboardController.Instance.AddTrakedKey(KeyCode.Keypad1, EQuadrants.Six, 2);
			KeyboardController.Instance.AddTrakedKey(KeyCode.Keypad4, EQuadrants.Seven, 2);
			KeyboardController.Instance.AddTrakedKey(KeyCode.Keypad7, EQuadrants.Eight, 2);
		}
		KeyboardController.Instance.AddTrakedKey(KeyCode.O, EQuadrants.Punch, 1);
		KeyboardController.Instance.AddTrakedKey(KeyCode.P, EQuadrants.Kick, 1);
		KeyboardController.Instance.AddTrakedKey(KeyCode.K, EQuadrants.Missile, 1);
		KeyboardController.Instance.AddTrakedKey(KeyCode.L, EQuadrants.Magic, 1);
		KeyboardController.Instance.AddTrakedKey(KeyCode.Insert, EQuadrants.Punch, 2);
		KeyboardController.Instance.AddTrakedKey(KeyCode.Delete, EQuadrants.Kick, 2);
		KeyboardController.Instance.AddTrakedKey(KeyCode.PageDown, EQuadrants.Missile, 2);
		KeyboardController.Instance.AddTrakedKey(KeyCode.PageUp, EQuadrants.Magic, 2);
		KeyboardController.Instance.AddTrakedKey(KeyCode.T, EQuadrants.TEST1);
		KeyboardController.Instance.AddTrakedKey(KeyCode.Y, EQuadrants.TEST2);
	}

	private void AddNavigator()
	{
		StartCoroutine(AddNavigatorRoutine());
	}

	private IEnumerator AddNavigatorRoutine()
	{
		while (!Stick.Instance)
		{
			yield return new WaitForSeconds(0.1f);
		}
		Stick.Instance.addEventListener(0, OnControlQuadrantPress);
		Stick.Instance.addEventListener(1, OnControlQuadrantPress);
		Stick.Instance.addEventListener(2, OnControlQuadrantRelease);
	}

	private void OnControlQuadrantPress(CallEventArgs args)
	{
		callQuadrant(0, args);
	}

	private void OnControlQuadrantRelease(CallEventArgs args)
	{
		callQuadrant(1, args);
	}

	private void callQuadrant(int type, CallEventArgs args)
	{
		AbstractController.Key key = (AbstractController.Key)args.Content;
		EQuadrants eQuadrants = key.Quadrant;
		int modelID = key.ModelID;
		if (_quadrantsInverted)
		{
			switch (eQuadrants)
			{
			case EQuadrants.One:
				eQuadrants = EQuadrants.Five;
				break;
			case EQuadrants.Two:
				eQuadrants = EQuadrants.Six;
				break;
			case EQuadrants.Three:
				eQuadrants = EQuadrants.Seven;
				break;
			case EQuadrants.Four:
				eQuadrants = EQuadrants.Eight;
				break;
			case EQuadrants.Five:
				eQuadrants = EQuadrants.One;
				break;
			case EQuadrants.Six:
				eQuadrants = EQuadrants.Two;
				break;
			case EQuadrants.Seven:
				eQuadrants = EQuadrants.Three;
				break;
			case EQuadrants.Eight:
				eQuadrants = EQuadrants.Four;
				break;
			}
		}
		if (IsQuadrantEnabled(eQuadrants))
		{
			callEvent(type, new int[2]
			{
				modelID,
				(int)eQuadrants
			});
		}
	}

	public void InvertQuadrants(bool useInvertQuadrants)
	{
		_quadrantsInverted = useInvertQuadrants;
	}

	private bool IsQuadrantEnabled(EQuadrants key)
	{
		if (!_punchEnabled && key == EQuadrants.Punch)
		{
			return false;
		}
		if (!_kickEnabled && key == EQuadrants.Kick)
		{
			return false;
		}
		if (!_stickEnabled && key.isStickQuadrant())
		{
			return false;
		}
		return true;
	}
}
