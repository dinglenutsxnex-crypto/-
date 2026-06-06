using System.Collections.Generic;
using SF3.Moves;

namespace SF3.GameModels
{
	public class BoneMirrorerData
	{
		protected List<int> _mirroredIds;

		public MirrorAxisID axisType { get; private set; }

		public virtual bool isDual
		{
			get
			{
				return false;
			}
		}

		public virtual int mirroredIDsCount
		{
			get
			{
				return _mirroredIds.Count;
			}
		}

		public BoneMirrorerData()
		{
			_mirroredIds = new List<int>();
		}

		public virtual int GetMirroredID(int normalID)
		{
			return normalID;
		}

		public void AddMirroredID(int value)
		{
			_mirroredIds.Add(value);
		}

		public void SetAxis(MirrorAxisID axisTypeValue)
		{
			axisType = axisTypeValue;
		}

		public virtual int GetMirrorIdByIndex(int index)
		{
			return _mirroredIds[index];
		}

		public virtual void MergeIDs(BoneMirrorerData fromData)
		{
			bool flag = false;
			foreach (int mirroredId in fromData._mirroredIds)
			{
				flag = false;
				for (int i = 0; i < _mirroredIds.Count; i++)
				{
					if (mirroredId == _mirroredIds[i])
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					_mirroredIds.Add(mirroredId);
				}
			}
		}
	}
}
