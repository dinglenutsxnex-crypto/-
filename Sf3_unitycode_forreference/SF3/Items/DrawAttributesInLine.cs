using System;
using System.Collections.Generic;
using Nekki.UI;
using SF3_Attributes;
using UnityEngine;

namespace SF3.Items
{
	[Serializable]
	public class DrawAttributesInLine
	{
		public class AttributeLabelDataForDrawing
		{
			public string baseAttributeValue;

			public bool attributeDifferent;

			public Color attributeColor;

			public string difference;

			public string differenceImageName;

			public AttributeLabelDataForDrawing(string baseValue)
			{
				attributeDifferent = false;
				baseAttributeValue = baseValue;
			}

			public void SetAttributeDifference(Color diffColor, string diffString, string diffImage)
			{
				attributeDifferent = true;
				attributeColor = diffColor;
				difference = diffString;
				differenceImageName = diffImage;
			}
		}

		private class AttributeLabelUnit
		{
			private List<UISprite> images;

			private List<UILabel> text;

			private Dictionary<Transform, int> objectsOrderWithWidth;

			private float space;

			private Transform parent;

			public float width { get; private set; }

			public AttributeLabelUnit(float spaceBetweenElements, Transform _parent)
			{
				space = spaceBetweenElements;
				images = new List<UISprite>();
				text = new List<UILabel>();
				objectsOrderWithWidth = new Dictionary<Transform, int>();
				width = 0f;
				parent = _parent;
			}

			public void AddSpacesToWidth()
			{
				if (space != 0f)
				{
					width += (float)(text.Count + images.Count - 1) * space;
				}
			}

			public void RescaleUnit(float scale)
			{
				width = 0f;
				foreach (UILabel item in text)
				{
					item.fontSize = Mathf.RoundToInt((float)item.fontSize * scale);
					objectsOrderWithWidth[item.transform] = item.width;
					width += item.width;
				}
				foreach (UISprite image in images)
				{
					image.width = Mathf.RoundToInt((float)image.width * scale);
					image.height = Mathf.RoundToInt((float)image.height * scale);
					objectsOrderWithWidth[image.transform] = image.width;
					width += image.width;
				}
				AddSpacesToWidth();
			}

			public void RecalculateWidth()
			{
				width = 0f;
				foreach (UILabel item in text)
				{
					width += item.width;
				}
				foreach (UISprite image in images)
				{
					width += image.width;
				}
				AddSpacesToWidth();
			}

			public void AddLabel(UILabel lbl, int lblWidth)
			{
				lbl.transform.parent = parent;
				text.Add(lbl);
				width += lblWidth;
				objectsOrderWithWidth.Add(lbl.transform, lblWidth);
			}

			public void AddLabel(UILabel lbl)
			{
				AddLabel(lbl, lbl.width);
			}

			public void AddImage(UISprite image, float scale = 1f)
			{
				AddImage(image, image.width, scale);
			}

			public void AddImage(UISprite image, int imgWidth, float scale = 1f)
			{
				image.transform.parent = parent;
				images.Add(image);
				width += Mathf.RoundToInt((float)imgWidth * scale);
				objectsOrderWithWidth.Add(image.transform, Mathf.RoundToInt((float)imgWidth * scale));
			}

			public Vector3 SetupFromPosition(Vector3 pos)
			{
				Vector3 vector = pos;
				foreach (KeyValuePair<Transform, int> item in objectsOrderWithWidth)
				{
					vector.x += (float)item.Value / 2f;
					item.Key.localPosition = vector;
					vector.x += (float)item.Value / 2f + space;
				}
				vector.x -= space;
				return vector;
			}
		}

		[Serializable]
		public class AttributeLblProperties
		{
			public Vector2Int iconSize;

			public Vector2Int spacing;

			public int fontSize;

			public Transform parent;
		}

		[Serializable]
		public class LblProperties
		{
			public Vector2Int lblSize;

			public UILabel.Overflow overflow;

			public UIWidget.Pivot pivot;

			public int fonSize;

			public Font font;
		}

		[Serializable]
		public class Vector2Int
		{
			public int x;

			public int y;
		}

		public Vector2Int arrowSize;

		public UIWidget.Pivot pivot;

		public UIWidget.Pivot attributeLinePivot = UIWidget.Pivot.Center;

		public AttributeLblProperties firstLblProperties;

		public bool skipFistLblProperties;

		public AttributeLblProperties normalLblProperties;

		public LblProperties attributeNameLblProp;

		private NekkiUILabel attributeNameLbl;

