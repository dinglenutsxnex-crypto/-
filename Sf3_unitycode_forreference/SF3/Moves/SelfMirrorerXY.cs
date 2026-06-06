using UnityEngine;

namespace SF3.Moves
{
	public class SelfMirrorerXY : AnimationMirrorer
	{
		protected override void CalculateAndSet(MirrorIndexes index)
		{
			Vector3 boneStartRotation = GetBoneStartRotation(index.leftIndex);
			Vector3 animatedTransformRotation = GetAnimatedTransformRotation(index.leftIndex);
			animatedTransformRotation.x = boneStartRotation.x - (animatedTransformRotation.x - boneStartRotation.x);
			animatedTransformRotation.y = boneStartRotation.y - (animatedTransformRotation.y - boneStartRotation.y);
			SetAnimatedTransformsAnimateThisFrame(index.leftIndex, true);
			SetAnimatedTransformsRotation(index.leftIndex, animatedTransformRotation);
		}
	}
}
