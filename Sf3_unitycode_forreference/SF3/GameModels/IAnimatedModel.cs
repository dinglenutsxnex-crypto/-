using System.Collections.Generic;
using SF3.Moves;
using UnityEngine;

namespace SF3.GameModels
{
	public interface IAnimatedModel
	{
		bool IsAI { get; }

		int[] GetBonesIDs();

		Bone GetBone(string nameBone);

		void SetRagdollActive(bool isActive);

		string GetName();

		void FillAnimatedTransforms(ref ModelAnimation.BlendAnimatedTransforms result);

		void UpdateBonesPositions(Dictionary<int, AnimatedTransform> bonesAnimationTransforms);

		ModelInfoAnimation GetModelAnimationByName(string name);

		EDirectionType GetMoveDirection();

		bool GetMirrored();

		void CheckMirrored(int? forceValue);

		void SetMoveDirection(EDirectionType directionVal);

		void ResetInterval();

		void AddForce(Vector3 impulse, string capsuleName);

		void ActivateRagdolAlignment(bool zImpulse);

		bool GetIsPhysics();

		void RemoveModelVariable(string varName);

		void AddAttackingCapsules(int intervalID, List<Capsule> capsules);

		void ClearAttackingCapsules();

		void ClearAttackingCapsules(int intervalID);

		void ShowSkins(bool isShow);

		void SwitchWeapon(bool toMirror);

		string GetYDirection();

		int GetDirectionSign();

		Bone GetPivotBone();

		void ResetBonesPosition();

		void EnableAttakingCollisions(bool isEnable);

		void EnableRepulsionCollisions(bool isEnable);

		void SetRagdollSleepState(bool isSleep, int priority);

		List<IntervalAttack> getLastIntervalAttack();
	}
}
