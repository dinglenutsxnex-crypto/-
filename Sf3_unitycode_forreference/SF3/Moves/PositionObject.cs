using System;
using SF3.GameModels;
using UnityEngine;

namespace SF3.Moves
{
	[Serializable]
	public class PositionObject
	{
		private EPlayerType _playerType;

		private EPivotObject _pivotObject;

		private string _partName;

		public EPlayerType playerType
		{
			get
			{
				return _playerType;
			}
			set
			{
				_playerType = value;
			}
		}

		public EPivotObject pivotObject
		{
			get
			{
				return _pivotObject;
			}
			set
			{
				_pivotObject = value;
			}
		}

		public string partName
		{
			get
			{
				return _partName;
			}
			set
			{
				_partName = value;
			}
		}

		public PositionObject()
		{
			_partName = string.Empty;
			_playerType = EPlayerType.None;
			_pivotObject = EPivotObject.ObjectNone;
		}

		public Vector3 GetPosition(Model modelState, bool useMirroring = false)
		{
			Vector3 result = Vector3.zero;
			Model modelByType = Model.GetModelByType(modelState, playerType);
			switch (_pivotObject)
			{
			case EPivotObject.ObjectAnimation:
				Debug.LogWarning("Havnt any actions here. Default result returned.");
				break;
			case EPivotObject.ObjectNodes:
				result = GetNodesPosition(modelByType, useMirroring);
				break;
			case EPivotObject.ObjectWall:
				result = GetWallPosition(modelByType);
				break;
			case EPivotObject.ObjectPivot:
				result = GetPivotPosition(modelByType, useMirroring);
				break;
			case EPivotObject.ObjectCOM:
				result = GetCOMPosition(modelByType);
				break;
			default:
				throw new Exception(string.Format("Cant find pivot object [{0}]", _pivotObject));
			}
			return result;
		}

		private Vector3 GetNodesPosition(Model modelState, bool useMirroring = false)
		{
			Bone bone = modelState.GetBone(partName);
			if (useMirroring && modelState.moveControl.mirrored)
			{
				bone = modelState.GetBone(modelState.modelComponents.GetBoneMirrorID(bone.boneID));
			}
			return bone.position;
		}

		private Vector3 GetCOMPosition(Model modelState)
		{
			return modelState.modelComponents.centerOfMassBone.position;
		}

		private Vector3 GetPivotPosition(Model modelState, bool useMirroring = false)
		{
			Bone pivotBone = modelState.modelComponents.pivotBone;
			return pivotBone.position;
		}

		private Vector3 GetWallPosition(Model modelState)
		{
			Vector3 zero = Vector3.zero;
			if (partName.Equals("Back"))
			{
				if (modelState.moveControl.moveDirection == EDirectionType.LEFT)
				{
					zero.x = SceneConfig.LocationRightBorder;
				}
				else
				{
					zero.x = SceneConfig.LocationLeftBorder;
				}
			}
			else if (modelState.moveControl.moveDirection == EDirectionType.LEFT)
			{
				zero.x = SceneConfig.LocationLeftBorder;
			}
			else
			{
				zero.x = SceneConfig.LocationRightBorder;
			}
			return zero;
		}
	}
}
