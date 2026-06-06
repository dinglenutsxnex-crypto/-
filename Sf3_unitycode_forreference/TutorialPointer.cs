using System.Collections.Generic;
using DG.Tweening;
using DOTweenUtils;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPointer : MonoBehaviour
{
	private List<Tweener> _tweeners = new List<Tweener>();

	protected bool _init;

	public virtual void Init(float duration = 0f)
	{
		PauseWindow.OnPauseDisabled += ShowSelection;
		PauseWindow.OnPauseEnabled += HideSelection;
		_init = true;
	}

	private void OnEnable()
	{
		foreach (Tweener tweener in _tweeners)
		{
			tweener.Restart();
		}
	}

	protected virtual void ShowSelection()
	{
	}

	protected virtual void HideSelection()
	{
	}

	protected void AddPositionTween(Transform transf, float amplitude, float duration, AnimationCurve curve)
	{
		Tweener item = transf.DOLocalMove(new Vector3(0f, amplitude, 0f), duration).SetEase(curve).SetLoops(int.MaxValue, LoopType.Yoyo)
			.SetUpdate(true);
		_tweeners.Add(item);
	}

	protected void AddColorTween(UIWidget widget, Color startColor, Color endColor, float duration)
	{
		if (!(widget == null))
		{
			widget.color = startColor;
			Tweener item = DONgui.ColorTween(widget, endColor, duration).SetEase(Ease.Linear).SetLoops(int.MaxValue, LoopType.Yoyo)
				.SetUpdate(true);
			_tweeners.Add(item);
		}
	}

	protected void AddColorTween(Graphic graphic, Color startColor, Color endColor, float duration)
	{
		if (!(graphic == null))
		{
			graphic.color = startColor;
			Tweener item = graphic.DOColor(endColor, duration).SetEase(Ease.Linear).SetLoops(int.MaxValue, LoopType.Yoyo)
				.SetUpdate(true);
			_tweeners.Add(item);
		}
	}

	private void OnDestroy()
	{
		PauseWindow.OnPauseDisabled -= ShowSelection;
		PauseWindow.OnPauseEnabled -= HideSelection;
	}
}
