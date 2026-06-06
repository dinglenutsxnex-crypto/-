using Nekki.Yaml;
using SF3.Moves;

public class TriggerEventCall : TriggerEvent
{
	public string Id;

	public TriggerEventCall(Mapping eventMap)
		: base(ETriggerEvents.QEVENT_CALL, eventMap)
	{
		if (eventMap != null)
		{
			YamlUtils.TryGetString(out Id, eventMap, "ID", string.Empty);
		}
	}

	protected override bool Equal()
	{
		if (arguments.Length > 0 && arguments[0] != null)
		{
			string value = arguments[0].ToString();
			return Id.Equals(value);
		}
		return false;
	}

	public override string ToString()
	{
		return string.Concat("TriggerEventCall [type: ", base.type, " name: ", name, " ", Id, "]");
	}
}
