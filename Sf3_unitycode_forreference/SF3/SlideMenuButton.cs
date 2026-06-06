using UnityEngine;

namespace SF3
{
	[RequireComponent(typeof(CanvasGroup))]
	public class SlideMenuButton : MonoBehaviour
	{
		private CanvasGroup _canvasGroup;

		[SerializeField]
		private float _onSelectAlpha = 1f;

		private float _regularAlpha;

		private void Awake()
		{
			_canvasGroup = GetComponent<CanvasGroup>();
			_regularAlpha = _canvasGroup.alpha;
		}

		public void Select()
		{
			_canvasGroup.alpha = _onSelectAlpha;
		}

		public void Deselect()
		{
			_canvasGroup.alpha = _regularAlpha;
		}
	}
}
