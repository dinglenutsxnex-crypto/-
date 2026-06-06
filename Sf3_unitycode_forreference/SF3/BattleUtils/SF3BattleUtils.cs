using UnityEngine;

namespace SF3.BattleUtils
{
	public class SF3BattleUtils : MonoBehaviour
	{
		private static SF3BattleUtils _instance;

		private SF3FPSUtil _fpsUtil;

		public static SF3BattleUtils instance
		{
			get
			{
				if (_instance == null)
				{
					GameObject gameObject = new GameObject("battle_utils");
					_instance = gameObject.AddComponent<SF3BattleUtils>();
				}
				return _instance;
			}
		}

		public static void Initialize()
		{
			SF3BattleUtils sF3BattleUtils = instance;
		}

		private void Awake()
		{
			_fpsUtil = new SF3FPSUtil();
		}

		private void Update()
		{
			_fpsUtil.Update();
		}

		public static float GetFPS()
		{
			return instance._fpsUtil.GetFPS();
		}

		public static int GetPing()
		{
			return NetworkConnection.current.GetPing();
		}
	}
}
