using UnityEngine;

[RequireComponent(typeof(UIWidget))]
public class ButtonSelectionNGUI : TutorialPointer
{
	[SerializeField]
	private UISprite _background;

	[SerializeField]
	private UISprite _innerSprite;

	[SerializeField]
	private UILabel _label;

	public Color startBackgroundColor;

	public Color endBackgroundColor;

	public Color startInnerColor;

	public Color endInnerColor;

	public override void Init(float duration)
	{
		if (!_init)
		{
			base.Init();
			AddColorTween(_background, startBackgroundColor, endBackgroundColor, duration);
			AddColorTween(_innerSprite, startInnerColor, endInnerColor, duration);
			AddColorTween(_label, startInnerColor, endInnerColor, duration);
		}
	}

	public void Set(UIWidget background, Vector2 borderPadding, UISprite icon, UILabel label)
	{
		int num = Mathf.RoundToInt(borderPadding.x);
		int num2 = Mathf.RoundToInt(borderPadding.y);
		_background.SetAnchor(background.gameObject, -num, -num2, num, num2);
		if (_innerSprite != null)
		{
			_innerSprite.gameObject.SetActive(icon != null);
			if (icon != null)
			{
				_innerSprite.atlas = icon.atlas;
				_innerSprite.spriteName = icon.spriteName;
				GeneralSettings(_innerSprite, icon);
			}
		}
		if (_label != null)
		{
			_label.gameObject.SetActive(label != null);
			if (label != null)
			{
				_label.bitmapFont = label.bitmapFont;
				_label.fontSize = label.fontSize;
				_label.text = label.text;
				GeneralSettings(_label, label);
			}
		}
	}

	private void GeneralSettings(UIWidget widget, UIWidget settings)
	{
		widget.pivot = settings.pivot;
		widget.SetAnchor(settings.gameObject, 0, 0, 0, 0);
	}
}
