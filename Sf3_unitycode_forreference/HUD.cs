using System;
using Nekki.UI;
using SF3;
using SF3.GameModels;
using UnityEngine;

public class HUD : UIModuleHolder
{
	[Serializable]
	public class ScoreUnit
	{
		[SerializeField]
		private NekkiUILabel _lblScore;

		private int _score;

		private float _scoreValue;

		public float _visualizationSpeed;

		public int Score
		{
			get
			{
				return _score;
			}
			set
			{
				if (value < _score)
				{
					Reset();
				}
				_score = value;
			}
		}

		public void Update()
		{
			if (!(_scoreValue > (float)_score))
			{
				_scoreValue += _visualizationSpeed * Time.deltaTime;
				if (_scoreValue > (float)_score)
				{
					_lblScore.text = _score + AdditionStringInfo();
				}
				else
				{
					_lblScore.text = (int)_scoreValue + AdditionStringInfo();
				}
			}
		}

		private string AdditionStringInfo()
		{
			return (FightController.Settings.ScoreCount == 0) ? string.Empty : string.Format(" / {0}", FightController.Settings.ScoreCount);
		}

		public void Reset()
		{
			_score = 0;
			_scoreValue = 0f;
			_lblScore.text = _score + AdditionStringInfo();
		}
	}

	[Serializable]
	public abstract class BaseSlideUnit
	{
		public enum DefaultStates
		{
			Empty = 0,
			Full = 1
		}

		[SerializeField]
		protected DefaultStates _defaultState;

		public string Name;

		protected float _scale;

		public abstract float Percent { get; set; }

		public void Init(float scale = 1f)
		{
			_scale = scale;
			Reset();
		}

		public virtual void Reset()
		{
			Percent = ((_defaultState == DefaultStates.Full) ? 1 : 0);
		}
	}

	[Serializable]
	public class SlideUnit : BaseSlideUnit
	{
		[SerializeField]
		private UIProgressBar _bar;

		private float _percent;

		public float Width
		{
			get
			{
				return NGUIMath.CalculateRelativeWidgetBounds(_bar.foregroundWidget.transform).size.x;
			}
		}

		public override float Percent
		{
			get
			{
				return _percent;
			}
			set
			{
				if ((bool)_bar)
				{
					_percent = value;
					_bar.value = _percent * _scale;
				}
			}
		}

		public override void Reset()
		{
			base.Reset();
			if ((bool)_bar && (bool)_bar.thumb)
			{
				_bar.thumb.gameObject.SetActive(_defaultState == DefaultStates.Full);
			}
		}

		public void HideThumb()
		{
			if ((bool)_bar && (bool)_bar.thumb)
			{
				_bar.thumb.gameObject.SetActive(false);
			}
		}

		public void ShowThumb()
		{
			if ((bool)_bar && (bool)_bar.thumb)
			{
				_bar.thumb.gameObject.SetActive(true);
			}
		}
	}

	[Serializable]
	public class ParticlesSlideUnit : BaseSlideUnit
	{
		public GameObject fullBarEffect;

		public ParticleSystem _particles;

		public float maxParticlesRate;

		public Transform scaleTransform;

		public override float Percent
		{
			get
			{
				return scaleTransform.localScale.x;
			}
			set
			{
				scaleTransform.localScale = new Vector3(value, scaleTransform.localScale.y, scaleTransform.localScale.x);
				ParticleSystem.EmissionModule emission = _particles.emission;
				emission.rateOverTime = new ParticleSystem.MinMaxCurve(maxParticlesRate * value);
			}
		}

		public override void Reset()
		{
			fullBarEffect.SetActive(false);
			base.Reset();
		}
	}

	private const float SCALE_FOR_3X4 = 0.75f;

	private bool _hpEnable;

	private bool _scoreEnable;

	[SerializeField]
	private GameObject _hpRoot;

	[SerializeField]
	private GameObject _scoreRoot;

	public Transform roundsUIParent;

