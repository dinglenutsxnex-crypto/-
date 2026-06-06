using UnityEngine;

namespace SF3.Settings
{
	public class DojoSettings : MonoBehaviour
	{
		[SerializeField]
		private Color _defaultLocationColor = Color.white;

		[SerializeField]
		private Color _locationColorInModule = Color.grey;

		[SerializeField]
		private float _locationColorChangeTime = 1f;

		public Color DefaultLocationColor
		{
			get
			{
				return _defaultLocationColor;
			}
		}

		public Color LocationColorInModule
		{
			get
			{
				return _locationColorInModule;
			}
		}

		public float LocationColorChangeTime
		{
			get
			{
				return _locationColorChangeTime;
			}
		}
	}
}
