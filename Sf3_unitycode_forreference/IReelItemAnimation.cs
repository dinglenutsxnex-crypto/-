using UnityEngine;

public interface IReelItemAnimation
{
	ReelItem item { get; }

	event ReelItemAnimationEnd onAnimationEnd;

	void Animate(string name, Vector3 moveTo);

	void Animate(string name, Vector3 moveTo, Vector3 rotateTo);

	void Animate(string name, Vector3 moveTo, Vector3 rotateTo, Vector3 scaleTo);

	void Stop();
}
