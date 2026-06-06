using System.Collections.Generic;

namespace SF3.GameModels
{
	public class BonesMirrorContainer
	{
		public List<BoneMirrorerData> mirroringByAxis { get; private set; }

		public BonesMirrorContainer()
		{
			mirroringByAxis = new List<BoneMirrorerData>();
		}

		public void AddBoneMirroring(BonesMirrorContainer mirrorDataValue)
		{
			foreach (BoneMirrorerData item in mirrorDataValue.mirroringByAxis)
			{
				BoneMirrorerData boneMirrorerData = null;
				foreach (BoneMirrorerData item2 in mirroringByAxis)
				{
					if (item.isDual == item2.isDual && item.axisType == item2.axisType)
					{
						boneMirrorerData = item2;
						break;
					}
				}
				if (boneMirrorerData == null)
				{
					mirroringByAxis.Add(item);
				}
				else
				{
					boneMirrorerData.MergeIDs(item);
				}
			}
		}

		public void AddBoneMirroring(List<BoneMirrorerData> mirrorDataValue)
		{
			mirroringByAxis.AddRange(mirrorDataValue);
		}

		public void AddBoneMirroring(BoneMirrorerData mirrorDataValue)
		{
			mirroringByAxis.Add(mirrorDataValue);
		}

		public int GetMirrorID(int normalID)
		{
			foreach (BoneMirrorerData item in mirroringByAxis)
			{
				int mirroredID = item.GetMirroredID(normalID);
				if (mirroredID != -1)
				{
					return mirroredID;
				}
			}
			return -1;
		}

		public BoneMirrorerData GetBoneMirrorerData(int boneID)
		{
			foreach (BoneMirrorerData item in mirroringByAxis)
			{
				if (item.GetMirroredID(boneID) != -1)
				{
					return item;
				}
			}
			return null;
		}
	}
}
