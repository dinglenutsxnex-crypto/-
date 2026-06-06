using System;
using System.Collections.Generic;
using System.Linq;
using SF3;
using SF3.UserData;

public class MissionsHolder
{
	public delegate void BoolDelegate(bool value);

	public delegate void TimeSpanDelegate(TimeSpan value);

	public class MissionTimerData
	{
		public long from { get; private set; }

		public long to { get; private set; }

		public IBattleInfo battle { get; private set; }

		public MissionTimerData(IBattleInfo battle, long from, long to)
		{
			this.battle = battle;
			this.from = from;
			this.to = to;
		}

		public TimeSpan TimeLeftToNextBattle(long now)
		{
			return TimeSpan.FromMilliseconds(to - now);
		}
	}

	private readonly List<IBattleInfo> _missions = new List<IBattleInfo>();

	private readonly List<IBattleInfo> _availableMissions = new List<IBattleInfo>();

	private readonly TimeSpan _countdown = TimeSpan.FromMilliseconds(JS.Instance.GetNumberConstant("battleMissionCooldownTime"));

	private readonly int _missionsMax = JS.Instance.GetIntegerConstant("battleMissionSlotCount");

	private MissionTimerData _nextToUpdate;

	public static MissionsHolder instance { get; private set; }

	public event BoolDelegate OnActivateTimer;

	public event TimeSpanDelegate OnTimerUpdate;

	public MissionsHolder()
	{
		instance = this;
		BattlesManager.OnBattlesUpdate += BattlesUpdateEventHandler;
		CheckMissions();
		BattlesUpdateEventHandler();
	}

	private void BattlesUpdateEventHandler()
	{
		_missions.Clear();
		_missions.AddRange(BattlesManager.instance.GetMissions());
		_availableMissions.Clear();
		foreach (IBattleInfo item in _missions.Where((IBattleInfo mission) => !mission.GetIsCompleted()))
		{
			_availableMissions.Add(item);
		}
		CalcNextToUpdate();
		UpdateTimers();
	}

	private void CalcNextToUpdate()
	{
		if (_availableMissions.Count < _missionsMax && _missions.Count == _missionsMax)
		{
			IBattleInfo battle;
			DateTime nextUpdateTime = GetNextUpdateTime(out battle);
			if (nextUpdateTime > DateTime.MinValue)
			{
				_nextToUpdate = new MissionTimerData(battle, nextUpdateTime.Subtract(_countdown).GetUnixTimeStampMilliseconds(), nextUpdateTime.GetUnixTimeStampMilliseconds());
			}
			else
			{
				_nextToUpdate = null;
			}
		}
		else
		{
			_nextToUpdate = null;
		}
	}

	private DateTime GetNextUpdateTime(out IBattleInfo battle)
	{
		battle = null;
		if (_missionsMax == _missions.Count && _missions.Count > 0)
		{
			IEnumerable<IBattleInfo> enumerable = _missions.Where((IBattleInfo info) => info.GetIsCompleted());
			IList<IBattleInfo> list = (enumerable as IList<IBattleInfo>) ?? enumerable.ToList();
			if (list.Any())
			{
				battle = list.MinBy((IBattleInfo info) => info.GetFinishTime());
			}
			if (battle != null)
			{
				IBattleInfo battleInfo = _missions.MaxBy((IBattleInfo info) => info.GetGenerationTime());
				if (battle.GetFinishTime() > battleInfo.GetGenerationTime())
				{
					return battle.GetFinishTime() + _countdown;
				}
				return battleInfo.GetGenerationTime() + _countdown;
			}
		}
		return DateTime.MinValue;
	}

	private void CheckMissions()
	{
		IBattleInfo battle;
		if (_missionsMax <= _missions.Count && GetNextUpdateTime(out battle) < NetworkConnection.current.getCurrentServerDateTime() && battle != null)
		{
			RefreshBattlesProcessing.RefreshBattles();
			_nextToUpdate = null;
		}
	}

	public void UpdateTimers()
	{
		if (_nextToUpdate == null)
		{
			ActivateTimer(false);
			return;
		}
		long unixTimeStampMilliseconds = NetworkConnection.current.getCurrentServerDateTime().GetUnixTimeStampMilliseconds();
		TimeSpan timeSpan = _nextToUpdate.TimeLeftToNextBattle(unixTimeStampMilliseconds);
		if (timeSpan > TimeSpan.Zero)
		{
			ActivateTimer(true);
			UpdateTimer(timeSpan);
		}
		else
		{
			ActivateTimer(false);
			instance.CheckMissions();
		}
	}

	private void ActivateTimer(bool active)
	{
		if (this.OnActivateTimer != null)
		{
			this.OnActivateTimer(active);
		}
	}

	private void UpdateTimer(TimeSpan timeToBattle)
	{
		if (this.OnTimerUpdate != null)
		{
			this.OnTimerUpdate(timeToBattle);
		}
	}
}
