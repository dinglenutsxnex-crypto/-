using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SF3.Moves;

namespace SF3
{
	[Serializable]
	public class GameVariables
	{
		[Serializable]
		public class LocalVariable
		{
			public string name;

			private int _frames;

			private float timeDelta;

			public object value { get; private set; }

			public int frames
			{
				get
				{
					return _frames;
				}
				set
				{
					if (value < -1)
					{
						value = -1;
					}
					_frames = value;
				}
			}

			public LocalVariable(string name, object value, int frames = -1)
			{
				this.value = value;
				this.frames = frames;
				timeDelta = 1f;
				this.name = name;
			}

			public bool Update()
			{
				if (frames == -1)
				{
					return false;
				}
				timeDelta -= GameTimeController.timeScale;
				if (timeDelta <= 0f)
				{
					timeDelta = 1f;
					frames--;
				}
				if (frames <= 0)
				{
					return true;
				}
				return false;
			}

			public void SetValue(object newValue)
			{
				value = newValue;
			}

			public void SetFrames(int newFrames)
			{
				frames = newFrames;
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("Variable name: [{0}], Value: [{1}], Frames: [{2}]", name, value.ToString(), frames.ToString());
				return stringBuilder.ToString();
			}
		}

		public delegate void VariableChangedHandler(int ownerId, string variableName);

		private static readonly Dictionary<int, Dictionary<string, List<VariableChangedHandler>>> VariablesToCheck;

		private static readonly Dictionary<int, Dictionary<string, LocalVariable>> InGameVariables;

		static GameVariables()
		{
			InGameVariables = new Dictionary<int, Dictionary<string, LocalVariable>>();
			VariablesToCheck = new Dictionary<int, Dictionary<string, List<VariableChangedHandler>>>();
		}

		public static void ClearGameVariables()
		{
			InGameVariables.Clear();
		}

		public static void Update()
		{
			foreach (KeyValuePair<int, Dictionary<string, LocalVariable>> inGameVariable in InGameVariables)
			{
				for (int i = 0; i < inGameVariable.Value.Count; i++)
				{
					if (inGameVariable.Value.ElementAt(i).Value.Update())
					{
						BattleController.ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_VARIABLE_DESTRUCTION, inGameVariable.Key, inGameVariable.Value.Keys.ElementAt(i)));
						inGameVariable.Value.Remove(inGameVariable.Value.Keys.ElementAt(i));
						i--;
					}
				}
			}
		}

		public static void AddVariable(int ownerID, string variableName, object variableValue, int variableFrames = -1)
		{
			if (!InGameVariables.ContainsKey(ownerID))
			{
				InGameVariables.Add(ownerID, new Dictionary<string, LocalVariable>());
			}
			if (InGameVariables[ownerID].ContainsKey(variableName))
			{
				InGameVariables[ownerID][variableName].SetValue(variableValue);
				InGameVariables[ownerID][variableName].SetFrames(variableFrames);
				BattleLog.SetVariable(ownerID, variableName, variableValue, variableFrames);
			}
			else
			{
				InGameVariables[ownerID].Add(variableName, new LocalVariable(variableName, variableValue, variableFrames));
				BattleLog.AddVariable(ownerID, variableName, variableValue, variableFrames);
			}
			VariableChanged(ownerID, variableName);
		}

		private static void VariableChanged(int ownerId, string variableName)
		{
			if (!VariablesToCheck.ContainsKey(ownerId) || !VariablesToCheck[ownerId].ContainsKey(variableName))
			{
				return;
			}
			foreach (VariableChangedHandler item in VariablesToCheck[ownerId][variableName])
			{
				item(ownerId, variableName);
			}
		}

		public static void AddVariable(string variableName, object variableValue, int frames = -1)
		{
			AddVariable(-1, variableName, variableValue, frames);
		}

		public static void RemoveVariable(int ownerID, string variableName)
		{
			if (InGameVariables.ContainsKey(ownerID) && InGameVariables[ownerID].ContainsKey(variableName))
			{
				BattleLog.RemoveVariable(ownerID, variableName, InGameVariables[ownerID][variableName].value, InGameVariables[ownerID][variableName].frames);
				InGameVariables[ownerID].Remove(variableName);
			}
		}

		public static LocalVariable GetVariable(int ownerModel, string nameVal)
		{
			if (InGameVariables.ContainsKey(ownerModel) && InGameVariables[ownerModel].ContainsKey(nameVal))
			{
				return InGameVariables[ownerModel][nameVal];
			}
			return null;
		}

		public static LocalVariable GetVariable(string nameVal)
		{
			return GetVariable(-1, nameVal);
		}

		public static List<LocalVariable> GetVariablesByOwner(int ownerModel)
		{
			List<LocalVariable> list = new List<LocalVariable>();
			if (InGameVariables.ContainsKey(ownerModel))
			{
				foreach (KeyValuePair<string, LocalVariable> item in InGameVariables[ownerModel])
				{
					list.Add(item.Value);
				}
			}
			return list;
		}

		public static void Subscribe(int ownerID, string varName, VariableChangedHandler handler)
		{
			if (!VariablesToCheck.ContainsKey(ownerID))
			{
				VariablesToCheck.Add(ownerID, new Dictionary<string, List<VariableChangedHandler>>());
			}
			if (!VariablesToCheck[ownerID].ContainsKey(varName))
			{
				VariablesToCheck[ownerID].Add(varName, new List<VariableChangedHandler>());
			}
			VariablesToCheck[ownerID][varName].Add(handler);
		}

		public static void Unsubscribe(int ownerID, string varName, VariableChangedHandler handler)
		{
			if (VariablesToCheck.ContainsKey(ownerID) && VariablesToCheck[ownerID].ContainsKey(varName))
			{
				VariablesToCheck[ownerID][varName].RemoveAll((VariableChangedHandler toRemove) => toRemove.Equals(handler));
			}
		}
	}
}
