using SF3.GameModels;
using UnityEngine;

namespace SF3.Effects
{
	public class SpriteAnimationEffect : GameEffectBase
	{
		private Animator animator;

		private bool checkToDisable;

		public AnimationClip animationClip;

		public float playTime;

		private float timer;

		public string stateName;

		private Vector3 worldPos;

		private Vector3 curPos;

		protected override void Awake()
		{
			base.Awake();
			animator = GetComponent<Animator>();
		}

		public override void Play(Model model, Vector3 atPos)
		{
			worldPos = atPos;
			FixPosition();
			base.Play(model, curPos);
			animator.Play(stateName);
			playTime = animationClip.length;
			checkToDisable = true;
			timer = 0f;
		}

		private void FixPosition()
		{
			curPos = Camera.main.WorldToViewportPoint(worldPos);
			curPos = UICamera.currentCamera.ViewportToWorldPoint(curPos);
			curPos.z = 1f;
		}

		private void Update()
		{
			if (checkToDisable)
			{
				FixPosition();
				transf.position = curPos;
				timer += GameTimeController.deltaTime;
				if (timer >= playTime)
				{
					checkToDisable = false;
					obj.SetActive(false);
				}
			}
		}
	}
}
