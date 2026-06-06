using System;
using System.Collections.Generic;
using SF3.GameModels;
using UnityEngine;
using sf3DTO;

namespace SF3.Moves
{
	[Serializable]
	public class ModelInfoAnimation
	{
		private Bone _originalPivotBone;

		private List<Bone> _pseudoPhysicsBones;

		public Model modelState { get; private set; }

		public InfoAnimation animation { get; private set; }

		public List<ModelIntervalAttack> modelAttackingIntervals { get; private set; }

		public MirrorerInfoContainer mirrorer { get; private set; }

		public int rootIndex { get; private set; }

		public int rootID { get; private set; }

		public int startFrame { get; private set; }

		public bool transitionApplied { get; private set; }

		private bool IsRight
		{
			get
			{
				Transform transform = ((!modelState.isPlayer) ? ModelsManager.Instance.Enemy.modelComponents.centerOfMassBone.transform : ModelsManager.Instance.Player.modelComponents.centerOfMassBone.transform);
				Transform transform2 = ((!modelState.isPlayer) ? ModelsManager.Instance.Player.modelComponents.centerOfMassBone.transform : ModelsManager.Instance.Enemy.modelComponents.centerOfMassBone.transform);
				return transform.position.x > transform2.position.x;
			}
		}

		public ModelInfoAnimation(InfoAnimation anim, Model modelStateValue)
		{
			modelState = modelStateValue;
			animation = anim;
			Gender gender = modelStateValue.GetGender();
			foreach (KeyValuePair<int, List<InfoAnimation.SoundsForFrame>> soundsForFrame in animation.soundsForFrames)
			{
				foreach (InfoAnimation.SoundsForFrame item in soundsForFrame.Value)
				{
					item.LoadSounds(LocationSettings.Instance.surfaceTypes, gender);
				}
			}
			InitIntervals(modelState);
			mirrorer = new MirrorerInfoContainer();
			rootIndex = -1;
			rootID = modelState.modelComponents.rootBone.boneID;
			_pseudoPhysicsBones = new List<Bone>();
			if (animation.physical)
			{
				return;
			}
			mirrorer.Init(modelState.modelComponents.mirrorBones, modelState.modelComponents, animation.animationBinary.bonesIDs);
			for (int i = 0; i < animation.animationBinary.bonesIDs.Length; i++)
			{
				if (rootID == animation.animationBinary.bonesIDs[i])
				{
					rootIndex = i;
				}
			}
			if (rootIndex < 0)
			{
				throw new Exception(string.Format("Model root bone ID is [{0}], but animation [{1}] binary havnt bone with this ID", modelState.modelComponents.rootBone.boneID, animation.name));
			}
			_originalPivotBone = modelState.GetBone(animation.align.alignPivot.partName);
			_pseudoPhysicsBones = new List<Bone>();
			foreach (Bone pseudoPhysicBone in modelStateValue.modelComponents.pseudoPhysicBones)
			{
				bool flag = false;
				int[] bonesIDs = animation.animationBinary.bonesIDs;
				foreach (int num in bonesIDs)
				{
					if (pseudoPhysicBone.boneID == num)
					{
						flag = true;
						break;
					}
				}
				if (!flag && pseudoPhysicBone.rigidBody != null)
				{
					_pseudoPhysicsBones.Add(pseudoPhysicBone);
				}
			}
		}

		private void InitIntervals(Model modelState)
		{
			modelAttackingIntervals = new List<ModelIntervalAttack>();
			foreach (IntervalAnimation interval in animation.intervals)
			{
				if (interval.type == EIntervalsType.INTERVAL_ATTACK)
				{
					ModelIntervalAttack item = new ModelIntervalAttack(modelState, (IntervalAttack)interval);
					modelAttackingIntervals.Add(item);
				}
			}
		}

		public void InitPlay(ActiveAnimation lastAnimation = null)
		{
			startFrame = animation.startFrame;
			transitionApplied = false;
			if (lastAnimation != null && (!modelState.IsAI || animation.aiTransistable))
			{
				AnimationTransition animationTransition = animation.TransitionExist(lastAnimation);
				if (animationTransition != null)
				{
					startFrame = lastAnimation.currentKey + animationTransition.frameShift;
					if (startFrame < animation.startFrame || startFrame > animation.endFrame)
					{
						startFrame = animation.startFrame;
					}
					transitionApplied = true;
				}
			}
			foreach (Bone pseudoPhysicsBone in _pseudoPhysicsBones)
			{
				pseudoPhysicsBone.rigidBody.isKinematic = false;
			}
		}

