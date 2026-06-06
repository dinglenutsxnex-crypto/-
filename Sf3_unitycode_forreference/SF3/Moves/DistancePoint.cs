using System;
using System.Collections.Generic;
using Nekki.Yaml;
using SF3.GameModels;
using UnityEngine;

namespace SF3.Moves
{
	[Serializable]
	public class DistancePoint
	{
		public enum EDistanceObject
		{
			OBJECT_NULL = 0,
			OBJECT_NODES = 1,
			OBJECT_PIVOT = 2,
			OBJECT_WALL = 3,
			OBJECT_FLOOR = 4,
			OBJECT_COM = 5,
			OBJECT_CENTER = 6,
			OBJECT_FRONT = 7,
			OBJECT_BACK = 8
		}

		public enum EDistanceFrame
		{
			DISTANCE_FRAME_NULL = 0,
			DISTANCE_FRAME_CURRENT = 1,
			DISTANCE_FRAME_PREVIOUS = 2
		}

		public enum EDistanceImpulse
		{
			IMPULSE_NONE = 0,
			IMPULSE_NOT_REVERSE = 1,
			IMPULSE_REVERSE = 2
		}

		private static Dictionary<string, EDistanceObject> _distanceObjectNamesCompliance;

		private EPlayerType _modelType;

		private EDistanceObject _distObject;

		private EDistanceFrame _frame;

		private string _part;

		private float _shiftX;

		private float _shiftY;

		private bool _isBackWall;

		static DistancePoint()
		{
			_distanceObjectNamesCompliance = new Dictionary<string, EDistanceObject>();
			_distanceObjectNamesCompliance.Add("Wall", EDistanceObject.OBJECT_WALL);
			_distanceObjectNamesCompliance.Add("Nodes", EDistanceObject.OBJECT_NODES);
			_distanceObjectNamesCompliance.Add("Bone", EDistanceObject.OBJECT_NODES);
			_distanceObjectNamesCompliance.Add("Pivot", EDistanceObject.OBJECT_PIVOT);
			_distanceObjectNamesCompliance.Add("Floor", EDistanceObject.OBJECT_FLOOR);
			_distanceObjectNamesCompliance.Add("COM", EDistanceObject.OBJECT_COM);
			_distanceObjectNamesCompliance.Add("Center", EDistanceObject.OBJECT_CENTER);
			_distanceObjectNamesCompliance.Add("Front", EDistanceObject.OBJECT_FRONT);
			_distanceObjectNamesCompliance.Add("Back", EDistanceObject.OBJECT_BACK);
		}

		public DistancePoint(Mapping distancePointMap)
		{
			_isBackWall = false;
			_frame = EDistanceFrame.DISTANCE_FRAME_CURRENT;
			_shiftX = 0f;
			_shiftY = 0f;
			Scalar text = distancePointMap.GetText("Player");
			if (text != null)
			{
				_modelType = Model.GetPlayerTypeByName(text.text);
			}
			text = distancePointMap.GetText("Object");
			if (text != null)
			{
				_distObject = GetDistanceObjectByName(text.text);
			}
			text = distancePointMap.GetText("Part");
			if (text != null)
			{
				_part = text.text;
				if (_distObject == EDistanceObject.OBJECT_WALL && _part == "Back")
				{
					_isBackWall = true;
				}
			}
		}

		public float GetPositionX(IAnimatedModel animatedModel)
		{
			return GetPosition(animatedModel).x;
		}

		public float GetPositionY(Model modelState)
		{
			return GetPosition(modelState).y * -1f;
		}

