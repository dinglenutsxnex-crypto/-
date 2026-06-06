using System;
using System.Collections.Generic;
using SF3.Effects;
using SF3.GameModels;
using UnityEngine;

namespace SF3.Moves
{
	[Serializable]
	public class ModelAnimation
	{
		private enum EAnimationEngineState
		{
			PAUSE = 0,
			PLAY = 1,
			IN_PAUSE_PROCESS = 2,
			IN_PLAY_PROCESS = 3,
			STOP = 4
		}

		[Serializable]
		public class BlendAnimatedTransforms
		{
			public float currentWeight;

			public Dictionary<int, AnimatedTransform> animatedTransforms;

			public BlendAnimatedTransforms()
			{
				currentWeight = -1f;
				animatedTransforms = new Dictionary<int, AnimatedTransform>();
			}

			public void Clear()
			{
				currentWeight = -1f;
				animatedTransforms.Clear();
			}
		}

		public const int MAX_ANIMATIONS_FOR_BLEND = 5;

		private EAnimationEngineState _engineState;

		private float _engineDeltaTime;

		private float _engineDeltaTimeCoef;

		private float _pauseFrames;

		private int _resumeFrames;

		private float _processInFrames;

		private float _currentProcessInFrames;

		private IAnimatedModel _animatedModel;

		private IEventSender _eventSender;

		private ActiveAnimation _mainActiveAnimation;

		private ActiveAnimation _lastActiveAnimation;

		private List<BlendAnimatedTransforms> _activeAnimatedTransforms;

		private int _currentMainAnimatedTransforms;

		private Dictionary<int, AnimatedTransform> _resultFrameAnimatedTransforms;

		private Vector3 _velocity;

		private Vector3 _rotation;

		private ModelInfoAnimation _animationInStack;

		private bool _playStackForced;

		private float _lastStartPlayedFrame;

		private Vector3 _impulseInStack;

		private string _rigidBodyNameInStack;

		private bool _selfEnd;

		private int _selfEndFrame;

		private Quaternion _defaultRootRotation;

		private float _acceleration;

		private float _moveVelocityX;

		private float _moveFramesCount;

		private List<ActiveAnimation> _playingAnimations;

		public List<IntervalAttack> lastAnimationIntervals = new List<IntervalAttack>();

		public bool physicalNow
		{
			get
			{
				return _mainActiveAnimation.modelInfoAnim.animation.physical;
			}
		}

		public string[] animationNames
		{
			get
			{
				if (mainAnimationInfo != null)
				{
					return mainAnimationInfo.animation.animationNames;
				}
				return null;
			}
		}

		public int animationKey
		{
			get
			{
				return _mainActiveAnimation.currentKey;
			}
		}

		public List<IntervalAnimation> animationIntervals
		{
			get
			{
				return _mainActiveAnimation.currentIntervals;
			}
		}

		public ModelInfoAnimation mainAnimationInfo
		{
			get
			{
				return _mainActiveAnimation.modelInfoAnim;
			}
		}

		public float delta
		{
			get
			{
				return _mainActiveAnimation.delta;
			}
		}

		public bool inUninterrupt
		{
			get
			{
				return _mainActiveAnimation.currentIntervals.Exists((IntervalAnimation interval) => interval.name == "Uninterrupt");
			}
		}

		public event Action<IntervalAnimation> OnIntervalStartEvent = delegate
		{
		};

		public event Action<IntervalAnimation> OnIntervalEndEvent = delegate
		{
		};

		public event Action<string> OnAnimationStartEvent = delegate
		{
		};

		public event Action<string> OnZeroFrameEvent = delegate
		{
		};

		public event Action<string> OnAnimationStopEvent = delegate
		{
		};

		public event Action<string> OnAnimationEndEvent = delegate
		{
		};

		public ModelAnimation(Model model)
		{
			_animatedModel = model;
			_eventSender = model;
			_playingAnimations = new List<ActiveAnimation>();
			_resultFrameAnimatedTransforms = new Dictionary<int, AnimatedTransform>();
			_activeAnimatedTransforms = new List<BlendAnimatedTransforms>(5);
			for (int i = 0; i < 5; i++)
			{
				_activeAnimatedTransforms.Add(new BlendAnimatedTransforms());
			}
		}

