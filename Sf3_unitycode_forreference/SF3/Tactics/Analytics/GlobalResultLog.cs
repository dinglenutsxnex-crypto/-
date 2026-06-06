using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SF3.Tactics.Analytics
{
	public static class GlobalResultLog
	{
		private const string LOG_DIRECTORY = "Assets/Tactics/Log";

		private static List<ResultInfo> resultLog;

		static GlobalResultLog()
		{
			resultLog = new List<ResultInfo>();
			BattleController.Instance.OnApplicationQuitEvent += WriteLogToFile;
		}

		private static void WriteLogToFile()
		{
			if (!Directory.Exists("Assets/Tactics/Log"))
			{
				Directory.CreateDirectory("Assets/Tactics/Log");
			}
			StringBuilder stringBuilder = new StringBuilder("Provocation,Reaction,Distance,StartFrame,ZeroFrame,RealHitFrame,CalcHitFrame,DamageDone,Result,DownFrames,RealRepulsion,CalcRepulsion\n");
			ResultInfo resultInfo = null;
			ResultInfo resultInfo2 = null;
			foreach (ResultInfo item in resultLog)
			{
				resultInfo2 = item;
				if (item.type == ResultType.NotExist)
				{
					resultInfo2 = null;
					if (resultInfo == null || resultInfo.provocation != item.provocation)
					{
						if (resultInfo != null)
						{
							resultInfo2 = resultInfo;
						}
						resultInfo = item;
					}
					else
					{
						resultInfo.downFrames++;
					}
				}
				else if (resultInfo != null)
				{
					stringBuilder.Append(resultInfo.ToCSVRow() + "\n");
					resultInfo = null;
				}
				if (resultInfo2 != null)
				{
					stringBuilder.Append(resultInfo2.ToCSVRow() + "\n");
				}
			}
			string text = DateTime.Now.ToString().Replace('/', '_').Replace(':', '_')
				.Replace(' ', '_');
			File.WriteAllText("Assets/Tactics/Log/" + text + ".csv", stringBuilder.ToString());
		}

		public static void Add(IEnumerable<ResultInfo> logItems)
		{
			resultLog.AddRange(logItems);
		}

		public static void Add(ResultInfo logItem)
		{
			resultLog.Add(logItem);
		}
	}
}
