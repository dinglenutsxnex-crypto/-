using System;
using SF3.Utils;
using UnityEngine;

public class PerkUnit : MonoBehaviour
{
	[SerializeField]
	private UISprite progress;

	[SerializeField]
	private UISprite background;

	[SerializeField]
	private UISprite whiteBack;

	private int _index;

	private float _size;

	private Vector3 _targetPosition;

	[Header("horizontal offset between perks in % of size")]
	[SerializeField]
	private float _delimeterSize;

	[SerializeField]
	private float _speed;

	[SerializeField]
	private UITweener[] _tween;

	public void Init(UIAtlas atlas, string color, string spriteName, int duration, Action<object> onDone, int index, float size, bool infinite, bool showExpiration, string actionName)
	{
		background.atlas = atlas;
		background.spriteName = spriteName;
		progress.atlas = atlas;
		progress.spriteName = spriteName;
		UpdateSize(size);
		UpdateIndex(index);
		if (!infinite)
		{
			BehaviourTimer.CreateGameFramesTimer(duration, (!showExpiration) ? null : new Action<float>(OnUpdate), this, onDone);
		}
		base.name = actionName;
		Color color2;
		if (ColorUtility.TryParseHtmlString(color, out color2))
		{
			whiteBack.color = color2;
		}
		AnimateShow();
	}

	public void UpdateIndex(int index)
	{
		_index = index;
		UpdateTargetPosition(false);
	}

	public void UpdateSize(float size)
	{
		_size = size;
		int num = Mathf.RoundToInt(size);
		background.width = num;
		background.height = num;
		progress.width = num;
		progress.height = num;
		whiteBack.width = num;
		whiteBack.height = num;
		UpdateTargetPosition(true);
	}

	private void UpdateTargetPosition(bool instant)
	{
		_targetPosition = new Vector3((_size + _size * (_delimeterSize / 100f)) * (float)_index, 0f, 0f);
		if (instant)
		{
			base.transform.localPosition = _targetPosition;
		}
	}

	private void OnUpdate(float timerProgress)
	{
		progress.fillAmount = timerProgress;
	}

	private void Update()
	{
		if (Vector3.Distance(base.transform.localPosition, _targetPosition) > 0.01f)
		{
			base.transform.localPosition = Vector3.LerpUnclamped(base.transform.localPosition, _targetPosition, Time.deltaTime * _speed);
		}
	}

	public void AnimateShow()
	{
		UITweener[] tween = _tween;
		foreach (UITweener uITweener in tween)
		{
			uITweener.PlayForward();
		}
	}

	public void AnimateHide(Action onDone)
	{
		if (!this)
		{
			onDone();
			return;
		}
		_tween[0].AddOnFinished(delegate
		{
			if ((bool)this)
			{
				onDone();
				UnityEngine.Object.Destroy(base.gameObject);
			}
		});
		UITweener[] tween = _tween;
		foreach (UITweener uITweener in tween)
		{
			uITweener.PlayReverse();
		}
	}
}
