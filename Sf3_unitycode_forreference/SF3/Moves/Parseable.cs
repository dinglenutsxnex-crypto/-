using System.Text;
using Nekki.Yaml;
using SF3.UserData;
using SimpleJSON;
using UnityEngine;

namespace SF3.Moves
{
	public abstract class Parseable
	{
		protected Mapping BaseMapping;

		private JSONClass _baseClass;

		protected JSONClass BaseClass
		{
			get
			{
				return _baseClass ?? (_baseClass = UserManager.Instance.YamlMappingToJsonNode(BaseMapping) as JSONClass);
			}
			set
			{
				_baseClass = value;
			}
		}

		protected bool TryGetString(out string outResult, string key, string defaultValue = "", string parseFailMsg = "", object sender = null, bool showParseFailMsg = true)
		{
			bool result = JsonUtils.TryGetString(out outResult, BaseClass, key, defaultValue);
			Message(key, parseFailMsg, sender, showParseFailMsg, result);
			return result;
		}

		protected bool TryGetInt(out int outResult, string key, int defaultValue = 0, string parseFailMsg = "", object sender = null, bool showParseFailMsg = true)
		{
			bool result = JsonUtils.TryGetInt(out outResult, BaseClass, key, defaultValue);
			Message(key, parseFailMsg, sender, showParseFailMsg, result);
			return result;
		}

		protected bool TryGetFloat(out float outResult, string key, float defaultValue = 0f, string parseFailMsg = "", object sender = null, bool showParseFailMsg = true)
		{
			bool result = JsonUtils.TryGetFloat(out outResult, BaseClass, key, defaultValue);
			Message(key, parseFailMsg, sender, showParseFailMsg, result);
			return result;
		}

		protected bool TryGetBool(out bool outResult, string key, bool defaultValue = false, string parseFailMsg = "", object sender = null, bool showParseFailMsg = true)
		{
			bool result = JsonUtils.TryGetBool(out outResult, BaseClass, key, defaultValue);
			Message(key, parseFailMsg, sender, showParseFailMsg, result);
			return result;
		}

		protected bool TryGetMapping(out Mapping outResult, string key, string parseFailMsg = "", object sender = null, bool showParseFailMsg = true)
		{
			outResult = BaseMapping.GetMapping(key);
			bool result = null != outResult;
			Message(key, parseFailMsg, sender, showParseFailMsg, result);
			return result;
		}

		protected bool TryGetBattleIdentifier(out int[] outResult, string key, string parseFailMsg = "", object sender = null, bool showParseFailMsg = true)
		{
			bool result = SF3Utils.TryParseBattleIdentifier(out outResult, key);
			Message(key, parseFailMsg, sender, showParseFailMsg, result);
			return result;
		}

		protected bool TryGetVector2(out Vector2 outResult, string key, string parseFailMsg = "", object sender = null, bool showParseFailMsg = true)
		{
			bool result = JsonUtils.TryParseVector2(out outResult, BaseClass, key, Vector2.zero);
			Message(key, parseFailMsg, sender, showParseFailMsg, result);
			return result;
		}

		private void Message(string key, string parseFailMsg, object sender, bool showParseFailMsg, bool result)
		{
			if (NeedShowParseFailMsg(showParseFailMsg, result))
			{
				ErrorMessage(key, parseFailMsg, sender);
			}
		}

		private static bool NeedShowParseFailMsg(bool showParseFailMsg, bool result)
		{
			return !result && showParseFailMsg;
		}

		private void ErrorMessage(string key, string errorMsg, object sender)
		{
			errorMsg = GetErrorMsg(key, errorMsg);
			sender = sender ?? this;
			Messenger.Error(errorMsg, sender);
		}

		private string GetErrorMsg(string key, string errorMsg)
		{
			StringBuilder stringBuilder = new StringBuilder((BaseMapping != null) ? string.Empty : "BaseMapping is null! ");
			stringBuilder.Append((!string.IsNullOrEmpty(errorMsg)) ? errorMsg : string.Format("Failed to parse value by key: [{0}]", key));
			return stringBuilder.ToString();
		}
	}
}
