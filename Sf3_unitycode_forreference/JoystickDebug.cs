using UnityEngine;

public class JoystickDebug : UIModuleHolder
{
	[SerializeField]
	private UILabel _dirrections;

	[SerializeField]
	private MultiTweenTransition _multiTween;

	[SerializeField]
	private UIButton _bt;

	private bool _active = true;

	public GameObject objToDisable;

	internal void Start()
	{
		_dirrections.text = string.Format("{0}\n{1}", Stick.Direction, Stick.Quadrant);
		GameController.Instance.addEventListener(0, Subscription);
		GameController.Instance.addEventListener(1, ResetController);
		_bt.onClick.Add(new EventDelegate(ShowHide));
	}

	private void Subscription(CallEventArgs callEventArgs)
	{
	}

	private void ResetController(CallEventArgs callEventArgs)
	{
	}

	private void OnButtonClick()
	{
		if (_active)
		{
			Hide();
		}
		else
		{
			Show();
		}
	}

	public void ShowHide()
	{
		_active = !_active;
		objToDisable.SetActive(_active);
	}

	private void Show()
	{
		_bt.normalSprite = "Debug_minimize";
		_multiTween.TweenIn();
		_active = true;
	}

	private void Hide()
	{
		_bt.normalSprite = "Debug_expand";
		_multiTween.TweenOut();
		_active = false;
	}
}
