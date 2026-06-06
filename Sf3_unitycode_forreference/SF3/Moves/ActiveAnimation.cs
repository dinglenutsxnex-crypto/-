using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SF3.GameModels;
using UnityEngine;

namespace SF3.Moves
{
	[Serializable]
	public class ActiveAnimation
	{
		public const int FRAMES_LEFT_FOR_END = 4;

		private float _delta;

		private int _currentKey;

		private float _currentKeyDelta;

		private bool _isMirrored;

		private EDirectionType _moveDirection;

		private int _nextIntervalStart;

		private int _nextIntervalFinish;

		private Vector3 _offsetPosition;

		private Vector3 _offsetRotation;

		private int _stopedFrame;

		private bool _zeroFrame;

		private bool _checkWeaponSwitchFrame;

		private int _switchFrameIndex;

		private bool _startLooped;

		private int _startLoopedFrame;

		private bool _isMain;

		private List<int> _dontPlaySoundsThisFrames;

		private ModelAnimation.BlendAnimatedTransforms _blendAnimatedTransforms;

		private Vector3 _lastFollowPositionObject;

		private Quaternion quaternionForBackward = Quaternion.AngleAxis(180f, Vector3.up);

		private AnimatedTransform currentInterpolationFrame = new AnimatedTransform();

		private AnimatedTransform nextInterpolationFrame = new AnimatedTransform();

		private Dictionary<int, IntervalAnimation> currentFixedIntervals = new Dictionary<int, IntervalAnimation>();

		public float delta
		{
			get
			{
				return _delta;
			}
		}

		public int currentKey
		{
			get
			{
				return _currentKey;
			}
		}

		public bool isMirrored
		{
			get
			{
				return _isMirrored;
			}
		}

		public float currentWeight { get; private set; }

		public bool isEnding { get; private set; }

		public List<IntervalAnimation> currentIntervals { get; private set; }

		public ModelInfoAnimation modelInfoAnim { get; private set; }

		public bool selfEnded { get; private set; }

		public Vector3 getVelocity
		{
			get
			{
				return modelInfoAnim.animation.velocty;
			}
		}

		public Vector3 getRotation
		{
			get
			{
				return modelInfoAnim.animation.rotation;
			}
		}

		public int FramesLeft
		{
			get
			{
				return modelInfoAnim.animation.endFrame - _currentKey;
			}
		}

		public event Action<ActiveAnimation> onAnimationStop = delegate
		{
		};

		public event Action<ActiveAnimation> onAnimationEnd = delegate
		{
		};

		public event Action<IntervalAnimation> onIntervalStart = delegate
		{
		};

		public event Action<IntervalAnimation> onIntervalEnd = delegate
		{
		};

		public event Action onZeroFrame = delegate
		{
		};

		public event Action<bool> onNeedWeaponSwitch;

		public ActiveAnimation()
		{
			_zeroFrame = true;
			_isMain = true;
			selfEnded = false;
			_dontPlaySoundsThisFrames = new List<int>();
			currentIntervals = new List<IntervalAnimation>();
			_currentKeyDelta = 0f;
			_nextIntervalStart = 0;
			_nextIntervalFinish = 99999;
			currentWeight = 0f;
			isEnding = false;
			_startLooped = false;
			_startLoopedFrame = 0;
		}

		public ActiveAnimation(ModelInfoAnimation anima, EDirectionType moveDirectionVal, bool isMirroredVal, Quaternion defaultRootRotation, ref ModelAnimation.BlendAnimatedTransforms activeTranfsValue)
			: this()
		{
			foreach (KeyValuePair<int, AnimatedTransform> animatedTransform in activeTranfsValue.animatedTransforms)
			{
				animatedTransform.Value.animateThisFrame = false;
			}
			modelInfoAnim = anima;
			_blendAnimatedTransforms = activeTranfsValue;
			_isMirrored = isMirroredVal;
			_moveDirection = moveDirectionVal;
			if (!modelInfoAnim.animation.physical)
			{
				anima.modelState.modelComponents.rootBone.SetLocalRotation(defaultRootRotation);
				_offsetPosition = modelInfoAnim.GetAnimationOffset(_isMirrored, _moveDirection);
				_offsetRotation = modelInfoAnim.GetAnimationRotationOffset(_isMirrored);
				_checkWeaponSwitchFrame = false;
				_switchFrameIndex = 0;
				if (modelInfoAnim.animation.weaponSwitchFrames.Count > 0)
				{
					_checkWeaponSwitchFrame = true;
				}
				_lastFollowPositionObject = Vector3.zero;
				if (modelInfoAnim.animation.align.followPositionObject)
				{
					Bone pivotBoneByParent = modelInfoAnim.GetPivotBoneByParent();
					_lastFollowPositionObject = pivotBoneByParent.position;
				}
			}
			else
			{
				_offsetPosition = (_offsetRotation = Vector3.zero);
			}
			_delta = modelInfoAnim.startFrame;
			_currentKey = Mathf.FloorToInt(_delta);
		}

