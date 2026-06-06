using UnityEngine;

namespace SF3.Moves
{
	public class DualXZMirrorer : AnimationMirrorer
	{
		protected override Vector3 GetRotationDelta(int index)
		{
			return base.GetRotationDelta(index).Negate(VectorExtensions.Vector3ValueType.X, VectorExtensions.Vector3ValueType.Z);
		}
	}
}
