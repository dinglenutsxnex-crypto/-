using System.Collections.Generic;
using SF3;
using SF3.GameModels;
using SF3.Moves;

public interface ITriggerAction
{
	TriggerAction.EActionType type { get; }

	string name { get; }

	EPlayerType targetType { get; }

	void Apply(Model modelState = null, List<ITriggerAction> actions = null);
}
