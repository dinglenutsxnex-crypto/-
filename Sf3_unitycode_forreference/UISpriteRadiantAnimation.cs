using UnityEngine;

public class UISpriteRadiantAnimation : MonoBehaviour
{
	private UIBasicSprite _sprite;

	private bool _animationActive;

	[SerializeField]
	private AnimationCurve _fillingCurve;

	[SerializeField]
	private float _fillTime = 1f;

	private float _currentFillTime;

	[SerializeField]
	private bool _normalLoop;

	[SerializeField]
	private bool _pingPong;

	[SerializeField]
	private bool _invertedLoop;

	[SerializeField]
	private bool _runOnEnable;

	private int _forward = 1;

	private bool _looped;

	private void Awake()
	{
		_sprite = GetComponent<UIBasicSprite>();
	}

	private void Update()
	{
		if (!_animationActive)
		{
			return;
		}
		_currentFillTime += Time.deltaTime * (float)_forward;
		if (_currentFillTime > _fillTime || _currentFillTime < 0f)
		{
			if (!_looped)
			{
				Stop();
				return;
			}
			if (_pingPong)
			{
				if (_forward == 1)
				{
					_currentFillTime = _fillTime;
					_forward = -1;
				}
				else
				{
					_currentFillTime = 0f;
					_forward = 1;
				}
			}
			else if (_normalLoop)
			{
				_currentFillTime = 0f;
				_forward = 1;
				_sprite.invert = false;
			}
			else if (_invertedLoop)
			{
				if (_forward == 1)
				{
					_currentFillTime = _fillTime;
					_forward = -1;
					_sprite.invert = true;
				}
				else
				{
					_currentFillTime = 0f;
					_forward = 1;
					_sprite.invert = false;
				}
			}
		}
		float time = _currentFillTime / _fillTime;
		_sprite.fillAmount = _fillingCurve.Evaluate(time);
	}

	private void OnEnable()
	{
		if (_runOnEnable)
		{
			Play();
		}
	}

	private void OnDisable()
	{
		Stop();
	}

	public void Play()
	{
		if (!(_sprite == null))
		{
			Reset();
			_looped = _normalLoop || _pingPong || _invertedLoop;
			_animationActive = true;
		}
	}

	public void Pause()
	{
		_animationActive = false;
	}

	public void Resume()
	{
		_animationActive = true;
	}

	public void Stop()
	{
		_animationActive = false;
		Reset();
	}

	private void Reset()
	{
		_forward = 1;
		_currentFillTime = 0f;
		_sprite.invert = false;
	}
}
