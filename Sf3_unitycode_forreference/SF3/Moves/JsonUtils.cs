using System.Globalization;
using SimpleJSON;
using UnityEngine;

namespace SF3.Moves
{
	internal static class JsonUtils
	{
		internal static bool TryGetString(out string result, JSONClass baseClass, string key, string defaultValue = null)
		{
			result = GetText(baseClass, key);
			if (result != null)
			{
				return true;
			}
			result = defaultValue;
			return false;
		}

		internal static bool TryGetBool(out bool result, JSONClass baseClass, string key, bool defaultValue)
		{
			string result2;
			if (TryGetString(out result2, baseClass, key))
			{
				result = result2.Equals("1") || result2.ToUpper().Equals("TRUE");
				return true;
			}
			result = defaultValue;
			return false;
		}

		internal static bool TryGetFloat(out float result, JSONClass baseClass, string key, float defaultValue)
		{
			string result2;
			if (TryGetString(out result2, baseClass, key) && float.TryParse(result2, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
			{
				return true;
			}
			result = defaultValue;
			return false;
		}

		internal static bool TryGetInt(out int result, JSONClass baseClass, string key, int defaultValue)
		{
			string result2;
			if (TryGetString(out result2, baseClass, key) && int.TryParse(result2, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
			{
				return true;
			}
			result = defaultValue;
			return false;
		}

		internal static bool TryParseVector2(out Vector2 result, JSONClass baseClass, string key, Vector2 defaultValue)
		{
			JSONNode node = GetNode(baseClass, key);
			if (node != null)
			{
				result = new Vector2(GetFloat(node, "X"), GetFloat(node, "Y"));
				return true;
			}
			result = defaultValue;
			return false;
		}

		public static float GetFloat(JSONNode mapp, string name, float defValue = 0f)
		{
			string text = GetText(mapp, name);
			return (text == null) ? defValue : float.Parse(text, CultureInfo.InvariantCulture);
		}

		public static string GetText(JSONNode jsonClass, string name, string defValue = null)
		{
			JSONNode node = GetNode(jsonClass, name);
			return (!(node != null)) ? defValue : ((JSONData)node).Value;
		}

		public static JSONNode GetNode(JSONNode node, string name)
		{
			if (node is JSONClass)
			{
				return ((JSONClass)node)[name];
			}
			if (node is JSONData)
			{
				return node;
			}
			if (node is JSONArray)
			{
				foreach (JSONNode child in ((JSONArray)node).Children)
				{
					JSONNode node2 = GetNode(child, name);
					if (node2 != null)
					{
						return node2;
					}
				}
			}
			return null;
		}
	}
}
