using System.Collections.Generic;

public class BattlesComp : IComparer<IBattleInfo>
{
	public int Compare(IBattleInfo x, IBattleInfo y)
	{
		if (x.GetID() < y.GetID())
		{
			return -1;
		}
		if (x.GetID() > y.GetID())
		{
			return 1;
		}
		return 0;
	}
}
