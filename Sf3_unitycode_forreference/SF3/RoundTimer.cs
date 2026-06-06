using UnityEngine;
using UnityEngine.UI;

namespace SF3
{
	public class RoundTimer : UIModuleHolder
	{
		[SerializeField]
		private UnityEngine.UI.Text _timerLabel;

		public void UpdateLabel(string time)
		{
			_timerLabel.text = time;
		}
	}
}
