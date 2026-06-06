using DG.Tweening;
using UnityEngine;

namespace SF3.TutorialAnimations
{
	public class MovingFromCenterAnimation : Animation
	{
		private const float DURATION = 0.8f;

		private const float MOVE_DELAY = 0.5f;

		public MovingFromCenterAnimation(RectTransform owner, Vector2 offset)
			: base(owner, offset)
		{
		}

		public override void InAnimation()
		{
			Sequence sequence = DOTween.Sequence();
			Owner.GetComponent<CanvasGroup>().alpha = 0f;
			sequence.AppendCallback(delegate
			{
				SetStartPosition();
			});
			sequence.Append(Owner.GetComponent<CanvasGroup>().DOFade(1f, 0.8f));
			sequence.AppendInterval(0.5f);
			sequence.Append(Owner.DOAnchorPosY(Offset.y, 0.8f).SetEase(Ease.InOutCubic));
			sequence.Play();
		}

		public override void OutAnimation(TweenCallback callback)
		{
			Owner.GetComponent<CanvasGroup>().DOFade(0f, 0.8f).OnComplete(callback);
		}

		private void SetStartPosition()
		{
			RectTransform component = NekkiCanvasRoot.instance.Canvas.GetComponent<RectTransform>();
			float num = (component.sizeDelta.y - Owner.sizeDelta.y) / 2f;
			Owner.anchoredPosition = new Vector3(Owner.anchoredPosition.x, 0f - num);
		}
	}
}