		public void Initialize()
		{
			if (this.OnAnimationStartEvent != null)
			{
				Delegate[] invocationList = this.OnAnimationStartEvent.GetInvocationList();
				Delegate[] array = invocationList;
				for (int i = 0; i < array.Length; i++)
				{
					Action<string> value = (Action<string>)array[i];
					OnAnimationStartEvent -= value;
				}
			}
			_playingAnimations.Clear();
			_resultFrameAnimatedTransforms.Clear();
			foreach (BlendAnimatedTransforms activeAnimatedTransform in _activeAnimatedTransforms)
			{
				activeAnimatedTransform.Clear();
			}
			UpdateBonesData();
			_playingAnimations.Clear();
			_mainActiveAnimation = new ActiveAnimation();
			_lastActiveAnimation = null;
			_animationInStack = null;
			_selfEnd = false;
			_selfEndFrame = 0;
			_playStackForced = false;
			_lastStartPlayedFrame = -1f;
			_impulseInStack = Vector3.zero;
			_rigidBodyNameInStack = string.Empty;
			_engineState = EAnimationEngineState.PAUSE;
			_engineDeltaTimeCoef = 1f;
			_velocity = (_rotation = Vector3.zero);
			_currentMainAnimatedTransforms = 0;
		}

		public void UpdateBonesData()
		{
			int[] bonesIDs = _animatedModel.GetBonesIDs();
			_resultFrameAnimatedTransforms.Clear();
			for (int i = 0; i < bonesIDs.Length; i++)
			{
				if (bonesIDs[i] != -1)
				{
					_resultFrameAnimatedTransforms.Add(bonesIDs[i], new AnimatedTransform());
				}
			}
			foreach (BlendAnimatedTransforms activeAnimatedTransform in _activeAnimatedTransforms)
			{
				activeAnimatedTransform.animatedTransforms.Clear();
				for (int j = 0; j < bonesIDs.Length; j++)
				{
					if (bonesIDs[j] != -1)
					{
						activeAnimatedTransform.animatedTransforms.Add(bonesIDs[j], new AnimatedTransform());
					}
				}
			}
		}

		public void Update()
		{
			if (_engineState == EAnimationEngineState.IN_PAUSE_PROCESS)
			{
				_currentProcessInFrames -= GameTimeController.gameTimeDelta;
				if (_currentProcessInFrames <= 0f)
				{
					PauseForced();
				}
				else
				{
					_engineDeltaTimeCoef = EffectsManager.Instance.slowmotionCurve.Evaluate(_currentProcessInFrames / _processInFrames);
				}
			}
			else if (_engineState == EAnimationEngineState.IN_PLAY_PROCESS)
			{
				_currentProcessInFrames -= GameTimeController.gameTimeDelta;
				if (_currentProcessInFrames <= 0f)
				{
					ResumeForced();
				}
				else
				{
					_engineDeltaTimeCoef = EffectsManager.Instance.slowmotionCurve.Evaluate(1f - _currentProcessInFrames / _processInFrames);
				}
			}
			if (_engineState != 0)
			{
				_engineDeltaTime = GameTimeController.gameTimeDelta * _engineDeltaTimeCoef;
				if (_animationInStack != null && (!_selfEnd || _mainActiveAnimation.delta >= (float)_selfEndFrame))
				{
					PlayInStack();
				}
				if (_playingAnimations.Count > 0)
				{
					ApplyAnimationVelocityAndRotation();
					CalculateNewFrameAnimations();
					if (!physicalNow)
					{
						BlendAnimations();
						ApplyBonesAnimation();
					}
				}
				return;
			}
			_pauseFrames -= GameTimeController.gameTimeDelta;
			if (_pauseFrames <= 0f)
			{
				if (_resumeFrames > 0)
				{
					EnableAnimatedModel(true);
				}
				Resume(_resumeFrames);
			}
		}

		private void ApplyAnimationVelocityAndRotation()
		{
			UpdateOffsets();
			ShiftPosition(_velocity * _engineDeltaTime);
			ShiftRotation(_rotation * _engineDeltaTime);
		}

		private void UpdateOffsets()
		{
			if (_moveFramesCount > 0f)
			{
				_moveVelocityX += _acceleration * _engineDeltaTime;
				_velocity = Vector3.right * _moveVelocityX;
				_moveFramesCount -= _engineDeltaTime;
				if (_moveFramesCount <= 0f)
				{
					_velocity = Vector3.zero;
				}
			}
		}

		private void CalculateNewFrameAnimations()
		{
			for (int num = _playingAnimations.Count - 1; num >= 0; num--)
			{
				if (_playingAnimations[num].Update(_engineDeltaTime))
				{
					num++;
				}
			}
		}

