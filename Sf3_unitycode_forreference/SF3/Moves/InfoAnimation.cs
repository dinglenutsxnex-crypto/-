using System;
using System.Collections.Generic;
using Nekki.Yaml;
using SF3.Audio;
using SF3.GameModels;
using UnityEngine;
using sf3DTO;

namespace SF3.Moves
{
	[Serializable]
	public class InfoAnimation : IHasFileName
	{
		[Serializable]
		public class SoundsForFrame
		{
			public enum EFrameSoundType
			{
				COMMON = 0,
				GENDER = 1,
				FLOOR = 2
			}

			[Serializable]
			public struct SoundsForFrameCommon
			{
				public EFrameSoundType frameSoundType;

				public Dictionary<string, List<string>> _sounds;

				public SoundsForFrameCommon(EFrameSoundType valueType)
				{
					frameSoundType = valueType;
					_sounds = new Dictionary<string, List<string>>();
				}

				public void SetSounds(string soundsKey, string[] soundsNames)
				{
					if (!_sounds.ContainsKey(soundsKey))
					{
						_sounds.Add(soundsKey, new List<string>());
					}
					_sounds[soundsKey].AddRange(soundsNames);
				}
			}

			public SoundsForFrameCommon soundsForFrameCommon { get; private set; }

			public int frame { get; private set; }

			private SoundsForFrame(int frameNumber)
			{
				frame = frameNumber;
			}

			public static List<SoundsForFrame> Parse(Sequence sNode)
			{
				List<SoundsForFrame> list = new List<SoundsForFrame>();
				string[] array = new string[0];
				foreach (Mapping item in sNode)
				{
					int num = int.Parse(item.GetText("Frame").value.ToString()) - 1;
					if (num < 0)
					{
						num = 0;
					}
					SoundsForFrame soundsForFrame = new SoundsForFrame(num);
					Sequence sequence = item.GetSequence("Common");
					if (sequence != null)
					{
						soundsForFrame.soundsForFrameCommon = new SoundsForFrameCommon(EFrameSoundType.COMMON);
						array = new string[sequence.nodesInside.Count];
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = sequence.nodesInside[i].value.ToString();
						}
						soundsForFrame.soundsForFrameCommon.SetSounds(string.Empty, array);
					}
					Mapping mapping2 = item.GetMapping("Gender");
					if (mapping2 != null)
					{
						soundsForFrame.soundsForFrameCommon = new SoundsForFrameCommon(EFrameSoundType.GENDER);
						foreach (Sequence item2 in mapping2.nodesInside)
						{
							array = new string[item2.nodesInside.Count];
							for (int j = 0; j < array.Length; j++)
							{
								array[j] = item2.nodesInside[j].value.ToString();
							}
							soundsForFrame.soundsForFrameCommon.SetSounds(item2.key, array);
						}
					}
					mapping2 = item.GetMapping("Floor");
					if (mapping2 != null)
					{
						soundsForFrame.soundsForFrameCommon = new SoundsForFrameCommon(EFrameSoundType.FLOOR);
						foreach (Sequence item3 in mapping2.nodesInside)
						{
							array = new string[item3.nodesInside.Count];
							for (int k = 0; k < array.Length; k++)
							{
								array[k] = item3.nodesInside[k].value.ToString();
							}
							soundsForFrame.soundsForFrameCommon.SetSounds(SF3Utils.GetSurfaceTypeByName(item3.key).ToString(), array);
						}
					}
					list.Add(soundsForFrame);
				}
				return list;
			}

			public void PlayRandomSound(string soundKey)
			{
				if (soundsForFrameCommon._sounds.ContainsKey(soundKey))
				{
					List<string> list = soundsForFrameCommon._sounds[soundKey];
					int count = list.Count;
					if (count > 0)
					{
						AudioManager.Instance.PlaySound(list[Rand.Range(0, count)]);
					}
				}
			}

			public void LoadSounds(List<ESurfaceType> surfaceTypes, Gender gender)
			{
				if (soundsForFrameCommon._sounds == null)
				{
					return;
				}
				foreach (KeyValuePair<string, List<string>> sound in soundsForFrameCommon._sounds)
				{
					if (sound.Key.IsNullOrEmpty() || CheckGender(sound.Key, gender) || CheckSurface(sound.Key, surfaceTypes))
					{
						Load(sound.Value);
					}
				}
			}

			private void Load(List<string> sounds)
			{
				foreach (string sound in sounds)
				{
					AudioManager.Instance.LoadSound(sound);
				}
			}

			private bool CheckGender(string soundType, Gender gender)
			{
				return soundType.ToLower().Equals(gender.ToString().ToLower());
			}

			private bool CheckSurface(string soundType, List<ESurfaceType> surfaceTypes)
			{
				foreach (ESurfaceType surfaceType in surfaceTypes)
				{
					if (soundType.Equals(surfaceType.ToString()))
					{
						return true;
					}
				}
				return false;
			}
		}

