using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SF3.KeyPressInfo
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct KeyData
	{
		private const float UNDEFINED_PRESS_DURATION = -1f;

		private static Dictionary<string, KeyPressState> stateCompliance = new Dictionary<string, KeyPressState>
		{
			{
				"Hold",
				KeyPressState.Hold
			},
			{
				"Tap",
				KeyPressState.Down
			},
			{
				"Release",
				KeyPressState.Up
			},
			{
				"Down",
				KeyPressState.Down
			},
			{
				"UnHold",
				KeyPressState.UnHold
			},
			{
				"Unhold",
				KeyPressState.UnHold
			},
			{
				"Up",
				KeyPressState.Up
			},
			{
				"Any",
				KeyPressState.Any
			},
			{
				"ANY",
				KeyPressState.Any
			},
			{
				string.Empty,
				KeyPressState.Any
			}
		};

		private static Dictionary<string, EQuadrants> codeComplince = new Dictionary<string, EQuadrants>
		{
			{
				"Up",
				EQuadrants.One
			},
			{
				"Up_Forward",
				EQuadrants.Two
			},
			{
				"Forward",
				EQuadrants.Three
			},
			{
				"Down_Forward",
				EQuadrants.Four
			},
			{
				"Down",
				EQuadrants.Five
			},
			{
				"Down_Back",
				EQuadrants.Six
			},
			{
				"Back",
				EQuadrants.Seven
			},
			{
				"Up_Back",
				EQuadrants.Eight
			},
			{
				"Punch",
				EQuadrants.Punch
			},
			{
				"Kick",
				EQuadrants.Kick
			},
			{
				"Missile",
				EQuadrants.Missile
			},
			{
				"Shadow",
				EQuadrants.Magic
			},
			{
				"TEST1",
				EQuadrants.TEST1
			},
			{
				"TEST2",
				EQuadrants.TEST2
			},
			{
				"Any",
				EQuadrants.Any
			},
			{
				"ANY",
				EQuadrants.Any
			},
			{
				"None",
				EQuadrants.None
			}
		};

		public EQuadrants battleCode { get; private set; }

		public KeyPressState pressState { get; private set; }

		public float pressDuration { get; private set; }

		public KeyData(BattleKey battleKey)
		{
			battleCode = battleKey.battleCode;
			pressState = battleKey.state;
			pressDuration = battleKey.pressDuration;
		}

		public KeyData(string battleCodeName)
		{
			battleCode = GetBattleCodeByName(battleCodeName);
			pressState = KeyPressState.Any;
			pressDuration = -1f;
		}

		public KeyData(string battleCodeName, string pressStateName)
		{
			battleCode = GetBattleCodeByName(battleCodeName);
			pressState = GetKeyPressStatebyName(pressStateName);
			pressDuration = -1f;
		}

		public KeyData(EQuadrants battleCode, IEnumerable<string> pressStateNames)
		{
			this.battleCode = battleCode;
			pressState = KeyPressState.Undefined;
			foreach (string pressStateName in pressStateNames)
			{
				pressState |= GetKeyPressStatebyName(pressStateName);
			}
			pressDuration = -1f;
		}

		public KeyData(string battleCodeName, IEnumerable<string> pressStateNames)
		{
			battleCode = GetBattleCodeByName(battleCodeName);
			pressState = KeyPressState.Undefined;
			foreach (string pressStateName in pressStateNames)
			{
				pressState |= GetKeyPressStatebyName(pressStateName);
			}
			pressDuration = -1f;
		}

		public KeyData(EQuadrants battleCode, KeyPressState pressState)
		{
			this.battleCode = battleCode;
			this.pressState = pressState;
			pressDuration = -1f;
		}

		public static KeyPressState GetKeyPressStatebyName(string pressStateName)
		{
			if (stateCompliance.ContainsKey(pressStateName))
			{
				return stateCompliance[pressStateName];
			}
			throw new Exception("PressState with name " + pressStateName + " is invalid ");
		}

		public static EQuadrants GetBattleCodeByName(string battleCodeName)
		{
			if (codeComplince.ContainsKey(battleCodeName))
			{
				return codeComplince[battleCodeName];
			}
			throw new Exception("BattleCode with name " + battleCodeName + " is invalid ");
		}

		public bool IncludedInConditionData(KeyData keyData)
		{
			return pressState == KeyPressState.Ultimate || ((int)(keyData.pressState & pressState) > 0 && (keyData.battleCode == EQuadrants.Any || battleCode == keyData.battleCode));
		}

		public override string ToString()
		{
			return string.Concat(battleCode, "[", pressState, (pressDuration != -1f) ? ("(" + pressDuration + ")") : string.Empty, "]");
		}
	}
}