		private NekkiUILabel attributeNameLblSecondLine;

		public float maxLineWidth;

		public Color attributeColor;

		public float spaceBetweenUnits;

		public float spaceBetweenUnitElements;

		public int startCount;

		public Color statHigher;

		public Color statLower;

		public Color normalColor;

		private List<UILabel> labels;

		private List<UISprite> sprites;

		public UIAtlas imagesAtlas;

		public UIFont labelsFont;

		public Font trueTypeFont;

		private const string STATS_HIGHER_IMAGE = "arrowGreenStats";

		private const string STATS_LOWER_IMAGE = "arrowRedStats";

		private static Dictionary<AttributeType, string> atributeNameToImageParity = new Dictionary<AttributeType, string>
		{
			{
				AttributeType.WeaponDamage,
				"weapon"
			},
			{
				AttributeType.BodyDefense,
				"body"
			},
			{
				AttributeType.HeadDefense,
				"head"
			},
			{
				AttributeType.UnarmedDamage,
				"unarmed"
			},
			{
				AttributeType.CriticalChance,
				"critical"
			},
			{
				AttributeType.RangedDamage,
				"ranged"
			}
		};

		private static Dictionary<AttributeType, float> atributeNameToImageScale = new Dictionary<AttributeType, float>
		{
			{
				AttributeType.WeaponDamage,
				1f
			},
			{
				AttributeType.BodyDefense,
				1f
			},
			{
				AttributeType.HeadDefense,
				1f
			},
			{
				AttributeType.UnarmedDamage,
				0.65f
			},
			{
				AttributeType.CriticalChance,
				1f
			},
			{
				AttributeType.RangedDamage,
				1f
			}
		};

		private static Dictionary<AttributeType, string> attributeNameToStringFormat = new Dictionary<AttributeType, string> { 
		{
			AttributeType.CriticalChance,
			"{0:#0.##%}"
		} };

		public void Init()
		{
			labels = new List<UILabel>();
			sprites = new List<UISprite>();
			for (int i = 0; i < startCount; i++)
			{
				CreateLabel();
				CreateSprite();
			}
			attributeNameLbl = CreateLabel<NekkiUILabel>();
			attributeNameLbl.Type = NekkiUILabel.Types.Localized;
			attributeNameLblSecondLine = CreateLabel<NekkiUILabel>();
			attributeNameLblSecondLine.Type = NekkiUILabel.Types.Localized;
			attributeNameLblSecondLine.fontSize = 14;
			labels.Remove(attributeNameLbl);
			labels.Remove(attributeNameLblSecondLine);
			DisableAll();
		}

		private UILabel CreateLabel()
		{
			return CreateLabel<UILabel>();
		}

		private T CreateLabel<T>() where T : UILabel
		{
			GameObject gameObject = new GameObject("AttributeText");
			gameObject.layer = LayerMask.NameToLayer("NGUI");
			gameObject.transform.parent = normalLblProperties.parent;
			gameObject.transform.localScale = Vector3.one;
			T val = gameObject.AddComponent<T>();
			if (labelsFont != null)
			{
				val.bitmapFont = labelsFont;
			}
			else if (trueTypeFont != null)
			{
				val.trueTypeFont = trueTypeFont;
			}
			val.depth = 1;
			val.spacingX = normalLblProperties.spacing.x;
			val.spacingY = normalLblProperties.spacing.y;
			val.pivot = pivot;
			val.overflowMethod = UILabel.Overflow.ResizeFreely;
			labels.Add(val);
			return val;
		}

		private UISprite CreateSprite()
		{
			GameObject gameObject = new GameObject("AttributeImage");
			gameObject.layer = LayerMask.NameToLayer("NGUI");
			gameObject.transform.parent = normalLblProperties.parent;
			gameObject.transform.localScale = Vector3.one;
			UISprite uISprite = gameObject.AddComponent<UISprite>();
			uISprite.atlas = imagesAtlas;
			uISprite.depth = 1;
			uISprite.pivot = pivot;
			sprites.Add(uISprite);
			return uISprite;
		}

		private void ApplyLblProperties(UILabel lbl, LblProperties properties)
		{
			lbl.color = normalColor;
			lbl.overflowMethod = properties.overflow;
			lbl.pivot = properties.pivot;
			lbl.fontSize = properties.fonSize;
			lbl.width = properties.lblSize.x;
			lbl.height = properties.lblSize.y;
			if (properties.font != null)
			{
				lbl.trueTypeFont = properties.font;
			}
		}

