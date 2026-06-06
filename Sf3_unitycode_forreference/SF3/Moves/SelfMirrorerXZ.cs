using UnityEngine;

namespace SF3.Moves
{
	public class SelfMirrorerXZ : AnimationMirrorer
	{
		protected override void CalculateAndSet(MirrorIndexes index)
		{
			Vector3 boneStartRotation = GetBoneStartRotation(index.leftIndex);
			Vector3 animatedTransformRotation = GetAnimatedTransformRotation(index.leftIndex);
			animatedTransformRotation.x = boneStartRotation.x - (animatedTransformRotation.x - boneStartRotation.x);
			animatedTransformRotation.z = boneStartRotation.z - (animatedTransformRotation.z - boneStartRotation.z);
			SetAnimatedTransformsAnimateThisFrame(index.leftIndex, true);
			SetAnimatedTransformsRotation(index.leftIndex, animatedTransformRotation);
		}
	}
}
