using System;
using System.Collections.Generic;
using SF3;
using SF3.Utils;
using UnityEngine;

public class SpriteAlphaAnimation : MonoBehaviour
{
	public enum EAnimationStyle
	{
		LOOP = 0,
		PING_PONG = 1,
		ONCE = 2
	}

	[SerializeField]
	private float _animationTime = 8f;

	[SerializeField]
	private AnimationCurve _animationCurve;

	[SerializeField]
	private EAnimationStyle _animationStyle;

	[SerializeField]
	private bool _activateOnStart = true;

	private float _timer;

	private float _alphaValue;

	private Color _matColor;

	[SerializeField]
	private bool _useUnitySprites = true;

	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	private SpriteRenderer[] _additionalSpriteRenderers;

	private List<SpriteRenderer> _resultSprites;

	private UISprite _uiSpriteRenderer;

	[SerializeField]
	private UIWidget[] _additionalUISpriteRenderers;

	private List<UIWidget> _resultUISprites;

	private EAnimationStyle _currentAnimationStyle;

	private bool _backAnimation;

	private bool _active;

	private float _currentAnimationTime;

	private void Awake()
	{
		_timer = 0f;
		_backAnimation = false;
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_resultSprites = new List<SpriteRenderer>();
		if (_spriteRenderer != null)
		{
			_resultSprites.Add(_spriteRenderer);
		}
		_resultSprites.AddRange(_additionalSpriteRenderers);
		_uiSpriteRenderer = GetComponent<UISprite>();
		_resultUISprites = new List<UIWidget>();
		if (_uiSpriteRenderer != null)
		{
			_resultUISprites.Add(_uiSpriteRenderer);
		}
		_resultUISprites.AddRange(_additionalUISpriteRenderers);
		if (_activateOnStart)
		{
			Activate(_animationTime);
		}
	}

	public void Disable(float byTimeSeconds)
	{
		try
		{
			if (!base.gameObject || !base.gameObject.activeSelf)
			{
				return;
			}
		}
		catch (Exception)
		{
			return;
		}
		_timer = 0f;
		_active = false;
		float currentAlpha = 1f;
		if (_useUnitySprites)
		{
			currentAlpha = _resultSprites[0].color.a;
		}
		else
		{
			currentAlpha = _resultUISprites[0].color.a;
		}
		BehaviourTimer.CreateSecondsTimer(byTimeSeconds, delegate(float progress)
		{
			_alphaValue = Mathf.Lerp(currentAlpha, 0f, progress);
			UpdateAlpha(_alphaValue);
		}, delegate
		{
		});
	}

	public void Disable(bool instantly = false)
	{
		if (instantly)
		{
			Disable(0f);
		}
		else
		{
			Disable(_animationTime);
		}
	}

	public void Activate(EAnimationStyle newAnimationStyle, float timeSeconds)
	{
		if (base.gameObject.activeSelf && (_resultSprites.Count != 0 || _resultUISprites.Count != 0))
		{
			_currentAnimationStyle = newAnimationStyle;
			_timer = 0f;
			_active = true;
			_currentAnimationTime = timeSeconds;
		}
	}

	public void Activate(float timeSeconds)
	{
		Activate(_animationStyle, timeSeconds);
	}

	public void Activate()
	{
		Activate(_animationStyle, _animationTime);
	}

	private void Update()
	{
		if (!_active)
		{
			return;
		}
		_timer += GameTimeController.deltaTimePaused;
		_alphaValue = _animationCurve.Evaluate((!_backAnimation) ? (_timer / _currentAnimationTime) : (1f - _timer / _currentAnimationTime));
		UpdateAlpha(_alphaValue);
		if (_timer >= _currentAnimationTime)
		{
			if (_currentAnimationStyle == EAnimationStyle.PING_PONG)
			{
				_backAnimation = !_backAnimation;
			}
			else if (_currentAnimationStyle == EAnimationStyle.LOOP)
			{
				_backAnimation = false;
			}
			else if (_currentAnimationStyle == EAnimationStyle.ONCE)
			{
				_backAnimation = false;
				_active = false;
			}
			_timer = 0f;
		}
	}

	private void UpdateAlpha(float value)
	{
		if (_useUnitySprites)
		{
			foreach (SpriteRenderer resultSprite in _resultSprites)
			{
				_matColor = resultSprite.color;
				_matColor.a = value;
				resultSprite.color = _matColor;
			}
			return;
		}
		foreach (UIWidget resultUISprite in _resultUISprites)
		{
			_matColor = resultUISprite.color;
			_matColor.a = value;
			resultUISprite.color = _matColor;
		}
	}
}