		public void DisableAll()
		{
			foreach (UILabel label in labels)
			{
				label.gameObject.SetActive(false);
			}
			foreach (UISprite sprite in sprites)
			{
				sprite.gameObject.SetActive(false);
			}
			attributeNameLbl.gameObject.SetActive(false);
			attributeNameLblSecondLine.gameObject.SetActive(false);
		}

		private UILabel GetFreeLabel()
		{
			for (int i = 0; i < labels.Count; i++)
			{
				if (!labels[i].gameObject.activeSelf)
				{
					labels[i].gameObject.SetActive(true);
					return labels[i];
				}
			}
			return CreateLabel();
		}

		private UISprite GetFreeSprite()
		{
			for (int i = 0; i < sprites.Count; i++)
			{
				if (!sprites[i].gameObject.activeSelf)
				{
					sprites[i].gameObject.SetActive(true);
					return sprites[i];
				}
			}
			return CreateSprite();
		}

		private void SetLabelsToPositions(List<AttributeLabelUnit> attributesLabels, bool vertical = false)
		{
			if (attributesLabels.Count <= 0)
			{
				return;
			}
			float num = 0f;
			float num2 = 0f;
			if (vertical)
			{
				num2 = 0f;
				for (int i = 0; i < attributesLabels.Count; i++)
				{
					if (num2 < attributesLabels[i].width || i == 0)
					{
						num2 = attributesLabels[i].width;
					}
				}
				num2 /= -2f;
			}
			else
			{
				num2 = (float)(attributesLabels.Count - 1) * spaceBetweenUnits;
				foreach (AttributeLabelUnit attributesLabel in attributesLabels)
				{
					num2 += attributesLabel.width;
				}
				if (num2 > maxLineWidth)
				{
					float scale = maxLineWidth / num2;
					num2 = (float)(attributesLabels.Count - 1) * spaceBetweenUnits;
					foreach (AttributeLabelUnit attributesLabel2 in attributesLabels)
					{
						attributesLabel2.RescaleUnit(scale);
						num2 += attributesLabel2.width;
					}
				}
			}
			Vector3 pos = Vector3.zero;
			if (attributeLinePivot == UIWidget.Pivot.Center)
			{
				pos = new Vector3((0f - num2) / 2f, 0f, 0f);
			}
			for (int j = 0; j < attributesLabels.Count; j++)
			{
				if (vertical)
				{
					attributesLabels[j].SetupFromPosition(new Vector3(num2, num, 0f));
					num -= spaceBetweenUnits;
				}
				else
				{
					pos = attributesLabels[j].SetupFromPosition(pos);
					pos.x += spaceBetweenUnits;
				}
			}
		}

		public void ShowAttributes(BaseItem item, bool vertical = false)
		{
			DisableAll();
			if (item == null)
			{
				return;
			}
			List<AttributeLabelUnit> list = new List<AttributeLabelUnit>();
			Dictionary<AttributeType, float> allAtributes = new Dictionary<AttributeType, float>();
			CalculateAllAtributes(item, ref allAtributes);
			bool flag = true;
			foreach (AttributeType key in atributeNameToImageParity.Keys)
			{
				if (allAtributes.ContainsKey(key))
				{
					if (flag && !skipFistLblProperties)
					{
						flag = false;
						AttributeLabelUnit attributeLabelUnit = CreateAttributeUnit(key, allAtributes, firstLblProperties, false);
						ApplyLblProperties(attributeNameLbl, attributeNameLblProp);
						attributeNameLbl.Alias = key.ToString();
						attributeNameLbl.gameObject.SetActive(true);
						attributeLabelUnit.AddLabel(attributeNameLbl, 0);
						attributeLabelUnit.AddSpacesToWidth();
						SetLabelsToPositions(new List<AttributeLabelUnit> { attributeLabelUnit }, vertical);
					}
					else
					{
						list.Add(CreateAttributeUnit(key, allAtributes, normalLblProperties, false));
					}
				}
			}
			SetLabelsToPositions(list, vertical);
		}