		public void UpdateFixedFrame(float fixedDelta)
		{
			if (_playingAnimations.Count > 0)
			{
				ApplyAnimationVelocityAndRotation();
				CalculateNewFrameAnimations(fixedDelta);
				if (!physicalNow)
				{
					BlendAnimations();
					ApplyBonesAnimation();
				}
			}
		}

		public string[] GetPlayingAnimations()
		{
			string[] array = null;
			if (_playingAnimations.Count > 0)
			{
				array = new string[_playingAnimations.Count];
				for (int i = 0; i < _playingAnimations.Count; i++)
				{
					array[i] = _playingAnimations[i].modelInfoAnim.animation.name;
				}
			}
			return array;
		}

		private void EndActiveAnimation()
		{
			_mainActiveAnimation.Stop(_animatedModel, ref _defaultRootRotation);
			if (_mainActiveAnimation.modelInfoAnim != null)
			{
				this.OnAnimationEndEvent(_mainActiveAnimation.modelInfoAnim.animation.name);
			}
			_eventSender.ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_ANIMATION_END, -2, _mainActiveAnimation.modelInfoAnim));
		}

		private void CalculateNewFrameAnimations(float fixedDelta)
		{
			for (int num = _playingAnimations.Count - 1; num >= 0; num--)
			{
				_playingAnimations[num].CalculateBonesAnimationFixed(fixedDelta);
			}
		}

		private void BlendAnimations()
		{
			foreach (KeyValuePair<int, AnimatedTransform> resultFrameAnimatedTransform in _resultFrameAnimatedTransforms)
			{
				resultFrameAnimatedTransform.Value.animateThisFrame = false;
			}
			AnimatedTransform animatedTransform = new AnimatedTransform();
			bool flag = true;
			int num = _currentMainAnimatedTransforms + 1;
			while (flag)
			{
				if (num >= _activeAnimatedTransforms.Count)
				{
					num = -1;
				}
				else
				{
					if (_activeAnimatedTransforms[num].currentWeight != -1f)
					{
						foreach (KeyValuePair<int, AnimatedTransform> animatedTransform2 in _activeAnimatedTransforms[num].animatedTransforms)
						{
							if (animatedTransform2.Value.animateThisFrame)
							{
								if (!_resultFrameAnimatedTransforms[animatedTransform2.Key].animateThisFrame)
								{
									_resultFrameAnimatedTransforms[animatedTransform2.Key].animateThisFrame = true;
									AnimatedTransform.CopyBoneTransform(animatedTransform2.Value, _resultFrameAnimatedTransforms[animatedTransform2.Key]);
								}
								else
								{
									AnimatedTransform.CopyBoneTransform(animatedTransform2.Value, animatedTransform);
									_resultFrameAnimatedTransforms[animatedTransform2.Key].SetPosition(Vector3.Slerp(_resultFrameAnimatedTransforms[animatedTransform2.Key].position, animatedTransform.position, _activeAnimatedTransforms[num].currentWeight));
									_resultFrameAnimatedTransforms[animatedTransform2.Key].SetRotation(Quaternion.Slerp(_resultFrameAnimatedTransforms[animatedTransform2.Key].rotation, animatedTransform.rotation, _activeAnimatedTransforms[num].currentWeight));
								}
							}
						}
					}
					if (num == _currentMainAnimatedTransforms)
					{
						flag = false;
					}
				}
				num++;
			}
		}

		private void ApplyBonesAnimation()
		{
			_animatedModel.UpdateBonesPositions(_resultFrameAnimatedTransforms);
		}

		public void StopForced()
		{
			PauseForced();
			_engineState = EAnimationEngineState.STOP;
		}

		public void Pause(int pauseFrames, int pauseProcessInFrames, int resumeFrames = 0)
		{
			if (_engineState != 0)
			{
				_pauseFrames = pauseFrames;
				_resumeFrames = resumeFrames;
				if (pauseProcessInFrames <= 0)
				{
					PauseForced();
					return;
				}
				_currentProcessInFrames = (_processInFrames = pauseProcessInFrames);
				_engineState = EAnimationEngineState.IN_PAUSE_PROCESS;
			}
		}

		public void PauseForced()
		{
			if (_engineState != 0)
			{
				_engineDeltaTimeCoef = 0f;
				EnableAnimatedModel(false);
				_engineState = EAnimationEngineState.PAUSE;
			}
		}

		public void Resume(int resumeProcessInFrames)
		{
			if (_engineState != EAnimationEngineState.PLAY)
			{
				if (resumeProcessInFrames <= 0)
				{
					ResumeForced();
					return;
				}
				_currentProcessInFrames = (_processInFrames = resumeProcessInFrames);
				_engineState = EAnimationEngineState.IN_PLAY_PROCESS;
			}
		}

		public void ResumeForced()
		{
			if (_engineState != EAnimationEngineState.PLAY)
			{
				_engineDeltaTimeCoef = 1f;
				EnableAnimatedModel(true);
				_engineState = EAnimationEngineState.PLAY;
			}
		}

		private void EnableAnimatedModel(bool enable)
		{
			_animatedModel.EnableAttakingCollisions(enable);
			_animatedModel.EnableRepulsionCollisions(enable);
			_animatedModel.SetRagdollSleepState(!enable, 2);
		}

		public void Play(string animationName, bool forcePlay = false)
		{
			ModelInfoAnimation modelAnimationByName = _animatedModel.GetModelAnimationByName(animationName);
			if (modelAnimationByName == null)
			{
				throw new Exception(string.Format("Cant find animation with name [{0}] in model [{1}]", animationName, _animatedModel.GetName()));
			}
			Play(modelAnimationByName, forcePlay);
		}

		private void Play(ModelInfoAnimation anima, bool forcePlay = false)
		{
			if (_lastStartPlayedFrame == GameTimeController.battleTime || _playStackForced)
			{
			}
			if (_engineState != EAnimationEngineState.PLAY && _engineState != EAnimationEngineState.IN_PAUSE_PROCESS)
			{
				ResumeForced();
			}
			if (_mainActiveAnimation.modelInfoAnim != null && !_mainActiveAnimation.isEnding)
			{
				EndActiveAnimation();
			}
			if (_playStackForced)
			{
				return;
			}
			_animationInStack = anima;
			_playStackForced = forcePlay;
			_selfEnd = false;
			_selfEndFrame = 0;
			if (_mainActiveAnimation.selfEnded)
			{
				_selfEnd = true;
				_selfEndFrame = _mainActiveAnimation.modelInfoAnim.animation.endFrame - _animationInStack.animation.blendingFrames;
				_mainActiveAnimation.CalculateStopedFrame();
			}
			else
			{
				_mainActiveAnimation.CalculateStopedFrame(_animationInStack.animation.blendingFrames);
			}
			if (_lastActiveAnimation == null)
			{
				PlayInStack();
				ApplyAnimationVelocityAndRotation();
				CalculateNewFrameAnimations();
				if (!physicalNow)
				{
					BlendAnimations();
					ApplyBonesAnimation();
				}
			}
		}

		public ModelInfoAnimation PlayFixed(string animationName, bool manual = false)
		{
			ModelInfoAnimation modelAnimationByName = _animatedModel.GetModelAnimationByName(animationName);
			PlayFixed(modelAnimationByName, manual);
			return modelAnimationByName;
		}

		public bool PlayFixed(ModelInfoAnimation animation, bool manual = false)
		{
			if ((animation != null && !manual) || (manual && animation != _animationInStack))
			{
				_playingAnimations.Clear();
				_animationInStack = animation;
				_selfEnd = false;
				_selfEndFrame = 0;
				_engineState = EAnimationEngineState.PLAY;
				foreach (BlendAnimatedTransforms activeAnimatedTransform in _activeAnimatedTransforms)
				{
					activeAnimatedTransform.currentWeight = -1f;
				}
				if (manual)
				{
					BlendAnimatedTransforms activeTranfsValue = _activeAnimatedTransforms[0];
					_mainActiveAnimation = new ActiveAnimation(_animationInStack, _animatedModel.GetMoveDirection(), _animatedModel.GetMirrored(), _defaultRootRotation, ref activeTranfsValue);
					_mainActiveAnimation.onIntervalEnd += OnIntervalEnd;
					_mainActiveAnimation.onIntervalStart += OnIntervalStart;
					_playingAnimations.Add(_mainActiveAnimation);
				}
				return true;
			}
			return false;
		}

		private void PlayInStack()
		{
			if (_playingAnimations.Count >= 5)
			{
				Debug.LogError(string.Format("The amount of active animations on model {0} has reached a critical point of {1}!!!", _animatedModel.GetName(), 5));
				Debug.LogError("----Current playing:");
				foreach (ActiveAnimation playingAnimation in _playingAnimations)
				{
					Debug.LogError(playingAnimation.modelInfoAnim.animation.name);
				}
				Debug.LogError("----");
				return;
			}
			_lastActiveAnimation = _mainActiveAnimation;
			_animatedModel.SetMoveDirection(_animationInStack.animation.MoveDirection(_animatedModel));
			_mainActiveAnimation.ClearActiveIntervals(_animatedModel);
			_mainActiveAnimation.SetMediocre();
			if (_lastActiveAnimation.modelInfoAnim != null)
			{
				_lastActiveAnimation.modelInfoAnim.InitFinishPlay();
			}
			_animationInStack.InitPlay((_mainActiveAnimation.modelInfoAnim == null) ? null : _mainActiveAnimation);
			if (!_animationInStack.transitionApplied)
			{
				lastAnimationIntervals.Clear();
				if (_animationInStack.mirrorer.enabled)
				{
					_animatedModel.CheckMirrored(_animationInStack.animation.mirrored);
				}
			}
			else
			{
				lastAnimationIntervals.AddRange(_animatedModel.getLastIntervalAttack());
			}
			_animatedModel.ResetInterval();
			BlendAnimatedTransforms activeTranfsValue = null;
			for (int i = _currentMainAnimatedTransforms + 1; i != _currentMainAnimatedTransforms; i++)
			{
				if (i >= _activeAnimatedTransforms.Count)
				{
					i = -1;
				}
				else if (_activeAnimatedTransforms[i].currentWeight == -1f)
				{
					activeTranfsValue = _activeAnimatedTransforms[i];
					_currentMainAnimatedTransforms = i;
					break;
				}
			}
			if (activeTranfsValue == null)
			{
				throw new Exception(string.Format("ModelAnimation cant find free element in transforms stack for new animation."));
			}
			bool isMirroredVal = _animatedModel.GetMirrored();
			if ((_lastActiveAnimation.modelInfoAnim != null && _lastActiveAnimation.modelInfoAnim.animation.selfNoMirroring) || (_animationInStack.animation.selfNoMirroring && _animationInStack.animation.name.Equals(_lastActiveAnimation.modelInfoAnim.animation.name)))
			{
				isMirroredVal = _lastActiveAnimation.isMirrored;
			}
			_mainActiveAnimation = new ActiveAnimation(_animationInStack, _animatedModel.GetMoveDirection(), isMirroredVal, _defaultRootRotation, ref activeTranfsValue);
			if (_animationInStack.transitionApplied)
			{
				_mainActiveAnimation.ApplyDuringInterval(ActivateAttackColliders, lastAnimationIntervals);
			}
			_mainActiveAnimation.onAnimationStop += OnAnimationStop;
			_mainActiveAnimation.onAnimationEnd += OnAnimationEnd;
			_mainActiveAnimation.onIntervalEnd += OnIntervalEnd;
			_mainActiveAnimation.onIntervalStart += OnIntervalStart;
			_mainActiveAnimation.onZeroFrame += OnZeroFrame;
			_mainActiveAnimation.onNeedWeaponSwitch += OnNeedWeaponSwitch;
			_velocity = _mainActiveAnimation.getVelocity;
			_velocity.x *= ((_animatedModel.GetMoveDirection() != EDirectionType.RIGHT) ? (-1f) : 1f);
			_rotation = _mainActiveAnimation.getRotation;
			if (physicalNow)
			{
				for (int num = _playingAnimations.Count - 1; num >= 0; num--)
				{
					_playingAnimations[num].ForceStop();
				}
				_animatedModel.SetRagdollActive(true);
				_animatedModel.AddForce(_impulseInStack, _rigidBodyNameInStack);
				_animatedModel.ActivateRagdolAlignment(Mathf.Abs(_impulseInStack.z) > Mathf.Abs(_impulseInStack.x));
			}
			else if (_animatedModel.GetIsPhysics())
			{
				_animatedModel.RemoveModelVariable("InPhysics");
				_animatedModel.SetRagdollActive(false);
			}
			_playingAnimations.Add(_mainActiveAnimation);
			_animationInStack = null;
			_selfEnd = false;
			_selfEndFrame = 0;
			_lastStartPlayedFrame = GameTimeController.battleTime;
			_impulseInStack = Vector3.zero;
			_rigidBodyNameInStack = string.Empty;
			if (this.OnAnimationStartEvent != null)
			{
				this.OnAnimationStartEvent(_mainActiveAnimation.modelInfoAnim.animation.name);
			}
			_eventSender.ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_ANIMATION_START, -2, _mainActiveAnimation.modelInfoAnim));
			BattleLog.AnimationStart(_animatedModel.GetName(), _mainActiveAnimation.modelInfoAnim.animation.name);
		}

		public void PlayByFrames(ModelInfoAnimation anima)
		{
			_playingAnimations.Clear();
			Play(anima);
		}

		public List<IntervalAnimation> GetIntervalTypeExist(EIntervalsType intervalType)
		{
			List<IntervalAnimation> list = new List<IntervalAnimation>();
			if (animationIntervals != null)
			{
				foreach (IntervalAnimation animationInterval in animationIntervals)
				{
					if (animationInterval.type == intervalType)
					{
						list.Add(animationInterval);
					}
				}
			}
			return list;
		}

		public bool IsIntervalNameExists(string name)
		{
			foreach (IntervalAnimation interval in _mainActiveAnimation.modelInfoAnim.animation.intervals)
			{
				if (interval.name.Equals(name))
				{
					return true;
				}
			}
			return false;
		}

		public void ShiftPosition(Vector3 vector)
		{
			foreach (ActiveAnimation playingAnimation in _playingAnimations)
			{
				playingAnimation.ShiftPosition(vector);
			}
		}

		public void ShiftRotation(Vector3 vector)
		{
			foreach (ActiveAnimation playingAnimation in _playingAnimations)
			{
				playingAnimation.ShiftRotation(vector);
			}
		}

		public void AddImpulse(Vector3 impulse, string rigidBodyName)
		{
			_impulseInStack = impulse;
			_rigidBodyNameInStack = rigidBodyName;
		}

		public void Move(float speed, float acceleration, int framesCount)
		{
			_moveVelocityX = speed;
			_acceleration = acceleration;
			_moveFramesCount = framesCount;
		}

		private void OnAnimationStop(ActiveAnimation activeAnim)
		{
			this.OnAnimationStopEvent(_mainActiveAnimation.modelInfoAnim.animation.name);
			_playingAnimations.Remove(activeAnim);
			_animatedModel.ResetBonesPosition();
			if (_selfEnd && _animationInStack != null)
			{
				PlayInStack();
			}
		}

		private void OnAnimationEnd(ActiveAnimation activeAnim)
		{
			if (!_mainActiveAnimation.modelInfoAnim.animation.looped && !_mainActiveAnimation.isEnding)
			{
				EndActiveAnimation();
			}
		}

		private void OnIntervalStart(IntervalAnimation intervalValue)
		{
			this.OnIntervalStartEvent(intervalValue);
			if (intervalValue.type == EIntervalsType.INTERVAL_ATTACK)
			{
				ActivateAttackColliders(intervalValue);
			}
			_eventSender.ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_INTERVAL_START, -2, intervalValue));
		}

		private void OnIntervalEnd(IntervalAnimation intervalValue)
		{
			this.OnIntervalEndEvent(intervalValue);
			if (intervalValue.type == EIntervalsType.INTERVAL_ATTACK)
			{
				DisableAttackColliders(intervalValue);
			}
			_eventSender.ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_INTERVAL_END, -2, intervalValue));
		}

		private void ActivateAttackColliders(IntervalAnimation intervalValue)
		{
			foreach (ModelIntervalAttack modelAttackingInterval in mainAnimationInfo.modelAttackingIntervals)
			{
				if (modelAttackingInterval.intervalStart == intervalValue.start)
				{
					if (!_animatedModel.GetMirrored())
					{
						_animatedModel.AddAttackingCapsules(modelAttackingInterval.GetHashCode(), modelAttackingInterval.normalAttackingCapsules);
					}
					else
					{
						_animatedModel.AddAttackingCapsules(modelAttackingInterval.GetHashCode(), modelAttackingInterval.mirrorAttackingCapsules);
					}
					break;
				}
			}
		}

		private void DisableAttackColliders(IntervalAnimation intervalValue)
		{
			foreach (ModelIntervalAttack modelAttackingInterval in mainAnimationInfo.modelAttackingIntervals)
			{
				if (modelAttackingInterval.Equal((IntervalAttack)intervalValue))
				{
					_animatedModel.ClearAttackingCapsules(modelAttackingInterval.GetHashCode());
				}
			}
		}

		private void OnNeedWeaponSwitch(bool switchToMirror)
		{
			_animatedModel.SwitchWeapon(switchToMirror);
		}

		private void OnZeroFrame()
		{
			if (this.OnZeroFrameEvent != null)
			{
				this.OnZeroFrameEvent(_mainActiveAnimation.modelInfoAnim.animation.name);
			}
		}
	}
}
