using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Jint.Native;
using Nekki;
using Nekki.Utils;
using Nekki.Yaml;
using Network.core.events;
using SF3.Audio;
using SF3.UserData;
using UnityEngine;
using sf3DTO;

namespace SF3
{
	public class BattlesManager : MonoBehaviour, IUserDataManager
	{
		private static BattleType _currentBattleType;

		private MissionsHolder _missionsHolder;

		private JSBattlesHolder _jsBattlesHolder;

		private Dictionary<int, IBattleInfo> _battles;

		private Dictionary<int, IBattleInfo> _battlesFutureAvailable;

		private string _battlesFilePath;

		private YamlDocumentNekki _battlesYaml;

		private Sequence _battlesNode;

		private bool _isSaveFileByTimer;

		public static IBattleInfo LastCompleteBattle;

		public static BattlesManager instance { get; private set; }

		public static IBattleInfo currentBattle { get; private set; }

		public static BattleType currentBattleType
		{
			get
			{
				return _currentBattleType;
			}
		}

		public static event Action OnBattlesUpdate;

		public static void Create()
		{
			if (instance == null)
			{
				BattlesManager battlesManager = new GameObject("_battlesManager").AddComponent<BattlesManager>();
				StaticObjectsManager.AddObject(battlesManager.gameObject);
			}
		}

		private void Awake()
		{
			instance = this;
			UserDataController.AddUserDataManager(UserDataManagerType.Battles, this);
		}

		private void UpdateEverySecond()
		{
			if (_missionsHolder != null)
			{
				_missionsHolder.UpdateTimers();
			}
		}

		public void Initialize()
		{
			GlobalTimer.Instnce.addEventListener(0, OnTimerTick);
			_jsBattlesHolder = new JSBattlesHolder();
			_battles = new Dictionary<int, IBattleInfo>();
			_battlesFutureAvailable = new Dictionary<int, IBattleInfo>();
			InitializeFromYAML();
			InitializeDojo();
			InitializeTestBattles();
			CheckFutureAvailableBattles();
			int selectedBattleID = UserManager.GetSelectedBattleID();
			if (GetBattle(selectedBattleID) == null)
			{
				IBattleInfo defaultBattle = GetDefaultBattle();
				if (defaultBattle != null)
				{
					UserManager.SetSelectedBattle(GetDefaultBattle().GetID());
				}
			}
			_missionsHolder = new MissionsHolder();
			InvokeRepeating("UpdateEverySecond", 0f, 1f);
			SaveFileBattles(true);
		}

		private void InitializeFromYAML()
		{
			_battlesFilePath = GlobalPath.GameDataCombine("User/battles.txt");
			string fileOrResourcesText = GlobalLoad.GetFileOrResourcesText(_battlesFilePath, "battlesDefault", string.Empty);
			_battlesYaml = YamlDocumentNekki.FromYamlContent(fileOrResourcesText);
			_battlesNode = _battlesYaml.GetRoot().GetSequence("Battles");
			for (int i = 0; i < _battlesNode.nodesInside.Count; i++)
			{
				Mapping mapping = (Mapping)_battlesNode.nodesInside[i];
				IBattleInfo battleInfo = null;
				if (mapping.GetSequence("Battles") == null)
				{
					battleInfo = BattleInfo.Create(mapping);
				}
				else
				{
					battleInfo = GenericBattleInfo.Create(mapping);
					if (battleInfo.HasExpirationTime())
					{
						battleInfo.SetBattleAvailable(false);
						NetworkConnection.TimeDispatcher.callDelegateAt(battleInfo.GetExpirationTime().GetUnixTimeStampMilliseconds(), BattleRefreshed, battleInfo);
						BattlesManager.OnBattlesUpdate();
					}
				}
				AddBattle(battleInfo);
			}
		}

		public void InitializeDojo()
		{
			string[] currentDojo = UserManager.GetCurrentDojo();
			BattleInfo jSBattle = GetJSBattle(int.Parse(currentDojo[0]));
			AddBattle(jSBattle);
		}

		private void InitializeTestBattles()
		{
			List<BattleInfo> battles = _jsBattlesHolder.GetBattles(sf3DTO.BattleType.Test);
			foreach (BattleInfo item in battles)
			{
				AddBattle(item, false);
			}
		}

		public void UpdateUserData(IMessage userBattleData)
		{
			BattleData battleData = null;
			battleData = ((!(userBattleData is Player)) ? ((BattleData)userBattleData) : ((Player)userBattleData).BattleData);
			UpdateBattlesData(battleData);
			int battleID = JS.CallFunction("getBrawlerBattleID", UserManager.Instance.GetCurrentChapter()).AsInteger();
			AddBattle(GetJSBattle(battleID));
			int selectedBattleID = UserManager.GetSelectedBattleID();
			if (GetBattle(selectedBattleID) == null)
			{
				UserManager.SetSelectedBattle(GetDefaultBattle().GetID());
			}
			BattlesManager.OnBattlesUpdate();
		}