		public void ShowComparedAttributes(BaseItem equiped, BaseItem selected_, bool vertical = false, bool nameOnSecondLine = false)
		{
			DisableAll();
			Dictionary<AttributeType, float> allAtributes = new Dictionary<AttributeType, float>();
			CalculateAllAtributes(selected_, ref allAtributes);
			Dictionary<AttributeType, float> allAtributes2 = new Dictionary<AttributeType, float>();
			CalculateAllAtributes(equiped, ref allAtributes2, false);
			List<AttributeLabelUnit> list = new List<AttributeLabelUnit>();
			Dictionary<AttributeType, AttributeLabelDataForDrawing> dictionary = new Dictionary<AttributeType, AttributeLabelDataForDrawing>();
			Equipment equipment = ((!(selected_ is Equipment)) ? null : ((Equipment)selected_));
			if (equipment.GetAttributesForCombat() == null)
			{
				return;
			}
			float num = 0f;
			foreach (KeyValuePair<AttributeType, float> item in allAtributes)
			{
				num = 0f;
				if (allAtributes2.ContainsKey(item.Key))
				{
					num = allAtributes2[item.Key];
					allAtributes2.Remove(item.Key);
				}
				string baseValue = string.Empty + item.Value;
				if (attributeNameToStringFormat.ContainsKey(item.Key))
				{
					baseValue = string.Format(attributeNameToStringFormat[item.Key], item.Value);
				}
				AttributeLabelDataForDrawing attributeLabelDataForDrawing = new AttributeLabelDataForDrawing(baseValue);
				double attributesDifference = GetAttributesDifference(item.Value, num);
				if (attributesDifference != 0.0)
				{
					baseValue = string.Empty + attributesDifference;
					if (attributeNameToStringFormat.ContainsKey(item.Key))
					{
						baseValue = string.Format(attributeNameToStringFormat[item.Key], attributesDifference);
					}
					attributeLabelDataForDrawing.SetAttributeDifference(GetAttributeColor(item.Value, num), baseValue, GetAttributeDifferenceImage(item.Value, num));
				}
				dictionary.Add(item.Key, attributeLabelDataForDrawing);
			}
			foreach (KeyValuePair<AttributeType, float> item2 in allAtributes2)
			{
				AttributeLabelDataForDrawing attributeLabelDataForDrawing2 = new AttributeLabelDataForDrawing("0");
				string diffString = string.Empty + item2.Value;
				if (attributeNameToStringFormat.ContainsKey(item2.Key))
				{
					diffString = string.Format(attributeNameToStringFormat[item2.Key], item2.Value);
				}
				attributeLabelDataForDrawing2.SetAttributeDifference(statLower, diffString, "arrowRedStats");
				dictionary.Add(item2.Key, attributeLabelDataForDrawing2);
			}
			bool flag = true;
			foreach (AttributeType key in atributeNameToImageParity.Keys)
			{
				if (dictionary.ContainsKey(key))
				{
					if (flag && !skipFistLblProperties)
					{
						flag = false;
						AttributeLabelUnit attributeLabelUnit = CreateAttributeUnit(key, dictionary, firstLblProperties, false);
						ApplyLblProperties(attributeNameLbl, attributeNameLblProp);
						attributeNameLbl.Alias = key.ToString();
						attributeNameLbl.gameObject.SetActive(true);
						attributeLabelUnit.AddLabel(attributeNameLbl, 0);
						attributeLabelUnit.AddSpacesToWidth();
						list.Add(attributeLabelUnit);
					}
					else if (nameOnSecondLine)
					{
						nameOnSecondLine = false;
						AttributeLabelUnit attributeLabelUnit2 = CreateAttributeUnit(key, dictionary, firstLblProperties, false);
						ApplyLblProperties(attributeNameLblSecondLine, attributeNameLblProp);
						attributeNameLblSecondLine.Alias = key.ToString();
						attributeNameLblSecondLine.gameObject.SetActive(true);
						attributeLabelUnit2.AddLabel(attributeNameLblSecondLine, 0);
						attributeLabelUnit2.AddSpacesToWidth();
						list.Add(attributeLabelUnit2);
					}
					else
					{
						list.Add(CreateAttributeUnit(key, dictionary, normalLblProperties));
					}
				}
			}
			SetLabelsToPositions(list, vertical);
		}

		private AttributeLabelUnit CreateAttributeUnit(AttributeType attributeName, Dictionary<AttributeType, AttributeLabelDataForDrawing> dataForDrawing, AttributeLblProperties lblProperties, bool addSpaces = true)
		{
			AttributeLabelUnit attributeLabelUnit = new AttributeLabelUnit(spaceBetweenUnitElements, lblProperties.parent);
			UISprite uISprite = CreateSprite(atributeNameToImageParity[attributeName], lblProperties.iconSize, attributeColor);
			float num = ((float)uISprite.width * atributeNameToImageScale[attributeName] - (float)uISprite.width) * -1f;
			attributeLabelUnit.AddImage(uISprite, atributeNameToImageScale[attributeName]);
			UILabel uILabel = CreateLabel(dataForDrawing[attributeName].baseAttributeValue, (!dataForDrawing[attributeName].attributeDifferent) ? normalColor : dataForDrawing[attributeName].attributeColor, lblProperties);
			attributeLabelUnit.AddLabel(uILabel, Mathf.RoundToInt((float)uILabel.width + num));
			if (addSpaces)
			{
				attributeLabelUnit.AddSpacesToWidth();
			}
			return attributeLabelUnit;
		}

