using System;
using UnityEngine.UI;

namespace SF3.Utils
{
	public static class ImageExtensions
	{
		public const float UpperLimit = 1f;

		public const float LowerLimit = 0f;

		public static BehaviourTimer.SingleTimer CreateFillTimer(this Image image, int framesDuration, float fromAmount, float toAmount, Action onFinish = null)
		{
			image.fillAmount = fromAmount;
			float delta = ((!(fromAmount > toAmount)) ? (0f - 1f / (float)framesDuration) : (1f / (float)framesDuration));
			return BehaviourTimer.CreateGameFramesTimer(framesDuration, delegate
			{
				image.fillAmount -= delta;
			}, null, new Action<object>[1]
			{
				delegate
				{
					image.fillAmount = toAmount;
					onFinish.InvokeSafe();
				}
			});
		}

		public static BehaviourTimer.SingleTimer CreateFillTimerAscending(this Image image, int framesDuration)
		{
			return image.CreateFillTimer(framesDuration, 0f, 1f);
		}

		public static BehaviourTimer.SingleTimer CreateFillTimerDescending(this Image image, int framesDuration)
		{
			return image.CreateFillTimer(framesDuration, 1f, 0f);
		}

		public static BehaviourTimer.SingleTimer CreateFillTimerAscending(this Image image, int framesDuration, Action onFinish)
		{
			return image.CreateFillTimer(framesDuration, 0f, 1f, onFinish);
		}

		public static BehaviourTimer.SingleTimer CreateFillTimerDescending(this Image image, int framesDuration, Action onFinish)
		{
			return image.CreateFillTimer(framesDuration, 1f, 0f, onFinish);
		}
	}
}
