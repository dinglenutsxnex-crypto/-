using System;
using System.Collections.Generic;
using SF3.GameModels;

namespace SF3.Moves
{
	public class TriggerActionController
	{
		private Dictionary<TriggerAction.EActionType, Dictionary<int, Action<ITriggerAction>>> actionFunctions;

		public TriggerActionController()
		{
			actionFunctions = new Dictionary<TriggerAction.EActionType, Dictionary<int, Action<ITriggerAction>>>();
		}

		public void AddCallback(TriggerAction.EActionType type, int modelId, Action<ITriggerAction> action)
		{
			if (!actionFunctions.ContainsKey(type))
			{
				actionFunctions[type] = new Dictionary<int, Action<ITriggerAction>>();
			}
			if (!actionFunctions[type].ContainsKey(modelId))
			{
				actionFunctions[type][modelId] = null;
			}
			actionFunctions[type][modelId] = action;
		}

		public void ApplyAction(TriggerAction action, object modelData)
		{
			if (!actionFunctions.ContainsKey(action.type))
			{
				return;
			}
			if (action.targetType == EPlayerType.Both)
			{
				if (actionFunctions[action.type].ContainsKey(1) && actionFunctions[action.type][1] != null)
				{
					actionFunctions[action.type][1](action);
				}
				if (actionFunctions[action.type].ContainsKey(2) && actionFunctions[action.type][2] != null)
				{
					actionFunctions[action.type][2](action);
				}
			}
			else if (modelData != null)
			{
				Model model = (Model)modelData;
				int key = -1;
				switch (action.targetType)
				{
				case EPlayerType.This:
					key = model.id;
					break;
				case EPlayerType.Enemy:
					key = model.enemy.id;
					break;
				case EPlayerType.Parent:
					key = model.parentModel.id;
					break;
				}
				if (actionFunctions[action.type].ContainsKey(key) && actionFunctions[action.type][key] != null)
				{
					actionFunctions[action.type][key](action);
				}
			}
		}

		public void ClearActionsCallbacks()
		{
			actionFunctions.Clear();
		}
	}
}
