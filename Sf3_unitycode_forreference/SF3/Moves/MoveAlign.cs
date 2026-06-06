using System;
using UnityEngine;

namespace SF3.Moves
{
	[Serializable]
	public class MoveAlign
	{
		public PositionObject alignPivot { get; private set; }

		public PositionObject alignPosition { get; private set; }

		public Vector3 shift { get; private set; }

		public Vector3 rotation { get; private set; }

		public bool axisX { get; private set; }

		public bool axisY { get; private set; }

		public bool axisZ { get; private set; }

		public bool followPositionObject { get; private set; }

		public void SetAlignPivot(PositionObject value)
		{
			alignPivot = value;
		}

		public void SetAlignPosition(PositionObject value)
		{
			alignPosition = value;
		}

		public void SetAxis(bool x, bool y, bool z)
		{
			axisX = x;
			axisY = y;
			axisZ = z;
		}

		public void SetRotation(Vector3 newRotation)
		{
			rotation = newRotation;
		}

		public void SetShift(Vector3 newShift)
		{
			shift = newShift;
		}

		public void SetFollowPositionObject(bool isFollow)
		{
			followPositionObject = isFollow;
		}
	}
}