		public List<FightController.EFightStage> stages;

		public List<SwitchFrame> weaponSwitchFrames;

		public bool aiTransistable;

		public string name { get; private set; }

		public string[] groupNames { get; private set; }

		public string[] animationNames { get; private set; }

		public AnimationBinaries animationBinary { get; private set; }

		public List<AnimationTransition> animationTransitions { get; private set; }

		public bool physical { get; private set; }

		public List<IntervalAnimation> intervals { get; private set; }

		public MoveAlign align { get; private set; }

		public AnimationDirection direction { get; private set; }

		public int startFrame { get; private set; }

		public int endFrame { get; private set; }

		public InterpolationType interpolation { get; private set; }

		public int midFrames { get; private set; }

		public List<InfoTrigger> triggers { get; private set; }

		public int framesCount { get; private set; }

		public Vector3 velocty { get; private set; }

		public Vector3 rotation { get; private set; }

		public bool looped { get; private set; }

		public int blendingFrames { get; private set; }

		public int? mirrored { get; private set; }

		public Dictionary<int, List<SoundsForFrame>> soundsForFrames { get; private set; }

		public Vector2 repulsionScale { get; private set; }

		public bool hasAttackInterval { get; private set; }

		public List<IntervalAttack> attackIntervals { get; private set; }

		public IntervalAnimation uninterrupt { get; private set; }

		public IntervalAnimation transitional { get; private set; }

		public string FileName { get; set; }

		public bool selfNoMirroring { get; private set; }

		public InfoAnimation(string animationNameVal, AnimationBinaries binaries = null)
		{
			name = animationNameVal;
			groupNames = new string[0];
			animationBinary = binaries;
			intervals = new List<IntervalAnimation>();
			triggers = new List<InfoTrigger>();
			stages = new List<FightController.EFightStage>();
			weaponSwitchFrames = new List<SwitchFrame>();
			align = null;
			looped = false;
			blendingFrames = 4;
			velocty = Vector3.zero;
			rotation = Vector3.zero;
			interpolation = InterpolationType.linear;
			endFrame = int.MaxValue;
			startFrame = 0;
			midFrames = 1;
			animationTransitions = new List<AnimationTransition>();
			soundsForFrames = new Dictionary<int, List<SoundsForFrame>>();
			repulsionScale = Vector2.one;
			mirrored = null;
			selfNoMirroring = false;
		}

		public bool StageExist(FightController.EFightStage stageValue)
		{
			foreach (FightController.EFightStage stage in stages)
			{
				if (stageValue == stage)
				{
					return true;
				}
			}
			return false;
		}

		public void SetRepulsionScale(Vector2 scale)
		{
			repulsionScale = scale;
		}

		public void SetGroups(List<string> groupsNamesValue)
		{
			groupNames = groupsNamesValue.ToArray();
		}

		public void AddTransition(AnimationTransition value)
		{
			animationTransitions.Add(value);
		}

		public AnimationTransition TransitionExist(ActiveAnimation lastAnimation)
		{
			foreach (AnimationTransition animationTransition in animationTransitions)
			{
				if (animationTransition.Equal(lastAnimation.modelInfoAnim.animation.name))
				{
					return animationTransition;
				}
			}
			return null;
		}

		public void InitDirection(Mapping directionEntry)
		{
			if (directionEntry == null)
			{
				direction = null;
			}
			else
			{
				direction = new AnimationDirection(directionEntry);
			}
		}

		public void SetIntervals(List<IntervalAnimation> newIntervals)
		{
			intervals = newIntervals;
			intervals.Sort((IntervalAnimation a, IntervalAnimation b) => (a.start != b.start) ? ((a.start > b.start) ? 1 : (-1)) : 0);
			attackIntervals = new List<IntervalAttack>();
			foreach (IntervalAnimation interval in intervals)
			{
				if (interval.type == EIntervalsType.INTERVAL_ATTACK)
				{
					attackIntervals.Add((IntervalAttack)interval);
					hasAttackInterval = true;
				}
				else if (interval.type == EIntervalsType.INTERVAL_TRANSITIONAL)
				{
					transitional = interval;
				}
				else if (interval.name.Equals("Uninterrupt"))
				{
					uninterrupt = interval;
				}
			}
		}

		public void SetSelfNoMirroring(bool isSelfNoMirroring)
		{
			selfNoMirroring = isSelfNoMirroring;
		}

		public void SetLooped(bool isLooped)
		{
			looped = isLooped;
		}

		public void SetBlendingFrames(int value)
		{
			blendingFrames = value;
		}

		public void AddWeaponSwitchFrame(bool mirror, int value)
		{
			weaponSwitchFrames.Add(new SwitchFrame(mirror, value));
		}

		public void SortSwitchFrames()
		{
			weaponSwitchFrames.Sort((SwitchFrame a, SwitchFrame b) => (a.Frame > b.Frame) ? 1 : (-1));
		}

