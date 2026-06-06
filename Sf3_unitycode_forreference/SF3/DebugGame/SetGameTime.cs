using SF3.Audio;
using UnityEngine;

namespace SF3.DebugGame
{
	public class SetGameTime : UIModuleHolder
	{
		public static float currentGameTime;

		public UILabel valueLbl;

		public UIScrollBar scrollBar;

		private bool visible = true;

		public GameObject objToDisable;

		public void GameTimeChanged(float value)
		{
			if (GameTimeController.gamePaused)
			{
				currentGameTime = GameTimeController.timeScale;
				scrollBar.value = currentGameTime;
			}
			else
			{
				currentGameTime = value;
				GameTimeController.ChangeGameTime(value);
				AudioManager.Instance.SetPitch(value);
			}
			valueLbl.text = Mathf.RoundToInt(currentGameTime * 100f) + string.Empty;
		}

		public void ShowHide()
		{
			visible = !visible;
			if (visible)
			{
				scrollBar.value = GameTimeController.timeScale;
				valueLbl.text = Mathf.RoundToInt(scrollBar.value * 100f) + string.Empty;
			}
			objToDisable.SetActive(visible);
		}
	}
}
