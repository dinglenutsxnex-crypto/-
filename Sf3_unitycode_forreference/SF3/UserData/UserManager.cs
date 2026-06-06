using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Google.Protobuf;
using Jint.Native;
using Jint.Runtime.Descriptors;
using Nekki;
using Nekki.Utils;
using Nekki.Yaml;
using Network.core.events;
using SF3.Items;
using SF3.Moves;
using SimpleJSON;
using UnityEngine;
using sf3DTO;

namespace SF3.UserData
{
	public class UserManager : MonoBehaviour, IUserDataManager
	{
		private enum DefaultUserType
		{
			Default = 0,
			Tutotial = 1,
			PostTutorial = 2
		}

		public enum DataFormat
		{
			JSON = 0,
			YAML = 1
		}

		public const string USER_CONFIG_FILE_NAME = "User/user";

		public const string USER_DEFAULT_JSON_WITH_TUTORIAL_CONFIG_FILE_NAME = "user_default.json";

		public const string USER_DEFAULT_JSON_CONFIG_FILE_NAME = "scripts/player-default.js";

		public static EventListener<CallEventArgsBase<string>, string> OnSetGlobalVariable = new EventListener<CallEventArgsBase<string>, string>();

		public static EventListener<CallEventArgsBase<string>, string> OnRemoveGlobalVariable = new EventListener<CallEventArgsBase<string>, string>();

		private static Dictionary<CurrencyType, List<Action<long>>> _onCurrencyChanged = new Dictionary<CurrencyType, List<Action<long>>>();

		private static UserManager _instance;

		private UserModelInfo _userModelInfo;

		private YamlDocumentNekki _userYamlDocument;

		private Mapping _userMapping;

		private JSONNode _jsonRoot;

		private JSONClass _jsonMainClass;

		private string _usersText = string.Empty;

		private bool _isSaveFileByTimer;

		private static IBattleInfo _currentDojoBattle;

		private const char VECTOR_SEPARATOR = '@';

		public static UserManager Instance
		{
			get
			{
				return _instance;
			}
		}

		public static UserModelInfo UserModelInfo
		{
			get
			{
				return Instance._userModelInfo;
			}
		}

		private string UsersPath
		{
			get
			{
				return UserFilePath();
			}
		}

		public static bool IsStartingTutorialCompleted { get; private set; }

		public static long ServerPlayerID { get; private set; }

		public static FightInfo CurrentDojoFight
		{
			get
			{
				return CurrentDojoBattle.GetCurrentFight();
			}
		}

		public static IBattleInfo CurrentDojoBattle
		{
			get
			{
				if (_currentDojoBattle == null)
				{
					string[] currentDojo = GetCurrentDojo();
					int battleID = int.Parse(currentDojo[0]);
					int currentFight = int.Parse(currentDojo[1]) - 1;
					_currentDojoBattle = BattlesManager.instance.GetBattle(battleID);
					_currentDojoBattle.SetCurrentFight(currentFight);
				}
				return _currentDojoBattle;
			}
		}

		public static event Action<int> OnLevelChanged;

		public static event Action<long> OnExperienceChanged;

		public static event Action OnStartingTutorialCompleted;

		private void Awake()
		{
			_instance = this;
			_userModelInfo = null;
			_jsonRoot = null;
			_jsonMainClass = null;
			OnSetGlobalVariable = new EventListener<CallEventArgsBase<string>, string>();
			OnRemoveGlobalVariable = new EventListener<CallEventArgsBase<string>, string>();
			_onCurrencyChanged = new Dictionary<CurrencyType, List<Action<long>>>();
			UserManager.OnStartingTutorialCompleted = delegate
			{
			};
			UserManager.OnLevelChanged = delegate
			{
			};
			UserManager.OnExperienceChanged = delegate
			{
			};
			UserDataController.AddUserDataManager(UserDataManagerType.User, this);
		}

		public static void Create()
		{
			if (_instance == null)
			{
				UserManager userManager = new GameObject("_userManager").AddComponent<UserManager>();
				StaticObjectsManager.AddObject(userManager.gameObject);
			}
			Instance.InitializeData();
		}

		private void InitializeData()
		{
			LoadUserData();
			foreach (CurrencyType enumerator2 in EnumsCompliancer.GetEnumerators<CurrencyType>())
			{
				_onCurrencyChanged.Add(enumerator2, new List<Action<long>>());
			}
			IsStartingTutorialCompleted = IsTutorialComplete();
			if (!IsStartingTutorialCompleted)
			{
				NetworkConnection.current.EnterDarkPocket();
			}
			GlobalTimer.Instnce.addEventListener(0, OnTimerTick);
			SaveFileUser();
		}

		private static void InitializeUser()
		{
			_instance._userModelInfo = new UserModelInfo(_instance._jsonMainClass);
			UserModelInfo.SetIsPlayer(true);
			UserModelInfo.SetIsControl(true);
			UserModelInfo.SetAIMode(AiMode.NoneMode);
			UserModelInfo.SetAlias(GetName());
		}

		private static string GetUserValue(string key, string defaultValue = null)
		{
			return (!Instance._jsonMainClass.ContainsKey(key)) ? defaultValue : Instance._jsonMainClass[key].Value;
		}

		private static void SetUserValue(string key, object value, JSONBinaryTag valueType = JSONBinaryTag.Value)
		{
			Instance._jsonMainClass[key] = value.ToString();
			ChangeNodeByType(Instance._jsonMainClass[key], value, valueType);
		}

		private static string GetValueFromInnerMap(string mapName, string key, JSONClass root = null)
		{
			root = root ?? Instance._jsonMainClass;
			if (!root.ContainsKey(mapName))
			{
				return string.Empty;
			}
			return root[mapName].AsObject[key];
		}

		private static void SetUserMapInnerValue(string mapName, string key, object value, JSONBinaryTag valueType = JSONBinaryTag.Value)
		{
			JSONNode node = Instance._jsonMainClass[mapName].AsObject[key];
			node = ChangeNodeByType(node, value, valueType);
		}

		private static JSONNode ChangeNodeByType(JSONNode node, object value, JSONBinaryTag valueType)
		{
			switch (valueType)
			{
			case JSONBinaryTag.IntValue:
				node.AsInt = (int)value;
				break;
			case JSONBinaryTag.LongValue:
				node.AsLong = (long)value;
				break;
			case JSONBinaryTag.FloatValue:
				node.AsFloat = (float)value;
				break;
			case JSONBinaryTag.DoubleValue:
				node.AsDouble = (double)value;
				break;
			case JSONBinaryTag.BoolValue:
				node.AsBool = (bool)value;
				break;
			default:
				node.Value = value.ToString();
				break;
			}
			return node;
		}

