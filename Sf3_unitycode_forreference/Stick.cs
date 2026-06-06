using System;
using Nekki;
using UnityEngine;

public class Stick : UIModuleHolder
{
	[Serializable]
	public class ControlPoint
	{
		[SerializeField]
		private EDirections _direction;

		[SerializeField]
		private Transform _t;

		private bool _posObtained;

		private Vector3 _pos;

		public Vector3 Position
		{
			get
			{
				if (!_posObtained)
				{
					_pos = _t.position;
					_posObtained = true;
				}
				return _pos;
			}
		}

		public EDirections Direction
		{
			get
			{
				return _direction;
			}
		}
	}

	[Serializable]
	public class StickUnit
	{
		[SerializeField]
		public string Name;

		[SerializeField]
		public EQuadrants Quadrant;

		[SerializeField]
		public UISprite Normal;

		[SerializeField]
		public UISprite Active;

		[SerializeField]
		public GameObject TutorUnit;

		[SerializeField]
		public GameObject ShadowHintUnit;

		public bool Tutor { get; private set; }

		public void SetTutor(EQuadrants[] array, bool active)
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
					SetState(active);
				}
			}
		}

		private void SetState(bool state)
		{
			Tutor = state;
			Normal.depth = ((!state) ? 2 : 100);
			Normal.enabled = !state;
			Active.depth = ((!state) ? 2 : 100);
			Active.enabled = !state;
			TutorUnit.SetActive(state);
		}

		private bool IsStickQuadrant(EQuadrants quadrant)
		{
			switch (quadrant)
			{
			case EQuadrants.Zero:
			case EQuadrants.One:
			case EQuadrants.Two:
			case EQuadrants.Three:
			case EQuadrants.Four:
			case EQuadrants.Five:
			case EQuadrants.Six:
			case EQuadrants.Seven:
			case EQuadrants.Eight:
				return true;
			default:
				return false;
			}
		}
	}

	[SerializeField]
	private UISprite Highlight;

	[SerializeField]
	private UISprite ContainerNorm;

	[SerializeField]
	private UISprite ContainerAction;

	[SerializeField]
	private UISprite ContainerTutor;

	[SerializeField]
	private GameObject ContainerShadowHints;

	[SerializeField]
	private UISprite StickNorm;

	[SerializeField]
	private UISprite StickAction;

	[SerializeField]
	private GameObject StickTutor;

	public StickUnit[] StickUnits;

	[SerializeField]
	private ControlPoint[] ControlPoints;

	public const int onStickBegan = 0;

	public const int onStickChange = 1;

	public const int onStickEnd = 2;

	private static Stick _instance;

	private static Vector3 _sheduledScale = Vector3.one;

	private bool informStickStart;

	private static EDirections _lastDirection = EDirections.Idle;

	public static Stick Instance
	{
		get
		{
			return _instance;
		}
	}

	public static EDirections Direction
	{
		get
		{
			return _lastDirection;
		}
		set
		{
			if (_lastDirection != value)
			{
				_instance.callEvent(2, new AbstractController.Key(KeyCode.None, _lastDirection.GetQuadrant(), 1));
				_lastDirection = value;
				_instance.callEvent(1, new AbstractController.Key(KeyCode.None, Direction.GetQuadrant(), 1));
			}
		}
	}

	public static EQuadrants Quadrant
	{
		get
		{
			return _lastDirection.GetQuadrant();
		}
	}

	public static void SetScale(float scale)
	{
		if ((bool)_instance)
		{
			_instance.transform.localScale = new Vector3(scale, scale, 1f);
		}
		else
		{
			_sheduledScale = new Vector3(scale, scale, 1f);
		}
	}

	internal void Start()
	{
		_instance = this;
		SetTutor(null);
		base.transform.localScale = _sheduledScale;
		GamepadController.Instance.SubscribeUIElement(base.gameObject);
		if (NekkiUtils.IsPhone())
		{
			SetScale(1.3f);
		}
	}

	public static void OpenInterface()
	{
		_instance.callEvent(2, new AbstractController.Key(KeyCode.None, _lastDirection.GetQuadrant(), 1));
	}

	public void DragProcess(Transform stick, bool inSafe)
	{
		if (inSafe)
		{
			Direction = EDirections.Idle;
			return;
		}
		if (!informStickStart)
		{
			informStickStart = true;
			callEvent(2, new AbstractController.Key(KeyCode.None, EDirections.Idle.GetQuadrant(), 1));
		}
		Vector3 position = stick.position;
		ControlPoint controlPoint = null;
		float num = float.MaxValue;
		for (int i = 0; i < ControlPoints.Length; i++)
		{
			if (controlPoint == null)
			{
				controlPoint = ControlPoints[i];
				num = Vector3.Distance(position, controlPoint.Position);
			}
			else if (Vector3.Distance(position, ControlPoints[i].Position) < num)
			{
				controlPoint = ControlPoints[i];
				num = Vector3.Distance(position, controlPoint.Position);
			}
		}
		if (controlPoint != null)
		{
			Direction = controlPoint.Direction;
		}
	}

	public void Release()
	{
		if (informStickStart)
		{
			callEvent(2, new AbstractController.Key(KeyCode.None, Direction.GetQuadrant(), 1));
			informStickStart = false;
		}
		Direction = EDirections.Idle;
	}

	public static void SetActive(bool value)
	{
		if ((bool)_instance)
		{
			_instance.gameObject.SetActive(value);
		}
	}

	public void SetTutor(EQuadrants[] buttons, bool active = true)
	{
		ContainerTutor.enabled = false;
		StickTutor.SetActive(false);
		StickUnit[] stickUnits = StickUnits;
		foreach (StickUnit stickUnit in stickUnits)
		{
			stickUnit.SetTutor(buttons, active);
			if (stickUnit.Tutor)
			{
				ContainerTutor.enabled = true;
				StickTutor.SetActive(true);
			}
		}
	}

	public void SetShadowHint(EQuadrants slot, bool active)
	{
		StickUnit[] stickUnits = StickUnits;
		foreach (StickUnit stickUnit in stickUnits)
		{
			if (stickUnit.Quadrant == slot && stickUnit.ShadowHintUnit != null)
			{
				stickUnit.ShadowHintUnit.SetActive(active);
				stickUnit.Normal.enabled = !active;
				stickUnit.Active.enabled = !active;
			}
		}
	}
}