		public void InitFinishPlay()
		{
			foreach (Bone pseudoPhysicsBone in _pseudoPhysicsBones)
			{
				pseudoPhysicsBone.rigidBody.isKinematic = true;
			}
		}

		public Vector3 GetAnimationRotationOffset(bool mirrored)
		{
			Vector3 result = Vector3.zero;
			if (animation.align != null)
			{
				result = animation.align.rotation;
			}
			return result;
		}

		public Vector3 GetAnimationOffset(bool mirrored, EDirectionType moveDirection)
		{
			Vector3 zero = Vector3.zero;
			if (animation.align == null)
			{
				return zero;
			}
			modelState.modelComponents.SetPivotBone(_originalPivotBone.boneName, modelState.moveControl.mirrored);
			Vector3 position = animation.align.alignPosition.GetPosition(modelState, true);
			zero = GetOffset(position, moveDirection);
			Vector3 shift = animation.align.shift;
			if (moveDirection == EDirectionType.LEFT)
			{
				shift.x *= -1f;
			}
			return zero + shift;
		}

		private Vector3 GetOffset(Vector3 pivotPosition, EDirectionType moveDirection)
		{
			if (modelState.modelComponents.pivotBone == null)
			{
				return Vector3.zero;
			}
			Bone bone = _originalPivotBone;
			List<Matrix4x4> list = new List<Matrix4x4>();
			Matrix4x4 item = default(Matrix4x4);
			Vector3 one = Vector3.one;
			AnimatedTransform animatedTransform = new AnimatedTransform();
			do
			{
				bone = bone.parentBone;
				animation.CopyFrameTransformByID(startFrame, bone.boneID, animatedTransform);
				Quaternion quaternion = animatedTransform.rotation;
				Vector3 position = animatedTransform.position;
				one = bone.localScale;
				if (moveDirection == EDirectionType.RIGHT && bone.parentBone == null)
				{
					quaternion = Quaternion.AngleAxis(180f, Vector3.up) * quaternion;
					position.x = 0f - position.x;
				}
				item.SetTRS(position, quaternion, one);
				list.Add(item);
			}
			while (bone.parentBone != null);
			item = Matrix4x4.identity;
			for (int num = list.Count - 1; num >= 0; num--)
			{
				item *= list[num];
			}
			AnimatedTransform animatedTransform2 = new AnimatedTransform();
			animation.CopyFrameTransformByID(startFrame, _originalPivotBone.boneID, animatedTransform2);
			Vector4 vector = animatedTransform2.position;
			vector.w = 1f;
			vector = item * vector;
			Vector3 vector2 = (pivotPosition - modelState.modelComponents.armatureBone.position) / modelState.modelComponents.armatureBone.lossyScale.x;
			Vector3 result = vector2 - (Vector3)vector;
			if (!animation.align.axisX)
			{
				result.x = 0f;
			}
			if (!animation.align.axisY)
			{
				result.y = 0f;
			}
			if (!animation.align.axisZ)
			{
				result.z = 0f;
			}
			return result;
		}

		public Bone GetPivotBoneByParent()
		{
			Bone result = null;
			if (animation.align.alignPosition.playerType == EPlayerType.Parent)
			{
				result = modelState.parentModel.GetBone(animation.align.alignPosition.partName);
			}
			else if (animation.align.alignPosition.playerType == EPlayerType.Enemy)
			{
				result = modelState.enemy.GetBone(animation.align.alignPosition.partName);
			}
			return result;
		}

		public bool CheckSoundsForCurrentFrame(int frame)
		{
			if (animation.soundsForFrames.ContainsKey(frame))
			{
				foreach (InfoAnimation.SoundsForFrame item in animation.soundsForFrames[frame])
				{
					if (item.soundsForFrameCommon.frameSoundType == InfoAnimation.SoundsForFrame.EFrameSoundType.COMMON)
					{
						item.PlayRandomSound(string.Empty);
					}
					else if (item.soundsForFrameCommon.frameSoundType == InfoAnimation.SoundsForFrame.EFrameSoundType.GENDER)
					{
						item.PlayRandomSound(modelState.modelInfo.gender.ToString());
					}
					else if (item.soundsForFrameCommon.frameSoundType == InfoAnimation.SoundsForFrame.EFrameSoundType.FLOOR)
					{
						item.PlayRandomSound(LocationSettings.GetFloorSurface(modelState.modelComponents.centerOfMassBone.position.x).ToString());
					}
				}
				return true;
			}
			return false;
		}
	}
}