		private static void SetUserValueAndSave(string key, object value, JSONBinaryTag valueType = JSONBinaryTag.Value)
		{
			SetUserValue(key, value, valueType);
			Instance.SaveFileUser();
		}

		private string UserFilePath()
		{
			return GlobalPath.GameDataCombine("User/user.json");
		}

		private void LoadUserData()
		{
			string text = FilesUtil.ReadFileText(UsersPath);
			if (text.IsNullOrEmpty())
			{
				text = GetDefaultUser(DefaultUserType.Tutotial);
			}
			LoadUser(text);
		}

		private void LoadUser(string text)
		{
			_usersText = text;
			_jsonRoot = JSON.Parse(_usersText);
			_jsonMainClass = _jsonRoot["User"].AsObject;
			SaveFileUser(true);
		}

		public void ClearUser()
		{
			NetworkConnection.current.ClearOfflineQueue();
			NetworkConnection.current.EnterDarkPocket();
			NetworkConnection.current.RestartConnection("Clearing User");
			FilesUtil.DeleteFile(GlobalPath.GameDataPath + "/User/user.json");
			NetworkConnection.current.BlockInputUntilNetworkEstablished();
		}

		private string GetDefaultUser(DefaultUserType forcePostTutorialConfig = DefaultUserType.Default)
		{
			switch (forcePostTutorialConfig)
			{
			case DefaultUserType.Default:
			{
				if (!IsTutorialComplete())
				{
					return GlobalLoad.GetLoadTextInternal("GameSettings", "user_default.json");
				}
				string jsContents2 = NetworkConfigManager.UnzipToString("scripts/player-default.js");
				return SaveAdditionalFields(JSToJson(jsContents2));
			}
			case DefaultUserType.Tutotial:
				return GlobalLoad.GetLoadTextInternal("GameSettings", "user_default.json");
			default:
			{
				string jsContents = NetworkConfigManager.UnzipToString("scripts/player-default.js");
				return SaveAdditionalFields(JSToJson(jsContents));
			}
			}
		}

		public void Initialize()
		{
			InitializeUser();
			UpdateUserData(null);
		}

		public void UpdateUserData(IMessage playerData)
		{
			if (playerData != null)
			{
				Player player = playerData as Player;
				BattleInterface.Instance.SetNickNameFull(player.PublicPlayer.ShortPlayer.Nickname);
				SetCurrentChapter(player.ChapterId);
				SetGender(player.Appearance.Gender);
				SetCurrencies(player.Currencies.RepeatedToList());
				SetRating(player.Rating);
				ServerPlayerID = player.PublicPlayer.ShortPlayer.PlayerId;
				SetName(player.PublicPlayer.ShortPlayer.DisplayName);
				SetLevel(player.PublicPlayer.ShortPlayer.Level);
				SetExperience(player.Experience);
				Dictionary<string, JsValue> dictionary = JsFunction.CalculateNewLevel(UserModelInfo.level, UserModelInfo.experience, 0L);
				SetLevelExperience(long.Parse(dictionary["LevelExperience"].ToString()));
				SetGlobalVariable("ab_tag", player.AbTag);
				SetPerks(player);
				SetEquipment(player);
				SetBoosters(player);
				_userModelInfo.UpdateModelInfo();
				if (ModelsManager.Instance != null && ModelsManager.Instance.Player != null)
				{
					ModelsManager.Instance.Player.UpdateModelInfo();
				}
			}
		}

		private void SetBoosters(Player currentPlayer)
		{
			CleanData("Boosters");
			if (currentPlayer.Inventory.Boosters.Count > 0)
			{
				List<SF3.Items.Booster> boosters = SF3.Items.Booster.Create(currentPlayer.Inventory.Boosters.RepeatedToList());
				SaveBoosters(boosters);
				_userModelInfo.SetBoosters(boosters);
			}
		}

		private void SetEquipment(Player currentPlayerData)
		{
			CleanData("Equipments");
			if (currentPlayerData.Inventory.Items.Count > 0)
			{
				List<Equipment> list = new List<Equipment>();
				foreach (Item item2 in currentPlayerData.Inventory.Items)
				{
					Equipment item = Equipment.Create(item2);
					_userModelInfo.UpdateItemPerksData(item);
					list.Add(item);
				}
				SaveEquipments(list);
				_userModelInfo.SetEquipments(list);
			}
			IsStartingTutorialCompleted = true;
			_instance.SaveFileUser(true);
			UserManager.OnStartingTutorialCompleted();
			UserBadgesManager.Instance.TutorialComplete();
		}

		private void SetPerks(Player currentPlayerData)
		{
			CleanData("Perks");
			if (currentPlayerData.Inventory.Perks.Count > 0)
			{
				List<SF3.Items.Perk> perks = currentPlayerData.Inventory.Perks.Select(SF3.Items.Perk.Create).ToList();
				SavePerks(perks);
				_userModelInfo.SetPerks(perks);
			}
		}

		private void CleanData(string key)
		{
			if (Instance._jsonMainClass.ContainsKey(key))
			{
				JSONArray asArray = Instance._jsonMainClass[key].AsArray;
				while (asArray.Count > 0)
				{
					asArray.Remove(0);
				}
			}
		}

		public static void SetCurrentDojo(int battleID, int fightID)
		{
			SetUserValue("CurrentDojo", battleID + "." + fightID);
			_currentDojoBattle = null;
			BattlesManager.instance.InitializeDojo();
			Instance.SaveFileUser();
		}

		public static string[] GetCurrentDojo()
		{
			return GetUserValue("CurrentDojo").Split('.');
		}

		public static Currency GetCurrency(CurrencyType type)
		{
			return UserModelInfo.GetCurrency(type);
		}

		public static List<Currency> GetCurrencies()
		{
			List<Currency> list = new List<Currency>();
			foreach (CurrencyType value in Enum.GetValues(typeof(CurrencyType)))
			{
				Currency currency = GetCurrency(value);
				if (currency.Value != 0)
				{
					list.Add(currency);
				}
			}
			return list;
		}

		public static long GetCurrencyValue(CurrencyType type)
		{
			return UserModelInfo.GetCurrencyValue(type);
		}

		public static void SubtractCurrency(Currency currency)
		{
			AddCurrency(currency.CurrencyType, -currency.Value);
		}

		public static void SubtractCurrency(CurrencyType type, long value)
		{
			AddCurrency(type, -value);
		}

		public static void AddCurrency(List<Currency> currencies)
		{
			foreach (Currency currency in currencies)
			{
				AddCurrency(currency.CurrencyType, currency.Value);
			}
		}

		public static void AddCurrency(Currency currency)
		{
			AddCurrency(currency.CurrencyType, currency.Value);
		}

