using UnityEngine;
using UnityEngine.UI;

public class TutorialArrowNative : TutorialPointer
{
	public float Offset;

	public float Amplitude;

	public AnimationCurve AnimCurve;

	public Color StartColor;

	public Color EndColor;

	[SerializeField]
	private Image _image;

	private RectTransform _rectTransf;

	public override void Init(float duration)
	{
		if (!_init)
		{
			base.Init();
			_rectTransf = GetComponent<RectTransform>();
			AddPositionTween(_image.transform, Amplitude, duration, AnimCurve);
			AddColorTween(_image, StartColor, EndColor, duration);
		}
	}

	public void SetPosition(RectTransform target, ArrowPosition position)
	{
		RectTransform component = NekkiUIRoot.Instance.canvas.gameObject.GetComponent<RectTransform>();
		Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(target);
		switch (position)
		{
		case ArrowPosition.Bottom:
		{
			float x = target.position.y - bounds.extents.y * component.localScale.y - Offset;
			SetRelativePosition(target.position.x, x, 0f);
			break;
		}
		case ArrowPosition.Top:
		{
			float x = target.position.y + bounds.extents.y * component.localScale.y + Offset;
			SetRelativePosition(target.position.x, x, 180f);
			break;
		}
		case ArrowPosition.Left:
		{
			float x = target.position.x - bounds.extents.x * component.localScale.x - Offset;
			SetRelativePosition(x, target.position.y, -90f);
			break;
		}
		case ArrowPosition.Right:
		{
			float x = target.position.x + bounds.extents.x * component.localScale.x + Offset;
			SetRelativePosition(x, target.position.y, 90f);
			break;
		}
		}
	}

	private void SetRelativePosition(float x, float y, float angle)
	{
		_rectTransf.localRotation = Quaternion.Euler(0f, 0f, angle);
		_rectTransf.position = new Vector3(x, y, _rectTransf.position.z);
	}

	protected override void ShowSelection()
	{
		_image.gameObject.SetActive(true);
	}

	protected override void HideSelection()
	{
		_image.gameObject.SetActive(false);
	}
}
