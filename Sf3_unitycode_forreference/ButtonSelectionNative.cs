using UnityEngine;
using UnityEngine.UI;

public class ButtonSelectionNative : TutorialPointer
{
	[SerializeField]
	private Image _background;

	[SerializeField]
	private Image _icon;

	[SerializeField]
	private UnityEngine.UI.Text _label;

	public Color StartBackgroundColor;

	public Color EndBackgroundColor;

	public Color StartInnerColor;

	public Color EndInnerColor;

	public override void Init(float duration)
	{
		if (!_init)
		{
			base.Init();
			AddColorTween(_background, StartBackgroundColor, EndBackgroundColor, duration);
			AddColorTween(_icon, StartInnerColor, EndInnerColor, duration);
			AddColorTween(_label, StartInnerColor, EndInnerColor, duration);
		}
	}

	public void Set(RectTransform background, Image selectorIcon, UnityEngine.UI.Text selectorLabel)
	{
		SetBackground(background);
		SetIcon(selectorIcon);
		SetLabel(selectorLabel);
	}

	private void SetBackground(RectTransform background)
	{
		CopyRectTransform(background, _background.rectTransform);
	}

	private void SetIcon(Image selectorIcon)
	{
		if (!(_icon == null))
		{
			_icon.gameObject.SetActive(selectorIcon != null);
			if (selectorIcon != null)
			{
				CopyImageParams(selectorIcon, _icon);
				CopyRectTransform(selectorIcon.rectTransform, _icon.rectTransform);
			}
		}
	}

	protected override void ShowSelection()
	{
		_background.gameObject.SetActive(true);
		_icon.gameObject.SetActive(true);
		_label.gameObject.SetActive(true);
	}

	protected override void HideSelection()
	{
		_background.gameObject.SetActive(false);
		_icon.gameObject.SetActive(false);
		_label.gameObject.SetActive(false);
	}

	private void SetLabel(UnityEngine.UI.Text selectorLabel)
	{
		if (!(_label == null))
		{
			_label.gameObject.SetActive(selectorLabel != null);
			if (selectorLabel != null)
			{
				CopyTextParams(selectorLabel, _label);
				CopyRectTransform(selectorLabel.rectTransform, _label.rectTransform);
			}
		}
	}

	private void CopyRectTransform(RectTransform source, RectTransform destination)
	{
		Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(source);
		destination.sizeDelta = new Vector2(bounds.size.x, bounds.size.y);
		destination.position = source.position;
	}

	private void CopyTextParams(UnityEngine.UI.Text source, UnityEngine.UI.Text destination)
	{
		destination.font = source.font;
		destination.fontSize = source.fontSize;
		destination.fontStyle = source.fontStyle;
		destination.resizeTextForBestFit = source.resizeTextForBestFit;
		destination.resizeTextMaxSize = source.resizeTextMaxSize;
		destination.resizeTextMinSize = source.resizeTextMinSize;
		destination.alignment = source.alignment;
		destination.horizontalOverflow = source.horizontalOverflow;
		destination.verticalOverflow = source.verticalOverflow;
		destination.lineSpacing = source.lineSpacing;
		destination.text = source.text;
	}

	private void CopyImageParams(Image source, Image destination)
	{
		destination.sprite = source.sprite;
	}
}