		public void ApplyDuringInterval(Action<IntervalAnimation> activateAttackColliders, List<IntervalAttack> lastIntervals)
		{
			foreach (IntervalAnimation interval in modelInfoAnim.animation.intervals)
			{
				if (interval.start <= _currentKey && interval.finish >= _currentKey && (lastIntervals.Count == 0 || !lastIntervals.Any((IntervalAttack a) => a.start == interval.start && a.finish == interval.finish)))
				{
					currentIntervals.Add(interval);
					if (interval.type == EIntervalsType.INTERVAL_ATTACK)
					{
						activateAttackColliders(interval);
					}
				}
			}
		}

		private void StartLoopPlay()
		{
			EndActiveIntervals(modelInfoAnim.modelState);
			ClearActiveIntervals(modelInfoAnim.modelState);
			_currentKeyDelta = 0f;
			_nextIntervalStart = 0;
			_nextIntervalFinish = 99999;
			_delta = modelInfoAnim.animation.startFrame;
			_currentKey = Mathf.FloorToInt(_delta);
			_startLooped = false;
			_startLoopedFrame = 0;
			selfEnded = false;
		}

		public void ShiftPosition(Vector3 vector)
		{
			if (!modelInfoAnim.animation.physical)
			{
				_offsetPosition += vector;
			}
			else
			{
				_offsetPosition = vector;
			}
		}

		public void ShiftRotation(Vector3 vector)
		{
			if (!modelInfoAnim.animation.physical)
			{
				_offsetRotation += vector;
			}
			else
			{
				_offsetRotation = vector;
			}
		}

		public void CheckSoundsForCurrentFrame()
		{
			if (!_dontPlaySoundsThisFrames.Contains(_currentKey) && modelInfoAnim.CheckSoundsForCurrentFrame(_currentKey))
			{
				_dontPlaySoundsThisFrames.Add(_currentKey);
			}
		}

		public bool Update(float engineDeltaTime)
		{
			if (_startLooped && _delta >= (float)_startLoopedFrame)
			{
				StartLoopPlay();
			}
			_delta += engineDeltaTime / (float)modelInfoAnim.animation.midFrames;
			_currentKey = Mathf.FloorToInt(_delta);
			_currentKeyDelta = _delta - (float)_currentKey;
			if (_zeroFrame)
			{
				_zeroFrame = false;
				if (this.onZeroFrame != null)
				{
					this.onZeroFrame();
				}
			}
			if (isEnding)
			{
				if (_delta > (float)_stopedFrame)
				{
					ForceStop();
					return true;
				}
			}
			else if (FramesLeft <= 4)
			{
				if (!modelInfoAnim.animation.looped)
				{
					selfEnded = true;
					EndActiveIntervals(modelInfoAnim.modelState);
					this.onAnimationEnd(this);
				}
				else
				{
					_startLooped = true;
					_startLoopedFrame = modelInfoAnim.animation.endFrame - modelInfoAnim.animation.blendingFrames;
				}
			}
			CalculateWeight();
			CheckSoundsForCurrentFrame();
			if (!modelInfoAnim.animation.physical)
			{
				if (_currentKeyDelta > 0f && _delta < (float)modelInfoAnim.animation.endFrame)
				{
					Interpolate(_currentKeyDelta);
				}
				else
				{
					modelInfoAnim.animation.CopyFrameTransforms(_currentKey, _blendAnimatedTransforms.animatedTransforms);
				}
				AddBackward();
				if (_isMirrored)
				{
					MirrorAnimation();
				}
			}
			ApplyOffsets();
			if (isEnding && !_isMain)
			{
				return false;
			}
			CheckIntervals();
			if (_checkWeaponSwitchFrame)
			{
				SwitchFrame switchFrame = modelInfoAnim.animation.weaponSwitchFrames[_switchFrameIndex];
				if (_currentKey >= switchFrame.Frame)
				{
					this.onNeedWeaponSwitch(switchFrame.IsMirror);
					_switchFrameIndex++;
					if (_switchFrameIndex >= modelInfoAnim.animation.weaponSwitchFrames.Count)
					{
						_checkWeaponSwitchFrame = false;
					}
				}
			}
			return false;
		}

