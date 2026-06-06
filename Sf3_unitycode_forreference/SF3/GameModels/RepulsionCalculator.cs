using UnityEngine;

namespace SF3.GameModels
{
	public class RepulsionCalculator
	{
		private const float MIN_SHIFT = 0.1f;

		private const float MAX_SHIFT = 10f;

		private const float OFFSET_LIMIT = 100f;

		public float Calculate(float comA0_x, float comB0_x, float comA1_x, float comB1_x)
		{
			float f = comA1_x - comA0_x;
			float f2 = comB1_x - comB0_x;
			float num = (Mathf.Abs(f) + Mathf.Abs(f2)) / 2f;
			float num2 = 0f;
			float num3 = Mathf.Abs(comA1_x - comB1_x);
			if (num3 + 2f * num < 100f)
			{
				num2 = 1f - num3 / 100f;
			}
			return Mathf.Clamp(num + num2, 0.1f, 10f);
		}
	}
}