		public void UpdateBattlesData(BattleData userBattleData)
		{
			if (userBattleData == null || userBattleData.Battles.Count == 0)
			{
				return;
			}
			if (_battlesNode != null)
			{
				_battlesNode.RemoveNodes();
			}
			Dictionary<int, IBattleInfo> dictionary = new Dictionary<int, IBattleInfo>(_battles);
			_battles.Clear();
			_battlesFutureAvailable.Clear();
			foreach (Battle battle in userBattleData.Battles)
			{
				IBattleInfo battleInfo = ((!dictionary.ContainsKey(battle.Battles[0].ModelId) || !(dictionary[battle.Battles[0].ModelId].GetGenerationTime() == NekkiUtils.GetUnixDateTimeFromMilliseconds(battle.GenTime.Value))) ? BattleInfo.Create(battle) : dictionary[battle.Battles[0].ModelId]);
				if (!battleInfo.HasExpirationTime())
				{
					battleInfo.SetBattleAvailable(true);
				}
				battleInfo.SetBattleHidden(false);
				battleInfo.MergeWith(battle, 0);
				AddBattle(battleInfo);
			}
			foreach (KeyValuePair<int, IBattleInfo> item in dictionary)
			{
				if (item.Value.GetBattleType() == sf3DTO.BattleType.Dojo || item.Value.GetBattleType() == sf3DTO.BattleType.Test)
				{
					AddBattle(item.Value, item.Value.GetBattleType() == sf3DTO.BattleType.Dojo);
				}
			}
			SaveFileBattles(true);
		}

		private void AddBattle(IBattleInfo resultBattle, bool saveToYAML = true)
		{
			if (_battles.ContainsKey(resultBattle.GetID()))
			{
				_battles[resultBattle.GetID()] = resultBattle;
			}
			else
			{
				_battles.Add(resultBattle.GetID(), resultBattle);
			}
			if (saveToYAML)
			{
				UpdateBattleInYAML(resultBattle);
			}
		}

		public void AddBattleLocal(int battleID)
		{
			if (!_battles.ContainsKey(battleID))
			{
				IBattleInfo battle = _jsBattlesHolder.GetBattle(battleID);
				if (battle == null)
				{
					Debug.LogError(string.Format("Cant find battle with id [{0}]", battleID));
					return;
				}
				battle.SetBattleAvailable(false);
				AddBattle(battle);
			}
		}

