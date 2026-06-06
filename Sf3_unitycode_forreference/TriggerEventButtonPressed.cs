using Nekki.Yaml;
using SF3.Moves;

public class TriggerEventButtonPressed : TriggerEvent
{
	private readonly string _id;

	public TriggerEventButtonPressed(Mapping eventMap)
		: base(ETriggerEvents.EVENT_TUTORIAL_BUTTON_PRESSED, eventMap)
	{
		name = name.ToLower();
		TryGetString(out _id, "ID", null, string.Empty, this);
	}

	protected override bool Equal()
	{
		return arguments[0].Equals(_id);
	}
}
