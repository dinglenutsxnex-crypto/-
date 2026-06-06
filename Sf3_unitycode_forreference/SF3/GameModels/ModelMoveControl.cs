using System;
using UnityEngine;

namespace SF3.GameModels
{
	public class ModelMoveControl
	{
		private Model _modelState;

		private Vector3 _positionVector;

		public int directionSign
		{
			get
			{
				return (moveDirection != EDirectionType.LEFT) ? 1 : (-1);
			}
		}

		public int positionSign
		{
			get
			{
				if (_modelState.GetCenterOfMassBone().position.x < _modelState.enemy.GetCenterOfMassBone().position.x)
				{
					return -1;
				}
				return 1;
			}
		}

		public bool? forceMirrored { get; set; }

		public bool mirrored { get; private set; }

		public EDirectionType moveDirection { get; private set; }

		public int enemySign
		{
			get
			{
				return directionSign * positionSign * -1;
			}
		}

		public event Action<EDirectionType> OnDirectionChangedEvent;

		public ModelMoveControl(Model modelState, IEventSender eventSender)
		{
			_modelState = modelState;
		}

		public void Initialize()
		{
			_positionVector = Vector3.zero;
		}

		public bool GetRealMirrored()
		{
			bool flag = false;
			if (_modelState.moveControl.moveDirection == EDirectionType.RIGHT)
			{
				if (_modelState.modelComponents.leftBoneForMirror.position.x > _modelState.modelComponents.rightBoneForMirror.position.x)
				{
					return false;
				}
				return true;
			}
			if (_modelState.modelComponents.leftBoneForMirror.position.x < _modelState.modelComponents.rightBoneForMirror.position.x)
			{
				return false;
			}
			return true;
		}

		public void CheckMirrored()
		{
			if (forceMirrored.HasValue)
			{
				mirrored = forceMirrored.Value;
				forceMirrored = null;
			}
			else if (_modelState.moveControl.moveDirection == EDirectionType.RIGHT)
			{
				if (_modelState.modelComponents.leftBoneForMirror.position.x > _modelState.modelComponents.rightBoneForMirror.position.x)
				{
					mirrored = false;
				}
				else
				{
					mirrored = true;
				}
			}
			else if (_modelState.modelComponents.leftBoneForMirror.position.x < _modelState.modelComponents.rightBoneForMirror.position.x)
			{
				mirrored = false;
			}
			else
			{
				mirrored = true;
			}
		}

		public void SetMoveDirection(EDirectionType directionVal)
		{
			if (moveDirection != directionVal)
			{
				moveDirection = directionVal;
				if (this.OnDirectionChangedEvent != null)
				{
					this.OnDirectionChangedEvent(directionVal);
				}
			}
		}

		public void AddRepulsive(Vector3 repulsive)
		{
			_positionVector = _modelState.modelComponents.rootBone.position;
			_positionVector.x += repulsive.x;
			_modelState.modelComponents.rootBone.SetPosition(_positionVector);
		}

		public string GetYDirection()
		{
			Vector3 forward = _modelState.modelComponents.GetBone("head").transform.forward;
			string result = "Upward";
			if (forward.y < 0f)
			{
				result = "Downward";
			}
			return result;
		}
	}
}