		public Vector3 GetPosition(IAnimatedModel animatedModel)
		{
			IAnimatedModel animatedModelByType = Model.GetAnimatedModelByType(animatedModel, _modelType);
			if (animatedModelByType == null)
			{
				return new Vector3(-50000f, 0f, 0f);
			}
			Vector3 result = Vector3.zero;
			switch (_distObject)
			{
			case EDistanceObject.OBJECT_NODES:
				result = GetNode(animatedModelByType);
				result.x += _shiftX * (float)animatedModelByType.GetDirectionSign();
				result.y -= _shiftY;
				break;
			case EDistanceObject.OBJECT_PIVOT:
				result = GetPivotNode(animatedModelByType);
				result.x += _shiftX * (float)animatedModelByType.GetDirectionSign();
				result.y -= _shiftY;
				break;
			case EDistanceObject.OBJECT_WALL:
				result = GetWallPoint(animatedModelByType);
				result.x += _shiftX * (float)animatedModelByType.GetDirectionSign();
				result.y -= _shiftY;
				break;
			case EDistanceObject.OBJECT_FLOOR:
				result = new Vector3(_shiftX, 0f - _shiftY, 0f);
				break;
			case EDistanceObject.OBJECT_COM:
				Debug.Log("Temporary disabled");
				result = Vector3.zero;
				break;
			case EDistanceObject.OBJECT_CENTER:
				result = new Vector3(SceneConfig.CenterX, 0f, 0f);
				break;
			case EDistanceObject.OBJECT_FRONT:
				result = Vector3.zero;
				if (animatedModel.GetIsPhysics())
				{
					if (animatedModel.GetYDirection() == "Upward")
					{
						result.x = (animatedModelByType.GetBone("pelvis").position.x - animatedModelByType.GetBone("head").position.x) * float.MaxValue;
					}
					else
					{
						result.x = (animatedModelByType.GetBone("head").position.x - animatedModelByType.GetBone("pelvis").position.x) * float.MaxValue;
					}
				}
				else
				{
					result.x = (float)animatedModelByType.GetDirectionSign() * float.MaxValue;
				}
				break;
			case EDistanceObject.OBJECT_BACK:
				result = Vector3.zero;
				if (animatedModel.GetIsPhysics())
				{
					Debug.Log("Back");
					if (animatedModel.GetYDirection() == "Upward")
					{
						result.x = (animatedModelByType.GetBone("head").position.x - animatedModelByType.GetBone("pelvis").position.x) * float.MaxValue;
					}
					else
					{
						result.x = (animatedModelByType.GetBone("pelvis").position.x - animatedModelByType.GetBone("head").position.x) * float.MaxValue;
					}
				}
				else
				{
					result.x = (float)animatedModelByType.GetDirectionSign() * float.MaxValue;
				}
				break;
			default:
				Debug.LogError("ERROR: unknown object type: " + _distObject);
				break;
			}
			return result;
		}

		protected Vector3 GetNode(IAnimatedModel animatedModel)
		{
			return GetPositionFrame(animatedModel.GetBone(_part));
		}

		protected Vector3 GetPivotNode(IAnimatedModel animatedModel)
		{
			return GetPositionFrame(animatedModel.GetPivotBone());
		}

		protected Vector3 GetPositionFrame(Bone modelBone)
		{
			switch (_frame)
			{
			case EDistanceFrame.DISTANCE_FRAME_CURRENT:
				return modelBone.position;
			case EDistanceFrame.DISTANCE_FRAME_PREVIOUS:
				return modelBone.previousPosition;
			default:
				Debug.LogError("DistancePoint: getPositionFrame - unknown frame: " + _frame);
				return modelBone.position;
			}
		}

		protected Vector3 GetWallPoint(IAnimatedModel animatedModel)
		{
			return (animatedModel.GetDirectionSign() == 1 != _isBackWall) ? new Vector3(SceneConfig.LocationRightBorder, 0f, 0f) : new Vector3(SceneConfig.LocationLeftBorder, 0f, 0f);
		}

		public static EDistanceObject GetDistanceObjectByName(string distanceObjectName)
		{
			if (_distanceObjectNamesCompliance.ContainsKey(distanceObjectName))
			{
				return _distanceObjectNamesCompliance[distanceObjectName];
			}
			Debug.LogError("Distance Object - parseType - unknownType: " + distanceObjectName);
			return EDistanceObject.OBJECT_NULL;
		}
	}
}