		public static void AddCurrency(CurrencyType type, long value)
		{
			long value2 = UserModelInfo.GetCurrencyValue(type) + value;
			SetCurrency(type, value2);
		}

		public static void SetCurrency(CurrencyType type, long value)
		{
			UserModelInfo.SetCurrency(type, value);
			SetUserMapInnerValue("Currency", type.ToString(), value, JSONBinaryTag.LongValue);
			Instance.SaveFileUser();
			foreach (Action<long> item in _onCurrencyChanged[type])
			{
				item(GetCurrencyValue(type));
			}
		}

		public static void SetCurrencies(List<Currency> currencies)
		{
			Dictionary<CurrencyType, long> dictionary = new Dictionary<CurrencyType, long>();
			foreach (CurrencyType enumerator4 in EnumsCompliancer.GetEnumerators<CurrencyType>())
			{
				dictionary.Add(enumerator4, 0L);
			}
			foreach (Currency currency in currencies)
			{
				dictionary[currency.CurrencyType] = currency.Value;
			}
			foreach (KeyValuePair<CurrencyType, long> item in dictionary)
			{
				SetCurrency(item.Key, item.Value);
			}
		}

		public static void AddActionForCurrency(CurrencyType type, Action<long> action)
		{
			_onCurrencyChanged[type].Add(action);
		}

		public static void RemoveActionForCurrency(CurrencyType type, Action<long> action)
		{
			_onCurrencyChanged[type].Remove(action);
		}

		private void SetCurrentChapter(int chapterID)
		{
			SetUserValue("Chapter", chapterID, JSONBinaryTag.IntValue);
		}

		public int GetCurrentChapter()
		{
			return int.Parse(GetUserValue("Chapter"));
		}

		public static void GiveRewards(FightResult result)
		{
			foreach (CurrencyType enumerator3 in EnumsCompliancer.GetEnumerators<CurrencyType>())
			{
				long rewardCurrency = result.GetRewardCurrency(enumerator3);
				if (rewardCurrency > 0)
				{
					AddCurrency(enumerator3, rewardCurrency);
				}
			}
			long num = 0L;
			foreach (KeyValuePair<string, long> fightRewardBonu in result.GetFightRewardBonus())
			{
				num += fightRewardBonu.Value;
			}
			if (num > 0)
			{
				AddCurrency(CurrencyType.Coin, num);
			}
			AddExperience(result.GetRewardExperience());
			GivePerks(result.GetRewardPerks());
			GiveBoosters(result.GetRewardBoosters());
			GiveEquipments(result.GetRewardEquipment());
		}

		public static void GiveRewards(SF3.Items.Booster openedBooster)
		{
			AddCurrency(openedBooster.currencies);
			AddExperience(openedBooster.experience);
			GivePerks(openedBooster.perks.Select((BaseItem x) => x as SF3.Items.Perk).ToList());
			GiveEquipments(openedBooster.equipments.Select((BaseItem x) => x as Equipment).ToList());
		}

		public static void GiveRewards(BoosterpackRewards rewards)
		{
			GivePerks(rewards.perks);
			GiveEquipments(rewards.equipments);
		}

		private static void GivePerks(List<SF3.Items.Perk> perks)
		{
			if (perks.Count > 0)
			{
				SavePerks(UserModelInfo.AddItems(perks, true));
			}
		}

		private static void GiveBoosters(List<SF3.Items.Booster> boosters)
		{
			if (boosters.Count > 0)
			{
				SaveBoosters(UserModelInfo.AddItems(boosters, true));
			}
		}

		private static void GiveEquipments(List<Equipment> equipments)
		{
			if (equipments.Count > 0)
			{
				SaveEquipments(UserModelInfo.AddItems(equipments, true));
			}
		}

		private void OnTimerTick(CallEventArgs callEventArgs)
		{
			if (_isSaveFileByTimer)
			{
				_isSaveFileByTimer = false;
				SaveFileUser(true);
			}
		}

		public void SaveFileUser(bool forcibly = false)
		{
			if (forcibly)
			{
				FilesUtil.WriteFileText(UsersPath, _jsonRoot.ToJSON(0));
			}
			else
			{
				_isSaveFileByTimer = true;
			}
		}

		public static void SetQualityLevel(string qualityLvL)
		{
			SetUserValueAndSave("GraphicsQuality", qualityLvL);
		}

		public static string GetPresetQuality()
		{
			string userValue = GetUserValue("PresetQuality");
			return (userValue != null) ? userValue.ToLower() : null;
		}

		public static void SetPresetQuality(string value)
		{
			SetUserValueAndSave("PresetQuality", value);
		}

		public static string GetTypeDevice()
		{
			string userValue = GetUserValue("TypeDevice");
			return (userValue != null) ? userValue.ToLower() : null;
		}

		public static void SetTypeDevice(string value)
		{
			SetUserValueAndSave("TypeDevice", value);
		}

		public static void SetName(string name)
		{
			SetUserValueAndSave("Name", name);
			UserModelInfo.SetAlias(name);
		}

		public static string GetName()
		{
			return GetUserValue("Name", "PLAYER");
		}

		public static void ApplyRatingDelta(double delta)
		{
			SetRating(GetRating() + delta);
			Debug.Log(string.Format("Rating delta:{0} New total:{1}", delta, GetRating()));
		}

		public static void SetRating(double rating)
		{
			SetUserValueAndSave("Rating", rating);
		}

		public static double GetRating()
		{
			return float.Parse(GetUserValue("Rating", "0"));
		}

		public static void SetGender(Gender gender)
		{
			SetUserValueAndSave("Gender", gender.ToString());
			UserModelInfo.SetGender(gender);
		}

		public static Gender GetGender()
		{
			string userValue = GetUserValue("Gender", string.Empty);
			return (userValue.Length == 0) ? Gender.Male : EnumsCompliancer.GetEnumerator<Gender>(userValue);
		}

		public static sf3DTO.Color GetSkinColor()
		{
			sf3DTO.Color color = new sf3DTO.Color();
			string valueFromInnerMap = GetValueFromInnerMap("SkinColor", "Color", GetAppearance());
			color.ColorId = ((valueFromInnerMap.Length != 0) ? int.Parse(valueFromInnerMap) : (-1));
			valueFromInnerMap = GetValueFromInnerMap("SkinColor", "Value", GetAppearance());
			color.Value = ((valueFromInnerMap.Length != 0) ? double.Parse(valueFromInnerMap, CultureInfo.InvariantCulture) : 0.0);
			return color;
		}

