using System.Collections.Generic;
using SF3_Attributes;

namespace SF3.Items
{
	public interface IAttributable
	{
		Attributes GetAttributesForCombat();

		SortedDictionary<AttributeType, float> GetAttributesForDisplayData();

		SortedDictionary<AttributeType, float> GetAttributesForCombatData();

		float GetAttributesForCombatValue(AttributeType attributeKey);

		float GetAttributesForDisplayValue(AttributeType attributeKey);
	}
}