	public GameObject roundSegmentPrefab;

	public float roundsUISpawnOffset;

	private UISprite[] roundsSprites;

	[SerializeField]
	private UITexture _avatar;

	[SerializeField]
	private NekkiUILabel _name16x9;

	[SerializeField]
	private NekkiUILabel _name4x3;

	[SerializeField]
	private SlideUnit _hpMain;

	[SerializeField]
	private SlideUnit _hpFall;

	[SerializeField]
	private SlideUnit _hpBg;

	[SerializeField]
	private ScoreUnit _score;

	[SerializeField]
	private GameObject _hpHolder;

	[SerializeField]
	private GameObject _shadowEnergyHolder;

	[SerializeField]
	private ParticlesSlideUnit _shadowEnergy;

	[SerializeField]
	private SlideUnit _shadowEnergySlide;

	[SerializeField]
	private UIAnchor _anchorName;

	private const float HP_UPADTE_TIME = 0.32f;

	private const float HP_FALL_UPDATE_TIME = 1f;

	private const float CRAZY_DECREASE_SPEED = 0.08f;

	private float _currentHP;

	private float _timeToHP;

	private float _currentSE;

	private float _hpUpdateSpeed;

	private float _hpFallUpdateSpeed;

	private SlideUnit _currentCrazySlide;

	private float _lastHitTime = -1f;

	public string Name
	{
		get
		{
			return currentNameLabel.text;
		}
		set
		{
			currentNameLabel.text = value;
		}
	}

	public NekkiUILabel currentNameLabel
	{
		get
		{
			if (CameraConfiguration.Current.aspect == CameraConfiguration.AspectRatio._16X9)
			{
				return _name16x9;
			}
			return _name4x3;
		}
	}

	public string Alias
	{
		set
		{
			currentNameLabel.Alias = value;
		}
	}

	public string Avatar
	{
		get
		{
			return _avatar.mainTexture.name;
		}
		set
		{
			Texture2D loadTexture2DInternal = GlobalLoad.GetLoadTexture2DInternal("avatars", value);
			if ((bool)loadTexture2DInternal)
			{
				_avatar.mainTexture = loadTexture2DInternal;
			}
		}
	}

	public float ShadowEnergy
	{
		get
		{
			return _currentSE;
		}
		set
		{
			value = Mathf.Clamp01(value);
			if (_currentSE != value)
			{
				_currentSE = value;
				_shadowEnergy.fullBarEffect.SetActive(_currentSE >= 0.99999f);
			}
		}
	}

	public float HP
	{
		get
		{
			return _hpMain.Percent;
		}
		set
		{
			value = Mathf.Clamp01(value);
			if (_currentHP != value)
			{
				_currentHP = value;
				_lastHitTime = GameTimeController.time;
				_hpUpdateSpeed = (_hpMain.Percent - _currentHP) / 0.32f;
				_hpFallUpdateSpeed = (_hpFall.Percent - _currentHP) / 1f;
				if (Math.Abs(value - _hpMain.Percent) > 0.01f)
				{
					_timeToHP = 1f;
				}
				else
				{
					_timeToHP = 0.02f;
				}
			}
		}
	}

	public int Score
	{
		get
		{
			return _score.Score;
		}
		set
		{
			_score.Score = value;
		}
	}

	public void SetAvatarTexture(Texture tex)
	{
		_avatar.mainTexture = tex;
		_avatar.MakePixelPerfect();
	}

	public void ColorRoundsWinned(int winned)
	{
		for (int i = 0; i < winned; i++)
		{
			roundsSprites[i].color = new Color(1f, 1f, 1f, 1f);
		}
	}

	public void SetupRoundUI(int number)
	{
		roundsSprites = new UISprite[number];
		for (int i = 0; i < number; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(roundSegmentPrefab);
			roundsSprites[i] = gameObject.GetComponent<UISprite>();
			gameObject.transform.parent = roundsUIParent;
			gameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
			gameObject.transform.localPosition = new Vector3(roundsUISpawnOffset * (float)i, 0f, 0f);
		}
	}

