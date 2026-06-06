using Nekki.UI;
using UnityEngine;

public class RewardCoin : MonoBehaviour
{
	[SerializeField]
	private NekkiUILabel _name;

	[SerializeField]
	private UILabel _coinsValue;

	[SerializeField]
	private UILabel _bonusValue;

	private TweenAlpha _tweenAlpha;

	private TweenPosition _tweenPosition;

	public EventDelegate.Callback onFinish
	{
		set
		{
			_tweenPosition.onFinished.Add(new EventDelegate(value));
		}
	}

	private void Awake()
	{
		_tweenPosition = GetComponent<TweenPosition>();
		if (_tweenPosition == null)
		{
			Debug.LogError("Missing position tween.");
		}
		_tweenAlpha = GetComponent<TweenAlpha>();
		if (_tweenAlpha == null)
		{
			Debug.LogError("Missing alpha tween.");
		}
	}

	public void SetCoins(string name, string value, int count)
	{
		_name.text = string.Concat(Localization.Get(name), " x", count.ToString());
		_coinsValue.text = value;
		SetAnimation();
	}

	public void Set(string name, string coinsValue, string bonusValue)
	{
		_name.text = Localization.Get(name);
		_coinsValue.text = coinsValue;
		_bonusValue.text = bonusValue;
		SetAnimation();
	}

	private void SetAnimation()
	{
		_tweenPosition.from = base.transform.localPosition;
		_tweenPosition.to = new Vector3(0f, base.transform.localPosition.y, 0f);
	}

	public void Animate()
	{
		_tweenAlpha.enabled = true;
		_tweenPosition.enabled = true;
		_tweenAlpha.PlayForward();
		_tweenPosition.PlayForward();
	}

	public void BreakAnimate()
	{
		_tweenAlpha.enabled = false;
		_tweenPosition.enabled = false;
		_tweenAlpha.value = 1f;
		_tweenPosition.value = _tweenPosition.to;
	}
}
