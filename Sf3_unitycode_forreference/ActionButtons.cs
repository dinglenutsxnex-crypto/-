using System;
using System.Collections.Generic;
using System.Linq;
using Nekki;
using SF3;
using UnityEngine;

public class ActionButtons : UIModuleHolder
{
	[Serializable]
	public class ActionUnit
	{
		[SerializeField]
		public string Name;

		[SerializeField]
		public EQuadrants Quadrant;

		[SerializeField]
		public ActionButton AButton;

		[SerializeField]
		public UISprite Sprite;

		[SerializeField]
		public UIButton Button;

		[SerializeField]
		public UISprite TutorTween;

		private float _baseAlpha;

		public bool Tutor { get; private set; }

		public bool Active
		{
			get
			{
				return AButton.gameObject.activeSelf;
			}
			set
			{
				AButton.gameObject.SetActive(value);
				TutorTween.gameObject.SetActive(value);
			}
		}

		public void Init()
		{
			_baseAlpha = Sprite.alpha;
		}

		public void SetTutor(EQuadrants[] array, bool activate)
		{
			if (array == null)
			{
				SetState(false);
				return;
			}
			foreach (EQuadrants eQuadrants in array)
			{
				if (eQuadrants == Quadrant)
				{
					SetState(activate);
					break;
				}
			}
		}

		private void SetState(bool state)
		{
			Color defaultColor = Button.defaultColor;
			Tutor = state;
			Sprite.depth = ((!state) ? 2 : 100);
			Sprite.alpha = ((!state) ? _baseAlpha : 0.9f);
			TutorTween.enabled = state;
			TutorTween.gameObject.SetActive(state);
			defaultColor.a = ((!state) ? _baseAlpha : 0.9f);
			Button.defaultColor = defaultColor;
		}
	}

	public const int OnButtonPress = 0;

	public const int OnButtonRelease = 1;

	private bool _active;

	public UISpriteAnimationAdvanced shadowEnergyActiveEffect;

	public GameObject shadowEnergyRotator;

	public ActionUnit[] ActionUnits;

	private static Vector3 _sheduledScale = Vector3.one;

	private bool _tutorActive;

	public static ActionButtons Instance { get; private set; }

	public void InitializeButtons()
	{
		StopShadowFull();
	}

	public static void SetScale(float scale)
	{
		if ((bool)Instance)
		{
			Instance.transform.localScale = new Vector3(scale, scale, 1f);
		}
		else
		{
			_sheduledScale = new Vector3(scale, scale, 1f);
		}
	}

	public List<UISprite> SetTutor(EQuadrants[] buttons, bool active = true)
	{
		ActionUnit[] actionUnits = ActionUnits;
		foreach (ActionUnit actionUnit in actionUnits)
		{
			actionUnit.SetTutor(buttons, active);
		}
		List<UISprite> list = new List<UISprite>();
		ActionUnit[] actionUnits2 = ActionUnits;
		foreach (ActionUnit actionUnit2 in actionUnits2)
		{
			list.Add(actionUnit2.TutorTween);
		}
		return list;
	}

	protected override void Awake()
	{
		base.Awake();
		Instance = this;
	}

	internal void Start()
	{
		ActionUnit[] actionUnits = ActionUnits;
		foreach (ActionUnit actionUnit in actionUnits)
		{
			actionUnit.Init();
		}
		SetTutor(null);
		base.transform.localScale = _sheduledScale;
		shadowEnergyActiveEffect.gameObject.SetActive(false);
		shadowEnergyRotator.SetActive(false);
		ActionUnit[] actionUnits2 = ActionUnits;
		foreach (ActionUnit actionUnit2 in actionUnits2)
		{
			switch (actionUnit2.Quadrant)
			{
			case EQuadrants.Punch:
			case EQuadrants.Kick:
			case EQuadrants.Missile:
			case EQuadrants.Magic:
				actionUnit2.AButton.Pressed += GetButtonAction(actionUnit2.Quadrant);
				break;
			default:
				Messenger.Error(string.Format("ActionUnit [{0}] is not supported.", actionUnit2.Quadrant.ToString()));
				break;
			}
		}
		GamepadController.Instance.SubscribeUIElement(base.gameObject);
		if (NekkiUtils.IsPhone())
		{
			SetScale(1.3f);
		}
	}

	public ActionUnit GetActionButtonByName(string name)
	{
		ActionUnit actionUnit = ActionUnits.FirstOrDefault((ActionUnit button) => button.Name.Equals(name));
		if (actionUnit == null)
		{
			Debug.LogError(string.Format("No button by name [{0}] found.", name));
		}
		return actionUnit;
	}

	public void ActionButtonHide(EQuadrants key, bool hide)
	{
		switch (key)
		{
		case EQuadrants.Punch:
			PunchButtonEnable(!hide);
			break;
		case EQuadrants.Kick:
			KickButtonEnable(!hide);
			break;
		case EQuadrants.Missile:
			MissileButtonEnable(!hide);
			break;
		case EQuadrants.Magic:
			ShadowButtonEnable(!hide);
			break;
		}
	}

	public void MissileButtonEnable(bool isEnable)
	{
		ActionButtonsEnable(isEnable, "Missile");
	}

	public void ShadowButtonEnable(bool isEnable)
	{
		ActionButtonsEnable(isEnable, "Magic");
	}

	public void KickButtonEnable(bool isEnable)
	{
		ActionButtonsEnable(isEnable, "Kick");
	}

	public void PunchButtonEnable(bool isEnable)
	{
		ActionButtonsEnable(isEnable, "Punch");
	}

	public void ActionButtonsEnable(bool isEnable)
	{
		ActionButtonsEnable(isEnable, "Missile", "Magic", "Kick", "Punch");
	}

	private void ActionButtonsEnable(bool isEnable, params string[] buttonNames)
	{
		foreach (string buttonName in buttonNames)
		{
			ActionUnit actionUnit = ActionUnits.FirstOrDefault((ActionUnit unit) => unit.Name.Equals(buttonName));
			if (actionUnit != null)
			{
				actionUnit.AButton.gameObject.SetActive(isEnable);
			}
		}
	}

	public static void PlayShadowFull()
	{
		if ((bool)Instance)
		{
			Instance.shadowEnergyActiveEffect.gameObject.SetActive(true);
			Instance.shadowEnergyActiveEffect.ResetToBeginning();
		}
	}

	public void StopShadowFull()
	{
		shadowEnergyActiveEffect.gameObject.SetActive(false);
	}

	private ActionButton.ActionButtonPressed GetButtonAction(EQuadrants quadrant)
	{
		return delegate(bool isPressed)
		{
			if (_active)
			{
				callEvent((!isPressed) ? 1 : 0, new AbstractController.Key(KeyCode.None, quadrant, 1));
			}
		};
	}

	public static void SetActive(bool active)
	{
		Instance._active = active;
		Instance.gameObject.SetActive(active);
		ActionUnit[] actionUnits = Instance.ActionUnits;
		foreach (ActionUnit actionUnit in actionUnits)
		{
			actionUnit.Active = active;
		}
	}

	public static bool QuadrantPressed(EQuadrants quadrant)
	{
		ActionUnit[] actionUnits = Instance.ActionUnits;
		foreach (ActionUnit actionUnit in actionUnits)
		{
			if (actionUnit.Tutor && actionUnit.Quadrant == quadrant)
			{
				return true;
			}
		}
		return false;
	}
}
