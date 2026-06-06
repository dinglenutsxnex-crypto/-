using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Nekki;
using SF3;
using SF3.GameModels;
using SF3.Moves;
using UnityEngine;

public class BattleLog : MonoBehaviour
{
	private static BattleLog _instance;

	private static string _filePath = string.Empty;

	private static TextWriter _writer;

	private Model _player;

	private Model _enemy;

	private static int _Lastframe;

	private Model _last;

	private static void InitInstance()
	{
		if (NekkiUtils.IsDebug && !_instance)
		{
			_instance = new GameObject("_battleLog").AddComponent<BattleLog>();
			StaticObjectsManager.AddObject(_instance.gameObject);
		}
	}

	private void Start()
	{
		if (NekkiUtils.IsDebug)
		{
			StartCoroutine(Flush());
		}
	}

	private void OnApplicationQuit()
	{
		if (NekkiUtils.IsDebug && _writer != null)
		{
			_writer.Flush();
			_writer.Close();
			_writer = null;
		}
	}

	public static void Begin(FightInfo fight, Model player, Model enemy)
	{
		if (!CheckWriteDenied())
		{
			_filePath = Path.Combine(GlobalPath.LogsPath, string.Format("BattleLog_{0}.log", NekkiUtils.CurrentTimeAsString));
			EndFight(null);
			_writer = FilesUtil.CreateTextWriter(_filePath);
			_instance._player = player;
			_instance._enemy = enemy;
			Append("----------------------------------------------------------------------------------------------------");
			Append("{0} VS {1}", player.GetAlias(), enemy.GetAlias());
			Append("Battle start: {0}", DateTime.Now);
			Append("Fight info:\n{0}", fight);
			Append("Player attributes:\n{0}", player.modelInfo.attributes.PrintBaseAttributes());
			Append("Player items:\n{0}", player.modelInfo.PrintEquipment());
			Append("Enemy attributes: \n{0}", enemy.modelInfo.attributes.PrintBaseAttributes());
			Append("Enemy items:\n{0}", enemy.modelInfo.PrintEquipment());
		}
	}

	public static void Frame(Model modelState, InfoTrigger actionVal)
	{
		if (!CheckWriteDenied())
		{
			if (_Lastframe != Mathf.RoundToInt(GameTimeController.battleTime))
			{
				_Lastframe = Mathf.RoundToInt(GameTimeController.battleTime);
				Expand(modelState, actionVal, _instance._last != modelState, true);
			}
			else
			{
				Expand(modelState, actionVal, _instance._last != modelState, false);
			}
			_instance._last = modelState;
		}
	}

