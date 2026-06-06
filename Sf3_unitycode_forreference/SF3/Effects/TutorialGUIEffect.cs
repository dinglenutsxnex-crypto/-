using Nekki.UI;
using SF3.GameModels;

namespace SF3.Effects
{
	public class TutorialGUIEffect : GUIEffect
	{
		private TweenAlpha _tweenAlpha;

		private TweenScale _tweenScale;

		private bool _isPlaying = true;

		protected override void Awake()
		{
			base.Awake();
			_tweenAlpha = GetComponent<TweenAlpha>();
			if (_tweenAlpha != null)
			{
				_tweenAlpha.AddOnFinished(new EventDelegate(delegate
				{
					base.gameObject.SetActive(false);
				}));
				_tweenAlpha.enabled = false;
			}
			_tweenPosition = GetComponent<TweenPosition>();
			if (_tweenPosition != null)
			{
				_tweenPosition.onFinished.Clear();
				_tweenPosition.enabled = false;
			}
			_tweenScale = GetComponent<TweenScale>();
			if (_tweenScale != null)
			{
				_tweenScale.onFinished.Clear();
				_tweenScale.enabled = false;
			}
		}

		public override void Play(Model model, bool leftSide, string alias, string[] values)
		{
			_isPlaying = true;
			base.model = model;
			base.gameObject.SetActive(true);
			SetLabelText(alias, values);
			bool flag = TutorialManager.Instance.IsAllComponentsInvisible(hideComponentIds);
			if (base.name.Contains("Tutorial") && !flag)
			{
				PauseWindow.DecrementShowCounter();
			}
			if (_tweenPosition != null)
			{
				base.transform.localPosition = _tweenPosition.from;
				_tweenPosition.ResetToBeginning();
				_tweenPosition.PlayForward();
			}
			if (_tweenScale != null)
			{
				_tweenScale.ResetToBeginning();
				_tweenScale.PlayForward();
			}
			TutorialManager.Instance.SetVisibleComponents(hideComponentIds, false);
		}

		public override void Disable()
		{
			if (_isPlaying)
			{
				_isPlaying = false;
				bool flag = TutorialManager.Instance.IsAllComponentsVisible(hideComponentIds);
				if (base.name.Contains("Tutorial") && !flag)
				{
					PauseWindow.IncrementShowCounter();
				}
				if (_tweenAlpha == null)
				{
					base.gameObject.SetActive(false);
					return;
				}
				GetComponent<NekkiUILabel>().alpha = _tweenAlpha.from;
				_tweenAlpha.ResetToBeginning();
				_tweenAlpha.PlayForward();
				TutorialManager.Instance.SetVisibleComponents(hideComponentIds, true);
			}
		}
	}
}
