using System.Collections.Generic;
using UnityEngine;

namespace SF3.Tactics
{
	public class Product
	{
		public List<InfoProduct> attackList;

		public List<InfoProduct> hitList;

		public List<InfoProduct> dodgeList;

		public List<InfoProduct> list;

		public Product()
		{
			attackList = new List<InfoProduct>();
			hitList = new List<InfoProduct>();
			dodgeList = new List<InfoProduct>();
			list = new List<InfoProduct>();
		}

		public void Add(InfoProduct info)
		{
			foreach (InfoProduct item in list)
			{
				if (info.animationIndex == item.animationIndex && info.infoType != item.infoType)
				{
					Debug.LogError(string.Concat("DUBLE INFO_PRODUCT - ", info.animationName, " ", info.infoType, " ", item.infoType));
				}
			}
			list.Add(info);
			switch (info.infoType)
			{
			case InfoType.HIT_A:
				attackList.Add(info);
				break;
			case InfoType.BOTH:
			case InfoType.HIT_B:
				hitList.Add(info);
				break;
			default:
				dodgeList.Add(info);
				break;
			}
		}

		public void SortProduct()
		{
		}

		private void AddAtack(InfoProduct info)
		{
		}
	}
}
