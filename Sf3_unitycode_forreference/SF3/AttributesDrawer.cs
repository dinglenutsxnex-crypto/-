using System.Collections.Generic;
using SF3_Attributes;
using UnityEngine;

namespace SF3
{
	public class AttributesDrawer : MonoBehaviour
	{
		[SerializeField]
		private GameObject attributePrf;

		[SerializeField]
		private Color _iconColor;

		[SerializeField]
		private Color _valueColor;

		[SerializeField]
		private Color _arrowColor;

		private UITable _table;

		private AttributeViewer _ad;

		private bool _init;

		public UITable table
		{
			get
			{
				if (_table == null)
				{
					_table = GetComponent<UITable>();
				}
				return _table;
			}
		}

		private void Init()
		{
			_table = GetComponent<UITable>();
			_init = true;
		}

		public void Draw(SortedDictionary<AttributeType, float> attrs, SortedDictionary<AttributeType, float> playerAttrs = null)
		{
			if (!IsValid())
			{
				return;
			}
			if (!_init)
			{
				Init();
			}
			base.gameObject.SetActive(true);
			Clear();
			foreach (KeyValuePair<AttributeType, float> attr in attrs)
			{
				AttributeViewer attributeViewer = CreateNewAttribute();
				attributeViewer.SetIconColor(_iconColor);
				attributeViewer.SetValueColor(_valueColor);
				attributeViewer.SetArrowColor(_arrowColor);
				float value = attr.Value;
				if (playerAttrs != null)
				{
					attributeViewer.SetAttribute(attr.Key, value, playerAttrs[attr.Key]);
				}
				else
				{
					attributeViewer.SetAttribute(attr.Key, value);
				}
			}
			_table.repositionNow = true;
		}

		private bool IsValid()
		{
			if (attributePrf != null)
			{
				return true;
			}
			Debug.LogError("Attribute prefab not set");
			return false;
		}

		private void Clear()
		{
			foreach (Transform item in base.transform)
			{
				Object.Destroy(item.gameObject);
			}
		}

		private AttributeViewer CreateNewAttribute()
		{
			GameObject gameObject = NGUITools.AddChild(_table.gameObject, attributePrf);
			return gameObject.GetComponent<AttributeViewer>();
		}

		private void Start()
		{
			Init();
			_table.repositionNow = true;
		}
	}
}
