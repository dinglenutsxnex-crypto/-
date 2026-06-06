using UnityEngine;

public class TutorialArrowNGUI : TutorialPointer
{
	public float offset;

	public float amplitude;

	public AnimationCurve animCurve;

	public Color startColor;

	public Color endColor;

	private Transform _transf;

	private UIWidget _widget;

	private UISprite _sprite;

	public override void Init(float duration)
	{
		if (!_init)
		{
			base.Init();
			_transf = GetComponent<Transform>();
			_widget = GetComponent<UIWidget>();
			Transform child = _transf.GetChild(0);
			_sprite = child.GetComponent<UISprite>();
			AddPositionTween(child, amplitude, duration, animCurve);
			AddColorTween(_sprite, startColor, endColor, duration);
		}
	}

	public void SetPosition(UIWidget targetWidget, ArrowPosition position)
	{
		_widget.SetAnchor(targetWidget.gameObject, 0, 0, 0, 0);
		switch (position)
		{
		case ArrowPosition.Bottom:
			SetRelativePosition(true, 0f, 0f - offset - (float)_sprite.height, 0f - offset, 0f);
			break;
		case ArrowPosition.Top:
			SetRelativePosition(true, 1f, offset, offset + (float)_sprite.height, 180f);
			break;
		case ArrowPosition.Left:
			SetRelativePosition(false, 0f, 0f - offset, 0f - offset - (float)_sprite.height, -90f);
			break;
		case ArrowPosition.Right:
			SetRelativePosition(false, 1f, offset + (float)_sprite.height, offset, 90f);
			break;
		}
	}

	private void SetRelativePosition(bool vertical, float relative, float offset1, float offset2, float angle)
	{
		_transf.localRotation = Quaternion.Euler(0f, 0f, angle);
		if (vertical)
		{
			_widget.bottomAnchor.Set(relative, offset1);
			_widget.topAnchor.Set(relative, offset2);
		}
		else
		{
			_widget.leftAnchor.Set(relative, offset1);
			_widget.rightAnchor.Set(relative, offset2);
		}
	}
}
