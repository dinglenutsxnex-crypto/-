using Nekki.Yaml;
using SF3.GameModels;
using SF3.Settings;
using UnityEngine;

namespace SF3.Moves
{
	public class TriggerActionWallReaction : TriggerAction
	{
		public enum EWallReaction
		{
			STOP = 0,
			REPULSE = 1
		}

		private readonly EWallReaction _wallReaction;

		public EWallReaction WallReaction
		{
			get
			{
				return _wallReaction;
			}
		}

		public TriggerActionWallReaction(Node yamNode)
			: base(EActionType.WALL_REACTION, yamNode)
		{
			Scalar text = ((Mapping)yamNode).GetMapping("WallReaction").GetText("Type");
			if (text != null)
			{
				SF3Utils.TryParseEnum(out _wallReaction, text.value.ToString(), EWallReaction.REPULSE);
			}
		}

		protected override void ApplyAction(object modelData)
		{
			switch (WallReaction)
			{
			case EWallReaction.REPULSE:
				ApplyRecoilOfWall((Model)modelData);
				break;
			case EWallReaction.STOP:
				StopByWall((Model)modelData);
				break;
			}
		}

		private void StopByWall(Model model)
		{
			model.modelAnimation.StopForced();
			model.modelComponents.ClearAttackingCapsules();
			model.modelComponents.rootBone.SetPosition(model.CurrentWallCollider.ClosestPointOnBounds(model.modelComponents.rootBone.previousPosition + new Vector3(0f, Random.Range(-5f, 5f), Random.Range(-5f, 5f))), false);
		}

		private void ApplyRecoilOfWall(Model model)
		{
			model.modelComponents.modelPhysics.SetRagdollActive(true);
			model.modelComponents.modelPhysics.SetIsTriggerActive(false);
			Vector3 impulse = (model.modelComponents.rootBone.position - model.modelComponents.rootBone.previousPosition).normalized * FightSettings.wallRepulsionCoefficient;
			impulse.x *= -1f;
			model.modelComponents.modelPhysics.AddForce(impulse, model.modelComponents.rootBone.boneName);
		}
	}
}
