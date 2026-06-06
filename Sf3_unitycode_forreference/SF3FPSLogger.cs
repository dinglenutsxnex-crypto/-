using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SF3;
using SF3.BattleUtils;
using SF3.Moves;
using UnityEngine;

public class SF3FPSLogger
{
	private static SF3FPSLogger _logger;

	private List<float> _fpsCounter = new List<float>();

	private bool _counterActive;

	private const float COUNTER_TICK = 0.5f;

	public static SF3FPSLogger instance
	{
		get
		{
			if (_logger == null)
			{
				_logger = new SF3FPSLogger();
			}
			return _logger;
		}
	}

	public void Initialize()
	{
		BattleController.RegisterEventCallback(ETriggerEvents.EVENT_STAGE_CHANGE, StartCounting);
		BattleController.RegisterEventCallback(ETriggerEvents.EVENT_STAGE_CHANGE, FinishCounting);
	}

	private IEnumerator Update()
	{
		while (_counterActive)
		{
			_fpsCounter.Add(SF3BattleUtils.GetFPS());
			yield return new WaitForSeconds(0.5f);
		}
	}

	private void StartCounting(BattleEventArgs args)
	{
		FightController.EFightStage eFightStage = (FightController.EFightStage)args.EventData;
		if (eFightStage == FightController.EFightStage.RoundFightStart)
		{
			_counterActive = true;
			_fpsCounter.Clear();
			Routiner.Go(Update());
		}
	}

	private void FinishCounting(BattleEventArgs args)
	{
		FightController.EFightStage eFightStage = (FightController.EFightStage)args.EventData;
		if (eFightStage == FightController.EFightStage.RoundFightEnd)
		{
			_counterActive = false;
			float num = 0f;
			for (int i = 0; i < _fpsCounter.Count; i++)
			{
				num += _fpsCounter[i];
			}
			num /= (float)_fpsCounter.Count;
			float num2 = 0f;
			for (int j = 0; j < _fpsCounter.Count; j++)
			{
				num2 += Mathf.Pow(_fpsCounter[j] - num, 2f);
			}
			num2 = Mathf.Sqrt(num2 / (float)_fpsCounter.Count);
			JObject jObject = new JObject();
			jObject["battleID"] = BattlesManager.currentBattle.GetID();
			jObject["fightID"] = BattlesManager.currentBattle.GetCurrentFight().fightID;
			jObject["rounds"] = RoundController.Instance.PlayerWinCount;
			jObject["fpsAverageValue"] = num;
			jObject["fpsStandardDeviation"] = num2;
			jObject["subtype"] = "fps_counter";
			Analytics.Logger.AddEvent("CLIENT_FIGHT_END", jObject);
		}
	}
}
