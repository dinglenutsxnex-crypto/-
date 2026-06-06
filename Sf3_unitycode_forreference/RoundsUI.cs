using System;
using DG.Tweening;
using SF3;
using SF3.Audio;
using SF3.Effects;
using UnityEngine;
using UnityEngine.UI;
using sf3DTO;

public class RoundsUI : UIModuleHolder
{
	[Serializable]
	public struct Shake
	{
		public bool roundStart;

		public bool roundWin;

		public bool roundLose;

		public bool fightStart;

		public bool fightEnd;

		public bool perfect;

		public bool great;

		public bool timeout;
	}

	[Serializable]
	public class TextAnimation
	{
		public UnityEngine.UI.Text label;

		public Image back;

		[HideInInspector]
		public bool active;

		private CanvasGroup _canvasGroup;

		public string SetText
		{
			set
			{
				label.text = value;
				active = true;
			}
		}

		public void Init()
		{
			label.gameObject.SetActive(false);
			back.gameObject.SetActive(false);
			_canvasGroup = label.gameObject.GetComponent<CanvasGroup>();
		}

		private void PlayAppearingAnimation(bool useFogging)
		{
			if (useFogging)
			{
				FoggingController.Instance.ShowFogging();
			}
			back.gameObject.SetActive(true);
			label.gameObject.SetActive(true);
			_canvasGroup.alpha = 1f;
			back.color = new UnityEngine.Color(1f, 1f, 1f, _minLabelVisibility);
			back.DOFade(0.65f, _animationTime);
			ShortcutExtensions.DOScale(endValue: new Vector3(_finalLabelScale, _finalLabelScale, _finalLabelScale), target: label.gameObject.transform, duration: _animationTime).OnComplete(delegate
			{
				ShakeCamera();
			});
		}

		private void PlayDisapperaingAnimation()
		{
			_canvasGroup.DOFade(_minLabelVisibility, _animationTime).OnComplete(delegate
			{
				label.gameObject.SetActive(false);
			});
			FoggingController.Instance.HideFogging();
			back.color = new UnityEngine.Color(1f, 1f, 1f, _maxLabelVisibility);
			back.DOFade(_minLabelVisibility, _animationTime).OnComplete(delegate
			{
				back.gameObject.SetActive(false);
			});
		}

		public void Show(Vector3 from, Vector3 to, bool useFogging, UnityEngine.Color? textColor = null)
		{
			label.gameObject.SetActive(true);
			label.color = ((!textColor.HasValue) ? UnityEngine.Color.white : textColor.Value);
			label.transform.localScale = from;
			PlayAppearingAnimation(useFogging);
		}

		public void Hide()
		{
			active = false;
			PlayDisapperaingAnimation();
		}
	}

	public class RoundMessageData
	{
		public int RoundNumber { get; private set; }

		public string PlayerName { get; private set; }

		public ERoundResult RoundResult { get; private set; }

		public sf3DTO.FightResult FightResultType { get; private set; }

		public sf3DTO.BattleType BattleType { get; private set; }

		public int RoundsWon { get; private set; }

		public RoundMessageData(int round)
		{
			RoundNumber = round;
		}

		public RoundMessageData(ERoundResult result)
		{
			RoundResult = result;
		}

		public RoundMessageData(ERoundResult result, string name)
		{
			RoundResult = result;
			PlayerName = name;
		}

		public RoundMessageData(SF3.FightResult result)
		{
			FightResultType = result.resultType;
			BattleType = result.BattleType;
			RoundsWon = result.roundsWon;
		}
	}

	public const string RoundNumberColor = "FE762B";

	[SerializeField]
	private UnityEngine.Color greatColor;

	[SerializeField]
	private UnityEngine.Color perfectColor;

	[SerializeField]
	private static float shakeFrames;

	[SerializeField]
	private static Vector3 shakeAmplitude;

	[SerializeField]
	private static Vector3 shakePeriod;

	[SerializeField]
	private static Shake shakeCamera;

	[SerializeField]
	private static float _animationTime = 0.4f;

	[SerializeField]
	private static float _finalLabelScale = 1f;

	[SerializeField]
	private static float _minLabelVisibility;

	[SerializeField]
	private static float _maxLabelVisibility = 0.65f;

	private static bool shake;

	public TextAnimation[] textAnimations;

	public float startSize;

	[SerializeField]
	private string sound_RoundStart = string.Empty;

	[SerializeField]
	private string sound_RoundFightStart = string.Empty;

	[SerializeField]
	private string sound_RoundFightWin = string.Empty;

	[SerializeField]
	private string sound_RoundFightFailed = string.Empty;

	[SerializeField]
	private string sound_FightWin = string.Empty;

	[SerializeField]
	private string sound_FightLose = string.Empty;

