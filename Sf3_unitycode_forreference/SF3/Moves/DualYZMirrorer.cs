using UnityEngine;

namespace SF3.Moves
{
	public class DualYZMirrorer : AnimationMirrorer
	{
		protected override Vector3 GetRotationDelta(int index)
		{
			return base.GetRotationDelta(index).Negate(VectorExtensions.Vector3ValueType.Y, VectorExtensions.Vector3ValueType.Z);
		}

		protected override Vector3 GetPositionDelta(int index)
		{
			return base.GetPositionDelta(index).Negate(default(VectorExtensions.Vector3ValueType));
		}
	}
}
