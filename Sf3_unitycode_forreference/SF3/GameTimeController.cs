using UnityEngine;

namespace SF3
{
	public static class GameTimeController
	{
		private const float DEFAULT_TIME_SCALE = 1f;

		public static float FIXED_DELTA_TIME;

		private static Vector3 _gravity;

		private static float _lastTimeScale;

		private static float _lastSystemTimeScale;

		private static bool _gamePaused;

		private static bool _systemPaused;

		private static bool _enableFlag;

		private static bool _disableFlag;

		public static float gameTimeDelta { get; private set; }

		public static float battleTime { get; private set; }

		public static float unscaledBattleTime { get; private set; }

		public static float deltaTime { get; private set; }

		public static float deltaTimePaused
		{
			get
			{
				return (!gamePaused) ? Time.deltaTime : 0f;
			}
		}

		public static int frameCount { get; private set; }

		public static float time { get; private set; }

		public static float timeScale { get; private set; }

		public static float unscaledTime { get; private set; }

		public static float unscaledDeltaTime { get; private set; }

		public static bool gamePaused
		{
			get
			{
				return _gamePaused || _systemPaused;
			}
		}

		public static bool systemPaused
		{
			get
			{
				return _systemPaused;
			}
		}

		static GameTimeController()
		{
			_gravity = Physics.gravity;
			FIXED_DELTA_TIME = Time.fixedDeltaTime;
			Reset();
		}

		public static void Reset()
		{
			_gamePaused = false;
			_systemPaused = false;
			ResetTimeScale();
			UpdateCache();
		}

		public static void SetPhysicTimeStamp(float stampValue)
		{
			FIXED_DELTA_TIME = stampValue;
			Time.fixedDeltaTime = stampValue;
		}

		public static void UpdateBattleTime()
		{
			if (!_systemPaused)
			{
				UpdateCache();
			}
			if (!gamePaused)
			{
				battleTime += gameTimeDelta;
			}
		}

		private static void UpdateCache()
		{
			timeScale = Time.timeScale;
			deltaTime = Time.deltaTime;
			frameCount++;
			time = Time.time;
			unscaledTime = Time.unscaledTime;
			unscaledDeltaTime = Time.unscaledDeltaTime;
		}

		public static void ResetTimeScale()
		{
			ChangeTimeScale(1f);
			_lastTimeScale = 1f;
			_lastSystemTimeScale = 1f;
		}

		public static void GameTimePause(bool pauseSounds = false)
		{
			if (!gamePaused)
			{
				_lastTimeScale = Time.timeScale;
				_gamePaused = true;
				ChangeGameTime(0f);
			}
		}

		public static void GameTimeResume()
		{
			if (gamePaused)
			{
				_gamePaused = false;
				ChangeGameTime(_lastTimeScale);
			}
		}

		public static void SystemTimePause()
		{
			if (!_systemPaused)
			{
				_systemPaused = true;
				_lastSystemTimeScale = Time.timeScale;
				Time.timeScale = 0f;
				timeScale = 0f;
			}
		}

		public static void SystemTimeResume()
		{
			if (_systemPaused)
			{
				_systemPaused = false;
				timeScale = _lastSystemTimeScale;
				Time.timeScale = _lastSystemTimeScale;
			}
		}

		public static void ChangeGameTime(float newTimeDelta)
		{
			ChangeTimeScale(newTimeDelta);
		}

		private static void FixClothes(float delta)
		{
			if (!_enableFlag && (double)delta < 0.1)
			{
				_enableFlag = true;
				_disableFlag = true;
				Cloth[] array = (Cloth[])Object.FindObjectsOfType(typeof(Cloth));
				Cloth[] array2 = array;
				foreach (Cloth cloth in array2)
				{
					cloth.useGravity = false;
				}
			}
			if (_disableFlag && (double)delta > 0.1)
			{
				Cloth[] array3 = (Cloth[])Object.FindObjectsOfType(typeof(Cloth));
				Cloth[] array4 = array3;
				foreach (Cloth cloth2 in array4)
				{
					cloth2.useGravity = true;
				}
				_disableFlag = false;
				_enableFlag = false;
			}
		}

		private static void ChangeTimeScale(float newTimeDelta)
		{
			float num4 = (gameTimeDelta = (Time.timeScale = (timeScale = Mathf.Clamp(newTimeDelta, 0f, 1f))));
			FixClothes(num4);
			if ((double)Time.timeScale > 0.01)
			{
				Time.fixedDeltaTime = FIXED_DELTA_TIME * num4;
				Physics.gravity = _gravity / num4;
			}
			else
			{
				Time.fixedDeltaTime = FIXED_DELTA_TIME * 0.01f;
				Physics.gravity = _gravity / 0.01f;
			}
		}
	}
}