		public static sf3DTO.Color GetHairColor()
		{
			sf3DTO.Color color = new sf3DTO.Color();
			string valueFromInnerMap = GetValueFromInnerMap("HairColor", "Color", GetAppearance());
			color.ColorId = ((valueFromInnerMap.Length != 0) ? int.Parse(valueFromInnerMap) : (-1));
			valueFromInnerMap = GetValueFromInnerMap("HairColor", "Value", GetAppearance());
			color.Value = ((valueFromInnerMap.Length != 0) ? double.Parse(valueFromInnerMap, CultureInfo.InvariantCulture) : 0.0);
			return color;
		}

		public static void SetHead(string head)
		{
			SetUserValueAndSave("Head", head);
		}

		public static JSONClass GetAppearance()
		{
			return Instance._jsonMainClass["Appearance"].AsObject;
		}

		public static string GetHead(string defaultHead = "head__01a")
		{
			JSONClass appearance = GetAppearance();
			if (appearance != null && appearance.ContainsKey("Head"))
			{
				return appearance["Head"].Value;
			}
			return defaultHead;
		}

		public static int GetHeadId()
		{
			string head = GetHead();
			Dictionary<string, JsValue> dictionary = JS.Instance.GetScope("Heads").AsDictionary();
			foreach (KeyValuePair<string, JsValue> item in dictionary)
			{
				Dictionary<string, JsValue> dictionary2 = item.Value.AsObject().AsDictionary();
				if (dictionary2["Head"].AsString().Equals(head))
				{
					return dictionary2["ID"].AsInteger();
				}
			}
			return -1;
		}

		public static void SetHair(string hair)
		{
			SetUserValueAndSave("Hair", hair);
		}

		public static string GetHair()
		{
			return GetUserValue("Hair", "hair-01");
		}

		public static void AddExperience(long value)
		{
			if (value > 0)
			{
				Dictionary<string, JsValue> dictionary = JsFunction.CalculateNewLevel(UserModelInfo.level, UserModelInfo.experience, value);
				long levelExperience = long.Parse(dictionary["LevelExperience"].ToString());
				int num = dictionary["Level"].AsInteger();
				long experience = long.Parse(dictionary["Experience"].ToString());
				StringBuilder stringBuilder = new StringBuilder();
				if (num > UserModelInfo.level)
				{
					SetLevel(num);
					SetLevelExperience(levelExperience);
					QuestController.Instance.ThrowEvent(ETriggerEvents.QEVENT_LEVEL_UP, num);
					stringBuilder.AppendLine("LEVEL UP: " + num + "!!!");
				}
				SetExperience(experience);
				stringBuilder.AppendLine("level: " + UserModelInfo.level);
				stringBuilder.AppendLine("experience: " + UserModelInfo.experience);
				stringBuilder.AppendLine("levelExperience: " + UserModelInfo.levelExperience);
				stringBuilder.AppendLine(UserModelInfo.attributes.PrintBaseAttributes());
				Debug.Log(stringBuilder.ToString());
				Instance.SaveFileUser();
			}
		}

		public static void SetLevel(int value)
		{
			SetUserValue("Level", value, JSONBinaryTag.IntValue);
			UserModelInfo.SetLevel(value);
			UserManager.OnLevelChanged(value);
		}

		public static void SetLevelExperience(long value)
		{
			SetUserValue("LevelExperience", value, JSONBinaryTag.LongValue);
			UserModelInfo.SetLevelExperience(value);
		}

		public static void SetExperience(long value)
		{
			SetUserValue("Experience", value, JSONBinaryTag.LongValue);
			UserModelInfo.SetExperience(value);
			UserManager.OnExperienceChanged(value);
		}

		public static long GetExperience()
		{
			return UserModelInfo.experience;
		}

		public static int GetLevel()
		{
			return UserModelInfo.level;
		}

		public static void AddItem(BaseItem itemValue, bool mergeStackLvl)
		{
			BaseItem baseItem = UserModelInfo.AddItem(itemValue, mergeStackLvl);
			if (itemValue is Equipment)
			{
				AddBaseItem("Equipments", (Equipment)baseItem);
			}
			else if (itemValue is SF3.Items.Perk)
			{
				AddBaseItem("Perks", (SF3.Items.Perk)baseItem);
			}
			else if (itemValue is SF3.Items.Booster)
			{
				AddBaseItem("Boosters", (SF3.Items.Booster)baseItem);
			}
			else
			{
				Debug.LogError(string.Format("Cant add to user item of type [{0}]", itemValue.GetType()));
			}
		}

		private static void AddBaseItem(string key, BaseItem item)
		{
			AddBaseItemToJSON(key, item);
		}

