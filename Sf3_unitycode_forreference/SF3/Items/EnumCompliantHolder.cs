using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SF3.Items
{
	public abstract class EnumCompliantHolder<T> where T : struct
	{
		private readonly Dictionary<string, T> _typeCompliance;

		protected EnumCompliantHolder()
		{
			_typeCompliance = new Dictionary<string, T>();
			foreach (T value in Enum.GetValues(typeof(T)))
			{
				_typeCompliance.Add(GetCompliantStringNameByType(value), value);
			}
		}

		protected T GetValueByKey(string key)
		{
			if (_typeCompliance.ContainsKey(key))
			{
				return _typeCompliance[key];
			}
			Debug.LogError("No value was found by key: [" + key + "]. Default value returned.");
			return GetValueDefault();
		}

		protected string GetKeyByType(T type)
		{
			string keyToSearch = GetCompliantStringNameByType(type);
			List<string> list = _typeCompliance.Keys.Where((string key) => key.Equals(keyToSearch)).ToList();
			int count = list.Count;
			if (count == 1)
			{
				return list[0];
			}
			Debug.LogError("No key was found by key: [" + keyToSearch + "]. Empty string returned.");
			return string.Empty;
		}

		protected virtual string GetCompliantStringNameByType(T type)
		{
			return type.ToString();
		}

		protected abstract T GetValueDefault();
	}
}
