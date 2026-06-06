using System;
using System.Linq;
using UnityEngine;

public class ReelItemAnimation : MonoBehaviour, IReelItemAnimation
{
	[Serializable]
	private class Animation
	{
		public string name = string.Empty;

		public UITweener.Style style;

		public AnimationCurve rotationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		public AnimationCurve positionCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		public AnimationCurve scaleCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		public float duration = 1f;
	}

	[SerializeField]
	private Animation[] animations;

	private TweenRotation rotationTween;

	private TweenPosition positionTween;

	private TweenScale scaleTween;

	private TweenAlpha _alphaTween;

	private string currentAnimation;

	private ReelItem _item;

	private bool isPlay;

	public ReelItem item
	{
		get
		{
			return _item ?? (_item = GetComponent<ReelItem>());
		}
	}

	public event ReelItemAnimationEnd onAnimationEnd;

	private Animation GetAnimationByName(string animationName)
	{
		return (!string.IsNullOrEmpty(animationName)) ? animations.FirstOrDefault((Animation t) => animationName.Equals(t.name)) : null;
	}

	public void Animate(string animationName, Vector3 moveTo)
	{
		Animation animation = SetAnimation(animationName);
		if (animation != null)
		{
			RunPosition(animation, moveTo);
		}
	}

	public void Animate(string animationName, Vector3 moveTo, Vector3 rotateTo)
	{
		Animation animation = SetAnimation(animationName);
		if (animation != null)
		{
			RunPosition(animation, moveTo);
			RunRotation(animation, rotateTo);
		}
	}

	public void Animate(string animationName, Vector3 moveTo, Vector3 rotateTo, Vector3 scaleTo)
	{
		Animation animation = SetAnimation(animationName);
		if (animation != null)
		{
			RunPosition(animation, moveTo);
			RunRotation(animation, rotateTo);
			RunScale(animation, scaleTo);
		}
	}

	private Animation SetAnimation(string animationName)
	{
		Animation animationByName = GetAnimationByName(animationName);
		if (animationByName == null)
		{
			Debug.LogError("Animation not found");
			return null;
		}
		base.gameObject.SetActive(true);
		currentAnimation = animationName;
		isPlay = true;
		return animationByName;
	}

	private void RunRotation(Animation anim, Vector3 rotateTo)
	{
		rotationTween.enabled = true;
		rotationTween.from = NormolizeAngel(base.transform.localRotation.eulerAngles);
		rotationTween.to = rotateTo;
		rotationTween.duration = anim.duration;
		rotationTween.animationCurve = anim.rotationCurve;
		rotationTween.ResetToBeginning();
		rotationTween.style = anim.style;
		rotationTween.PlayForward();
	}

	private void RunScale(Animation anim, Vector3 scaleTo)
	{
		scaleTween.enabled = true;
		scaleTween.from = base.transform.localScale;
		scaleTween.to = scaleTo;
		scaleTween.duration = anim.duration;
		scaleTween.animationCurve = anim.scaleCurve;
		scaleTween.ResetToBeginning();
		scaleTween.style = anim.style;
		scaleTween.PlayForward();
	}

	private void RunPosition(Animation anim, Vector3 moveTo)
	{
		positionTween.enabled = true;
		positionTween.from = base.transform.localPosition;
		positionTween.to = moveTo;
		positionTween.duration = anim.duration;
		positionTween.animationCurve = anim.positionCurve;
		positionTween.ResetToBeginning();
		positionTween.style = anim.style;
		positionTween.PlayForward();
	}

	public void Stop()
	{
		if (positionTween != null && rotationTween != null)
		{
			positionTween.enabled = false;
			rotationTween.enabled = false;
			_item.gameObject.SetActive(true);
		}
	}

	private Vector3 NormolizeAngel(Vector3 angle)
	{
		Vector3 result = angle;
		if (angle.x > 180f)
		{
			result.x -= 360f;
		}
		if (angle.y > 180f)
		{
			result.y -= 360f;
		}
		if (angle.z > 180f)
		{
			result.z -= 360f;
		}
		return result;
	}

	private void Awake()
	{
		rotationTween = GetComponent<TweenRotation>();
		if (rotationTween == null)
		{
			Debug.LogError("Tween Rotation not found");
		}
		positionTween = GetComponent<TweenPosition>();
		if (positionTween == null)
		{
			Debug.LogError("Tween Position not found");
		}
		scaleTween = GetComponent<TweenScale>();
		if (scaleTween == null)
		{
			Debug.LogError("Tween Scale not found");
		}
		positionTween.onFinished.Add(new EventDelegate(AnimationEnd));
		_item = GetComponent<ReelItem>();
	}

	private void AnimationEnd()
	{
		string text = currentAnimation;
		currentAnimation = null;
		isPlay = false;
		UpdateDepth();
		if (this.onAnimationEnd != null)
		{
			this.onAnimationEnd(text, this);
		}
	}

	private void FixedUpdate()
	{
		if (isPlay)
		{
			UpdateDepth();
		}
	}

	private void UpdateDepth()
	{
		_item.UpdateDepth(9999 - (int)base.transform.localPosition.z);
	}
}
