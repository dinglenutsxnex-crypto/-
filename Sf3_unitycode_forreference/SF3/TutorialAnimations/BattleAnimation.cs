using DG.Tweening;
using UnityEngine;

namespace SF3.TutorialAnimations
{
	public class BattleAnimation : Animation
	{
		private const float DURATION = 0.25f;

		public BattleAnimation(RectTransform owner, Vector2 offset)
			: base(owner, offset)
		{
		}

		public override void InAnimation()
		{
			SetStartPosition();
			Owner.DOAnchorPosY(Offset.y, 0.25f);
		}

		public override void OutAnimation(TweenCallback callback)
		{
			Owner.GetComponent<CanvasGroup>().DOFade(0f, 0.25f).OnComplete(callback);
		}

		private void SetStartPosition()
		{
			Vector2 anchoredPosition = Owner.anchoredPosition + Offset;
			if (Owner.pivot.y == 1f)
			{
				anchoredPosition.y += Owner.sizeDelta.y;
			}
			else
			{
				anchoredPosition.y -= Owner.sizeDelta.y;
			}
			Owner.anchoredPosition = anchoredPosition;
		}
	}
}
