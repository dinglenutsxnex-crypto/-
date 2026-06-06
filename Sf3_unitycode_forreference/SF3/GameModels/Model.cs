using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using DG.Tweening.Core.Surrogates;
using Nekki;
using SF3.Effects;
using SF3.Items;
using SF3.Moves;
using SF3.Settings;
using SF3_Attributes;
using UnityEngine;
using sf3DTO;

namespace SF3.GameModels
{
	[Serializable]
	public class Model : ExtentionBehaviour, IEventSender, IAnimatedModel, ISkinnedModel, IModelHUD, IShadowFormModel
	{
		public bool disableAnimation;

		[HideInInspector]
		public bool manualAnimation;

		[HideInInspector]
		public bool animationSelected;

		[HideInInspector]
		public int animationFrame;

		[HideInInspector]
		public string animationName;

		[HideInInspector]
		public int curAnimationFrames;

		[HideInInspector]
		public string[] animationsNames;

		public static HitResult hitResult;

		private ModelShadow _modelShadow;

		private int _id;

		private ModelDissolveWeapon modelDessolveWeapon;

		private PlayerBattleKeyManager _battleKeys;

		public static bool disableEffects;

		private Coroutine animateTransperentCoroutine;

		private bool isEventFloorHit;

		private int countEventSlowDown;

		private int _blockableCount;

		private int _dodgeableCount;

		public const int PLAYER_ID = 1;

		public const int ENEMY_ID = 2;

		private static int _nextModelIDGlobal;

		public const string ARMATURE_BONE_NAME = "Armature";

		public const string LEFT_MIRROR_BONE_NAME = "foot_l";

		public const string RIGHT_MIRROR_BONE_NAME = "foot_r";

		public const string WEAPON_L_BONE_NAME = "weapon_l";

		public const string WEAPON_R_BONE_NAME = "weapon_r";

		private static readonly Dictionary<string, EPlayerType> _modelSelectionTypeCompliance;

		private static readonly Dictionary<string, EPivotObject> _pivotObjectCompliance;

		public InfoAnimation AnimationCurrent
		{
			get
			{
				return modelAnimation.mainAnimationInfo.animation;
			}
		}

		public ModelComponents modelComponents { get; private set; }

		public Model enemy { get; private set; }

		public Model parentModel { get; private set; }

		public List<Model> childModels { get; private set; }

		public ModelInfo modelInfo { get; private set; }

		public ModelMoveControl moveControl { get; private set; }

		public int id
		{
			get
			{
				return _id;
			}
		}

		public ModelCollision modelCollisions { get; private set; }

		public ModelMoves modelMoves { get; private set; }

		public ModelAnimation modelAnimation { get; private set; }

		public ModelShadowForm modelShadowForm { get; private set; }

		public RagdolAlignment ragdolAlignment { get; private set; }

		public ModelStatusControl statusControl { get; private set; }

		public List<IntervalAnimation> animationIntervals
		{
			get
			{
				return modelAnimation.animationIntervals;
			}
		}

		public bool isControl
		{
			get
			{
				if (!modelInfo.isControl)
				{
					return false;
				}
				return modelInfo.isControl;
			}
		}

		public bool isPlayer
		{
			get
			{
				return modelInfo.isPlayer;
			}
		}

		public bool IsAI
		{
			get
			{
				return GetAiMode() != AiMode.NoneMode;
			}
		}

		public float health
		{
			get
			{
				return modelInfo.currentLife;
			}
		}

		public ModelInfoAnimation animationInfo
		{
			get
			{
				return modelAnimation.mainAnimationInfo;
			}
		}

		public PlayerBattleKeyManager keyManager
		{
			get
			{
				if (_battleKeys == null)
				{
					_battleKeys = BattleKeyManager.Instance.GetBattleKeysByModelID(id);
				}
				return _battleKeys;
			}
		}

		public bool isDead { get; private set; }

		public bool active { get; private set; }

		public float Transparency
		{
			get
			{
				return modelComponents.Transparency;
			}
		}

		public Collider CurrentWallCollider { get; private set; }

		public ModelAi Ai { get; private set; }

		static Model()
		{
			hitResult = default(HitResult);
			_nextModelIDGlobal = 2;
			_modelSelectionTypeCompliance = new Dictionary<string, EPlayerType>
			{
				{
					"Me",
					EPlayerType.This
				},
				{
					"Enemy",
					EPlayerType.Enemy
				},
				{
					"Parent",
					EPlayerType.Parent
				},
				{
					"Both",
					EPlayerType.Both
				},
				{
					"Child",
					EPlayerType.Child
				}
			};
			_pivotObjectCompliance = new Dictionary<string, EPivotObject>
			{
				{
					"Nodes",
					EPivotObject.ObjectNodes
				},
				{
					"Wall",
					EPivotObject.ObjectWall
				},
				{
					"Animation",
					EPivotObject.ObjectAnimation
				},
				{
					"Pivot",
					EPivotObject.ObjectPivot
				},
				{
					"COM",
					EPivotObject.ObjectCOM
				}
			};
		}

