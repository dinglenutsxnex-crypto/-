using Nekki.Yaml;
using SF3.GameModels;
using SF3.Moves;

public class TriggerActionShowStatusIcon : TriggerAction
{
	private readonly string _icon;

	private readonly RpnValue<int> _duration;

	private readonly RpnValue<bool> _infinite;

	private readonly RpnValue<bool> _showExpiration;

	private readonly RpnValue<bool> _stackable;

	private readonly string _color;

	public string Icon
	{
		get
		{
			return _icon;
		}
	}

	public int Duration
	{
		get
		{
			return _duration;
		}
	}

	public bool Infinite
	{
		get
		{
			return _infinite;
		}
	}

	public bool ShowExpiration
	{
		get
		{
			return _showExpiration;
		}
	}

	public bool Stackable
	{
		get
		{
			return _stackable;
		}
	}

	public string Color
	{
		get
		{
			return _color;
		}
	}

	public TriggerActionShowStatusIcon(Node node)
		: base(EActionType.SHOW_STATUS_ICON, node)
	{
		if (node != null && !(node is Scalar))
		{
			TryGetString(out _icon, "Icon", string.Empty, string.Empty, this);
			if (!TryGetString(out _color, "BackgroundColor", string.Empty, string.Empty, null, false))
			{
				_color = "#FFFFFF";
			}
			string outResult;
			if (TryGetString(out outResult, "Frames", string.Empty, string.Empty, null, false))
			{
				_duration = outResult;
				_infinite = false;
			}
			else
			{
				_duration = int.MaxValue;
				_infinite = true;
			}
			if (TryGetString(out outResult, "ShowExpiration", string.Empty, string.Empty, null, false))
			{
				_showExpiration = outResult;
			}
			else
			{
				_showExpiration = false;
			}
			if (TryGetString(out outResult, "Stackable", string.Empty, string.Empty, null, false))
			{
				_stackable = outResult;
			}
			else
			{
				_stackable = true;
			}
		}
	}

	protected override void ApplyAction(object modelData)
	{
		PerkCarousel.ShowStatusIcon(((Model)modelData).id, this);
	}
}
