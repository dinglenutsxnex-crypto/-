using System;

namespace SF3
{
	public static class SF3Utils
	{
		public static bool IsFinishPrePlayMode;

		public static EPlayerType GetPlayerTypeByName(string name)
		{
			name = name.ToLower();
			EPlayerType result = EPlayerType.None;
			switch (name)
			{
			case "me":
				result = EPlayerType.This;
				break;
			case "enemy":
				result = EPlayerType.Enemy;
				break;
			case "parent":
				result = EPlayerType.Parent;
				break;
			case "both":
				result = EPlayerType.Both;
				break;
			case "child":
				result = EPlayerType.Child;
				break;
			}
			return result;
		}

		public static ESurfaceType GetSurfaceTypeByName(string name)
		{
			name = name.ToLower();
			ESurfaceType result = ESurfaceType.None;
			switch (name)
			{
			case "ground":
				result = ESurfaceType.Ground;
				break;
			case "metal":
				result = ESurfaceType.Metal;
				break;
			case "stone":
				result = ESurfaceType.Stone;
				break;
			case "wood":
				result = ESurfaceType.Wood;
				break;
			}
			return result;
		}

		public static bool TryParseEnum<T>(out T outParam, string key, T defaultValue) where T : struct
		{
			bool result = false;
			try
			{
				outParam = (T)Enum.Parse(typeof(T), key, true);
				result = true;
			}
			catch (Exception)
			{
				outParam = defaultValue;
			}
			return result;
		}

		public static bool TryParseBattleIdentifier(out int[] result, string id)
		{
			result = new int[2];
			if (id.IsNullOrEmpty())
			{
				return false;
			}
			string[] array = id.Split('.');
			if (array.Length >= 2)
			{
				result[0] = int.Parse(array[0]);
				result[1] = GetFightIdToUseInSolution(int.Parse(array[1]));
				return true;
			}
			return false;
		}

		private static int GetFightIdToUseInSolution(int fightIdUsedInYaml)
		{
			return fightIdUsedInYaml - 1;
		}

		public static int GetFightIdtoUseInYaml(int fightIdUsedInSolution)
		{
			return fightIdUsedInSolution + 1;
		}

		public static int SecondsToFrames(float seconds, int targetFramerate = 60)
		{
			return (int)(seconds * (float)targetFramerate);
		}

		public static int SecondsToFrames(int seconds, int targetFramerate = 60)
		{
			return seconds * targetFramerate;
		}

		public static float FramesToSeconds(int frames, int targetFramerate = 60)
		{
			return frames / targetFramerate;
		}
	}
}
