using Nekki.Yaml;
using SF3.Moves;
using SF3.Utils;

public class TriggerActionWait : TriggerActionQuest
{
	private readonly int _frames;

	private readonly bool _screenLock;

	public TriggerActionWait(Node yamlNode)
		: base(EActionType.WAIT, yamlNode, false)
	{
		TryGetInt(out _frames, "Frames", 0, string.Empty, null, false);
		TryGetBool(out _screenLock, "ScreenLock", false, string.Empty, null, false);
	}

	protected override void ApplyAction(object modelData)
	{
		if (_screenLock)
		{
			UIBlocker.Instance.Block();
		}
		BehaviourTimer.CreateFramesTimer(_frames, delegate
		{
			if (_screenLock)
			{
				UIBlocker.Instance.Unblock();
			}
			CloseAction();
		});
	}
}