	[SerializeField]
	private string sound_Perfect = string.Empty;

	[SerializeField]
	private string sound_Great = string.Empty;

	[SerializeField]
	private string sound_Timeout = string.Empty;

	protected override void Awake()
	{
		base.Awake();
		for (int i = 0; i < textAnimations.Length; i++)
		{
			textAnimations[i].Init();
		}
		AudioManager.Instance.LoadSound(sound_RoundStart, sound_RoundFightStart, sound_RoundFightWin, sound_RoundFightFailed, sound_FightWin, sound_FightLose, sound_Perfect, sound_Great, sound_Timeout);
		ConfigurableDialogModule.onDialogOpened = (ConfigurableDialogModule.DialogOpened)Delegate.Combine(ConfigurableDialogModule.onDialogOpened, new ConfigurableDialogModule.DialogOpened(onDialogOpened));
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		ConfigurableDialogModule.onDialogOpened = (ConfigurableDialogModule.DialogOpened)Delegate.Remove(ConfigurableDialogModule.onDialogOpened, new ConfigurableDialogModule.DialogOpened(onDialogOpened));
	}

	public void ShowText(string text, bool useFogging = false, UnityEngine.Color? textColor = null)
	{
		Clear();
		for (int i = 0; i < textAnimations.Length; i++)
		{
			if (!textAnimations[i].active)
			{
				textAnimations[i].SetText = text;
				textAnimations[i].Show(startSize * Vector3.one, Vector3.one, useFogging, textColor);
				break;
			}
		}
	}

	public static void ShakeCamera()
	{
		if (shake)
		{
			EffectsManager.ShakeCamera(shakeFrames, shakeAmplitude, shakePeriod);
		}
	}

	public void ShowPerfect()
	{
		ShowText("PERFECT!", false, perfectColor);
		PlaySoundAndShake(sound_Perfect, shakeCamera.perfect);
	}

	public void ShowGreat()
	{
		ShowText("GREAT!", false, greatColor);
		PlaySoundAndShake(sound_Great, shakeCamera.great);
	}

	public void ShowRoundEnd(RoundMessageData data)
	{
		if (data.RoundResult == ERoundResult.TIME_OUT)
		{
			string text = "TIMEOUT!";
			PlaySoundAndShake(sound_Timeout, shakeCamera.timeout);
			ShowText(text);
		}
	}

	public void ShowRoundEnd_PVP(RoundMessageData data)
	{
		string text;
		if (data.RoundResult == ERoundResult.TIME_OUT)
		{
			text = "TIMEOUT!";
			PlaySoundAndShake(sound_Timeout, shakeCamera.timeout);
		}
		else if (data.RoundResult == ERoundResult.PLAYER_WIN)
		{
			text = data.PlayerName + " WINS";
			PlaySoundAndShake(sound_RoundFightWin, shakeCamera.roundWin);
		}
		else
		{
			text = data.PlayerName + " WINS";
			PlaySoundAndShake(sound_RoundFightFailed, shakeCamera.roundLose);
		}
		ShowText(text);
	}

	public void ShowRoundStart(RoundMessageData data)
	{
		ShowText("ROUND " + data.RoundNumber.ToColored("FE762B"), true);
		PlaySoundAndShake(sound_RoundStart, shakeCamera.roundStart);
	}

	public void ShowFightStart()
	{
		ShowText("FIGHT!");
		PlaySoundAndShake(sound_RoundFightStart, shakeCamera.fightStart);
	}

	public void ShowFightEnd(RoundMessageData data)
	{
		if (data.BattleType == sf3DTO.BattleType.Survival)
		{
			ShowText(string.Format("ROUNDS WON: " + data.RoundsWon.ToColored("FE762B")));
			return;
		}
		sf3DTO.FightResult fightResultType = data.FightResultType;
		if (fightResultType == sf3DTO.FightResult.Win)
		{
			ShowText("YOU WIN!");
			PlaySoundAndShake(sound_FightWin, shakeCamera.fightEnd);
		}
		else
		{
			ShowText("YOU LOSE!");
		}
	}

	private void PlaySoundAndShake(string soundName, bool enable = false)
	{
		AudioManager.Instance.PlaySound(soundName);
		shake = enable;
	}

	public void Clear()
	{
		for (int i = 0; i < textAnimations.Length; i++)
		{
			if (textAnimations[i].active)
			{
				textAnimations[i].Hide();
			}
		}
	}

	private void onDialogOpened(DialogConfig config)
	{
		TextAnimation[] array = textAnimations;
		foreach (TextAnimation textAnimation in array)
		{
			if ((bool)textAnimation.label)
			{
				textAnimation.label.gameObject.SetActive(false);
			}
		}
	}
}
