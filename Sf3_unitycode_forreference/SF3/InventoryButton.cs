using SF3.GameModels;
using UnityEngine;

namespace SF3
{
	public class InventoryButton : MonoBehaviour
	{
		public Vector3 buttonOffset;

		public float timeToShow = 2f;

		private float timer;

		private bool becomingVisible;

		private bool visible;

		private Transform transf;

		private Transform playerPivot;

		private BoxCollider boxCollider;

		private UISprite sprite;

		private Model modelState;

		private const string IDLE_ANIM = "Stance_Idle";

		private Vector3 offset;

		private Vector3 boneOffset;

		private void Awake()
		{
			transf = base.transform;
			boxCollider = GetComponent<BoxCollider>();
			sprite = GetComponent<UISprite>();
			Show(false);
		}

		private void Show(bool show)
		{
			visible = show;
			boxCollider.enabled = show;
			sprite.enabled = show;
		}

		private void SetToPos()
		{
			offset = Camera.main.WorldToScreenPoint(playerPivot.transform.position);
			if (UICamera.currentCamera != null)
			{
				boneOffset = UICamera.currentCamera.ScreenToWorldPoint(offset);
				boneOffset.z = transf.position.z;
				transf.position = boneOffset + buttonOffset;
			}
		}

		private void Update()
		{
			if (playerPivot == null)
			{
				modelState = ModelsManager.Instance.Player;
				if (modelState != null)
				{
					playerPivot = modelState.modelComponents.rootBone.transform;
				}
			}
			else if (!visible)
			{
				if (modelState.enemy.IsAI)
				{
					return;
				}
				if (becomingVisible)
				{
					if (modelState.animationInfo.animation.name != "Stance_Idle")
					{
						becomingVisible = false;
						return;
					}
					timer += GameTimeController.deltaTime;
					if (timer >= timeToShow)
					{
						SetToPos();
						Show(true);
						becomingVisible = false;
					}
				}
				else if (modelState.animationInfo != null && modelState.animationInfo.animation.name == "Stance_Idle")
				{
					becomingVisible = true;
					timer = 0f;
				}
			}
			else
			{
				SetToPos();
				if (modelState.animationInfo.animation.name != "Stance_Idle")
				{
					Show(false);
				}
			}
		}
	}
}
