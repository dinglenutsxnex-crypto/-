using UnityEngine;

[RequireComponent(typeof(UIProgressBar))]
public class ProgressBarAnimation : MonoBehaviour
{
	public delegate void AnimationEnd();

	public float duration = 1f;

	public float to;

	public AnimationEnd onAnimationEnd;

	private UIProgressBar progressBar;

	private bool animationPlaying;

	private float from;

	private float timeStart;

	public bool isPlay
	{
		get
		{
			return animationPlaying;
		}
	}

	private void Awake()
	{
		progressBar = GetComponent<UIProgressBar>();
		animationPlaying = false;
	}

	public void Play(float animateTo)
	{
		to = animateTo;
		Play();
	}

	[ContextMenu("Play")]
	public void Play()
	{
		animationPlaying = true;
		from = progressBar.value;
		timeStart = Time.time;
	}

	public void Stop(bool triggerEvent = true)
	{
		animationPlaying = false;
		if (onAnimationEnd != null && triggerEvent)
		{
			onAnimationEnd();
		}
	}

	public void Break()
	{
		progressBar.value = to;
		Stop();
	}

	private void Update()
	{
		if (animationPlaying)
		{
			float t = (Time.time - timeStart) / duration;
			progressBar.value = Mathf.Lerp(from, to, t);
			if (progressBar.value == to)
			{
				progressBar.value = to;
				Stop();
			}
		}
	}
}
