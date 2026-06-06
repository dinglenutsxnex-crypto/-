using UnityEngine;

[RequireComponent(typeof(UISpriteAnimation))]
public class UISpriteAnimationDelay : MonoBehaviour
{
	public float delay = 1f;

	private UISpriteAnimation _anim;

	private float _finish;

	private bool _animationComplited;

	private void Start()
	{
		_anim = GetComponent<UISpriteAnimation>();
		if (_anim != null)
		{
			_anim.loop = false;
		}
		_animationComplited = false;
	}

	private void Update()
	{
		if (_anim != null && !_anim.isPlaying)
		{
			float time = Time.time;
			if (!_animationComplited)
			{
				_finish = time;
				_animationComplited = true;
			}
			if (time - _finish >= delay)
			{
				_anim.Play();
				_animationComplited = false;
			}
		}
	}
}
