using System.Collections.Generic;
using SF3.Moves;

namespace SF3.GameModels
{
	public interface IBonesHolder
	{
		Bone GetBone(string nameBone);

		Bone GetBone(int idBone);

		int GetBonesCount();

		int[] GetBonesIDs();

		void FillAnimatedTransforms(ref ModelAnimation.BlendAnimatedTransforms result);

		void UpdateBonesPositions(Dictionary<int, AnimatedTransform> bonesAnimationTransforms);
	}
}