		private static void AddBaseItemToYAML(string key, BaseItem item)
		{
			Mapping mapping = new Mapping(key, item.ToYaml());
			bool flag = false;
			Sequence sequence = Instance._userMapping.GetSequence(key);
			if (sequence == null)
			{
				Instance._userMapping.Add(new Sequence(key, mapping));
				flag = true;
			}
			else
			{
				for (int i = 0; i < sequence.nodesInside.Count; i++)
				{
					Scalar text = ((Mapping)sequence.nodesInside[i]).GetText("ID");
					if (text != null && text.text == item.id.ToString())
					{
						sequence.Replace(i, mapping);
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				sequence.AddNode(mapping);
			}
			Instance.SaveFileUser();
		}

		private static void AddBaseItemToJSON(string key, BaseItem item)
		{
			JSONClass jSONClass = item.ToJSON();
			JSONClass jsonMainClass = Instance._jsonMainClass;
			JSONArray jSONArray = null;
			if (!jsonMainClass.ContainsKey(key))
			{
				jSONArray = new JSONArray();
				jSONArray.Add(jSONClass);
				jsonMainClass[key] = jSONArray;
			}
			else
			{
				bool flag = false;
				jSONArray = jsonMainClass[key] as JSONArray;
				for (int i = 0; i < jSONArray.Count; i++)
				{
					JSONClass asObject = jSONArray[i].AsObject;
					int num = 0;
					num = (int)((!(item is SF3.Items.Booster)) ? item.id : (item as SF3.Items.Booster).instance_id);
					if (asObject.ContainsKey("ID") && asObject["ID"].AsInt.Equals(num))
					{
						jSONArray[i] = jSONClass;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					jSONArray.Add(jSONClass);
				}
			}
			Instance.SaveFileUser();
		}

		private static void SaveBaseItems(string key, BaseItem[] items)
		{
			foreach (BaseItem item in items)
			{
				AddBaseItem(key, item);
			}
		}

		public static void SaveEquipments(List<Equipment> items)
		{
			SaveBaseItems("Equipments", items.ToArray());
		}

		public static void SavePerks(List<SF3.Items.Perk> perks)
		{
			SaveBaseItems("Perks", perks.ToArray());
		}

		public static void SaveBoosters(List<SF3.Items.Booster> boosters)
		{
			SaveBaseItems("Boosters", boosters.ToArray());
		}

		public static void RemoveBoosterFromYAML(SF3.Items.Booster booster)
		{
			JSONClass jsonMainClass = Instance._jsonMainClass;
			if (!jsonMainClass.ContainsKey("Boosters"))
			{
				return;
			}
			JSONArray asArray = jsonMainClass["Boosters"].AsArray;
			foreach (JSONClass item in asArray)
			{
				long asLong = item["ID"].AsLong;
				if (asLong == booster.instance_id)
				{
					asArray.Remove((JSONNode)item);
					break;
				}
			}
			Instance.SaveFileUser();
		}

		public static void EquipItem(int oldItemId, Equipment newItem)
		{
			bool flag = false;
			bool flag2 = oldItemId == -1;
			JSONArray asArray = Instance._jsonMainClass["Equipments"].AsArray;
			foreach (JSONClass item in asArray)
			{
				int asInt = item["ID"].AsInt;
				if (!flag && asInt == newItem.id)
				{
					item["Equipped"].AsInt = 1;
					flag = true;
				}
				else if (!flag2 && asInt == oldItemId)
				{
					item.Remove("Equipped");
					flag2 = true;
				}
				if (flag && flag2)
				{
					break;
				}
			}
			Instance.SaveFileUser();
			QuestController.Instance.ThrowEvent(ETriggerEvents.QEVENT_EQUIP, newItem);
			EquipRequest equipRequest = new EquipRequest();
			equipRequest.Equip = true;
			equipRequest.ModelId = newItem.id;
			UserDataController.Send_Equip(equipRequest);
		}

		public static void UnEquipItem(Equipment itemValue)
		{
			JSONArray asArray = Instance._jsonMainClass["Equipments"].AsArray;
			foreach (JSONClass item in asArray)
			{
				if (item.ContainsKey("ID") && item["ID"].AsInt == itemValue.id)
				{
					item.Remove("Equipped");
					Instance.SaveFileUser();
					QuestController.Instance.ThrowEvent(ETriggerEvents.QEVENT_UNEQUIP, itemValue);
					break;
				}
			}
			EquipRequest equipRequest = new EquipRequest();
			equipRequest.Equip = false;
			equipRequest.ModelId = itemValue.id;
			UserDataController.Send_Equip(equipRequest);
		}

		public static void RemoveEquipment(int itemId)
		{
			JSONArray asArray = Instance._jsonMainClass["Equipments"].AsArray;
			foreach (JSONClass item in asArray)
			{
				if (item.ContainsKey("ID") && item["ID"].AsInt == itemId)
				{
					asArray.Remove((JSONNode)item);
					return;
				}
			}
			Instance.SaveFileUser();
		}

		public static void RemoveEquipment(Equipment itemToRemove)
		{
			RemoveEquipment(itemToRemove.id);
		}

		public static void UpdateEquipmentData(int itemId)
		{
			Equipment equipment = UserModelInfo.GetItemByID(itemId) as Equipment;
			if (equipment == null)
			{
				return;
			}
			JSONArray asArray = Instance._jsonMainClass["Equipments"].AsArray;
			for (int i = 0; i < asArray.Count; i++)
			{
				JSONClass asObject = asArray[i].AsObject;
				if (asObject.ContainsKey("ID") && asObject["ID"].AsInt == itemId)
				{
					asArray[i] = equipment.ToJSON();
					break;
				}
			}
			Instance.SaveFileUser();
		}

		public static void UpdateEquipmentData(Equipment updatedItem)
		{
			UpdateEquipmentData(updatedItem.id);
		}

		public static bool IsEquiped(EquipmentType type)
		{
			JSONArray asArray = Instance._jsonMainClass["Equipments"].AsArray;
			foreach (JSONClass item in asArray)
			{
				int asInt = item["ID"].AsInt;
				Equipment equipment;
				if (ItemsManager.TryGetEquipmentById(asInt, out equipment) && equipment.GetEquipmentType() == type)
				{
					return true;
				}
			}
			return false;
		}

		public static Inventory GetInventory()
		{
			Inventory inventory = new Inventory();
			inventory.Boosters.AddRange(UserModelInfo.GetDTOBoosters());
			inventory.Items.AddRange(UserModelInfo.GetEquipment().Select(DtoExtensions.AsDto));
			inventory.Perks.AddRange(UserModelInfo.GetPerks().Select(DtoExtensions.AsDto));
			return inventory;
		}

		public static void SetGlobalVariable(string varName, object varValue, bool isServerVariable = false, JSONBinaryTag varType = JSONBinaryTag.Value)
		{
			if (varName.Length == 0 || varValue.ToString().Length == 0)
			{
				return;
			}
			bool flag = false;
			JSONClass asObject = Instance._jsonMainClass["GlobalVariables"].AsObject;
			JSONArray jSONArray = ((!isServerVariable) ? asObject["Local"].AsArray : asObject["Server"].AsArray);
			for (int i = 0; i < jSONArray.Count; i++)
			{
				JSONClass asObject2 = jSONArray[i].AsObject;
				if (asObject2.ContainsKey(varName))
				{
					JSONClass jSONClass = new JSONClass();
					JSONData jSONData = new JSONData(string.Empty);
					ChangeNodeByType(jSONData, varValue, varType);
					jSONClass.Add(varName, jSONData);
					jSONArray[i] = jSONClass;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				JSONClass jSONClass2 = new JSONClass();
				JSONData jSONData2 = new JSONData(string.Empty);
				ChangeNodeByType(jSONData2, varValue, varType);
				jSONClass2.Add(varName, jSONData2);
				jSONArray.Add("Variable", jSONClass2);
			}
			OnSetGlobalVariable.callEvent(varName);
			Instance.SaveFileUser();
		}

		public static string GetGlobalVariable(string varName)
		{
			if (varName.IsNullOrEmpty())
			{
				return null;
			}
			JSONClass asObject = Instance._jsonMainClass["GlobalVariables"].AsObject;
			return FindVariable(asObject, varName, "Local") ?? FindVariable(asObject, varName, "Server");
		}

		private static string FindVariable(JSONClass globalClass, string varName, string key)
		{
			JSONArray asArray = globalClass[key].AsArray;
			for (int i = 0; i < asArray.Count; i++)
			{
				JSONClass asObject = asArray[i].AsObject;
				if (asObject.ContainsKey(varName))
				{
					return asObject[varName].Value;
				}
			}
			return null;
		}

		public static bool RemoveGlobalVariable(string varName)
		{
			if (varName.Length == 0)
			{
				return false;
			}
			varName = varName.ToUpper();
			bool flag = false;
			JSONClass asObject = Instance._jsonMainClass["GlobalVariables"].AsObject;
			flag = RemoveVariable(asObject, varName, "Local");
			flag = RemoveVariable(asObject, varName, "Server");
			if (flag)
			{
				Instance.SaveFileUser();
				OnRemoveGlobalVariable.callEvent(varName);
			}
			return flag;
		}

		private static bool RemoveVariable(JSONClass globalClass, string varName, string key)
		{
			JSONArray asArray = globalClass[key].AsArray;
			for (int i = 0; i < asArray.Count; i++)
			{
				JSONClass asObject = asArray[i].AsObject;
				foreach (KeyValuePair<string, JSONNode> item in asObject)
				{
					string text = item.Key.ToUpper();
					if (text.Equals(varName))
					{
						asArray.Remove((JSONNode)asObject);
						return true;
					}
				}
			}
			return false;
		}

		public static void SetQuestsLog(string name)
		{
			long num = (long)GlobalTimer.GetTime;
			JSONArray jSONArray = null;
			if (!Instance._jsonMainClass.ContainsKey("QuestsLog"))
			{
				jSONArray = new JSONArray();
				Instance._jsonMainClass.Add(jSONArray);
			}
			else
			{
				jSONArray = Instance._jsonMainClass["QuestsLog"].AsArray;
			}
			JSONClass jSONClass = null;
			foreach (JSONNode item in jSONClass)
			{
				if (item is JSONClass)
				{
					string value = item.AsObject["Name"].Value;
					if (value.Equals(name))
					{
						jSONClass = item.AsObject;
						break;
					}
				}
			}
			if (jSONClass == null)
			{
				jSONClass = new JSONClass();
				jSONClass.Add("Name", name);
				jSONClass.Add("TimeStamp", "[" + num + "]");
				jSONArray.Add("QuestsLog", jSONClass);
			}
			else
			{
				JSONNode jSONNode2 = jSONClass["TimeStamp"];
				string text = jSONNode2.Value.Remove(0, 1);
				text = "[" + num + "," + text;
				jSONNode2 = text;
				jSONArray.Remove((JSONNode)jSONClass);
				List<JSONNode> list = jSONArray.Children.ToList();
				while (jSONArray.Count > 0)
				{
					jSONArray.Remove(0);
				}
				jSONArray.Add(jSONClass);
				foreach (JSONNode item2 in list)
				{
					jSONArray.Add(item2);
				}
			}
			Instance.SaveFileUser(true);
		}

		public static void SetIntentModule(Mapping data)
		{
			Mapping mapping = (Mapping)Instance._userMapping.GetNode("Intent");
			if (mapping != null)
			{
				Instance._userMapping.Remove(mapping);
			}
			Instance._userMapping.Add(data);
			Instance.SaveFileUser(true);
		}

		public static void SetIntentModule(JSONClass data)
		{
			if (Instance._jsonMainClass.ContainsKey("Intent"))
			{
				Instance._jsonMainClass.Remove("Intent");
			}
			Instance._jsonMainClass.Add("Intent", data);
			Instance.SaveFileUser(true);
		}

		public static object GetIntentModule()
		{
			return (!Instance._jsonMainClass.ContainsKey("Intent")) ? null : Instance._jsonMainClass["Intent"];
		}

		public static void AddQuestQueue(InfoTrigger trigger)
		{
			if (!trigger.unresumable)
			{
				JSONArray jSONArray = null;
				if (Instance._jsonMainClass.ContainsKey("Quests"))
				{
					jSONArray = Instance._jsonMainClass["Quests"].AsArray;
					jSONArray.Add("Name", trigger.fullName);
				}
				else
				{
					jSONArray = new JSONArray();
					jSONArray.Add("Name", trigger.fullName);
					Instance._jsonMainClass.Add("Quests", jSONArray);
				}
				Instance.SaveFileUser(true);
			}
		}

		public static void RemoveQuestQueue(InfoTrigger trigger)
		{
			JSONArray asArray = Instance._jsonMainClass["Quests"].AsArray;
			JSONNode jSONNode = GetQuest(trigger.fullName) as JSONNode;
			if (jSONNode != null)
			{
				asArray.Remove(jSONNode);
				Instance.SaveFileUser(true);
			}
		}

		public static object GetQuest(string name)
		{
			if (Instance._jsonMainClass.ContainsKey("Quests"))
			{
				JSONArray asArray = Instance._jsonMainClass["Quests"].AsArray;
				foreach (JSONNode item in asArray)
				{
					if (name.Equals(item.Value))
					{
						return item;
					}
				}
			}
			return null;
		}

		public static List<string> GetQuestQueue()
		{
			List<string> result = new List<string>();
			if (Instance._jsonMainClass.ContainsKey("Quests"))
			{
				return Instance._jsonMainClass["Quests"].AsArray.Children.Select((JSONNode x) => x.Value).ToList();
			}
			return result;
		}

		public static Vector3 GetMapCameraPosition()
		{
			string globalVariable = GetGlobalVariable("MapCameraPosition");
			if (globalVariable.IsNullOrEmpty())
			{
				return Vector3.zero;
			}
			string[] array = globalVariable.Split('@');
			return new Vector3(float.Parse(array[0], CultureInfo.InvariantCulture), float.Parse(array[1], CultureInfo.InvariantCulture), float.Parse(array[2], CultureInfo.InvariantCulture));
		}

		public static void SetMapCameraPosition(Vector3 position)
		{
			SetGlobalVariable("MapCameraPosition", position.x.ToString(CultureInfo.InvariantCulture) + '@' + position.y.ToString(CultureInfo.InvariantCulture) + '@' + position.z.ToString(CultureInfo.InvariantCulture));
		}

		public static int GetSelectedBattleID()
		{
			return int.Parse(GetGlobalVariable("SelectedBattle"));
		}

		public static void SetSelectedBattle(int battleID)
		{
			SetGlobalVariable("SelectedBattle", battleID, false, JSONBinaryTag.IntValue);
		}

		public static void SetFirstRun(bool isFirstRun)
		{
			SetGlobalVariable("FirstRun", (!isFirstRun) ? 1 : 0, false, JSONBinaryTag.IntValue);
		}

		public static void SetTutorialState(int state)
		{
			SetGlobalVariable("STATE_tutorial", state, false, JSONBinaryTag.IntValue);
		}

		public static void ResetSelectedBattle()
		{
			SetSelectedBattle(0);
		}

		public static void SetMapScale(float scale)
		{
			SetUserValueAndSave("MapScale", scale, JSONBinaryTag.FloatValue);
		}

		public static float GetMapScale()
		{
			string userValue = GetUserValue("MapScale", string.Empty);
			return (userValue.Length != 0) ? Convert.ToSingle(userValue) : 1f;
		}

		public static void SetPlayerID(ExtendedPlayer extPlayer)
		{
			if (extPlayer != null && extPlayer.PrimaryPlayer != null)
			{
				long playerId = extPlayer.PrimaryPlayer.PublicPlayer.ShortPlayer.PlayerId;
				SetPlayerID(playerId);
			}
		}

		public static bool SetABTag(string abTag)
		{
			Debug.Log("<color=green>ABTag: " + abTag + "</color>");
			string aBTag = GetABTag();
			if (!aBTag.Equals(abTag))
			{
				SetUserValue("ABTag", abTag);
				return true;
			}
			return false;
		}

		public static string GetABTag()
		{
			return GetUserValue("ABTag", string.Empty);
		}

		public static void SetPlayerID(long id)
		{
			Debug.Log("Player ID set to " + id);
			SetUserValueAndSave("PlayerID", id, JSONBinaryTag.LongValue);
		}

		public static void SetPlayerID(NetworkEvent e)
		{
			SetPlayerID(e.getExtensible<ExtendedPlayer>());
		}

		public static long GetPlayerID()
		{
			string userValue = GetUserValue("PlayerID", string.Empty);
			return (userValue.Length != 0) ? Convert.ToInt64(userValue) : (-1);
		}

		public static void TutorialComplete()
		{
			RemoveGlobalVariable("TUTORIAL");
			IsStartingTutorialCompleted = true;
			Instance.SaveFileUser(true);
			UserManager.OnStartingTutorialCompleted();
			NetworkConnection.current.ExitDarkPocket();
			NetworkConnection.current.BlockInputUntilNetworkEstablished();
		}

		public static bool IsTutorialComplete()
		{
			string globalVariable = GetGlobalVariable("TUTORIAL");
			return globalVariable == null || globalVariable.Equals("0");
		}

		private string JSToJson(string jsContents)
		{
			string[] skippedNames = new string[3] { "Permissions", "Gender", "Currencies" };
			string[] skippedClasses = new string[3] { "Equipments", "Perks", "Boosters" };
			return JSUtils.JSToJson(jsContents, "User", skippedNames, skippedClasses);
		}

		private string SaveAdditionalFields(string jsonFromJs)
		{
			JSONNode jSONNode = JSON.Parse(jsonFromJs);
			JSONClass asObject = jSONNode["User"].AsObject;
			Dictionary<string, JsValue> jsDict = JS.Instance.GetScope("PlayerDefaults").AsDictionary();
			AddGender(asObject, jsDict);
			AddCurrencyBlock(asObject, jsDict);
			AddEquipmentsBlock(asObject, jsDict);
			AddPerksBlock(asObject, jsDict);
			AddAppearanceBlock(asObject, jsDict);
			return jSONNode.ToJSON(0);
		}

		private void AddGender(JSONClass user, Dictionary<string, JsValue> jsDict)
		{
			if (jsDict.ContainsKey("Gender"))
			{
				int key = jsDict["Gender"].AsInteger();
				Dictionary<int, string> enumDictionary = JS.Instance.EnumsCompliancer.GetEnumDictionary("Gender".ToUpper());
				string text = enumDictionary[key];
				user["Gender"] = text;
			}
		}

		private void AddAppearanceBlock(JSONClass user, Dictionary<string, JsValue> jsDict)
		{
			JSONClass jSONClass = new JSONClass();
			Dictionary<string, JsValue> dictionary = jsDict["Appearance"].AsDictionary();
			jSONClass.Add("Head", new JSONData(dictionary["Head"].AsDictionary()["Head"].AsString()));
			JSONClass jSONClass2 = new JSONClass();
			jSONClass2.Add("Color", dictionary["HairColor"].AsDictionary()["Color"].AsDictionary()["ID"].AsInteger().ToString());
			jSONClass2.Add("Value", dictionary["HairColor"].AsDictionary()["Value"].AsFloat().ToString());
			jSONClass.Add("HairColor", jSONClass2);
			jSONClass2 = new JSONClass();
			jSONClass2.Add("Color", dictionary["SkinColor"].AsDictionary()["Color"].AsDictionary()["ID"].AsInteger().ToString());
			jSONClass2.Add("Value", dictionary["SkinColor"].AsDictionary()["Value"].AsFloat().ToString());
			jSONClass.Add("SkinColor", jSONClass2);
			user["Appearance"] = jSONClass;
		}

		private void AddCurrencyBlock(JSONClass user, Dictionary<string, JsValue> jsDict)
		{
			JSONClass jSONClass = new JSONClass();
			Dictionary<string, JsValue> dictionary = jsDict["Currencies"].AsDictionary();
			Dictionary<string, int> enumDictionaryInverted = JS.Instance.EnumsCompliancer.GetEnumDictionaryInverted("Currency".ToUpper());
			string key = enumDictionaryInverted["Bonus"].ToString();
			string key2 = enumDictionaryInverted["Coin"].ToString();
			int aData = dictionary[key].AsInteger();
			jSONClass.Add("Bonus", new JSONData(aData));
			int aData2 = dictionary[key2].AsInteger();
			jSONClass.Add("Coin", new JSONData(aData2));
			user["Currency"] = jSONClass;
		}

		private void AddEquipmentsBlock(JSONClass user, Dictionary<string, JsValue> jsDict)
		{
			JSONArray jSONArray = new JSONArray();
			string text = "Equipments";
			IEnumerable<KeyValuePair<string, PropertyDescriptor>> ownProperties = jsDict[text].AsArray().GetOwnProperties();
			foreach (KeyValuePair<string, PropertyDescriptor> item in ownProperties)
			{
				PropertyDescriptor value = item.Value;
				JSONClass jSONClass = new JSONClass();
				if (value.Value.IsObject())
				{
					Dictionary<string, JsValue> dictionary = value.Value.AsDictionary();
					Dictionary<string, JsValue> itemDict = dictionary["Model"].AsObject().AsDictionary();
					SetIntValue(jSONClass, itemDict, "ID");
					SetStringValue(jSONClass, itemDict, "Alias");
					SetStringValue(jSONClass, itemDict, "Model");
					SetIntValue(jSONClass, dictionary, "Equipped", 1);
					SetIntValue(jSONClass, itemDict, "Default");
					SetIntValue(jSONClass, itemDict, "Level");
					SetIntValue(jSONClass, itemDict, "Hidden");
					SetIntValue(jSONClass, itemDict, "Slots");
					SetDoubleValue(jSONClass, dictionary, "StackLevel", "SL");
					SetEnumValue(jSONClass, itemDict, "Type", "ItemType");
					SetEnumValue(jSONClass, itemDict, "Faction");
					SetEnumValue(jSONClass, itemDict, "Rarity");
					SetStringArrayValue(jSONClass, itemDict, "Tags");
					SetStringArrayValue(jSONClass, itemDict, "ShadowTag");
					SetStringValue(jSONClass, itemDict, "ShadowMark");
					AddEquipmentPerks(jSONClass, dictionary, "Perks");
					jSONArray.Add(jSONClass);
				}
			}
			user[text] = jSONArray;
		}

		private void AddEquipmentPerks(JSONClass itemClass, Dictionary<string, JsValue> itemDict, string key)
		{
			if (!itemDict.ContainsKey(key))
			{
				return;
			}
			JSONArray jSONArray = new JSONArray();
			IEnumerable<KeyValuePair<string, PropertyDescriptor>> ownProperties = itemDict[key].AsArray().GetOwnProperties();
			foreach (KeyValuePair<string, PropertyDescriptor> item in ownProperties)
			{
				PropertyDescriptor value = item.Value;
				JSONClass jSONClass = new JSONClass();
				if (value.Value.IsObject())
				{
					Dictionary<string, JsValue> dictionary = value.Value.AsDictionary();
					Dictionary<string, JsValue> itemDict2 = dictionary["Perk"].AsObject().AsDictionary();
					SetIntValue(jSONClass, itemDict2, "ID");
					SetDoubleValue(jSONClass, dictionary, "StackLevel", "SL");
					SetIntValue(jSONClass, dictionary, "SlotIndex", null, "Slot");
					jSONArray.Add(jSONClass);
				}
			}
			itemClass[key] = jSONArray;
		}

		private void AddPerksBlock(JSONClass user, Dictionary<string, JsValue> jsDict)
		{
			JSONArray jSONArray = new JSONArray();
			string text = "Perks";
			IEnumerable<KeyValuePair<string, PropertyDescriptor>> ownProperties = jsDict[text].AsArray().GetOwnProperties();
			foreach (KeyValuePair<string, PropertyDescriptor> item in ownProperties)
			{
				PropertyDescriptor value = item.Value;
				JSONClass jSONClass = new JSONClass();
				if (value.Value.IsObject())
				{
					Dictionary<string, JsValue> dictionary = value.Value.AsDictionary();
					Dictionary<string, JsValue> itemDict = dictionary["Model"].AsObject().AsDictionary();
					SetIntValue(jSONClass, itemDict, "ID");
					SetDoubleValue(jSONClass, dictionary, "StackLevel", "SL");
					SetIntValue(jSONClass, itemDict, "Slot");
					jSONArray.Add(jSONClass);
				}
			}
			user[text] = jSONArray;
		}

		private static void SetIntValue(JSONClass itemClass, Dictionary<string, JsValue> itemDict, string key, int? defaultValue = null, string firstKey = null)
		{
			string key2 = firstKey ?? key;
			if (itemDict.ContainsKey(key2))
			{
				itemClass[key].AsInt = (defaultValue.HasValue ? defaultValue.Value : itemDict[key2].AsInteger());
			}
		}

		private static void SetDoubleValue(JSONClass itemClass, Dictionary<string, JsValue> itemDict, string key, string firstKey = null)
		{
			if (itemDict.ContainsKey(firstKey ?? key))
			{
				itemClass[key].AsDouble = itemDict[firstKey ?? key].AsNumber();
			}
		}

		private static void SetStringValue(JSONClass itemClass, Dictionary<string, JsValue> itemDict, string key)
		{
			if (itemDict.ContainsKey(key))
			{
				itemClass[key] = itemDict[key].AsString();
			}
		}

		private static void SetEnumValue(JSONClass itemClass, Dictionary<string, JsValue> itemDict, string key, string firstKey = null)
		{
			string text = firstKey ?? key;
			if (itemDict.ContainsKey(text))
			{
				Dictionary<int, string> enumDictionary = JS.Instance.EnumsCompliancer.GetEnumDictionary(text.ToUpper());
				itemClass[key] = enumDictionary[itemDict[text].AsInteger()].ToString();
			}
		}

		private static void SetStringArrayValue(JSONClass itemClass, Dictionary<string, JsValue> itemDict, string key)
		{
			if (!itemDict.ContainsKey(key))
			{
				return;
			}
			JSONArray jSONArray = new JSONArray();
			IEnumerable<KeyValuePair<string, PropertyDescriptor>> ownProperties = itemDict[key].AsArray().GetOwnProperties();
			foreach (KeyValuePair<string, PropertyDescriptor> item in ownProperties)
			{
				if (item.Value.Value.IsString())
				{
					jSONArray.Add(item.Value.Value.AsString());
				}
			}
			itemClass[key] = jSONArray;
		}

		public JSONNode YamlMappingToJsonNode(Node yamlNode)
		{
			if (yamlNode == null)
			{
				return null;
			}
			JSONNode jSONNode = ((!(yamlNode is Mapping)) ? ((JSONNode)new JSONArray()) : ((JSONNode)new JSONClass()));
			foreach (Node item in yamlNode)
			{
				Scalar scalar = item as Scalar;
				if (scalar != null)
				{
					TryParseScalarType(jSONNode, scalar);
					continue;
				}
				Mapping mapping = item as Mapping;
				if (mapping != null)
				{
					jSONNode.Add(mapping.key, YamlMappingToJsonNode(mapping));
					continue;
				}
				Sequence sequence = item as Sequence;
				if (sequence != null)
				{
					jSONNode.Add(sequence.key, YamlMappingToJsonNode(sequence));
				}
			}
			return jSONNode;
		}

		private void TryParseScalarType(JSONNode node, Scalar scalar)
		{
			string text = scalar.text;
			node[scalar.key] = scalar.text;
			int result;
			long result2;
			float result3;
			double result4;
			bool result5;
			if (int.TryParse(text, out result))
			{
				node[scalar.key].AsInt = result;
			}
			else if (long.TryParse(text, out result2))
			{
				node[scalar.key].AsLong = result2;
			}
			else if (float.TryParse(text, out result3))
			{
				node[scalar.key].AsFloat = result3;
			}
			else if (double.TryParse(text, out result4))
			{
				node[scalar.key].AsDouble = result4;
			}
			else if (bool.TryParse(text, out result5))
			{
				node[scalar.key].AsBool = result5;
			}
		}

		static UserManager()
		{
			UserManager.OnLevelChanged = delegate
			{
			};
			UserManager.OnExperienceChanged = delegate
			{
			};
			UserManager.OnStartingTutorialCompleted = delegate
			{
			};
		}
	}
}
