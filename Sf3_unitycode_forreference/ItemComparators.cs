using System.Collections.Generic;
using SF3.Items;

public static class ItemComparators
{
	private static readonly Dictionary<ComparerPurpose, IComparer<BaseItem>> Comparators;

	static ItemComparators()
	{
		Comparators = new Dictionary<ComparerPurpose, IComparer<BaseItem>>
		{
			{
				ComparerPurpose.ReelDriverAsc,
				new ReelDriverComparerAsc()
			},
			{
				ComparerPurpose.ReelDriverDesc,
				new ReelDriverComparerDesc()
			}
		};
	}

	public static IComparer<BaseItem> GetComparator(ComparerPurpose purpose)
	{
		return Comparators[purpose];
	}
}
