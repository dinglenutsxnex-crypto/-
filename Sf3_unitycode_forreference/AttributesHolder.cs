using System;
using System.Collections.Generic;
using Jint.Native;
using SF3.Items;
using SF3_Attributes;

public class AttributesHolder : IAttributesHolder, ICloneable
{
	private readonly Dictionary<AttributePurpose, Attributes> _holder;

	private AttributesHolder()
	{
		_holder = new Dictionary<AttributePurpose, Attributes>();
	}

	public AttributesHolder(EquipmentType type)
		: this()
	{
		foreach (AttributePurpose value in Enum.GetValues(typeof(AttributePurpose)))
		{
			_holder.Add(value, new Attributes());
		}
	}

	public object Clone()
	{
		AttributesHolder attributesHolder = new AttributesHolder();
		foreach (KeyValuePair<AttributePurpose, Attributes> item in _holder)
		{
			attributesHolder._holder.Add(item.Key, (Attributes)item.Value.Clone());
		}
		return attributesHolder;
	}

	public void Clear()
	{
		_holder.Clear();
	}

	public void UpdateAttributesVisible(JsValue attributesNew)
	{
		_holder[AttributePurpose.Display].SetAttributes(attributesNew.AsDictionary());
	}

	public void UpdateAttributesBattle(JsValue attributesNew)
	{
		_holder[AttributePurpose.Combat].SetAttributes(attributesNew.AsDictionary());
	}

	public Attributes GetAttributes(AttributePurpose purpose)
	{
		if (_holder.ContainsKey(purpose))
		{
			return _holder[purpose];
		}
		return new Attributes();
	}

	public SortedDictionary<AttributeType, float> GetAttributesData(AttributePurpose purpose)
	{
		return GetAttributes(purpose).attributes;
	}

	public float GetAttributeValue(AttributePurpose purpose, AttributeType key)
	{
		return GetAttributes(purpose).GetAttribute(key);
	}
}