		public void SetAlign(MoveAlign value)
		{
			align = value;
		}

		public void SetForceMirrored(int value)
		{
			mirrored = value;
		}

		public void SetVelocity(Vector3 value)
		{
			velocty = value;
		}

		public void SetRotation(Vector3 value)
		{
			rotation = value;
		}

		public void AddTrigger(InfoTrigger trigerVar)
		{
			if (trigerVar != null)
			{
				triggers.Add(trigerVar);
			}
			else
			{
				Debug.LogError("Cant add null trigger!!!");
			}
		}

		public bool HasTriggerEvent(ETriggerEvents triggerEvent)
		{
			foreach (InfoTrigger trigger in triggers)
			{
				if (trigger.HasEvent(triggerEvent) != null)
				{
					return true;
				}
			}
			return false;
		}

		public List<IntervalAnimation> GetIntervals(int keyFrame)
		{
			List<IntervalAnimation> list = new List<IntervalAnimation>();
			foreach (IntervalAnimation interval in intervals)
			{
				if (interval.IsKeyInInterval(keyFrame))
				{
					list.Add(interval);
				}
			}
			return list;
		}

		public int GetNextStartIntervalKey(int currentKey)
		{
			foreach (IntervalAnimation interval in intervals)
			{
				if (interval.start > currentKey)
				{
					return interval.start;
				}
			}
			return 99999;
		}

		public EDirectionType MoveDirection(IAnimatedModel animatedModel)
		{
			if (direction == null || !direction.IsAppliedTo(animatedModel.IsAI))
			{
				return animatedModel.GetMoveDirection();
			}
			return direction.MoveDirection(animatedModel);
		}

		public void SetStartFrame(int newStartFrame)
		{
			startFrame = newStartFrame;
		}

		public void SetEndFrame(int newFinishFrame)
		{
			endFrame = newFinishFrame;
		}

		public void SetMidFrames(int newMidFrames)
		{
			midFrames = newMidFrames;
		}

		public void AddSounds(List<SoundsForFrame> soundsFrameIn)
		{
			foreach (SoundsForFrame item in soundsFrameIn)
			{
				if (!soundsForFrames.ContainsKey(item.frame))
				{
					soundsForFrames.Add(item.frame, new List<SoundsForFrame>());
				}
				soundsForFrames[item.frame].Add(item);
			}
		}

		public void CopyFrameTransformByIndex(int frameNumber, int boneIndex, AnimatedTransform copyTo)
		{
			animationBinary.CopyFrameTransformByIndex(frameNumber, boneIndex, copyTo);
		}

		public void CopyFrameTransformByID(int frameNumber, int boneID, AnimatedTransform copyTo)
		{
			animationBinary.CopyFrameTransformByID(frameNumber, boneID, copyTo);
		}

		public void CopyFrameTransforms(int frameNumber, AnimatedTransform[] copyTo)
		{
			animationBinary.CopyFrameTransforms(frameNumber, copyTo);
		}

		public void CopyFrameTransforms(int frameNumber, Dictionary<int, AnimatedTransform> copyTo)
		{
			animationBinary.CopyFrameTransforms(frameNumber, copyTo);
		}

		public void InitAnimation()
		{
			name = name.ToLower();
			for (int i = 0; i < groupNames.Length; i++)
			{
				groupNames[i] = groupNames[i].ToLower();
			}
			foreach (AnimationTransition animationTransition in animationTransitions)
			{
				for (int j = 0; j < animationTransition.animations.Count; j++)
				{
					animationTransition.animations[j] = animationTransition.animations[j].ToLower();
				}
			}
			animationNames = new string[groupNames.Length + 1];
			groupNames.CopyTo(animationNames, 0);
			animationNames[animationNames.Length - 1] = name;
			midFrames++;
			if (animationBinary == null)
			{
				physical = true;
			}
			else
			{
				physical = false;
				if (startFrame > 0)
				{
					startFrame--;
				}
				else
				{
					startFrame = 0;
				}
				if (endFrame < 1)
				{
					endFrame = 0;
				}
				else
				{
					endFrame--;
					if (endFrame >= animationBinary.frames.Length)
					{
						endFrame = animationBinary.frames.Length - 1;
					}
				}
				foreach (SwitchFrame weaponSwitchFrame in weaponSwitchFrames)
				{
					if (weaponSwitchFrame.Frame > 0)
					{
						weaponSwitchFrame.Frame--;
					}
				}
				framesCount = endFrame - startFrame;
			}
			if (!physical)
			{
				blendingFrames = ((blendingFrames > framesCount) ? framesCount : ((blendingFrames >= 0) ? blendingFrames : 0));
			}
		}

		public bool HasNameOrGroup(string nameValue)
		{
			string[] array = animationNames;
			foreach (string text in array)
			{
				if (text.Equals(nameValue))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsAttacking()
		{
			return HasNameOrGroup("attack");
		}
	}
}
