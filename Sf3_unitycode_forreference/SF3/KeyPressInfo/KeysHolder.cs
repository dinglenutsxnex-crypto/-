using System;
using System.Collections.Generic;

namespace SF3.KeyPressInfo
{
	public class KeysHolder : IDisposable
	{
		private Dictionary<EQuadrants, BattleKey> keyMap;

		private Dictionary<EQuadrants, EQuadrants> invertionMap;

		private Dictionary<EQuadrants, EQuadrants> conflitcingMap;

		public bool inverted { get; private set; }

		public event Action<KeyData> OnKeyPressStateChanged = delegate
		{
		};

		public KeysHolder(Dictionary<EQuadrants, EQuadrants> invertionMap, Dictionary<EQuadrants, EQuadrants> conflitcingMap)
		{
			keyMap = new Dictionary<EQuadrants, BattleKey>();
			this.invertionMap = invertionMap;
			this.conflitcingMap = conflitcingMap;
		}

		public void Initialize()
		{
			foreach (BattleKey value in keyMap.Values)
			{
				value.ResetState();
			}
		}

		public void Update(float gameTimeDelta)
		{
			foreach (BattleKey value in keyMap.Values)
			{
				value.Update(gameTimeDelta);
			}
		}

		public void ResetState(EQuadrants quadrant)
		{
			if (keyMap.ContainsKey(quadrant))
			{
				keyMap[quadrant].ResetState();
			}
		}

		public void KeyDown(EQuadrants code)
		{
			CheckConflicts(code);
			BattleKey battleKey;
			if (keyMap.ContainsKey(code))
			{
				battleKey = keyMap[code];
			}
			else
			{
				battleKey = ((!invertionMap.ContainsKey(code)) ? new BattleKey(code, OnStateChanged) : new BattleKey(code, invertionMap[code], OnStateChanged));
				keyMap[code] = battleKey;
			}
			battleKey.KeyDown();
		}

		public void KeyUp(EQuadrants code)
		{
			CheckConflicts(code);
			if (keyMap.ContainsKey(code))
			{
				keyMap[code].KeyUp();
			}
		}

		public void Invert(bool invert)
		{
			inverted = invert;
			foreach (KeyValuePair<EQuadrants, EQuadrants> item in invertionMap)
			{
				if (keyMap.ContainsKey(item.Key))
				{
					keyMap[item.Key].Invert(invert);
				}
				if (keyMap.ContainsKey(item.Value))
				{
					keyMap[item.Value].Invert(invert);
				}
			}
		}

		public List<KeyData> GetKeys()
		{
			List<KeyData> list = new List<KeyData>();
			foreach (BattleKey value in keyMap.Values)
			{
				if (value.state != KeyPressState.None)
				{
					list.Add(new KeyData(value));
				}
			}
			return list;
		}

		public void Dispose()
		{
			foreach (BattleKey value in keyMap.Values)
			{
				value.Dispose();
			}
		}

		private void CheckConflicts(EQuadrants code)
		{
			if (conflitcingMap.ContainsKey(code) && keyMap.ContainsKey(conflitcingMap[code]))
			{
				keyMap[conflitcingMap[code]].ResetState();
			}
		}

		private void OnStateChanged(BattleKey key)
		{
			if (key.state != KeyPressState.None)
			{
				this.OnKeyPressStateChanged(new KeyData(key));
			}
		}
	}
}
