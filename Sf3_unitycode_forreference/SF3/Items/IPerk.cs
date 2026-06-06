using System;

namespace SF3.Items
{
	public interface IPerk : ISlotItem, IFactionable, IRarable, IStackable, IDescribed, ICloneable
	{
		object GetAttributeValue(string name);

		new string ToString();

		PerkType GetPerkType();

		string GetName();
	}
}
