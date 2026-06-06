using System;
using System.Collections.Generic;
using Nekki;
using SF3.Moves;
using UnityEngine;

namespace SF3.GameModels
{
	public class ModelCollision
	{
		public class CheckCollisionResult
		{
			public bool isMainCollision;

			public bool isCollision;

			public CheckCollisionResult()
			{
				isMainCollision = false;
				isCollision = false;
			}
		}

		private StrikeData strike;

		private HashSet<IntervalAttack> _interval;

		private Model _modelState;

		private IEventSender _eventSender;

		private RepulsionCalculator _repulsionCalculator;

		private bool _attackCollisionsEnable;

		public float modelBackPos { get; private set; }

		public float modelBackPosPrevios { get; private set; }

		public List<IntervalAttack> intervals
		{
			get
			{
				return new List<IntervalAttack>(_interval);
			}
		}

		public bool RepulsionCollisionsEnabled { get; private set; }

		public bool RepulsionCollisionsEnabledBoth
		{
			get
			{
				return RepulsionCollisionsEnabled && _modelState.enemy.modelCollisions.RepulsionCollisionsEnabled;
			}
		}

		public event Action<float> OnRepulsion = delegate
		{
		};

		public ModelCollision(Model modelState)
		{
			_modelState = modelState;
			_eventSender = modelState;
			strike = new StrikeData();
			_interval = new HashSet<IntervalAttack>();
			_repulsionCalculator = new RepulsionCalculator();
			EnableAttackCollisions(false);
			EnableRepulsionCollisions(false);
		}

		public void Initialize()
		{
			ResetInterval();
			EnableAttackCollisions(true);
			EnableRepulsionCollisions(true);
			CalculateModelBackPosition();
			modelBackPosPrevios = modelBackPos;
		}

		public void EnableAttackCollisions(bool isEnable)
		{
			_attackCollisionsEnable = isEnable;
		}

		public void EnableRepulsionCollisions(bool isEnable)
		{
			RepulsionCollisionsEnabled = isEnable;
		}

