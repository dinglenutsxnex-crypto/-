using System.Collections.Generic;
using SF3.GameModels;
using UnityEngine;

namespace SF3.Moves
{
	public abstract class AnimationMirrorer
	{
		private readonly List<MirrorIndexes> _listOfIndexes = new List<MirrorIndexes>();

		private Dictionary<int, AnimatedTransform> _targetAnimatedTransforms;

		private IBonesHolder _bonesholder;

		public void AddMirrorIndexes(int newIndexRight, int newIndexLeft = -1)
		{
			if (newIndexLeft == -1)
			{
				newIndexLeft = newIndexRight;
			}
			_listOfIndexes.Add(new MirrorIndexes(newIndexRight, newIndexLeft));
		}

		public void CrutchMirror(Dictionary<int, AnimatedTransform> targetAnimatedTransforms, IBonesHolder bonesHolder)
		{
			_targetAnimatedTransforms = targetAnimatedTransforms;
			_bonesholder = bonesHolder;
			foreach (MirrorIndexes listOfIndex in _listOfIndexes)
			{
				if (targetAnimatedTransforms.ContainsKey(listOfIndex.rightIndex))
				{
					CalculateAndSet(listOfIndex);
				}
			}
		}

		protected virtual void CalculateAndSet(MirrorIndexes indexes)
		{
			Vector3 resultPosition = GetResultPosition(indexes.rightIndex, indexes.leftIndex);
			Vector3 resultPosition2 = GetResultPosition(indexes.leftIndex, indexes.rightIndex);
			Vector3 resultRotation = GetResultRotation(indexes.rightIndex, indexes.leftIndex);
			Vector3 resultRotation2 = GetResultRotation(indexes.leftIndex, indexes.rightIndex);
			SetAnimatedTransformsAnimateThisFrame(indexes.rightIndex, true);
			SetAnimatedTransformsAnimateThisFrame(indexes.leftIndex, true);
			SetAnimatedTransformsRotation(indexes.rightIndex, resultRotation);
			SetAnimatedTransformsRotation(indexes.leftIndex, resultRotation2);
			SetAnimatedTransformsPosition(indexes.rightIndex, resultPosition);
			SetAnimatedTransformsPosition(indexes.leftIndex, resultPosition2);
		}

		private Bone GetBone(int index)
		{
			return _bonesholder.GetBone(index);
		}

		protected Vector3 GetBoneStartPosition(int index)
		{
			return GetBone(index).startPosition;
		}

		protected Vector3 GetBoneStartRotation(int index)
		{
			return GetBone(index).startRotation;
		}

		protected Vector3 GetAnimatedTransformRotation(int index)
		{
			return _targetAnimatedTransforms[index].rotation.eulerAngles;
		}

		private Vector3 GetAnimatedTransformPosition(int index)
		{
			return _targetAnimatedTransforms[index].position;
		}

		protected void SetAnimatedTransformsAnimateThisFrame(int index, bool enable)
		{
			_targetAnimatedTransforms[index].animateThisFrame = enable;
		}

		protected void SetAnimatedTransformsRotation(int index, Vector3 boneRotation)
		{
			_targetAnimatedTransforms[index].SetRotation(Quaternion.Euler(boneRotation));
		}

		protected void SetAnimatedTransformsPosition(int index, Vector3 bonePosition)
		{
			_targetAnimatedTransforms[index].SetPosition(bonePosition);
		}

		protected virtual Vector3 GetPositionDelta(int index)
		{
			return GetAnimatedTransformPosition(index) - GetBoneStartPosition(index);
		}

		protected virtual Vector3 GetRotationDelta(int index)
		{
			return GetAnimatedTransformRotation(index) - GetBoneStartRotation(index);
		}

		private Vector3 GetResultRotation(int startRotationBoneIndex, int deltaRotationBoneIndex)
		{
			return GetBoneStartRotation(startRotationBoneIndex) + GetRotationDelta(deltaRotationBoneIndex);
		}

		private Vector3 GetResultPosition(int startPositionBoneIndex, int deltaPositionBoneIndex)
		{
			return GetBoneStartPosition(startPositionBoneIndex) + GetPositionDelta(deltaPositionBoneIndex).Negate();
		}
	}
}