		public void ForceStop()
		{
			_blendAnimatedTransforms.currentWeight = -1f;
			this.onAnimationStop(this);
		}

		public void CalculateStopedFrame(int blendFramesCount = 4)
		{
			if (modelInfoAnim != null)
			{
				_stopedFrame = _currentKey + blendFramesCount;
				if (_stopedFrame > modelInfoAnim.animation.endFrame)
				{
					_stopedFrame = modelInfoAnim.animation.endFrame;
				}
			}
		}

		public void Stop(IAnimatedModel bonesHolder, ref Quaternion rootRotation)
		{
			if (modelInfoAnim != null)
			{
				isEnding = true;
				if (modelInfoAnim.animation.physical)
				{
					bonesHolder.FillAnimatedTransforms(ref _blendAnimatedTransforms);
				}
				else
				{
					rootRotation = Quaternion.Euler(_blendAnimatedTransforms.animatedTransforms[modelInfoAnim.rootID].rotation.eulerAngles - _offsetRotation);
				}
			}
		}

		private void CalculateWeight()
		{
			if (currentWeight < 1f)
			{
				currentWeight = Mathf.Min(_delta / (float)modelInfoAnim.animation.blendingFrames, 1f);
				_blendAnimatedTransforms.currentWeight = currentWeight;
			}
		}

