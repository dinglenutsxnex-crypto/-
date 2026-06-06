using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

namespace SF3.TutorialAnimations
{
	public class Animation
	{
		protected RectTransform Owner;

		protected Vector2 Offset;

		public Animation([CanBeNull] RectTransform owner, Vector2 offset)
		{
			Owner = owner;
			Offset = offset;
		}

		public virtual void InAnimation()
		{
			Owner.localPosition = new Vector3(Owner.localPosition.x + Offset.x, Owner.localPosition.y + Offset.y, Owner.localPosition.z);
		}

		public virtual void OutAnimation(TweenCallback callback)
		{
			callback();
		}
	}
}
