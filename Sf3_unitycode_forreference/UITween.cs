using System;
using UnityEngine;
using UnityEngine.UI;

public class UITween : MonoBehaviour
{
	public enum TweenTypes
	{
		Alpha = 0,
		Position = 1,
		Scale = 2
	}

	public enum TweenBehaviourTypes
	{
		Once = 0,
		Pinpong = 1
	}

	public enum Directions
	{
		Forward = 0,
		Backward = 1
	}

	public TweenTypes Type;

	public TweenBehaviourTypes Behaviour;

	public AnimationCurve Curve;

	public Vector3 From;

	public Vector3 To;

	public Color clFrom;

	public Color clTo;

	public float Duration;

	public bool PlayOnAwake;

	private Image _image;

	private float _time;

	private bool _isActive;

	private Directions _direction;

	private Action onFinished;

	private void Start()
	{
		_image = GetComponent<Image>();
		if (PlayOnAwake)
		{
			Play();
		}
	}

	public void Play()
	{
		Play(Behaviour);
	}

	public void Play(TweenBehaviourTypes type)
	{
		_time = 0f;
		_isActive = true;
		Behaviour = type;
	}

	public void Stop()
	{
		_isActive = false;
	}

	private void Update()
	{
		if (_isActive)
		{
			Go();
		}
	}

	private void Go()
	{
		if (_time < Duration)
		{
			_time += Time.deltaTime;
			Step((!((double)Math.Abs(_time) > 1E-05)) ? 0f : (_time / Duration));
			return;
		}
		if (Behaviour == TweenBehaviourTypes.Pinpong)
		{
			_time = 0f;
			_direction = ((_direction == Directions.Forward) ? Directions.Backward : Directions.Forward);
			return;
		}
		_isActive = false;
		if (onFinished != null)
		{
			onFinished();
		}
	}

	private void Step(float position)
	{
		position = Mathf.Clamp01(position);
		float t = Curve.Evaluate(position);
		switch (Type)
		{
		case TweenTypes.Alpha:
			switch (_direction)
			{
			case Directions.Forward:
				_image.color = Color.Lerp(clFrom, clTo, t);
				Debug.LogWarning(_image.color);
				break;
			case Directions.Backward:
				_image.color = Color.Lerp(clTo, clFrom, t);
				Debug.LogWarning(_image.color);
				break;
			}
			break;
		}
	}
}