		private AttributeLabelUnit CreateAttributeUnit(AttributeType attributeName, Dictionary<AttributeType, float> attributesValues, AttributeLblProperties lblProperties, bool addSpaces = true)
		{
			AttributeLabelUnit attributeLabelUnit = new AttributeLabelUnit(spaceBetweenUnitElements, lblProperties.parent);
			UISprite uISprite = CreateSprite(atributeNameToImageParity[attributeName], lblProperties.iconSize, attributeColor);
			float num = ((float)uISprite.width * atributeNameToImageScale[attributeName] - (float)uISprite.width) * -1f;
			attributeLabelUnit.AddImage(uISprite, atributeNameToImageScale[attributeName]);
			string text = attributesValues[attributeName].ToString();
			if (attributeNameToStringFormat.ContainsKey(attributeName))
			{
				text = string.Format(attributeNameToStringFormat[attributeName], attributesValues[attributeName]);
			}
			UILabel uILabel = CreateLabel(text, normalColor, lblProperties);
			attributeLabelUnit.AddLabel(uILabel, Mathf.RoundToInt((float)uILabel.width + num));
			if (addSpaces)
			{
				attributeLabelUnit.AddSpacesToWidth();
			}
			return attributeLabelUnit;
		}

		private UILabel CreateLabel(string text, Color color, AttributeLblProperties lblProperties)
		{
			UILabel freeLabel = GetFreeLabel();
			freeLabel.fontSize = lblProperties.fontSize;
			freeLabel.text = text;
			freeLabel.color = color;
			freeLabel.spacingX = lblProperties.spacing.x;
			freeLabel.spacingY = lblProperties.spacing.y;
			return freeLabel;
		}

		private UISprite CreateSprite(string attImage, Vector2Int size)
		{
			return CreateSprite(attImage, size, Color.white);
		}

		private UISprite CreateSprite(string attImage, Vector2Int size, Color color)
		{
			UISprite freeSprite = GetFreeSprite();
			freeSprite.spriteName = attImage;
			freeSprite.keepAspectRatio = UIWidget.AspectRatioSource.Free;
			freeSprite.color = color;
			freeSprite.width = size.x;
			freeSprite.height = size.y;
			return freeSprite;
		}

		public string GetAttributeDifferenceImage(float selectedValue, float equippedValue)
		{
			float num = selectedValue - equippedValue;
			if (num < 0f)
			{
				return "arrowRedStats";
			}
			if (num > 0f)
			{
				return "arrowGreenStats";
			}
			return string.Empty;
		}

		public Color GetAttributeColor(float selectedValue, float equippedValue)
		{
			float num = selectedValue - equippedValue;
			if (num < 0f)
			{
				return statLower;
			}
			if (num > 0f)
			{
				return statHigher;
			}
			return normalColor;
		}

		public double GetAttributesDifference(float selectedValue, float equippedValue)
		{
			double num = Math.Round((double)selectedValue - (double)equippedValue, 3);
			if (num < 0.0)
			{
				num *= -1.0;
			}
			return num;
		}

		private void CalculateAllAtributes(BaseItem item, ref Dictionary<AttributeType, float> allAtributes, bool considerSlots = true)
		{
			Equipment equipment = item as Equipment;
			if (equipment == null)
			{
				return;
			}
			if (equipment.GetAttributesForCombat() != null)
			{
				foreach (KeyValuePair<AttributeType, float> attributesForCombatDatum in equipment.GetAttributesForCombatData())
				{
					if (!allAtributes.ContainsKey(attributesForCombatDatum.Key))
					{
						allAtributes.Add(attributesForCombatDatum.Key, GetAttributeValue(item, attributesForCombatDatum));
					}
					else
					{
						allAtributes[attributesForCombatDatum.Key] += GetAttributeValue(item, attributesForCombatDatum);
					}
				}
			}
			Debug.LogError("Tying to use obsolete [itemsIn] field using old API");
		}

		private float GetAttributeValue(BaseItem item, KeyValuePair<AttributeType, float> attribute)
		{
			return (!(item is Equipment)) ? attribute.Value : ((Equipment)item).GetAttributesForDisplayValue(attribute.Key);
		}
	}
}