	public static void Hit(string enemy, string current, float baseDamage, float attackAttribute, string attackAttributeName, float defenseAttribute, string defenceAttributeName, float resutlDamage, bool block, bool crit, string animDef, string animHit)
	{
		if (!CheckWriteDenied())
		{
			char[] array = new char[300];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = '-';
			}
			string text = new string(array);
			Append(string.Empty);
			int num = 50 - (enemy.Length + 4);
			string text2 = string.Format("[{4}] {2} {0} hit {1} {3}", enemy, current, text.Substring(0, num), text.Substring(0, 100 - num - 7 - enemy.Length - current.Length), GetTime());
			Append(text2);
			Append("                                baseDamage       :  {0}", baseDamage);
			Append("                                attackAttribute  :  {0} ({1})", attackAttribute, attackAttributeName);
			Append("                                defenseAttribute :  {0} ({1})", defenseAttribute, defenceAttributeName);
			Append("                                animation def    :  {0}", animDef);
			Append("                                animation hit    :  {0}", animHit);
			Append("                                resutlDamage     :  {0}", Math.Round(resutlDamage, 3));
			Append("                                block            :  {0}", block.ToString());
			Append("                                crit             :  {0}", crit.ToString());
			Append(text.Substring(0, text2.Length));
			Append(string.Empty);
		}
	}

	public static void RoundStart(int currentRound)
	{
		if (!CheckWriteDenied())
		{
			Append("----------------------------------------------------------------------------------------------------");
			Append("                                        ROUND START {0}", currentRound);
			Append("----------------------------------------------------------------------------------------------------");
		}
	}

	public static void RoundEnd(bool isPlayerWin)
	{
		if (!CheckWriteDenied())
		{
			Append("END ROUND: " + ((!isPlayerWin) ? "ENEMY WIN" : "PLAYER WIN"));
		}
	}

	public static void EndFight(bool? isPlayerWin)
	{
		if (!CheckWriteDenied())
		{
			Append("----------------------------------------------------------------------------------------------------");
			if (isPlayerWin.HasValue)
			{
				Append("                                     FIGHT END: " + ((!isPlayerWin.Value) ? "ENEMY WIN" : "PLAYER WIN"));
			}
			else
			{
				Append("                                     FIGHT END: FRENDSHIP");
			}
			Append("----------------------------------------------------------------------------------------------------");
			if (_writer != null)
			{
				_writer.Flush();
				_writer.Close();
				_writer = null;
			}
		}
	}

	public static void UpdateModels(Model player, Model enemy)
	{
		if (!CheckWriteDenied())
		{
			_instance._player = player;
			_instance._enemy = enemy;
		}
	}

	public static void AnimationStart(string playerName, string animationName)
	{
		if (!CheckWriteDenied())
		{
			Append(string.Format("[{0}] ANIMATION START: {1};   PLAYER NAME: {2}", GetTime(), animationName, playerName));
		}
	}

	public static void AddVariable(int ownerId, string variableName, object variableValue, int variableFrames)
	{
		if (!CheckWriteDenied())
		{
			AppendExtended(string.Format("----------  ADD VAIRABLE:  Owner: {0}, Variable: {1}, Value: {2}, Frames: {3}  -----------", GetName(ownerId), variableName, variableValue, variableFrames), true, string.Empty);
		}
	}

	public static void SetVariable(int ownerId, string variableName, object variableValue, int variableFrames)
	{
		if (!CheckWriteDenied())
		{
			AppendExtended(string.Format("----------  SET VAIRABLE:  Owner: {0}, Variable: {1}, Value: {2}, Frames: {3}  -----------", GetName(ownerId), variableName, variableValue, variableFrames), true, string.Empty);
		}
	}

	public static void RemoveVariable(int ownerId, string variableName, object variableValue, int variableFrames)
	{
		if (!CheckWriteDenied())
		{
			AppendExtended(string.Format("----------  REMOVE VAIRABLE:  Owner: {0}, Variable: {1}, Value: {2}, Frames: {3}  -----------", GetName(ownerId), variableName, variableValue, variableFrames), true, string.Empty);
		}
	}

	private IEnumerator Flush()
	{
		while ((bool)base.transform)
		{
			yield return new WaitForSeconds(1f);
			if (_writer != null)
			{
				_writer.Flush();
			}
		}
	}

	private static void AppendExtended(string format, bool addTime = false, string wrapper = null)
	{
		if (addTime)
		{
			format = string.Format("[{0}] {1}", GetTime(), format);
		}
		if (wrapper != null)
		{
			Append(wrapper);
			Append(format);
			Append(wrapper);
		}
		else
		{
			Append(format);
		}
	}

	private static void Append(string format, params object[] args)
	{
		InitInstance();
		if (_writer != null)
		{
			_writer.WriteLine(format, args);
		}
	}

	private static void Expand(Model modelState, InfoTrigger actionVal, bool printName, bool printFrame)
	{
		if (printFrame)
		{
			Append(string.Empty);
		}
		int num = 7;
		int num2 = 30;
		int num3 = 65;
		List<string> list = new List<string>();
		list.Add(actionVal.calledByEvent.ToString());
		List<string> list2 = list;
		char[,] array = new char[110, list2.Count];
		for (int i = 0; i < array.GetLength(0); i++)
		{
			for (int j = 0; j < array.GetLength(1); j++)
			{
				array[i, j] = ' ';
			}
		}
		if (printName)
		{
			string text = ((!string.IsNullOrEmpty(modelState.GetAlias())) ? modelState.GetAlias() : modelState.id.ToString());
			for (int k = 0; k < text.Length; k++)
			{
				if (num + k < array.GetLength(0))
				{
					array[num + k, 0] = text[k];
				}
			}
		}
		if (printFrame)
		{
			string text2 = _Lastframe.ToString();
			for (int l = 0; l < text2.Length; l++)
			{
				if (l < array.GetLength(0))
				{
					array[l, 0] = text2[l];
				}
			}
		}
		string text3 = ((!string.IsNullOrEmpty(actionVal.name)) ? actionVal.name : actionVal.parentName);
		for (int m = 0; m < text3.Length; m++)
		{
			if (num3 + m < array.GetLength(0))
			{
				array[num3 + m, 0] = text3[m];
			}
		}
		for (int n = 0; n < list2.Count; n++)
		{
			for (int num4 = 0; num4 < list2[n].Length; num4++)
			{
				if (num2 + num4 < array.GetLength(0))
				{
					array[num2 + num4, n] = list2[n][num4];
				}
			}
		}
		for (int num5 = 0; num5 < array.GetLength(1); num5++)
		{
			string text4 = string.Empty;
			for (int num6 = 0; num6 < array.GetLength(0); num6++)
			{
				text4 += array[num6, num5];
			}
			Append(text4);
		}
	}

	private static bool CheckWriteDenied()
	{
		return !NekkiUtils.IsDebug;
	}

	private static string GetTime()
	{
		return Time.time.ToString(CultureInfo.InvariantCulture);
	}

	private static string GetName(int ownerId)
	{
		return (_instance._player != null && ownerId == _instance._player.id) ? _instance._player.modelInfo.alias : ((!(_instance._enemy != null) || ownerId != _instance._enemy.id) ? "NONE" : _instance._enemy.modelInfo.alias);
	}
}
