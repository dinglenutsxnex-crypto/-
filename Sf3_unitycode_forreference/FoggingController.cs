using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FoggingController : MonoBehaviour
{
	[SerializeField]
	private Image _background;

	[SerializeField]
	private float _fadeDuration = 1f;

	[SerializeField]
	private float _fadeDelay;

	[SerializeField]
	private float _opacity = 0.5f;

	private static FoggingController _instance;

	public static FoggingController Instance
	{
		get
		{
			if (_instance == null)
			{
				NekkiUIRootModules.Instance.MountNativeModule("Fogging");
			}
			return _instance;
		}
	}

	public bool Active
	{
		get
		{
			return _background.gameObject.activeSelf;
		}
	}

	private void Awake()
	{
		_instance = this;
		_background.color = new Color(0f, 0f, 0f, 0f);
		_background.gameObject.SetActive(false);
	}

	private void TweenBackground(float value, float duration = 0f, float delay = 0f, Action onDone = null)
	{
		if (duration > 0f || delay > 0f)
		{
			_background.DOFade(value, duration).SetDelay(delay).OnComplete(delegate
			{
				onDone.InvokeSafe();
			});
		}
		else
		{
			_background.color = new Color(_background.color.r, _background.color.g, _background.color.b, value);
			onDone.InvokeSafe();
		}
	}

	public void ShowFogging()
	{
		_background.gameObject.SetActive(true);
		_instance.TweenBackground(_opacity, _fadeDuration, _fadeDelay);
	}

	public void HideFogging()
	{
		if (_background.enabled)
		{
			_instance.TweenBackground(0f, _fadeDuration, _fadeDelay, delegate
			{
				_background.gameObject.SetActive(false);
			});
		}
	}

	public void ShowFogging(float value, float duration, float delay, Action onDone = null)
	{
		_instance.TweenBackground(value, duration, delay, onDone);
	}

	public void ChangeOpacity(float to)
	{
		_opacity = to;
		_background.DOFade(_opacity, 0.5f);
	}
}
