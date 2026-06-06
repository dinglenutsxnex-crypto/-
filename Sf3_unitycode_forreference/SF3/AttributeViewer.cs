using System.Collections.Generic;
using Nekki.UI;
using SF3.Settings;
using SF3_Attributes;
using UnityEngine;

namespace SF3
{
	public class AttributeViewer : MonoBehaviour
	{
		private static readonly Dictionary<AttributeType, string> AtributeNameToSpriteName = new Dictionary<AttributeType, string>
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
				"range"
			},
			{
				AttributeType.MagicPower,
				"magic"
			},
			{
				AttributeType.Cooldown,
				"cooldown"
			}
		};

		private static readonly List<AttributeType> AttributesInPercent = new List<AttributeType> { AttributeType.CriticalChance };

		[SerializeField]
		private UISprite iconSprite;

		[SerializeField]
		private UISprite arrowIcon;

		[SerializeField]
		private NekkiUILabel valueLabel;

		public void SetAttribute(AttributeType iconName, float itemAttrVal)
		{
			SetAttribute(iconName, itemAttrVal, itemAttrVal);
		}

		public void SetAttribute(AttributeType iconName, float itemAttrVal, float playerAttrVal)
		{
			iconSprite.spriteName = AtributeNameToSpriteName[iconName];
			if (AttributesInPercent.Contains(iconName))
			{
				valueLabel.text = itemAttrVal * 100f + "%";
			}
			else
			{
				valueLabel.text = itemAttrVal.ToString();
			}
			arrowIcon.gameObject.SetActive(itemAttrVal != playerAttrVal);
			SetViewColor(itemAttrVal, playerAttrVal);
			SetArrowScale(itemAttrVal, playerAttrVal);
		}

		private void SetViewColor(float itemAttrVal, float playerAttrVal)
		{
			Color color = arrowIcon.color;
			if (playerAttrVal > itemAttrVal)
			{
				color = GameSettings.ItemSettings.lowerAttributeColor;
			}
			else if (playerAttrVal < itemAttrVal)
			{
				color = GameSettings.ItemSettings.higherAttributeColor;
			}
			SetArrowColor(color);
			SetValueColor(color);
		}

		private void SetArrowScale(float itemAttrVal, float playerAttrVal)
		{
			Vector3 localScale = arrowIcon.transform.localScale;
			localScale.y = Mathf.Abs(localScale.y);
			if (playerAttrVal > itemAttrVal)
			{
				localScale.y *= -1f;
			}
			arrowIcon.transform.localScale = localScale;
		}

		public void SetIconColor(Color color)
		{
			iconSprite.color = color;
		}

		public void SetValueColor(Color color)
		{
			valueLabel.color = color;
		}

		public void SetArrowColor(Color color)
		{
			arrowIcon.color = color;
		}
	}
}
