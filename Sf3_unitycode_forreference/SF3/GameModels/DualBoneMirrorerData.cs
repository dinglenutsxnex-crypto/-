using System.Collections.Generic;

namespace SF3.GameModels
{
	public class DualBoneMirrorerData : BoneMirrorerData
	{
		protected List<int> _mirrorMirroredIds;

		public override bool isDual
		{
			get
			{
				return true;
			}
		}

		public override int mirroredIDsCount
		{
			get
			{
				return _mirrorMirroredIds.Count;
			}
		}

		public DualBoneMirrorerData()
		{
			_mirrorMirroredIds = new List<int>();
		}

		public override void MergeIDs(BoneMirrorerData fromDataValue)
		{
			base.MergeIDs(fromDataValue);
			DualBoneMirrorerData dualBoneMirrorerData = (DualBoneMirrorerData)fromDataValue;
			bool flag = false;
			foreach (int mirrorMirroredId in dualBoneMirrorerData._mirrorMirroredIds)
			{
				flag = false;
				for (int i = 0; i < _mirrorMirroredIds.Count; i++)
				{
					if (mirrorMirroredId == _mirrorMirroredIds[i])
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					_mirrorMirroredIds.Add(mirrorMirroredId);
				}
			}
		}

		public void AddMirrorMirroredID(int value)
		{
			_mirrorMirroredIds.Add(value);
		}

		public override int GetMirroredID(int normalID)
		{
			for (int i = 0; i < _mirrorMirroredIds.Count; i++)
			{
				if (i < _mirroredIds.Count && _mirroredIds[i] == normalID)
				{
					return _mirrorMirroredIds[i];
				}
				if (i < _mirroredIds.Count && _mirrorMirroredIds[i] == normalID)
				{
					return _mirroredIds[i];
				}
			}
			return -1;
		}

		public override int GetMirrorIdByIndex(int index)
		{
			return _mirrorMirroredIds[index];
		}
	}
}
