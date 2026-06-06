using Nekki.Yaml;
using SF3;
using SF3.Moves;
using UnityEngine;

public class TriggerActionCenterCameraToMapPoint : TriggerActionQuest
{
	private readonly Vector2 _position;

	private readonly bool _instantFocus;

	public TriggerActionCenterCameraToMapPoint(Node yamlNode)
		: base(EActionType.CENTER_CAMERA_TO_MAP_POINT, yamlNode, false)
	{
		TryGetVector2(out _position, "Position", string.Empty, this);
		TryGetBool(out _instantFocus, "InstantFocus", false, string.Empty, null, false);
	}

	protected override void ApplyAction(object modelData)
	{
		MapController.Instance.GoToCamera(_position, _instantFocus, base.CloseAction);
	}
}
