using System;
using System.Collections.Generic;
using Jint.Native;
using Jint.Native.Object;
using Nekki.Yaml;
using SimpleJSON;
using UnityEngine;

namespace SF3.Items
{
	public abstract class BaseItem : ICloneable, UserBadgesManager.IBadgeItem
	{
		private string _image;

		public int id { get; protected set; }

		public string model { get; protected set; }

		public string alias { get; protected set; }

		public string Image
		{
			get
			{
				return _image;
			}
			protected set
			{
				_image = value;
			}
		}

		protected BaseItem()
		{
		}

		public BaseItem(KeyValuePair<string, JsValue> keyValuePair)
			: this()
		{
			JsValue value = keyValuePair.Value;
			id = int.Parse(GetNodeSafe(value, "ID", "error_no_ID"));
			model = GetNodeSafe(value, "Model", string.Empty);
			alias = GetNodeSafe(value, "Alias", "error_no_alias");
			_image = GetNodeSafe(value, "Image", "error_no_image");
			if (_image.Equals("error_no_image") && model.Length > 0)
			{
				_image = model;
			}
		}

		public void SetModel(string value)
		{
			model = value;
		}

		public virtual List<Node> ToYaml()
		{
			List<Node> list = new List<Node>();
			list.Add(new Scalar("ID", id.ToString()));
			return list;
		}

		public virtual JSONClass ToJSON()
		{
			JSONClass jSONClass = new JSONClass();
			JSONData aItem = new JSONData(id.ToString());
			jSONClass.Add("ID", aItem);
			return jSONClass;
		}

		protected string GetNodeSafe(Mapping itemData, string key, string defaultValue)
		{
			Node node = itemData.GetNode(key);
			if (node != null)
			{
				if (string.IsNullOrEmpty(node.ToString()))
				{
					return defaultValue;
				}
				return node.ToString();
			}
			return defaultValue;
		}

		protected string GetNodeSafe(Dictionary<string, JsValue> dic, string key, string defaultValue)
		{
			if (dic == null)
			{
				Debug.LogError("Achtung! JSDictionary is null");
				return defaultValue;
			}
			if (dic.ContainsKey(key))
			{
				string text = dic[key].ToString();
				if (string.IsNullOrEmpty(text) || text.Equals("undefined"))
				{
					return defaultValue;
				}
				return text;
			}
			return defaultValue;
		}

		protected string GetNodeSafe(JsValue table, string key, string defaultValue)
		{
			if (null == table)
			{
				Debug.LogError("Achtung! Not found : Key[" + key + "], returning default value --->" + defaultValue);
				return defaultValue;
			}
			if (table != JsValue.Null)
			{
				ObjectInstance objectInstance = table.AsObject();
				if (objectInstance != null)
				{
					string text = objectInstance.Get(key).ToString();
					if (string.IsNullOrEmpty(text) || text.Equals("undefined"))
					{
						return defaultValue;
					}
					return text;
				}
			}
			return defaultValue;
		}

		protected string GetNodeSafe(JSONClass itemData, string key, string defaultValue)
		{
			if (itemData.ContainsKey(key))
			{
				JSONNode jSONNode = itemData[key];
				if (string.IsNullOrEmpty(jSONNode.Value))
				{
					return defaultValue;
				}
				return jSONNode.Value;
			}
			return defaultValue;
		}

		protected bool TryParseEnum<T>(out T outParam, string key, T defaultValue) where T : struct
		{
			return SF3Utils.TryParseEnum(out outParam, key, defaultValue);
		}

		public override string ToString()
		{
			StringWrapper stringWrapper = StringWrapper.Create(StringWrapperPurpose.Log);
			stringWrapper.Head("BaseItem data");
			stringWrapper.Wrap("id", id);
			stringWrapper.Wrap("model", model);
			stringWrapper.Wrap("alias", alias);
			stringWrapper.Wrap("image", Image);
			stringWrapper.Separator();
			return stringWrapper.ToString();
		}

		public virtual object Clone()
		{
			return MemberwiseClone() as BaseItem;
		}

		public int GetBadgeID()
		{
			return id;
		}
	}
}
