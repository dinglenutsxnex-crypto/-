using System;
using System.Collections.Generic;
using System.Linq;
using SF3.UserData;
using UnityEngine;
using sf3DTO;

namespace SF3
{
	public class MapBattleUI : MonoBehaviour
	{
		[SerializeField]
		private MapBattleButton _mapBattleIconPrefab;

		[SerializeField]
		private GameObject _missionsTimerObject;

		[SerializeField]
		private UILabel _missionsTimerLabel;

		[SerializeField]
		private GameObject _mapPanelObject;

		private Dictionary<int, MapBattleButton> _mapBattlesIcons;

		private GameObject _gameObject;

		public List<MapBattleButton> mapBattlesIcons
		{
			get
			{
				return _mapBattlesIcons.Values.ToList();
			}
		}

		public IBattleInfo selectedBattle { get; private set; }

		public void Initialize()
		{
			_gameObject = base.gameObject;
			_mapBattlesIcons = new Dictionary<int, MapBattleButton>();
			selectedBattle = null;
			MissionsHolder.instance.OnActivateTimer += SetMissionsTimerActive;
			MissionsHolder.instance.OnTimerUpdate += UpdateMissionsTimer;
		}

		public void SetMissionsTimerActive(bool active)
		{
			if (_missionsTimerObject.activeSelf != active)
			{
				_missionsTimerObject.SetActive(active);
			}
		}

		private void OnDestroy()
		{
			MissionsHolder.instance.OnActivateTimer -= SetMissionsTimerActive;
			MissionsHolder.instance.OnTimerUpdate -= UpdateMissionsTimer;
		}

		public void UpdateMissionsTimer(TimeSpan timeToBattle)
		{
			string text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeToBattle.Hours, timeToBattle.Minutes, timeToBattle.Seconds);
			_missionsTimerLabel.text = text;
		}

		public void RemoveMapBattle(int battleID)
		{
			if (_mapBattlesIcons.ContainsKey(battleID) && (bool)_mapBattlesIcons[battleID].transform)
			{
				if (selectedBattle != null && selectedBattle.GetID() == battleID)
				{
					UnSelectBattle();
				}
				UnityEngine.Object.Destroy(_mapBattlesIcons[battleID].transform.gameObject);
				_mapBattlesIcons.Remove(battleID);
			}
		}

		public void RemoveMapBattle(IBattleInfo battle)
		{
			RemoveMapBattle(battle.GetID());
		}

		public MapBattleButton CreateMapBattle(IBattleInfo battle, MapBattleButton.DecorationType decorationType)
		{
			BattleInfo battleInfo = battle.GetBattleInfo();
			if (!_mapBattlesIcons.ContainsKey(battle.GetID()))
			{
				MapBattleButton mapBattleButton = UnityEngine.Object.Instantiate(_mapBattleIconPrefab);
				mapBattleButton.Initialize(battleInfo.id, _gameObject.transform, battleInfo.name, battleInfo.icon, decorationType, _mapPanelObject);
				_mapBattlesIcons.Add(battleInfo.id, mapBattleButton);
				if (battle.GetBattleType() == sf3DTO.BattleType.Mission && battle.GetIsCompleted())
				{
					mapBattleButton.gameObject.SetActive(false);
				}
				if (battle.HasExpirationTime())
				{
					mapBattleButton.SetTimer(battle.GetExpirationTime());
				}
				if (battle.GetBattleType() == sf3DTO.BattleType.Daily && battle.GetIsHidden())
				{
					DateTime nextDailyUpdateTime = ((DailyBattleInfo)battle).NextDailyUpdateTime;
					if (nextDailyUpdateTime != DateTime.MinValue)
					{
						mapBattleButton.SetTimer(nextDailyUpdateTime);
						NetworkConnection.TimeDispatcher.removeDelegate(OnDailyUpdate);
						long unixTimeStampMilliseconds = nextDailyUpdateTime.GetUnixTimeStampMilliseconds();
						NetworkConnection.TimeDispatcher.callDelegateAt(unixTimeStampMilliseconds, OnDailyUpdate);
					}
				}
			}
			UpdateBattleIconPosition(battleInfo);
			return _mapBattlesIcons[battle.GetID()];
		}

		private void OnDailyUpdate(object args)
		{
			MapController.Instance.MissClick();
			RefreshBattlesProcessing.RefreshBattles();
		}

		public void UpdateBattleIconPosition(BattleInfo battle)
		{
			_mapBattlesIcons[battle.id].transform.localPosition = GetBattleIconPosition(battle);
		}

		public void UpdateBattleIconScale(float scale)
		{
			foreach (MapBattleButton value in _mapBattlesIcons.Values)
			{
				value.transform.localPosition *= scale;
			}
		}

		public Vector3 GetBattleIconPosition(BattleInfo battle)
		{
			return GetBattleIconPosition(battle.GetLocation());
		}

		internal Vector3 GetBattleIconPosition(LocationInfo info)
		{
			return new Vector2((0f - _gameObject.GetComponent<UIWidget>().localSize.x) / 2f + info.position.x, _gameObject.GetComponent<UIWidget>().localSize.y / 2f - info.position.y);
		}

		public Vector3 GetSelectedBattleIconPosition()
		{
			return GetBattleIconPosition(selectedBattle.GetBattleInfo());
		}

		public void ClearBattles()
		{
			if (_mapBattlesIcons.Count > 0)
			{
				foreach (MapBattleButton value in _mapBattlesIcons.Values)
				{
					if ((bool)value.transform)
					{
						UnityEngine.Object.Destroy(value.transform.gameObject);
					}
				}
			}
			_mapBattlesIcons.Clear();
		}

		public IBattleInfo GetBattleMapp(int userSelectedBattle)
		{
			List<IBattleInfo> battlesVisible = BattlesManager.instance.GetBattlesVisible();
			foreach (IBattleInfo item in battlesVisible)
			{
				if (item.GetID() == userSelectedBattle)
				{
					return item;
				}
			}
			IBattleInfo battlesVisibleByType = BattlesManager.instance.GetBattlesVisibleByType(sf3DTO.BattleType.Main);
			if (battlesVisibleByType != null)
			{
				return battlesVisibleByType;
			}
			return (battlesVisible.Count <= 0) ? null : battlesVisible[0];
		}

		public void SelectBattle(IBattleInfo battle)
		{
			UnSelectBattle();
			selectedBattle = battle;
			if (selectedBattle != null && !selectedBattle.GetIsHidden())
			{
				_mapBattlesIcons[selectedBattle.GetID()].EnableSelector(true);
			}
		}

		public void UnSelectBattle()
		{
			if (selectedBattle != null && _mapBattlesIcons.ContainsKey(selectedBattle.GetID()))
			{
				_mapBattlesIcons[selectedBattle.GetID()].EnableSelector(false);
				selectedBattle = null;
			}
		}

		public bool IsSelectBattle(int battleID)
		{
			return selectedBattle != null && selectedBattle.GetID() == battleID;
		}

		public MapBattleButton GetIcon(int battleID)
		{
			return (!_mapBattlesIcons.ContainsKey(battleID)) ? null : _mapBattlesIcons[battleID];
		}
	}
}
