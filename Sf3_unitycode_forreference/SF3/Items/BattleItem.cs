using System.Collections.Generic;
using System.Linq;
using Jint.Native;
using Jint.Native.Array;
using Jint.Native.Object;
using Nekki.Yaml;
using SimpleJSON;

namespace SF3.Items
{
	public abstract class BattleItem : BaseItem
	{
		protected List<string> _tags;

		protected BattleItem()
		{
			_tags = new List<string>();
		}

		public BattleItem(KeyValuePair<string, JsValue> keyValuePair)
			: base(keyValuePair)
		{
			_tags = new List<string>();
			ObjectInstance objectInstance = keyValuePair.Value.AsObject();
			if (!objectInstance.HasOwnProperty("Tags"))
			{
				return;
			}
			ArrayInstance arrayInstance = objectInstance["Tags"].AsArray();
			for (int i = 0; i < arrayInstance.Length; i++)
			{
				string item = arrayInstance[i].AsString();
				if (!_tags.Contains(item))
				{
					_tags.Add(item);
				}
			}
		}

		public override List<Node> ToYaml()
		{
			List<Node> list = base.ToYaml();
			if (_tags.Count > 0)
			{
				List<Node> list2 = new List<Node>();
				foreach (string tag in _tags)
				{
					list2.Add(new Scalar(string.Empty, tag));
				}
				list.Add(new Sequence("Tags", list2));
			}
			return list;
		}

		public override JSONClass ToJSON()
		{
			JSONClass jSONClass = base.ToJSON();
			if (_tags.Count > 0)
			{
				JSONArray jSONArray = new JSONArray();
				foreach (string tag in _tags)
				{
					JSONData aItem = new JSONData(tag);
					jSONArray.Add(aItem);
				}
				jSONClass.Add("Tags", jSONArray);
			}
			return jSONClass;
		}

		public List<string> GetTags()
		{
			_tags = _tags.Distinct().ToList();
			return new List<string>(_tags);
		}

		public void SetTags(List<string> tags)
		{
			_tags = tags.Distinct().ToList();
		}

		public void AddTags(List<string> tags)
		{
			foreach (string tag in tags)
			{
				if (!_tags.Contains(tag))
				{
					_tags.Add(tag);
				}
			}
		}

		public void AddTag(string tag)
		{
			if (!_tags.Contains(tag))
			{
				_tags.Add(tag);
			}
		}

		public void RemoveTag(string tag)
		{
			_tags.Remove(tag);
		}

		public void ClearTags()
		{
			_tags.Clear();
		}

		public override string ToString()
		{
			StringWrapper stringWrapper = StringWrapper.Create();
			stringWrapper.Add(base.ToString());
			stringWrapper.Head("BattleItem data");
			foreach (string tag in _tags)
			{
				stringWrapper.Wrap("Tag", tag);
			}
			stringWrapper.Separator();
			return stringWrapper.ToString();
		}

		public override object Clone()
		{
			BattleItem battleItem = base.Clone() as BattleItem;
			battleItem._tags = new List<string>(_tags);
			return battleItem;
		}
	}
}
