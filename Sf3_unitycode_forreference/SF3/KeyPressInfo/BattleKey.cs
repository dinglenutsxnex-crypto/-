using System;
using System.Globalization;
using SF3.Moves;
using SF3.Settings;

namespace SF3.KeyPressInfo
{
	public class BattleKey : IDisposable
	{
		private static float FRAMES_TO_HOLDING;

		private static float FRAMES_TO_CLEARING;

		private KeyPressState _state = KeyPressState.None;

		private EQuadrants invertedCode;

		private Action<BattleKey> onStateChanged = delegate
		{
		};

		public float pressDuration { get; private set; }

		public EQuadrants code { get; private set; }

		public EQuadrants battleCode { get; private set; }

		public KeyPressState state
		{
			get
			{
				return _state;
			}
			private set
			{
				if (_state != value)
				{
					_state = value;
					onStateChanged(this);
				}
			}
		}

		public int clicks { get; private set; }

		public bool inverted { get; private set; }

		public BattleKey(EQuadrants code, EQuadrants invertedCode, Action<BattleKey> onStateChanged)
		{
			this.code = code;
			battleCode = code;
			this.invertedCode = invertedCode;
			this.onStateChanged = (Action<BattleKey>)Delegate.Combine(this.onStateChanged, onStateChanged);
		}

		public BattleKey(EQuadrants code, Action<BattleKey> onStateChanged)
			: this(code, code, onStateChanged)
		{
		}

		public static void Init()
		{
			FRAMES_TO_HOLDING = float.Parse(FightSettings.GetEventProperty(ETriggerEvents.EVENT_KEY_PRESSED, "HoldFrames"), CultureInfo.InvariantCulture);
			FRAMES_TO_CLEARING = float.Parse(FightSettings.GetEventProperty(ETriggerEvents.EVENT_KEY_PRESSED, "ClearFrames"), CultureInfo.InvariantCulture);
		}

		public void Update(float gameTimeDelta)
		{
			if (state == KeyPressState.None)
			{
				return;
			}
			pressDuration += gameTimeDelta;
			if (state == KeyPressState.Down)
			{
				if (pressDuration >= FRAMES_TO_HOLDING)
				{
					state = KeyPressState.Hold;
					pressDuration = 0f;
				}
			}
			else if ((state == KeyPressState.Up || state == KeyPressState.UnHold) && pressDuration >= FRAMES_TO_CLEARING)
			{
				state = KeyPressState.None;
				clicks = 0;
				pressDuration = 0f;
			}
		}

		public void KeyDown()
		{
			if (state == KeyPressState.None)
			{
				clicks = 1;
			}
			else
			{
				clicks++;
			}
			state = KeyPressState.Down;
			pressDuration = 0f;
		}

		public void KeyUp()
		{
			if (state == KeyPressState.Down)
			{
				state = KeyPressState.Up;
			}
			else
			{
				state = KeyPressState.UnHold;
			}
			pressDuration = 0f;
		}

		public void Invert(bool invert)
		{
			if (invert)
			{
				battleCode = invertedCode;
			}
			else
			{
				battleCode = code;
			}
			inverted = invert;
		}

		public void ResetState()
		{
			state = KeyPressState.None;
		}

		public void Dispose()
		{
			onStateChanged = delegate
			{
			};
			state = KeyPressState.None;
		}
	}
}