		public void UpdateAnimation()
		{
			modelAnimation.Update();
		}

		public void UpdateShadow()
		{
			if (ShadowMapController.Instance != null)
			{
				UnityEngine.Color color = ShadowMapController.Instance.CalculateShadowAtPoint(modelComponents.rootBone.position.x);
				ChangeSkinsColor(color, false);
			}
		}

		public void UpdateFloor()
		{
			if (!modelComponents.modelPhysics.ragdollActive)
			{
				return;
			}
			float num = Convert.ToSingle(FightSettings.GetEventProperty(ETriggerEvents.EVENT_FLOOR_HIT, "Height"));
			float num2 = SceneConfig.PointFloor + num;
			float num3 = modelComponents.modelCapsules.floorRepulsion.GetMinYPosition() - num2;
			if (num3 <= 0f)
			{
				if (!isEventFloorHit)
				{
					ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_FLOOR_HIT));
					isEventFloorHit = true;
				}
				UpdateVelocity();
			}
			else
			{
				isEventFloorHit = false;
				countEventSlowDown = 0;
			}
		}

		private void UpdateVelocity()
		{
			int num = Convert.ToInt32(FightSettings.GetEventProperty(ETriggerEvents.EVENT_SLOWDOWN, "Min"));
			int num2 = Convert.ToInt32(FightSettings.GetEventProperty(ETriggerEvents.EVENT_SLOWDOWN, "Max"));
			Bone centerOfMassBone = modelComponents.centerOfMassBone;
			float num3 = Convert.ToSingle(FightSettings.GetEventProperty(ETriggerEvents.EVENT_SLOWDOWN, "SpeedThreshold"));
			float num4 = Math.Abs(centerOfMassBone.position.x - centerOfMassBone.previousPosition.x);
			if ((countEventSlowDown > num && num4 <= num3) || countEventSlowDown > num2)
			{
				ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_SLOWDOWN, -2));
			}
			countEventSlowDown++;
		}

		public int[] GetBonesIDs()
		{
			return modelComponents.GetBonesIDs();
		}

		public void SetRagdollActive(bool isActive)
		{
			isEventFloorHit = false;
			countEventSlowDown = 0;
			modelComponents.modelPhysics.SetRagdollActive(isActive);
		}

		public string GetName()
		{
			return base.name;
		}

		public EDirectionType GetMoveDirection()
		{
			return moveControl.moveDirection;
		}

		public bool GetMirrored()
		{
			return moveControl.mirrored;
		}

		public void CheckMirrored(int? forceValue)
		{
			moveControl.CheckMirrored();
			if (forceValue.HasValue)
			{
				if (forceValue == -1)
				{
					moveControl.forceMirrored = !moveControl.mirrored;
				}
				else if (forceValue.GetValueOrDefault() == 0 && forceValue.HasValue)
				{
					moveControl.forceMirrored = false;
				}
				else if (forceValue == 1)
				{
					moveControl.forceMirrored = true;
				}
				moveControl.CheckMirrored();
			}
		}

		public void SetTransparent(float value)
		{
			modelComponents.SetTransparent(value);
		}

		public void SetOpaque()
		{
			modelComponents.SetOpaque();
		}

		private void AnimateTransperent(float from, float to, float duration, Action animationEndCallback = null)
		{
			animateTransperentCoroutine = StartCoroutine(AnimateTransperentCorutine(from, to, duration, animationEndCallback));
		}

		private IEnumerator AnimateTransperentCorutine(float from, float to, float duration, Action animationEndCallback = null)
		{
			float startTime = Time.time;
			float endTime = startTime + duration;
			while (Time.time <= endTime)
			{
				float timePassed = Time.time - startTime;
				float currentAlpha = Mathf.Lerp(from, to, timePassed / duration);
				SetTransparent(currentAlpha);
				yield return new WaitForEndOfFrame();
			}
			SetTransparent(to);
			if (to > from && from > 0.999f)
			{
				SetOpaque();
			}
			animateTransperentCoroutine = null;
			animationEndCallback.InvokeSafe();
		}

		public bool IsAnimateTransparentInprogress()
		{
			return animateTransperentCoroutine != null;
		}

		public void AnimateShadowColor(UnityEngine.Color to, float duration, Action animationEndCallback = null)
		{
			DOTween.To(() => _modelShadow.ShadowTextureRenderer.shadowColor, delegate(ColorWrapper x)
			{
				_modelShadow.ShadowTextureRenderer.shadowColor = x;
			}, to, duration).OnComplete(animationEndCallback.InvokeSafe);
		}

		public void Fade(float duration = 1f, bool show = false)
		{
			StopCoroutineSafe(animateTransperentCoroutine);
			animateTransperentCoroutine = null;
			modelComponents.IsVisible = show;
			if (!show)
			{
				AnimateTransperent(1f, 0f, duration);
				AnimateShadowColor(UnityEngine.Color.white, duration);
			}
			else
			{
				AnimateTransperent(0f, 1f, duration);
				AnimateShadowColor(_modelShadow.SavedShadowColor, duration);
			}
		}

		public void SetMoveDirection(EDirectionType directionVal)
		{
			moveControl.SetMoveDirection(directionVal);
		}

		public void FillAnimatedTransforms(ref ModelAnimation.BlendAnimatedTransforms result)
		{
			modelComponents.FillAnimatedTransforms(ref result);
		}

		public void UpdateBonesPositions(Dictionary<int, AnimatedTransform> bonesAnimationTransforms)
		{
			modelComponents.UpdateBonesPositions(bonesAnimationTransforms);
		}

		public ModelInfoAnimation GetModelAnimationByName(string name)
		{
			return modelMoves.GetModelAnimationByName(name);
		}

		public void ResetInterval()
		{
			modelCollisions.ResetInterval();
		}

		public void AddForce(Vector3 impulse, string capsuleName)
		{
			modelComponents.modelPhysics.AddForce(impulse, capsuleName);
		}

		public void ActivateRagdolAlignment(bool zImpulse)
		{
			ragdolAlignment.Activate(zImpulse);
		}

		public bool GetIsPhysics()
		{
			return modelComponents.modelPhysics.ragdollActive;
		}

		public void RemoveModelVariable(string varName)
		{
			GameVariables.RemoveVariable(id, varName);
		}

		public void AddAttackingCapsules(int intervalID, List<Capsule> capsules)
		{
			modelComponents.AddAttackingCapsules(intervalID, capsules);
		}

		public void ResetBonesPosition()
		{
			modelComponents.ResetBonesPosition();
		}

		public void ClearAttackingCapsules()
		{
			modelComponents.ClearAttackingCapsules();
		}

		public void ClearAttackingCapsules(int intervalID)
		{
			modelComponents.ClearAttackingCapsules(intervalID);
		}

		public string GetYDirection()
		{
			return moveControl.GetYDirection();
		}

		public int GetDirectionSign()
		{
			return moveControl.directionSign;
		}

		public Bone GetPivotBone()
		{
			return modelComponents.pivotBone;
		}

		public void EnableAttakingCollisions(bool isEnable)
		{
			modelCollisions.EnableAttackCollisions(isEnable);
		}

		public void EnableRepulsionCollisions(bool isEnable)
		{
			modelCollisions.EnableRepulsionCollisions(isEnable);
		}

		public void SetRagdollSleepState(bool isSleep, int priority)
		{
			modelComponents.modelPhysics.SetRagdollSleepState(isSleep, priority);
		}

		public List<IntervalAttack> getLastIntervalAttack()
		{
			return modelCollisions.intervals;
		}

		public void SetEnemy(Model enemyModel)
		{
			enemy = enemyModel;
		}

		public List<IntervalAnimation> GetIntervalExist(EIntervalsType intervalType)
		{
			return modelAnimation.GetIntervalTypeExist(intervalType);
		}

		public void ShiftPosition(Vector3 vector)
		{
			modelComponents.rootBone.ShiftPosition(vector);
			modelAnimation.ShiftPosition(vector);
		}

		public Bone GetBone(string nameBone)
		{
			return modelComponents.GetBone(nameBone);
		}

		public Bone GetBone(int boneID)
		{
			return modelComponents.GetBone(boneID);
		}

		public int GetEquippedIDForType(EquipmentType type)
		{
			return modelInfo.GetEquippedIDForType(type);
		}

		public Equipment GetEquippedForType(EquipmentType type)
		{
			return modelInfo.GetEquippedByType(type);
		}

		public Bone GetCenterOfMassBone()
		{
			return modelComponents.centerOfMassBone;
		}

		public bool IsNeedsMirroringModelObjectSwitch()
		{
			moveControl.CheckMirrored();
			return modelComponents.IsNeedsMirroringModelObjectSwitch(moveControl.mirrored);
		}

		public void ShowSkins(bool isShow)
		{
			modelComponents.ShowSkins(isShow, moveControl.mirrored);
		}

		public void ChangeSkinsColor(UnityEngine.Color color, bool useAlpha = true)
		{
			modelComponents.ChangeSkinsColor(color, useAlpha);
		}

		public void SetParent(Model newParent)
		{
			parentModel = newParent;
		}

		public List<InfoTrigger> GetEventTriggers(ETriggerEvents eventType)
		{
			return modelMoves.GetTriggersByEvent(eventType);
		}

		public void UpdateEquationLineCollision()
		{
			modelComponents.modelCapsules.UpdateEquationLineCollision();
		}

		public string GetAlias()
		{
			if (modelInfo == null)
			{
				return string.Empty;
			}
			return modelInfo.alias;
		}

		public float GetCurrentLife()
		{
			if (modelInfo == null)
			{
				return 0f;
			}
			return modelInfo.currentLife;
		}

		public float GetMaxLife()
		{
			if (modelInfo == null)
			{
				return 0f;
			}
			return modelInfo.maxLife;
		}

		public int GetScore()
		{
			if (modelInfo == null)
			{
				return 0;
			}
			return modelInfo.score;
		}

		public void ActivateShadowForm(bool instantly = false)
		{
			modelShadowForm.ActivateShadowForm(instantly);
		}

		public void DisableShadowForm()
		{
			modelShadowForm.DisableShadowForm();
		}

		public bool GetShadowFormActive()
		{
			return modelShadowForm.shadowFormActive;
		}

		public void UpdateShadowForm()
		{
			modelShadowForm.Update();
		}

		public void SetGender(Gender gender)
		{
			modelInfo.SetGender(gender);
			EquipItem(modelInfo.GetEquipped(EquipmentType.Armor).id, false);
			EquipItem(modelInfo.GetEquipped(EquipmentType.Helmet).id, false);
		}

		public Gender GetGender()
		{
			return modelInfo.gender;
		}

		public bool IsEquipmentEquipped(int id)
		{
			Equipment equipmentByID = modelInfo.GetEquipmentByID(id);
			return equipmentByID != null && modelInfo.GetEquipmentByID(id).IsEquipped();
		}

		public void UpdateModelInfo()
		{
			ModelsManager.Instance.DisposeModelsChild(id);
			EffectsManager.Instance.DisposeEffectsByModel(id);
			modelComponents.CreateModelObjects();
			modelComponents.ShowSkins(true, moveControl.mirrored);
			if (modelComponents.IsVisible)
			{
				modelComponents.SetTransparent(1f);
			}
			else
			{
				modelComponents.SetTransparent(0f);
			}
			InitializeModelAnimation(true);
			SetShadowCommandBufferDirty();
			if (GetEquippedIDForType(EquipmentType.Ranged) != -1)
			{
				ActionButtons.Instance.MissileButtonEnable(true);
			}
			else
			{
				ActionButtons.Instance.MissileButtonEnable(false);
			}
		}

		public void EquipItem(int equippedItemID, bool throwEventEquip = true)
		{
			ModelsManager.Instance.DisposeModelsChild(id);
			EffectsManager.Instance.DisposeEffectsByModel(id);
			ModelInfo.EquippedResult equippedResult = modelInfo.EquipItem(equippedItemID);
			if (equippedResult != null && !string.IsNullOrEmpty(equippedResult.newItem.model))
			{
				modelComponents.EquipItem(equippedResult.newItem.GetEquipmentType(), equippedResult.newItem, moveControl.mirrored);
				InitializeModelAnimation(true);
				SetShadowCommandBufferDirty();
				if (equippedResult.newItem.GetEquipmentType() == EquipmentType.Ranged)
				{
					ActionButtons.Instance.MissileButtonEnable(true);
				}
				if (throwEventEquip)
				{
					ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_EQUIP, -2, equippedResult.newItem.model));
				}
				UpdateShadow();
			}
		}

		public void UnEquipItem(EquipmentType type, bool throwEventEquip = true)
		{
			int equippedIDForType = modelInfo.GetEquippedIDForType(type);
			if (equippedIDForType >= 0)
			{
				ModelsManager.Instance.DisposeModelsChild(id);
				EffectsManager.Instance.DisposeEffectsByModel(id);
				Equipment defaultEquipment = modelInfo.GetDefaultEquipment(type);
				if (defaultEquipment == null)
				{
					defaultEquipment = Equipment.GetDefaultEquipment(type);
				}
				if (defaultEquipment != null)
				{
					EquipItem(defaultEquipment.id);
				}
				else
				{
					modelInfo.UnEquipItem(equippedIDForType);
					modelComponents.UnEquipItem(type, moveControl.mirrored);
					InitializeModelAnimation(true);
					SetShadowCommandBufferDirty();
				}
				if (type == EquipmentType.Ranged)
				{
					ActionButtons.Instance.MissileButtonEnable(false);
				}
				if (throwEventEquip)
				{
					ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_EQUIP, -2, type));
				}
				UpdateShadow();
			}
		}

		public void EquipItemNotExisted(Equipment newItem, bool throwEventEquip = true)
		{
			ModelsManager.Instance.DisposeModelsChild(id);
			EffectsManager.Instance.DisposeEffectsByModel(id);
			modelInfo.EquipItemNotExisted(newItem);
			modelComponents.EquipItem(newItem.GetEquipmentType(), newItem, moveControl.mirrored);
			SetShadowCommandBufferDirty();
			if (newItem.GetEquipmentType() == EquipmentType.Ranged)
			{
				ActionButtons.Instance.MissileButtonEnable(true);
			}
			if (throwEventEquip)
			{
				ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_EQUIP, -2, newItem.model));
			}
			InitializeModelAnimation(true);
		}

		public void UnEquipItemNotExisted(Equipment newItem, bool throwEventEquip = true)
		{
			if (modelInfo.UnEquipItemNotExisted(newItem))
			{
				ModelsManager.Instance.DisposeModelsChild(id);
				EffectsManager.Instance.DisposeEffectsByModel(id);
				EquipmentType equipmentType = newItem.GetEquipmentType();
				Equipment equipped = modelInfo.GetEquipped(equipmentType);
				if (equipped != null)
				{
					modelComponents.EquipItem(equipmentType, equipped, moveControl.mirrored);
				}
				else
				{
					modelComponents.UnEquipItem(equipmentType, moveControl.mirrored);
				}
				SetShadowCommandBufferDirty();
				if (equipmentType == EquipmentType.Ranged)
				{
					ActionButtons.Instance.MissileButtonEnable(false);
				}
				InitializeModelAnimation(true);
				if (throwEventEquip)
				{
					ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_EQUIP, -2, (equipped == null) ? string.Empty : equipped.model));
				}
			}
		}

		public void EquipItemNotExisted(IPerk newItem, bool throwEventEquip = true)
		{
			ModelsManager.Instance.DisposeModelsChild(id);
			EffectsManager.Instance.DisposeEffectsByModel(id);
			modelInfo.AddPerkInCollection(newItem);
			InitializeModelAnimation(true);
		}

		public void UnEquipItemNotExisted(IPerk newItem, bool throwEventEquip = true)
		{
			ModelsManager.Instance.DisposeModelsChild(id);
			EffectsManager.Instance.DisposeEffectsByModel(id);
			modelInfo.RemovePerkFromCollection(newItem);
			InitializeModelAnimation(true);
		}

		public void ThrowEvent(BattleEventArgs args)
		{
			args.SetSender(id);
			BattleController.ThrowEvent(args);
		}

		public void CheckKeyInverted()
		{
			bool invert = enemy.modelComponents.rootBone.position.x < modelComponents.rootBone.position.x;
			if (keyManager != null)
			{
				keyManager.Invert(invert);
			}
			if (id == 1)
			{
				BattleInterface.Instance.ShadowPerksInvert(invert);
			}
		}

		private void OnHit()
		{
			modelInfo.attributes.CalculateFinallyAttributes();
			modelInfo.attributes.ApplyStrikeModifiers();
			modelInfo.attributes.ApplyHitModifiers();
			enemy.modelInfo.attributes.ApplyHitModifiers();
			hitResult.CalculateHitOnMe(this);
			TakeHit();
		}

		private void TakeHit()
		{
			if (FightController.Settings.IsScoreFight)
			{
				GameVariables.LocalVariable variable = GameVariables.GetVariable(id, "BLOCK");
				if (variable == null || !(Math.Abs((double)variable.value - 1.0) < 0.009999999776482582))
				{
					enemy.modelInfo.IncreaseScore(1);
					if (FightController.Settings.ScoreCount > 0 && enemy.modelInfo.score >= FightController.Settings.ScoreCount)
					{
						FightController.Instance.WinCurrentRound((!enemy.isPlayer) ? ERoundResult.ENEMY_WIN : ERoundResult.PLAYER_WIN);
					}
				}
			}
			if (FightController.Settings.IsHpFight)
			{
				modelInfo.ChangeLife(0f - hitResult.DamageTaken);
			}
			if (hitResult.StrikeData.intervalAttack.impulse != Vector3.zero)
			{
				string rigidBodyName = string.Empty;
				if (hitResult.StrikeData.collisionEdge != null)
				{
					rigidBodyName = hitResult.StrikeData.collisionEdge.rigidBodyName;
				}
				modelAnimation.AddImpulse(hitResult.Impulse, rigidBodyName);
			}
			ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_POST_HIT, -2, hitResult));
		}

		public void ApplyEvent(BattleEventArgs args)
		{
			if (args.SenderID != id)
			{
				return;
			}
			modelDessolveWeapon.ApplyEvent(args);
			switch (args.EventType)
			{
			case ETriggerEvents.EVENT_STRIKE:
				DisableSlowMotion();
				StrikeEnemyModel((StrikeData)args.EventData);
				break;
			case ETriggerEvents.EVENT_HIT:
				OnHit();
				break;
			case ETriggerEvents.EVENT_ANIMATION_START:
				SetRepulsionRectScale(args);
				_dodgeableCount = modelAnimation.mainAnimationInfo.modelAttackingIntervals.Count;
				_blockableCount = modelAnimation.mainAnimationInfo.modelAttackingIntervals.Count;
				break;
			case ETriggerEvents.EVENT_INTERVAL_START:
				SlowmotionOnIntervalEvent(args, true);
				DodgeCheckOnIntervalEvent(args);
				BlockCheckOnIntervalEvent(args);
				break;
			case ETriggerEvents.EVENT_INTERVAL_END:
				SlowmotionOnIntervalEvent(args, false);
				break;
			case ETriggerEvents.EVENT_BORDER_HIT:
				if (EffectsManager.slowMotionActive)
				{
					DisableSlowMotion();
				}
				break;
			}
		}

		private void StrikeEnemyModel(StrikeData strike)
		{
			modelInfo.attributes.CalculateFinallyAttributes();
			modelInfo.attributes.ApplyStrikeModifiers();
			strike.direction = moveControl.directionSign;
			strike.attackingModel = this;
			strike.criticalHit = ((NekkiMath.randomFloat(0f, 1f) <= strike.attackingModel.modelInfo.attributes.GetFinallyAttribute(AttributeType.CriticalChance)) ? 1 : 0);
			hitResult.SetStrikeData(strike);
			enemy.ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_HIT, -2, strike));
		}

		private void OnZeroHP()
		{
			isDead = true;
		}

		public ModelObject GetWeapon()
		{
			return modelComponents.GetEquipedModelObject(EquipmentType.Weapon);
		}

		public void RestrictAnimations(List<string> animations)
		{
			modelMoves.RestrictAnimations(animations);
		}

		private void SetRepulsionRectScale(BattleEventArgs args)
		{
			modelComponents.modelCapsules.repulsionRect.SetScale(((ModelInfoAnimation)args.EventData).animation.repulsionScale);
		}

		public void DisableSlowMotion()
		{
			if (EffectsManager.slowMotionActive)
			{
				EffectsManager.StopAll("LastHitSlowmotion");
				EffectsManager.StopAll("DollyZoom");
				EffectsManager.PlayEffect(this, "DisableSlowmotion", id);
			}
		}

		private void SlowmotionOnIntervalEvent(BattleEventArgs args, bool enable)
		{
			if (BattlesManager.currentBattleType == BattleType.Dojo || args.SenderID != id || EffectsManager.slowMotionActive == enable || ((IntervalAnimation)args.EventData).type != EIntervalsType.INTERVAL_SLOWMOTION)
			{
				return;
			}
			IntervalSlowmotion intervalSlowmotion = (IntervalSlowmotion)args.EventData;
			if (enable)
			{
				if (intervalSlowmotion.distance >= 0 && GetDistanceToEnemy() <= (float)intervalSlowmotion.distance && !(hitResult.CalculateDamageCanBeDoneToEnemy(intervalSlowmotion.intervalAttack, this) < enemy.health) && !(enemy.health <= 0f))
				{
					SlowMotionEffectEnable(enable);
				}
			}
			else if (!modelAnimation.mainAnimationInfo.animation.looped)
			{
				SlowMotionEffectEnable(enable);
			}
		}

		private void SlowMotionEffectEnable(bool enable)
		{
			if (enable)
			{
				EffectsManager.PlayEffect(this, "LastHitSlowmotion", id);
				EffectsManager.PlayEffect(this, "DollyZoom", id);
				EffectsManager.PlayEffect(this, "ActivateSlowmotion", id);
			}
			else
			{
				EffectsManager.StopAll("LastHitSlowmotion");
				EffectsManager.StopAll("DollyZoom");
				EffectsManager.PlayEffect(this, "DisableSlowmotion", id);
			}
		}

		private void BlockCheckOnIntervalEvent(BattleEventArgs args)
		{
			if (_blockableCount <= 0 || args.SenderID != id || ((IntervalAnimation)args.EventData).type != EIntervalsType.INTERVAL_BLOCKABLE)
			{
				return;
			}
			IntervalBlockable intervalBlockable = (IntervalBlockable)args.EventData;
			if (intervalBlockable.distance >= 0)
			{
				float distanceToEnemy = GetDistanceToEnemy();
				if (distanceToEnemy <= (float)intervalBlockable.distance)
				{
					_blockableCount--;
					hitResult.SetStrikeData(new StrikeData(intervalBlockable.intervalAttack));
					enemy.ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_BLOCK_CHECK, -2, intervalBlockable));
				}
			}
		}

		private void DodgeCheckOnIntervalEvent(BattleEventArgs args)
		{
			if (_dodgeableCount > 0 && args.SenderID == id && ((IntervalAnimation)args.EventData).type == EIntervalsType.INTERVAL_DODGEABLE)
			{
				IntervalDodgeable intervalDodgeable = (IntervalDodgeable)args.EventData;
				if (intervalDodgeable.distance >= 0 && GetDistanceToEnemy() <= (float)intervalDodgeable.distance)
				{
					_dodgeableCount--;
					hitResult.SetStrikeData(new StrikeData(intervalDodgeable.intervalAttack));
					enemy.ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_DODGE_CHECK, -2, intervalDodgeable));
				}
			}
		}

		private float GetDistanceToEnemy()
		{
			return Math.Abs(enemy.modelComponents.centerOfMassBone.position.x - modelComponents.centerOfMassBone.position.x);
		}

		public Vector3 GetPositionOf(DistancePoint.EDistanceObject objectType, string objectName)
		{
			Vector3 result;
			switch (objectType)
			{
			case DistancePoint.EDistanceObject.OBJECT_COM:
				return modelComponents.centerOfMassBone.position;
			case DistancePoint.EDistanceObject.OBJECT_NODES:
				return GetBone(objectName).position;
			case DistancePoint.EDistanceObject.OBJECT_PIVOT:
				return modelComponents.pivotBone.position;
			case DistancePoint.EDistanceObject.OBJECT_WALL:
				result = ((!objectName.Equals("Back")) ? ((moveControl.moveDirection != EDirectionType.RIGHT) ? new Vector3(SceneConfig.LocationLeftBorder, 0f, 0f) : new Vector3(SceneConfig.LocationRightBorder, 0f, 0f)) : ((moveControl.moveDirection != EDirectionType.RIGHT) ? new Vector3(SceneConfig.LocationRightBorder, 0f, 0f) : new Vector3(SceneConfig.LocationLeftBorder, 0f, 0f)));
				break;
			case DistancePoint.EDistanceObject.OBJECT_CENTER:
				result = new Vector3(SceneConfig.CenterX, 0f, 0f);
				break;
			default:
				Debug.LogError("Unexpected object argument ^^");
				return Vector3.zero;
			}
			return result;
		}

		public void SetShadowCommandBufferDirty()
		{
			if (_modelShadow != null)
			{
				_modelShadow.SetCommandBufferDirty();
			}
		}

		public void WallHit(WallConfig.EWallType wallType, Collider wall)
		{
			CurrentWallCollider = wall;
			ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_WALL_HIT, -2, wallType));
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			PropertyInfo[] properties = GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo.CanWrite && !propertyInfo.PropertyType.IsValueType && propertyInfo.Name != "tag")
				{
					propertyInfo.SetValue(this, null, null);
				}
			}
			FieldInfo[] fields = GetType().GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				if (!fieldInfo.IsStatic && !fieldInfo.IsInitOnly && !fieldInfo.FieldType.IsValueType)
				{
					fieldInfo.SetValue(this, null);
				}
			}
		}

		public void Destroy()
		{
			EffectsManager.Instance.DisposeEffectsByModel(id);
			GlobalLoad.Unload(base.gameObject);
		}

		public int GetId()
		{
			return _id;
		}

		public void SetHair(string hair)
		{
			modelInfo.SetHair(hair);
			EquipItem(modelInfo.GetDefaultEquipment(EquipmentType.Helmet).id);
		}

		private void LateUpdate()
		{
			if (modelDessolveWeapon != null)
			{
				modelDessolveWeapon.EndFrameUpdate();
			}
		}

		public void OverrideMaterial(string materialName, bool value, float transitionTime)
		{
			modelComponents.OverrideMaterial(materialName, value, transitionTime);
		}

		public void SwitchWeapon(bool toMirror)
		{
			ModelObject weapon = GetWeapon();
			weapon.ShowSkins(true, toMirror ^ moveControl.mirrored);
		}

		public void Activate(bool isActive)
		{
			active = isActive;
			base.gameObject.SetActive(active);
			modelComponents.modelCapsules.RenderCapsules(isActive);
		}

		public static Model Create(ModelInfo _modelInfo, int modelID = -1)
		{
			if (_modelInfo == null)
			{
				throw new Exception(string.Format("ModelInfo is null"));
			}
			Model model = new GameObject().AddComponent<Model>();
			model.CreateModel(_modelInfo, modelID);
			return model;
		}

		private void CreateModel(ModelInfo _modelInfo, int modelID = -1)
		{
			_nextModelIDGlobal++;
			if (modelID == -1)
			{
				_id = _nextModelIDGlobal;
			}
			else
			{
				_id = modelID;
			}
			modelInfo = _modelInfo;
			modelInfo.Initialize(id);
			base.gameObject.name = _modelInfo.alias;
			base.transform.position = Vector3.zero;
			base.gameObject.AddComponent<ObjectMaterialsControll>();
			statusControl = new ModelStatusControl(this);
			moveControl = new ModelMoveControl(this, this);
			modelComponents = new ModelComponents(modelInfo, this, base.transform);
			modelComponents.SetTransparent(1f);
			_modelShadow = GetComponentInChildren<ModelShadow>();
			if (_modelShadow != null)
			{
				_modelShadow.CreateShadow();
			}
			ragdolAlignment = base.gameObject.GetComponent<RagdolAlignment>();
			if (ragdolAlignment == null)
			{
				ragdolAlignment = base.gameObject.AddComponent<RagdolAlignment>();
			}
			ragdolAlignment.Initialize(modelComponents.rootBone.transform);
			modelCollisions = new ModelCollision(this);
			modelShadowForm = new ModelShadowForm(id, this, isPlayer, modelComponents);
			CreateAi();
			childModels = new List<Model>();
			modelAnimation = new ModelAnimation(this);
			modelMoves = new ModelMoves();
			base.gameObject.layer = LayerMask.NameToLayer("CharacterAndFoe");
			modelDessolveWeapon = new ModelDissolveWeapon(modelComponents, modelShadowForm, modelAnimation);
			Activate(false);
			if (modelID == 1)
			{
				StickHelper.Instance.ClearDisarmCooldowns();
			}
		}

		public void Initialize(Model _parentModel = null, bool needInit = true)
		{
			Debug.Log("[Model Initialize]");
			isDead = false;
			InitDebugAnimationsList();
			if (modelInfo.isPlayer)
			{
				moveControl.forceMirrored = false;
			}
			else
			{
				moveControl.forceMirrored = true;
			}
			childModels.Clear();
			statusControl.Reset();
			modelComponents.Initialize();
			if (_modelShadow != null)
			{
				_modelShadow.Initialize();
			}
			modelInfo.ResetModelInfoStats();
			modelInfo.onZeroHP += OnZeroHP;
			parentModel = _parentModel;
			if (parentModel != null)
			{
				modelInfo.attributes.InheritSummaryFrom(parentModel.modelInfo.attributes);
				enemy = parentModel.enemy;
			}
			else if (isPlayer)
			{
				SetEnemy(ModelsManager.Instance.Enemy);
			}
			else
			{
				SetEnemy(ModelsManager.Instance.Player);
			}
			using (new TimerNode("INIT ANIMS " + id, "Model.Initialize"))
			{
				InitializeModelAnimation();
			}
			InitializeAi();
			isEventFloorHit = false;
			countEventSlowDown = 0;
			modelCollisions.Initialize();
			moveControl.Initialize();
			modelShadowForm.Reset();
			ShowSkins(true);
			SetOpaque();
			ApplyMaskColor();
			modelComponents.modelPhysics.SetToRagdollPose();
			Activate(true);
			if (needInit)
			{
				ThrowBirth();
			}
		}

		public void ThrowBirth()
		{
			BattleController.ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_BIRTH, id));
		}

		private void InitDebugAnimationsList()
		{
			List<InfoAnimation> animations = MovesController.GetAnimations();
			animationsNames = new string[animations.Count];
			for (int i = 0; i < animationsNames.Length; i++)
			{
				animationsNames[i] = animations[i].name;
			}
		}

		private void ApplyMaskColor()
		{
			if (!isPlayer)
			{
				if (IsItemEqual(EquipmentType.Armor))
				{
					modelComponents.ApplyMaskColor(EquipmentType.Armor);
				}
				if (IsItemEqual(EquipmentType.Helmet))
				{
					modelComponents.ApplyMaskColor(EquipmentType.Helmet);
				}
			}
		}

		private bool IsItemEqual(EquipmentType type)
		{
			Equipment equipped = ModelsManager.Instance.Enemy.modelInfo.GetEquipped(type);
			Equipment equipped2 = ModelsManager.Instance.Player.modelInfo.GetEquipped(type);
			return equipped != null && equipped2 != null && equipped.model == equipped2.model;
		}

		public void InitializeModelAnimation(bool sendEndAnimEvent = false)
		{
			modelMoves.LoadMoves(this);
			modelAnimation.Initialize();
			if (sendEndAnimEvent)
			{
				ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_ANIMATION_END, id));
			}
		}

		public static EPlayerType GetPlayerTypeByName(string typeName)
		{
			if (_modelSelectionTypeCompliance.ContainsKey(typeName))
			{
				return _modelSelectionTypeCompliance[typeName];
			}
			Debug.LogError("ModelType - parseType - unknownType: " + typeName);
			return EPlayerType.None;
		}

		public static EPivotObject GetPivotObjectByName(string pivotObjectName)
		{
			if (_pivotObjectCompliance.ContainsKey(pivotObjectName))
			{
				return _pivotObjectCompliance[pivotObjectName];
			}
			Debug.LogError("EPivotObject - parseType - unknownType: " + pivotObjectName);
			return EPivotObject.ObjectNone;
		}

		public static Model GetModelByType(Model modelValue, EPlayerType playerTypeValue)
		{
			switch (playerTypeValue)
			{
			case EPlayerType.None:
			case EPlayerType.This:
				return modelValue;
			case EPlayerType.Parent:
				return modelValue.parentModel;
			case EPlayerType.Enemy:
				return modelValue.enemy;
			default:
				throw new Exception(string.Format("Cant get player by type [{0}]", playerTypeValue));
			}
		}

		public static IAnimatedModel GetAnimatedModelByType(IAnimatedModel animatedModel, EPlayerType playerTypeValue)
		{
			switch (playerTypeValue)
			{
			case EPlayerType.None:
			case EPlayerType.This:
				return animatedModel;
			case EPlayerType.Parent:
				return ((Model)animatedModel).parentModel;
			case EPlayerType.Enemy:
				return ((Model)animatedModel).enemy;
			default:
				throw new Exception(string.Format("Cant get player by type [{0}]", playerTypeValue));
			}
		}

		private void CreateAi()
		{
			Ai = new ModelAi(this);
		}

		private void InitializeAi()
		{
			Ai.Initialize();
		}

		public void UpdateAi()
		{
			Ai.UpdateAi();
		}

		public int GetEnemySign()
		{
			return moveControl.enemySign;
		}

		public void SetAiMode(AiMode mode)
		{
			modelInfo.SetAIMode(mode);
			Ai.ChangeAiModeTo(mode);
		}

		public AiMode GetAiMode()
		{
			return modelInfo.aiMode;
		}
	}
}
