using System;
using UnityEngine;

public class MultiCardProgress : MonoBehaviour
{
	public MultiCardProgressAnimationEnd onAnimationEnd;

	[SerializeField]
	private UIProgressBar progressBar;

	[SerializeField]
	private UIProgressBar addedProgressBar;

	private ProgressBarAnimation progressAnim;

	private TweenAlpha addedProgressAnim;

	private float savedAddedProgress;

	private bool isPlay;

	private int repeatLvelsup;

	private int _levelups;

	public float progress
	{
		get
		{
			return progressBar.value;
		}
		protected set
		{
			progressBar.value = value;
		}
	}

	public float addedProgress
	{
		get
		{
			return addedProgressBar.value;
		}
		protected set
		{
			addedProgressBar.value = value;
		}
	}

	public int levelup
	{
		get
		{
			return _levelups;
		}
		protected set
		{
			_levelups = value;
			progressBar.gameObject.SetActive(_levelups == 0);
		}
	}

	private void Awake()
	{
		progressAnim = progressBar.gameObject.GetComponent<ProgressBarAnimation>();
		ProgressBarAnimation progressBarAnimation = progressAnim;
		progressBarAnimation.onAnimationEnd = (ProgressBarAnimation.AnimationEnd)Delegate.Combine(progressBarAnimation.onAnimationEnd, new ProgressBarAnimation.AnimationEnd(OnAnimationEnd));
		addedProgressAnim = addedProgressBar.gameObject.GetComponentInChildren<TweenAlpha>();
		addedProgressAnim.onFinished.Add(new EventDelegate(OnAddedAnimationEnd));
	}

	public void SetProgress(float current, float added, int levelupCount)
	{
		progress = current;
		addedProgress = added;
		levelup = levelupCount;
		savedAddedProgress = addedProgress;
		if (levelup > 0)
		{
			progressBar.gameObject.SetActive(true);
			addedProgress = 1f;
		}
	}

	public void AnimateProgress()
	{
		isPlay = true;
		repeatLvelsup = _levelups;
		if (repeatLvelsup == 0)
		{
			progressAnim.Play(savedAddedProgress);
			return;
		}
		progressBar.gameObject.SetActive(true);
		addedProgress = 1f;
		progressAnim.Play(1f);
	}

	private void OnAnimationEnd()
	{
		repeatLvelsup--;
		if (repeatLvelsup == 0)
		{
			progress = 0f;
			addedProgress = savedAddedProgress;
			progressAnim.Play(addedProgress);
			return;
		}
		if (repeatLvelsup > 0)
		{
			progress = 0f;
			progressAnim.Play(1f);
			return;
		}
		if (onAnimationEnd != null)
		{
			onAnimationEnd();
		}
		isPlay = false;
	}

	public void AnimateAddedProgress()
	{
		isPlay = true;
		repeatLvelsup = _levelups;
		if (repeatLvelsup == 0)
		{
			addedProgress = savedAddedProgress;
			addedProgressAnim.ResetToBeginning();
			addedProgressAnim.PlayForward();
		}
		else
		{
			progressBar.gameObject.SetActive(true);
			addedProgress = 1f;
			addedProgressAnim.ResetToBeginning();
			addedProgressAnim.PlayForward();
		}
	}

	private void OnAddedAnimationEnd()
	{
		isPlay = false;
	}

	public void BreakAnimation()
	{
		if (isPlay)
		{
			progressAnim.Stop(false);
			isPlay = false;
		}
	}
}
