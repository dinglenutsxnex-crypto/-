using Nekki.Yaml;
using SF3.Moves;

public class TriggerEventDragEnd : TriggerEvent
{
	private readonly string _draggableId;

	private readonly string _targetId;

	public TriggerEventDragEnd(Mapping eventMap)
		: base(ETriggerEvents.EVENT_TUTORIAL_DRAG_END, eventMap)
	{
		name = name.ToLower();
		TryGetString(out _draggableId, "DraggableID", null, string.Empty, this);
		TryGetString(out _targetId, "TargetID", null, string.Empty, this);
	}

	protected override bool Equal()
	{
		return arguments[0].Equals(_draggableId) && arguments[1].Equals(_targetId);
	}
}
