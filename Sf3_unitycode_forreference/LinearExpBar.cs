using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DOTweenUtils;
using Jint.Native;
using Nekki.UI;
using UnityEngine;

public class LinearExpBar : MonoBehaviour, IAnimatedExpBar
{
	public delegate void AnimationEndDelegate();

	private long _addedExp;

	[Header("Progressbar")]
	[SerializeField]
	private UIProgressBar _progressBar;

	[SerializeField]
	private UILabel _nextLevelLbl;

	[SerializeField]
	private UILabel _currentLevelLbl;

	[SerializeField]
	private float _progressDelay = 0.4f;

	[SerializeField]
	private UISprite _hallo;

	[Header("Given exp")]
	[SerializeField]
	private GameObject _givenExpObj;

	[SerializeField]
	private NekkiUILabel _givenExplbl;

	[Header("New level lbl")]
	[SerializeField]
	private GameObject _newLevelObj;

	[SerializeField]
	private NekkiUILabel _levelLbl;

	private UIWidget _widget;

	private TweenAlpha _mainInAnimation;

	private TweenScale _mainOutAnimation;

	private TweenScale _nextLevelBounceAnimation;

	private TweenPosition _givenExpInAnimation;

	private TweenScale _newLevelInAnimation;

	private ProgressBarAnimation _progressBarAnimation;

	private long _currentLevel;

	private long _fromExp;

	private long _maxExp;

	private long _needExp;

	private long _animTo;

	private bool _hasNewLevel;

	private int _halloWidth;

	private int _halloHeight;

	private const int HALLO_WIDTH = 0;

	private const int HALLO_HEIGHT = 300;

	public long CurrentLevel
	{
		get
		{
			return _currentLevel;
		}
		set
		{
			_currentLevel = value;
			_currentLevelLbl.text = _currentLevel.ToString();
			_nextLevelLbl.text = (_currentLevel + 1).ToString();
			_levelLbl.text = _currentLevelLbl.text;
			Dictionary<string, JsValue> dictionary = JsFunction.CalculateNewLevel(_currentLevel, 0L, 0L);
			_maxExp = long.Parse(dictionary["LevelExperience"].ToString());
		}
	}

	public long FromExp
	{
		get
		{
			return _fromExp;
		}
		set
		{
			_fromExp = value;
			if (_maxExp > 0)
			{
				_progressBar.value = (float)_fromExp / (float)_maxExp;
			}
		}
	}

	public long AddedExp
	{
		get
		{
			return _addedExp;
		}
		set
		{
			_addedExp = value;
			_needExp = FromExp + _addedExp;
			_givenExplbl.text = value + "xp";
		}
	}

	public event ExpBarAnimationEnd onAnimationEnd;

	private void Awake()
	{
		Init();
	}

	private void Update()
	{
		if (_hallo.gameObject.activeSelf)
		{
			UpdateHalloPosition();
		}
	}

	private void Init()
	{
		_mainInAnimation = GetComponent<TweenAlpha>();
		_mainOutAnimation = GetComponent<TweenScale>();
		_nextLevelBounceAnimation = _nextLevelLbl.gameObject.GetComponent<TweenScale>();
		_givenExpInAnimation = _givenExpObj.GetComponent<TweenPosition>();
		_newLevelInAnimation = _newLevelObj.GetComponent<TweenScale>();
		_progressBarAnimation = _progressBar.gameObject.GetComponent<ProgressBarAnimation>();
		_mainInAnimation.onFinished.Add(new EventDelegate(StartExpInAnimation));
		_givenExpInAnimation.onFinished.Add(new EventDelegate(StartProgressAnimation));
		ProgressBarAnimation progressBarAnimation = _progressBarAnimation;
		progressBarAnimation.onAnimationEnd = (ProgressBarAnimation.AnimationEnd)Delegate.Combine(progressBarAnimation.onAnimationEnd, new ProgressBarAnimation.AnimationEnd(AfterProgressAnimation));
		_nextLevelBounceAnimation.onFinished.Add(new EventDelegate(AfterBounceAnimation));
		_mainOutAnimation.onFinished.Add(new EventDelegate(StartNewLevelAnimation));
		_newLevelInAnimation.onFinished.Add(new EventDelegate(AnimationEnd));
		_widget = GetComponent<UIWidget>();
		_halloWidth = _hallo.width;
		_halloHeight = _hallo.height;
		_hallo.gameObject.SetActive(false);
	}

	public void AnimateExp()
	{
		if (FromExp > _needExp)
		{
			Debug.LogError("Argument 'to' less then 'currentExp'");
			return;
		}
		_hasNewLevel = false;
		_givenExpObj.SetActive(false);
		_progressBar.gameObject.SetActive(true);
		_newLevelObj.SetActive(false);
		_widget.alpha = 0f;
		_animTo = AddedExp;
		StartMainInAnimation();
	}

	public void BreakAnimation()
	{
		StopAllAnimation();
		CalculateNewLevel();
		_givenExpObj.SetActive(true);
		base.transform.localScale = Vector3.one;
		_progressBar.gameObject.SetActive(!_hasNewLevel);
		_givenExpObj.gameObject.SetActive(!_hasNewLevel);
		_newLevelObj.SetActive(_hasNewLevel);
	}

