using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(UISprite))]
public class UISpriteAnimationAdvanced : UISpriteAnimation
{
	[HideInInspector]
	[SerializeField]
	protected int mFirstFrame;

	protected override void Update()
	{
		if (!mActive || mSpriteNames.Count <= 1 || !Application.isPlaying || mFPS <= 0)
		{
			return;
		}
		mDelta += RealTime.deltaTime;
		float num = 1f / (float)mFPS;
		if (!(num < mDelta))
		{
			return;
		}
		mDelta = 0f;
		if (++mIndex >= mSpriteNames.Count)
		{
			mIndex = mFirstFrame;
			mActive = mLoop;
		}
		if (mActive)
		{
			mSprite.spriteName = mSpriteNames[mIndex];
			if (mSnap)
			{
				mSprite.MakePixelPerfect();
			}
		}
	}
}
