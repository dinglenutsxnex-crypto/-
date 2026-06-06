using System;

namespace SF3
{
	[Serializable]
	public class LocationFloorSurfaceSettings
	{
		[Serializable]
		public class FloorSurface
		{
			public float startX;

			public float endX;

			public ESurfaceType surfaceType;
		}

		public float surfaceY;

		public FloorSurface[] floorSurface;
	}
}
