public class UIButtonNotInteractive : UIButton
{
	public override void SetState(State state, bool immediate)
	{
		if (!mInitDone)
		{
			mInitDone = true;
			OnInit();
		}
		if (mState != state)
		{
			mState = state;
		}
	}
}
