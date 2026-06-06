using System;
using System.Collections.Generic;
using SF3.GameModels;

namespace SF3.Moves
{
	[Serializable]
	public class MirrorerInfoContainer
	{
		private List<AnimationMirrorer> _mirrorers;

		private IBonesHolder _bonesHolder;

		public bool enabled { get; private set; }

		public MirrorerInfoContainer()
		{
			enabled = false;
		}

		public void Mirror(Dictionary<int, AnimatedTransform> currentFrameBoneAnim)
		{
			if (!enabled)
			{
				return;
			}
			foreach (AnimationMirrorer mirrorer in _mirrorers)
			{
				mirrorer.CrutchMirror(currentFrameBoneAnim, _bonesHolder);
			}
		}

		public void Init(BonesMirrorContainer mirrorContainer, IBonesHolder bonesHolder, int[] bonesID)
		{
			SelfMirrorerXY selfMirrorerXY = new SelfMirrorerXY();
			SelfMirrorerYZ selfMirrorerYZ = new SelfMirrorerYZ();
			SelfMirrorerXZ selfMirrorerXZ = new SelfMirrorerXZ();
			DualSimpleMirrorer dualSimpleMirrorer = new DualSimpleMirrorer();
			DualXYMirrorer dualXYMirrorer = new DualXYMirrorer();
			DualYZMirrorer dualYZMirrorer = new DualYZMirrorer();
			DualXZMirrorer dualXZMirrorer = new DualXZMirrorer();
			_mirrorers = new List<AnimationMirrorer>();
			_mirrorers.Add(selfMirrorerXY);
			_mirrorers.Add(selfMirrorerYZ);
			_mirrorers.Add(selfMirrorerXZ);
			_mirrorers.Add(dualSimpleMirrorer);
			_mirrorers.Add(dualXYMirrorer);
			_mirrorers.Add(dualYZMirrorer);
			_mirrorers.Add(dualXZMirrorer);
			_bonesHolder = bonesHolder;
			foreach (BoneMirrorerData item in mirrorContainer.mirroringByAxis)
			{
				bool isDual = item.isDual;
				MirrorAxisID axisType = item.axisType;
				AnimationMirrorer animationMirrorer = null;
				if (isDual)
				{
					switch (axisType)
					{
					case MirrorAxisID.None:
						animationMirrorer = dualSimpleMirrorer;
						break;
					case MirrorAxisID.XY:
						animationMirrorer = dualXYMirrorer;
						break;
					case MirrorAxisID.YZ:
						animationMirrorer = dualYZMirrorer;
						break;
					case MirrorAxisID.XZ:
						animationMirrorer = dualXZMirrorer;
						break;
					}
				}
				else
				{
					switch (axisType)
					{
					case MirrorAxisID.XY:
						animationMirrorer = selfMirrorerXY;
						break;
					case MirrorAxisID.YZ:
						animationMirrorer = selfMirrorerYZ;
						break;
					case MirrorAxisID.XZ:
						animationMirrorer = selfMirrorerXZ;
						break;
					}
				}
				for (int i = 0; i < item.mirroredIDsCount; i++)
				{
					int mirrorIdByIndex = item.GetMirrorIdByIndex(i);
					int mirroredID = item.GetMirroredID(mirrorIdByIndex);
					bool flag = false;
					foreach (int num in bonesID)
					{
						if (num == mirrorIdByIndex || num == mirroredID)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						animationMirrorer.AddMirrorIndexes(mirrorIdByIndex, mirroredID);
						enabled = true;
					}
				}
			}
		}
	}
}
