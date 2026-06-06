using SF3.UserData;
using UnityEngine;
using sf3DTO;

namespace SF3.Utils
{
	public static class LocalizationInitializer
	{
		private const string FEMALE_POSTFIX = "_f";

		public static void Init(SystemLanguage language, SystemLanguage defaultLanguage)
		{
			Localization.Init(language, defaultLanguage, OverridenGetLocale);
		}

		private static LocaleImport.LocaleString OverridenGetLocale(string key)
		{
			LocaleImport.LocaleString localeString = null;
			if (UserManager.UserModelInfo.gender == Gender.Female)
			{
				localeString = Localization.GetLocale(key + "_f");
			}
			if (localeString == null)
			{
				localeString = Localization.GetLocale(key);
			}
			return localeString;
		}
	}
}
