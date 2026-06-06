using System;
using System.Collections.Generic;
using Jint.Native;
using SF3.Items;
using SF3_Attributes;
using UnityEngine;

namespace SF3.GameModels
{
	[Serializable]
	public class ModelAttributes
	{
		private Attributes _baseModelAttributes;

		private Attributes _summaryBaseAttributes;

		private Attributes _finallyAttributes;

		private ModelAttributesModifier _attributesModifier;

		public ModelAttributes()
		{
			_baseModelAttributes = new Attributes();
			_summaryBaseAttributes = new Attributes();
			_finallyAttributes = new Attributes();
		}

		public void Initialize(int modelID)
		{
			_attributesModifier = ModelsAttributesController.Instance.GetModelAttributeModifier(modelID);
		}

		public void CalculateWarriorSummaryAttributes(Dictionary<EquipmentType, Equipment> equippedItems, double stacklevel)
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<EquipmentType, Equipment> equippedItem in equippedItems)
			{
				list.Add(equippedItem.Value.id);
			}
			Dictionary<string, JsValue> warriorAttributes = JsFunction.GetWarriorAttributes(stacklevel, list.ToArray());
			Attributes attributes = new Attributes();
			attributes.SetAttributes(warriorAttributes);
			CalculateSummaryAttributes(new List<Attributes> { attributes });
		}

		public void CalculateSummaryAttributes(Dictionary<EquipmentType, Equipment> equippedItems)
		{
			List<Attributes> list = new List<Attributes>();
			foreach (Equipment value in equippedItems.Values)
			{
				list.Add(value.GetAttributesForCombat());
				if (!value.HasSlots())
				{
					continue;
				}
				foreach (ItemSlot slotItem in value.GetSlotItems())
				{
					if (slotItem.HasPerk())
					{
						IAttributable attributable = slotItem.perk as IAttributable;
						if (attributable != null)
						{
							list.Add(attributable.GetAttributesForCombat());
						}
					}
				}
			}
			CalculateSummaryAttributes(list);
		}

		public void CalculateSummaryAttributes(List<Attributes> attrs)
		{
			attrs.Add(_baseModelAttributes);
			_summaryBaseAttributes = Attributes.CalculateResultAttributes(attrs);
			_summaryBaseAttributes.ApplyFactors();
			attrs.Remove(_baseModelAttributes);
		}

		public void CalculateFinallyAttributes()
		{
			_summaryBaseAttributes.CopyTo(_finallyAttributes);
		}

		public void ApplyStrikeModifiers()
		{
			ApplyModifiers(_attributesModifier.strikeModifiers);
		}

		public void ApplyHitModifiers()
		{
			ApplyModifiers(_attributesModifier.hitModifiers);
		}

		public void ApplyModifiers(List<ModelAttributesModifier.ModifiedAttribute> attributesMods)
		{
			foreach (ModelAttributesModifier.ModifiedAttribute attributesMod in attributesMods)
			{
				if (attributesMod.isFactor)
				{
					if (_finallyAttributes.attributes.ContainsKey(attributesMod.attribute))
					{
						_finallyAttributes.attributes[attributesMod.attribute] += Mathf.Log(attributesMod.value, 4f) * 10f;
					}
				}
				else if (_finallyAttributes.attributes.ContainsKey(attributesMod.attribute))
				{
					_finallyAttributes.attributes[attributesMod.attribute] = attributesMod.value;
				}
				else
				{
					_finallyAttributes.attributes.Add(attributesMod.attribute, attributesMod.value);
				}
			}
		}

		private float GetAttribute(Attributes attrs, AttributeType attributeType)
		{
			if (attrs.attributes.ContainsKey(attributeType))
			{
				return attrs.attributes[attributeType];
			}
			Debug.LogError(string.Format("Attribute with name {0} is not exist", attributeType));
			return 0f;
		}

		public float GetSummaryAttribute(AttributeType attributeType)
		{
			return GetAttribute(_summaryBaseAttributes, attributeType);
		}

		public float GetFinallyAttribute(AttributeType attributeType)
		{
			return GetAttribute(_finallyAttributes, attributeType);
		}

		public string PrintBaseAttributes()
		{
			return _summaryBaseAttributes.ToString();
		}

		public void InheritSummaryFrom(ModelAttributes prototype)
		{
			foreach (KeyValuePair<AttributeType, float> attribute in prototype._summaryBaseAttributes.attributes)
			{
				_baseModelAttributes.attributes[attribute.Key] = attribute.Value;
			}
		}
	}
}