		public bool CheckAttackCollisions()
		{
			bool result = false;
			if (_attackCollisionsEnable && _modelState.animationInfo != null && _modelState.enemy.GetIntervalExist(EIntervalsType.INTERVAL_INVINCIBLE).Count == 0)
			{
				List<IntervalAnimation> intervalExist = _modelState.GetIntervalExist(EIntervalsType.INTERVAL_ATTACK);
				if (intervalExist.Count > 0 && !_modelState.enemy.isDead)
				{
					foreach (IntervalAttack item in intervalExist)
					{
						if (CheckCollisions(item))
						{
							_eventSender.ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_STRIKE, -2, strike));
							result = true;
						}
					}
				}
			}
			return result;
		}

		public bool CheckRepulsion()
		{
			if (_modelState.enemy.GetIntervalExist(EIntervalsType.INTERVAL_NO_REPULSION).Count == 0)
			{
				IntervalExcludeRepulsion intervalExcludeRepulsion = (IntervalExcludeRepulsion)_modelState.modelAnimation.animationIntervals.Find((IntervalAnimation e) => e is IntervalExcludeRepulsion);
				if (intervalExcludeRepulsion != null)
				{
					_modelState.modelComponents.modelCapsules.repulsionRect.exclusiveKeys = ((!_modelState.GetMirrored()) ? intervalExcludeRepulsion.bonesToExclude : intervalExcludeRepulsion.bonesToExcludeMirrored);
				}
				else
				{
					_modelState.modelComponents.modelCapsules.repulsionRect.exclusiveKeys = null;
				}
				IntervalExcludeRepulsion intervalExcludeRepulsion2 = (IntervalExcludeRepulsion)_modelState.enemy.modelAnimation.animationIntervals.Find((IntervalAnimation e) => e is IntervalExcludeRepulsion);
				if (intervalExcludeRepulsion2 != null)
				{
					_modelState.enemy.modelComponents.modelCapsules.repulsionRect.exclusiveKeys = ((!_modelState.enemy.GetMirrored()) ? intervalExcludeRepulsion2.bonesToExclude : intervalExcludeRepulsion2.bonesToExcludeMirrored);
				}
				else
				{
					_modelState.enemy.modelComponents.modelCapsules.repulsionRect.exclusiveKeys = null;
				}
				return CrossModel(_modelState.modelComponents.modelCapsules.repulsionRect, _modelState.enemy.modelComponents.modelCapsules.repulsionRect);
			}
			return false;
		}

		public float GetOffsetFromWall()
		{
			if (modelBackPos < SceneConfig.LeftBorderX)
			{
				return SceneConfig.LeftBorderX - modelBackPos;
			}
			if (modelBackPos > SceneConfig.RightBorderX)
			{
				return SceneConfig.RightBorderX - modelBackPos;
			}
			return 0f;
		}

		public void CheckBorderHit()
		{
			CalculateModelBackPosition();
			modelBackPos = Mathf.Lerp(modelBackPosPrevios, modelBackPos, 0.15f);
			if (modelBackPosPrevios < SceneConfig.LocationRightBorder && modelBackPos >= SceneConfig.LocationRightBorder)
			{
				_eventSender.ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_BORDER_HIT, -2));
			}
			else if (modelBackPosPrevios > SceneConfig.LocationLeftBorder && modelBackPos <= SceneConfig.LocationLeftBorder)
			{
				_eventSender.ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_BORDER_HIT, -2));
			}
			modelBackPosPrevios = modelBackPos;
		}

		public void CalculateModelBackPosition()
		{
			if (_modelState.GetIsPhysics())
			{
				if (_modelState.modelComponents.rootBone.position.x > _modelState.enemy.modelComponents.rootBone.position.x)
				{
					modelBackPos = _modelState.modelComponents.modelCapsules.repulsionRect.pointRight;
				}
				else
				{
					modelBackPos = _modelState.modelComponents.modelCapsules.repulsionRect.pointLeft;
				}
			}
			else
			{
				modelBackPos = _modelState.modelComponents.rootBone.position.x;
			}
		}

		public bool CheckWalls(bool isRepulsion)
		{
			CalculateModelBackPosition();
			float offsetFromWall = GetOffsetFromWall();
			bool flag = offsetFromWall != 0f;
			float num = modelBackPos - _modelState.modelComponents.rootBone.position.x;
			if (flag)
			{
				_modelState.ShiftPosition(new Vector3(offsetFromWall, 0f, 0f));
				modelBackPos = ((!_modelState.GetIsPhysics()) ? _modelState.modelComponents.rootBone.position.x : (_modelState.modelComponents.rootBone.position.x + num));
				if (isRepulsion)
				{
					_modelState.enemy.ShiftPosition(new Vector3(offsetFromWall, 0f, 0f));
				}
			}
			return flag && isRepulsion;
		}

		private void RepulsionBones()
		{
			int positionSign = _modelState.moveControl.positionSign;
			float x = _modelState.modelComponents.centerOfMassBone.previousPosition.x;
			float x2 = _modelState.enemy.modelComponents.centerOfMassBone.previousPosition.x;
			float x3 = _modelState.modelComponents.centerOfMassBone.position.x;
			float x4 = _modelState.enemy.modelComponents.centerOfMassBone.position.x;
			float num = (float)positionSign * _repulsionCalculator.Calculate(x, x2, x3, x4);
			if (RepulsionCollisionsEnabledBoth)
			{
				_modelState.ShiftPosition(new Vector3(num, 0f, 0f));
				_modelState.enemy.ShiftPosition(new Vector3(0f - num, 0f, 0f));
			}
			else
			{
				if (RepulsionCollisionsEnabled)
				{
					_modelState.ShiftPosition(new Vector3(num * 2f, 0f, 0f));
				}
				if (_modelState.enemy.modelCollisions.RepulsionCollisionsEnabled)
				{
					_modelState.enemy.ShiftPosition(new Vector3((0f - num) * 2f, 0f, 0f));
				}
			}
			this.OnRepulsion(Mathf.Abs(num * 2f));
		}

		private bool CheckCollisions(IntervalAttack intervalAttack)
		{
			if (_interval.Contains(intervalAttack))
			{
				return false;
			}
			if (!intervalAttack.isCollision)
			{
				_interval.Add(intervalAttack);
				strike.intervalAttack = intervalAttack;
				strike.Reset();
				return true;
			}
			foreach (KeyValuePair<int, List<Capsule>> attackingCapsule in _modelState.modelComponents.modelCapsules.attackingCapsules)
			{
				foreach (Capsule item in attackingCapsule.Value)
				{
					if (CrossModel(item, _modelState.enemy.modelComponents.modelCapsules.collisionCapsules, false))
					{
						_interval.Add(intervalAttack);
						strike.intervalAttack = intervalAttack;
						strike.GetMaterials(_modelState);
						return true;
					}
				}
			}
			return false;
		}

		private bool CrossModel(RepulsionRect myRect, RepulsionRect enemyRect)
		{
			myRect.Calclulate();
			enemyRect.Calclulate();
			if (myRect.center.y > enemyRect.center.y)
			{
				if (myRect.pointBot >= enemyRect.pointUp)
				{
					return false;
				}
			}
			else if (enemyRect.pointBot >= myRect.pointUp)
			{
				return false;
			}
			if (myRect.center.x > enemyRect.center.x)
			{
				float num = enemyRect.pointRight - myRect.pointLeft;
				if (num > 0f)
				{
					RepulsionBones();
					return true;
				}
			}
			else
			{
				float num2 = myRect.pointRight - enemyRect.pointLeft;
				if (num2 > 0f)
				{
					RepulsionBones();
					return true;
				}
			}
			return false;
		}

		private bool CrossModel(Capsule attackingcapsule, List<Capsule> capsules, bool isRepulsive)
		{
			Vector2 resultA = Vector3.zero;
			Vector2 resultB = Vector3.zero;
			float radius = attackingcapsule.radius;
			Vector3 startMarginPosition = attackingcapsule.startMarginPosition;
			Vector3 finishMarginPosition = attackingcapsule.finishMarginPosition;
			EquationLine equationLine = attackingcapsule.equationLine;
			foreach (Capsule capsule in capsules)
			{
				float radius2 = capsule.radius;
				Vector3 startMarginPosition2 = capsule.startMarginPosition;
				Vector3 finishMarginPosition2 = capsule.finishMarginPosition;
				EquationLine equationLine2 = capsule.equationLine;
				if (Vector2D.getIntersectOfCapsulesPoint2D(startMarginPosition, finishMarginPosition, radius, startMarginPosition2, finishMarginPosition2, radius2, ref resultA, ref resultB, equationLine, equationLine2))
				{
					float num = (startMarginPosition.z + finishMarginPosition.z) / 2f;
					float num2 = (startMarginPosition2.z + finishMarginPosition2.z) / 2f;
					float middleZ = (num + num2) / 2f;
					UpdateStrikeHit(attackingcapsule, capsule, resultA, resultB, middleZ, isRepulsive);
					return true;
				}
			}
			return false;
		}

		private void UpdateStrikeHit(Capsule attackingEdge, Capsule collisionEdge, Vector3 point, Vector3 pointStrikeEffect, float middleZ, bool isRepulsive)
		{
			if (isRepulsive)
			{
				RepulsionBones();
				return;
			}
			strike.attackEdge = attackingEdge;
			strike.collisionEdge = collisionEdge;
			strike.strikePoint = point;
			strike.strikeEffectPoint = pointStrikeEffect;
			strike.strikePoint.z = (strike.strikeEffectPoint.z = middleZ);
		}

		public void ResetInterval()
		{
			_interval.Clear();
		}
	}
}