		private void CheckIntervals()
		{
			bool flag = false;
			if (_currentKey > _nextIntervalFinish)
			{
				for (int i = 0; i < currentIntervals.Count; i++)
				{
					if (_currentKey > currentIntervals[i].finish)
					{
						IntervalAnimation obj = currentIntervals[i];
						currentIntervals.RemoveAt(i);
						this.onIntervalEnd(obj);
						i--;
					}
				}
				flag = true;
			}
			if (_currentKey >= _nextIntervalStart)
			{
				foreach (IntervalAnimation interval in modelInfoAnim.animation.intervals)
				{
					if (interval.start == _nextIntervalStart)
					{
						currentIntervals.Add(interval);
						this.onIntervalStart(interval);
					}
				}
				_nextIntervalStart = modelInfoAnim.animation.GetNextStartIntervalKey(_currentKey);
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			foreach (IntervalAnimation currentInterval in currentIntervals)
			{
				if (currentInterval.finish < _nextIntervalFinish)
				{
					_nextIntervalFinish = currentInterval.finish;
				}
			}
		}

		public void EndActiveIntervals(Model modelState)
		{
			currentIntervals.ForEach(delegate(IntervalAnimation intVal)
			{
				this.onIntervalEnd(intVal);
			});
			modelState.modelComponents.ClearAttackingCapsules();
		}

		public void ClearActiveIntervals(IAnimatedModel animModel)
		{
			if (modelInfoAnim != null)
			{
				currentIntervals.Clear();
				animModel.ClearAttackingCapsules();
			}
		}

		private void AddBackward()
		{
			AnimatedTransform animatedTransform = _blendAnimatedTransforms.animatedTransforms[modelInfoAnim.rootID];
			Vector3 position = default(Vector3);
			if (_moveDirection == EDirectionType.RIGHT)
			{
				animatedTransform.SetRotation(quaternionForBackward * animatedTransform.rotation);
				position.x = 0f - animatedTransform.position.x;
				if (!modelInfoAnim.modelState.moveControl.mirrored)
				{
					position.z = 0f - animatedTransform.position.z;
				}
				else
				{
					position.z = animatedTransform.position.z;
				}
			}
			else
			{
				position.x = animatedTransform.position.x;
				if (modelInfoAnim.modelState.moveControl.mirrored)
				{
					position.z = 0f - animatedTransform.position.z;
				}
				else
				{
					position.z = animatedTransform.position.z;
				}
			}
			position.y = animatedTransform.position.y;
			animatedTransform.SetPosition(position);
		}

		private void ApplyOffsets()
		{
			if (!modelInfoAnim.animation.physical)
			{
				AnimatedTransform animatedTransform = _blendAnimatedTransforms.animatedTransforms[modelInfoAnim.rootID];
				Vector3 position = animatedTransform.position;
				if (modelInfoAnim.animation.align.followPositionObject)
				{
					Bone pivotBoneByParent = modelInfoAnim.GetPivotBoneByParent();
					_offsetPosition += pivotBoneByParent.position - _lastFollowPositionObject;
					_lastFollowPositionObject = pivotBoneByParent.position;
				}
				position += _offsetPosition;
				if (modelInfoAnim.modelState.moveControl.mirrored == !_isMirrored)
				{
					position.z *= -1f;
				}
				animatedTransform.SetPosition(position);
				animatedTransform.SetRotation(Quaternion.Euler(animatedTransform.rotation.eulerAngles + _offsetRotation));
			}
			else
			{
				modelInfoAnim.modelState.modelComponents.rootBone.ShiftPosition(_offsetPosition);
				modelInfoAnim.modelState.modelComponents.rootBone.SetShiftRotation(_offsetRotation);
			}
		}

		private void MirrorAnimation()
		{
			modelInfoAnim.mirrorer.Mirror(_blendAnimatedTransforms.animatedTransforms);
		}

		private void Interpolate(float weight)
		{
			if (modelInfoAnim.animation.interpolation == InterpolationType.linear)
			{
				if (modelInfoAnim.animation.animationBinary.animationTangents == null)
				{
					LinearInterpolation(weight);
				}
				else
				{
					TangentCubicInterpolation(weight);
				}
			}
			else
			{
				Debug.LogError(string.Format("Havnt [{0}] interpolation!!", modelInfoAnim.animation.interpolation));
			}
		}

		private void LinearInterpolation(float weight)
		{
			int frameNumber = _currentKey + 1;
			for (int i = 0; i < modelInfoAnim.animation.animationBinary.bonesCount; i++)
			{
				if (_blendAnimatedTransforms.animatedTransforms.ContainsKey(modelInfoAnim.animation.animationBinary.bonesIDs[i]))
				{
					AnimatedTransform animatedTransform = _blendAnimatedTransforms.animatedTransforms[modelInfoAnim.animation.animationBinary.bonesIDs[i]];
					animatedTransform.animateThisFrame = true;
					modelInfoAnim.animation.CopyFrameTransformByIndex(_currentKey, i, currentInterpolationFrame);
					modelInfoAnim.animation.CopyFrameTransformByIndex(frameNumber, i, nextInterpolationFrame);
					animatedTransform.SetPosition(Vector3.Slerp(currentInterpolationFrame.position, nextInterpolationFrame.position, weight));
					animatedTransform.SetRotation(Quaternion.Slerp(currentInterpolationFrame.rotation, nextInterpolationFrame.rotation, weight));
				}
			}
		}

		private void TangentCubicInterpolation(float weight)
		{
			int num = _currentKey - 1;
			int frameNumber = _currentKey;
			num = _currentKey + 1;
			int frameNumber2 = ((num <= modelInfoAnim.animation.endFrame) ? num : modelInfoAnim.animation.endFrame);
			num = _currentKey + 2;
			AnimatedTransform animatedTransform = new AnimatedTransform();
			AnimatedTransform animatedTransform2 = new AnimatedTransform();
			AnimatedTransform animatedTransform3 = new AnimatedTransform();
			AnimatedTransform animatedTransform4 = new AnimatedTransform();
			AnimatedTransform animatedTransform5 = new AnimatedTransform();
			AnimatedTransform animatedTransform6 = new AnimatedTransform();
			float num2 = 0.5f;
			for (int i = 0; i < modelInfoAnim.animation.animationBinary.bonesCount; i++)
			{
				if (_blendAnimatedTransforms.animatedTransforms.ContainsKey(modelInfoAnim.animation.animationBinary.bonesIDs[i]))
				{
					AnimatedTransform animatedTransform7 = _blendAnimatedTransforms.animatedTransforms[modelInfoAnim.animation.animationBinary.bonesIDs[i]];
					animatedTransform7.animateThisFrame = true;
					modelInfoAnim.animation.CopyFrameTransformByIndex(frameNumber, i, animatedTransform);
					modelInfoAnim.animation.CopyFrameTransformByIndex(frameNumber2, i, animatedTransform2);
					modelInfoAnim.animation.animationBinary.animationTangents.CopyFrameTransformByIndex(frameNumber, i, animatedTransform3);
					modelInfoAnim.animation.animationBinary.animationTangents.CopyFrameTransformByIndex(frameNumber2, i, animatedTransform4);
					Vector3 position = animatedTransform3.position;
					Quaternion rotation = animatedTransform3.rotation;
					Vector3 position2 = animatedTransform4.position;
					Quaternion rotation2 = animatedTransform4.rotation;
					animatedTransform5.SetPosition(animatedTransform.position + num2 * position);
					Quaternion rotation3 = Quaternion.Slerp(Quaternion.identity, rotation, num2) * animatedTransform.rotation;
					animatedTransform5.SetRotation(rotation3);
					animatedTransform6.SetPosition(animatedTransform2.position - num2 * position2);
					Quaternion rotation4 = Quaternion.Slerp(Quaternion.identity, Quaternion.Inverse(rotation2), num2) * animatedTransform2.rotation;
					animatedTransform6.SetRotation(rotation4);
					Vector3 position3 = InterpolateBy4(animatedTransform.position, animatedTransform5.position, animatedTransform6.position, animatedTransform2.position, weight);
					animatedTransform7.SetPosition(position3);
					Quaternion rotation5 = InterpolateBy4Bezier(animatedTransform.rotation, animatedTransform5.rotation, animatedTransform6.rotation, animatedTransform2.rotation, weight);
					animatedTransform7.SetRotation(rotation5);
				}
			}
		}

		private void SphericalCubicInterpolation(float weight)
		{
			int num = _currentKey - 1;
			int frameNumber = ((num >= 0) ? num : 0);
			int frameNumber2 = _currentKey;
			num = _currentKey + 1;
			int frameNumber3 = ((num <= modelInfoAnim.animation.endFrame) ? num : modelInfoAnim.animation.endFrame);
			num = _currentKey + 2;
			int frameNumber4 = ((num <= modelInfoAnim.animation.endFrame) ? num : modelInfoAnim.animation.endFrame);
			AnimatedTransform animatedTransform = new AnimatedTransform();
			AnimatedTransform animatedTransform2 = new AnimatedTransform();
			AnimatedTransform animatedTransform3 = new AnimatedTransform();
			AnimatedTransform animatedTransform4 = new AnimatedTransform();
			AnimatedTransform animatedTransform5 = new AnimatedTransform();
			AnimatedTransform animatedTransform6 = new AnimatedTransform();
			float num2 = 0.3333f;
			for (int i = 0; i < modelInfoAnim.animation.animationBinary.bonesCount; i++)
			{
				if (_blendAnimatedTransforms.animatedTransforms.ContainsKey(modelInfoAnim.animation.animationBinary.bonesIDs[i]))
				{
					AnimatedTransform animatedTransform7 = _blendAnimatedTransforms.animatedTransforms[modelInfoAnim.animation.animationBinary.bonesIDs[i]];
					animatedTransform7.animateThisFrame = true;
					modelInfoAnim.animation.CopyFrameTransformByIndex(frameNumber, i, animatedTransform);
					modelInfoAnim.animation.CopyFrameTransformByIndex(frameNumber2, i, animatedTransform2);
					modelInfoAnim.animation.CopyFrameTransformByIndex(frameNumber3, i, animatedTransform3);
					modelInfoAnim.animation.CopyFrameTransformByIndex(frameNumber4, i, animatedTransform4);
					Vector3 vector = 0.5f * (animatedTransform3.position - animatedTransform.position);
					Quaternion b = Quaternion.Slerp(Quaternion.identity, animatedTransform3.rotation * Quaternion.Inverse(animatedTransform.rotation), 0.5f);
					Vector3 vector2 = 0.5f * (animatedTransform4.position - animatedTransform2.position);
					Quaternion rotation = Quaternion.Slerp(Quaternion.identity, animatedTransform4.rotation * Quaternion.Inverse(animatedTransform2.rotation), 0.5f);
					animatedTransform5.SetPosition(animatedTransform2.position + num2 * vector);
					Quaternion rotation2 = Quaternion.Slerp(Quaternion.identity, b, num2) * animatedTransform2.rotation;
					animatedTransform5.SetRotation(rotation2);
					animatedTransform6.SetPosition(animatedTransform3.position - num2 * vector2);
					Quaternion rotation3 = Quaternion.Slerp(Quaternion.identity, Quaternion.Inverse(rotation), num2) * animatedTransform3.rotation;
					animatedTransform6.SetRotation(rotation3);
					Vector3 position = InterpolateBy4(animatedTransform2.position, animatedTransform5.position, animatedTransform6.position, animatedTransform3.position, weight);
					animatedTransform7.SetPosition(position);
					Quaternion rotation4 = InterpolateBy4Bezier(animatedTransform2.rotation, animatedTransform5.rotation, animatedTransform6.rotation, animatedTransform3.rotation, weight);
					animatedTransform7.SetRotation(rotation4);
				}
			}
		}

		public static Vector3 InterpolateVector3By2(Vector3 a0, Vector3 a1, float t)
		{
			float num = 1f - t;
			return new Vector3(a0.x * num + a1.x * t, a0.y * num + a1.y * t, a0.z * num + a1.z * t);
		}

		public static Vector3 InterpolateVector3By3(Vector3 a0, Vector3 a1, Vector3 a2, float t)
		{
			return InterpolateVector3By2(InterpolateVector3By2(a0, a1, t), InterpolateVector3By2(a1, a2, t), t);
		}

		public static Vector3 InterpolateVector3By4(Vector3 a0, Vector3 a1, Vector3 a2, Vector3 a3, float t)
		{
			return InterpolateVector3By3(InterpolateVector3By2(a0, a1, t), InterpolateVector3By2(a1, a2, t), InterpolateVector3By2(a2, a3, t), t);
		}

		public static Vector3 VecFromQuat(Quaternion q)
		{
			float angle;
			Vector3 axis;
			q.ToAngleAxis(out angle, out axis);
			return angle * axis;
		}

		public static Quaternion QuatFromVec(Vector3 vec)
		{
			return Quaternion.AngleAxis(vec.magnitude, vec.normalized);
		}

		public static Quaternion InterpolateQuaternionBy4(Quaternion q0, Quaternion q1, Quaternion q2, Quaternion q3, float t)
		{
			Vector3 vec = InterpolateVector3By4(VecFromQuat(q0), VecFromQuat(q1), VecFromQuat(q2), VecFromQuat(q3), t);
			return QuatFromVec(vec);
		}

		private static Quaternion InterpolateBy4Bezier(Quaternion q1, Quaternion q2, Quaternion q3, Quaternion q4, float t)
		{
			Quaternion a = Quaternion.Slerp(q1, q2, t);
			Quaternion quaternion = Quaternion.Slerp(q2, q3, t);
			Quaternion b = Quaternion.Slerp(q3, q4, t);
			Quaternion a2 = Quaternion.Slerp(a, quaternion, t);
			Quaternion b2 = Quaternion.Slerp(quaternion, b, t);
			return Quaternion.Slerp(a2, b2, t);
		}

		private static Vector3 InterpolateBy4(Vector3 q1, Vector3 q2, Vector3 q3, Vector3 q4, float t)
		{
			Vector3 a = Vector3.Lerp(q1, q2, t);
			Vector3 vector = Vector3.Lerp(q2, q3, t);
			Vector3 b = Vector3.Lerp(q3, q4, t);
			Vector3 a2 = Vector3.Lerp(a, vector, t);
			Vector3 b2 = Vector3.Lerp(vector, b, t);
			return Vector3.Lerp(a2, b2, t);
		}

		public string CurrentIntervalsToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Frame [{0}] intervals: ", _currentKey);
			foreach (IntervalAnimation currentInterval in currentIntervals)
			{
				stringBuilder.Append(currentInterval.name.ToString() + " | ");
			}
			return stringBuilder.ToString();
		}

