using System;
using System.Collections.Generic;

[Serializable]
public class AnimationTransition
{
	public int frameShift { get; private set; }

	public List<string> animations { get; private set; }

	public AnimationTransition(int frameShiftValue = 0)
	{
		frameShift = frameShiftValue;
		animations = new List<string>();
	}

	public void SetFrameShift(int value)
	{
		frameShift = value;
	}

	public void AddAnimation(string value)
	{
		animations.Add(value);
	}

	public void AddAnimations(string[] value)
	{
		animations.AddRange(value);
	}

	public bool Equal(string lastAnimation)
	{
		foreach (string animation in animations)
		{
			if (animation.Equals(lastAnimation))
			{
				return true;
			}
		}
		return false;
	}
}