	internal void Start()
	{
		Init();
		if (CameraConfiguration.Current.aspect == CameraConfiguration.AspectRatio._16X9)
		{
			_name16x9.gameObject.SetActive(true);
			_name4x3.gameObject.SetActive(false);
		}
		else
		{
			_name16x9.gameObject.SetActive(false);
			_name4x3.gameObject.SetActive(true);
		}
	}

	private void Init()
	{
		ScoreBarEnable(false);
		HPBarEnable(true);
		ShadowFormEnable(true);
		if (CameraConfiguration.Current.aspect == CameraConfiguration.AspectRatio._16X9)
		{
			_hpFall.Init();
			_hpMain.Init();
			_hpBg.Init();
		}
		else
		{
			_hpFall.Init(0.75f);
			_hpMain.Init(0.75f);
			_hpBg.Init(0.75f);
		}
		_shadowEnergySlide.Init();
		_currentHP = HP;
	}

	public void ScoreBarEnable(bool isEnable)
	{
		_scoreEnable = isEnable;
		_scoreRoot.SetActive(_scoreEnable);
	}

	public void HPBarEnable(bool isEnable)
	{
		_hpEnable = isEnable;
		_hpHolder.SetActive(_hpEnable);
	}

	public void ShadowFormEnable(bool isEnable)
	{
		_shadowEnergyHolder.SetActive(isEnable);
	}

	public void Reset()
	{
		_hpFall.Reset();
		_hpMain.Reset();
		_hpBg.Reset();
		_score.Reset();
		_shadowEnergySlide.Reset();
		_currentHP = HP;
	}

	protected virtual void OnHit()
	{
	}

	internal void Update()
	{
		if (_hpEnable)
		{
			if (_hpMain.Percent - _currentHP > 0f)
			{
				_hpMain.Percent = Math.Max(0f, _hpMain.Percent - GameTimeController.unscaledDeltaTime * _hpUpdateSpeed);
			}
			else if (_hpMain.Percent - _currentHP < 0f)
			{
				_hpMain.Percent = _currentHP;
			}
			if (_hpMain.Percent <= 0f)
			{
				_hpMain.HideThumb();
			}
			if (_hpFall.Percent <= 0.8f)
			{
				_hpBg.Percent = _hpFall.Percent + 0.2f;
			}
			else
			{
				_hpBg.Percent = 1f;
			}
			if (_hpFall.Percent - _hpMain.Percent > 0f && _lastHitTime + _timeToHP < GameTimeController.time)
			{
				_hpFall.Percent = Math.Max(0f, _hpFall.Percent - GameTimeController.unscaledDeltaTime * _hpFallUpdateSpeed);
			}
			else if (_hpFall.Percent - _hpMain.Percent < 0f)
			{
				_hpFall.Percent = _hpMain.Percent;
			}
			if (_currentCrazySlide != null && _currentCrazySlide.Percent > 0f && _currentCrazySlide.Percent < 1f)
			{
				_currentCrazySlide.Percent = Math.Max(0f, _currentCrazySlide.Percent - GameTimeController.unscaledDeltaTime * 0.08f);
			}
		}
		if (_scoreEnable)
		{
			_score.Update();
		}
	}

	private void UpdateShadowEnergyBar(float value)
	{
		_shadowEnergySlide.Percent = value;
		_shadowEnergy.fullBarEffect.SetActive(value >= 1f);
	}

	public void SetModel(Model model)
	{
		if (model.isPlayer)
		{
			SetupRoundUI(FightController.Instance.CurrentFight.roundsToWin);
		}
		else
		{
			SetupRoundUI(FightController.Instance.CurrentFight.roundsToLose);
		}
		ShadowFormController.Instance.RegisterForEvent_SetShadowCharge(model.id, UpdateShadowEnergyBar);
	}
}
