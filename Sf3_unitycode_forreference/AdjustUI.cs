using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class AdjustUI : MonoBehaviour
{
	public enum AnchorModes
	{
		None = 0,
		Pixel = 1,
		Relatively = 2
	}

	public enum ScaleModes
	{
		None = 0,
		BaseOfHeight = 1,
		BaseOnWidth = 2,
		BaseOnMinimum = 3,
		BaseOnMaximum = 4,
		Both = 5
	}

	public enum AnchorPivots
	{
		BottomLeft = 0,
		Bottom = 1,
		BottomRight = 2,
		TopLeft = 3,
		Top = 4,
		TopRight = 5,
		MiddleLeft = 6,
		Middle = 7,
		MiddleRight = 8
	}

	public AnchorModes AnchorMode;

	public AnchorPivots Pivot;

	public AnchorPivots AnchorPivot;

	public float OffsetX;

	public float OffsetY;

	public ScaleModes ScaleMode;

	public float ScaleX = 1f;

	public float ScaleY = 1f;

	public RectTransform Transform;

	public static Canvas Canvas;

	public bool ExecuteOnUpdate;

	private static float ScreenWidth
	{
		get
		{
			return Screen.width;
		}
	}

	private static float ScreenHeight
	{
		get
		{
			return Screen.height;
		}
	}

	private void Start()
	{
		Adjust();
	}

	private void OnEnabe()
	{
		Transform = GetComponent<RectTransform>();
		Canvas = GetComponentInParent<Canvas>();
		Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
	}

	private void Update()
	{
		if (ExecuteOnUpdate || Application.isEditor)
		{
			Adjust();
		}
	}

	private void Adjust()
	{
		if (!Transform || !Canvas)
		{
			OnEnabe();
		}
		Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		switch (ScaleMode)
		{
		case ScaleModes.None:
			Transform.localScale = new Vector3(1f, 1f, 1f);
			break;
		case ScaleModes.BaseOfHeight:
		{
			float num8 = ScreenHeight * ScaleY;
			Transform.localScale = new Vector3(num8 / Transform.rect.height, num8 / Transform.rect.height, 1f);
			break;
		}
		case ScaleModes.BaseOnWidth:
		{
			float num5 = ScreenWidth * ScaleX;
			Transform.localScale = new Vector3(num5 / Transform.rect.width, num5 / Transform.rect.width, 1f);
			break;
		}
		case ScaleModes.Both:
		{
			float num3 = ScreenWidth * ScaleX;
			float num4 = ScreenHeight * ScaleY;
			Transform.localScale = new Vector3(num3 / Transform.rect.width, num4 / Transform.rect.height, 1f);
			break;
		}
		case ScaleModes.BaseOnMaximum:
		{
			float num6 = ScreenWidth * ScaleX;
			float num7 = ScreenHeight * ScaleY;
			if (num6 > num7)
			{
				Transform.localScale = new Vector3(num6 / Transform.rect.width, num6 / Transform.rect.width, 1f);
			}
			else
			{
				Transform.localScale = new Vector3(num7 / Transform.rect.height, num7 / Transform.rect.height, 1f);
			}
			break;
		}
		case ScaleModes.BaseOnMinimum:
		{
			float num = ScreenWidth * ScaleX;
			float num2 = ScreenHeight * ScaleY;
			if (num < num2)
			{
				Transform.localScale = new Vector3(num / Transform.rect.width, num / Transform.rect.width, 1f);
			}
			else
			{
				Transform.localScale = new Vector3(num2 / Transform.rect.height, num2 / Transform.rect.height, 1f);
			}
			break;
		}
		}
		switch (Pivot)
		{
		case AnchorPivots.BottomLeft:
			Transform.pivot = new Vector2(0f, 0f);
			break;
		case AnchorPivots.Bottom:
			Transform.pivot = new Vector2(0.5f, 0f);
			break;
		case AnchorPivots.BottomRight:
			Transform.pivot = new Vector2(1f, 0f);
			break;
		case AnchorPivots.TopLeft:
			Transform.pivot = new Vector2(0f, 1f);
			break;
		case AnchorPivots.Top:
			Transform.pivot = new Vector2(0.5f, 1f);
			break;
		case AnchorPivots.TopRight:
			Transform.pivot = new Vector2(1f, 1f);
			break;
		case AnchorPivots.MiddleLeft:
			Transform.pivot = new Vector2(0f, 0.5f);
			break;
		case AnchorPivots.Middle:
			Transform.pivot = new Vector2(0.5f, 0.5f);
			break;
		case AnchorPivots.MiddleRight:
			Transform.pivot = new Vector2(1f, 0.5f);
			break;
		}
		AnchorModes anchorMode = AnchorMode;
		if (anchorMode != 0 && (anchorMode == AnchorModes.Pixel || anchorMode == AnchorModes.Relatively))
		{
			switch (AnchorPivot)
			{
			case AnchorPivots.BottomLeft:
				Transform.localPosition = new Vector3(0.5f * ScreenWidth * -1f, 0.5f * ScreenHeight * -1f, 0f);
				break;
			case AnchorPivots.Bottom:
				Transform.localPosition = new Vector3(0f, 0.5f * ScreenHeight * -1f, 0f);
				break;
			case AnchorPivots.BottomRight:
				Transform.localPosition = new Vector3(0.5f * ScreenWidth * 1f, 0.5f * ScreenHeight * -1f, 0f);
				break;
			case AnchorPivots.TopLeft:
				Transform.localPosition = new Vector3(0.5f * ScreenWidth * -1f, 0.5f * ScreenHeight, 0f);
				break;
			case AnchorPivots.Top:
				Transform.localPosition = new Vector3(0f, 0.5f * ScreenHeight, 0f);
				break;
			case AnchorPivots.TopRight:
				Transform.localPosition = new Vector3(0.5f * ScreenWidth * 1f, 0.5f * ScreenHeight, 0f);
				break;
			case AnchorPivots.MiddleLeft:
				Transform.localPosition = new Vector3(0.5f * ScreenWidth * -1f, 0f, 0f);
				break;
			case AnchorPivots.Middle:
				Transform.localPosition = new Vector3(0f, 0f, 0f);
				break;
			case AnchorPivots.MiddleRight:
				Transform.localPosition = new Vector3(0.5f * ScreenWidth * 1f, 0f, 0f);
				break;
			}
			if (AnchorMode == AnchorModes.Pixel)
			{
				Transform.localPosition += new Vector3(OffsetX, OffsetY);
			}
			else
			{
				Transform.localPosition += new Vector3(Transform.rect.width * OffsetX * Transform.localScale.x, Transform.rect.height * OffsetY * Transform.localScale.y);
			}
		}
	}
}