	private void StartMainInAnimation()
	{
		_mainInAnimation.PlayForward();
	}

	private void StartExpInAnimation()
	{
		if (_needExp == FromExp)
		{
			AnimationEnd();
		}
		else if (_animTo == 0)
		{
			StartProgressAnimation();
		}
		else
		{
			StartCoroutine("StartExpInAnimationCorutine");
		}
	}

	private IEnumerator StartExpInAnimationCorutine()
	{
		yield return new WaitForSeconds(_progressDelay);
		_givenExpObj.SetActive(true);
		_givenExpInAnimation.PlayForward();
	}

	private void StartProgressAnimation()
	{
		if (_needExp < 0)
		{
			return;
		}
		_hallo.gameObject.SetActive(true);
		if (_needExp >= _maxExp && _maxExp > 0)
		{
			_needExp -= _maxExp;
			_progressBarAnimation.Play(1f);
			_hasNewLevel = true;
			return;
		}
		if (_maxExp > 0)
		{
			_progressBarAnimation.Play((float)_needExp / (float)_maxExp);
		}
		_needExp = -1L;
	}

	private void AfterProgressAnimation()
	{
		if (_needExp >= 0)
		{
			HalloEndAnimation(0.5f);
			StartNextLevelBounceAnimation();
		}
		else if (_needExp < 0 && _hasNewLevel)
		{
			StartMainOutAnimation();
		}
		else
		{
			AnimationEnd();
		}
	}

	private void StartNextLevelBounceAnimation()
	{
		_nextLevelBounceAnimation.PlayForward();
	}

	private void AfterBounceAnimation()
	{
		CurrentLevel++;
		_nextLevelBounceAnimation.ResetToBeginning();
		if (_needExp == 0 && !_hasNewLevel)
		{
			StartMainOutAnimation();
			return;
		}
		FromExp = 0L;
		StartProgressAnimation();
	}

	private void StartMainOutAnimation()
	{
		_mainOutAnimation.PlayForward();
	}

	private void StartNewLevelAnimation()
	{
		_progressBar.gameObject.SetActive(false);
		_givenExpObj.gameObject.SetActive(false);
		_newLevelObj.SetActive(true);
		base.transform.localScale = Vector3.one;
		_newLevelInAnimation.PlayForward();
	}

	private void AnimationEnd()
	{
		_hallo.gameObject.SetActive(false);
		if (this.onAnimationEnd != null)
		{
			this.onAnimationEnd();
		}
	}

	private void StopAllAnimation()
	{
		_mainInAnimation.enabled = false;
		_mainInAnimation.value = _mainInAnimation.to;
		_givenExpInAnimation.enabled = false;
		_givenExpInAnimation.value = _givenExpInAnimation.to;
		_progressBarAnimation.onAnimationEnd = AfterProgressAnimation;
		_progressBarAnimation.Break();
		_nextLevelBounceAnimation.enabled = false;
		_nextLevelBounceAnimation.value = _nextLevelBounceAnimation.from;
		_mainOutAnimation.enabled = false;
		_mainOutAnimation.value = _mainOutAnimation.to;
		_newLevelInAnimation.enabled = false;
		_newLevelInAnimation.value = _newLevelInAnimation.to;
		AnimationEnd();
	}

	private void CalculateNewLevel()
	{
		while (_needExp > 0)
		{
			if (_needExp >= _maxExp && _maxExp > 0)
			{
				_needExp -= _maxExp;
				CurrentLevel++;
				_hasNewLevel = true;
			}
			else
			{
				FromExp = _needExp;
				_needExp = -1L;
			}
		}
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
	}

	private void HalloEndAnimation(float duration)
	{
		if (!(_hallo.alpha < 1f))
		{
			_hallo.gameObject.SetActive(true);
			DONgui.Fade(_hallo, 0f, duration).OnComplete(delegate
			{
				_hallo.alpha = 1f;
			});
			DOTween.To(() => _hallo.width, delegate(int x)
			{
				_hallo.width = x;
			}, 0, duration).OnComplete(delegate
			{
				_hallo.width = _halloWidth;
			});
			DOTween.To(() => _hallo.height, delegate(int x)
			{
				_hallo.height = x;
			}, 300, duration).OnComplete(delegate
			{
				_hallo.height = _halloHeight;
			});
		}
	}

	private void UpdateHalloPosition()
	{
		float num = _progressBar.transform.localPosition.x - (float)(_progressBar.backgroundWidget.width / 2);
		float num2 = _progressBar.value * (float)_progressBar.backgroundWidget.width;
		_hallo.gameObject.transform.localPosition = new Vector3(num + num2, _progressBar.transform.localPosition.y);
	}

	private void OnDestroy()
	{
		ProgressBarAnimation progressBarAnimation = _progressBarAnimation;
		progressBarAnimation.onAnimationEnd = (ProgressBarAnimation.AnimationEnd)Delegate.Remove(progressBarAnimation.onAnimationEnd, new ProgressBarAnimation.AnimationEnd(AfterProgressAnimation));
	}
}
