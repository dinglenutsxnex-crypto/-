namespace SF3.BattleUtils
{
	public class SF3FPSUtil
	{
		private const float COUNTER_TICK = 0.5f;

		private int _frameCounter;

		private float _timeCounter;

		private float _lastFramerate;

		public void Update()
		{
			if (_timeCounter < 0.5f)
			{
				_timeCounter += GameTimeController.deltaTime;
				_frameCounter++;
				if (_timeCounter >= 0.5f)
				{
					_lastFramerate = (float)_frameCounter / _timeCounter;
					_frameCounter = 0;
					_timeCounter = 0f;
				}
			}
		}

		public float GetFPS()
		{
			return _lastFramerate;
		}
	}
}
