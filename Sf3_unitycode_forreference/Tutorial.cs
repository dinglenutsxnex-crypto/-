using System;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
	[Serializable]
	public class ColorSync
	{
		private enum SyncDirrections
		{
			Forward = 0,
			Backward = 1
		}

		private Color _currentColor;

		[SerializeField]
		public Color From;

		[SerializeField]
		public Color To;

		[SerializeField]
		public AnimationCurve Curve;

		[SerializeField]
		public float Duration;

		private float _currentTime;

		private SyncDirrections _dirrection;

		private List<UISprite> _sprites = new List<UISprite>();

		public void Update()
		{
			if (_sprites.Count == 0)
			{
				return;
			}
			_currentTime += Time.unscaledDeltaTime * (float)((_dirrection == SyncDirrections.Forward) ? 1 : (-1));
			if (_currentTime > Duration)
			{
				_currentTime = Duration;
				_dirrection = SyncDirrections.Backward;
			}
			if (_currentTime < 0f)
			{
				_currentTime = 0f;
				_dirrection = SyncDirrections.Forward;
			}
			_currentColor = Color.Lerp(From, To, Curve.Evaluate(_currentTime / Duration));
			foreach (UISprite sprite in _sprites)
			{
				sprite.color = _currentColor;
			}
		}

		public void Add(List<UISprite> uiSprites)
		{
			foreach (UISprite uiSprite in uiSprites)
			{
				if (!_sprites.Contains(uiSprite))
				{
					_sprites.Add(uiSprite);
				}
			}
		}
	}

	public ColorSync Sync;

	private static int _activeCount;

	public static Tutorial Instance { get; private set; }

	public static bool Active
	{
		get
		{
			return _activeCount > 0;
		}
	}

	public static void SetTutor(EQuadrants[] quadrants, bool active)
	{
		Instance.Sync.Add(ActionButtons.Instance.SetTutor(quadrants, active));
		Stick.Instance.SetTutor(quadrants, active);
		if (active)
		{
			_activeCount += quadrants.Length;
		}
		else
		{
			_activeCount = ((_activeCount > quadrants.Length) ? (_activeCount - quadrants.Length) : 0);
		}
	}

	public static void Reset()
	{
		if (ActionButtons.Instance != null && Stick.Instance != null)
		{
			ActionButtons.Instance.SetTutor(null);
			Stick.Instance.SetTutor(null);
			_activeCount = 0;
		}
	}

	private void Start()
	{
		Instance = this;
	}

	private void Update()
	{
		if (Active && Sync != null)
		{
			Sync.Update();
		}
	}
}
