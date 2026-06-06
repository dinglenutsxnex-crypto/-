using System;
using UnityEngine;

namespace SF3.Items
{
	[Serializable]
	public class AtlasForType
	{
		[SerializeField]
		private ShopCategoryType _subType;

		[SerializeField]
		private UIAtlas _atlas;

		public ShopCategoryType SubType
		{
			get
			{
				return _subType;
			}
		}

		public UIAtlas Atlas
		{
			get
			{
				return _atlas;
			}
		}
	}
}