		public void SetMediocre()
		{
			_isMain = false;
		}

		public bool CalculateBonesAnimationFixed(float fixedDelta)
		{
			_currentKey = Mathf.FloorToInt(fixedDelta);
			float num = fixedDelta - (float)_currentKey;
			_blendAnimatedTransforms.currentWeight = 1f;
			CheckIntervalsFixed(fixedDelta);
			if (num > 0f)
			{
				Interpolate(num);
			}
			else
			{
				modelInfoAnim.animation.CopyFrameTransforms(_currentKey, _blendAnimatedTransforms.animatedTransforms);
			}
			if (_isMirrored)
			{
				MirrorAnimation();
			}
			return true;
		}

		private void CheckIntervalsFixed(float fixedDelta)
		{
			foreach (IntervalAnimation interval in modelInfoAnim.animation.intervals)
			{
				if (fixedDelta >= (float)interval.start && fixedDelta < (float)(interval.finish + 1))
				{
					if (!currentFixedIntervals.ContainsKey(interval.GetHashCode()))
					{
						currentFixedIntervals[interval.GetHashCode()] = interval;
						this.onIntervalStart(interval);
					}
				}
				else if (currentFixedIntervals.ContainsKey(interval.GetHashCode()))
				{
					currentFixedIntervals.Remove(interval.GetHashCode());
					this.onIntervalEnd(interval);
				}
			}
		}
	}
}
