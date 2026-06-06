using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jint.Native;
using Nekki.Yaml;
using SimpleJSON;
using UnityEngine;

namespace SF3_Attributes
{
	[Serializable]
	public class Attributes : ICloneable
	{
		public class SortAttributeComparer : IComparer<AttributeType>
		{
			private readonly Dictionary<AttributeType, int> _atributeNamesForSorted = new Dictionary<AttributeType, int>
			{
				{
					AttributeType.None,
					0
				},
				{
					AttributeType.WeaponDamage,
					1
				},
				{
					AttributeType.BodyDefense,
					2
				},
				{
					AttributeType.CriticalChance,
					3
				},
				{
					AttributeType.HeadDefense,
					4
				},
				{
					AttributeType.UnarmedDamage,
					5
				},
				{
					AttributeType.RangedDamage,
					6
				},
				{
					AttributeType.MagicPower,
					7
				},
				{
					AttributeType.Cooldown,
					8
				}
			};

			public int Compare(AttributeType x, AttributeType y)
			{
				return _atributeNamesForSorted[x].CompareTo(_atributeNamesForSorted[y]);
			}
		}

		private SortedDictionary<AttributeType, float> baseAttributes { get; set; }

		public SortedDictionary<AttributeType, float> attributes { get; protected set; }

		public Dictionary<AttributeType, float> factors { get; protected set; }

		public Attributes()
		{
			baseAttributes = new SortedDictionary<AttributeType, float>(new SortAttributeComparer());
			attributes = new SortedDictionary<AttributeType, float>(new SortAttributeComparer());
			factors = new Dictionary<AttributeType, float>();
		}

		public void SetAttributes(Dictionary<string, JsValue> attributesNew)
		{
			baseAttributes.Clear();
			foreach (KeyValuePair<string, JsValue> item in attributesNew)
			{
				AttributeType attributeTypeByName = ComplianceUtils.GetAttributeTypeByName(item.Key);
				baseAttributes.Add(attributeTypeByName, (float)item.Value.AsNumber());
			}
			ResetAttributes();
		}

		public void ResetAttributes()
		{
			attributes = new SortedDictionary<AttributeType, float>(baseAttributes, new SortAttributeComparer());
		}

		public object Clone()
		{
			Attributes attributes = new Attributes();
			foreach (KeyValuePair<AttributeType, float> baseAttribute in baseAttributes)
			{
				attributes.baseAttributes.Add(baseAttribute.Key, baseAttribute.Value);
			}
			foreach (KeyValuePair<AttributeType, float> factor in factors)
			{
				attributes.factors.Add(factor.Key, factor.Value);
			}
			attributes.ResetAttributes();
			return attributes;
		}

		public Mapping ToYAML()
		{
			return new Mapping("baseAttributes", ((IEnumerable<KeyValuePair<AttributeType, float>>)baseAttributes).Select((Func<KeyValuePair<AttributeType, float>, Node>)((KeyValuePair<AttributeType, float> attrValue) => new Scalar(attrValue.Key.ToString(), attrValue.Value.ToString()))).ToList());
		}

		public JSONClass ToJSON()
		{
			throw new NotImplementedException();
		}

		public void AddAttribute(AttributeType attribKey, float attribValue)
		{
			if (baseAttributes.ContainsKey(attribKey))
			{
				baseAttributes[attribKey] += attribValue;
			}
			else
			{
				baseAttributes.Add(attribKey, attribValue);
			}
		}

		public void AddFactor(AttributeType attribKey, float factorValue)
		{
			if (factors.ContainsKey(attribKey))
			{
				factors[attribKey] *= factorValue;
			}
			else
			{
				factors.Add(attribKey, factorValue);
			}
		}

		public static Attributes CalculateResultAttributes(List<Attributes> attributesArr)
		{
			Attributes attributes = new Attributes();
			foreach (Attributes item in attributesArr)
			{
				foreach (KeyValuePair<AttributeType, float> attribute in item.attributes)
				{
					attributes.AddAttribute(attribute.Key, attribute.Value);
				}
				foreach (KeyValuePair<AttributeType, float> factor in item.factors)
				{
					attributes.AddFactor(factor.Key, factor.Value);
				}
			}
			return attributes;
		}

		public float GetAttribute(AttributeType attribKey)
		{
			if (attributes.ContainsKey(attribKey))
			{
				return attributes[attribKey];
			}
			Debug.LogWarning(string.Format("Attribute with name {0} is not exist", attribKey));
			return 0f;
		}

		public void ApplyFactors()
		{
			ResetAttributes();
			foreach (KeyValuePair<AttributeType, float> factor in factors)
			{
				if (attributes.ContainsKey(factor.Key))
				{
					attributes[factor.Key] += JsFunction.CalculateAttributeFactor(factor.Value);
				}
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("baseAttributes:");
			foreach (KeyValuePair<AttributeType, float> attribute in attributes)
			{
				stringBuilder.Append(attribute.Key);
				stringBuilder.Append(" = ");
				stringBuilder.AppendLine(attribute.Value.ToString());
			}
			return stringBuilder.ToString();
		}

		public void CopyTo(Attributes to)
		{
			to.baseAttributes.Clear();
			foreach (KeyValuePair<AttributeType, float> baseAttribute in baseAttributes)
			{
				to.baseAttributes.Add(baseAttribute.Key, baseAttribute.Value);
			}
			to.factors.Clear();
			foreach (KeyValuePair<AttributeType, float> factor in factors)
			{
				to.factors.Add(factor.Key, factor.Value);
			}
			to.ResetAttributes();
		}

		public void Clear()
		{
			baseAttributes.Clear();
			attributes.Clear();
			factors.Clear();
		}
	}
}
