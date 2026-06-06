using SF3.Items;
using SF3_Attributes;

public class AttributeNameTypeComplianceHolder : EnumCompliantHolder<AttributeType>, IAttributeNameTypeComplianceHolder
{
	public AttributeType GetAttributeTypeByName(string name)
	{
		return GetValueByKey(name);
	}

	public string GetAttributeNameByType(AttributeType type)
	{
		return GetKeyByType(type);
	}

	protected override AttributeType GetValueDefault()
	{
		return AttributeType.None;
	}

	protected override string GetCompliantStringNameByType(AttributeType type)
	{
		return type.ToString();
	}
}