		private void UpdateBattleInYAML(IBattleInfo battleData)
		{
			bool flag = false;
			for (int i = 0; i < _battlesNode.nodesInside.Count; i++)
			{
				Mapping mapping = (Mapping)_battlesNode.nodesInside[i];
				if (mapping.GetText("ID").text.Equals(battleData.GetID().ToString()))
				{
					_battlesNode.Replace(i, battleData.ToYAML());
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				_battlesNode.AddNode(battleData.ToYAML());
			}
			SaveFileBattles();
		}

		public static bool NeedSendChapter()
		{
			int currentChapter = UserManager.Instance.GetCurrentChapter();
			int? nextChapter = instance.GetNextChapter();
			int availableChapter = instance.GetAvailableChapter();
			return nextChapter.HasValue && currentChapter != nextChapter && ((availableChapter < 0 && instance.GetMainBattle() == null) || (availableChapter >= 0 && currentChapter != availableChapter));
		}

		public static FightInfo GetFightInfo(int battleID, int fightID)
		{
			return (instance.GetBattle(battleID) == null) ? null : instance.GetBattle(battleID).GetFights()[fightID];
		}

		public static void CompleteFight(FightResult result)
		{
			if (result.CanGiveRewards())
			{
				UserManager.GiveRewards(result);
			}
			result.currentBattle.CompleteFight(result.resultType);
			if (result.currentBattle.GetIsCompleted())
			{
				instance.SetBattleHidden(result.currentBattle.GetID(), true);
				LastCompleteBattle = result.currentBattle;
			}
			instance.AddAvailableBattles(result);
			UserDataController.Send_FinishFight(result);
			instance.UpdateBattleInYAML(result.currentBattle);
		}

		public static void EnterBattle(FightInfo enterFight, Action onEnterCallback = null)
		{
			if (enterFight == null)
			{
				Debug.LogError("enterFight is null");
				return;
			}
			IBattleInfo battle = instance.GetBattle(enterFight.battleID);
			if (!battle.GetIsAvailable())
			{
				Debug.LogError(string.Format("Battle with id [{0}] is unavailable", enterFight.battleID));
				return;
			}
			currentBattle = battle;
			_currentBattleType = ((enterFight.battleID != UserManager.CurrentDojoBattle.GetID()) ? BattleType.Fight : BattleType.Dojo);
			if (!currentBattle.GetIsCompleted() && currentBattle.HasCooldown())
			{
				currentBattle.SetBattleAvailable(false);
				currentBattle.SetExpirationTime();
				NetworkConnection.TimeDispatcher.callDelegateAt(currentBattle.GetExpirationTime().GetUnixTimeStampMilliseconds(), instance.BattleRefreshed, currentBattle);
				BattlesManager.OnBattlesUpdate();
			}
			instance.UpdateBattleInYAML(currentBattle);
			LocationInfo location = currentBattle.GetBattleInfo().location;
			AudioManager.Instance.ClearAllSound();
			LocationAudioSettings.PlayMusicByName(location.music);
			SceneManager.Instance.LoadLocationScene(location.locationName, onEnterCallback);
		}

		public List<IBattleInfo> GetMissions()
		{
			return (from info in _battles
				where info.Value.GetBattleType() == sf3DTO.BattleType.Mission
				select info.Value).ToList();
		}

		public BattleInfo GetJSBattle(int battleID)
		{
			return _jsBattlesHolder.GetBattle(battleID);
		}

		public void LoadBattleFromServerCHEAT(int battleID, Action<IBattleInfo> callback = null, bool saveInYAMLIDData = true)
		{
			if (currentBattleType == BattleType.Fight)
			{
				return;
			}
			NetworkConnection.Send("cheat_generate_battle", new Int32Value
			{
				Value = battleID
			}, delegate(NetworkEvent e)
			{
				Battle extensible = e.getExtensible<Battle>();
				if (extensible == null)
				{
					Debug.LogError("Server does not want to give us a battle :c ");
				}
				else
				{
					Debug.Log(string.Format("Succesfully get battle with ID [{0}] from server", extensible.Battles[0].ModelId));
					List<BattleInfo> list = extensible.Battles.Select((GeneratedBattle bt) => GetJSBattle(bt.ModelId)).ToList();
					GenericBattleInfo genericBattleInfo = new GenericBattleInfo(list);
					if (genericBattleInfo != null)
					{
						try
						{
							genericBattleInfo.MergeWith(extensible, 0);
						}
						catch (Exception ex)
						{
							Debug.LogError(ex.Message);
						}
						Debug.Log(string.Format("Succesfully merge battle with ID [{0}] in real battle", list[0].id));
						genericBattleInfo.SetTestBattleType();
						AddBattle(genericBattleInfo);
					}
					else
					{
						Debug.LogError(string.Format("Havnt battle with id [{0}] in JS.", list[0].id));
					}
					if (callback != null)
					{
						callback(genericBattleInfo);
					}
				}
			});
		}

		private void SetBattleHidden(int visibleBattleID, bool isHidden)
		{
			if (visibleBattleID != UserManager.CurrentDojoBattle.GetID() && _battles.ContainsKey(visibleBattleID))
			{
				bool flag = isHidden || _battles[visibleBattleID].GetIsCompleted();
				if (_battles[visibleBattleID].GetIsHidden() != flag)
				{
					_battles[visibleBattleID].SetBattleHidden(flag);
					UpdateBattleInYAML(_battles[visibleBattleID]);
				}
			}
		}

		public List<IBattleInfo> GetBattlesFutureAvailable()
		{
			return _battlesFutureAvailable.Values.ToList();
		}

		public List<IBattleInfo> GetBattles()
		{
			return _battles.Values.ToList();
		}

		public List<IBattleInfo> GetBattlesVisible()
		{
			List<IBattleInfo> battles = GetBattles();
			return battles.Where((IBattleInfo btt) => !btt.GetIsHidden()).ToList();
		}

		public IBattleInfo GetBattlesVisibleByType(sf3DTO.BattleType type)
		{
			List<IBattleInfo> battlesVisible = GetBattlesVisible();
			foreach (IBattleInfo item in battlesVisible)
			{
				if (item.GetBattleType() == type)
				{
					return item;
				}
			}
			return null;
		}

		public IBattleInfo GetBattle(int battleID)
		{
			if (_battles.ContainsKey(battleID))
			{
				return _battles[battleID];
			}
			return (!_battlesFutureAvailable.ContainsKey(battleID)) ? null : _battlesFutureAvailable[battleID];
		}

		private void AddAvailableBattles(FightResult result)
		{
			if (!(result.currentBattle is BattleInfo))
			{
				return;
			}
			_battlesFutureAvailable.Clear();
			List<int> availableBattlesFromGraph = _jsBattlesHolder.battlesGraph.GetAvailableBattlesFromGraph((BattleInfo)result.currentBattle);
			foreach (int item in availableBattlesFromGraph)
			{
				if (!_battles.ContainsKey(item))
				{
					BattleInfo jSBattle = GetJSBattle(item);
					jSBattle.SetBattleAvailable(false);
					_battlesFutureAvailable.Add(item, jSBattle);
				}
			}
		}

		public int GetAvailableChapter()
		{
			IBattleInfo mainBattle = GetMainBattle(_battlesFutureAvailable);
			return (mainBattle != null) ? mainBattle.GetChapters()[0] : (-1);
		}

		public IBattleInfo GetMainBattle(Dictionary<int, IBattleInfo> battles = null)
		{
			Dictionary<int, IBattleInfo> dictionary = battles ?? _battles;
			return dictionary.Values.FirstOrDefault((IBattleInfo b) => b.GetBattleType() == sf3DTO.BattleType.Main);
		}

		private int GetMainBattleID(int currentChapter = -1)
		{
			if (currentChapter == UserManager.Instance.GetCurrentChapter())
			{
				IBattleInfo mainBattle = GetMainBattle();
				if (mainBattle != null)
				{
					return mainBattle.GetID();
				}
			}
			Dictionary<string, JsValue> dictionary = JS.Instance.GetScope("Battles").AsDictionary();
			foreach (KeyValuePair<string, JsValue> item in dictionary)
			{
				Dictionary<string, JsValue> dictionary2 = item.Value.AsDictionary();
				int num = dictionary2["Chapters"].AsArray()[0].AsDictionary()["ID"].AsInteger();
				if (num == currentChapter)
				{
					return dictionary2["ID"].AsInteger();
				}
			}
			return -1;
		}

		public int? GetNextChapter()
		{
			IBattleInfo mainBattle = GetMainBattle(_battlesFutureAvailable);
			if (mainBattle != null)
			{
				return mainBattle.GetChapters()[0];
			}
			int mainBattleID = GetMainBattleID(UserManager.Instance.GetCurrentChapter());
			List<int> availableBattlesFromGraph = _jsBattlesHolder.battlesGraph.GetAvailableBattlesFromGraph(mainBattleID, int.MaxValue);
			int battleID = availableBattlesFromGraph.FirstOrDefault((int b) => GetJSBattle(b).GetBattleType() == sf3DTO.BattleType.Main);
			mainBattle = GetJSBattle(battleID);
			if (mainBattle == null)
			{
				return null;
			}
			return mainBattle.GetChapters()[0];
		}

		private void CheckFutureAvailableBattles()
		{
			_battlesFutureAvailable.Clear();
			List<int> list = new List<int>();
			list.AddRange(_jsBattlesHolder.battlesGraph.GetAvailableBattlesFromGraph(_battles.Values.OfType<BattleInfo>().ToList()));
			foreach (int item in list)
			{
				if (!_battles.ContainsKey(item))
				{
					BattleInfo jSBattle = GetJSBattle(item);
					jSBattle.SetBattleAvailable(false);
					_battlesFutureAvailable.Add(item, jSBattle);
				}
			}
		}

		private void BattleRefreshed(object bt)
		{
			IBattleInfo battleInfo = (IBattleInfo)bt;
			battleInfo.SetBattleAvailable(true);
			battleInfo.ClearExpirationTime();
			BattlesManager.OnBattlesUpdate();
			UpdateBattleInYAML(battleInfo);
		}

		private void SaveFileBattles(bool forcibly = false)
		{
			if (forcibly)
			{
				_isSaveFileByTimer = false;
				_battlesYaml.SaveToFile(_battlesFilePath, false);
			}
			else
			{
				_isSaveFileByTimer = true;
			}
		}

		private void OnTimerTick(CallEventArgs callEventArgs)
		{
			if (_isSaveFileByTimer)
			{
				_isSaveFileByTimer = false;
				SaveFileBattles(true);
			}
		}

		public List<IBattleInfo> GetBattlesVisibleAvaible()
		{
			List<IBattleInfo> battlesVisible = GetBattlesVisible();
			List<IBattleInfo> list = new List<IBattleInfo>();
			foreach (IBattleInfo item in battlesVisible)
			{
				if (item.GetIsAvailable())
				{
					list.Add(item);
				}
			}
			return list;
		}

		public IBattleInfo GetDefaultBattle()
		{
			IBattleInfo result = null;
			List<IBattleInfo> battlesVisibleAvaible = GetBattlesVisibleAvaible();
			if (battlesVisibleAvaible.Count > 0)
			{
				battlesVisibleAvaible.Sort(new BattlesComp());
				result = battlesVisibleAvaible[0];
			}
			return result;
		}

		static BattlesManager()
		{
			BattlesManager.OnBattlesUpdate = delegate
			{
			};
			_currentBattleType = BattleType.Dojo;
		}
	}
}
