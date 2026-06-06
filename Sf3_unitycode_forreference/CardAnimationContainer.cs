using System;
using UnityEngine;

[RequireComponent(typeof(TweenRotation))]
[RequireComponent(typeof(TweenPosition))]
public class CardAnimationContainer : MonoBehaviour
{
	[Serializable]
	public class Animation
	{
		public string name;

		public AnimationCurve rotationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		public AnimationCurve movementCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		public float duration = 1f;
	}

	public Animation[] animations;

	private TweenRotation _tweenRotation;

	private TweenPosition _tweenPosition;

	private bool _isInit;

	private Animation GetAnimationByName(string name)
	{
		Animation[] array = animations;
		foreach (Animation animation in array)
		{
			if (animation.name == name)
			{
				return animation;
			}
		}
		Debug.LogError("Animation not found");
		return null;
	}

	public void Move(Vector3 to, string animation)
	{
		Init();
		Animation animationByName = GetAnimationByName(animation);
		if (animationByName != null)
		{
			_tweenRotation.enabled = true;
			_tweenRotation.from = base.transform.localRotation.eulerAngles;
			_tweenRotation.to = Vector3.zero;
			_tweenRotation.animationCurve = animationByName.rotationCurve;
			_tweenRotation.duration = animationByName.duration;
			_tweenPosition.enabled = true;
			_tweenPosition.from = base.transform.localPosition;
			_tweenPosition.to = to;
			_tweenPosition.animationCurve = animationByName.movementCurve;
			_tweenPosition.duration = animationByName.duration;
			_tweenPosition.PlayForward();
			_tweenRotation.PlayForward();
		}
	}

	private void Init()
	{
		if (!_isInit)
		{
			_tweenPosition = GetComponent<TweenPosition>();
			_tweenRotation = GetComponent<TweenRotation>();
			if (_tweenPosition == null)
			{
				Debug.LogError("Tween position not found");
				return;
			}
			if (_tweenRotation == null)
			{
				Debug.LogError("Tween rotation not found");
				return;
			}
			_tweenPosition.enabled = false;
			_tweenRotation.enabled = false;
			_isInit = true;
		}
	}

	private void Start()
	{
		Init();
	}
}
