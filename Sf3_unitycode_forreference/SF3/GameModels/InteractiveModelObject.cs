using System.Collections;
using System.Collections.Generic;
using SF3.Effects;
using SF3.Moves;
using SF3.Utils;
using UnityEngine;

namespace SF3.GameModels
{
	public class InteractiveModelObject : MonoBehaviour, IShadowFormModel
	{
		private bool _shadowFormActive;

		private BehaviourTimer.SingleTimer _shadowFormBlendTimer;

		private float _kinematicApplyTimer;

		public static List<InteractiveModelObject> droppedInteractiveObjects { get; private set; }

		public ModelObject modelObject { get; private set; }

		static InteractiveModelObject()
		{
			droppedInteractiveObjects = new List<InteractiveModelObject>();
		}

		public void SetModelObject(ModelObject _modelObject, bool shadowFormActive)
		{
			modelObject = _modelObject;
			_shadowFormActive = shadowFormActive;
			droppedInteractiveObjects.Add(this);
			_kinematicApplyTimer = -1f;
			if (!EffectsManager.freezeFrameActive)
			{
				return;
			}
			foreach (SkeletonObject skeleton in modelObject.skeletons)
			{
				skeleton.skeletonRagdoll.SetRagdollSleepState(true, 0);
			}
		}

		private void Update()
		{
			if (modelObject == null || _kinematicApplyTimer == -1f || !(_kinematicApplyTimer < GameTimeController.battleTime))
			{
				return;
			}
			_kinematicApplyTimer = -1f;
			foreach (SkeletonObject skeleton in modelObject.skeletons)
			{
				skeleton.skeletonRagdoll.SetActive(false);
				skeleton.skeletonRagdoll.FreezeObject(true);
			}
		}

		public static void Reset()
		{
			if (droppedInteractiveObjects.Count <= 0)
			{
				return;
			}
			foreach (InteractiveModelObject droppedInteractiveObject in droppedInteractiveObjects)
			{
				if (!(droppedInteractiveObject == null))
				{
					droppedInteractiveObject.modelObject.DestroyItem();
					Object.DestroyImmediate(droppedInteractiveObject.gameObject);
				}
			}
			droppedInteractiveObjects.Clear();
		}

		public void OnCollision(SkeletonObject skeletonObject, Collision collision)
		{
			if (FloorController.Instance.IsFloor(collision.gameObject))
			{
				string materialName = modelObject.modelMaterials.modelMaterials[modelObject.skeletons.IndexOf(skeletonObject)].materialName;
				BattleController.ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_ITEM_FLOOR_HIT, 1, new StrikeData
				{
					attackingEdgeMat = materialName
				}));
				skeletonObject.OnCollision -= OnCollision;
				if (_kinematicApplyTimer == -1f)
				{
					_kinematicApplyTimer = GameTimeController.battleTime + 120f;
				}
			}
		}

		public void ActivateShadowForm(bool instant = false)
		{
			if (_shadowFormBlendTimer != null)
			{
				_shadowFormBlendTimer.Stop();
			}
			if (modelObject != null)
			{
				_shadowFormBlendTimer = BehaviourTimer.CreateGameFramesTimer(45, UpdateShadowFormBlend, null, delegate
				{
					_shadowFormBlendTimer = null;
					modelObject.ChangeToNonDissolveShadowForm();
				});
			}
		}

		public void DisableShadowForm()
		{
			if (_shadowFormBlendTimer != null)
			{
				_shadowFormBlendTimer.Stop();
			}
			_shadowFormBlendTimer = BehaviourTimer.CreateGameFramesTimer(45, delegate(float progress)
			{
				UpdateShadowFormBlend(1f - progress);
			}, null, delegate
			{
				if (!ModelsManager.Instance.Player.modelShadowForm.shadowFormActive && !ModelsManager.Instance.Enemy.modelShadowForm.shadowFormActive)
				{
					GlowEffectController.instance.DisableGlow();
				}
				modelObject.ReturnDefaultMaterial();
				_shadowFormBlendTimer = null;
			});
		}

		private void UpdateShadowFormBlend(float progress)
		{
			modelObject.UpdateShadowFormBlend(progress);
		}

		public bool GetShadowFormActive()
		{
			return _shadowFormActive;
		}

		public static void HideAll()
		{
			foreach (InteractiveModelObject droppedInteractiveObject in droppedInteractiveObjects)
			{
				Routiner.Go(AnimateTransperentCorutine(droppedInteractiveObject.modelObject, 1f, 0f, 0.5f));
			}
		}

		private static IEnumerator AnimateTransperentCorutine(ModelObject m, float from, float to, float duration)
		{
			float startTime = Time.time;
			float endTime = startTime + duration;
			while (Time.time <= endTime)
			{
				float timePassed = Time.time - startTime;
				float currentAlpha = Mathf.Lerp(from, to, timePassed / duration);
				m.SetTransparent(currentAlpha);
				yield return new WaitForEndOfFrame();
			}
			m.SetTransparent(to);
			if (to > from && from > 0.999f)
			{
				m.SetOpaque();
			}
		}

		public void UpdateShadowForm()
		{
		}
	}
}
