using System;
using Nekki.UI;
using SF3.UserData;
using UnityEngine;

public class ExpCircle : MonoBehaviour
{
	public delegate void AnimationEnd();

	[Serializable]
	private class CircleAnimation
	{
		[HideInInspector]
		public NekkiUISprite target;

		public AnimationEnd onAnimationEnd;

		public float time = 1f;

		private float _moveTo;

		private float _moveFrom;

		private float _startTime;

		private bool _isPlay;

		public void Start(float moveTo)
		{
			_startTime = Time.time;
			_moveTo = moveTo;
			_moveFrom = target.fillAmount;
			_isPlay = true;
		}

		public void Stop()
		{
			_isPlay = false;
		}

		public void Animate()
		{
			if (!_isPlay)
			{
				return;
			}
			float t = (Time.time - _startTime) / time;
			target.fillAmount = Mathf.Lerp(_moveFrom, _moveTo, t);
			if (target.fillAmount == _moveTo)
			{
				_isPlay = false;
				Debug.Log("Animation end");
				Debug.Log(onAnimationEnd);
				if (onAnimationEnd != null)
				{
					onAnimationEnd();
				}
			}
		}
	}

	[Serializable]
	private class NewLevelAnimation
	{
		[HideInInspector]
		public NekkiUILabel newLvl;

		public NekkiUILabel lvl;

		public AnimationEnd onAnimationEnd;

		public float time = 1f;

		private float _startTime;

		private bool _isPlay;

		public void Start()
		{
			_startTime = Time.time;
			_isPlay = true;
			newLvl.gameObject.SetActive(true);
			lvl.gameObject.SetActive(false);
		}

		public void Stop()
		{
			_isPlay = false;
		}

		public void Animate()
		{
			if (_isPlay && Time.time - _startTime > time)
			{
				_isPlay = false;
				newLvl.gameObject.SetActive(false);
				lvl.gameObject.SetActive(true);
				if (onAnimationEnd != null)
				{
					onAnimationEnd();
				}
			}
		}
	}

	public AnimationEnd onAnimationEnd;

	[SerializeField]
	private NekkiUISprite _circleProgress;

	[SerializeField]
	private NekkiUILabel _lvlLabel;

	[SerializeField]
	private NekkiUILabel _newLevelLabel;

	[SerializeField]
	private CircleAnimation _circleAnimation;

	[SerializeField]
	private NewLevelAnimation _newLevelAnimation;

	private long _exp;

	private int _lvl;

	private long _maxExp;

	private long _needAdd;

	public int lvl
	{
		get
		{
			return _lvl;
		}
		set
		{
			_lvl = value;
			_maxExp = UserManager.UserModelInfo.levelExperience;
			if (_lvlLabel != null)
			{
				_lvlLabel.text = value.ToString();
			}
		}
	}

	public long exp
	{
		get
		{
			return _exp;
		}
		set
		{
			_exp = value;
			if (_maxExp == 0)
			{
				Debug.LogError("NotSetLevel");
			}
			else
			{
				_circleProgress.fillAmount = (float)_exp / (float)_maxExp;
			}
		}
	}

	public void Init(int currentLevel, int currentExp)
	{
		lvl = currentLevel;
		exp = currentExp;
		_circleAnimation.target = _circleProgress;
		_circleAnimation.onAnimationEnd = StartNewLevelAnimation;
		_newLevelAnimation.lvl = _lvlLabel;
		_newLevelAnimation.newLvl = _newLevelLabel;
		_newLevelAnimation.onAnimationEnd = StartCircleAnimation;
	}

	public void AddExp(long addedExp)
	{
		if (addedExp == 0)
		{
			if (onAnimationEnd != null)
			{
				onAnimationEnd();
			}
		}
		else
		{
			_needAdd = _exp + addedExp;
			StartCircleAnimation();
		}
	}

	private void StartCircleAnimation()
	{
		if (_needAdd > _maxExp)
		{
			_needAdd -= _maxExp;
			_circleAnimation.Start(1f);
		}
		else
		{
			_circleAnimation.Start((float)_needAdd / (float)_maxExp);
			_needAdd = -1L;
		}
	}

	private void StartNewLevelAnimation()
	{
		if (_needAdd >= 0)
		{
			lvl++;
			exp = 0L;
			_newLevelAnimation.Start();
		}
		else if (onAnimationEnd != null)
		{
			onAnimationEnd();
		}
	}

	private void FixedUpdate()
	{
		_circleAnimation.Animate();
		_newLevelAnimation.Animate();
	}

	public void BreakAnimation()
	{
		_circleAnimation.Stop();
		_newLevelAnimation.Stop();
		_lvlLabel.gameObject.SetActive(true);
		_newLevelLabel.gameObject.SetActive(false);
		if (_needAdd <= 0)
		{
			return;
		}
		while (_needAdd > 0)
		{
			lvl++;
			if (_needAdd > _maxExp)
			{
				_needAdd -= _maxExp;
				continue;
			}
			exp = _needAdd;
			_needAdd = -1L;
		}
	}
}
