using Nekki.UI;
using SF3.GameModels;
using UnityEngine;

namespace SF3.Effects
{
	public class GUIEffect : GameEffectBase
	{
		public UIAnchor.Side side;

		public Vector3 localPos;

		public UIAnchor.Side leftAnchor = UIAnchor.Side.TopLeft;

		public UIAnchor.Side rightAnchor = UIAnchor.Side.TopRight;

		public float leftSidePos;

		public float rightSidePos;

		public bool offsetEffectByAspect;

		public bool usesStack;

		public int priorityInStack;

		public bool changePivot;

		public string[] hideComponentIds;

		private UITweener[] _tweeners;

		protected TweenPosition _tweenPosition;

		private bool _leftSide;

		public bool LeftSide
		{
			get
			{
				return _leftSide;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			transf.parent = NekkiUIRoot.Instance.GetAnchor(side);
			transf.localScale = Vector3.one;
			transf.localPosition = localPos;
			_tweeners = GetComponents<UITweener>();
			UITweener longestTweener = GetLongestTweener();
			if (longestTweener != null)
			{
				longestTweener.onFinished.Add(new EventDelegate(delegate
				{
					obj.SetActive(false);
				}));
			}
		}

		private UITweener GetLongestTweener()
		{
			UITweener uITweener = null;
			for (int i = 0; i < _tweeners.Length; i++)
			{
				if (_tweeners[i].tweenGroup == 0)
				{
					if (uITweener == null)
					{
						uITweener = _tweeners[i];
					}
					else if (_tweeners[i].duration + _tweeners[i].delay > uITweener.duration + uITweener.delay)
					{
						uITweener = _tweeners[i];
					}
				}
			}
			return uITweener;
		}

		public void MoveTo(float y)
		{
			Vector3 localPosition = transf.localPosition;
			localPosition.y = y;
			_tweenPosition = TweenPosition.Begin(base.gameObject, 0.2f, localPosition);
		}

		public override void Play(Model model)
		{
			base.model = model;
			obj.SetActive(true);
			UITweener[] tweeners = _tweeners;
			foreach (UITweener uITweener in tweeners)
			{
				if (uITweener.tweenGroup == 0)
				{
					uITweener.ResetToBeginning();
					uITweener.enabled = true;
				}
			}
			TutorialManager.Instance.SetVisibleComponents(hideComponentIds, false);
		}

		public override void Play(Model model, bool leftSide)
		{
			Play(model, leftSide, localPos.y);
		}

		public override void Play(Model model, bool leftSide, string alias, string[] values)
		{
			Play(model, leftSide, localPos.y, alias, values);
		}

		public override void Play(Model model, bool leftSide, float yPos)
		{
			_leftSide = leftSide;
			Vector3 localPosition = localPos;
			localPosition.y = yPos;
			if (changePivot)
			{
				if (_leftSide)
				{
					GetComponent<UIWidget>().pivot = UIWidget.Pivot.Left;
					transf.parent = NekkiUIRoot.Instance.GetAnchor(leftAnchor);
					localPosition.x = leftSidePos;
				}
				else
				{
					GetComponent<UIWidget>().pivot = UIWidget.Pivot.Right;
					transf.parent = NekkiUIRoot.Instance.GetAnchor(rightAnchor);
					localPosition.x = rightSidePos;
				}
			}
			if (offsetEffectByAspect)
			{
				localPosition.x *= ConstantsSF3.GetCurrentAspect;
			}
			transf.localPosition = localPosition;
			Play(model);
		}

		public override void Play(Model model, bool leftSide, float yPos, string alias, string[] values)
		{
			SetLabelText(alias, values);
			Play(model, leftSide, yPos);
		}

		protected void SetLabelText(string alias, string[] values)
		{
			if (alias != null)
			{
				string text = Localization.Get(alias);
				if (values != null && values.Length > 0)
				{
					text = string.Format(text, values);
				}
				GetComponent<NekkiUILabel>().text = text;
			}
		}

		public override void Disable()
		{
			base.Disable();
			TutorialManager.Instance.SetVisibleComponents(hideComponentIds, true);
		}

		private void OnDisable()
		{
			if (_tweenPosition != null)
			{
				_tweenPosition.enabled = false;
			}
			if (usesStack)
			{
				EffectsManager.DeteleEffectFromStack(this);
			}
		}
	}
}
