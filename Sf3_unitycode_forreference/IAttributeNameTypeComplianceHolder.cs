using SF3_Attributes;

public interface IAttributeNameTypeComplianceHolder
{
	AttributeType GetAttributeTypeByName(string name);

	string GetAttributeNameByType(AttributeType type);
}
