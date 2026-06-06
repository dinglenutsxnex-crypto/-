using System.Collections.Generic;

public interface ICardAnimation
{
	event CardAnimationEnd onAnimationEnd;

	void Init(RewardDataProvider rewardDataProvider, List<IReelItemAnimation> reelItemAnimations, RewardInfo rewardInfo);

	void Animate();

	void FadeIn();

	void Break();

	void AnimateSelectCard(ReelItem item);
}
