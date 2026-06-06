public interface IAnimatedExpBar
{
	long CurrentLevel { get; set; }

	long FromExp { get; set; }

	long AddedExp { get; set; }

	event ExpBarAnimationEnd onAnimationEnd;

	void AnimateExp();

	void BreakAnimation();

	void Hide();

	void Show();
}
